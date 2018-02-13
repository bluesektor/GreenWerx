// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TreeMon.Models.Flags;
using TreeMon.Utilites.Security;

namespace TreeMon.Utilites.Extensions
{
    public static class StringEx
    {
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }

        public static string StripGuids(this string input, char delimiter)
        {
            if (!input.Contains(delimiter))
            {
                return input;
            }
        
            string[] tokens = input.Split(delimiter);

            Guid newGuid;
            foreach (string token in tokens)
            {
                if (string.IsNullOrWhiteSpace(token))
                    continue;

                if (Guid.TryParse(token, out newGuid))
                {
                   input = input.Replace(token, "");
                }
            }
            input = input.Replace("..", ".");
            return input;
        }

        public static T ConvertTo<T>(this string input )
        {
         
            object value = default(T);

            if (string.IsNullOrEmpty(input))
            {
               return (T)value;
            }

            try
            {
                string typeName = typeof(T).ToString();

                typeName = typeName.ToLower().Replace("system.", "");

                if (string.IsNullOrEmpty(input))
                {
                    return (T)value;
                }

                switch (typeName)
                {

                    case "string":
                        value = input;
                        break;
                    case "int16":
                        value = Convert.ToInt16(input);
                        break;
                    case "int32":
                            value = Convert.ToInt32(input);
                        break;
                    case "int64":
                        value = Convert.ToInt64(input);
                        break;
                    case "int":
                        value = Convert.ToInt32(input);
                        break;
                    case "double":
                        value = Convert.ToDouble(input);
                        break;
                    case "boolean":
                    case "bool":
                        if (input == "on" || input == "true" || input == "1"||  input == "+" )
                            value = true;
                        else
                            value = false;
                        break;
                    case "datetime":
                        value = Convert.ToDateTime(input);
                        break;
                    case "byte":
                        value = Convert.ToByte(input);
                        break;
                    case "sbyte":
                        value = Convert.ToSByte(input);
                        break;
                    case "char":
                        value = Convert.ToChar(input);
                        break;
                    case "decimal":
                        value = Convert.ToDecimal(input);
                        break;
                    case "single":
                    case "float":
                        value = Convert.ToSingle(input);
                        break;
                    case "uint":
                        value = Convert.ToUInt32(input);
                        break;
                    case "uint16":
                        value = Convert.ToUInt16(input);
                        break;
                    case "uint32":
                        value = Convert.ToUInt32(input);
                        break;
                    case "uint64":
                        value = Convert.ToUInt64(input);
                        break;
                    case "long":
                        value = Convert.ToInt64(input);
                        break;
                    case "ulong":
                        value = Convert.ToUInt64(input);
                        break;
                    case "short":
                        value = Convert.ToInt16(input);
                        break;
                    case "ushort":
                        value = Convert.ToUInt16(input);
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.Assert(false,ex.Message);
            }
            return (T)value;
        }

        public static T ConvertTo<T>(this string input, out bool succeeded )
        {
            succeeded = true;
            object value = default(T);

            if (string.IsNullOrEmpty(input))
            {
                succeeded = false;
                return (T)value;
            }

            try
            {
                string typeName = typeof(T).ToString();

                typeName = typeName.ToLower().Replace("system.", "");

                if (string.IsNullOrEmpty(input))
                {
                    return (T)value;
                }

                switch (typeName)
                {

                    case "string":
                        value = input;
                        break;
                    case "int16":
                        value = Convert.ToInt16(input);
                        break;
                    case "int32":
                        value = Convert.ToInt32(input);
                        break;
                    case "int64":
                        value = Convert.ToInt64(input);
                        break;
                    case "int":
                        value = Convert.ToInt32(input);
                        break;
                    case "double":
                        value = Convert.ToDouble(input);
                        break;
                    case "boolean":
                    case "bool":
                        if (input == "on" || input == "true" || input == "1" || input == "+")
                            value = true;
                        else
                            value = false;
                        break;
                    case "datetime":
                        value = Convert.ToDateTime(input);
                        break;
                    case "byte":
                        value = Convert.ToByte(input);
                        break;
                    case "sbyte":
                        value = Convert.ToSByte(input);
                        break;
                    case "char":
                        value = Convert.ToChar(input);
                        break;
                    case "decimal":
                        value = Convert.ToDecimal(input);
                        break;
                    case "single":
                    case "float":
                        value = Convert.ToSingle(input);
                        break;
                    case "uint":
                        value = Convert.ToUInt32(input);
                        break;
                    case "uint16":
                        value = Convert.ToUInt16(input);
                        break;
                    case "uint32":
                        value = Convert.ToUInt32(input);
                        break;
                    case "uint64":
                        value = Convert.ToUInt64(input);
                        break;
                    case "long":
                        value = Convert.ToInt64(input);
                        break;
                    case "ulong":
                        value = Convert.ToUInt64(input);
                        break;
                    case "short":
                        value = Convert.ToInt16(input);
                        break;
                    case "ushort":
                        value = Convert.ToUInt16(input);
                        break;
                }
            }
            catch (Exception ex)
            {
                succeeded = false;
                Debug.Assert(false, ex.Message);
            }
            return (T)value;
        }

        public static bool EqualsIgnoreCase(this string input, string data, bool currentCulture = true)
        {
            if (string.IsNullOrWhiteSpace(data) == true )
                return false;

            if (currentCulture)
                return  input.Equals(data, StringComparison.CurrentCultureIgnoreCase);

            return input.Equals(data, StringComparison.OrdinalIgnoreCase);
        }

        public static bool EqualsEx( string input, string data, bool ignoreCase = true, bool currentCulture = true)
        {
            try
            {
                if (ignoreCase && currentCulture)
                    return input.Equals(data, StringComparison.CurrentCultureIgnoreCase);


                if (ignoreCase == true && currentCulture == false)
                    return input.Equals(data, StringComparison.OrdinalIgnoreCase);

                if (ignoreCase == false && currentCulture == true)
                    return input.Equals(data, StringComparison.CurrentCulture);


                return input.Equals(data, StringComparison.Ordinal);
            }
            catch {
                return false;
            }
        }

        /// <summary>
        /// Removes all characters that are NOT alphanumeric.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToSafeString(this string input, bool removeWhiteSpace)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            char[] arr = input.ToCharArray();

            if(removeWhiteSpace)
                arr = Array.FindAll<char>(arr, (c => (  char.IsLetterOrDigit(c)))); 
            else
                arr = Array.FindAll<char>(arr, (c => (  char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)))); 

            string res = new string(arr);
            return res;
        }

        //alpha numeric id.
        public static string GenerateAlphaNumId()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                i *= ((int)b + 1);
            }
            return string.Format("{0:x}", i - DateTime.UtcNow.Ticks);
        }

        ///Checks for sql injection variables.
        //
        public static bool IsSQLSafeString(string data)
        {
            if (string.IsNullOrEmpty(data))
                return true;

            //Defines the set of characters that will be checked.
            //You can add to this list, or remove items from this list, as appropriate for your site
            string[] blackList = {  "|","&",";","$","%","'","\"","\\'","\\\"","<>",
                                     "()",")","+","0x0d","0x0a",",","\\","#","0x08",
                                     "eval(", "open(", "sysopen(", "system(",
                                     "--", ";--", ";", "/*", "*/", "@@", "@", "char ", "nchar",
                                     "varchar", "nvarchar", "alter ", "begin", " cast", " create",
                                     "cursor ", "declare ", "delete ", "drop", " end", " exec",
                                     "execute ", "fetch ", "insert", "kill", "open", "select ",
                                     "sys", "sysobjects", "syscolumns", "table ", "update " };

            for (int i = 0; i < blackList.Length; i++)
            {
                if ((data.IndexOf(blackList[i], StringComparison.OrdinalIgnoreCase) >= 0))
                    return false;
            }

            return true;
        }

        public static string GetValueType(string value)
        {
            if ( string.IsNullOrWhiteSpace(value))
                return string.Empty;
        
            bool btemp;
            if (bool.TryParse(value, out btemp))
                return SettingFlags.Types.Boolean;
           
            DateTime datemp;
            if (DateTime.TryParse(value, out datemp))
                return SettingFlags.Types.DateTime;

            if (value.Contains(".")) {
            
                    decimal dtmp;
                if (decimal.TryParse(value, out dtmp))
                    return SettingFlags.Types.Decimal;
            }

            int itmp;
            if (int.TryParse(value, out itmp))
                return SettingFlags.Types.Numeric;

            // SettingFlags.Types.EncryptedString todo figure out if way to detect encrypted string (would need key).

            return SettingFlags.Types.String;
        }

        /// <summary>
        /// This is used when validating the Setting object.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool ValueMatchesType(string value, string type)
        {
            if (string.IsNullOrWhiteSpace(type) || string.IsNullOrWhiteSpace(value))
                return false;

            switch (type.ToUpper())
            {
                case SettingFlags.Types.EncryptedString:
                case SettingFlags.Types.String:
                    return true;

                
                case SettingFlags.Types.Numeric: 
                    int itmp;
                    if (int.TryParse(value, out itmp))
                        return true;
                    break;

                case SettingFlags.Types.Decimal:
                    decimal dtmp;
                    if (decimal.TryParse(value, out dtmp))
                        return true;
                    break;

                case SettingFlags.Types.DateTime:
                    DateTime datemp;
                    if (DateTime.TryParse(value, out datemp))
                        return true;
                    break;
    
                case SettingFlags.Types.Boolean:
                    bool btemp;
                    if (bool.TryParse(value, out btemp))
                        return true;
                    break;
            }
            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="startToken"></param>
        /// <param name="endToken">Pass null or empty string to go to end of string.</param>
        /// <param name="data"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        public static string ReplaceIncluding(string startToken, string endToken, string data, string replacement)
        {
            int startIdx = data.IndexOf(startToken);

            if (startIdx < 0)
                return data;


            int endIdx = 0;
            //if no end token go to the end of the string.
            if (string.IsNullOrWhiteSpace(endToken))
                endIdx = data.Length;
            else
                endIdx = data.IndexOf(endToken, startIdx) + endToken.Length;

            if (endIdx < startIdx || endIdx - startIdx <= 0)
                return data;//end token wasnt found

            string substrToReplace = data.Substring(startIdx, endIdx - startIdx);

            data = data.Replace(substrToReplace, replacement);

            startIdx = data.IndexOf(startToken);

            if (startIdx < 0)
                return data;

            return ReplaceIncluding(startToken, endToken, data, replacement);//recurse through rest of string
        }


        public static byte[] StreamToByte(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

    }
}
