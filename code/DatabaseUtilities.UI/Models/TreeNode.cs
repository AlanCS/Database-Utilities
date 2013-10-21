using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;

namespace DatabaseUtilities.UI.Models
{
    public class BaseTreeNode : INotifyPropertyChanged
    {
        #region Data

        bool? _isChecked = false;
        BaseTreeNode _parent;

        #endregion // Data

        public BaseTreeNode()
        {
            this.Children = new System.Collections.Generic.List<BaseTreeNode>();
        }

        public void Initialize()
        {
            foreach (BaseTreeNode child in this.Children)
            {
                child._parent = this;
                child.Initialize();
            }
        }

        public string Name { get; set; }

        #region Properties

        public List<BaseTreeNode> Children { get; private set; }

        private bool _isExpanded = true;
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                _isExpanded = value;
                OnPropertyChanged("IsExpanded");
            }
        }

        #region IsChecked

        /// <summary>
        /// Gets/sets the state of the associated UI toggle (ex. CheckBox).
        /// The return value is calculated based on the check state of all
        /// child TreeNodes.  Setting this property to true or false
        /// will set all children to the same check state, and setting it 
        /// to any value will cause the parent to verify its check state.
        /// </summary>
        public bool? IsChecked
        {
            get { return _isChecked; }
            set
            {
                this.SetIsChecked(value, true, true);
                if (OnChecked != null) OnChecked(this);
            }
        }

        public delegate void OnCheckedHandler(BaseTreeNode node);
        public static OnCheckedHandler OnChecked;

        void SetIsChecked(bool? value, bool updateChildren, bool updateParent)
        {
            if (value == _isChecked)
                return;

            _isChecked = value;

            if (updateChildren && _isChecked.HasValue)
                this.Children.ForEach(c => c.SetIsChecked(_isChecked, true, false));

            if (updateParent && _parent != null)
                _parent.VerifyCheckState();

            this.OnPropertyChanged("IsChecked");
        }

        void VerifyCheckState()
        {
            bool? state = null;
            for (int i = 0; i < this.Children.Count; ++i)
            {
                bool? current = this.Children[i].IsChecked;
                if (i == 0)
                {
                    state = current;
                }
                else if (state != current)
                {
                    state = null;
                    break;
                }
            }
            this.SetIsChecked(state, false, true);
        }

        #endregion // IsChecked

        #endregion // Properties

        #region INotifyPropertyChanged Members

        void OnPropertyChanged(string prop)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public List<T> GetNodesOfType<T>() where T : BaseTreeNode
        {
            var list = new List<T>();

            if (this is T)
                list.Add((T)this);

            if (this.Children.Count > 0)
                this.Children.ForEach(c => list.AddRange(c.GetNodesOfType<T>()));

            return list;
        }
    }

    public interface INode
    {
        string Name { get; }
    }

    public class GenericNode : BaseTreeNode, INode
    {
        private string _name = string.Empty;
        private string _legend = string.Empty;
        public GenericNode(string name, IEnumerable<Models.Environment> environments)
        {
            _name = name;
            _legend = environments.Count() + " environments";

            foreach (var item in environments) this.Children.Add(new EnvironmentNode(item));
        }

        public string Name
        {
            get { return _name; }
        }

        public string Legend
        {
            get { return _legend; }
        }

        public override string ToString()
        {
            return Name;
        }
    }



    public class EnvironmentNode : BaseTreeNode, INode
    {
        private Environment _environment = null;
        public EnvironmentNode(Environment environment)
        {
            _environment = environment;

            foreach (var item in environment.Servers) this.Children.Add(new ServerNode(item));
        }

        public string Name
        {
            get { return _environment.Name; }
        }

        public string Legend
        {
            get { return _environment.Servers.Count + " servers"; }
        }


        public override string ToString()
        {
            return Name;
        }
    }

    public class ServerNode : BaseTreeNode, INode
    {
        private DAL.Server _server = null;
        public ServerNode(DAL.Server server)
        {
            _server = server;
            foreach (var item in _server.Databases) this.Children.Add(new DatabaseNode(item));
        }

        public string Name
        {
            get { return _server.Name; }
        }

        public string Legend
        {
            get { return _server.Databases.Count + " databases"; }
        }


        public override string ToString()
        {
            return Name;
        }
    }

    public class DatabaseNode : BaseTreeNode, INode
    {
        public DatabaseNode(DAL.Database database)
        {
            Database = database;
        }

        public string Name
        {
            get { return Database.Name; }
        }

        public DAL.Database Database { get; set; }

        public Int64 UniqueId
        {
            get { return Database.DatabaseServerId; }
        }

        public string Legend
        {
            get { return string.Format("{0} Mb DataFile(s) | {1} Mb LogFile(s) | Last Modified: {2} ", Math.Round(Database.DataFilesSizeKb / 1024, 2), Math.Round(Database.LogFilesSizeKb / 1024, 2), Database.Created.ToString("g")); }
        }


        public override string ToString()
        {
            return Name;
        }
    }
}