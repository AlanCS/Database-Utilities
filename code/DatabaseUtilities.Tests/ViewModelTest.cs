using DatabaseUtilities.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Rhino.Mocks;
using System.Collections.Generic;
using DatabaseUtilities.DAL;
using System.Data;
using System.Data.SqlClient;

namespace DatabaseUtilities.Tests
{
    
    
    /// <summary>
    ///This is a test class for ViewModelTest and is intended
    ///to contain all ViewModelTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ViewModelTest
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
        ///A test for ViewModel Constructor
        ///</summary>
        [TestMethod()]
        public void ViewModelConstructorTest()
        {
            var vw = new ViewModel();

            if (vw.Connections.Count == 0)
                Assert.Fail("didn't get connections");

            vw.Errors.CollectionChanged += delegate(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
            {
                Assert.Fail(e.NewItems[0] as string);
            };

            vw.Initialize();

            if (vw.SelectedConnection == null)
                Assert.Fail("didn't select first connection by default");

            if (vw.Databases.Count == 0)
                Assert.Fail("didn't get any databases");

            if (vw.SelectedDatabase == null)
                Assert.Fail("didn't select first database by default");

            if (vw.Tables.Count == 0)
                Assert.Fail("didn't get any tables");

            if (vw.StoredProcedures.Count == 0)
                Assert.Fail("didn't get any stored procedures");

            if (string.IsNullOrEmpty(vw.SelectedTable))
                Assert.Fail("didn't select first table by default");

            if (string.IsNullOrEmpty(vw.GeneratedCode1) || !vw.GeneratedCode1.Contains("create table"))
                Assert.Fail("didn't generate code to create table properly");

            if (string.IsNullOrEmpty(vw.GeneratedCode2) || !vw.GeneratedCode2.Contains("create proc") || !vw.GeneratedCode2.Contains("select "))
                Assert.Fail("didn't generate code to create stored procedure properly");

            vw.SelectedStoredProcedure = vw.StoredProcedures[0];

            if (string.IsNullOrEmpty(vw.GeneratedCode1) || !vw.GeneratedCode1.Contains("create proc"))
                Assert.Fail("didn't generate code to create proc properly");

            if (string.IsNullOrEmpty(vw.GeneratedCode2) || !vw.GeneratedCode2.Contains(".Open()") || !vw.GeneratedCode2.Contains("CommandText"))
                Assert.Fail("didn't generate c# code to call stored procedure properly");

            var spLengthWithoutExecuting = vw.GeneratedCode2.Length;

            vw.CanExecuteAllStoredProcedures = true;

            if (string.IsNullOrEmpty(vw.GeneratedCode2) || !vw.GeneratedCode2.Contains(".Open()") || !vw.GeneratedCode2.Contains("CommandText"))
                Assert.Fail("didn't generate c# code to call stored procedure properly");

            var spLengthWithExecuting = vw.GeneratedCode2.Length;

            if (spLengthWithExecuting <= spLengthWithoutExecuting)
                Assert.Fail("didn't generate extra code for retrieving SP values");

            vw.OpenSql();
        }


        [TestMethod()]
        public void ViewModel_Table()
        {
            var vw = new ViewModel();

            var mocker = new MockRepository();
            vw.core.database = mocker.CreateMock<DAL.IDatabase>();

            var columns = new List<Column>();
            columns.Add(new Column() { Name = "Id", Type = "int", IsPrimaryKey = true, IsNullable = false });
            columns.Add(new Column() { Name = "Name", Type = "varchar",  Length = 100, IsNullable = false });
            columns.Add(new Column() { Name = "DateOfBirth", Type = "smalldatetime", Default = "1900-01-01", IsNullable = false });
            columns.Add(new Column() { Name = "Active", Type = "bit", Default = "1", IsNullable = false });



            vw.core.database.Expect(c => c.ChangeDatabase(null, string.Empty)).Return(string.Empty).IgnoreArguments();
            vw.core.database.Expect(c => c.ChangeConnection(null)).Return(string.Empty).IgnoreArguments();
            vw.core.database.Expect(c => c.GetDatabases()).Return(new List<string>()).IgnoreArguments();


            vw.core.database.Expect(c => c.GetColumnsForTable(string.Empty)).Return(columns).IgnoreArguments();

            vw.core.database.Replay();

            vw.Errors.CollectionChanged += delegate(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
            {
                Assert.Fail(e.NewItems[0] as string);
            };

            vw.Initialize();

            vw.SelectedTable = "[dbo].[test]";

            if (string.IsNullOrEmpty(vw.SelectedTable))
                Assert.Fail("didn't select first table by default");

            if (string.IsNullOrEmpty(vw.GeneratedCode1) || !vw.GeneratedCode1.Contains("create table"))
                Assert.Fail("didn't generate code to create table properly");

            if (string.IsNullOrEmpty(vw.GeneratedCode2) || !vw.GeneratedCode2.Contains("create proc") || !vw.GeneratedCode2.Contains("select "))
                Assert.Fail("didn't generate code to create stored procedure properly");

            vw.GenerateSP_UpdateInsert();

            if (!vw.GeneratedCode2.Contains("create proc") || !vw.GeneratedCode2.Contains("update ") || !vw.GeneratedCode2.Contains("insert "))
                Assert.Fail("didn't generate code to create stored procedure properly");

            vw.GenerateSP_Delete();

            if (!vw.GeneratedCode2.Contains("create proc") || !vw.GeneratedCode2.Contains("delete "))
                Assert.Fail("didn't generate code to create stored procedure properly");
        }

         [TestMethod()]
        public void ViewModel_StoredProcedure()
        {
            #region arrange
            var vw = new ViewModel();

            string spName = "[dbo].[test]";

            var mocker = new MockRepository();
            vw.core.database = mocker.CreateMock<DAL.IDatabase>();

            var columns = new List<Column>();
            columns.Add(new Column() { Name = "@Id", Type = "int", IsNullable = false });
            columns.Add(new Column() { Name = "@Name", Type = "varchar", Length = 100, IsNullable = false });
            columns.Add(new Column() { Name = "@DateOfBirth", Type = "smalldatetime", Default = "1900-01-01", IsNullable = false });
            columns.Add(new Column() { Name = "@Active", Type = "bit", Default = "1", IsNullable = false });

            var storedProcedureBody = @"create proc " + spName + @"

@Id                                     int,
@Name                                   varchar(100),
@DateOfBirth                            smalldatetime,
@Active                                 bit

as

-- execute complex SQL logic here";

            var dataset3Results = new DataSet();
            var dataTable = new DataTable();
            dataset3Results.Tables.Add(dataTable);

            dataTable.Columns.Add(new DataColumn("Id", SqlDbType.Int.GetType()));
            dataTable.Columns.Add(new DataColumn("Name", SqlDbType.VarChar.GetType()));
            dataTable.Columns.Add(new DataColumn("Birth", SqlDbType.SmallDateTime.GetType()));
            dataTable.Columns.Add(new DataColumn("Active", SqlDbType.Bit.GetType()));

            dataset3Results.Tables.Add(new DataTable());
            dataset3Results.Tables.Add(new DataTable());

            string error = string.Empty;


            vw.core.database.Expect(c => c.ChangeConnection(null)).Return(string.Empty).IgnoreArguments();
            vw.core.database.Expect(c => c.GetDatabases()).Return(new List<string>()).IgnoreArguments();
            vw.core.database.Expect(c => c.GetColumnsForStoredProcedure(string.Empty)).Return(columns).IgnoreArguments();
            vw.core.database.Expect(c => c.GetStoredProcedureBody(string.Empty)).Return(storedProcedureBody).IgnoreArguments();

            vw.core.database.Expect(c => c.ExecuteStoredProcedure(string.Empty, null, out error)).Return(dataset3Results).IgnoreArguments();

            vw.core.database.Replay();

            #endregion


            vw.Errors.CollectionChanged += delegate(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
            {
                Assert.Fail(e.NewItems[0] as string);
            };

            vw.Initialize();

            vw.SelectedStoredProcedure = spName;

            if (string.IsNullOrEmpty(vw.GeneratedCode1) || !vw.GeneratedCode1.Contains("create proc"))
                Assert.Fail("didn't generate code to create proc properly");

            if (!vw.GeneratedCode1.Contains("--exec " + spName))
                Assert.Fail("commented code to call SP with sample parameters is missing");

            if (string.IsNullOrEmpty(vw.GeneratedCode2) || !vw.GeneratedCode2.Contains(".Open()") || !vw.GeneratedCode2.Contains("CommandText"))
                Assert.Fail("didn't generate c# code to call stored procedure properly");

            var spLengthWithoutExecuting = vw.GeneratedCode2.Length;

            vw.CanExecuteAllStoredProcedures = true;

            if (string.IsNullOrEmpty(vw.GeneratedCode2) || !vw.GeneratedCode2.Contains(".Open()") || !vw.GeneratedCode2.Contains("CommandText"))
                Assert.Fail("didn't generate c# code to call stored procedure properly");

            var spLengthWithExecuting = vw.GeneratedCode2.Length;

            if (spLengthWithExecuting <= spLengthWithoutExecuting)
                Assert.Fail("didn't generate extra code for retrieving SP values");

            if (!vw.GeneratedCode2.Contains(".ExecuteReader()"))
                Assert.Fail("didn't generate c# code correctly");

            vw.core.database.VerifyAllExpectations();
        }
    }


}
