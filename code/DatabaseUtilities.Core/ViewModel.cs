using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Diagnostics;

namespace DatabaseUtilities.Core
{
    public partial class ViewModel : INotifyPropertyChanged
    {
        LogManagement logManagent = new LogManagement();
        public SqlServerCore core = new SqlServerCore();


        public ViewModel()
        {
            foreach (var item in ConfigurationManager.ConnectionStrings.Cast<ConnectionStringSettings>())
                Connections.Add(item);
        }

        private ObservableCollection<System.Configuration.ConnectionStringSettings> _Connections = new ObservableCollection<System.Configuration.ConnectionStringSettings>();
        public ObservableCollection<System.Configuration.ConnectionStringSettings> Connections
        {
            get
            {
                return _Connections;
            }

            set
            {
                _Connections = value;
                RaisePropertyChanged("Connections");
            }
        }


        private System.Configuration.ConnectionStringSettings _SelectedConnection = null;
        public System.Configuration.ConnectionStringSettings SelectedConnection
        {
            get
            {
                return _SelectedConnection;
            }

            set
            {
                if (_SelectedConnection == value)
                    return;

                _SelectedConnection = value;

                RaisePropertyChanged("SelectedConnection");

                if (!core.ChangeConnection(value))
                    return;

                Databases.Clear();

                foreach (var item in core.GetDatabases())
                    Databases.Add(item);

                if (Databases.Count > 0)
                    SelectedDatabase = Databases[0];
            }
        }

        private ObservableCollection<string> _Errors = new ObservableCollection<string>();
        public ObservableCollection<string> Errors
        {
            get
            {
                return _Errors;
            }

            set
            {
                if (_Errors == value)
                    return;

                RaisePropertyChanged("Errors");
            }
        }

        private ObservableCollection<string> _Databases = new ObservableCollection<string>();
        public ObservableCollection<string> Databases
        {
            get
            {
                return _Databases;
            }

            set
            {
                if (_Databases == value)
                    return;

                RaisePropertyChanged("Databases");
            }
        }

        private ObservableCollection<string> _Tables = new ObservableCollection<string>();
        public ObservableCollection<string> Tables
        {
            get
            {
                return _Tables;
            }

            set
            {
                _Tables = value;
                RaisePropertyChanged("Tables");
            }
        }

        private ObservableCollection<string> _StoredProcedures = new ObservableCollection<string>();
        public ObservableCollection<string> StoredProcedures
        {
            get
            {
                return _StoredProcedures;
            }

            set
            {
                _StoredProcedures = value;
                RaisePropertyChanged("SPs");
            }
        }


        private string _SelectedDatabase = null;
        public string SelectedDatabase
        {
            get
            {
                return _SelectedDatabase;
            }

            set
            {
                if (_SelectedDatabase == value)
                    return;

                _SelectedDatabase = value;
                RaisePropertyChanged("SelectedDatabase");

                Tables.Clear();
                StoredProcedures.Clear();

                ClearFilters();

                if (!core.ChangeDatabase(this.SelectedConnection, value))
                    return;

                logManagent.Write("last_opened_database", String.Format("{0}_{1}", Connections.IndexOf(SelectedConnection), Databases.IndexOf(SelectedDatabase)));

                foreach (var item in core.GetTables())
                    Tables.Add(item);

                foreach (var item in core.GetStoredProcedures())
                    StoredProcedures.Add(item);

                if (Tables.Count > 0)
                    SelectedTable = Tables[0];
            }
        }

        private string _SelectedTable = null;
        public string SelectedTable
        {
            get
            {
                return _SelectedTable;
            }

            set
            {
                if (_SelectedTable == value)
                    return;

                _SelectedTable = value;

                RaisePropertyChanged("SelectedTable");

                GeneratedCode1 = GeneratedCode2 = string.Empty;

                if (string.IsNullOrEmpty(SelectedTable))
                    return;

                GeneratedCode1 = core.GenerateCodeCreateTable(SelectedTable);
                GenerateSP_Select();
            }
        }

        public void GenerateSP_Select()
        {
            GeneratedCode2 = core.GenerateSP_Select(SelectedTable);
        }

        private string _SelectedStoredProcedure = null;
        public string SelectedStoredProcedure
        {
            get
            {
                return _SelectedStoredProcedure;
            }

            set
            {
                if (_SelectedStoredProcedure == value)
                    return;

                _SelectedStoredProcedure = value;

                RaisePropertyChanged("SelectedStoredProcedure");

                GeneratedCode1 = GeneratedCode2 = string.Empty;

                if (string.IsNullOrEmpty(SelectedStoredProcedure))
                    return;

                core.GetStoredProcedureColumns(SelectedStoredProcedure);

                GeneratedCode1 = core.GenerateCodeForStoredProcedure(SelectedStoredProcedure);

                GetCSharpCodeForStoredProcedure();
            }
        }

        public void GetCSharpCodeForStoredProcedure(bool ForceExecution = false)
        {
            if (!string.IsNullOrEmpty(SelectedStoredProcedure))
                GeneratedCode2 = core.GenerateCSharpCodeForSP(SelectedStoredProcedure, CanExecuteAllStoredProcedures || ForceExecution);
        }



        public void GenerateSP_UpdateInsert()
        {
            GeneratedCode2 = core.GenerateSP_UpdateInsert(SelectedTable);
        }


        public void GenerateSP_Delete()
        {
            GeneratedCode2 = core.GenerateSP_Delete(SelectedTable);
        }


        private bool _CanExecuteAllStoredProcedures = false;
        public bool CanExecuteAllStoredProcedures
        {
            get
            {
                return _CanExecuteAllStoredProcedures;
            }

            set
            {
                if (_CanExecuteAllStoredProcedures == value)
                    return;

                _CanExecuteAllStoredProcedures = value;

                RaisePropertyChanged("CanExecuteAllStoredProcedures");
                RaisePropertyChanged("ShowButtonExecute");

                GetCSharpCodeForStoredProcedure();
            }
        }

        public bool ShowButtonExecute
        {
            get
            {
                return !CanExecuteAllStoredProcedures;
            }
        }

        private string _GeneratedCode1 = string.Empty;
        public string GeneratedCode1
        {
            get
            {
                return _GeneratedCode1;
            }

            set
            {
                if (_GeneratedCode1 == value)
                    return;

                _GeneratedCode1 = value;

                RaisePropertyChanged("GeneratedCode1");
            }
        }



        private string _GeneratedCode2 = string.Empty;
        public string GeneratedCode2
        {
            get
            {
                return _GeneratedCode2;
            }

            set
            {
                if (_GeneratedCode2 == value)
                    return;

                _GeneratedCode2 = value;

                RaisePropertyChanged("GeneratedCode2");
            }
        }

        #region filters

        private void ClearFilters()
        {
            FilterTableName = FilterTableColumn = FilterSpName = FilterSpColumn = string.Empty;
        }

        private string _FilterSpName = string.Empty;
        public string FilterSpName
        {
            get
            {
                return _FilterSpName;
            }

            set
            {
                _FilterSpName = value;
                RaisePropertyChanged("FilterSpName");
            }
        }

        private string _FilterSpColumn = string.Empty;
        public string FilterSpColumn
        {
            get
            {
                return _FilterSpColumn;
            }

            set
            {
                _FilterSpColumn = value;
                RaisePropertyChanged("FilterSpColumn");
            }
        }
        private string _FilterTableName = string.Empty;
        public string FilterTableName
        {
            get
            {
                return _FilterTableName;
            }

            set
            {
                _FilterTableName = value;
                RaisePropertyChanged("FilterTableName");
            }
        }
        private string _FilterTableColumn = string.Empty;
        public string FilterTableColumn
        {
            get
            {
                return _FilterTableColumn;
            }

            set
            {
                _FilterTableColumn = value;
                RaisePropertyChanged("FilterTableColumn");
            }
        }

        #endregion

        public void Initialize()
        {
            var lastDatabase = logManagent.Read("last_opened_database").Split('_');
            if (lastDatabase.Length <= 1)
                SelectedConnection = Connections[1];
            else
            {
                SelectedConnection = Connections[Convert.ToInt32(lastDatabase[0])];
                SelectedDatabase = Databases[Convert.ToInt32(lastDatabase[1])];
            }

            var lastSPFilters = logManagent.Read("last_sp_filters").Split(new string[] { "||" }, StringSplitOptions.None);
            if (lastSPFilters.Length > 1)
            {
                FilterSpName = lastSPFilters[0];
                FilterSpColumn = lastSPFilters[1];
            }

            var lastTableFilters = logManagent.Read("last_table_filters").Split(new string[] { "||" }, StringSplitOptions.None);
            if (lastTableFilters.Length > 1)
            {
                FilterTableName = lastTableFilters[0];
                FilterTableColumn = lastTableFilters[1];
            }

            core.ErrorOccurred += new SqlServerCore.ErrorOccurredHandler(core_ErrorOccurred);
        }

        void core_ErrorOccurred(string error)
        {
            Errors.Add(String.Format("{0}: {1}", DateTime.Now, error));
        }



        public void GetSPContent(string SP)
        {
            GeneratedCode1 = core.GenerateCodeForStoredProcedure(SP);
        }


        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged == null)
                return;

            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public void OpenSql()
        {
            var file = Path.Combine(System.Environment.CurrentDirectory, "logs") + @"\query.sql";

            File.WriteAllText(file, this.GeneratedCode2);

            Process.Start(file);

        }
    }
}
