// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TreeMon.Models.Datasets;
using TreeMon.Data;
using TreeMon.Managers.DataSets;
using TreeMon.Models.Logging;
using System.Collections.Generic;

namespace TreeMon.Web.Tests.Managers
{
    [TestClass]
    public class DatasetManagerTests
    {
        private string connectionKey = "MSSQL_TEST";

        [TestInitialize]
        public void TestSetup()
        {
            TreeMonDbContext context = new TreeMonDbContext(connectionKey);

            Random rand = new Random();
            //Seed AuthenticationLog
            for (int i = 0; i < 100; i++)
            {
                context.Insert<AuthenticationLog>(new AuthenticationLog()
                {
                    Authenticated = i % 3 == 0 ? false : true,
                    AuthenticationDate = DateTime.UtcNow.AddMonths(rand.Next(0, 12) * -1),
                    UserName = "alpha",
                    DateCreated = DateTime.Now
                });
            }

            context.Insert<AuthenticationLog>(new AuthenticationLog()
            {
                Authenticated = false,
                AuthenticationDate = DateTime.UtcNow.AddMonths(-1),
                UserName = "beta_fail_login",
                DateCreated = DateTime.Now
            });

            //Seed Measurements
            for (int i = 0; i < 100; i++)
            {
                context.Insert<MeasurementLog>(new MeasurementLog()
                {
                    DateTaken = DateTime.UtcNow.AddMonths(rand.Next(0, 12) * -1),
                    Measure = rand.Next(0, 100),
                    UnitOfMeasure = "centimeters",
                    DateCreated = DateTime.Now
                });
            }
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        //backlog depricate
        ////The DatasetManager helps retrieve data from any table. It's
        ////primary use is to create report data/graphs. Still pretty basic.
        //[TestMethod]
        //public void DatasetManager_GetScalarData_AuthenticationLog_Username_Field()
        //{
        //    //the getfilters function gets distinct values from a table field that will
        //    //used to build the query to get the dataset. Controler only pulling from fireincidents for now. need to make it dynamic.
        //    List<DataScreen> filters = new List<DataScreen>();
        //    DataScreen qf = new DataScreen();
        //    filters.Add(qf);


        //    //List<DataScreen> filters = JsonConvert.DeserializeObject<List<DataScreen>>(body);

        //    DatasetManager dm = new DatasetManager(new TreeMonDbContext(connectionKey));
        //    DataPoint dp = dm.GetScalarData("AuthenticationLog", "UserName", null);//get usernames in AuthenticationLog

        //    Assert.AreEqual(dp.ValueType.ToUpper(), "STRING");
        //    Assert.AreEqual(dp.Value.ToUpper(), "ALPHA");

        //}

        //backlog depricate
        ////The DatasetManager helps retrieve data from any table. It's
        ////primary use is to create report data/graphs. Still pretty basic.
        //[TestMethod]
        //public void DatasetManager_GetScalarData_AuthenticationLog_Username_Field_FailedLogin()
        //{
        //    //the getfilters function gets distinct values from a table field that will
        //    //used to build the query to get the dataset. Controler only pulling from fireincidents for now. need to make it dynamic.
        //    List<DataScreen> filters = new List<DataScreen>();
        //    DataScreen qf = new DataScreen();
        //    qf.Field = "Authenticated";
        //    qf.Operator = "=";
        //    qf.Value = "0";
        //    qf.Type = "sql";//tell the parser is a sql query
        //    qf.Junction = "AND"; //since more filters are to follow add a conjunction.
        //    filters.Add(qf);

        //    #region Date range query filter
        //    qf = new DataScreen();
        //    qf.Field = "AuthenticationDate";
        //    qf.Operator = "BETWEEN";
        //    qf.Type = "sql";//tell the parser is a sql query
        //    qf.Value = DateTime.UtcNow.AddMonths(-1).AddDays(-1).ToShortDateString();
        //    qf.Order = 0;//this is the first part of the between statement
        //    qf.Junction = "AND";
        //    filters.Add(qf);

        //    qf = new DataScreen();
        //    qf.Field = "AuthenticationDate";
        //    qf.Operator = "BETWEEN";
        //    qf.Type = "sql";//tell the parser is a sql query
        //    qf.Value = DateTime.UtcNow.AddMonths(-1).AddDays(1).ToShortDateString();
        //    qf.Order = 1;//second part of the between statement
        //    qf.Junction = "AND"; 
        //    filters.Add(qf);
        //    #endregion

        //    qf = new DataScreen();
        //    qf.Field = "UserName";
        //    qf.Operator = "=";
        //    qf.Type = "sql";//tell the parser is a sql query
        //    qf.Value = "beta_fail_login";
        //    filters.Add(qf);

        //    DatasetManager dm = new DatasetManager(new TreeMonDbContext(connectionKey));
        //    DataPoint dp = dm.GetScalarData("AuthenticationLog", "UserName", filters);//get usernames in AuthenticationLog

        //    Assert.AreEqual(dp.ValueType.ToUpper(), "STRING");
        //    Assert.AreEqual(dp.Value, "beta_fail_login");

        //}


        [TestMethod]
        public void DatasetManager_GetData_Measurements()
        {
            DatasetManager dm = new DatasetManager(connectionKey, "TESTSESSION");
            List<DataPoint> series = dm.GetData("MeasurementLog", "Measure", null);

            Assert.IsTrue(series.Count > 0);
        }

        [TestMethod]
        public void DatasetManager_GetData_Measurements_DataScreens()
        {
            //the getfilters function gets distinct values from a table field that will
            //used to build the query to get the dataset. Controler only pulling from fireincidents for now. need to make it dynamic.
            List<DataScreen> filters = new List<DataScreen>();
            DataScreen qf = new DataScreen();
            qf.Field = "UnitOfMeasure";
            qf.Operator = "=";
            qf.Value = "centimeters";
            qf.Type = "sql";//tell the parser is a sql query
            qf.Junction = "AND"; //since more filters are to follow add a conjunction.
            filters.Add(qf);

            qf = new DataScreen();
            qf.Field = "Measure";
            qf.Operator = "BETWEEN";
            qf.Type = "sql";//tell the parser is a sql query
            qf.Value = "20";
            qf.Order = 0;//this is the first part of the between statement
            qf.Junction = "AND";
            filters.Add(qf);

            qf = new DataScreen();
            qf.Field = "Measure";
            qf.Operator = "BETWEEN";
            qf.Type = "sql";//tell the parser is a sql query
            qf.Value = "50";
            qf.Order = 1;//second part of the between statement
            filters.Add(qf);

            DatasetManager dm = new DatasetManager(connectionKey,"TESTSESSION");
            List<DataPoint> series = dm.GetData("MeasurementLog", "Measure", null);

            Assert.IsTrue(series.Count > 0);
        }
    }

    #region Some Implementation examples from the client and stuff.....

    #region api calls to controller
    /*
     /// //  dataArray += "," + JSON.stringify({ Caption: 'order unit by value', Field: 'unit', Operator: 'order', Value: 'sunday,monday,tuesday,wednesday,thursday,friday,saturday', Order: 0, Type: 'custom' })
        /// : '/api/Reports/fire/Dataset/ssss/Return/series',
        /// //'/api/Reports/fire.heatmap.distribution/Dataset/<DBFieldsHere?/Return/coordinates',
        /// 
        /// 
        ///   dataArray += "," + JSON.stringify({ Caption: 'order unit by value', Field: 'unit', Operator: 'order', Value: '0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23', Order: 0, Type: 'custom' })
        ///      url: '/api/Reports/fire.field.count/Dataset/hour_alm_dttm/Return/series',
     */
    #endregion

    #region Javascript GetFilters function
    /*
     *   var dataArray = "";

            if ($('#chkFilterCancelled').prop("checked")) {
                dataArray = JSON.stringify({ Caption: 'cancelled', Field: 'cancelled', Operator: '=', Value: 0, Order: 0, Type: 'sql', Junction: 'AND' });
            }

            if ($('#chkFirstResponderOnly').prop("checked")) {
                dataArray += "," + JSON.stringify({ Caption: 'chkFirstResponderOnly', Field: 'resp', Operator: 'first', Value: 0, Order: 0, Junction: '', Type: 'linq' });
            }
   
            var dateStart = $('#dateStart').val();
            var dateEnd =  $('#dateEnd').val();
        
            if (dateStart.length >= 8  ) {

                if (dateEnd.length === 0 )
                {
                    dateEnd = Today();
                    $('#dateEnd').val(dateEnd);
                }

                dataArray +=","+ JSON.stringify({ Caption: 'dateStart', Field: 'alm_dttm', Operator: 'BETWEEN', Value: dateStart, Order: 0, Junction: 'AND', Type: 'sql' });
                dataArray += "," +JSON.stringify({ Caption: 'dateEnd', Field: 'alm_dttm', Operator: 'BETWEEN', Value: dateEnd, Order: 1, Junction: 'AND', Type: 'sql' }) ;

            }

            var $selectedFilters = null;
            $selectedUsers = $('#lstFilters').jtable('selectedRows');
            var countDown = $selectedUsers.length;

            //Create a DataScreen json array.
            //Notice the junction for this is an OR.
            $selectedUsers.each(function () {
                var DataScreen = $(this).data('record');
        
                countDown--;
                if (countDown > 0) {
                    dataArray +="," + JSON.stringify({ Caption: DataScreen.Caption, Field: DataScreen.Field, Operator: DataScreen.Operator, Value: DataScreen.Value, Order: DataScreen.Order, Type: 'sql', Junction: 'OR' });
                } else if (countDown == 0) {//last one
                    dataArray +=","+ JSON.stringify({ Caption: DataScreen.Caption, Field: DataScreen.Field, Operator: DataScreen.Operator, Value: DataScreen.Value, Order: DataScreen.Order, Type: 'sql', Junction: 'AND' });
                }
            });
    
            if ($('#txtTimeMin').val().length > 0) {
                dataArray += "," + JSON.stringify({ Caption: 'txtResponseTimeMin', Field: 'resp', Operator: '>', Value: $('#txtResponseTimeMin').val(), Order: 0, Type: 'sql', Junction: 'AND' });
            }

            if ($('#txtResponseTimeMax').val().length > 0) {
                dataArray +=  "," +JSON.stringify({ Caption: 'txtResponseTimeMax', Field: 'resp', Operator: '<', Value: $('#txtResponseTimeMax').val(), Order: 1, Type: 'sql', Junction: 'AND' });
            }

    
            var unit = $("#cboUnitFilter option:selected").val();

            //this was intentionally put last so the final or will be removed when building the query on the server.
            if ( unit.length > 0 ){ //$('#txtFilterUnits').val().length > 0) {

                dataArray += "," + JSON.stringify({ Caption: 'unit', Field: '( unit', Operator: 'LIKE', Value: unit + '%' + '\')', Order: 0, Type: 'sql', Junction: '' });
                //var units = $('#txtFilterUnits').val().split(',');
                //for (var i = 0; i < units.length; i++) {
                //    if (i == 0 && units.length > 1) {//add start paren.
                //        dataArray += JSON.stringify({ Caption: 'unit', Field: '( unit', Operator: 'LIKE', Value: units[i] + '%', Order: 0, Type: 'sql', Junction: 'OR' }) + ",";
                //    }
                //    else if (i == units.length - 1) {  //add end paren. and no end comma
                //        dataArray += JSON.stringify({ Caption: 'unit', Field: 'unit', Operator: 'LIKE', Value: units[i] + "%'  )", Order: 0, Type: 'sql', Junction: 'OR' });
                //    }
                //    else {
                //        dataArray += JSON.stringify({ Caption: 'unit', Field: 'unit', Operator: 'LIKE', Value: units[i] + '%', Order: 0, Type: 'sql', Junction: 'OR' }) + ",";
                //    }
                //}
              
         }

     */
    #endregion
    #endregion
}
