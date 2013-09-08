using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace DatabaseUtilities.DAL
{
    public static class ApplicationKeeper
    {
        private static Dictionary<string, object> ApplicationReplacer = new Dictionary<string, object>();

        public static void AddUpdate(string key, object obj)
        {
            if (HttpContext.Current == null)
                ApplicationReplacer[key] = obj;
            else
                HttpContext.Current.Application[key] = obj;
        }

        public static object Get(string key)
        {
            if (HttpContext.Current == null)
            {
                if (!ApplicationReplacer.ContainsKey(key)) return null;
                return ApplicationReplacer[key];
            }

            if (!HttpContext.Current.Application.AllKeys.Contains(key)) return null;
            return HttpContext.Current.Application[key];
        }

        public static void Remove(string key)
        {
            if (HttpContext.Current == null)
            {
                if (!ApplicationReplacer.ContainsKey(key)) return;
                ApplicationReplacer.Remove(key);
                return;
            }

            if (!HttpContext.Current.Application.AllKeys.Contains(key)) return ;
            HttpContext.Current.Application.Remove(key);
        }
    }
}
