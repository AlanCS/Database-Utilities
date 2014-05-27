using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using DatabaseUtilities.DAL;
using DatabaseUtilities.Core;
using System.ComponentModel;
using DatabaseUtilities.UI.Models;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Text.RegularExpressions;
using Xceed.Wpf.Toolkit;
using System.IO;
using System.Data.SqlClient;

namespace DatabaseUtilities.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow2 : Window
    {


        LogManagement logManagent = new LogManagement();

        public MainWindow2()
        {
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
            this.Closed += MainWindow2_Closed;
            InitializeComponent();
        }

        ObservableCollection<Models.GroupingColumn> ColumnsToGroup = new ObservableCollection<GroupingColumn>();
        ObservableCollection<Models.GroupingColumn> ColumnsGrouped = new ObservableCollection<GroupingColumn>();

        ObservableCollection<DatabaseObjectWithParents> searchResults = new ObservableCollection<DatabaseObjectWithParents>();
        ObservableCollection<DatabaseObjectWithParents> tableRelatedObjects = new ObservableCollection<DatabaseObjectWithParents>();
        ObservableCollection<DatabaseObjectWithParents> storedProceduresRelatedObjects = new ObservableCollection<DatabaseObjectWithParents>();
        ObservableCollection<DatabaseObjectWithParents> viewRelatedObjects = new ObservableCollection<DatabaseObjectWithParents>();

        private void LoadSnapshot(bool ForceRefresh = false)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                Menu_RefreshSnapshot.IsEnabled = false;
                RefreshingSnapshot.Visibility = System.Windows.Visibility.Visible;
            }));

            Global.Snapshot = new DatabaseService().GetCachedSnapshot(ForceRefresh);
            Global.SearchDBText = logManagent.Read("Global.SearchDBText");

            var DatabaseServerIds = logManagent.Read("Global.CurrentSearchableDatabases").Split(new char[] { ',' },  StringSplitOptions.RemoveEmptyEntries).Select(c => Convert.ToInt64(c)).ToArray();
            Global.CurrentSearchableDatabases = Global.Snapshot.Servers.SelectMany(c => c.Databases).Where(c => DatabaseServerIds.Contains(c.DatabaseServerId)).ToArray();

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                updateLabels();
            }));

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                Menu_RefreshSnapshot.IsEnabled = true;
                RefreshingSnapshot.Visibility = System.Windows.Visibility.Collapsed;
            }));

            if (HasSearched())
                DoSearch();
        }

        private bool HasSearched()
        {
            bool hasSearched = false;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                hasSearched = txtColumn.Text != string.Empty || txtName.Text != string.Empty;
            }));

            return hasSearched;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (logManagent.Read("left_column_width") != string.Empty)
                MainGrid.ColumnDefinitions[0].Width = new GridLength(Convert.ToDouble(logManagent.Read("left_column_width")), GridUnitType.Pixel);

            //ColumnsToGroup.Add(new GroupingColumn() { Name = "Database", Value = "Database" });
            //ColumnsToGroup.Add(new GroupingColumn() { Name = "Server", Value = "Server" });
            //ColumnsToGroup.Add(new GroupingColumn() { Name = "Environment", Value = "Environment" });
            //ColumnsToGroup.Add(new GroupingColumn() { Name = "Schema", Value = "Object.Schema" });

            //lstAvailableGroups.ItemsSource = ColumnsToGroup;
            //lstGroups.ItemsSource = ColumnsGrouped;

            GridSearchResult.ItemsSource = GetCollectionView(searchResults);
            GridTablesOut.ItemsSource = GetCollectionView(tableRelatedObjects);
            GridStoredProceduresOut.ItemsSource = GetCollectionView(storedProceduresRelatedObjects);
            GridViewOut.ItemsSource = GetCollectionView(viewRelatedObjects);

            var columnOrder = logManagent.Read("grid_columns_order").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(c => Convert.ToInt32(c)).ToArray();

            for (int j = 0; j < columnOrder.Length; j++) GridSearchResult.Columns[j].DisplayIndex = columnOrder[j];

            HistoryNameSearched.AddRange(logManagent.Read("search_name").Split(new char[] { ',' }, StringSplitOptions.None));
            HistoryColumnSearched.AddRange(logManagent.Read("search_column").Split(new char[] { ',' }, StringSplitOptions.None));

            if (HistoryNameSearched.Count > 0)
                txtName.Text = HistoryNameSearched.First();

            if (HistoryColumnSearched.Count > 0)
                txtColumn.Text = HistoryColumnSearched.First();

            System.Threading.ThreadPool.QueueUserWorkItem(delegate
            {
                LoadSnapshot(false);
            }, null);

            txtName.Focus();
        }

        private ICollectionView GetCollectionView(ObservableCollection<DatabaseObjectWithParents> list)
        {
            var result = CollectionViewSource.GetDefaultView(list);
            result.SortDescriptions.Add(new System.ComponentModel.SortDescription("Object.Schema", System.ComponentModel.ListSortDirection.Ascending));
            result.SortDescriptions.Add(new System.ComponentModel.SortDescription("Object.Name", System.ComponentModel.ListSortDirection.Ascending));
            result.GroupDescriptions.Add(new PropertyGroupDescription("ObjectType"));
            return result;
        }

        void MainWindow2_Closed(object sender, EventArgs e)
        {
            logManagent.WriteNow("last_checked_databases", string.Join(",", Global.CurrentSearchableDatabases.Select(c => c.DatabaseServerId)));

            var columnOrder = GridSearchResult.Columns.Select(c => c.DisplayIndex.ToString()).ToArray();
            logManagent.WriteNow("grid_columns_order", string.Join(",", columnOrder));

            logManagent.WriteNow("search_name", string.Join(",", HistoryNameSearched));
            logManagent.WriteNow("search_column", string.Join(",", HistoryColumnSearched));
        }

        private RelatedObjects GetRelatedObjects(DatabaseObjectWithColumns obj, DAL.Database database)
        {
            var result = new RelatedObjects();


            result.ObjectsDependingOnThis.AddRange(database.Tables.Where(c => c.ObjectsDependingOnThis.Contains(obj.Id)));
            result.ObjectsDependingOnThis.AddRange(database.StoredProcedures.Where(c => c.ObjectsDependingOnThis.Contains(obj.Id)));
            result.ObjectsDependingOnThis.AddRange(database.Views.Where(c => c.ObjectsDependingOnThis.Contains(obj.Id)));


            result.ObjectsThatThisDependsOn.AddRange(database.Tables.Where(c => c.ObjectsThatThisDependsOn.Contains(obj.Id)));
            result.ObjectsThatThisDependsOn.AddRange(database.StoredProcedures.Where(c => c.ObjectsThatThisDependsOn.Contains(obj.Id)));
            result.ObjectsThatThisDependsOn.AddRange(database.Views.Where(c => c.ObjectsThatThisDependsOn.Contains(obj.Id)));

            return result;
        }



        private void DoSearch()
        {
            var searchStart = DateTime.Now;

            string nameFilter = string.Empty;
            string columnFilter = string.Empty;

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                GridLoading.Visibility = System.Windows.Visibility.Visible;
                searchResults.Clear();
                nameFilter = txtName.Text;
                columnFilter = txtColumn.Text;
            }));

            Regex regexName = null;
            Regex regexColumn = null;

            IEnumerable<DatabaseObjectWithColumns> tables;
            IEnumerable<DatabaseObjectWithColumns> storedProcedures;
            IEnumerable<DatabaseObjectWithColumns> views;

            bool IsRegex = true;

            try
            {
                regexName = new Regex(nameFilter, RegexOptions.IgnoreCase);
                regexColumn = new Regex(columnFilter, RegexOptions.IgnoreCase);
            }
            catch (ArgumentException ex)
            {
                IsRegex = false;
            }

            if (!IsRegex)
            {
                tables = Global.CurrentSearchableDatabases.SelectMany(c => c.Tables).Where(c => c.SchemaAndName.ContainsCaseInsensitive(nameFilter));
                if (columnFilter != string.Empty) tables = tables.Where(c => c.Columns.Any(d => d.Name.ContainsCaseInsensitive(columnFilter)));

                storedProcedures = Global.CurrentSearchableDatabases.SelectMany(c => c.StoredProcedures).Where(c => c.SchemaAndName.ContainsCaseInsensitive(nameFilter));
                if (columnFilter != string.Empty) storedProcedures = storedProcedures.Where(c => c.Columns.Any(d => d.Name.ContainsCaseInsensitive(columnFilter)));

                views = Global.CurrentSearchableDatabases.SelectMany(c => c.Views).Where(c => c.SchemaAndName.ContainsCaseInsensitive(nameFilter));
                if (columnFilter != string.Empty) views = views.Where(c => c.Columns.Any(d => d.Name.ContainsCaseInsensitive(columnFilter)));
            }
            else
            {
                tables = Global.CurrentSearchableDatabases.SelectMany(c => c.Tables).Where(c => regexName.IsMatch(c.SchemaAndName));
                if (columnFilter != string.Empty) tables = tables.Where(c => c.Columns.Any(d => regexColumn.IsMatch(d.Name)));

                storedProcedures = Global.CurrentSearchableDatabases.SelectMany(c => c.StoredProcedures).Where(c => regexName.IsMatch(c.SchemaAndName));
                if (columnFilter != string.Empty) storedProcedures = storedProcedures.Where(c => c.Columns.Any(d => regexColumn.IsMatch(d.Name)));

                views = Global.CurrentSearchableDatabases.SelectMany(c => c.Views).Where(c => regexName.IsMatch(c.SchemaAndName));
                if (columnFilter != string.Empty) views = views.Where(c => c.Columns.Any(d => regexColumn.IsMatch(d.Name)));
            }

            var allItems = new List<DatabaseObjectWithColumns>();
            allItems.AddRange(tables);
            allItems.AddRange(storedProcedures);
            allItems.AddRange(views);

            System.Diagnostics.Debug.WriteLine((IsRegex ? "Plain" : "Regex") + " search for name: {0} and column: {1}", nameFilter, columnFilter);

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {

                foreach (var item in allItems.Select(c => new DatabaseObjectWithParents(c))) searchResults.Add(item);

            }));

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {

                GridLoading.Visibility = System.Windows.Visibility.Collapsed;
                txtLastSearch.Text = string.Format("{0}s ({1})", Math.Round(DateTime.Now.Subtract(searchStart).TotalSeconds, 1), IsRegex ? "Regex" : "Plain");

            }), DispatcherPriority.ContextIdle, null);
        }





        private void Menu_About_Click(object sender, RoutedEventArgs e)
        {
            var window = new About();
            window.ShowDialog();
            window.Close();
        }

        private void Menu_License_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://raw.github.com/AlanCS/Database-Utilities/master/license.txt");
        }

        private void btnOpenSQL_Click(object sender, RoutedEventArgs e)
        {
            var file = System.IO.Path.Combine(System.Environment.CurrentDirectory, "settings") + @"\query.sql";

            if (LastSelectedObject.Object != null)
            {
                if (LastSelectedObject.Object is DatabaseUtilities.DAL.StoredProcedure)
                {
                    File.WriteAllText(file, new SqlServerCore().GenerateCodeForStoredProcedureCall(LastSelectedStoredProcedure));
                }
                else
                {
                    if (string.IsNullOrEmpty(txtTable2.SelectedText))
                        File.WriteAllText(file, txtTable2.Text);
                    else
                        File.WriteAllText(file, txtTable2.SelectedText);
                }
            }

            OpenSQLServer(file);

        }


        DatabaseObjectWithParents LastSelectedObject = null;
        DAL.Table LastSelectedTable = null;
        DAL.StoredProcedure LastSelectedStoredProcedure = null;
        DAL.View LastSelectedView = null;
        DAL.Database LastSelectedDatabase = null;
        DAL.Server LastSelectedServer = null;

        private void DisplayLastSelectedObject()
        {
            LastSelectedDatabase = Global.Snapshot.Servers.SelectMany(c => c.Databases).SingleOrDefault(c => c.DatabaseServerId == LastSelectedObject.DatabaseServerId);
            LastSelectedServer = Global.Snapshot.Servers.SingleOrDefault(c => c.Id == LastSelectedDatabase.ServerId);

            var relatedObjects = GetRelatedObjects(LastSelectedObject.Object, LastSelectedDatabase);

            int newTabIndex = 0;

            if (LastSelectedObject.Object is DAL.Table)
            {
                LastSelectedTable = LastSelectedObject.Object as DAL.Table;

                var code = new StringBuilder();

                code.AppendFormat("use {0}" + System.Environment.NewLine, LastSelectedObject.Database);
                code.AppendLine("go");
                code.AppendFormat("create table [{0}].[{1}] (" + System.Environment.NewLine, LastSelectedTable.Schema, LastSelectedTable.Name);
                code.Append(string.Join("," + System.Environment.NewLine, LastSelectedTable.Columns.Select(c => c.ToSQL())));
                code.AppendLine(System.Environment.NewLine + ")" + System.Environment.NewLine + "go");

                txtTable1.Text = code.ToString();

                btnSelect_Click(null, null);

                tableRelatedObjects.Clear();

                foreach (var item in relatedObjects.ObjectsThatThisDependsOn.Select(c => new DatabaseObjectWithParents(c))) tableRelatedObjects.Add(item);
                foreach (var item in relatedObjects.ObjectsDependingOnThis.Select(c => new DatabaseObjectWithParents(c))) tableRelatedObjects.Add(item);

                newTabIndex = 0;
            }

            if (LastSelectedObject.Object is DAL.StoredProcedure)
            {
                LastSelectedStoredProcedure = LastSelectedObject.Object as DAL.StoredProcedure;

                txtStoredProcedure1.Text = new SqlServerCore().GenerateCodeForStoredProcedure(LastSelectedStoredProcedure, LastSelectedObject.Database);

                storedProceduresRelatedObjects.Clear();
                foreach (var item in relatedObjects.ObjectsThatThisDependsOn.Select(c => new DatabaseObjectWithParents(c))) storedProceduresRelatedObjects.Add(item);
                foreach (var item in relatedObjects.ObjectsDependingOnThis.Select(c => new DatabaseObjectWithParents(c))) storedProceduresRelatedObjects.Add(item);

                txtStoredProcedure2.Text = new SqlServerCore().GenerateCSharpCodeForSP(LastSelectedStoredProcedure);

                newTabIndex = 1;
            }

            if (LastSelectedObject.Object is DAL.View)
            {
                LastSelectedView = LastSelectedObject.Object as DAL.View;

                txtView1.Text = LastSelectedView.Text;

                viewRelatedObjects.Clear();
                foreach (var item in relatedObjects.ObjectsThatThisDependsOn.Select(c => new DatabaseObjectWithParents(c))) viewRelatedObjects.Add(item);
                foreach (var item in relatedObjects.ObjectsDependingOnThis.Select(c => new DatabaseObjectWithParents(c))) viewRelatedObjects.Add(item);

                txtView2.Text = new SqlServerCore().GenerateSP_Select(LastSelectedView, LastSelectedObject.Database);

                newTabIndex = 2;
            }

            if (databaseTabs.SelectedIndex != newTabIndex)
            {
                // has to changed this way, as a double click inside a tab prevents the tab from being changed by just calling .SelectedIndex normally
                Application.Current.Dispatcher.BeginInvoke((Action)delegate
                {
                    databaseTabs.SelectedIndex = newTabIndex;
                }, DispatcherPriority.Render, null);
            }
        }

        private void GridSearchResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems == null || e.AddedItems.Count == 0) return;

            LastSelectedObject = e.AddedItems[0] as DatabaseObjectWithParents;

            if (LastSelectedObject == null) return;

            DisplayLastSelectedObject();
        }

        //private void AddGroupingColumn(object sender, RoutedEventArgs e)
        //{
        //    var column = lstAvailableGroups.SelectedItem as GroupingColumn;

        //    if (column == null) return;

        //    this.ColumnsGrouped.Add(column);
        //    this.ColumnsToGroup.Remove(column);
        //    collection.GroupDescriptions.Add(new PropertyGroupDescription(column.Value));
        //}

        private void GridSplitter_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {

            logManagent.Write("left_column_width", MainGrid.ColumnDefinitions[0].Width.Value.ToString());
        }

        private void OpenDatabases_Click(object sender, RoutedEventArgs e)
        {
            var window = new SelectDatabase();
            window.ShowDialog();

            var hasChangedAnything = window.DialogResult ?? false;

            if (!hasChangedAnything) return;

            updateLabels();
        }

        private void updateLabels()
        {
            txtSearchIn.Text = Global.SearchDBText;

            txtFooterSearchSummary.Text = string.Format("{0} databases ({1} tables, {2} SPs, {3} views)",
                Global.CurrentSearchableDatabases.Count(),
                Global.CurrentSearchableDatabases.Sum(c => c.Tables.Count),
                Global.CurrentSearchableDatabases.Sum(c => c.StoredProcedures.Count),
                Global.CurrentSearchableDatabases.Sum(c => c.Views.Count));

            if (!Global.Snapshot.SnapshotTaken.HasValue)
                txtFooterLastUpdatedSnapshot.Text = "?";
            else
                txtFooterLastUpdatedSnapshot.Text = Global.Snapshot.SnapshotTaken.Value.ToString("dd/MM/yy HH:mm:ss");
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            if (LastSelectedTable == null) return;
            txtTable2.Text = new SqlServerCore().GenerateSP_Select(LastSelectedTable, LastSelectedObject.Database);
        }

        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {

            if (LastSelectedTable == null) return;
            txtTable2.Text = new SqlServerCore().GenerateSP_UpdateInsert(LastSelectedTable, LastSelectedObject.Database);
        }

        private void txtSearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                btnSearch_Click(sender, e);
            }
        }


        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(delegate
            {
                DoSearch();
            }, null);

            AddToSearchHistory(ref HistoryNameSearched, txtName.Text);
            AddToSearchHistory(ref HistoryColumnSearched, txtColumn.Text);
        }

        private void AddToSearchHistory(ref List<string> history, string newSearch)
        {
            if (history.Contains(newSearch))
                history.Remove(newSearch);

            if (history.Count == 100)
                history.RemoveAt(99);

            history.Insert(0, newSearch);
        }

        private void Menu_RefreshSnapshot_Click(object sender, RoutedEventArgs e)
        {
            var msgBoxResult = System.Windows.Forms.MessageBox.Show("This might take a few minutes, and will be done in the background.\n Are you sure ?", "Confirm", System.Windows.Forms.MessageBoxButtons.OKCancel, System.Windows.Forms.MessageBoxIcon.Question);
            if (msgBoxResult != System.Windows.Forms.DialogResult.OK)
                return;

            System.Threading.ThreadPool.QueueUserWorkItem(delegate
            {
                LoadSnapshot(true);
            }, null);
        }

        private void GridRelatedObjects_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var grid = sender as DataGrid;

            if (grid == null) return;

            LastSelectedObject = grid.SelectedItem as DatabaseObjectWithParents;

            if (LastSelectedObject == null) return;

            DisplayLastSelectedObject();

        }


        private List<string> HistoryNameSearched = new List<string>();
        private List<string> HistoryColumnSearched = new List<string>();

        private void txtName_KeyUp(object sender, KeyEventArgs e)
        {
            if (!(e.Key == Key.Up || e.Key == Key.Down)) return;

            ManageKeyUpOrDown(ref HistoryNameSearched, txtName, e.Key == Key.Up);
        }

        private void txtColumn_KeyUp(object sender, KeyEventArgs e)
        {
            if (!(e.Key == Key.Up || e.Key == Key.Down)) return;

            ManageKeyUpOrDown(ref HistoryColumnSearched, txtColumn, e.Key == Key.Up);
        }

        private void ManageKeyUpOrDown(ref List<string> history, WatermarkTextBox current, bool isUp)
        {
            var index = history.IndexOf(current.Text);

            if (isUp) index++;
            else index--;

            if (index < 0 || index >= history.Count) return;

            current.Text = history[index];
        }

        private void OpenSQLServer(string filePath = "")
        {
            if (LastSelectedDatabase == null)
            {
                Process.Start("ssms.exe");
                return;
            }

            var connection = new SqlConnectionStringBuilder(LastSelectedServer.ConnectionString);

            var parameter = string.Format("-S {0} -d {1}", connection.DataSource, LastSelectedDatabase.Name);
            if(filePath != string.Empty)
                parameter = string.Format(@"""{0}"" {1}", filePath, parameter);

            Process.Start("ssms.exe", parameter);
        }

        private void Menu_OpenSQLServer_Click(object sender, RoutedEventArgs e)
        {
            btnOpenSQL_Click(sender, e);
        }

        private void btnExecuteSP_Click(object sender, RoutedEventArgs e)
        {
             if (LastSelectedStoredProcedure == null) return;

            txtStoredProcedure2.Text = new SqlServerCore().GenerateCSharpCodeForSP(LastSelectedStoredProcedure, true);
        }

        private void Menu_Connections_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("connections.xml");
        }

    }



}
