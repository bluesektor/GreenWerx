// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TreeMon.Models.Datasets;

namespace TreeMon.Utilites.Extensions
{
    public static class FilterEx
    {
        public static List<dynamic> FilterInput(List<dynamic> input, DataFilter filter, out int resultCount )
        {
            resultCount = 0;
            List<dynamic> res = input;

            if (filter == null)
            {
                if (input != null)
                    resultCount = input.Count;

                return input;
            }
          
            foreach (DataScreen f in filter.Screens)
            {
                if (string.IsNullOrWhiteSpace(f.Command) || string.IsNullOrWhiteSpace(f.Field))
                    continue;

                switch (f.Command?.ToUpper())
                {
                    case "CONTAINS":
                    case "SEARCHBY":
                    case "SEARCH!BY":
                        res = SearchBy(res, f);
                        break;
                    case "ORDERBY":
                    case "ORDERBYDESC":
                        //order by ascending is default
                        res = OrderBy(res, f);
                        break;
                    case "DISTINCTBY":
                        res = DistinctBy(res, f);
                        break;
                    case "DISTINCT":
                        res = res.Distinct().ToList();
                        break;
                }
            }
            resultCount = res.Count;

            if (filter.PageResults)
                res = res.Paginate(filter.StartIndex, filter.PageSize);

            //todo make sure there's no data leaks for users under roles
            //this will need to be changed for store items and stuff(we wan't enought data to get out to show
            //product info.
            //todo the roleweight need to be defined, but for now if a basic user is logged in
            //they should be in RoleWeight 1 or 2. Roleweight 4 and above should start getting into admin range.
            //todo we can check the data's role weight and operator here also.
            ////if (filter.UserRoleWeight < 4)
            ////{
            ////    res = res.Select(s => new
            ////    {
            ////        Name = s.Name,
            ////        UUID = s.UUID
            ////    }).Cast<dynamic>().ToList();
            ////}

            return res;
        }

        ////public static List<dynamic> FilterQueryable(IQueryable<dynamic> input, string screens)
        ////{
        ////    Debug.Assert(false, "NOT IMPLEMENTED. yet");
        ////    //input = input.Where(w => w?.UUID != screen.Value).ToList();
        ////    return new List<dynamic>();
        ////}

        public static List<dynamic> DistinctBy(List<dynamic> input, DataScreen screen)
        {

            if (input == null || input.Count == 0)
            {
                return input;
            }
            switch (screen.Field?.ToUpper())
            {
                //Node Properties
                case "UUID":
                    input = input.DistinctBy(d => d.UUID).ToList();
                    break;
                case "UUIDTYPE":
                    input = input.DistinctBy(d => d.UUIDType).ToList();
                    break;
                case "UUPARENTID":
                    input = input.DistinctBy(d => d.UUParentID).ToList();
                    break;
                case "UUPARENTIDTYPE":
                    input = input.DistinctBy(d => d.UUParentIDType).ToList();
                    break;
                case "NAME":
                    input = input.DistinctBy(d => d.Name).ToList();
                    break;
                case "STATUS":
                    input = input.DistinctBy(d => d.Status).ToList();
                    break;
                case "ACCOUNTUUID":
                    input = input.DistinctBy(d => d.AccountUUID).ToList();
                    break;
                case "ACTIVE":
                    input = input.DistinctBy(d => d.Active).ToList();
                    break;
                case "DELETED":
                    input = input.DistinctBy(d => d.Deleted).ToList();
                    break;
                case "PRIVATE":
                    input = input.DistinctBy(d => d.Private).ToList();
                    break;
                case "SORTORDER":
                    input = input.DistinctBy(d => d.SortOrder).ToList();
                    break;
                case "CREATEDBY":
                    input = input.DistinctBy(d => d.CreatedBy).ToList();
                    break;
                case "DATECREATED":
                    input = input.Where(w => Convert.ToDateTime(w?.DateCreated) == Convert.ToDateTime(screen.Value)).ToList();
                    break;
                //Strain Specific
                case "HARVESTTIME":
                    input = input.DistinctBy(d => d.HarvestTime).ToList();
                    break;
                //Category Specific
                case "CATEGORYTYPE":
                    input = input.DistinctBy(d => d.CategoryType).ToList();
                    break;
                //Product Specific
                case "CATEGORY":
                    input = input.DistinctBy(d => d.CategoryUUID).ToList();
                    break;
                case "MANUFACTURER":
                    input = input.DistinctBy(d => d.ManufacturerUUID).ToList();
                    break;
                case "DEPARTMENT":
                    input = input.DistinctBy(d => d.DepartmentUUID).ToList();
                    break;
                //Vendor Specific
                case "BREEDER":
                    input = input.DistinctBy(d => d.Breeder).ToList();
                    break;
                case "GROWER":
                    input = input.Where(w => w?.Grower).ToList();
                    break;
                case "DISPENSARY":
                    input = input.DistinctBy(d => d.Dispensary).ToList();
                    break;
                case "BREEDERTYPE":
                    input = input.DistinctBy(d => d.BreederType).ToList();
                    break;
                //Units Of Measure
                case "CATEGORYUUID":
                    #region original code not ported. may not be needed
                    ////CategoryManager categoryManager = new CategoryManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
                    ////Category category = categoryManager.Get(f.SearchTerm);
                    ////if (category == null)
                    ////    continue;
                    ////input = input.Where(w => w.Category?.EqualsIgnoreCase(category.Name.ToUpper()).ToList();
                    #endregion
                    Debug.Assert(false, "Verify the search term is category and not uuid");
                    input = input.DistinctBy(d => d.BreederType).ToList();
                    break;
            }
            return input;
        }

        public static List<dynamic> OrderBy(List<dynamic> input, DataScreen screen)
        {
            if (input == null || input.Count == 0)
            {
                return input;
            }

            bool orderbyDesc = false;
            if (screen.Command.ToUpper().Contains("DESC"))
                orderbyDesc = true;

            switch (screen.Field.ToUpper())
            {
                //Node Properties
                case "UUID":
                    if (orderbyDesc)
                        input = input.OrderByDescending(d => d.UUID).ToList();
                    else
                        input = input.OrderBy(d => d.UUID).ToList();
                    break;
                case "UUIDTYPE":
                    if (orderbyDesc)
                        input = input.OrderByDescending(d => d.UUIDType).ToList();
                    else
                        input = input.OrderBy(d => d.UUIDType).ToList();
                    break;
                case "UUPARENTID":
                    if (orderbyDesc)
                        input = input.OrderByDescending(d => d.UUParentID).ToList();
                    else
                        input = input.OrderBy(d => d.UUParentID).ToList();
                    break;
                case "UUPARENTIDTYPE":
                    if (orderbyDesc)
                        input = input.OrderByDescending(d => d.UUParentIDType).ToList();
                    else
                        input = input.OrderBy(d => d.UUParentIDType).ToList();
                    break;
                case "NAME":
                    if (orderbyDesc)
                        input = input.OrderByDescending(d => d.Name).ToList();
                    else
                        input = input.OrderBy(d => d.Name).ToList();
                    break;
                case "STATUS":
                    if (orderbyDesc)
                        input = input.OrderByDescending(d => d.Status).ToList();
                    else
                        input = input.OrderBy(d => d.Status).ToList();
                    break;
                case "ACCOUNTUUID":
                    if (orderbyDesc)
                        input = input.OrderByDescending(d => d.AccountUUID).ToList();
                    else
                        input = input.OrderBy(d => d.AccountUUID).ToList();
                    break;
                case "ACTIVE":
                    if (orderbyDesc)
                        input = input.OrderByDescending(d => d.Active).ToList();
                    else
                        input = input.OrderBy(d => d.Active).ToList();
                    break;
                case "DELETED":
                    if (orderbyDesc)
                        input = input.OrderByDescending(d => d.Deleted).ToList();
                    else
                        input = input.OrderBy(d => d.Deleted).ToList();
                    break;
                case "PRIVATE":
                    if (orderbyDesc)
                        input = input.OrderByDescending(d => d.Private).ToList();
                    else
                        input = input.OrderBy(d => d.Private).ToList();
                    break;
                case "SORTORDER":
                    if (orderbyDesc)
                        input = input.OrderByDescending(d => d.SortOrder).ToList();
                    else
                        input = input.OrderBy(d => d.SortOrder).ToList();
                    break;
                case "CREATEDBY":
                    if (orderbyDesc)
                        input = input.OrderByDescending(d => d.CreatedBy).ToList();
                    else
                        input = input.OrderBy(d => d.CreatedBy).ToList();
                    break;
                case "DATECREATED":
                    if (orderbyDesc)
                        input = input.OrderByDescending(d => d.DateCreated).ToList();
                    else
                        input = input.Where(w => Convert.ToDateTime(w?.DateCreated) == Convert.ToDateTime(screen.Value)).ToList();
                    break;
                //Strain Specific
                case "HARVESTTIME":
                    if (orderbyDesc)
                        input = input.OrderByDescending(d => d.HarvestTime).ToList();
                    else
                        input = input.OrderBy(d => d.HarvestTime).ToList();
                    break;
                //Category Specific
                case "CATEGORYTYPE":
                    if (orderbyDesc)
                        input = input.OrderByDescending(d => d.CategoryType).ToList();
                    else
                        input = input.OrderBy(d => d.CategoryType).ToList();
                    break;
                //Product Specific
                case "CATEGORY":
                    if (orderbyDesc)
                        input = input.OrderByDescending(d => d.CategoryUUID).ToList();
                    else
                        input = input.OrderBy(d => d.CategoryUUID).ToList();
                    break;
                case "MANUFACTURER":
                    if (orderbyDesc)
                        input = input.OrderByDescending(d => d.ManufacturerUUID).ToList();
                    else
                        input = input.OrderBy(d => d.ManufacturerUUID).ToList();
                    break;
                case "DEPARTMENT":
                    if (orderbyDesc)
                        input = input.OrderByDescending(d => d.DepartmentUUID).ToList();
                    else
                        input = input.OrderBy(d => d.DepartmentUUID).ToList();
                    break;
                //Vendor Specific
                case "BREEDER":
                    if (orderbyDesc)
                        input = input.OrderByDescending(d => d.Breeder).ToList();
                    else
                        input = input.OrderBy(d => d.Breeder).ToList();
                    break;
                case "GROWER":
                    if (orderbyDesc)
                        input = input.OrderByDescending(d => d.Grower).ToList();
                    else
                        input = input.Where(w => w?.Grower).ToList();
                    break;
                case "DISPENSARY":
                    if (orderbyDesc)
                        input = input.OrderByDescending(d => d.Dispensary).ToList();
                    else
                        input = input.OrderBy(d => d.Dispensary).ToList();
                    break;
                case "BREEDERTYPE":
                    if (orderbyDesc)
                        input = input.OrderByDescending(d => d.BreederType).ToList();
                    else
                        input = input.OrderBy(d => d.BreederType).ToList();
                    break;
                //Units Of Measure
                case "CATEGORYUUID":
                    #region original code not ported. may not be needed
                    ////CategoryManager categoryManager = new CategoryManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
                    ////Category category = categoryManager.Get(f.SearchTerm);
                    ////if (category == null)
                    ////    continue;
                    ////input = input.Where(w => w.Category?.EqualsIgnoreCase(category.Name.ToUpper()).ToList();
                    #endregion
                    Debug.Assert(false, "todo Verify the search term is category and not uuid");
                    if (orderbyDesc)
                        input = input.OrderByDescending(d => d.CategoryUUID).ToList();
                    else
                        input = input.OrderBy(d => d.CategoryUUID).ToList();
                    break;
                //Finance account transaction
                case "TRANSACTIONDATE":
                    if (orderbyDesc)
                        input = input.OrderByDescending(d => d.TransactionDate).ToList();
                    else
                        input = input.OrderBy(d => d.TransactionDate).ToList();
                    break;
            }

            return input;
        }

        public static List<dynamic> SearchBy(List<dynamic> input, DataScreen screen)
        {
            if (input == null || input.Count == 0 || string.IsNullOrWhiteSpace(screen.Value) || string.IsNullOrWhiteSpace(screen.Field) || string.IsNullOrWhiteSpace(screen.Command))
            {
                return input;
            }
            try
            {
                switch (screen.Field?.ToUpper())
                {
                    //Node Properties
                    case "UUID":
                        if (screen.Command.EqualsIgnoreCase("CONTAINS"))
                            input = input.Where(w => w.UUID?.ToUpper().Contains( screen.Value )).ToList();
                        else if (screen.Command.EqualsIgnoreCase("SEARCH!BY"))
                            input = input.Where(w => w.UUID != screen.Value).ToList();
                        else
                            input = input.Where(w => w.UUID == screen.Value).ToList();
                        break;
                    case "VARIETYUUID":
                        if (screen.Command.EqualsIgnoreCase("CONTAINS"))
                            input = input.Where(w => w.CategoryUUID?.ToUpper().Contains(screen.Value.ToUpper())).ToList();
                        else if (screen.Command.EqualsIgnoreCase("SEARCH!BY"))
                            input = input.Where(w => w.CategoryUUID != screen.Value).ToList();
                        else
                            input = input.Where( w => w.CategoryUUID?.ToUpper()?.Trim() == screen.Value?.ToUpper()?.Trim() ).ToList();
                        break;
                    case "UUIDTYPE":
                        if (screen.Command.EqualsIgnoreCase("CONTAINS"))
                            input = input.Where(w => w.UUIDType?.ToUpper().Contains(screen.Value.ToUpper())).ToList();
                        else if (screen.Command.EqualsIgnoreCase("SEARCH!BY"))
                            input = input.Where(w => w.UUIDType != screen.Value).ToList();
                        else
                            input = input.Where(w => w.UUIDType == screen.Value).ToList();
                        break;
                    case "UUPARENTID":
                        if (screen.Command.EqualsIgnoreCase("CONTAINS"))
                            input = input.Where(w => w.UUParentID?.ToUpper().Contains(screen.Value.ToUpper())).ToList();
                        else if (screen.Command.EqualsIgnoreCase("SEARCH!BY"))
                            input = input.Where(w => w.UUParentID != screen.Value).ToList();
                        else
                            input = input.Where(w => w.UUParentID == screen.Value).ToList();
                        break;
                   
                    case "UUPARENTIDTYPE":
                        if (screen.Command.EqualsIgnoreCase("CONTAINS"))
                            input = input.Where(w => w.UUParentIDType?.ToUpper().Contains(screen.Value.ToUpper())).ToList();
                        else if (screen.Command.EqualsIgnoreCase("SEARCH!BY"))
                            input = input.Where(w => w.UUParentIDType != screen.Value).ToList();
                        else
                            input = input.Where(w => w.UUParentIDType == screen.Value).ToList();
                        break;
                    case "NAME":
                        if (screen.Command.EqualsIgnoreCase("CONTAINS"))
                            input = input.Where(w => w.Name?.ToUpper()?.Contains(screen.Value.ToUpper().ToUpper())).ToList();
                        else if (screen.Command.EqualsIgnoreCase("SEARCH!BY"))
                            input = input.Where(w => w.Name?.IndexOf(screen.Value, StringComparison.OrdinalIgnoreCase) == -1).ToList();
                        else
                            input = input.Where(w => w.Name?.IndexOf(screen.Value, StringComparison.OrdinalIgnoreCase) != -1).ToList();
                        break;
                    case "STATUS":
                        if (screen.Command.EqualsIgnoreCase("CONTAINS"))
                            input = input.Where(w => w.Status?.ToUpper().Contains(screen.Value.ToUpper())).ToList();
                        else if (screen.Command.EqualsIgnoreCase("SEARCH!BY"))
                            input = input.Where(w => w.Status?.IndexOf(screen.Value, StringComparison.OrdinalIgnoreCase) == -1).ToList();
                        else
                            input = input.Where(w => w.Status?.IndexOf(screen.Value, StringComparison.OrdinalIgnoreCase) != -1).ToList();
                        break;
                    case "ACCOUNTUUID":
                        if (screen.Command.EqualsIgnoreCase("CONTAINS"))
                            input = input.Where(w => w.AccountUUID?.ToUpper().Contains(screen.Value.ToUpper())).ToList();
                        else if (screen.Command.EqualsIgnoreCase("SEARCH!BY"))
                            input = input.Where(w => w.AccountUUID != screen.Value).ToList();
                        else
                            input = input.Where(w => w.AccountUUID == screen.Value).ToList();
                        break;
                    case "ACTIVE":
                      
                       if (screen.Command.EqualsIgnoreCase("SEARCH!BY"))
                            input = input.Where(w => Convert.ToBoolean(w.Active) != Convert.ToBoolean(screen.Value)).ToList();
                        else
                            input = input.Where(w => Convert.ToBoolean(w.Active) == Convert.ToBoolean(screen.Value)).ToList();
                        break;
                    case "BREEDER":
                        if (screen.Command.EqualsIgnoreCase("SEARCH!BY"))
                            input = input.Where(w => Convert.ToBoolean(w.Breeder) != Convert.ToBoolean(screen.Value)).ToList();
                        else
                            input = input.Where(w => Convert.ToBoolean(w.Breeder) == Convert.ToBoolean(screen.Value)).ToList();
                        break;
                    case "DELETED":
                       
                       if (screen.Command.EqualsIgnoreCase("SEARCH!BY"))
                            input = input.Where(w => Convert.ToBoolean(w.Deleted) != Convert.ToBoolean(screen.Value)).ToList();
                        else
                            input = input.Where(w => Convert.ToBoolean(w?.Deleted) == Convert.ToBoolean(screen.Value)).ToList();
                        break;
                    case "PRIVATE":
                
                       if (screen.Command.EqualsIgnoreCase("SEARCH!BY"))
                            input = input.Where(w => Convert.ToBoolean(w.Private) != Convert.ToBoolean(screen.Value)).ToList();
                        else
                            input = input.Where(w => Convert.ToBoolean(w.Private) == Convert.ToBoolean(screen.Value)).ToList();
                        break;
                    case "SORTORDER":
                     
                       if (screen.Command.EqualsIgnoreCase("SEARCH!BY"))
                            input = input.Where(w => w.SortOrder != StringEx.ConvertTo<int>(screen.Value)).ToList();
                        else
                            input = input.Where(w => w.SortOrder == StringEx.ConvertTo<int>(screen.Value)).ToList();
                        break;
                    case "CREATEDBY":
                        if (screen.Command.EqualsIgnoreCase("CONTAINS"))
                            input = input.Where(w => w.CreatedBy?.ToUpper().Contains(screen.Value.ToUpper())).ToList();
                        else if (screen.Command.EqualsIgnoreCase("SEARCH!BY"))
                            input = input.Where(w => w.CreatedBy != screen.Value).ToList();
                        else
                            input = input.Where(w => w.CreatedBy == screen.Value).ToList();
                        break;
                    case "DATECREATED":
                         if (screen.Command.EqualsIgnoreCase("SEARCH!BY"))
                            input = input.Where(w => Convert.ToDateTime(w.DateCreated) != Convert.ToDateTime(screen.Value)).ToList();
                        else
                            input = input.Where(w => Convert.ToDateTime(w.DateCreated) == Convert.ToDateTime(screen.Value)).ToList();
                        break;
                    //Strain Specific
                    case "HARVESTTIME":
                        if (screen.Command.EqualsIgnoreCase("SEARCH!BY"))
                            input = input.Where(w => w.HarvestTime != screen.Value).ToList();
                        else
                            input = input.Where(w => w.HarvestTime == StringEx.ConvertTo<int>(screen.Value)).ToList();
                        break;
                    //Category Specific
                    case "CATEGORYTYPE":
                        if (screen.Command.EqualsIgnoreCase("CONTAINS"))
                            input = input.Where(w => w.CategoryType?.ToUpper().Contains(screen.Value.ToUpper())).ToList();
                        else if (screen.Command.EqualsIgnoreCase("SEARCH!BY"))
                            input = input.Where(w => w.CategoryType?.ToUpper() != screen.Value).ToList();
                        else
                        {
                            input = input.Where(w => StringEx.EqualsEx(w.CategoryType, screen.Value))?.ToList();
                        }
                        break;
                    //Location and Finance Account
                    case "LOCATIONTYPE":
                        if (screen.Command.EqualsIgnoreCase("CONTAINS"))
                            input = input.Where(w => w.LocationType?.ToUpper().Contains(screen.Value.ToUpper())).ToList();
                        else if (screen.Command.EqualsIgnoreCase("SEARCH!BY"))
                            input = input.Where(w => w.LocationType?.ToString()?.ToUpper() != screen.Value).ToList();
                        else
                        {
                            input = input.Where(w => StringEx.EqualsEx(w.LocationType, screen.Value)).ToList();
                        }
                            break;
                    //Product Specific
                    case "CATEGORY":
                        if (screen.Command.EqualsIgnoreCase("CONTAINS"))
                            input = input.Where(w => w.CategoryUUID?.ToUpper().Contains(screen.Value.ToUpper())).ToList();
                        else if (screen.Command.EqualsIgnoreCase("SEARCH!BY"))
                            input = input.Where(w => w.CategoryUUID != screen.Value).ToList();
                        else
                            input = input.Where(w => w.CategoryUUID == screen.Value).ToList();
                        break;
                    case "MANUFACTURER":
                        if (screen.Command.EqualsIgnoreCase("CONTAINS"))
                            input = input.Where(w => w.ManufacturerUUID?.ToUpper().Contains(screen.Value.ToUpper())).ToList();
                        else if (screen.Command.EqualsIgnoreCase("SEARCH!BY"))
                            input = input.Where(w => w.ManufacturerUUID != screen.Value).ToList();
                        else
                            input = input.Where(w => w.ManufacturerUUID == screen.Value).ToList();
                        break;
                    case "DEPARTMENT":
                        if (screen.Command.EqualsIgnoreCase("CONTAINS"))
                            input = input.Where(w => w.DepartmentUUID?.ToUpper().Contains(screen.Value.ToUpper())).ToList();
                        else if (screen.Command.EqualsIgnoreCase("SEARCH!BY"))
                            input = input.Where(w => w.DepartmentUUID != screen.Value).ToList();
                        else
                            input = input.Where(w => w.DepartmentUUID == screen.Value).ToList();
                        break;
                     
                   
                    case "GROWER":
                      
                       if (screen.Command.EqualsIgnoreCase("SEARCH!BY"))
                            input = input.Where(w => w.Grower != screen.Value).ToList();
                        else
                            input = input.Where(w => w.Grower == true).ToList();
                        break;
                    case "DISPENSARY":
                      
                       if (screen.Command.EqualsIgnoreCase("SEARCH!BY"))
                            input = input.Where(w => w.Dispensary != screen.Value).ToList();
                        else
                            input = input.Where(w => w.Dispensary == true).ToList();
                        break;
                    case "BREEDERTYPE":
                        if (screen.Command.EqualsIgnoreCase("CONTAINS"))
                            input = input.Where(w => w.BreederType?.ToUpper().Contains(screen.Value.ToUpper())).ToList();
                        else if (screen.Command.EqualsIgnoreCase("SEARCH!BY"))
                            input = input.Where(w => w.BreederType?.ToUpper() != screen.Value).ToList();
                        else
                            input = input.Where(w => StringEx.EqualsEx(w.BreederType,screen.Value)).ToList();
                        break;
                    //Units Of Measure
                    case "CATEGORYUUID":
                        if (screen.Command.EqualsIgnoreCase("CONTAINS"))
                            input = input.Where(w => w.CategoryUUID?.ToUpper().Contains(screen.Value.ToUpper())).ToList();
                        else if (screen.Command.EqualsIgnoreCase("SEARCH!BY"))
                            input = input.Where(w => w.CategoryUUID != screen.Value).ToList();
                        else
                            input = input.Where(w => w.CategoryUUID == screen.Value).ToList();
                        break;

                  
                }
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
            }
            return input;
        }

     

        public static List<T> Paginate<T>(this List<T> input, int? startIndex = 0, int? pageSize = 1)
        {
            if (startIndex == null || pageSize == null)
                return input;

            List<T> value = default(List<T>);

            if (input == null)
            {
                return (List<T>)value;
            }
            if (input.Count < pageSize)
                return input;

            int index =  startIndex ?? default(int);
            int size = pageSize ?? default(int);
            int pageNumber = index / size;
            return input.Skip(pageNumber * size).Take(size).ToList();
        }

        public static List<dynamic> Paginate(this List<dynamic> input, int startIndex = 0, int pageSize = 1)
        {
            List<dynamic> value = new List<dynamic>();

            if (input == null)
            {
                return value;
            }
            if (input.Count < pageSize)
                return input;

            int pageNumber = startIndex / pageSize;
            return input.Skip(pageNumber * pageSize).Take(pageSize).ToList();
        }

        public static List<T> PaginateByPage<T>(this List<T> input, int page = 1, int pageSize = 1)
        {
            List<T> value = default(List<T>);

            if (input == null)
            {
                return (List<T>)value;
            }
            if (input.Count < pageSize)
                return input;


            return input.Skip(page * pageSize).Take(pageSize).ToList();
        }

        ////public static IEnumerable<TSource> DistinctBy<TSource, TKey>   (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        ////{
        ////    HashSet<TKey> seenKeys = new HashSet<TKey>();
        ////    foreach (TSource element in source)
        ////    {
        ////        if (seenKeys.Add(keySelector(element)))
        ////        {
        ////            yield return element;
        ////        }
        ////    }
        ////}
    }
}
