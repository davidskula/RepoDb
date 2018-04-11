﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace RepoDb.Extensions
{
    public static class DataReaderExtension
    {
        internal static IEnumerable<T> AsEnumerable<T>(this IDataReader reader)
        {
            var properties = typeof(T).GetProperties().ToList();
            var dictionary = new Dictionary<int, PropertyInfo>();
            for (var i = 0; i < reader.FieldCount; i++)
            {
                var property = properties.FirstOrDefault(p => p.Name.ToLower() == reader.GetName(i).ToLower());
                if (property != null)
                {
                    dictionary.Add(i, property);
                }
            }
            var list = new List<T>();
            while (reader.Read())
            {
                var obj = Activator.CreateInstance<T>();
                foreach (var kvp in dictionary)
                {
                    var value = reader.IsDBNull(kvp.Key) ? null : reader[kvp.Key];
                    kvp.Value.SetValue(obj, value);
                }
                list.Add(obj);
            }
            return list;
        }

        internal static IEnumerable<object> AsObjects(this IDataReader reader)
        {
            var list = new List<object>();
            while (reader.Read())
            {
                var obj = new ExpandoObject() as IDictionary<string, object>;
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    var value = reader.IsDBNull(i) ? null : reader[i];
                    obj.Add(reader.GetName(i), value);
                }
                list.Add(obj);
            }
            return list;
        }
    }
}