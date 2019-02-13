// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace TreeMon.Utilites.Helpers
{
    public class ObjectHelper
    {
        protected ObjectHelper() { }

        public static T DeepCopy<T>(T other)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, other);
                ms.Position = 0;
                return (T)formatter.Deserialize(ms);
            }
        }

        public static string GetPropertyValue(string propName, object o)
        {
            try
            {
                object res = o.GetType().GetProperty(propName).GetValue(o);
                if (res == null)
                    return string.Empty;

                return res.ToString();
             }
            catch (Exception ex)
            {
                return "ERROR:" + ex.Message;
            }
        }

        public static T Merge<T>(T target, T source)
        {
            try
            {
                typeof(T)
                .GetProperties()
                .Select((PropertyInfo x) => new KeyValuePair<PropertyInfo, object>(x, x.GetValue(source, null)))
                .Where((KeyValuePair<PropertyInfo, object> x) => x.Value != null).ToList()
                .ForEach((KeyValuePair<PropertyInfo, object> x) => x.Key.SetValue(target, x.Value, null));
            }
            catch {  Debug.Assert(false, "MERGE FAILED!"); }
            //return the modified copy of Target
            return target;
        }
    }
}
