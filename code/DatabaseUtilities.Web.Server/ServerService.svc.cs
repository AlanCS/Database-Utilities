using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace DatabaseUtilities.Web.Server
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ServerService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select ServerService.svc or ServerService.svc.cs at the Solution Explorer and start debugging.
    public class ServerService : IServerService
    {
        public DAL.Snapshot GetCachedSnapshot()
        {
            return new DAL.DatabaseService().GetCachedSnapshot();
        }
    }
}
