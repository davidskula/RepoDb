using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Requests;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// Contains the extension methods for <see cref="IDbConnection"/> object.
    /// </summary>
    public static partial class DbConnectionExtension
    {
        #region QueryTableFunc<TEntity>

        /// <summary>
        /// Queries a data from the database table function.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="funcName">Function name.</param>
        /// <param name="parameters">The dynamic expression of function parameters.</param>
        /// <param name="whereOrPrimaryKey">The dynamic expression or the primary key value to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The top number of data to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force to query from the database.
        /// </param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public static IEnumerable<TEntity> QueryTableFunc<TEntity>(this IDbConnection connection,
            string funcName,
            object parameters,
            object whereOrPrimaryKey,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return QueryTableFunc<TEntity>(connection: connection,
                funcName: funcName,
                parameters: parameters == null ? null : QueryField.Parse(parameters),
                where: WhereOrPrimaryKeyToQueryGroup<TEntity>(connection, whereOrPrimaryKey, transaction),
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Queries a data from the database table function.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="funcName">Function name.</param>
        /// <param name="parameters">Collection of function parameters.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The top number of data to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force to query from the database.
        /// </param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public static IEnumerable<TEntity> QueryTableFunc<TEntity>(this IDbConnection connection,
            string funcName,
            IEnumerable<QueryField> parameters,
            QueryField where,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return QueryTableFunc<TEntity>(connection: connection,
                funcName: funcName,
                parameters: parameters,
                where: ToQueryGroup(where),
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Queries a data from the database table function.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="funcName">Function name.</param>
        /// <param name="parameters">Collection of function parameters.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The top number of data to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force to query from the database.
        /// </param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public static IEnumerable<TEntity> QueryTableFunc<TEntity>(this IDbConnection connection,
            string funcName,
            IEnumerable<QueryField> parameters,
            IEnumerable<QueryField> where,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return QueryTableFunc<TEntity>(connection: connection,
                funcName: funcName,
                parameters: parameters,
                where: ToQueryGroup(where),
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Queries a data from the database table function.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="funcName">Function name.</param>
        /// <param name="parameters">The dynamic expression of function parameters.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The top number of data to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force to query from the database.
        /// </param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public static IEnumerable<TEntity> QueryTableFunc<TEntity>(this IDbConnection connection,
            string funcName,
            object parameters,
            Expression<Func<TEntity, bool>> where,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null) where TEntity : class
        {
            return QueryTableFunc<TEntity>(connection: connection,
                funcName: funcName,
                parameters: parameters == null ? null : QueryField.Parse(parameters),
                where: ToQueryGroup(where),
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Queries a data from the database table function.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="funcName">Function name.</param>
        /// <param name="parameters">Collection of function parameters.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The top number of data to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force to query from the database.
        /// </param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public static IEnumerable<TEntity> QueryTableFunc<TEntity>(this IDbConnection connection,
            string funcName,
            IEnumerable<QueryField> parameters,
            QueryGroup where,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return QueryTableFuncInternal<TEntity>(connection: connection,
                funcName: funcName,
                parameters: parameters,
                where: where,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Queries a data from the database table function.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="funcName">Function name.</param>
        /// <param name="parameters">Collection of function parameters.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The top number of data to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force to query from the database.
        /// </param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        internal static IEnumerable<TEntity> QueryTableFuncInternal<TEntity>(this IDbConnection connection,
            string funcName,
            IEnumerable<QueryField> parameters,
            QueryGroup where,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return QueryTableFuncInternalBase<TEntity>(connection: connection,
                funcName: funcName,
                parameters: parameters,
                fields: FieldCache.Get<TEntity>(),
                where: where,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        #endregion

        #region QueryAsync<TEntity>

        /// <summary>
        /// Queries a data from the database table function in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="funcName">Function name.</param>
        /// <param name="parameters">The dynamic expression of function parameters.</param>
        /// <param name="whereOrPrimaryKey">The dynamic expression or the primary key value to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The top number of data to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force to query from the database.
        /// </param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public static Task<IEnumerable<TEntity>> QueryTableFuncAsync<TEntity>(this IDbConnection connection,
            string funcName,
            object parameters,
            object whereOrPrimaryKey,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return QueryTableFuncAsync<TEntity>(connection: connection,
                funcName: funcName,
                parameters: parameters == null ? null : QueryField.Parse(parameters),
                where: WhereOrPrimaryKeyToQueryGroup<TEntity>(connection, whereOrPrimaryKey, transaction),
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Queries a data from the database table function in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="funcName">Function name.</param>
        /// <param name="parameters">Collection of function parameters.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The top number of data to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force to query from the database.
        /// </param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public static Task<IEnumerable<TEntity>> QueryTableFuncAsync<TEntity>(this IDbConnection connection,
            string funcName,
            IEnumerable<QueryField> parameters,
            QueryField where,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return QueryTableFuncAsync<TEntity>(connection: connection,
                funcName: funcName,
                parameters: parameters,
                where: ToQueryGroup(where),
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Queries a data from the database table function in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="funcName">Function name.</param>
        /// <param name="parameters">Collection of function parameters.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The top number of data to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force to query from the database.
        /// </param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public static Task<IEnumerable<TEntity>> QueryTableFuncAsync<TEntity>(this IDbConnection connection,
            string funcName,
            IEnumerable<QueryField> parameters,
            IEnumerable<QueryField> where,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return QueryTableFuncAsync<TEntity>(connection: connection,
                funcName: funcName,
                parameters: parameters,
                where: ToQueryGroup(where),
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Queries a data from the database table function in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="funcName">Function name.</param>
        /// <param name="parameters">The dynamic expression of function parameters.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The top number of data to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force to query from the database.
        /// </param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public static Task<IEnumerable<TEntity>> QueryTableFuncAsync<TEntity>(this IDbConnection connection,
            string funcName,
            object parameters,
            Expression<Func<TEntity, bool>> where,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null) where TEntity : class
        {
            return QueryTableFuncAsync<TEntity>(connection: connection,
                funcName: funcName,
                parameters: parameters == null ? null : QueryField.Parse(parameters),
                where: ToQueryGroup(where),
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Queries a data from the database table function in an asynchronous way.
        /// </summary>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="funcName">Function name.</param>
        /// <param name="parameters">Collection of function parameters.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The top number of data to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force to query from the database.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public static Task<IEnumerable<TEntity>> QueryTableFuncAsync<TEntity>(this IDbConnection connection,
            string funcName,
            IEnumerable<QueryField> parameters,
            QueryGroup where,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return QueryTableFuncAsyncInternal<TEntity>(connection: connection,
                funcName: funcName,
                parameters: parameters,
                where: where,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Queries a data from the database table function in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="funcName">Function name.</param>
        /// <param name="parameters">Collection of function parameters.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The top number of data to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force to query from the database.
        /// </param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        internal static Task<IEnumerable<TEntity>> QueryTableFuncAsyncInternal<TEntity>(this IDbConnection connection,
            string funcName,
            IEnumerable<QueryField> parameters,
            QueryGroup where,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return QueryTableFuncAsyncInternalBase<TEntity>(connection: connection,
                funcName: funcName,
                parameters: parameters,
                fields: FieldCache.Get<TEntity>(),
                where: where,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        #endregion

        #region QueryInternalBase<TEntity>

        /// <summary>
        /// Queries a data from the database table function.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="funcName">Function name.</param>
        /// <param name="parameters">Collection of function parameters.</param>
        /// <param name="fields">The list of fields to be queried.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The top number of data to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force to query from the database.
        /// </param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        internal static IEnumerable<TEntity> QueryTableFuncInternalBase<TEntity>(this IDbConnection connection,
            string funcName,
            IEnumerable<QueryField> parameters = null,
            IEnumerable<Field> fields = null,
            QueryGroup where = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Get Cache
            if (cacheKey != null)
            {
                var item = cache?.Get(cacheKey, false);
                if (item != null)
                {
                    return (IEnumerable<TEntity>)item.Value;
                }
            }

            // Variables
            var commandType = CommandType.Text;
            var request = new QueryTableFuncRequest(typeof(TEntity),
                funcName,
                connection,
                transaction,
                parameters?.GetFields(),
                fields,
                where,
                orderBy,
                top,
                hints,
                statementBuilder);
            var commandText = CommandTextCache.GetQueryTableFuncText(request);
            var param = (object)null;

            // Converts to propery mapped object
            if (where != null)
            {
                param = QueryGroup.AsMappedObject(new[] { where.MapTo<TEntity>() });
            }

            if (parameters != null)
            {
                param = parameters.AsObject(param);
            }

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(commandText, param, null);
                trace.BeforeQuery(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return null;
                }
                commandText = (cancellableTraceLog.Statement ?? commandText);
                param = (cancellableTraceLog.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = ExecuteQueryInternal<TEntity>(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                skipCommandArrayParametersCheck: true);

            // After Execution
            if (trace != null)
            {
                trace.AfterQuery(new TraceLog(commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Set Cache
            if (cacheKey != null)
            {
                cache?.Add(cacheKey, result, cacheItemExpiration, false);
            }

            // Result
            return result;
        }

        #endregion

        #region QueryAsyncInternalBase<TEntity>

        /// <summary>
        /// Queries a data from the database table function in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="funcName">Function name.</param>
        /// <param name="parameters">Collection of function parameters.</param>
        /// <param name="fields">The list of fields to be queried.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The top number of data to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force to query from the database.
        /// </param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        internal static async Task<IEnumerable<TEntity>> QueryTableFuncAsyncInternalBase<TEntity>(this IDbConnection connection,
            string funcName,
            IEnumerable<QueryField> parameters = null,
            IEnumerable<Field> fields = null,
            QueryGroup where = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Get Cache
            if (cacheKey != null)
            {
                var item = cache?.Get(cacheKey, false);
                if (item != null)
                {
                    return (IEnumerable<TEntity>)item.Value;
                }
            }

            // Variables
            var commandType = CommandType.Text;
            var request = new QueryTableFuncRequest(typeof(TEntity),
                funcName,
                connection,
                transaction,
                parameters?.GetFields(),
                fields,
                where,
                orderBy,
                top,
                hints,
                statementBuilder);
            var commandText = CommandTextCache.GetQueryTableFuncText(request);
            var param = (object)null;

            // Converts to propery mapped object
            if (where != null)
            {
                param = QueryGroup.AsMappedObject(new[] { where.MapTo<TEntity>() });
            }

            if (parameters != null)
            {
                param = parameters.AsObject(param);
            }

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(commandText, param, null);
                trace.BeforeQuery(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return null;
                }
                commandText = (cancellableTraceLog.Statement ?? commandText);
                param = (cancellableTraceLog.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = await ExecuteQueryAsyncInternal<TEntity>(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                skipCommandArrayParametersCheck: true);

            // After Execution
            if (trace != null)
            {
                trace.AfterQuery(new TraceLog(commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Set Cache
            if (cacheKey != null)
            {
                cache?.Add(cacheKey, result, cacheItemExpiration, false);
            }

            // Result
            return result;
        }

        #endregion
    }
}
