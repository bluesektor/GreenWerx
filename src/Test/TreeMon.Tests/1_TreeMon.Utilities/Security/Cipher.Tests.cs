// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TreeMon.Utilites.Security;

namespace TreeMon.Web.Tests._1_TreeMon.Utilities.Security
{
    [TestClass]
    public class CipherTests
    {
        [TestMethod]
        public void Cipher_GenerateSalt()
        {
            string salt = Cipher.GenerateSalt();
            Assert.IsFalse(string.IsNullOrWhiteSpace(salt));
        }

        [TestMethod]
        public void Cipher_RandomString()
        {
            string res = Cipher.RandomString(5);
            Assert.AreEqual(5, res.Length);
        }

        [TestMethod]
        public void Cipher_Crypt()
        {
           string res = Cipher.Crypt("password", "encryptme", true);
           Assert.AreNotEqual("encryptme", res);
           res = Cipher.Crypt("password", res, false);
           Assert.AreEqual("encryptme", res);
        }

    }
}
