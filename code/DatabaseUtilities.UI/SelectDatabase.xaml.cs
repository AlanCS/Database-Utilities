using DatabaseUtilities.Core;
using DatabaseUtilities.DAL;
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
using System.Windows.Shapes;

namespace DatabaseUtilities.UI
{
    /// <summary>
    /// Interaction logic for SelectDatabase.xaml
    /// </summary>
    public partial class SelectDatabase : Window
    {

        LogManagement logManagent = new LogManagement();

        public SelectDatabase()
        {
            InitializeComponent();
            this.Loaded += SelectDatabase_Loaded;
            this.Closing += SelectDatabase_Closing;
        }

        void SelectDatabase_Loaded(object sender, RoutedEventArgs e)
        {
            tree.ItemsSource = GetDatabasesTree();
        }

        void SelectDatabase_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UpdateSearchedDatabases();

            logManagent.Write("Global.SearchDBText", Global.SearchDBText);
            var DatabaseServerIds = string.Join(",", Global.CurrentSearchableDatabases.Select(c => c.DatabaseServerId.ToString()).ToArray());
            logManagent.Write("Global.CurrentSearchableDatabases", DatabaseServerIds);

            this.DialogResult = HasChangedAnything;
        }

        bool HasChangedAnything = false;

        public List<Models.BaseTreeNode> GetDatabasesTree()
        {
            var environments = Global.Snapshot.Servers
                .GroupBy(c => c.Environment)
                .Select(c => new Models.Environment(c.Key, c))
                .OrderBy(c=> c.Name)
                .ToArray();

            var root = new List<Models.BaseTreeNode>();
            root.Add(new Models.GenericNode("All Environments", environments));
            root[0].Initialize();

            var databaseNodes = root[0].GetNodesOfType<Models.DatabaseNode>();
            foreach (var item in databaseNodes.Where(c => Global.CurrentSearchableDatabases.Contains(c.Database)))
                item.IsChecked = true;

            var serverNodes = root[0].GetNodesOfType<Models.ServerNode>();
            foreach (var item in serverNodes)
                item.IsExpanded = false;

            Models.BaseTreeNode.OnChecked += (e) =>
            {
                HasChangedAnything = true;
            };

            return root;
        }

        public void UpdateSearchedDatabases()
        {
            var rootNodes = tree.ItemsSource as List<Models.BaseTreeNode>;

            var checkedDatabases = rootNodes[0].GetNodesOfType<Models.DatabaseNode>().Where(c => c.IsChecked ?? false == true).Select(c => c.Database).ToArray();

            Global.CurrentSearchableDatabases = checkedDatabases;

            Global.SearchDBText = string.Join(", ", GetSearchedDatabaseNames());
        }

        public List<string> GetSearchedDatabaseNames()
        {
            var rootNodes = tree.ItemsSource as List<Models.BaseTreeNode>;

            return GetSearchedDatabaseNames(rootNodes);
        }

        private List<string> GetSearchedDatabaseNames(IEnumerable<Models.BaseTreeNode> nodes)
        {
            var checkedDatabases = new List<String>();

            foreach (var node in nodes)
            {
                if (node.IsChecked ?? false == true)
                {
                    checkedDatabases.Add(((Models.INode)node).Name);
                    continue;
                }

                if (!node.IsChecked.HasValue)
                {
                    if (node.Children.Count > 0)
                        checkedDatabases.AddRange(GetSearchedDatabaseNames(node.Children));
                }
            }

            return checkedDatabases;
        }

    }
}
