using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DatabaseUtilities.DAL
{
    [Serializable]
    [DataContract]
    public class Database
    {
        [DataMember]
        public int ConnectionId { get; set; }
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
        public ulong DatabaseConnectionId
        {
            get
            {
                ulong id = Id > ConnectionId ? (uint)ConnectionId | ((ulong)Id << 32) :
                         (uint)Id | ((ulong)ConnectionId << 32);
                return id;
            }
            set
            {
                throw new Exception("can only be read");
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
    }
}
