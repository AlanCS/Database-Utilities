using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DatabaseUtilities.DAL;

namespace DatabaseUtilities.UI.Models
{
    public class RelatedObjects
    {
        public RelatedObjects()
        {
            this.ObjectsDependingOnThis = new List<DatabaseObjectWithColumns>();
            this.ObjectsThatThisDependsOn = new List<DatabaseObjectWithColumns>();
        }
        public List<DatabaseObjectWithColumns> ObjectsDependingOnThis { get; set; }
        public List<DatabaseObjectWithColumns> ObjectsThatThisDependsOn  { get; set; }

        public override string ToString()
        {
            return string.Format("In: {0} Out: {1}", this.ObjectsDependingOnThis.Count, this.ObjectsThatThisDependsOn.Count);
        }
    }
}
