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
using DatabaseUtilities.Core.Models;

namespace DatabaseUtilities.Core
{
    public partial class ViewModel2 : INotifyPropertyChanged
    {
        LogManagement logManagent = new LogManagement();
        public SqlServerCore core = new SqlServerCore();

        public ViewModel2()
        {
        }

        #region public methods

        public void Search()
        {

        }

        public void UpdateTableScriptToSelect()
        {

        }

        public void UpdateTableScriptToInsertUpdate()
        {

        }

        #endregion

        #region properties

        private bool _IsLoadingSnapshot = false;
        public bool IsLoadingSnapshot
        {
            get
            {
                return _IsLoadingSnapshot;
            }

            set
            {
                if (_IsLoadingSnapshot == value)  return;

                RaisePropertyChanged("IsLoadingSnapshot");
            }
        }

        private string _DatabasesSearching = string.Empty;
        public string DatabasesSearching
        {
            get
            {
                return _DatabasesSearching;
            }

            set
            {
                if (_DatabasesSearching == value) return;

                RaisePropertyChanged("DatabasesSearching");
            }
        }

        private string _FilterTable = string.Empty;
        public string FilterTable
        {
            get
            {
                return _FilterTable;
            }

            set
            {
                if (value.Equals(_FilterTable))
                    return;

                _FilterTable = value;
                RaisePropertyChanged("FilterTable");
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
                if (value.Equals(_FilterTableColumn))
                    return;

                _FilterTableColumn = value;
                RaisePropertyChanged("FilterTableColumn");
            }
        }

        ObservableCollection<DatabaseObjectWithParents> _SearchResults = new ObservableCollection<DatabaseObjectWithParents>();
        public ObservableCollection<DatabaseObjectWithParents> SearchResults
        {
            get
            {
                return _SearchResults;
            }

            set
            {
                if (value.Equals(_SearchResults))
                    return;

                _SearchResults = value;
                RaisePropertyChanged("SearchResults");
            }
        }

        private bool _IsSearching = false;
        public bool IsSearching
        {
            get
            {
                return _IsSearching;
            }

            set
            {
                if (_IsSearching == value) return;

                RaisePropertyChanged("IsSearching");
            }
        }

        public ObjectTypes _ObjectType = ObjectTypes.Table;
        public ObjectTypes ObjectType
        {
            get
            {
                return _ObjectType;
            }

            private set
            {
                if (value.Equals(_ObjectType))
                    return;

                _ObjectType = value;
                RaisePropertyChanged("ObjectType");
            }
        }

        private DatabaseObjectWithParents _Selected = null;
        public DatabaseObjectWithParents Selected
        {
            get
            {
                return _Selected;
            }

            set
            {
                if (value.Equals(_Selected))
                    return;

                _Selected = value;
                RaisePropertyChanged("Selected");
            }
        }

        private DateTime? _LastUpdatedSnapshot = null;
        public DateTime? LastUpdatedSnapshot
        {
            get
            {
                return _LastUpdatedSnapshot;
            }

            set
            {
                if (value.Equals(_LastUpdatedSnapshot))
                    return;

                _LastUpdatedSnapshot = value;
                RaisePropertyChanged("LastUpdatedSnapshot");
            }
        }


        private DateTime? _LastSearched = null;
        public DateTime? LastSearched
        {
            get
            {
                return _LastSearched;
            }

            set
            {
                if (value.Equals(_LastSearched))
                    return;

                _LastSearched = value;
                RaisePropertyChanged("LastSearched");
            }
        }

        public enum ObjectTypes
        {
            Table, StoredProcedure, View
        }

        #region table related

        private string _Table_Definition = string.Empty;
        public string Table_Definition
        {
            get
            {
                return _Table_Definition;
            }

            set
            {
                if (value.Equals(_Table_Definition))
                    return;

                _Table_Definition = value;
                RaisePropertyChanged("Table_Definition");
            }
        }

        private string _Table_Script = string.Empty;
        public string Table_Script
        {
            get
            {
                return _Table_Script;
            }

            set
            {
                if (value.Equals(_Table_Script))
                    return;

                _Table_Script = value;
                RaisePropertyChanged("Table_Script");
            }
        }

        ObservableCollection<DatabaseObjectWithParents> _TableRelatedObjects = new ObservableCollection<DatabaseObjectWithParents>();
        public ObservableCollection<DatabaseObjectWithParents> TableRelatedObjects
        {
            get
            {
                return _TableRelatedObjects;
            }

            set
            {
                if (value.Equals(_TableRelatedObjects))
                    return;

                _TableRelatedObjects = value;
                RaisePropertyChanged("TableRelatedObjects");
            }
        }

        #endregion

        #region stored procedure related

        private string _StoredProcedure_Definition = string.Empty;
        public string StoredProcedure_Definition
        {
            get
            {
                return _StoredProcedure_Definition;
            }

            set
            {
                if (value.Equals(_StoredProcedure_Definition))
                    return;

                _StoredProcedure_Definition = value;
                RaisePropertyChanged("StoredProcedure_Definition");
            }
        }

        private string _StoredProcedure_Script = string.Empty;
        public string StoredProcedure_Script
        {
            get
            {
                return _StoredProcedure_Script;
            }

            set
            {
                if (value.Equals(_StoredProcedure_Script))
                    return;

                _StoredProcedure_Script = value;
                RaisePropertyChanged("StoredProcedure_Script");
            }
        }

        ObservableCollection<DatabaseObjectWithParents> _StoredProcedureRelatedObjects = new ObservableCollection<DatabaseObjectWithParents>();
        public ObservableCollection<DatabaseObjectWithParents> StoredProcedureRelatedObjects
        {
            get
            {
                return _StoredProcedureRelatedObjects;
            }

            set
            {
                if (value.Equals(_StoredProcedureRelatedObjects))
                    return;

                _StoredProcedureRelatedObjects = value;
                RaisePropertyChanged("StoredProcedureRelatedObjects");
            }
        }

        #endregion

        #region view related

        private string _View_Definition = string.Empty;
        public string View_Definition
        {
            get
            {
                return _View_Definition;
            }

            set
            {
                if (value.Equals(_View_Definition))
                    return;

                _View_Definition = value;
                RaisePropertyChanged("View_Definition");
            }
        }

        private string _View_Script = string.Empty;
        public string View_Script
        {
            get
            {
                return _View_Script;
            }

            set
            {
                if (value.Equals(_View_Script))
                    return;

                _View_Script = value;
                RaisePropertyChanged("View_Script");
            }
        }

        ObservableCollection<DatabaseObjectWithParents> _ViewRelatedObjects = new ObservableCollection<DatabaseObjectWithParents>();
        public ObservableCollection<DatabaseObjectWithParents> ViewRelatedObjects
        {
            get
            {
                return _ViewRelatedObjects;
            }

            set
            {
                if (value.Equals(_ViewRelatedObjects))
                    return;

                _ViewRelatedObjects = value;
                RaisePropertyChanged("ViewRelatedObjects");
            }
        }

        #endregion

        #endregion

        #region events

        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged == null)
                return;

            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
