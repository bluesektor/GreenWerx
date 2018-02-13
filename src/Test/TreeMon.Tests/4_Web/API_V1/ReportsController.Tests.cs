// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using TreeMon.Data;
using TreeMon.Data.Logging.Models;
using TreeMon.Managers;
using TreeMon.Managers.Membership;
using TreeMon.Managers.Store;
using TreeMon.Models.App;
using TreeMon.Models.Datasets;
using TreeMon.Models.Logging;
using TreeMon.Models.Membership;
using TreeMon.Models.Store;

namespace TreeMon.Web.Tests.API.V1
{

    [TestClass]
    public class ReportsControllerTests
    {
        private readonly string connectionKey = "MSSQL_TEST";
        private string _ownerAuthToken = "";

        [TestInitialize]
        public void TestSetup()
        {
            ServiceResult res = TestHelper.InitializeControllerTestData(connectionKey,  "OWNER");
            Assert.AreNotEqual(res.Code, 500, res.Message);//should not be error. if it is assert.
            _ownerAuthToken = res.Result.ToString();
            Assert.IsNotNull(ConfigurationManager.AppSettings["DefaultDbConnection"]);
            Random rand = new Random();
            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            for(int i = 0; i < 100; i++)
            {
                MeasurementLog m = new MeasurementLog()
                {
                    AccountUUID = SystemFlag.Default.Account,
                    Name = "name-" + i.ToString(),
                    DateTaken = DateTime.UtcNow.AddMonths(rand.Next(0, 12) * -1),
                    Measure = (float)rand.Next(0, 100),
                    MeasureType = "numeric",
                    UnitOfMeasure = "inches"
                };
                context.Insert<MeasurementLog>(m);
            }

            #region from DatasetManager.Tests.cs
            //TreeMonDbContext context = new TreeMonDbContext(connectionKey);

            //Random rand = new Random();
            ////Seed AuthenticationLog
            //for (int i = 0; i < 100; i++)
            //{
            //    context.Insert<AuthenticationLog>(new AuthenticationLog()
            //    {
            //        Authenticated = i % 3 == 0 ? false : true,
            //        AuthenticationDate = DateTime.UtcNow.AddMonths(rand.Next(0, 12) * -1),
            //        UserName = "alpha"
            //    });
            //}

            //context.Insert<AuthenticationLog>(new AuthenticationLog()
            //{
            //    Authenticated = false,
            //    AuthenticationDate = DateTime.UtcNow.AddMonths(-1),
            //    UserName = "beta_fail_login"
            //});

            ////Seed Measurements
            //for (int i = 0; i < 100; i++)
            //{
            //    context.Insert<MeasurementLog>(new MeasurementLog()
            //    {
            //        DateTaken = DateTime.UtcNow.AddMonths(rand.Next(0, 12) * -1),
            //        Measure = rand.Next(0, 100),
            //        UnitOfMeasure = "centimeters"
            //    });
            //}
            #endregion
        }

        ////category (aka type ) : the table being accessed. 
        ////field : table field
        ////resultFormat :  scalar, series
        ////
        //[TestMethod]
        //public void Api_ReportsController_GetDataset_Distinct()
        //{
        //    TreeMonDbContext context = new TreeMonDbContext(connectionKey);

        //    Task.Run(async () =>
        //    {
        //        #region filters
        //        ////the getfilters function gets distinct values from a table field that will
        //        ////used to build the query to get the dataset. Controler only pulling from fireincidents for now. need to make it dynamic.
        //        List<QueryFilter> filters = new List<QueryFilter>();
        //        QueryFilter qf = new QueryFilter();
        //        qf.Field = "name";
        //        qf.Operator = "distinct";
        //        //qf.Value = "0";
        //        qf.Type = "sql";//tell the parser is a sql query
        //        //qf.Junction = "AND"; //since more filters are to follow add a conjunction.
        //        filters.Add(qf);

        //        //#region Date range query filter
        //        //qf = new QueryFilter();
        //        //qf.Field = "AuthenticationDate";
        //        //qf.Operator = "BETWEEN";
        //        //qf.Type = "sql";//tell the parser is a sql query
        //        //qf.Value = DateTime.UtcNow.AddMonths(-1).AddDays(-1).ToShortDateString();
        //        //qf.Order = 0;//this is the first part of the between statement
        //        //qf.Junction = "AND";
        //        //filters.Add(qf);

        //        //qf = new QueryFilter();
        //        //qf.Field = "AuthenticationDate";
        //        //qf.Operator = "BETWEEN";
        //        //qf.Type = "sql";//tell the parser is a sql query
        //        //qf.Value = DateTime.UtcNow.AddMonths(-1).AddDays(1).ToShortDateString();
        //        //qf.Order = 1;//second part of the between statement
        //        //qf.Junction = "AND";
        //        //filters.Add(qf);
        //        //#endregion

        //        //qf = new QueryFilter();
        //        //qf.Field = "UserName";
        //        //qf.Operator = "=";
        //        //qf.Type = "sql";//tell the parser is a sql query
        //        //qf.Value = "beta_fail_login";
        //        //filters.Add(qf);
        //        #endregion

        //        string postData = JsonConvert.SerializeObject(filters);

        //        //See DatasetManager.Tests.cs for filters
        //        ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Reports/MeasurementLog/Dataset/name", postData, _ownerAuthToken);

        //       Assert.IsNotNull(res);
        //       Assert.AreEqual(res.Code, 200);

        //       List<DataPoint> dataset = JsonConvert.DeserializeObject<List<DataPoint>>(res.Result.ToString());
        //        Assert.IsNotNull(dataset);
        //        Assert.IsTrue(dataset.Count > 0);

        //    }).GetAwaiter().GetResult();
        //}

        //[TestMethod]
        //public void Api_ReportsController_GetDataset_Series_By_UOM_Between_Values()
        //{
        //    ////the getfilters function gets distinct values from a table field that will
        //    ////used to build the query to get the dataset. Controler only pulling from fireincidents for now. need to make it dynamic.
        //    List<QueryFilter> filters = new List<QueryFilter>();
        //    QueryFilter qf = new QueryFilter();
        //    qf.Field = "UnitOfMeasure";
        //    qf.Operator = "=";
        //    qf.Value = "inches";
        //    qf.Type = "sql";//tell the parser is a sql query
        //    qf.Junction = "AND"; //since more filters are to follow add a conjunction.
        //    filters.Add(qf);

        //    #region range query filter
        //    qf = new QueryFilter();
        //    qf.Field = "Measure";
        //    qf.Operator = "BETWEEN";
        //    qf.Type = "sql";//tell the parser is a sql query
        //    qf.Value = "20";
        //    qf.Order = 0;//this is the first part of the between statement
        //    qf.Junction = "AND";
        //    filters.Add(qf);

        //    qf = new QueryFilter();
        //    qf.Field = "Measure";
        //    qf.Operator = "BETWEEN";
        //    qf.Type = "sql";//tell the parser is a sql query
        //    qf.Value = "50";
        //    qf.Order = 1;//second part of the between statement
        //    filters.Add(qf);
        //    #endregion


        //    //See DatasetManager.Tests.cs for filters
        //    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //    string postData = JsonConvert.SerializeObject(filters);
           
        //    Task.Run(async () =>
        //    {
               
        //        ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Reports/MeasurementLog/Dataset/measure", postData, _ownerAuthToken);

        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);

        //        List<DataPoint> dataset = JsonConvert.DeserializeObject<List<DataPoint>>(res.Result.ToString());
        //        Assert.IsNotNull(dataset);
        //        Assert.IsTrue(dataset.Count > 0);


        //    }).GetAwaiter().GetResult();
        //}


    }
}
