// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
namespace TreeMon.Models.Flags
{
    public class SettingFlags
    {
        public struct Types
        {
            public const string String = "STRING";

            public const string EncryptedString = "STRING.ENCRYPTED";

            public const string Numeric = "INT";

            public const string Decimal = "DECIMAL";

            public const string DateTime = "DATETIME";

            public const string Boolean = "BOOL";
        }
    }
}
