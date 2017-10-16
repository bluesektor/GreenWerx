// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TreeMon.Utilites.Security;
using static TreeMon.Utilites.Security.PasswordHash;

namespace TreeMon.Web.Tests._1_TreeMon.Utilities.Security
{
    [TestClass]
    public class PasswordHashTests
    {
        [TestMethod]
        public void PasswordHash_CheckConstants()
        {
            Assert.AreEqual(PasswordHash.SALT_BYTE_SIZE, 24);
            Assert.AreEqual(PasswordHash.HASH_BYTE_SIZE, 24);
            Assert.AreEqual(PasswordHash.PBKDF2_ITERATIONS, 1000);
            Assert.AreEqual(PasswordHash.ITERATION_INDEX, 0);
            Assert.AreEqual(PasswordHash.SALT_INDEX, 1);
            Assert.AreEqual(PasswordHash.PBKDF2_INDEX, 2);
        }

        [TestMethod]
        public void PasswordHash_CheckStrength()
        {

            PasswordScore s = PasswordHash.CheckStrength("");
            Assert.AreEqual(s, PasswordScore.Blank);

            s = PasswordHash.CheckStrength("abc");
            Assert.AreEqual(s, PasswordScore.VeryWeak);

            s = PasswordHash.CheckStrength("abc");
            Assert.AreEqual(s, PasswordScore.VeryWeak);

            s = PasswordHash.CheckStrength("sdfttsae");
            Assert.AreEqual(s, PasswordScore.Weak);
            
            s = PasswordHash.CheckStrength("123456789ab");
            Assert.AreEqual(s, PasswordScore.Medium);


            s = PasswordHash.CheckStrength("123456789abcd");
            Assert.AreEqual(s, PasswordScore.Strong);

            s = PasswordHash.CheckStrength("#A23456789abcd");
            Assert.AreEqual(s, PasswordScore.VeryStrong);
        }

        [TestMethod]
        public void PasswordHash_CreateHash()
        {
            string pwd =  PasswordHash.CreateHash("password");
            Assert.AreNotEqual(pwd, "password");
        }

        [TestMethod]
        public void PasswordHash_ExtractHashPassword()
        {
            string pwd = PasswordHash.CreateHash("password");
            string hash = PasswordHash.ExtractHashPassword(pwd);
            Assert.AreNotEqual(pwd, hash);
            Assert.IsTrue(pwd.Contains( hash));
        }

        [TestMethod]
        public void PasswordHash_ExtractIterations()
        {
            string pwd = PasswordHash.CreateHash("password");
            int itr = PasswordHash.ExtractIterations(pwd);
            Assert.AreEqual(itr, 1000);
        }

        [TestMethod]
        public void PasswordHash_ExtractSalt()
        {
            string pwd = PasswordHash.CreateHash("password");
            string salt = PasswordHash.ExtractSalt(pwd);
            Assert.IsTrue(pwd.Contains(salt));
        }
        [TestMethod]
        public void PasswordHash_ValidatePassword()
        {
            string pwd = PasswordHash.CreateHash("password");
            Assert.IsTrue(PasswordHash.ValidatePassword("password", pwd));
            Assert.IsFalse(PasswordHash.ValidatePassword("abc", pwd));
            Assert.IsFalse(PasswordHash.ValidatePassword("password", "abc"));
            Assert.IsFalse(PasswordHash.ValidatePassword("password", "1:c"));
        }

        

    }
}
