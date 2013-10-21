using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace DatabaseUtilities.DAL.Config
{
    public class CustomConnectionSection : ConfigurationSection 
    {
        [ConfigurationProperty("", IsRequired = true, IsDefaultCollection = true)]
        public MyConfigInstanceCollection Instances
        {
            get { return (MyConfigInstanceCollection)this[""]; }
            set { this[""] = value; }
        }


    }

    public class MyConfigInstanceCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new MyConfigInstanceElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            //set to whatever Element Property you want to use for a key
            return ((MyConfigInstanceElement)element).id;
        }
    }

    public class MyConfigInstanceElement : ConfigurationElement
    {
        //Make sure to set IsKey=true for property exposed as the GetElementKey above
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)base["name"]; }
            set { base["name"] = value; }
        }

        [ConfigurationProperty("server", IsRequired = true)]
        public string Server
        {
            get { return (string)base["server"]; }
            set { base["server"] = value; }
        }

        [ConfigurationProperty("environment", IsRequired = false, DefaultValue="unknown")]
        public string Environment
        {
            get { return (string)base["environment"]; }
            set { base["environment"] = value; }
        }

        [ConfigurationProperty("id", IsRequired = true)]
        public int id
        {
            get { return (int)base["id"]; }
            set { base["id"] = value; }
        }

    }
}
