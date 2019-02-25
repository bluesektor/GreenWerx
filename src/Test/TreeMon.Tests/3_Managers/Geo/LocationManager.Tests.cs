// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeMon.Managers.Geo;
using TreeMon.Models.Geo;
using TreeMon.Models.Membership;
using TreeMon.Utilites.Extensions;
using TreeMon.Web.Tests;

namespace TreeMon.Tests._3_Managers.Geo
{
    [TestClass]
    public   class LocationManager_Tests
    {
        private string connectionKey = "dev_treemon";
        private string sessionKey = "TESTSESSION";
        private string _ownerAuthToken = "";
       
        [TestInitialize]
        public void TestSetup()
        {
            _ownerAuthToken = TestHelper.InitializeControllerTestData(connectionKey, "OWNER").Result.ToString();
          
        }


        [TestMethod]
        public void LocationManager_Search()
        {
            // Search(string name, string locationType)
            LocationManager lm = new LocationManager(connectionKey, _ownerAuthToken);

            List<Location> locations = lm.Search("85395", "coordinate");

            Assert.IsTrue(locations.Count > 0);

            var coord = locations.FirstOrDefault();
            Assert.IsNotNull(coord.Latitude);
            Assert.IsNotNull(coord.Longitude);
        }

        [TestMethod]
        public void LocationManager_GetLocationsIn()
        {
            // Search(string name, string locationType)
            LocationManager lm = new LocationManager(connectionKey, _ownerAuthToken);

            GeoCoordinate result = lm.GetLocationsIn("85395", 25);
            //var group = result.Distances.GroupBy(g => g.Name);//.Where(g =>g.Count() > 1);
            Assert.IsTrue(result.Distances.Count > 0);
            //website found these 112
            //website missed 85326 
            //i missed 85039 but this is only a point on the map, maybe some type of business code
            string[] zips = "85326,85395,85340,85309,85355,85039,85392,85307,85329,85037,85338,85379,85335,85353,85305,85388,85363,85323,85345,85351,85372,85033,85303,85376,85380,85374,85396,85035,85378,85043,85301,85311,85312,85318,85381,85385,85375,85031,85302,85382,85019,85304,85339,85005,85017,85009,85306,85051,85387,85015,85308,85041,85007,85013,85021,85053,85029,85003,85070,85026,85001,85002,85010,85011,85030,85036,85038,85046,85060,85061,85062,85063,85064,85066,85067,85068,85069,85071,85072,85073,85074,85075,85076,85078,85079,85080,85082,85065,85012,85004,85014,85310,85361,85006,85023,85383,85373,85343,85045,85020,85083,85048,85027,85016,85022,85040,85042,85034,85028,85008,85018,85085,85032".Split(',');

            foreach (string code in zips)
            {
                bool found = false;
                foreach (var geo in result.Distances)
                {
                    if (result.Distances.Count(x => x.Name == geo.Name) > 1) 
                        break;


                        if (code == geo.Name)
                        {
                            found = true;
                            break;
                        }
             
                 
                }
                Assert.IsTrue(found);
            }
        }
    }
}
