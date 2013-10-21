using DatabaseUtilities.DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace DatabaseUtilities.Tests
{


    /// <summary>
    ///This is a test class for DatabaseTest and is intended
    ///to contain all DatabaseTest Unit Tests
    ///</summary>
    [TestClass()]
    public class IntegrationTests
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        [TestMethod()]
        public void GetCachedSnapshot()
        {
            #region arrange
            var db = new DatabaseService();
            #endregion

            #region act GetCachedSnapshot
            var snapshot = db.GetCachedSnapshot();       
            #endregion

            #region assert
            Assert.AreNotEqual(0, snapshot.Servers.Count);

            Assert.AreNotEqual(0, snapshot.Servers[0].Databases.Count);

            Assert.AreNotEqual(0, snapshot.Servers[0].Databases[0].Tables.Count);

            var allTables = snapshot.Servers.SelectMany(c => c.Databases).SelectMany(c => c.Tables).ToArray();
            Assert.AreNotEqual(0, allTables.Count());
            Assert.AreEqual(0, allTables.Where(c => c.Columns.Count == 0).Count());
            

            Assert.AreNotEqual(0, snapshot.Servers[0].Databases[0].Views.Count);
            Assert.AreNotEqual(0, snapshot.Servers[0].Databases[0].Views[0].Columns.Count);
            if (string.IsNullOrEmpty(snapshot.Servers[0].Databases[0].Views[0].Text)) Assert.Fail("didn't return code");

            Assert.AreNotEqual(0, snapshot.Servers[0].Databases[0].StoredProcedures.Count);
            Assert.AreNotEqual(0, snapshot.Servers[0].Databases[0].StoredProcedures[0].Columns.Count);


            var allStoredProcedures = snapshot.Servers.SelectMany(c => c.Databases).SelectMany(c => c.StoredProcedures).ToArray();
            Assert.AreNotEqual(0, allStoredProcedures.Count());
            Assert.AreEqual(0, allStoredProcedures.Where(c => string.IsNullOrEmpty(c.Text)).Count());
            #endregion

            #region act GetChangesSince and LastUpdatedObject
            var differential = snapshot.GetChangesSince(DateTime.Now.AddDays(-1));
            var lastUpdated = snapshot.LastUpdatedObject;
            #endregion


            #region act FillSnapshotWithLatestChanges
            db.FillSnapshotWithLatestChanges(snapshot, snapshot.LastUpdatedObject.Value.AddDays(-3));
            #endregion


            #region GetCachedSnapshot that triggers FillSnapshotWithLatestChanges
            ApplicationKeeper.Remove("LastTimeCheckedForUpdates");
            db.GetCachedSnapshot();
            #endregion

            /// save it to file to check size and format
            new XmlSerializer(typeof(Snapshot)).Serialize(new FileStream("snapshot.txt", FileMode.Create), snapshot);
        }



    }
}
