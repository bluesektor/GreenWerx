// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

//https://maps.googleapis.com/maps/api/geocode/json?address=Winnetka&key=Y




namespace TreeMon.Models.Geo
{
    public class GoogleGeo
    {
        //The "status" field within the Geocoding response object contains the status of the request, and may contain 
        //debugging information to help you track down why geocoding is not working.The "status" field may contain the following values:
        public struct ResponseStatus
        {
            public const string OK = "OK";//indicates that no errors occurred; the address was successfully parsed and at least one geocode was returned.
            public const string ZeroResults = "ZERO_RESULTS"; //indicates that the geocode was successful but returned no results. This may occur if the geocoder was passed a non-existent address.
            public const string OverLimit = "OVER_QUERY_LIMIT";// indicates that you are over your quota.
            public const string Denied = "REQUEST_DENIED"; //indicates that your request was denied.
            public const string InvalidRequest = "INVALID_REQUEST";  // generally indicates that the query (address, components or latlng) is missing.
            public const string Error = "UNKNOWN_ERROR";  // indicates that the request could not be processed due to a server error.The request may succeed if you try again.
        }
    }

    public class Rootobject
    {
        public Result[] results { get; set; }
        public string status { get; set; }
    }

    public class Result
    {
        public Address_Components[] address_components { get; set; }
        public string formatted_address { get; set; }
        public Geometry geometry { get; set; }
        public string place_id { get; set; }
        public string[] types { get; set; }
    }

    public class Geometry
    {
        public Coordinates location { get; set; }
        public string location_type { get; set; }
        public Viewport viewport { get; set; }
    }

    public class Coordinates//Location
    {
        public float lat { get; set; }
        public float lng { get; set; }
    }

    public class Viewport
    {
        public Northeast northeast { get; set; }
        public Southwest southwest { get; set; }
    }

    public class Northeast
    {
        public float lat { get; set; }
        public float lng { get; set; }
    }

    public class Southwest
    {
        public float lat { get; set; }
        public float lng { get; set; }
    }

    public class Address_Components
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public string[] types { get; set; }
    }

}
