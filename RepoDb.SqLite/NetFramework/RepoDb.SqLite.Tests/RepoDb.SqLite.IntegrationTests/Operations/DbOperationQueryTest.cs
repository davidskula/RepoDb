﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.SqLite.IntegrationTests.Models;
using System.Data.SQLite;
using System.Linq;

namespace RepoDb.SqLite.IntegrationTests.Operations
{
    [TestClass]
    public class DbOperationQueryTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Bootstrap.Initialize();
            //Database.Initialize();
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            //Database.Cleanup();
        }

        [TestMethod]
        public void TestExtractColumnType()
        {
            using (var connection = new SQLiteConnection(@"Data Source=C:\SqLite\Databases\RepoDb.db;Version=3;"))
            {
                using (var reader = connection.ExecuteReader("select * from completetable where id = 5;"))
                {
                    //var text = string.Empty;
                    //for (var i = 0; i < reader.FieldCount; i++)
                    //{
                    //    text = string.Concat(text, $"{reader.GetName(i)} : {reader.GetFieldType(i)}\n");
                    //}
                    while (reader.Read())
                    {
                        var value = reader.GetValue(0);
                        var ordinal = reader.GetOrdinal("ColumnTime");
                        value = reader.GetValue(ordinal);
                    }
                }
            }
        }

        [TestMethod]
        public void TestDbRepositoryAverage()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(@"Data Source=C:\SqLite\Databases\RepoDb.db;Version=3;"))
            {
                // Act
                //repository.InsertAll(tables);

                // Act
                var result = connection.Average<CompleteTable>(e => e.ColumnInt,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryBatchQuery()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(@"Data Source=C:\SqLite\Databases\RepoDb.db;Version=3;"))
            {
                // Act
                //repository.InsertAll(tables);

                // Act
                var result = connection.BatchQuery<CompleteTable>(3,
                    1,
                    OrderField.Parse(new { Id = Order.Ascending }),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryCount()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(@"Data Source=C:\SqLite\Databases\RepoDb.db;Version=3;"))
            {
                // Act
                //repository.InsertAll(tables);

                // Act
                var result = connection.Count<CompleteTable>((object)null);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryDelete()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(@"Data Source=C:\SqLite\Databases\RepoDb.db;Version=3;"))
            {
                // Act
                //repository.InsertAll(tables);

                // Act
                var result = connection.Delete<CompleteTable>(1);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestDbRepositoryQuery()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(@"Data Source=C:\SqLite\Databases\RepoDb.db;Version=3;"))
            {
                // Act
                //repository.InsertAll(tables);

                // Act
                var result = connection.QueryAll<CompleteTable>();

                // Assert
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.FirstOrDefault(t => t.Id == table.Id));
                });
            }
        }
    }
}
