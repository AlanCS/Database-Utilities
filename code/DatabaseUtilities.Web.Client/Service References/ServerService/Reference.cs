﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18052
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This code was auto-generated by Microsoft.Silverlight.ServiceReference, version 5.0.61118.0
// 
namespace DatabaseUtilities.Web.Client.ServerService {
    using System.Runtime.Serialization;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Snapshot", Namespace="http://schemas.datacontract.org/2004/07/DatabaseUtilities.DAL")]
    public partial class Snapshot : object, System.ComponentModel.INotifyPropertyChanged {
        
        private System.Collections.ObjectModel.ObservableCollection<DatabaseUtilities.Web.Client.ServerService.Connection> ConnectionsField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.ObjectModel.ObservableCollection<DatabaseUtilities.Web.Client.ServerService.Connection> Connections {
            get {
                return this.ConnectionsField;
            }
            set {
                if ((object.ReferenceEquals(this.ConnectionsField, value) != true)) {
                    this.ConnectionsField = value;
                    this.RaisePropertyChanged("Connections");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Connection", Namespace="http://schemas.datacontract.org/2004/07/DatabaseUtilities.DAL")]
    public partial class Connection : object, System.ComponentModel.INotifyPropertyChanged {
        
        private System.Collections.ObjectModel.ObservableCollection<DatabaseUtilities.Web.Client.ServerService.Database> DatabasesField;
        
        private string FullConnectionStringField;
        
        private string GroupField;
        
        private int IdField;
        
        private string NameField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.ObjectModel.ObservableCollection<DatabaseUtilities.Web.Client.ServerService.Database> Databases {
            get {
                return this.DatabasesField;
            }
            set {
                if ((object.ReferenceEquals(this.DatabasesField, value) != true)) {
                    this.DatabasesField = value;
                    this.RaisePropertyChanged("Databases");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string FullConnectionString {
            get {
                return this.FullConnectionStringField;
            }
            set {
                if ((object.ReferenceEquals(this.FullConnectionStringField, value) != true)) {
                    this.FullConnectionStringField = value;
                    this.RaisePropertyChanged("FullConnectionString");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Group {
            get {
                return this.GroupField;
            }
            set {
                if ((object.ReferenceEquals(this.GroupField, value) != true)) {
                    this.GroupField = value;
                    this.RaisePropertyChanged("Group");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Id {
            get {
                return this.IdField;
            }
            set {
                if ((this.IdField.Equals(value) != true)) {
                    this.IdField = value;
                    this.RaisePropertyChanged("Id");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Name {
            get {
                return this.NameField;
            }
            set {
                if ((object.ReferenceEquals(this.NameField, value) != true)) {
                    this.NameField = value;
                    this.RaisePropertyChanged("Name");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Database", Namespace="http://schemas.datacontract.org/2004/07/DatabaseUtilities.DAL")]
    public partial class Database : object, System.ComponentModel.INotifyPropertyChanged {
        
        private int ConnectionIdField;
        
        private System.DateTime CreatedField;
        
        private decimal DataFilesSizeKbField;
        
        private ulong DatabaseConnectionIdField;
        
        private int IdField;
        
        private decimal LogFilesSizeKbField;
        
        private string NameField;
        
        private System.Collections.ObjectModel.ObservableCollection<DatabaseUtilities.Web.Client.ServerService.StoredProcedure> StoredProceduresField;
        
        private System.Collections.ObjectModel.ObservableCollection<DatabaseUtilities.Web.Client.ServerService.Table> TablesField;
        
        private System.Collections.ObjectModel.ObservableCollection<DatabaseUtilities.Web.Client.ServerService.View> ViewsField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int ConnectionId {
            get {
                return this.ConnectionIdField;
            }
            set {
                if ((this.ConnectionIdField.Equals(value) != true)) {
                    this.ConnectionIdField = value;
                    this.RaisePropertyChanged("ConnectionId");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime Created {
            get {
                return this.CreatedField;
            }
            set {
                if ((this.CreatedField.Equals(value) != true)) {
                    this.CreatedField = value;
                    this.RaisePropertyChanged("Created");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public decimal DataFilesSizeKb {
            get {
                return this.DataFilesSizeKbField;
            }
            set {
                if ((this.DataFilesSizeKbField.Equals(value) != true)) {
                    this.DataFilesSizeKbField = value;
                    this.RaisePropertyChanged("DataFilesSizeKb");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public ulong DatabaseConnectionId {
            get {
                return this.DatabaseConnectionIdField;
            }
            set {
                if ((this.DatabaseConnectionIdField.Equals(value) != true)) {
                    this.DatabaseConnectionIdField = value;
                    this.RaisePropertyChanged("DatabaseConnectionId");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Id {
            get {
                return this.IdField;
            }
            set {
                if ((this.IdField.Equals(value) != true)) {
                    this.IdField = value;
                    this.RaisePropertyChanged("Id");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public decimal LogFilesSizeKb {
            get {
                return this.LogFilesSizeKbField;
            }
            set {
                if ((this.LogFilesSizeKbField.Equals(value) != true)) {
                    this.LogFilesSizeKbField = value;
                    this.RaisePropertyChanged("LogFilesSizeKb");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Name {
            get {
                return this.NameField;
            }
            set {
                if ((object.ReferenceEquals(this.NameField, value) != true)) {
                    this.NameField = value;
                    this.RaisePropertyChanged("Name");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.ObjectModel.ObservableCollection<DatabaseUtilities.Web.Client.ServerService.StoredProcedure> StoredProcedures {
            get {
                return this.StoredProceduresField;
            }
            set {
                if ((object.ReferenceEquals(this.StoredProceduresField, value) != true)) {
                    this.StoredProceduresField = value;
                    this.RaisePropertyChanged("StoredProcedures");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.ObjectModel.ObservableCollection<DatabaseUtilities.Web.Client.ServerService.Table> Tables {
            get {
                return this.TablesField;
            }
            set {
                if ((object.ReferenceEquals(this.TablesField, value) != true)) {
                    this.TablesField = value;
                    this.RaisePropertyChanged("Tables");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.ObjectModel.ObservableCollection<DatabaseUtilities.Web.Client.ServerService.View> Views {
            get {
                return this.ViewsField;
            }
            set {
                if ((object.ReferenceEquals(this.ViewsField, value) != true)) {
                    this.ViewsField = value;
                    this.RaisePropertyChanged("Views");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="StoredProcedure", Namespace="http://schemas.datacontract.org/2004/07/DatabaseUtilities.DAL")]
    public partial class StoredProcedure : DatabaseUtilities.Web.Client.ServerService.DatabaseObjectWithColumns {
        
        private string TextField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Text {
            get {
                return this.TextField;
            }
            set {
                if ((object.ReferenceEquals(this.TextField, value) != true)) {
                    this.TextField = value;
                    this.RaisePropertyChanged("Text");
                }
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Table", Namespace="http://schemas.datacontract.org/2004/07/DatabaseUtilities.DAL")]
    public partial class Table : DatabaseUtilities.Web.Client.ServerService.DatabaseObjectWithColumns {
        
        private long RowsField;
        
        private decimal TotalSpaceKbField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long Rows {
            get {
                return this.RowsField;
            }
            set {
                if ((this.RowsField.Equals(value) != true)) {
                    this.RowsField = value;
                    this.RaisePropertyChanged("Rows");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public decimal TotalSpaceKb {
            get {
                return this.TotalSpaceKbField;
            }
            set {
                if ((this.TotalSpaceKbField.Equals(value) != true)) {
                    this.TotalSpaceKbField = value;
                    this.RaisePropertyChanged("TotalSpaceKb");
                }
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="View", Namespace="http://schemas.datacontract.org/2004/07/DatabaseUtilities.DAL")]
    public partial class View : DatabaseUtilities.Web.Client.ServerService.DatabaseObjectWithColumns {
        
        private string TextField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Text {
            get {
                return this.TextField;
            }
            set {
                if ((object.ReferenceEquals(this.TextField, value) != true)) {
                    this.TextField = value;
                    this.RaisePropertyChanged("Text");
                }
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="DatabaseObjectWithColumns", Namespace="http://schemas.datacontract.org/2004/07/DatabaseUtilities.DAL")]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(DatabaseUtilities.Web.Client.ServerService.Table))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(DatabaseUtilities.Web.Client.ServerService.View))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(DatabaseUtilities.Web.Client.ServerService.StoredProcedure))]
    public partial class DatabaseObjectWithColumns : object, System.ComponentModel.INotifyPropertyChanged {
        
        private System.Collections.ObjectModel.ObservableCollection<DatabaseUtilities.Web.Client.ServerService.Column> ColumnsField;
        
        private System.DateTime CreatedDateField;
        
        private ulong DatabaseConnectionIdField;
        
        private int IdField;
        
        private System.DateTime LastModifiedDateField;
        
        private string NameField;
        
        private string SchemaField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.ObjectModel.ObservableCollection<DatabaseUtilities.Web.Client.ServerService.Column> Columns {
            get {
                return this.ColumnsField;
            }
            set {
                if ((object.ReferenceEquals(this.ColumnsField, value) != true)) {
                    this.ColumnsField = value;
                    this.RaisePropertyChanged("Columns");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime CreatedDate {
            get {
                return this.CreatedDateField;
            }
            set {
                if ((this.CreatedDateField.Equals(value) != true)) {
                    this.CreatedDateField = value;
                    this.RaisePropertyChanged("CreatedDate");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public ulong DatabaseConnectionId {
            get {
                return this.DatabaseConnectionIdField;
            }
            set {
                if ((this.DatabaseConnectionIdField.Equals(value) != true)) {
                    this.DatabaseConnectionIdField = value;
                    this.RaisePropertyChanged("DatabaseConnectionId");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Id {
            get {
                return this.IdField;
            }
            set {
                if ((this.IdField.Equals(value) != true)) {
                    this.IdField = value;
                    this.RaisePropertyChanged("Id");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime LastModifiedDate {
            get {
                return this.LastModifiedDateField;
            }
            set {
                if ((this.LastModifiedDateField.Equals(value) != true)) {
                    this.LastModifiedDateField = value;
                    this.RaisePropertyChanged("LastModifiedDate");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Name {
            get {
                return this.NameField;
            }
            set {
                if ((object.ReferenceEquals(this.NameField, value) != true)) {
                    this.NameField = value;
                    this.RaisePropertyChanged("Name");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Schema {
            get {
                return this.SchemaField;
            }
            set {
                if ((object.ReferenceEquals(this.SchemaField, value) != true)) {
                    this.SchemaField = value;
                    this.RaisePropertyChanged("Schema");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Column", Namespace="http://schemas.datacontract.org/2004/07/DatabaseUtilities.DAL")]
    public partial class Column : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string DefaultField;
        
        private bool IsNullableField;
        
        private bool IsPrimaryKeyField;
        
        private short LengthField;
        
        private string NameField;
        
        private int PrecisionField;
        
        private int ScaleField;
        
        private string TypeField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Default {
            get {
                return this.DefaultField;
            }
            set {
                if ((object.ReferenceEquals(this.DefaultField, value) != true)) {
                    this.DefaultField = value;
                    this.RaisePropertyChanged("Default");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsNullable {
            get {
                return this.IsNullableField;
            }
            set {
                if ((this.IsNullableField.Equals(value) != true)) {
                    this.IsNullableField = value;
                    this.RaisePropertyChanged("IsNullable");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsPrimaryKey {
            get {
                return this.IsPrimaryKeyField;
            }
            set {
                if ((this.IsPrimaryKeyField.Equals(value) != true)) {
                    this.IsPrimaryKeyField = value;
                    this.RaisePropertyChanged("IsPrimaryKey");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public short Length {
            get {
                return this.LengthField;
            }
            set {
                if ((this.LengthField.Equals(value) != true)) {
                    this.LengthField = value;
                    this.RaisePropertyChanged("Length");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Name {
            get {
                return this.NameField;
            }
            set {
                if ((object.ReferenceEquals(this.NameField, value) != true)) {
                    this.NameField = value;
                    this.RaisePropertyChanged("Name");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Precision {
            get {
                return this.PrecisionField;
            }
            set {
                if ((this.PrecisionField.Equals(value) != true)) {
                    this.PrecisionField = value;
                    this.RaisePropertyChanged("Precision");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Scale {
            get {
                return this.ScaleField;
            }
            set {
                if ((this.ScaleField.Equals(value) != true)) {
                    this.ScaleField = value;
                    this.RaisePropertyChanged("Scale");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Type {
            get {
                return this.TypeField;
            }
            set {
                if ((object.ReferenceEquals(this.TypeField, value) != true)) {
                    this.TypeField = value;
                    this.RaisePropertyChanged("Type");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServerService.IServerService")]
    public interface IServerService {
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IServerService/GetCachedSnapshot", ReplyAction="http://tempuri.org/IServerService/GetCachedSnapshotResponse")]
        System.IAsyncResult BeginGetCachedSnapshot(System.AsyncCallback callback, object asyncState);
        
        DatabaseUtilities.Web.Client.ServerService.Snapshot EndGetCachedSnapshot(System.IAsyncResult result);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IServerServiceChannel : DatabaseUtilities.Web.Client.ServerService.IServerService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class GetCachedSnapshotCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public GetCachedSnapshotCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public DatabaseUtilities.Web.Client.ServerService.Snapshot Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((DatabaseUtilities.Web.Client.ServerService.Snapshot)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ServerServiceClient : System.ServiceModel.ClientBase<DatabaseUtilities.Web.Client.ServerService.IServerService>, DatabaseUtilities.Web.Client.ServerService.IServerService {
        
        private BeginOperationDelegate onBeginGetCachedSnapshotDelegate;
        
        private EndOperationDelegate onEndGetCachedSnapshotDelegate;
        
        private System.Threading.SendOrPostCallback onGetCachedSnapshotCompletedDelegate;
        
        private BeginOperationDelegate onBeginOpenDelegate;
        
        private EndOperationDelegate onEndOpenDelegate;
        
        private System.Threading.SendOrPostCallback onOpenCompletedDelegate;
        
        private BeginOperationDelegate onBeginCloseDelegate;
        
        private EndOperationDelegate onEndCloseDelegate;
        
        private System.Threading.SendOrPostCallback onCloseCompletedDelegate;
        
        public ServerServiceClient() {
        }
        
        public ServerServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ServerServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServerServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServerServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public System.Net.CookieContainer CookieContainer {
            get {
                System.ServiceModel.Channels.IHttpCookieContainerManager httpCookieContainerManager = this.InnerChannel.GetProperty<System.ServiceModel.Channels.IHttpCookieContainerManager>();
                if ((httpCookieContainerManager != null)) {
                    return httpCookieContainerManager.CookieContainer;
                }
                else {
                    return null;
                }
            }
            set {
                System.ServiceModel.Channels.IHttpCookieContainerManager httpCookieContainerManager = this.InnerChannel.GetProperty<System.ServiceModel.Channels.IHttpCookieContainerManager>();
                if ((httpCookieContainerManager != null)) {
                    httpCookieContainerManager.CookieContainer = value;
                }
                else {
                    throw new System.InvalidOperationException("Unable to set the CookieContainer. Please make sure the binding contains an HttpC" +
                            "ookieContainerBindingElement.");
                }
            }
        }
        
        public event System.EventHandler<GetCachedSnapshotCompletedEventArgs> GetCachedSnapshotCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> OpenCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> CloseCompleted;
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult DatabaseUtilities.Web.Client.ServerService.IServerService.BeginGetCachedSnapshot(System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginGetCachedSnapshot(callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        DatabaseUtilities.Web.Client.ServerService.Snapshot DatabaseUtilities.Web.Client.ServerService.IServerService.EndGetCachedSnapshot(System.IAsyncResult result) {
            return base.Channel.EndGetCachedSnapshot(result);
        }
        
        private System.IAsyncResult OnBeginGetCachedSnapshot(object[] inValues, System.AsyncCallback callback, object asyncState) {
            return ((DatabaseUtilities.Web.Client.ServerService.IServerService)(this)).BeginGetCachedSnapshot(callback, asyncState);
        }
        
        private object[] OnEndGetCachedSnapshot(System.IAsyncResult result) {
            DatabaseUtilities.Web.Client.ServerService.Snapshot retVal = ((DatabaseUtilities.Web.Client.ServerService.IServerService)(this)).EndGetCachedSnapshot(result);
            return new object[] {
                    retVal};
        }
        
        private void OnGetCachedSnapshotCompleted(object state) {
            if ((this.GetCachedSnapshotCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.GetCachedSnapshotCompleted(this, new GetCachedSnapshotCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void GetCachedSnapshotAsync() {
            this.GetCachedSnapshotAsync(null);
        }
        
        public void GetCachedSnapshotAsync(object userState) {
            if ((this.onBeginGetCachedSnapshotDelegate == null)) {
                this.onBeginGetCachedSnapshotDelegate = new BeginOperationDelegate(this.OnBeginGetCachedSnapshot);
            }
            if ((this.onEndGetCachedSnapshotDelegate == null)) {
                this.onEndGetCachedSnapshotDelegate = new EndOperationDelegate(this.OnEndGetCachedSnapshot);
            }
            if ((this.onGetCachedSnapshotCompletedDelegate == null)) {
                this.onGetCachedSnapshotCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnGetCachedSnapshotCompleted);
            }
            base.InvokeAsync(this.onBeginGetCachedSnapshotDelegate, null, this.onEndGetCachedSnapshotDelegate, this.onGetCachedSnapshotCompletedDelegate, userState);
        }
        
        private System.IAsyncResult OnBeginOpen(object[] inValues, System.AsyncCallback callback, object asyncState) {
            return ((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(callback, asyncState);
        }
        
        private object[] OnEndOpen(System.IAsyncResult result) {
            ((System.ServiceModel.ICommunicationObject)(this)).EndOpen(result);
            return null;
        }
        
        private void OnOpenCompleted(object state) {
            if ((this.OpenCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.OpenCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void OpenAsync() {
            this.OpenAsync(null);
        }
        
        public void OpenAsync(object userState) {
            if ((this.onBeginOpenDelegate == null)) {
                this.onBeginOpenDelegate = new BeginOperationDelegate(this.OnBeginOpen);
            }
            if ((this.onEndOpenDelegate == null)) {
                this.onEndOpenDelegate = new EndOperationDelegate(this.OnEndOpen);
            }
            if ((this.onOpenCompletedDelegate == null)) {
                this.onOpenCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnOpenCompleted);
            }
            base.InvokeAsync(this.onBeginOpenDelegate, null, this.onEndOpenDelegate, this.onOpenCompletedDelegate, userState);
        }
        
        private System.IAsyncResult OnBeginClose(object[] inValues, System.AsyncCallback callback, object asyncState) {
            return ((System.ServiceModel.ICommunicationObject)(this)).BeginClose(callback, asyncState);
        }
        
        private object[] OnEndClose(System.IAsyncResult result) {
            ((System.ServiceModel.ICommunicationObject)(this)).EndClose(result);
            return null;
        }
        
        private void OnCloseCompleted(object state) {
            if ((this.CloseCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.CloseCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void CloseAsync() {
            this.CloseAsync(null);
        }
        
        public void CloseAsync(object userState) {
            if ((this.onBeginCloseDelegate == null)) {
                this.onBeginCloseDelegate = new BeginOperationDelegate(this.OnBeginClose);
            }
            if ((this.onEndCloseDelegate == null)) {
                this.onEndCloseDelegate = new EndOperationDelegate(this.OnEndClose);
            }
            if ((this.onCloseCompletedDelegate == null)) {
                this.onCloseCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnCloseCompleted);
            }
            base.InvokeAsync(this.onBeginCloseDelegate, null, this.onEndCloseDelegate, this.onCloseCompletedDelegate, userState);
        }
        
        protected override DatabaseUtilities.Web.Client.ServerService.IServerService CreateChannel() {
            return new ServerServiceClientChannel(this);
        }
        
        private class ServerServiceClientChannel : ChannelBase<DatabaseUtilities.Web.Client.ServerService.IServerService>, DatabaseUtilities.Web.Client.ServerService.IServerService {
            
            public ServerServiceClientChannel(System.ServiceModel.ClientBase<DatabaseUtilities.Web.Client.ServerService.IServerService> client) : 
                    base(client) {
            }
            
            public System.IAsyncResult BeginGetCachedSnapshot(System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[0];
                System.IAsyncResult _result = base.BeginInvoke("GetCachedSnapshot", _args, callback, asyncState);
                return _result;
            }
            
            public DatabaseUtilities.Web.Client.ServerService.Snapshot EndGetCachedSnapshot(System.IAsyncResult result) {
                object[] _args = new object[0];
                DatabaseUtilities.Web.Client.ServerService.Snapshot _result = ((DatabaseUtilities.Web.Client.ServerService.Snapshot)(base.EndInvoke("GetCachedSnapshot", _args, result)));
                return _result;
            }
        }
    }
}