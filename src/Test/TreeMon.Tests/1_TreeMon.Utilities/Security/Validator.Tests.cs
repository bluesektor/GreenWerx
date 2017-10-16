// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TreeMon.Utilites.Security;

namespace TreeMon.Web.Tests._1_TreeMon.Utilities.Security
{
    [TestClass]
    public class ValidatorTests
    {
        //[TestMethod]
        //public void Validator_HasSqlCommand()
        //{
        //    Validator v = new Validator();
        //    Assert.IsTrue(v.HasSqlCommand("asdfasdf insert into"));
        //    Assert.IsTrue(v.HasSqlCommand("asdfasdf select *"));
        //    Assert.IsTrue(v.HasSqlCommand("asdfasdf select from"));
        //    Assert.IsTrue(v.HasSqlCommand("drop index asdfasdf "));
        //    Assert.IsTrue(v.HasSqlCommand("drop table asdfasdf "));
        //    Assert.IsTrue(v.HasSqlCommand("drop database asdfasdf "));
        //    Assert.IsTrue(v.HasSqlCommand("asdfasdf  delete from"));
        //    Assert.IsTrue(v.HasSqlCommand("asdfasdf  union all"));
        //    Assert.IsTrue(v.HasSqlCommand("asdfasdf  create database"));
        //    Assert.IsTrue(v.HasSqlCommand("create table asdfasdf "));
        //    Assert.IsTrue(v.HasSqlCommand(" asdfasdf  create index"));
        //    Assert.IsTrue(v.HasSqlCommand("create unique asdfasdf "));
        //    Assert.IsFalse(v.HasSqlCommand("createunique asdfasdf "));
        //}

        //[TestMethod]
        //public void Validator_HasBadTags()
        //{
        //    Assert.IsTrue(Validator.HasBadTags("asdfasdf <script>"));
        //    Assert.IsTrue(Validator.HasBadTags("asdfasdf select < applet >"));
        //    Assert.IsTrue(Validator.HasBadTags("asdfasdf select < body >"));
        //    Assert.IsTrue(Validator.HasBadTags("drop index < embed > "));
        //    Assert.IsTrue(Validator.HasBadTags("drop table < frame > "));
        //    Assert.IsTrue(Validator.HasBadTags("drop database < frameset >"));
        //    Assert.IsTrue(Validator.HasBadTags("asdfasdf  delete < html >"));
        //    Assert.IsTrue(Validator.HasBadTags("asdfasdf  union < iframe >"));
        //    Assert.IsTrue(Validator.HasBadTags("asdfasdf  create < img >"));
        //    Assert.IsTrue(Validator.HasBadTags("create table < style > "));
        //    Assert.IsTrue(Validator.HasBadTags(" asdfasdf  create < layer >"));
        //    Assert.IsTrue(Validator.HasBadTags("createunique < link > "));
        //    Assert.IsTrue(Validator.HasBadTags("createunique < ilayer > "));
        //    Assert.IsTrue(Validator.HasBadTags("createunique < meta > "));
        //    Assert.IsTrue(Validator.HasBadTags("createunique < object > "));
        //    Assert.IsFalse(Validator.HasBadTags("createunique asdfasdf "));
        //}

        //[TestMethod]
        //public void Validator_HasCodeInjection()
        //{
        //    string[] codeSites = "chopapp.com, hastebin.com,tny.cz,snipt.org,pastie.org,privatepaste.com,reviewboard.org,copypastecode.com,aspin.com,zubrag.com,snipt.net,darkcoding.net,codelifter.com,gonet.biz,dpaste.com,codepad.org,friendpaste.com,codepaste.net,slexy.org".Split(',');
           
        //    foreach (string codeSite in codeSites)
        //    {
        //      Assert.IsTrue(  Validator.HasCodeInjection("abcdesasd" + codeSite + "xys"),"failed on:" + codeSite);
        //    }

        //}

        //[TestMethod]
        //public void Validator_HasReservedLoginName()
        //{
        //    string[] logins = "admin,guest,mod,m0d,owner,0wner,account,job,help,billing,press,spam,sales,support,supp0rt,service,manager,director,president,ceo,editor,email,test,secure,root,NULL,operator,webmaster,backup,demo,test,trial,member,private member,private,moderator,m0derator".Split(',');

        //    foreach (string login in logins) {
        //      Assert.IsTrue(   Validator.HasReservedLoginName(login + "@domain.com"));
        //    }
        //}

        //[TestMethod]
        //public void Validator_IsEmailInjectionAttempt()
        //{
        //    string[] resWords = "to:,bcc:,cc:,),[,[:space:],],*,:,multipart, subject:".Split(',');

        //    foreach (string word in resWords) {
        //        Assert.IsTrue(Validator.IsEmailInjectionAttempt(word + "myemail@test.com"));
        //    }
           

        //}
        //[TestMethod]
        //public void Validator_IsValidEmailFormat()
        //{
        //    Assert.IsTrue(Validator.IsValidEmailFormat("test@com"));
        //    Assert.IsTrue(Validator.IsValidEmailFormat("test@test.com"));
        //    Assert.IsFalse(Validator.IsValidEmailFormat("test@@test.com"));
        //    Assert.IsFalse(Validator.IsValidEmailFormat("test@test..com"));
        //    Assert.IsFalse(Validator.IsValidEmailFormat("@testtest.com"));
           
        //}
    }
}
