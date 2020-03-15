using RepoDb.Extensions;
using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace RepoDb.Requests
{
    /// <summary>
    /// A class that holds the value of the query fable function operation arguments.
    /// </summary>
    internal class QueryAllTableFuncRequest : BaseRequest, IEquatable<QueryAllTableFuncRequest>
    {
        private int? m_hashCode = null;

        public QueryAllTableFuncRequest(Type type,
            string funcName,
            IDbConnection connection,
            IDbTransaction transaction,
            IEnumerable<Field> parameters = null,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            string hints = null,
            IStatementBuilder statementBuilder = null)
            : this(funcName,
                  connection,
                  transaction,
                  parameters,
                  fields,
                  orderBy,
                  hints,
                  statementBuilder)
        {
            Type = type;
        }

        public QueryAllTableFuncRequest(string funcName,
            IDbConnection connection,
            IDbTransaction transaction,
            IEnumerable<Field> parameters = null,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            string hints = null,
            IStatementBuilder statementBuilder = null)
            : base(funcName,
                  connection,
                  transaction,
                  statementBuilder)
        {
            Parameters = parameters?.AsList();
            Fields = fields?.AsList();
            OrderBy = orderBy?.AsList();
            Hints = hints;
        }

        /// <summary>
        /// Gets the list of the function parameters.
        /// </summary>
        public IEnumerable<Field> Parameters { get; }

        /// <summary>
        /// Gets the list of the target fields.
        /// </summary>
        public IEnumerable<Field> Fields { get; set; }

        /// <summary>
        /// Gets the list of the order fields.
        /// </summary>
        public IEnumerable<OrderField> OrderBy { get; }

        /// <summary>
        /// Gets the hints for the table.
        /// </summary>
        public string Hints { get; }

        #region Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <see cref="QueryAllTableFuncRequest"/>.
        /// </summary>
        /// <returns>The hashcode value.</returns>
        public override int GetHashCode()
        {
            // Make sure to return if it is already provided
            if (m_hashCode != null)
            {
                return m_hashCode.Value;
            }

            // Get first the entity hash code
            var hashCode = string.Concat(Name, ".Query").GetHashCode();

            // Get the qualifier <see cref="Field"/> objects
            if (Fields != null)
            {
                foreach (var field in Fields)
                {
                    hashCode += field.GetHashCode();
                }
            }

            if (Parameters != null)
            {
                foreach (var field in Parameters)
                {
                    hashCode += field.GetHashCode();
                }
            }

            // Add the order fields
            if (OrderBy != null)
            {
                foreach (var orderField in OrderBy)
                {
                    hashCode += orderField.GetHashCode();
                }
            }

            // Add the hints
            if (!string.IsNullOrEmpty(Hints))
            {
                hashCode += Hints.GetHashCode();
            }

            // Set and return the hashcode
            return (m_hashCode = hashCode).Value;
        }

        /// <summary>
        /// Compares the <see cref="QueryAllTableFuncRequest"/> object equality against the given target object.
        /// </summary>
        /// <param name="obj">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equals.</returns>
        public override bool Equals(object obj)
        {
            return obj?.GetHashCode() == GetHashCode();
        }

        /// <summary>
        /// Compares the <see cref="QueryAllTableFuncRequest"/> object equality against the given target object.
        /// </summary>
        /// <param name="other">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(QueryAllTableFuncRequest other)
        {
            return other?.GetHashCode() == GetHashCode();
        }

        /// <summary>
        /// Compares the equality of the two <see cref="QueryAllTableFuncRequest"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="QueryAllTableFuncRequest"/> object.</param>
        /// <param name="objB">The second <see cref="QueryAllTableFuncRequest"/> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(QueryAllTableFuncRequest objA, QueryAllTableFuncRequest objB)
        {
            if (ReferenceEquals(null, objA))
            {
                return ReferenceEquals(null, objB);
            }
            return objB?.GetHashCode() == objA.GetHashCode();
        }

        /// <summary>
        /// Compares the inequality of the two <see cref="QueryAllTableFuncRequest"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="QueryAllTableFuncRequest"/> object.</param>
        /// <param name="objB">The second <see cref="QueryAllTableFuncRequest"/> object.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(QueryAllTableFuncRequest objA, QueryAllTableFuncRequest objB)
        {
            return (objA == objB) == false;
        }

        #endregion
    }
}
