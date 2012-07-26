using DatabaseUtilities.DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Configuration;
using System.Linq;

namespace DatabaseUtilities.Tests
{


    /// <summary>
    ///This is a test class for DatabaseTest and is intended
    ///to contain all DatabaseTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DatabaseTest
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


        /// <summary>
        ///A test for ChangeConnection
        ///</summary>
        [TestMethod()]
        public void ChangeConnectionTest()
        {
            #region arrange
            var db = new SqlServerDatabase();

            // make sure you have a valid connection string in the app.config
            var validConnection = ConfigurationManager.ConnectionStrings.Cast<ConnectionStringSettings>().ToArray()[1].ConnectionString;

            var invalidConnection = validConnection.Replace("Source=", "Source=sdagda");
            #endregion

            #region act
            var error1 = db.ChangeConnection(validConnection);

            var error2 = db.ChangeConnection(invalidConnection);

            db.Dispose();
            #endregion

            #region assert
            if (error1 != string.Empty)
                Assert.Fail("shouldn't have thrown an error: " + error1);

            if (error2 == string.Empty)
                Assert.Fail("didn't detect invalid connection string");
            #endregion
        }


        /// <summary>
        ///A test for ChangeConnection
        ///</summary>
        [TestMethod()]
        public void DatabaseAccessTests()
        {
            #region arrange
            var db = new SqlServerDatabase();

            // make sure you have a valid connection string in the app.config
            var validConnection = ConfigurationManager.ConnectionStrings.Cast<ConnectionStringSettings>().ToArray()[1].ConnectionString;

            #endregion

            #region act

            var errorConnection = db.ChangeConnection(validConnection);

            var databases = db.GetDatabases();

            if (databases.Count == 0)
                Assert.Fail("didn't return any database");

            var errorDatabase = db.ChangeDatabase(validConnection, databases[0]);

            #endregion

            #region assert
            if (errorConnection != string.Empty)
                Assert.Fail("when connecting to server: " + errorConnection);

            if (databases.Count == 0)
                Assert.Fail("no databases returned");

            if (errorDatabase != string.Empty)
                Assert.Fail("when connecting to database: " + errorDatabase);
            #endregion

            /// uses the already existing connection to continue testing

            #region act

            var tables = db.GetTables();

            var storedProcedures = db.GetStoredProcedures();

            #endregion

            #region assert
            if (tables.Count == 0)
                Assert.Fail("no tables returned");

            if (storedProcedures.Count == 0)
                Assert.Fail("no SPs returned");
            #endregion

            #region act
            var columnsTable = db.GetColumnsForTable(tables[0]);
            var columnsStoredProcedures = db.GetColumnsForStoredProcedure(storedProcedures[0]);
            #endregion

            #region assert
            if (columnsTable.Count == 0)
                Assert.Fail("no columns for table {0} returned", tables[0]);

            if (columnsStoredProcedures.Count == 0)
                Assert.Fail("no input parameters for SP {0} returned", storedProcedures[0]);
            #endregion

            #region act
            var storedProcedureText = db.GetStoredProcedureBody(storedProcedures[0]);

            string errorSP = string.Empty;

            var result = db.ExecuteStoredProcedure(storedProcedures[0], columnsStoredProcedures, out errorSP);
            #endregion

            if (string.IsNullOrEmpty(storedProcedureText))
                Assert.Fail("stored procedure body not returned");

            if (errorSP != string.Empty || result == null)
                Assert.Fail("error when executing sp: {0}", errorSP);

            db.Dispose();
        }
    }
}
