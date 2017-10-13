using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeMon.Data;
using TreeMon.Models.Datasets;
using TreeMon.Models.Geo;
using TreeMon.Utilites.Helpers;

/// <summary>
/// TODO remove this from the project after DatasetManager tests are written
/// </summary>
namespace TreeMon.Managers.DataSets
{
    public  class FireIncidentManager
    {
        private IDbContext _dbContext = null;

        public object ObjectHelpers { get; private set; }

        /// <summary>
        /// This builds a full address from an incident.
        /// This is to standardize the return because the address hash needs exactly the same
        /// address to be computed.
        /// </summary>
        /// <param name="fi"></param>
        /// <returns></returns>
        public string GetFullAddress(FireIncident fi)
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

        public string GetFullAddress(GeoLocation geo)
        {
            string res = "";
            if (!string.IsNullOrWhiteSpace( geo.Address1 ))
                res = geo.Address1.Trim();

            if (!string.IsNullOrWhiteSpace(geo.Address2))
                res += " " + geo.Address2.Trim();

            if (!string.IsNullOrWhiteSpace(geo.City))
                res += " " + geo.City.Trim();

            if (!string.IsNullOrWhiteSpace(geo.State))
                res += " " + geo.State.Trim();

            if (!string.IsNullOrWhiteSpace(geo.Postal))
                res += " " + geo.Postal.Trim();

            if (!string.IsNullOrWhiteSpace(geo.Country))
                res += " " + geo.Country.Trim();

            return res;
        }

        public FireIncidentManager(IDbContext dbContext)
        {
            Debug.Assert(dbContext != null, "FireIncidentManager CONTEXT IS NULL!");

            if (dbContext != null)
                _dbContext = dbContext;
        }

        public List<FireIncident> GetIncidents(bool excludeCancelledCalls = true )
        {
            //AccountMember = UsersInAccount table.

            IEnumerable<FireIncident> incidents = _dbContext.GetAll<FireIncident>().Where(iw => iw.cancelled != excludeCancelledCalls);
              

            return incidents.ToList();
        }

        public List<FireIncident> GetIncidents(DataQuery queryObj, List<QueryFilter> filters = null)
        {
             IEnumerable<FireIncident> incidents = _dbContext.Select<FireIncident>(queryObj.SQL, queryObj.Parameters);
            return FilterResult(incidents.ToList(), filters);
        }

        /// <summary>
        /// todo v2. build this out more. its static for now.
        /// </summary>
        /// <param name="incidents"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        protected List<FireIncident> FilterResult(List<FireIncident> incidents, List<QueryFilter> filters)
        {
            if (filters == null)
                return incidents;

            List<FireIncident> res = new List<FireIncident>();

            if (incidents == null)
                return res;


          
            QueryFilter filter = filters.Find(f => f.Type == "custom" && f.Operator == "order" && f.Field == "unit" );
            if (filter == null)
                return incidents.ToList();

            string[] tokens = filter.Value.Split(',');

            int index = 0;

            foreach(string token in tokens)
            {
                FireIncident fi = null;
                
                    fi = incidents.Find(ft => ft.unit.ToUpper() == token.Trim().ToUpper());
                
                
                if(fi == null)
                {                      
                    fi = new FireIncident() { unit = token,  resp=0, arv_dttm = DateTime.MinValue };//hour_alm_dttm = 0,
                }

                index++; //for debuging the eck
                res.Add(fi);
            }
            return res;
        }

        // do a include? 
        //  district
        // station
        //    city
        //    zip
        public List<QueryFilter> GetFilters(string column)
        {
            IEnumerable<FireIncident> incidents = GetIncidents();

            if (string.IsNullOrWhiteSpace(column))
                return new List<QueryFilter>();
          
            column = column.ToLower();
            switch (column)
            {
                case "district":
                    incidents = incidents.GroupBy(x => x.district).Select(x => x.First());
                    break;
                case "station":
                    incidents = incidents.GroupBy(x => x.station).Select(x => x.First());
                    break;
                case "city":
                    incidents = incidents.GroupBy(x => x.city).Select(x => x.First());
                    break;
                case "zip":
                    incidents = incidents.GroupBy(x => x.zip).Select(x => x.First());
                    break;
                default:
                    return new List<QueryFilter>();
            }
          

            List<QueryFilter> filters = new List<QueryFilter>();

            foreach (FireIncident fi in incidents)
            {
                string caption = ObjectHelper.GetPropertyValue(column, fi);

                if (string.IsNullOrWhiteSpace(caption)|| caption.Contains("ERROR:"))
                    continue;

                filters.Add(new QueryFilter()
                {
                    Caption = caption,
                    Field= column,
                    Operator = "=",//equalto
                    Value = caption,
                      Order = 0
                });
            }

            return filters;
        }
      
       public int Update(FireIncident fi)
       {
            if (fi == null)
                return -1;

          return  _dbContext.Update<FireIncident>(fi);
       }

        public int Update(DataQuery queryObj)
        {
            return  _dbContext.ExecuteNonQuery(queryObj.SQL, queryObj.Parameters);
     
        }

    }
}
