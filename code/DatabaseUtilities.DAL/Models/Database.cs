using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DatabaseUtilities.DAL
{
    [Serializable]
    [DataContract]
    public class Database: IEquatable<Database>
    {
        public Database()
        {
            this.Tables = new List<Table>();
            this.StoredProcedures = new List<StoredProcedure>();
            this.Views = new List<View>();
        }

        [DataMember]
        public int ServerId { get; set; }
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public decimal DataFilesSizeKb { get; set; }
        [DataMember]
        public decimal LogFilesSizeKb { get; set; }

        [DataMember]
        public DateTime Created { get; set; }

        [DataMember]
        public long DatabaseServerId
        {
            get
            {
                var id = ((long)Id << 32) + ServerId;

                return id;
            }
            set
            {
                
            }
        }

        [DataMember]
        public List<StoredProcedure> StoredProcedures { get; set; }
        [DataMember]
        public List<Table> Tables { get; set; }
        [DataMember]
        public List<View> Views { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public override int GetHashCode()
        {
            return this.DatabaseServerId.GetHashCode();
        }

        bool IEquatable<Database>.Equals(Database other)
        {
            return this.DatabaseServerId == other.DatabaseServerId;
        }
    }
}
