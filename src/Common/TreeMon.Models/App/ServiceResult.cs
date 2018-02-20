// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
namespace TreeMon.Models.App
{

    /// <summary>
    /// Note, if you want to return objects to the client, json encode them into the Result.
    /// </summary>
    public class ServiceResponse
    {
        protected ServiceResponse() { }
        public static ServiceResult OK(string message = "", object result = null, int recordCount = 0 )
        {
            return new ServiceResult() { Code = 200, Status = "OK", Message = message, Result = result, TotalRecordCount = recordCount };
        }

        public static ServiceResult Error(string message = "", object result = null)
        {
            return new ServiceResult() { Code = 500, Status = "ERROR", Message = message, Result = result };
        }

        #region To use jquery jtable on the client you'll have to uncomment these and return them in the controller
        ////public static ServiceResult OK( object records = null, int allRecordsCount = 0, string message = "", string result = "")
        ////{
        ////    return new ServiceResult() { Code = 200, Status = "OK", Records = records, TotalRecordCount = allRecordsCount, Message = message, Result = result };
        ////}

        ////public static ServiceResult OK("",object record = null, string message = "", string result = "")
        ////{
        ////    return new ServiceResult() { Code = 200, Status = "OK", Record = record, Message = message, Result = result };
        ////}
        #endregion

    }

    ///  return new ServiceResult { Code = 500, Status = "ERROR", Result = "-1", Message = "Error message here." };
    ///   return new ServiceResult { Code = 200, Status = "OK",  Message = "" };
    ///   return new ServiceResult { Code = 200, Status = "OK", Result = resultJsonObj, Message = "" };
    public class ServiceResult
    {
        public int RoleWeight { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }
        public object Result { get; set; }
        public string Status { get; set; }
        public int TotalRecordCount { get; set; }
        #region To use jquery jtable on the client you'll have to uncomment these and return them in the controller
        ////public object Records;
        ////public object Record;
        #endregion
    }

   
}
