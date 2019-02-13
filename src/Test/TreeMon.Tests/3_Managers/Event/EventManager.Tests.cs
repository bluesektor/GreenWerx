// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Profile;
using TreeMon.Data.Logging.Models;
using TreeMon.Managers.Events;
using TreeMon.Managers.Geo;
using TreeMon.Managers.Membership;
using TreeMon.Models.Geo;
using TreeMon.Models.Membership;
using TreeMon.Web.Tests;
using TreeMon.Models.Events;
using TreeMon.Utilites.Extensions;
using System.Globalization;
using TreeMon.Models.App;

namespace TreeMon.Tests._3_Managers.Event
{
    [TestClass]
    public class EventManagerTests
    {
        Rootobject _ionicEvent = new Rootobject();
        private string _connectionKey = "dev_treemon";
        private string _ownerAuthToken = "";
        List<User> users = new List<User>();
        List<Profile> profiles = new List<Profile>();
        List<Location> locations = new List<Location>();
        RoleManager _roleManager;
        string _roleUuidForSpeaker = "";

        [TestInitialize]
        public void TestSetup()
        {
            //  delete FROM [TreemonSystemTest].[dbo].[EventMembers]
            //  delete  FROM [TreemonSystemTest].[dbo].[EventLocations]
            //  delete  FROM [TreemonSystemTest].[dbo].[EventGroups]
            //  delete   FROM [TreemonSystemTest].[dbo].[Events]
            //  delete   FROM [TreemonSystemTest].[dbo].[EventInventory]

            _ownerAuthToken = TestHelper.InitializeControllerTestData(_connectionKey, "OWNER").Result.ToString();
            string directory = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug", "");
            string seedFile  = Path.Combine(directory, "App_Data\\SeedData\\Events.Ionic.TestData.json");

            _roleManager = new RoleManager(_connectionKey);

            if (!File.Exists(seedFile))
            {
                Debug.Assert(false, "Events.Ionic.TestData.json FILE IS MISSING");
                return;
            }
            string eventData = File.ReadAllText(seedFile);

         
            Role role = _roleManager.Search("speaker").FirstOrDefault();
            if (role == null)
            {
                role = new Role() { Name = "speaker", AccountUUID = SystemFlag.Default.Account, AppType = "web", Persists = true };
                role.UUID = Guid.NewGuid().ToString("N");
                role.UUParentID = "";
                role.UUParentIDType = "Role";
                role.UUIDType = "Role";
                role.DateCreated = DateTime.UtcNow;
                role.StartDate = DateTime.UtcNow;
                role.EndDate = DateTime.UtcNow;
                _roleManager.Insert(role);
              
            }
            _roleUuidForSpeaker = role.UUID;
           
            _ionicEvent = JsonConvert.DeserializeObject<Rootobject>(eventData);
            AddLocations(); //EventLocation todo
            AddUsers(); //EventMember   todo
        }

        [TestMethod]
        public void EventManager_Insert_Ionic()
        {
            ServiceResult result = new ServiceResult();
            EventManager eventManager = new EventManager(_connectionKey, _ownerAuthToken);

            TreeMon.Models.Events.Event mainEvent = new Models.Events.Event()
            {
                Name = "Ionic Conference",
                Category = "Development",
                Body = "Lorem Ipsum Dolor Sit Amet Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos.Phasellus nec rutrum metus.Vivamus sed quam quam.",
                //EventDateTime = DateTime.toDate("2018-09-01 08:00", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                //StartDate = DateTime.toDate("2018-09-01 08:00", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                //EndDate = DateTime.toDate("2018-09-01 24:00", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                EventDateTime = "2018-09-01 08:00".toDate() ?? DateTime.Now,
                StartDate = "2018-09-01 08:00".toDate() ?? DateTime.Now,
                EndDate = "2018-09-01 24:00".toDate()??DateTime.Now,
                Private = false,
                DateCreated = DateTime.Now,
                CreatedBy = SystemFlag.Default.Account,
                AccountUUID = SystemFlag.Default.Account,
                Active = true
            };

            result =  eventManager.Insert(mainEvent);
            //if (result.Code != 200)
            //    Debug.Assert(false, "insert failed");

            foreach (var sched in _ionicEvent.schedule)
            {
                // sched.date
                foreach (var grp in sched.groups)
                {
                    EventGroup evtGroup = new EventGroup()
                    {
                        Category = "Session",
                        StartDate = mainEvent.StartDate,
                        Private = false,
                        UUParentID = mainEvent.UUID,
                        UUParentIDType = mainEvent.UUIDType,
                        DateCreated = DateTime.Now,
                        CreatedBy = SystemFlag.Default.Account,
                        AccountUUID = SystemFlag.Default.Account,
                        Active = true
                         
                    };
                    string[] time = grp.time.Split(':');
                    TimeSpan ts = new TimeSpan(time[0].ConvertTo<int>(), time[0].Split(' ')[0].ConvertTo<int>(), 0);
                 
                    evtGroup.StartDate = evtGroup.StartDate + ts;
                    evtGroup.EndDate = evtGroup.StartDate.AddHours(1);
                    result = eventManager.InsertEventGroup(evtGroup);
                    if (result.Code != 200)
                        Debug.Assert(false, "insert failed");

                    foreach (var ses in grp.sessions)
                    {
                        //ses.location
                        // ses.speakerNames
                       
                        TreeMon.Models.Events.Event e = new TreeMon.Models.Events.Event()
                        {
                            Name = ses.name,
                            Body = ses.description,
                            Category = ses.tracks.FirstOrDefault(),
                            EventDateTime = DateTime.Parse(sched.date),
                            StartDate = mainEvent.StartDate,
                            EndDate = mainEvent.EndDate,
                            Private  = false,
                            UUParentID  = mainEvent.UUID,
                            UUParentIDType  = mainEvent.UUIDType,
                            DateCreated = DateTime.Now,
                            CreatedBy = SystemFlag.Default.Account,
                            AccountUUID = SystemFlag.Default.Account,
                            Active = true,
                        };

                         time = ses.timeStart.Split(':');
                         ts = new TimeSpan(time[0].ConvertTo<int>(), time[0].Split(' ')[0].ConvertTo<int>(), 0);

                        e.StartDate = e.StartDate + ts;

                        time = ses.timeEnd.Split(':');
                        ts = new TimeSpan(time[0].ConvertTo<int>(), time[0].Split(' ')[0].ConvertTo<int>(), 0);
                        e.EndDate = e.EndDate + ts;

                       result=    eventManager.Insert(e);
                        //if (result.Code != 200)
                        //    Debug.Assert(false, "insert failed");

                        if (ses.speakerNames == null)
                            continue;

                        foreach (string speakerName in ses.speakerNames)
                        {
                            var user = users.FirstOrDefault(w => w.Name == speakerName);
                            EventMember em = new EventMember()
                            {
                                EventUUID = e.UUID,
                                UserUUID = user.UUID,
                                Name = user.Name,
                                Image = user.Image
                            };
                           result = eventManager.InsertEventMember(em);
                            //if (result.Code != 200)
                            //    Debug.Assert(false, "insert failed");
                        }
                        Random rnd = new Random();
                        int r = rnd.Next(locations.Count);
                        //todo add locations 
                        // ses.location
                        EventLocation l = new EventLocation()
                        {
                            Name = ses.location,
                            EventUUID = e.UUID,
                           // LocationUUID  = locations[r].UUID,//couldn't see what rooms were tied to what building so picking a random building
                            DateCreated = DateTime.Now,
                            CreatedBy = SystemFlag.Default.Account,
                            AccountUUID = SystemFlag.Default.Account,
                            Active = true,
                        };
                        eventManager.InsertEventLocation(l);
                    }
                }

            }
        }

        [TestMethod]
        public void EventManager_FixEventIds()
        {
            ServiceResult result = new ServiceResult();
            EventManager eventManager = new EventManager(_connectionKey, _ownerAuthToken);

            var events = eventManager.GetEvents();

            var groups = eventManager.GetEventGroups();
             Random rnd = new Random();
           
            foreach (var group in groups)
            {
                 int r = rnd.Next(events.Count);
                group.EventUUID = events[r].UUID;
                if (eventManager.UpdateGroup(group).Code != 200)
                    Debug.Assert(false, "FAILED TO UPDATE GROUP");

            }
            //Event groups have wrong EventUUIDs
            //load all events.
            //load all event groups 

            //loop through event groups and get random event uuid
            //update event group EventUUID
           

        }



        //[TestMethod]
        //public void EventManager_GetEvent()
        //{
        //    EventManager nm = new EventManager(connectionKey);
        //    ServiceResult sr = nm.Insert(new Event
        //    {
        //        Name = "TEST_Event",
        //        UUID = Guid.NewGuid().ToString("N"),
        //        UUIDType = "Event",
        //        AccountUUID = "a"
        //    }, false);
        //    Event n = nm.GetEvent("TEST_Event");
        //    Assert.IsNotNull(n);
        //    n = nm.GetEventBy(n.UUID);
        //    Assert.IsNotNull(n);
        //    Assert.IsTrue(nm.GetEvents(n.AccountUUID).Count > 0);
        //}
        //[TestMethod]
        //public void EventManager_UpdateEvent()
        //{
        //    EventManager nm = new EventManager(connectionKey);
        //    Event n = nm.GetEvent("TEST_Event");
        //    n.Name = "Event_UPDATED";
        //    Assert.AreEqual(nm.UpdateEvent(n).Code, 200);
        //    Event nu = nm.GetEvent("Event_UPDATED");
        //    Assert.IsNotNull(nu);
        //}
        //[TestMethod]
        //public void EventManager_DeleteEvent()
        //{
        //    EventManager nm = new EventManager(connectionKey);
        //    ServiceResult sr = nm.Insert(new Event
        //    {
        //        Name = "DELETE_Event",
        //        UUID = Guid.NewGuid().ToString("N"),
        //        UUIDType = "Event",
        //        AccountUUID = "a"
        //    }, false);
        //    Event n = nm.GetEvent("DELETE_Event");
        //    Assert.IsTrue(nm.DeleteEvent(n) > 0);
        //    Assert.IsTrue(nm.DeleteEvent(n,true) > 0);
        //}

        //loop through map array and add location
        //
        private void AddLocations()
        {
            //EventManager eventManager = new EventManager(_connectionKey, _ownerAuthToken);
            LocationManager lm = new LocationManager(_connectionKey, _ownerAuthToken);

            foreach (var l in _ionicEvent.map)
            {

                var tmpLocation = new Location()
                {
                    Name = l.name,
                    Latitude = l.lat,
                    Longitude = l.lng,
                    LocationType = "coordinate",
                    RoleOperation = ">=",
                    RoleWeight = 1,
                    DateCreated = DateTime.Now,
                    CreatedBy = SystemFlag.Default.Account,
                    AccountUUID = SystemFlag.Default.Account,
                    Active = true
                };
               ServiceResult res =  lm.Insert(tmpLocation);
                if (res.Code != 200)
                    Debug.Assert(false, res.Message);

                locations.Add(tmpLocation);


            }
        }

        private void AddUsers()
        {
            UserManager um = new UserManager(_connectionKey, _ownerAuthToken);
            foreach (var l in _ionicEvent.speakers)
            {
                var user = new User()
                {
                    Name = l.name,
                    Image = l.profilePic,
                    Email = l.email,
                    RoleOperation = ">=",
                    RoleWeight = 1,
                    DateCreated = DateTime.Now,
                    CreatedBy = SystemFlag.Default.Account,
                    AccountUUID = SystemFlag.Default.Account,
                    Active = true
                };
                um.Insert(user);
                users.Add(user);

                User testUser = TestHelper.GenerateTestUser("testuser"); //this creates the same user in memory for testing.
                
                _roleManager.AddUserToRole(_roleUuidForSpeaker, user, testUser);

                Profile p = new Profile()
                {
                    Location = l.location,
                    Name = l.name,
                    UserUUID = user.UUID,
                    Image = l.profilePic,
                    RoleOperation = ">=",
                    RoleWeight = 1,
                    DateCreated = DateTime.Now,
                    CreatedBy = SystemFlag.Default.Account,
                    AccountUUID = SystemFlag.Default.Account,
                    Active = true
                };
                um.InsertProfile(p);
                profiles.Add(p);
            }
        }


    }


    public class Rootobject
    {
        public Schedule[] schedule { get; set; }
        public Speaker[] speakers { get; set; }
        public Map[] map { get; set; }
    }

    public class Schedule
    {
        public string date { get; set; }
        public Group[] groups { get; set; }
    }

    public class Group
    {
        public string time { get; set; }
        public Session[] sessions { get; set; }
    }

    public class Session
    {
        public string name { get; set; }
        public string timeStart { get; set; }
        public string timeEnd { get; set; }
        public string location { get; set; }
        public string[] tracks { get; set; }
        public string id { get; set; }
        public string description { get; set; }
        public string[] speakerNames { get; set; }
    }

    public class Speaker
    {
        public string name { get; set; }
        public string profilePic { get; set; }
        public string twitter { get; set; }
        public string about { get; set; }
        public string location { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string id { get; set; }
    }

    public class Map
    {
        public string name { get; set; }
        public float lat { get; set; }
        public float lng { get; set; }
        public bool center { get; set; }
    }

}
