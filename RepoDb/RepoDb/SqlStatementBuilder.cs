﻿using RepoDb.Extensions;
using RepoDb.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System;

namespace RepoDb
{
    /// <summary>
    /// A class used to build a SQL Statement for SQL Server. This is the default statement builder used by the library.
    /// </summary>
    public sealed class SqlStatementBuilder : IStatementBuilder
    {
        /// <summary>
        /// Creates a new instance of <see cref="SqlStatementBuilder"/> object.
        /// </summary>
        public SqlStatementBuilder() { }

        #region CreateBatchQuery

        /// <summary>
        /// Creates a SQL Statement for batch query operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The list of fields to query.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="page">The page of the batch.</param>
        /// <param name="rowsPerBatch">The number of rows per batch.</param>
        /// <param name="orderBy">The list of fields for ordering.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlTableHints"/> class.</param>
        /// <returns>A sql statement for batch query operation.</returns>
        public string CreateBatchQuery(QueryBuilder queryBuilder,
            string tableName,
            IEnumerable<Field> fields,
            QueryGroup where = null,
            int? page = null,
            int? rowsPerBatch = null,
            IEnumerable<OrderField> orderBy = null,
            string hints = null)
        {
            // There should be fields
            if (fields == null || fields?.Any() == false)
            {
                throw new InvalidOperationException($"No batch queryable fields for '{tableName}'.");
            }

            // Build the SQL Statement
            queryBuilder = queryBuilder ?? new QueryBuilder();
            queryBuilder
                .Clear()
                .With()
                .WriteText("CTE")
                .As()
                .OpenParen()
                .Select()
                .RowNumber()
                .Over()
                .OpenParen()
                .OrderByFrom(orderBy)
                .CloseParen()
                .As("[RowNumber],")
                .FieldsFrom(fields)
                .From()
                .TableNameFrom(tableName);

            // Build the query optimizers
            if (hints != null)
            {
                queryBuilder
                    .WriteText(hints);
            }

            // Build the filter and ordering
            queryBuilder
                .WhereFrom(where)
                .CloseParen()
                .Select()
                .FieldsFrom(fields)
                .From()
                .WriteText("CTE")
                .WriteText(string.Concat("WHERE ([RowNumber] BETWEEN ", (page * rowsPerBatch) + 1, " AND ", (page + 1) * rowsPerBatch, ")"))
                .OrderByFrom(orderBy)
                .End();

            // Return the query
            return queryBuilder.GetString();
        }

        #endregion

        #region CreateCount

        /// <summary>
        /// Creates a SQL Statement for count operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlTableHints"/> class.</param>
        /// <returns>A sql statement for create operation.</returns>
        public string CreateCount(QueryBuilder queryBuilder,
            string tableName,
            QueryGroup where = null,
            string hints = null)
        {
            queryBuilder = queryBuilder ?? new QueryBuilder();
            queryBuilder
                .Clear()
                .Select()
                .CountBig()
                .WriteText("(1) AS [Counted]")
                .From()
                .TableNameFrom(tableName);

            // Build the query optimizers
            if (hints != null)
            {
                queryBuilder
                    .WriteText(hints);
            }

            // Build the filter and ordering
            queryBuilder
                .WhereFrom(where)
                .End();

            // Return the query
            return queryBuilder.GetString();
        }

        #endregion

        #region CreateDelete

        /// <summary>
        /// Creates a SQL Statement for delete operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression.</param>
        /// <returns>A sql statement for delete operation.</returns>
        public string CreateDelete(QueryBuilder queryBuilder,
            string tableName,
            QueryGroup where = null)
        {
            queryBuilder = queryBuilder ?? new QueryBuilder();
            queryBuilder
                .Clear()
                .Delete()
                .From()
                .TableNameFrom(tableName)
                .WhereFrom(where)
                .End();
            return queryBuilder.GetString();
        }

        #endregion

        #region CreateDeleteAll

        /// <summary>
        /// Creates a SQL Statement for delete-all operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <returns>A sql statement for delete-all operation.</returns>
        public string CreateDeleteAll(QueryBuilder queryBuilder,
            string tableName)
        {
            queryBuilder = queryBuilder ?? new QueryBuilder();
            queryBuilder
                .Clear()
                .Delete()
                .From()
                .TableNameFrom(tableName)
                .End();
            return queryBuilder.GetString();
        }

        #endregion

        #region CreateInsert

        /// <summary>
        /// Creates a SQL Statement for insert operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="primaryField">The primary field from the database.</param>
        /// <param name="fields">The list of fields to be inserted.</param>
        /// <returns>A sql statement for insert operation.</returns>
        public string CreateInsert(QueryBuilder queryBuilder,
            string tableName,
            DbField primaryField = null,
            IEnumerable<Field> fields = null)
        {
            // Variables needed
            var primaryName = primaryField?.Name;
            var isIdentity = primaryField?.IsIdentity == true;
            var insertableFields = fields
                .Where(f => !(isIdentity == true && f.Name.ToLower() == primaryName?.ToLower()));

            // Build the SQL Statement
            queryBuilder = queryBuilder ?? new QueryBuilder();
            queryBuilder
                .Clear()
                .Insert()
                .Into()
                .TableNameFrom(tableName)
                .OpenParen()
                .FieldsFrom(insertableFields)
                .CloseParen()
                .Values()
                .OpenParen()
                .ParametersFrom(insertableFields)
                .CloseParen()
                .End();
            var result = isIdentity == true ? "SCOPE_IDENTITY()" : string.IsNullOrEmpty(primaryName) == false ? primaryName.AsParameter() : "NULL";
            queryBuilder
                .Select()
                .WriteText(result)
                .As("[Result]")
                .End();

            // Return the query
            return queryBuilder.GetString();
        }

        #endregion

        #region CreateMerge

        /// <summary>
        /// Creates a SQL Statement for merge operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="primaryField">The primary field from the database.</param>
        /// <param name="fields">The list of fields to be merged.</param>
        /// <param name="qualifiers">The list of the qualifier fields.</param>
        /// <returns>A sql statement for merge operation.</returns>
        public string CreateMerge(QueryBuilder queryBuilder,
            string tableName,
            DbField primaryField = null,
            IEnumerable<Field> fields = null,
            IEnumerable<Field> qualifiers = null)
        {
            // Get the needed properties
            var primaryName = primaryField?.Name;
            var isIdentity = primaryField?.IsIdentity == true;

            // Check for all the qualifier (from the entity table)
            if (qualifiers != null)
            {
                var unmatchesQualifiers = qualifiers?.Where(field =>
                    fields?.FirstOrDefault(f =>
                        field.Name.ToLower() == f.Name.ToLower()) == null);
                if (unmatchesQualifiers?.Count() > 0)
                {
                    throw new InvalidOperationException($"The qualifiers '{unmatchesQualifiers.Select(field => field.AsField()).Join(", ")}' are not " +
                        $"present at the given fields.");
                }
            }
            else
            {
                // Use the primary for qualifiers if there is no any
                if (string.IsNullOrEmpty(primaryName) == false)
                {
                    qualifiers = Field.From(primaryName);
                }
                else
                {
                    throw new InvalidOperationException($"No qualifier fields found for '{tableName}'.");
                }
            }

            // Get the insertable and updateable fields
            var insertableFields = fields
                .Where(field => (field.Name.ToLower() == primaryName?.ToLower() && isIdentity) == false);
            var updateableFields = fields
                .Where(field => (field.Name.ToLower() == primaryName?.ToLower()) == false);

            // Build the SQL Statement
            queryBuilder = queryBuilder ?? new QueryBuilder();
            queryBuilder
                .Clear()
                // MERGE T USING S
                .Merge()
                .TableNameFrom(tableName)
                .As("T")
                .Using()
                .OpenParen()
                .Select()
                .ParametersAsFieldsFrom(fields)
                .CloseParen()
                .As("S")
                // QUALIFIERS
                .On()
                .OpenParen()
                .WriteText(qualifiers?
                    .Select(
                        field => field.AsJoinQualifier("S", "T"))
                            .Join(" AND "))
                .CloseParen()
                // WHEN NOT MATCHED THEN INSERT VALUES
                .When()
                .Not()
                .Matched()
                .Then()
                .Insert()
                .OpenParen()
                .FieldsFrom(insertableFields)
                .CloseParen()
                .Values()
                .OpenParen()
                .AsAliasFieldsFrom(insertableFields, "S")
                .CloseParen()
                // WHEN MATCHED THEN UPDATE SET
                .When()
                .Matched()
                .Then()
                .Update()
                .Set()
                .FieldsAndAliasFieldsFrom(updateableFields, "S")
                .End();

            // Return the query
            return queryBuilder.GetString();
        }

        #endregion

        #region CreateQuery

        /// <summary>
        /// Creates a SQL Statement for query operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The list of fields.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="orderBy">The list of fields for ordering.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlTableHints"/> class.</param>
        /// <returns>A sql statement for query operation.</returns>
        public string CreateQuery(QueryBuilder queryBuilder,
            string tableName,
            IEnumerable<Field> fields,
            QueryGroup where = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = null,
            string hints = null)
        {
            // There should be fields
            if (fields == null || fields.Any() == false)
            {
                throw new InvalidOperationException($"There are no queryable fields found for '{tableName}'.");
            }

            // Build the SQL Statement
            queryBuilder = queryBuilder ?? new QueryBuilder();

            // Build the base
            queryBuilder
                .Clear()
                .Select()
                .TopFrom(top)
                .FieldsFrom(fields)
                .From()
                .TableNameFrom(tableName);

            // Build the query optimizers
            if (hints != null)
            {
                queryBuilder
                    .WriteText(hints);
            }

            // Build the filter and ordering
            queryBuilder
                .WhereFrom(where)
                .OrderByFrom(orderBy)
                .End();

            // Return the query
            return queryBuilder.GetString();
        }

        #endregion

        #region CreateTruncate

        /// <summary>
        /// Creates a SQL Statement for truncate operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <returns>A sql statement for truncate operation.</returns>
        public string CreateTruncate(QueryBuilder queryBuilder,
            string tableName)
        {
            queryBuilder = queryBuilder ?? new QueryBuilder();

            // Build the SQL Statement
            queryBuilder
                .Clear()
                .Truncate()
                .Table()
                .TableNameFrom(tableName)
                .End();

            // Return the query
            return queryBuilder.GetString();
        }

        #endregion

        #region CreateUpdate

        /// <summary>
        /// Creates a SQL Statement for inline-update operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="primaryField">The primary field from the database.</param>
        /// <param name="fields">The list of fields to be updated.</param>
        /// <param name="where">The query expression.</param>
        /// <returns>A sql statement for update operation.</returns>
        public string CreateUpdate(QueryBuilder queryBuilder,
            string tableName,
            DbField primaryField = null,
            IEnumerable<Field> fields = null,
            QueryGroup where = null)
        {
            // Variables needed
            var primaryName = primaryField?.Name;
            var isIdentity = primaryField?.IsIdentity == true;
            var updatableFields = fields
                .Where(f => !(isIdentity == true && f.Name.ToLower() == primaryName?.ToLower()));

            // Append the proper prefix
            where?.AppendParametersPrefix();

            // Build the SQL Statement
            queryBuilder = queryBuilder ?? new QueryBuilder();
            queryBuilder
                .Clear()
                .Update()
                .TableNameFrom(tableName)
                .Set()
                .FieldsAndParametersFrom(updatableFields)
                .WhereFrom(where)
                .End();

            // Return the query
            return queryBuilder.GetString();
        }

        #endregion
    }
}
