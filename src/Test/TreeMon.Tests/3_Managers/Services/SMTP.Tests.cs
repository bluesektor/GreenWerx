// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;

namespace TreeMon.Web.Tests._3_Managers.Services
{
    [TestClass]
    public class SMTPTests
    {
        [TestMethod]
        public void SMTP_VerifySettings()
        {
            //Check for supporting keys since it sends email
            Assert.IsNotNull(ConfigurationManager.AppSettings["SiteDomain"]);
            Assert.IsNotNull(ConfigurationManager.AppSettings["SiteEmail"]);
            Assert.IsNotNull(ConfigurationManager.AppSettings["AppKey"]);
            Assert.IsNotNull(ConfigurationManager.AppSettings["MailHost"]);
            Assert.IsNotNull(ConfigurationManager.AppSettings["MailPort"]);
            Assert.IsNotNull(ConfigurationManager.AppSettings["EmailHostUser"]);
            Assert.IsNotNull(ConfigurationManager.AppSettings["EmailHostPassword"]);
            Assert.IsNotNull(ConfigurationManager.AppSettings["UseSSL"]);
            Assert.IsNotNull(ConfigurationManager.AppSettings["TemplatePasswordResetEmail"]);

        }
    }
}
