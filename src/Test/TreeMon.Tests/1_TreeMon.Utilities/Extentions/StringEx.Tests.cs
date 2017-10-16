// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TreeMon.Utilites.Extensions;

namespace TreeMon.Web.Tests._1_TreeMon.Utilities.Extentions
{
    [TestClass]
    public class StringExTests
    {
        //[TestMethod]
        //public void StringEx_StripGuids_Test()
        //{
        //    string g = "ABCD*" + Guid.NewGuid().ToString() + "*EFG";
        //    g = StringEx.StripGuids(g, '*');
        //    Assert.IsFalse(g.Contains("-")); //make sure no guid is in string
        //    Assert.AreEqual("ABCD**EFG", g); 
        //}

        //[TestMethod]
        //public void StringEx_ConvertTo_staticFunc_Test()
        //{
        //    Assert.AreEqual(StringEx.ConvertTo<string>("test"), "test");
        //    Assert.AreEqual(StringEx.ConvertTo<Int16>("16"), 16);
        //    Assert.AreEqual(StringEx.ConvertTo<Int32>("32"), 32);
        //    Assert.AreEqual(StringEx.ConvertTo<Int64>("64"), 64);
        //    Assert.AreEqual(StringEx.ConvertTo<int>("2"), 2);
        //    Assert.AreEqual(StringEx.ConvertTo<double>("1.2"), 1.2);
        //    Assert.IsTrue(StringEx.ConvertTo<Boolean>("on"));
        //    Assert.IsTrue(StringEx.ConvertTo<Boolean>("true"));
        //    Assert.IsTrue(StringEx.ConvertTo<Boolean>("1"));
        //    Assert.IsTrue(StringEx.ConvertTo<Boolean>("+"));
        //    Assert.IsTrue(StringEx.ConvertTo<bool>("on"));
        //    Assert.IsTrue(StringEx.ConvertTo<bool>("true"));
        //    Assert.IsTrue(StringEx.ConvertTo<bool>("1"));
        //    Assert.IsTrue(StringEx.ConvertTo<bool>("+"));
        //    DateTime dt = StringEx.ConvertTo<DateTime>("12/23/1985");
        //    Assert.AreEqual(12, dt.Month);
        //    Assert.AreEqual(23, dt.Day);
        //    Assert.AreEqual(1985, dt.Year);
        //    Assert.AreEqual(101, StringEx.ConvertTo<byte>("101"));
        //    Assert.AreEqual(8, StringEx.ConvertTo<sbyte>("8"));
        //    Assert.AreEqual('|', StringEx.ConvertTo<char>("|"));
        //    Assert.AreEqual((decimal)123.45, StringEx.ConvertTo<decimal>("123.45"));
        //    Assert.AreEqual((Single)123.449996948242, StringEx.ConvertTo<Single>("123.45"));
        //    Assert.AreEqual((float)123.449996948242, StringEx.ConvertTo<float>("123.45"));
        //    Assert.AreEqual((uint)8, StringEx.ConvertTo<uint>("8"));
        //    Assert.AreEqual((UInt16)16, StringEx.ConvertTo<UInt16>("16"));
        //    Assert.AreEqual((UInt32)32, StringEx.ConvertTo<UInt32>("32"));
        //    Assert.AreEqual((long)123456789123, StringEx.ConvertTo<long>("123456789123"));
        //    Assert.AreEqual((ulong)123456789123, StringEx.ConvertTo<ulong>("123456789123"));
        //    Assert.AreEqual((short)1234, StringEx.ConvertTo<short>("1234"));
        //    Assert.AreEqual((ushort)1234, StringEx.ConvertTo<ushort>("1234"));
        //}

        //[TestMethod]
        //public void StringEx_ConvertTo_extension_Test()
        //{
        //    Assert.AreEqual( "test", "test".ConvertTo<string>());
        //    Assert.AreEqual(16, "16".ConvertTo<Int16>());
        //    Assert.AreEqual("32".ConvertTo<Int32>(), 32);
        //    Assert.AreEqual("64".ConvertTo<Int64>(), 64);
        //    Assert.AreEqual("2".ConvertTo<int>(), 2);

        //    Assert.AreEqual("1.2".ConvertTo<double>(), 1.2);
        //    Assert.IsTrue("on".ConvertTo<Boolean>());
        //    Assert.IsTrue("true".ConvertTo<Boolean>());
        //    Assert.IsTrue("1".ConvertTo<Boolean>());
        //    Assert.IsTrue("+".ConvertTo<Boolean>());
        //    Assert.IsTrue("on".ConvertTo<bool>());
        //    Assert.IsTrue("true".ConvertTo<bool>());
        //    Assert.IsTrue("1".ConvertTo<bool>());
        //    Assert.IsTrue("+".ConvertTo<bool>());
        //    DateTime dt = "12/23/1985".ConvertTo<DateTime>();
        //    Assert.AreEqual(12, dt.Month);
        //    Assert.AreEqual(23, dt.Day);
        //    Assert.AreEqual(1985, dt.Year);
        //    Assert.AreEqual(101, "101".ConvertTo<byte>());
        //    Assert.AreEqual(8, "8".ConvertTo<sbyte>());
        //    Assert.AreEqual('|', "|".ConvertTo<char>());
        //    Assert.AreEqual((decimal)123.45, "123.45".ConvertTo<decimal>());
        //    Assert.AreEqual((Single)123.449996948242, "123.45".ConvertTo<Single>());
        //    Assert.AreEqual((float)123.449996948242, "123.45".ConvertTo<float>());
        //    Assert.AreEqual((uint)8, "8".ConvertTo<uint>());
        //    Assert.AreEqual((UInt16)16, "16".ConvertTo<UInt16>());
        //    Assert.AreEqual((UInt32)32, "32".ConvertTo<UInt32>());
        //    Assert.AreEqual((long)123456789123, "123456789123".ConvertTo<long>());
        //    Assert.AreEqual((ulong)123456789123, "123456789123".ConvertTo<ulong>());
        //    Assert.AreEqual((short)1234, "1234".ConvertTo<short>());
        //    Assert.AreEqual((ushort)1234, "1234".ConvertTo<ushort>());
        //}

        //[TestMethod]
        //public void StringEx_ToSafeString()
        //{
        //    string safe = "asd$asdf#".ToSafeString();
        //    Assert.IsFalse(safe.Contains("$"));
        //    Assert.IsFalse(safe.Contains("#"));

        //}

        //[TestMethod]
        //public void StringEx_GenerateAlphaNumId()
        //{
        //    string id = StringEx.GenerateAlphaNumId();
        //    Assert.IsFalse(string.IsNullOrWhiteSpace(id));
        //    Assert.IsFalse(id.Contains("#"));//backlog test for more special chars
        //}

        //[TestMethod]
        //public void StringEx_IsSQLSafeString()
        //{
        //        Assert.IsFalse(StringEx.IsSQLSafeString("|"         ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString("&"         ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString(";"         ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString("$"         ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString("%"         ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString("'"         ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString("\""        ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString("\\'"       ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString("\\\""      ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString("<>"        ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString("()"        ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString(")"         ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString("+"         ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString("0x0d"      ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString("0x0a"      ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString(","         ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString("\\"        ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString("#"         ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString("0x08"      ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString(   "eval("  ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString( "open("    ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString( "sysopen(" ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString( "system("  ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString(   "--"     ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString( ";--"      ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString( ";"        ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString( "/*"       ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString( "*/"       ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString( "@@"       ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString( "@"        ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString( "char "    ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString( "nchar"    ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString("varchar"   ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString( "nvarchar" ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString( "alter "   ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString( "begin"    ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString( " cast"    ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString( " create"  ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString( "cursor "  ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString( "declare " ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString( "delete "  ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString( "drop"     ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString( " end"     ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString( " exec"    ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString( "execute " ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString( "fetch "   ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString( "insert"   ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString( "kill"     ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString( "open"     ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString( "select "  ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString(   "sys"    ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString( "sysobjects"));
        //        Assert.IsFalse(StringEx.IsSQLSafeString( "syscolumns"));
        //        Assert.IsFalse(StringEx.IsSQLSafeString( "table "   ));
        //        Assert.IsFalse(StringEx.IsSQLSafeString("update "));
        //    Assert.IsTrue(StringEx.IsSQLSafeString("test the string 1234"));
        //}

        //[TestMethod]
        //public void StringEx_ValueMatchesType()
        //{
        //    Assert.IsFalse(StringEx.ValueMatchesType(string.Empty,"STRING"));
        //    Assert.IsFalse(StringEx.ValueMatchesType("  ","STRING"));
        //    Assert.IsTrue(StringEx.ValueMatchesType("abc", "STRING.ENCRYPTED.SYSTEM"));
        //    Assert.IsTrue(StringEx.ValueMatchesType("abc", "STRING"));
        //    Assert.IsTrue(StringEx.ValueMatchesType("123", "NUMERIC"));
        //    Assert.IsTrue(StringEx.ValueMatchesType("123", "INT"));
        //    Assert.IsTrue(StringEx.ValueMatchesType("123.25", "DECIMAL"));
        //    Assert.IsTrue(StringEx.ValueMatchesType("12/3/1985", "DATE TIME"));
        //    Assert.IsTrue(StringEx.ValueMatchesType("12/3/1986", "DATETIME"));
        //    Assert.IsTrue(StringEx.ValueMatchesType("true", "TRUE/FALSE"));
        //    Assert.IsTrue(StringEx.ValueMatchesType("false", "TRUE/FALSE"));
        //}

        //[TestMethod]
        //public void StringEx_ReplaceIncluding()
        //{
        //    string s = "abc1xyz2ghi";
        //    s = StringEx.ReplaceIncluding("1", "2", s, "def");
        //    Assert.AreEqual("abcdefghi", s);
        //}
    }
}
