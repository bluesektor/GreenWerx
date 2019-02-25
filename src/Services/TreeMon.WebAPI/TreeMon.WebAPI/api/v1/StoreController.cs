// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TreeMon.Data.Logging;
using TreeMon.Managers.Finance;
using TreeMon.Managers.Geo;
using TreeMon.Managers.Inventory;
using TreeMon.Managers.Store;
using TreeMon.Models.App;
using TreeMon.Models.Datasets;
using TreeMon.Models.Geo;
using TreeMon.Models.Store;
using TreeMon.Utilites.Extensions;
using TreeMon.Web;
using TreeMon.Web.api;
using TreeMon.Web.api.Helpers;
using TreeMon.Web.Filters;
using TreeMon.WebAPI.Helpers;
using WebApi.OutputCache.V2;

namespace TreeMon.WebAPI.api.v1
{
    [CacheOutput(ClientTimeSpan = 100, ServerTimeSpan = 100)]
    public class StoreController : ApiBaseController
    {
        public StoreController()
        {
        }


        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 0)]
        [HttpGet]
        [Route("api/Store/Cart/{cartUUID}/Items")]
        public ServiceResult GetCartItems(string cartUUID)
        {
            StoreManager storeManager = new StoreManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            List<dynamic> itemsInCart = storeManager.GetItemsInCart(cartUUID).Cast<dynamic>().ToList();
            return ServiceResponse.OK("", itemsInCart, itemsInCart.Count);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 0)]
        [HttpGet]
        [Route("api/Store/Cart/{cartUUID}")]
        public ServiceResult GetShoppingCart(string cartUUID)
        {

            StoreManager storeManager = new StoreManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            ShoppingCart cart = storeManager.GetCart(cartUUID);

            if (cart == null)
                return ServiceResponse.Error("Cart wasn't found.");

            CartView cv = this.GetCartView(cart);
            return ServiceResponse.OK("", cv);
        }
        [HttpPost]
        [HttpGet]
        [Route("api/Store")]
        public ServiceResult GetStoreItems()
        {
            LocationManager lm = new LocationManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            Location location = lm.GetAll()?.FirstOrDefault(w => w.isDefault == true && w.LocationType.EqualsIgnoreCase("ONLINE STORE"));

            if (location == null)
            {
                return ServiceResponse.Error("Store location id could not be found. To fix this add a location and set the type to Online Store and then select default.");
            }

            InventoryManager inventoryManager = new InventoryManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            List<dynamic> Inventory = (List<dynamic>)inventoryManager.GetItems(location.AccountUUID)
                    .Where(w => w.LocationUUID == location.UUID &&
                                w.Deleted == false &&
                                w.Published == true)
                    // && w.Expires && w.Private == false
                    .Cast<dynamic>().ToList();

            int count;

             DataFilter tmpFilter = this.GetFilter(Request);
            Inventory = Inventory.Filter( tmpFilter, out count);
            return ServiceResponse.OK("", Inventory, count);
        }

        

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 0)]
        [HttpGet]
        [Route("api/Store/NewCart")]
        public ServiceResult NewShoppingCart()
        {
            StoreManager storeManager = new StoreManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            ShoppingCart cart = new ShoppingCart();
            if (CurrentUser != null)
            {
                cart.UserUUID = CurrentUser.UUID;
                cart.CreatedBy = CurrentUser.UUID;
            }
            cart.DateCreated = DateTime.UtcNow;
            cart.Active = true;
            cart.Deleted = false;
            cart.RoleOperation = ">=";
            cart.RoleWeight = 0;

            return storeManager.AddCart(cart);

        }

   
        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 0)]
        [HttpPost]
        [Route("api/Store/Cart/{cartUUID}/Add/{inventoryUUID}/quantity/{quantity}")]
        public ServiceResult AddToCart(string cartUUID, string inventoryUUID, float quantity)
        {
            if (string.IsNullOrWhiteSpace(inventoryUUID))
                return ServiceResponse.Error("Invalid inventory item id.");

            if(quantity <= 0)
                return ServiceResponse.Error("Invalid quantity.");

            StoreManager storeManager = new StoreManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            
            //todo return the shopping cart so the client is upto date.
            ServiceResult res = storeManager.AddToCart(cartUUID, inventoryUUID, quantity);
            if (res.Code != 200)
                return res;

            ShoppingCart cart = storeManager.GetCart(cartUUID);
            if (cart == null)
                return ServiceResponse.Error("Could not retrieve cart.");

            return ServiceResponse.OK("", cart);
        }

        /// <summary>
        /// Deletes the shopping cart item.
        /// </summary>
        /// <param name="productId">The product id.</param>
        /// <returns></returns>
        [HttpPost]
        [HttpDelete]
        [Route("api/Store/Cart/{cartUUID}/Item/{cartItemUUID}/Delete")]
        public ServiceResult DeleteShoppingCartItem(string cartUUID,string cartItemUUID)
        {
            if (string.IsNullOrWhiteSpace(cartItemUUID))
                return ServiceResponse.Error("Invalid cart item id.");

           StoreManager storeManager = new StoreManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            ServiceResult res = storeManager.DeleteCartItem(cartItemUUID);
            if (res.Code != 200)
                return res;

            ShoppingCart cart = storeManager.GetCart(cartUUID);

            if (cart == null)
                return ServiceResponse.Error("Could not retrieve cart.");

            return ServiceResponse.OK("", cart);
        }


        [HttpPost]
        [Route("api/Store/CheckOut")]
        public ServiceResult CheckOut(CartView cart)
        {
            string ipAddress = new NetworkHelper().GetClientIpAddress(Request);
            StoreManager sm = new StoreManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            return sm.ProcessPayment(cart, ipAddress);
        }


        //Todo optimize this process. Cache on client and cache here.
        // add better data retrieval..
        //
        private CartView GetCartView( ShoppingCart cart)
        {
            CartView cv = new CartView();
            try
            {
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<ShoppingCart, CartView>();
                });

                IMapper mapper = config.CreateMapper();
                cv = mapper.Map<ShoppingCart, CartView>(cart);
            }
            catch (Exception ex)
            {
                SystemLogger logger = new SystemLogger(Globals.DBConnectionKey);
                logger.InsertException(ex, "StoreController", "GetCartView", ContextHelper.GetContextData());
                return cv;
            }
            StoreManager storeManager = new StoreManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
                cv.CartItems = storeManager.GetItemsInCart(cv.UUID);
          
            PriceManager cm = new PriceManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            cv.PriceRules = cm.GetPriceRules(cv.UUID, "shoppingcart");

            LocationManager lm = new LocationManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            cv.BillingAddress = (Location)lm.Get(cv.BillingLocationUUID);

            if (cv.ShippingSameAsBiling == false)
                cv.ShippingAddress = (Location)lm.Get(cv.ShippingLocationUUID);

            return cv;
        }
    }
}