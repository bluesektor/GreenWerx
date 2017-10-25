// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TreeMon.Utilites.Security;

namespace TreeMon.Web.Tests._1_TreeMon.Utilities.Security
{
    [TestClass]
    public class CaptchaTests
    {
        [TestMethod]
        public void Captcha_IsValidCaptcha()
        {
            //Assert.IsTrue(Captcha.IsValidCaptcha("abc", "ABC"));
            //Assert.IsFalse(Captcha.IsValidCaptcha("abc", "xyz"));
        }

        [TestMethod]
        public void Captcha_RandomText()
        {
            //string code = Captcha.RandomText(5);
            //Assert.AreEqual(5, code.Length);
        }

    }
}
