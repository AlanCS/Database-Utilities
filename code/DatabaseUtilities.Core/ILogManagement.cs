using System;
namespace DatabaseUtilities.Core
{
    interface ILogManagement
    {
        string Read(string key);
        void Write(string key, string content);
    }
}
