// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using TreeMon.Data;
using TreeMon.Data.Logging.Models;
using TreeMon.Models;
using TreeMon.Models.Geo;
using TreeMon.Utilites.Extensions;
using TreeMon.Utilites.Helpers;


//https://www.census.gov/geo/maps-data/data/gazetteer2015.html
//ZIP Code Tabulation Areas
//Column Label      Description
//Column 1	        GEOID Five digit ZIP Code Tabulation Area Census Code
//Column 2	        ALAND Land Area(square meters) - Created for statistical purposes only
//Column 3	        AWATER Water Area(square meters) - Created for statistical purposes only
//Column 4	        ALAND_SQMI Land Area(square miles) - Created for statistical purposes only
//Column 5	        AWATER_SQMI Water Area(square miles) - Created for statistical purposes only
//Column 6	        INTPTLAT Latitude(decimal degrees) First character is blank or "-" denoting North or South latitude respectively
//Column 7	        INTPTLONG Longitude(decimal degrees) First character is blank or "-" denoting East or West longitude respectively
namespace TreeMon.Managers.Geo
{
    public class PostalCodeManager : LocationManager
    {
        protected int _apiRequestsPerDay = 2500;
        protected int _apiRequestsPerSecond = 10;
        private const double MaxDistance = 100.0;

        public PostalCodeManager(string connectionKey, string sessionKey) : base(connectionKey, sessionKey)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(connectionKey), "LocationManager CONTEXT IS NULL!");
            this._connectionKey = connectionKey;
            this.ZipCodeManager = new ZipCodes();
        }



        //private Location SaveCooridnate(string postalCode )
        //{
        //    var coordinate = GetCoordinate(postalCode);

        //    if (coordinate != null && coordinate.Latitude != null && coordinate.Longitude != null)
        //        return coordinate;

        //    var locationLatLong = this.GetAll()?.FirstOrDefault(x => x.Name.EqualsIgnoreCase(postalCode) && x.Latitude != null && x.Longitude != null);

        //    if (locationLatLong != null)
        //    {
        //        //todo add as coordinate
        //        return locationLatLong;
        //    }

        //    //todo Get lat and long 


        //    //then update or insert
        //    var location = this.GetAll()?.FirstOrDefault(x => x.Name.EqualsIgnoreCase(postalCode));

        //    if (location == null)
        //    {   //add  as coordinate

        //    }


        //    if (ConfigurationManager.AppSettings["google.maps.requestperday"] != null)
        //        _apiRequestsPerDay = ConfigurationManager.AppSettings["google.maps.requestperday"].ToString().ConvertTo<int>();

        //    if (ConfigurationManager.AppSettings["google.maps.requestpersecond"] != null)
        //        _apiRequestsPerSecond = ConfigurationManager.AppSettings["google.maps.requestpersecond"].ToString().ConvertTo<int>();

        //    // if (geos.Count > _apiRequestsPerDay)
        //    //      geos = geos.Take(_apiRequestsPerDay).ToList();

        //    long totalElapsedTime = 0;
        //    int millisecondCap = 1000; //or 1 second.
        //    //If we go below this time on all the requests then we'll go over the throttle limit. 
        //    int minRequesTimeThreshold = millisecondCap / _apiRequestsPerSecond;
        //    Stopwatch stopwatch = new Stopwatch();

        //    int index = 1;

        //    var address = this.GetFullAddress(location);  //location.Address1 + " " + location.City + " " + location.State + " " + location.Postal;
        //                                                  //location.Name = address;

        //    // jade key
        //    //var requestUri = string.Format("https://maps.googleapis.com/maps/api/geocode/xml?key=YOURGOOGLEAPIKEY&address={0}&sensor=false", Uri.EscapeDataString(address));
        //    //steve o gmail key
        //    var requestUri = string.Format("https://maps.googleapis.com/maps/api/geocode/xml?key=YOURGOOGLEAPIKEY&address={0}&sensor=false", Uri.EscapeDataString(address));
        //    stopwatch.Restart(); // Begin timing.
        //    var request = WebRequest.Create(requestUri);
        //    var response = request.GetResponse();
        //    var xdoc = XDocument.Load(response.GetResponseStream());
        //    // var xdoc = XDocument.Parse(_googleGeoXml); //test parse
        //    var status = xdoc.Element("GeocodeResponse").Element("status");

        //    switch (status.Value)
        //    {
        //        case GoogleGeo.ResponseStatus.OK:
        //            // location.DescriptionEx = xdoc.ToString();
        //            var result = xdoc.Element("GeocodeResponse").Element("result");
        //            var locationElement = result.Element("geometry").Element("location");
        //            location.Latitude = locationElement.Element("lat").Value.ConvertTo<float>();
        //            location.Longitude = locationElement.Element("lng").Value.ConvertTo<float>();

        //            //DataQuery dqFC = new DataQuery();
        //            // dqFC.SQL = string.Format("UPDATE FireDeptIncidents SET Latitude={0}, Longitude={1} WHERE inci_id='{2}'", location.Latitude, location.Longitude, location.inci_id);
        //            // int res = fim.Update(dqFC);


        //            // SetProgressBar(index, "updated:" + address);
        //            break;

        //        case GoogleGeo.ResponseStatus.OverLimit:
        //            //SetProgressBar(-1, "Status: OverLimit");
        //            //todo log this
        //            return null;

        //        case GoogleGeo.ResponseStatus.Denied:
        //            //todo log this
        //            // SetProgressBar(-1, "Status: Denied");
        //            //SetProgressBar(0, xdoc.ToString());
        //            return null;
        //    }

        //    // Stop timing.
        //    stopwatch.Stop();
        //    long elapsedTime = stopwatch.ElapsedMilliseconds;//How long it took to get and process the response.

        //    if (elapsedTime < minRequesTimeThreshold)
        //    {
        //        //SetProgressBar(-1, "suspending for:" + (minRequesTimeThreshold - (int)elapsedTime).ToString());
        //        Thread.Sleep(minRequesTimeThreshold - (int)elapsedTime);//sleep is in milliseconds
        //        totalElapsedTime += elapsedTime;

        //        // millisecond =   .001 or 10−3 or 1 / 1000
        //        //so 1 request every 100 milliseconds
        //    }
        //}

        //public void ImportZipCodes(string pathToFile) {
        //    ZipCodes codes = LoadZipCodeCoordinates(pathToFile);

        //    foreach (int zipCode in codes.Keys)
        //    {
        //        ZipCode loc = codes[zipCode];
        //        SaveCooridnate(loc.Code.ToString(), loc.State, loc.Latitude, loc.Longitude);
   
        //    }
        //    return;
        //}

        private Location SaveCooridnate(string postalCode, string state, double latitude, double longitude)
        {
            var coordinate = GetCoordinate(postalCode);

            if (coordinate != null && coordinate.Latitude == latitude && coordinate.Longitude == longitude)
                return coordinate;

            var loc = new Location();
            loc.Name = postalCode;
            loc.State = state;
            loc.Postal = postalCode;
            loc.Latitude = latitude;
            loc.Longitude = longitude;
            loc.LocationType = "coordinate";
            loc.RoleOperation = ">=";
            loc.RoleWeight = 1;
           // loc.IpNumStart = locations[locId].StartIpNum;
           // loc.IpNumEnd = locations[locId].EndIpNum;
            loc.DateCreated = DateTime.Now;
            loc.CreatedBy = SystemFlag.Default.Account;
            loc.AccountUUID = SystemFlag.Default.Account;
            loc.Active = true;

            using (var context = new TreeMonDbContext(this._connectionKey))
            { context.Insert<Location>(loc); }
            return loc;
        }

        /// <summary>
        /// This builds a full address from an incident.
        /// This is to standardize the return because the address hash needs exactly the same
        /// address to be computed.
        /// </summary>
        /// <param name="fi"></param>
        /// <returns></returns>
        /*   public string GetFullAddress(Incident fi)
           {
               string res = "";

               // fi..Trim() + " " + fi..Trim() + " " + fi.st_type.Trim() + " " + fi.st_suffix.Trim();

               res = fi.number.ToString();

               if (!string.IsNullOrWhiteSpace(fi.st_prefix))
                   res += " " + fi.st_prefix.Trim();



               if (!string.IsNullOrWhiteSpace(fi.street))
                   res += " " + fi.street.Trim();

               if (!string.IsNullOrWhiteSpace(fi.city))
                   res += " " + fi.city.Trim();


               res += " Arizona";

               if (!string.IsNullOrWhiteSpace(fi.zip))
                   res += " " + fi.zip.Trim();

               res += " United States";

               return res;
           }

           private void UpdateCooridnates()
        {
            //Addresses in sqlite
            //TreeMon.Data.TreeMonDbContext appData = new TreeMon.Data.TreeMonDbContext("SQLITE");
            //List<GeoLocation> geos = appData.GetAll<GeoLocation>()?.Where(glw => glw.Latitude == 0 && glw.Longitude == 0).ToList(); //lat = 0 and long = 0 is prime meridian.. nothing there

            //Addresses in Datasets sql

            List<Location> geos = this.GetLocations(""); //fim.GetIncidents(false)?.Where(fiw => fiw.Latitude <= 0).GroupBy(x => x.inci_id).Select(y => y.First()).ToList();

            if (ConfigurationManager.AppSettings["google.maps.requestperday"] != null)
                _apiRequestsPerDay = ConfigurationManager.AppSettings["google.maps.requestperday"].ToString().ConvertTo<int>();

            if (ConfigurationManager.AppSettings["google.maps.requestpersecond"] != null)
                _apiRequestsPerSecond = ConfigurationManager.AppSettings["google.maps.requestpersecond"].ToString().ConvertTo<int>();

            if (geos.Count() > _apiRequestsPerDay)
                geos = geos.Take(_apiRequestsPerDay).ToList();

            long totalElapsedTime = 0;
            int millisecondCap = 1000; //or 1 second.
            //If we go below this time on all the requests then we'll go over the throttle limit. 
            int minRequesTimeThreshold = millisecondCap / _apiRequestsPerSecond;
            Stopwatch stopwatch = new Stopwatch();

            int index = 1;

            //    foreach (GeoLocation location in geos)
            foreach (Location location in geos)
            {
                var address = this.GetFullAddress(location);  //location.Address1 + " " + location.City + " " + location.State + " " + location.Postal;
                                                             //location.Name = address;

                
                //var requestUri = string.Format("https://maps.googleapis.com/maps/api/geocode/xml?key=YOURGOOGLEAPIKEY&address={0}&sensor=false", Uri.EscapeDataString(address));
                
                var requestUri = string.Format("https://maps.googleapis.com/maps/api/geocode/xml?key=YOURGOOGLEAPIKEY&address={0}&sensor=false", Uri.EscapeDataString(address));
                stopwatch.Restart(); // Begin timing.
                var request = WebRequest.Create(requestUri);
                var response = request.GetResponse();
                var xdoc = XDocument.Load(response.GetResponseStream());
                // var xdoc = XDocument.Parse(_googleGeoXml); //test parse
                var status = xdoc.Element("GeocodeResponse").Element("status");

                switch (status.Value)
                {
                    case GoogleGeo.ResponseStatus.OK:
                        // location.DescriptionEx = xdoc.ToString();
                        var result = xdoc.Element("GeocodeResponse").Element("result");
                        var locationElement = result.Element("geometry").Element("location");
                        location.Latitude = locationElement.Element("lat").Value.ConvertTo<float>();
                        location.Longitude = locationElement.Element("lng").Value.ConvertTo<float>();

                        //DataQuery dqFC = new DataQuery();
                       // dqFC.SQL = string.Format("UPDATE FireDeptIncidents SET Latitude={0}, Longitude={1} WHERE inci_id='{2}'", location.Latitude, location.Longitude, location.inci_id);
                       // int res = fim.Update(dqFC);

                      
                       // SetProgressBar(index, "updated:" + address);
                        break;

                    case GoogleGeo.ResponseStatus.OverLimit:
                        //SetProgressBar(-1, "Status: OverLimit");
                        //todo log this
                        return;

                    case GoogleGeo.ResponseStatus.Denied:
                        //todo log this
                       // SetProgressBar(-1, "Status: Denied");
                        //SetProgressBar(0, xdoc.ToString());
                        return;
                }

                // Stop timing.
                stopwatch.Stop();
                long elapsedTime = stopwatch.ElapsedMilliseconds;//How long it took to get and process the response.

                if (elapsedTime < minRequesTimeThreshold)
                {
                    //SetProgressBar(-1, "suspending for:" + (minRequesTimeThreshold - (int)elapsedTime).ToString());
                    Thread.Sleep(minRequesTimeThreshold - (int)elapsedTime);//sleep is in milliseconds
                    totalElapsedTime += elapsedTime;

                    // millisecond =   .001 or 10−3 or 1 / 1000
                    //so 1 request every 100 milliseconds
                }

                index++;
            }

        }

           */


        #region zip code distance calculator
        public GeoCoordinate GetZipsIn(string zipCode, double distance)
        {
            GeoCoordinate result = new GeoCoordinate();// assign the found zip codes to the cache. then
            if (distance > MaxDistance)
                return result;

            Location loc = this.Search(zipCode, "coordinate")?.FirstOrDefault();
            if (loc == null)
                return result;

 

            return result;
            //Get the data for the zipCode arg.
            //ZipCode zip = new ZipCode()
            //{
            //    Latitude = loc.Latitude ?? 0,
            //    Longitude = loc.Longitude ?? 0,
            //    UUID = loc.UUID,
            //    Code = loc.Name
            //};

            // MathHelper.Distance(
            /*
               if (distance > MaxDistance)
                 {
                     throw new ArgumentOutOfRangeException("distance",
                         string.Format("Must be less than {0}.", MaxDistance));
                 }

                 IEnumerable<ZipCodeDistance> codes1 = null;
                 if (startingZipCode.DistanceCache == null)
                 {
                     // grab all less than the MaxDistance in first pass
                     codes1 = from c in this.Values
                              let d = c - startingZipCode
                              where (d <= MaxDistance)
                              orderby d
                              select new ZipCodeDistance() { ZipCode = c, Distance = d };
                     // this might just be temporary storage depending on caching settings
                     startingZipCode.DistanceCache = codes1;
                 }
                 else
                 {
                     // grab the cached copy
                     codes1 = startingZipCode.DistanceCache;
                 }
                 List<ZipCodeDistance> filtered = new List<ZipCodeDistance>();

                 foreach (ZipCodeDistance zcd in codes1)
                 {
                     // since the list is pre-sorted, we can now drop out 
                     // quickly and efficiently as soon as something doesn't
                     // match
                     if (zcd.Distance > distance)
                     {
                         break;
                     }
                     filtered.Add(zcd);
                 }

                 // if no caching, don't leave the cached result in place
                 if (!IsCaching) { startingZipCode.DistanceCache = null; }
                 return filtered;
             */

           // return this.ZipCodeManager.FindLessThanDistance(zip, distance).ToList();

            
                  // Console.WriteLine("Find all zips < 25 miles from 13126:");
                  // var distanced = codes.FindLessThanDistance(codes[13126], 25);
        }

        //todo load zipcodes in constructor? async. make static? 
        protected ZipCodes ZipCodeManager { get; }
        #region implementation
        // ZipCodes codes = ZipCodeReader.ReadZipCodes(reader);
        // codes.IsCaching = true;

        // Console.WriteLine("From 90210 to 73487 in miles: {0:0.##}", 
        //    codes[90210].Distance(codes[73487], Measurement.Miles));
        // Console.WriteLine("Find all zips < 25 miles from 13126:");
        // var distanced = codes.FindLessThanDistance(codes[13126], 25);
        #endregion
        public enum Measurement
        {
            Miles,
            Kilometers
        }

        public class ZipCode
        {
            public string UUID { get; set; }
            private double _cosLatitude = 0.0;
            private double _latitutde;
            private IEnumerable<ZipCodeDistance> _cachedZipDistance;
            /// <summary>
            /// Two-digit state code
            /// </summary>
            public string State { get; set; }

            /// <summary>
            /// 5 digit Postal Code
            /// </summary>
           // public int Code { get; set; }

            public string Code { get; set; }

            /// <summary>
            /// Latitude, in Radians
            /// </summary>
            public double Latitude
            {
                get { return _latitutde; }
                set
                {
                    _latitutde = value;
                    _cosLatitude = Math.Cos(value);
                }
            }

            /// <summary>
            /// Precomputed value of the Cosine of Latitutde
            /// </summary>
            private double CosineOfLatitutde
            {
                get { return _cosLatitude; }
            }

            /// <summary>
            /// Longitude, in Radians
            /// </summary>
            public double Longitude { get; set; }

            public double Distance(ZipCode compare)
            {
                return Distance(compare, Measurement.Miles);
            }

            /// <summary>
            /// Computes the distance between two zip codes using the Haversine formula
            /// (http://en.wikipedia.org/wiki/Haversine_formula).
            /// </summary>
            /// <param name="compare"></param>
            /// <param name="m"></param>
            /// <returns></returns>
            public double Distance(ZipCode compare, Measurement m)
            {
                double dLon = compare.Longitude - this.Longitude;
                double dLat = compare.Latitude - this.Latitude;

                double a = Math.Pow(Math.Sin(dLat / 2.0), 2) +
                        this.CosineOfLatitutde *
                        compare.CosineOfLatitutde *
                        Math.Pow(Math.Sin(dLon / 2.0), 2.0);

                double c = 2 * Math.Asin(Math.Min(1.0, Math.Sqrt(a)));
                double d = (m == Measurement.Miles ? 3956 : 6367) * c;

                return d;
            }

            public static double operator -(ZipCode z1, ZipCode z2)
            {
                if (z1 == null || z2 == null) { throw new ArgumentNullException(); }
                return z1.Distance(z2);
            }

            public static double ToRadians(double d)
            {
                return (d / 180) * Math.PI;
            }

            internal IEnumerable<ZipCodeDistance> DistanceCache
            {
                get
                {
                    return _cachedZipDistance;
                }
                set
                {
                    _cachedZipDistance = value;
                }
            }
        }

        public class ZipCodeDistance
        {
            public ZipCode ZipCode { get; set; }
            public double Distance { get; set; }
        }

        public class ZipCodes : Dictionary<string,ZipCode>  //<int, ZipCode>
        {
            private const double MaxDistance = 100.0;

            /// <summary>
            /// Gets and sets whether the ZipCodes class caches all search results.
            /// </summary>
            public bool IsCaching { get; set; }
            /// <summary>
            /// Find all Zip Codes less than a specified distance
            /// </summary>
            /// <param name="startingZipCode">Provide the starting zip code as an object</param>
            /// <param name="distance">Maximum distance from starting zip code</param>
            /// <returns>List of ZipCodeDistance objects, sorted by distance.</returns>
            public IEnumerable<ZipCodeDistance> FindLessThanDistance(ZipCode startingZipCode, double distance)
            {
                if (distance > MaxDistance)
                {
                    throw new ArgumentOutOfRangeException("distance",
                        string.Format("Must be less than {0}.", MaxDistance));
                }

                IEnumerable<ZipCodeDistance> codes1 = null;
                if (startingZipCode.DistanceCache == null)
                {
                    // grab all less than the MaxDistance in first pass
                    codes1 = from c in this.Values
                             let d = c - startingZipCode
                             where (d <= MaxDistance)
                             orderby d
                             select new ZipCodeDistance() { ZipCode = c, Distance = d };
                    // this might just be temporary storage depending on caching settings
                    startingZipCode.DistanceCache = codes1;
                }
                else
                {
                    // grab the cached copy
                    codes1 = startingZipCode.DistanceCache;
                }
                List<ZipCodeDistance> filtered = new List<ZipCodeDistance>();

                foreach (ZipCodeDistance zcd in codes1)
                {
                    // since the list is pre-sorted, we can now drop out 
                    // quickly and efficiently as soon as something doesn't
                    // match
                    if (zcd.Distance > distance)
                    {
                        break;
                    }
                    filtered.Add(zcd);
                }

                // if no caching, don't leave the cached result in place
                if (!IsCaching) { startingZipCode.DistanceCache = null; }
                return filtered;
            }
        }





        #endregion



        // From documentation found here: http://www.census.gov/geo/www/gazetteer/places2k.html
        //   The ZCTA file contains data for all 5 digit ZCTAs in the 50 states, 
        //            District of Columbia and Puerto Rico as of Census 2000. The file is plain ASCII text, one line per record.

        //   Columns 1-2: United States Postal Service State Abbreviation
        //   Columns 3-66: Name (e.g. 35004 5-Digit ZCTA - there are no post office names)
        //   Columns 67-75: Total Population (2000)
        //   Columns 76-84: Total Housing Units (2000)
        //   Columns 85-98: Land Area (square meters) - Created for statistical purposes only.
        //   Columns 99-112: Water Area (square meters) - Created for statistical purposes only.
        //   Columns 113-124: Land Area (square miles) - Created for statistical purposes only.
        //   Columns 125-136: Water Area (square miles) - Created for statistical purposes only.
        //   Columns 137-146: Latitude (decimal degrees) First character is blank or "-" denoting North or South latitude respectively
        //   Columns 147-157: Longitude (decimal degrees) First character is blank or "-" denoting East or West longitude respectively       
        private ZipCodes LoadZipCodeCoordinates(string pathToFile)
        {
            ZipCodes codes = new ZipCodes();
             string[] fileLines = File.ReadAllLines(pathToFile);

            string sep = "\t";

            foreach (string line in fileLines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                string code = ""; //int
                double lat = 0;
                double lon = 0;

                string[] tokens = line.Split(sep.ToCharArray());

                //if (!Int32.TryParse(line.Substring(2, 5), out code) ||
                //    !double.TryParse(line.Substring(136, 10), out lat) ||
                //    !double.TryParse(line.Substring(146, 10), out lon))
                //    continue;// skip lines that aren't valid
                if ( //!Int32.TryParse(tokens[0], out code) ||
                   !double.TryParse(tokens[5], out lat) ||
                   !double.TryParse(tokens[6], out lon))
                    continue;// skip lines that aren't valid

                if (codes.ContainsKey(code))
                    continue;  // there are a few duplicates due to state boundaries,   ignore them

                codes.Add(code, new ZipCode()
                            {
                              //  State = line.Substring(0, 2),
                                Code = code,                            
                                Latitude = ZipCode.ToRadians(lat),
                                Longitude = ZipCode.ToRadians(lon),
                            });
            }
            return codes;
           }        
        }

}
