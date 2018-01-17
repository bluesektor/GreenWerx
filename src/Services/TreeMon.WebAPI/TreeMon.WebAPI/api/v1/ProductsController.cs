// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using TreeMon.Data.Logging;
using TreeMon.Managers;
using TreeMon.Managers.General;
using TreeMon.Managers.Membership;
using TreeMon.Managers.Plant;
using TreeMon.Managers.Store;
using TreeMon.Models;
using TreeMon.Models.App;
using TreeMon.Models.Datasets;
using TreeMon.Models.General;
using TreeMon.Models.Membership;
using TreeMon.Models.Plant;
using TreeMon.Models.Store;
using TreeMon.Utilites.Extensions;
using TreeMon.Utilites.Helpers;
using TreeMon.Web.Filters;
using TreeMon.Web.Models;
using TreeMon.WebAPI.Models;
using TMG = TreeMon.Models.General;

namespace TreeMon.Web.api.v1
{
    public class ProductsController : ApiBaseController
    {
        public ProductsController()
        {

        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight =4)]
        [HttpPost]
        [Route("api/Products/Add")]
        [Route("api/Products/Insert")]
        public ServiceResult Insert(Product n)
        {
            if (n == null)
                return ServiceResponse.Error("Invalid product posted to server.");

            string authToken = Request.Headers?.Authorization?.Parameter;
            SessionManager sessionManager = new SessionManager(Globals.DBConnectionKey);

            UserSession us = sessionManager.GetSession(authToken);
            if (us == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            if (string.IsNullOrWhiteSpace(us.UserData))
                return ServiceResponse.Error("Couldn't retrieve user data.");

            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            if (string.IsNullOrWhiteSpace(n.CreatedBy))
            {
                n.CreatedBy = CurrentUser.UUID;
                n.AccountUUID = CurrentUser.AccountUUID;
                n.DateCreated = DateTime.UtcNow;
            }

            #region Convert to product from productview because entity frameworks doesn't recognize casting.

            //var config = new MapperConfiguration(cfg =>
            //{
            //    cfg.CreateMap<ProductForm, Product>();
            //});

            //IMapper mapper = config.CreateMapper();
            //var dest = mapper.Map<ProductForm, Product>(n);
            #endregion

            ProductManager productManager = new ProductManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            return productManager.Insert(n, true);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpPost]
        [HttpGet]
        [Route("api/Products/{name}")]
        public ServiceResult Get( string name )
        {
            if (string.IsNullOrWhiteSpace(name))
                return ServiceResponse.Error("You must provide a name for the strain.");

            ProductManager productManager = new ProductManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            List<Product> s = productManager.Search(name);

            if (s == null || s.Count == 0)
                return ServiceResponse.Error("Product could not be located for the name " + name);

            return ServiceResponse.OK("",s);
        }


        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpPost]
        [HttpGet]
        [Route("api/ProductsBy/{uuid}")]
        public ServiceResult GetBy(string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return ServiceResponse.Error("You must provide a name for the strain.");

            ProductManager productManager = new ProductManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            Product p = (Product)productManager.Get(uuid);

            if (p == null)
                return ServiceResponse.Error("Product could not be located for the uuid " + uuid);

            return ServiceResponse.OK("", p);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpPost]
        [HttpGet]
        [Route("api/Product/{uuid}/{type}/Details")]
        public ServiceResult GetProductDetails(string uuid, string type)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return ServiceResponse.Error("You must provide a name for the strain.");


            string refUUID = "";
            string refType = "";
            string refAccount = "";
            string ManufacturerUUID = "";
            string strainUUID = "";

            if (type.EqualsIgnoreCase( "PRODUCT"))
            {
                ProductManager productManager = new ProductManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
                Product p = (Product)productManager.Get(uuid);

                if (p == null)
                    return ServiceResponse.Error("Product could not be located for the uuid " + uuid);

                refUUID = p.UUID;
                refType = p.UUIDType;
                refAccount = p.AccountUUID;
                ManufacturerUUID = p.ManufacturerUUID;
                strainUUID = p.StrainUUID;
            }
           

            AccountManager am = new AccountManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            Account a = (Account)am.Get(ManufacturerUUID);

            AttributeManager atm = new AttributeManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            List<TMG.Attribute> attributes =  atm.GetAttributes(refUUID, refType, refAccount).Where(w => w.Deleted == false).ToList();
            if (attributes == null)
                attributes = new List<TMG.Attribute>();
            if(a != null) { 
                attributes.Add(new TMG.Attribute()
                {
                     Name = "Manufacturer",
                     AccountUUID = a.AccountUUID,
                     UUIDType  = a.UUIDType,
                     Active = a.Active,
                     CreatedBy = a.CreatedBy,
                     DateCreated = a.DateCreated,
                     Deleted = a.Deleted,
                     Id = a.Id,
                     Private = a.Private,
                     Status = a.Status,
                     Value = a.Name,
                     ValueType = "string"
                });
            }

            #region plant related info
            StrainManager pm = new StrainManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            Strain s = (Strain)pm.Get(strainUUID);
            if (s != null)
            {
                attributes.Add(new TMG.Attribute()
                {
                    Name = "Strain Name",
                    AccountUUID = s.AccountUUID,
                    UUIDType = s.UUIDType,
                    Active = s.Active,
                    CreatedBy = s.CreatedBy,
                    DateCreated = s.DateCreated,
                    Deleted = s.Deleted,
                    Id = s.Id,
                    Private = s.Private,
                    Status = s.Status,
                    Value = s.Name,
                    ValueType = "string"
                });

                attributes.Add(new TMG.Attribute()
                {
                    Name = "Indica Percent",
                    AccountUUID = s.AccountUUID,
                    UUIDType = s.UUIDType,
                    Active = s.Active,
                    CreatedBy = s.CreatedBy,
                    DateCreated = s.DateCreated,
                    Deleted = s.Deleted,
                    Id = s.Id,
                    Private = s.Private,
                    Status = s.Status,
                    Value = s.IndicaPercent.ToString(),
                    ValueType = "number"
                });

                attributes.Add(new TMG.Attribute()
                {
                    Name = "Sativa Percent",
                    AccountUUID = s.AccountUUID,
                    UUIDType = s.UUIDType,
                    Active = s.Active,
                    CreatedBy = s.CreatedBy,
                    DateCreated = s.DateCreated,
                    Deleted = s.Deleted,
                    Id = s.Id,
                    Private = s.Private,
                    Status = s.Status,
                    Value = s.SativaPercent.ToString(),
                    ValueType = "number"
                });

                if (!string.IsNullOrWhiteSpace(s.Generation))
                {
                    attributes.Add(new TMG.Attribute()
                    {
                        Name = "Generation",
                        AccountUUID = s.AccountUUID,
                        UUIDType = s.UUIDType,
                        Active = s.Active,
                        CreatedBy = s.CreatedBy,
                        DateCreated = s.DateCreated,
                        Deleted = s.Deleted,
                        Id = s.Id,
                        Private = s.Private,
                        Status = s.Status,
                        Value = s.Generation,
                        ValueType = "string"
                    });
                }

               CategoryManager cm = new CategoryManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
               Category c = (Category)cm.Get(s.CategoryUUID);
                if (c != null)
                {
                    attributes.Add(new TMG.Attribute()
                    {
                        Name = "Variety",
                        AccountUUID = c.AccountUUID,
                        UUIDType = c.UUIDType,
                        Active = c.Active,
                        CreatedBy = c.CreatedBy,
                        DateCreated = c.DateCreated,
                        Deleted = c.Deleted,
                        Id = c.Id,
                        Private = c.Private,
                        Status = c.Status,
                        Value = c.Name,
                        ValueType = "string"
                    });
                }
            }
            #endregion

            return ServiceResponse.OK("", attributes);
        }

        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [HttpGet]
        [Route("api/Products")]
        public ServiceResult GetProducts(string filter = "")
        {
            ProductManager productManager = new ProductManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            List<dynamic> Products = (List<dynamic>)productManager.GetAll("").Where(w => w.Deleted == false).Cast<dynamic>().ToList();
            int count;
            DataFilter tmpFilter = this.GetFilter(filter);
            Products = FilterEx.FilterInput(Products, tmpFilter, out count);
            return ServiceResponse.OK("", Products, count);
        }


        /// <summary>
        /// filter ideas: instock=true,
        /// </summary>
        /// <param name="category"></param>
        /// <param name="filter"></param>
        /// <param name="startIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sorting"></param>
        /// <returns></returns>
        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 1)]
        [HttpPost]
        [HttpGet]
        [Route("api/Products/Categories/{category}/")]
        public ServiceResult GetProducts(string category, string filter = "")
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            ProductManager productManager = new ProductManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            List<dynamic> products = (List<dynamic>)productManager.GetAll(category)
                                                                    .Where( pw => ( pw.AccountUUID == CurrentUser.AccountUUID) && pw.Deleted == false )
                                                                    .Cast<dynamic>().ToList();

            int count;
            DataFilter tmpFilter = this.GetFilter(filter);
            products = FilterEx.FilterInput(products, tmpFilter, out count );

            if (products == null || products.Count == 0)
                return ServiceResponse.Error("No products available.");
            
            return ServiceResponse.OK("", products, count);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpPost]
        [HttpGet]
        [Route("api/Products/Categories")]
        public ServiceResult GetProductCategories(string filter = "")
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");


            CategoryManager catManager = new CategoryManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);
            List<dynamic> categories = (List<dynamic>)catManager.GetCategories(CurrentUser.AccountUUID, false, true).Where(w => (w.CategoryType?.EqualsIgnoreCase("PRODUCT")??false) ).Cast<dynamic>().ToList();
            int count;

                            DataFilter tmpFilter = this.GetFilter(filter);
                categories = FilterEx.FilterInput(categories, tmpFilter, out count);
            return ServiceResponse.OK("", categories, count);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [HttpGet]
        [Route("api/Products/Categories/Move")]
        public ServiceResult MoveProductsToCategory()
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            ServiceResult res;
           
            try
            {
                Task<string> content = ActionContext.Request.Content.ReadAsStringAsync();
                if (content == null)
                    return ServiceResponse.Error("No products were sent.");

                string body = content.Result;

                if (string.IsNullOrEmpty(body))
                    return ServiceResponse.Error("No products were sent.");

                List<Product> products = JsonConvert.DeserializeObject<List<Product>>(body);

                ProductManager pm = new ProductManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
                res = pm.Update(products);
            }
            catch (Exception ex)
            {
                res = ServiceResponse.Error(ex.Message);
                Debug.Assert(false, ex.Message);
                SystemLogger logger = new SystemLogger(Globals.DBConnectionKey);
                logger.InsertError(ex.Message, "ProductController", "MoveProductCategories");
            }
            return res;
        }

        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [HttpDelete]
        [Route("api/Products/Delete")]
        public ServiceResult Delete(Product n)
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");


            if (n == null || string.IsNullOrWhiteSpace(n.UUID))
                return ServiceResponse.Error("Invalid account was sent.");

            ProductManager productManager = new ProductManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            return productManager.Delete(n);
           
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [HttpDelete]
        [Route("api/Products/Delete/{productUUID}")]
        public ServiceResult Delete(string productUUID)
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            ProductManager productManager = new ProductManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            Product p = (Product)productManager.Get(productUUID);

            if (p == null || string.IsNullOrWhiteSpace(p.UUID))
                return ServiceResponse.Error("Invalid account was sent.");

            return productManager.Delete(p);

        }

        /// <summary>
        /// Fields updated..
        ///     Category 
        ///     Name 
        ///     Cost
        ///     Price
        ///     Weight 
        ///     WeightUOM
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [HttpPatch]
        [Route("api/Products/Update")]
        public ServiceResult Update(Product pv)
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");


            if (pv== null)
                return ServiceResponse.Error("Invalid product sent to server.");

            ProductManager productManager = new ProductManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            var dbP = productManager.GetAll().FirstOrDefault( pw => pw.UUID == pv.UUID );

            if ( dbP == null )
                return ServiceResponse.Error( "Product was not found." );

           

            dbP.CategoryUUID = pv.CategoryUUID;
            dbP.Name = pv.Name;

            dbP.Link = pv.Link;
            dbP.LinkProperties = pv.LinkProperties;
           
            dbP.Virtual = pv.Virtual;
            dbP.Image= pv.Image;
            dbP.DepartmentUUID = pv.DepartmentUUID;
            dbP.SKU = pv.SKU;
            dbP.Virtual = pv.Virtual;

            dbP.StrainUUID = pv.StrainUUID;
            dbP.ManufacturerUUID = pv.ManufacturerUUID;

            dbP.Price = pv.Price;
       
            dbP.Weight = pv.Weight;
            dbP.UOMUUID = pv.UOMUUID;

            //dbP.Expires           =
            //dbP.Category          =
            dbP.Description = pv.Description;

            #region future implementation. may need to be implemented in inventory.
            //     dbP.Cost = pv.Cost;
            //dbP.StockCount        =
            //dbP.Discount          =
            //dbP.MarkUp            =
            //dbP.MarkUpType        =
            //dbP.Condition         =
            //dbP.Quality           =
            //dbP.Rating            =
            //dbP.LocationUUID      =
            //dbP.ParentId          =
            //dbP.Status            =
            //dbP.Active            =
            //dbP.Deleted           =
            //dbP.Private           =
            //dbP.SortOrder         =

            #endregion

            return productManager.Update(dbP);
        }
    }
}
