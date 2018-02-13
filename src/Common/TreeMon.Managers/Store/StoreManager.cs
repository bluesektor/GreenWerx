// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using Dapper;
using MoreLinq;
using Omni.Managers.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Transactions;
using TreeMon.Data;
using TreeMon.Data.Logging;
using TreeMon.Managers.Documents;
using TreeMon.Managers.Inventory;
using TreeMon.Models;
using TreeMon.Models.App;
using TreeMon.Models.Finance;
using TreeMon.Models.Flags;
using TreeMon.Models.Inventory;
using TreeMon.Models.Membership;
using TreeMon.Models.Store;
using TreeMon.Utilites.Extensions;
using TreeMon.Utilites.Helpers;
using TreeMon.Utilites.Security;

namespace TreeMon.Managers.Store
{
    public class StoreManager : BaseManager
    {
        private readonly string _sessionKey;
        private readonly SystemLogger _logger;

        public StoreManager(string connectionKey, string sessionKey) : base(connectionKey, sessionKey)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(connectionKey), "StoreManager CONTEXT IS NULL!");

            _sessionKey = sessionKey;
            this._connectionKey = connectionKey;
            _logger = new SystemLogger(connectionKey);
        }

        public ServiceResult AddCart(ShoppingCart cart)
        {
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                try
                {
                    if (context.Insert<ShoppingCart>(cart))
                        return ServiceResponse.OK("Cart created", cart);
                }
                catch (Exception ex)
                {
                    _logger.InsertError(ex.Message, "AddCart", MethodInfo.GetCurrentMethod().Name);
                }
            }

            return ServiceResponse.Error("Failed to create shopping cart.");
        }

        public ServiceResult UpdateCart(ShoppingCart cart)
        {
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                try
                {
                    if (context.Update<ShoppingCart>(cart) > 0)
                        return ServiceResponse.OK();

                }
                catch (Exception ex)
                {
                    _logger.InsertError(ex.Message, "StoreManager", MethodInfo.GetCurrentMethod().Name);
                }
            }
            return ServiceResponse.Error("Failed to update the cart.");
        }

        public ServiceResult DeleteCart(string cartUUID)
        {

            List<dynamic> cartItems = this.GetItemsInCart(cartUUID);
            foreach (dynamic item in cartItems)
            {
                //Call the delete cart item function because it will add the items back to the inventory.
                ServiceResult res = this.DeleteCartItem(item.CartItemUUID);
                if (res.Code != 200)
                    return res;

            }
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                try
                {
                    ShoppingCart cart = context.GetAll<ShoppingCart>().FirstOrDefault(w => w.UUID == cartUUID);
                    if (cart == null)
                        return ServiceResponse.Error("Shopping cart wasn't found");

                    if (context.Delete<ShoppingCart>(cart) > 0)
                        return ServiceResponse.OK("Cart deleted");
                }
                catch (Exception ex)
                {
                    _logger.InsertError(ex.Message, "AddCart", MethodInfo.GetCurrentMethod().Name);
                }
            }

            return ServiceResponse.Error("Shopping cart not deleted.");
        }

        public ShoppingCart GetCart(string cartUUID)
        {
          
            ShoppingCart cart = new ShoppingCart();
            try
            {
                using (var context = new TreeMonDbContext(this._connectionKey))
                {
                    try
                    {
                        cart = context.GetAll<ShoppingCart>().FirstOrDefault(w => w.UUID == cartUUID);
                    }
                    catch (Exception ex)
                    {
                        _logger.InsertError(ex.Message, "AddCart", MethodInfo.GetCurrentMethod().Name);
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.InsertError(ex.Message, "StoreManager", "GetCart:" + cartUUID);
            }
            return cart;

        }

        public ServiceResult AddToCart(string cartUUID, string inventoryItemUUID, float quantity)
        {
            InventoryManager inventoryManager = new InventoryManager(this._connectionKey, _sessionKey);

            InventoryItem item = (InventoryItem)inventoryManager.Get(inventoryItemUUID);

            if (item == null)
                return ServiceResponse.Error("Product wasn't found.");

            ShoppingCartItem existingItem;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                existingItem = context.GetAll<ShoppingCartItem>().FirstOrDefault(w => w.ShoppingCartUUID == cartUUID && w.ItemType == item.ReferenceType && w.ItemUUID == item.UUID);
            }

            if (existingItem != null)
            {
                return this.AddCartItem(cartUUID, existingItem.UUID, quantity);
            }
            SessionManager sm = new SessionManager(_connectionKey);
            
            
            ShoppingCartItem cartItem = new ShoppingCartItem()
            {
                Id = 0,
                Quantity = quantity,
                Price = item.Price,
                SKU = item.SKU,
                ItemType = item.ReferenceType,
                ItemUUID = item.UUID,
                UserUUID = sm.GetSession(_sessionKey)?.UserUUID,
                DateAdded = DateTime.UtcNow,
                DateCreated = DateTime.UtcNow,
                RoleOperation = ">=1",
                RoleWeight = 1,
                SessionKey = _sessionKey,
                ShoppingCartUUID = cartUUID


                //        Status = StoreFlag.PayStatus.PendingIncome <= todo change to incart? status should be awaitingpayment
            };

            //if(!item.Virtual)
            //  cartItem.ShippingMethodUUID = this.CheapestShipping()?.UUID;//TODO IMPLEMENT THIS get the cheapest shipping method for our user

            float remainingStock = 0;

            using (var transactionScope = new TransactionScope())
            using (var dbContext = new TreeMonDbContext(this._connectionKey))
            {
                try
                {
                    if (!item.Virtual)
                    {
                        remainingStock = item.Quantity - quantity;
                        if (remainingStock < 0)
                        {
                            //todo alert admin
                            return ServiceResponse.Error("Not enough remaining stock. Adjust the quantity if it's greater than one.");
                        }
                        //Update the inventory this way we can  make sure next customer doesn't attempt to buy something not available (possibly).
                        item.Quantity = remainingStock;
                        if (dbContext.Update<InventoryItem>(item) <= 0)
                        {
                            _logger.InsertError("Failed to update inventory. " + item.UUID, "StoreManager", MethodInfo.GetCurrentMethod().Name);
                            return ServiceResponse.Error("Failed to update inventory.");
                        }
                    }

                    if (!dbContext.Insert<ShoppingCartItem>(cartItem))
                    {
                        _logger.InsertError("Failed to insert cartItem. " + cartItem.UUID, "StoreManager", MethodInfo.GetCurrentMethod().Name);
                    }
                    transactionScope.Complete();
                }
                catch (Exception ex)
                {
                    _logger.InsertError(ex.Message, "StoreManager", MethodInfo.GetCurrentMethod().Name);
                }
            }
         
            return ServiceResponse.OK();
        }

        public ServiceResult AddCartItem(string cartUUID, string cartItemUUID, float quantity)
        {
            InventoryManager inventoryManager = new InventoryManager(this._connectionKey, _sessionKey);
            ShoppingCartItem cartItem = this.GetCartItem(cartItemUUID);

            if (cartItem == null)
                return ServiceResponse.Error("Item not found in cart.");

            InventoryItem item = (InventoryItem)inventoryManager.Get(cartItem.ItemUUID);
            if (item == null)
                return ServiceResponse.Error("Product wasn't found.");
            
           

            int itemsInCart = 0;
            float remainingStock = 0;
            if (!item.Virtual)
            {
                if (item.Quantity <= 0)
                    return ServiceResponse.Error("The product " + item.Name + " is sold out.");
                //If the amount requested is greater than whats in stock then set the ammount ordered to the amount on hand.
                if (item.Quantity < quantity)
                {
                    quantity = item.Quantity;
                }

                cartItem.Quantity += quantity;//add to cart quantity
                item.Quantity -= quantity; //remove from inventory
            }

            User u = this.GetUser(_sessionKey);
            string trackingID = u == null ? _sessionKey : u.UUID;

           using (var transactionScope = new TransactionScope())
            using (var dbContext = new TreeMonDbContext(this._connectionKey))
            {
                try
                {
                    if (cartItem.Quantity <= 0 && dbContext.Delete<ShoppingCartItem>(cartItem) <= 0) {
                        _logger.InsertError("Failed to delete shopping cart item. " + cartItem.UUID, "StoreManager", MethodInfo.GetCurrentMethod().Name);
                        return ServiceResponse.Error("Failed to update shopping cart.");
                    }
                    ////else
                    ////{
                    ////    int changeCount = dbContext.Update<ShoppingCartItem>(cartItem);
                  
                    ////}

                    if (dbContext.Update<InventoryItem>(item) <= 0) {
                        _logger.InsertError("Failed to update inventory. " + item.UUID, "StoreManager", MethodInfo.GetCurrentMethod().Name);
                        return ServiceResponse.Error("Failed to update inventory.");
                    }

                    transactionScope.Complete();
                    remainingStock = item.Quantity;
                }
                catch (Exception ex) {
                    _logger.InsertError(ex.Message, "StoreManager", MethodInfo.GetCurrentMethod().Name);
                }
                itemsInCart = dbContext.GetAll<ShoppingCartItem>().Count(w => w.UserUUID == trackingID);
            }
            return ServiceResponse.OK("", "{ \"CartUUID\" :\"" + trackingID + "\",    \"CartItemUUID\" :\"" + cartItem.UUID + "\", \"RemainingStock\" : \"" + remainingStock + "\", \"IsVirtual\" : \"" + item.Virtual + "\" , \"ItemsInCart\" : \"" + itemsInCart + "\" }");
        }

        public ServiceResult DeleteCartItem(string cartItemUUID)
        {
            InventoryManager inventoryManager = new InventoryManager(this._connectionKey, _sessionKey);
            ShoppingCartItem cartItem = this.GetCartItem(cartItemUUID);

            if (cartItem == null)
                return ServiceResponse.Error("Item not found in cart.");

            InventoryItem item = (InventoryItem)inventoryManager.Get(cartItem.ItemUUID);
            if (item == null)
                return ServiceResponse.Error("Product wasn't found.");

            if (!item.Virtual)
            {
                item.Quantity += cartItem.Quantity; //add the item back to inventory.
            }

            using (var transactionScope = new TransactionScope())
            using (var dbContext = new TreeMonDbContext(this._connectionKey))
            {
                try
                {
                    if (dbContext.Delete<ShoppingCartItem>(cartItem) <= 0) {
                        _logger.InsertError("Failed to delete shopping cart item. " + cartItem.UUID, "StoreManager", MethodInfo.GetCurrentMethod().Name);
                        return ServiceResponse.Error("Failed to delete shopping cart.");
                    }

                    if (dbContext.Update<InventoryItem>(item) <= 0) {
                        _logger.InsertError("Failed to update inventory. " + item.UUID, "StoreManager", MethodInfo.GetCurrentMethod().Name);
                        return ServiceResponse.Error("Failed to update inventory.");
                    }
                    transactionScope.Complete();
                }
                catch (Exception ex)
                {
                    _logger.InsertError(ex.Message, "StoreManager", MethodInfo.GetCurrentMethod().Name);
                    return ServiceResponse.Error("Failed to delete item.");
                }
            }
            return ServiceResponse.OK();
        }

        public ShoppingCartItem GetCartItem(string cartItemUUID)
        {
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                return context.GetAll<ShoppingCartItem>().FirstOrDefault(w => w.UUID == cartItemUUID);
            }
        }

        public List<dynamic> GetItemsInCart(string shoppingCartUUID)
        {
            if (string.IsNullOrWhiteSpace(shoppingCartUUID))
                return new List<dynamic>();
            try
            {
                using (var context = new TreeMonDbContext(this._connectionKey))
                {


                    var res = context.GetAll<ShoppingCartItem>().Where(w => w.ShoppingCartUUID == shoppingCartUUID)
                                  .Join(context.GetAll<InventoryItem>(),
                                      cartItem => cartItem.ItemUUID, //cartItem.ItemType
                                      invItem => invItem.UUID, //invItem.ReferenceType
                                      (cartItem, invItem) => new { cartItem, invItem })
                                       .Select(s => new
                                       {
                                           Name = s.invItem.Name,
                                           Weight = s.invItem.Weight,
                                           WeightUOM = context.GetAll<UnitOfMeasure>().FirstOrDefault(w => w.UUID == s.invItem.UOMUUID)?.Name,             // s.invItem.Name,
                                           Virtual = s.invItem.Virtual,
                                           Image = s.invItem.Image,
                                           CartItemUUID = s.cartItem.UUID,
                                           Quantity = s.cartItem.Quantity,
                                           Price = s.cartItem.Price,
                                           TotalPrice = s.cartItem.TotalPrice,
                                           UserUUID = s.cartItem.UserUUID,
                                           ItemUUID = s.cartItem.ItemUUID,
                                           ItemType = s.cartItem.ItemType
                                       }).Cast<dynamic>().ToList();
                    ////.Join(context.GetAll<UnitOfMeasure>(),
                    ////     ii => ii.invItem.UOMUUID,
                    ////     uom => uom.UUID,
                    ////    (uom, ii) => new { uom, ii })
                    ////.Select(s => new {
                    ////    Name = s.uom.invItem.Name,
                    ////    Weight = s.uom.invItem.Weight,
                    ////    WeightUOM = s.ii.Name,
                    ////    Virtual = s.uom.invItem.Virtual,
                    ////    Image = s.uom.invItem.Image,
                    ////    CartItemUUID = s.uom.cartItem.UUID,
                    ////    Quantity = s.uom.cartItem.Quantity,
                    ////    Price = s.uom.cartItem.Price,
                    ////    TotalPrice = s.uom.cartItem.TotalPrice,
                    ////    UserUUID = s.uom.cartItem.UserUUID,
                    ////    ItemUUID = s.uom.cartItem.ItemUUID,
                    ////    ItemType = s.uom.cartItem.ItemType
                    ////}).Cast<dynamic>().ToList();
                    return res;
                }
            }
            catch (Exception ex)
            {
                this._logger.InsertError(ex.Message, "StoreManager", "GetItemsInCart:" + shoppingCartUUID);
            }
            return new List<dynamic>();
        }

    

        /// <summary>
        /// Gets the shipping methods.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public List<ShippingMethod> GetShippingMethods()
        {
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                return context.GetAll<ShippingMethod>().OrderBy(sm => sm.Price).ToList();
            }
        }

        /// <summary>
        /// Cheapests the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public ShippingMethod CheapestShipping()
        {
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                return context.GetAll<ShippingMethod>().OrderBy(sm => sm.Price).FirstOrDefault();
            }
        }


        //pass cart by ref because it persists in the session, so we want to update the
        //cart in case something happens and we need to go back and to update/insert.
        //userId is whom the order is for,this is using current session userid.
        // If an admin created the order we'll have to create a 
        //new order function CreateOrderFor(userId )
        //
        public ServiceResult ProcessPayment(CartView cart, string ipAddress)
        {
            if (cart == null)
                return ServiceResponse.Error("Invalid  check out form.");


            if (string.IsNullOrEmpty(cart.FinanceAccountUUID)) {
                return ServiceResponse.Error("You must select a payment method.");
            }

            User user = this.GetUser(this.SessionKey);
            Order order = new Order();
            FinanceAccount financeAccount = new FinanceAccount();

            try {
             
                using (var transactionScope = new TransactionScope())
                using (var context = new TreeMonDbContext(this._connectionKey))
                {
                    if (user == null) {  //use form.email to see if there is already an account. if not create one.
                        user = context.GetAll<User>().FirstOrDefault(w => w.UUID == cart.UserUUID);
                    }

                    if (user == null) {
                        return ServiceResponse.Error("You must login or create an account.");
                    }

                     financeAccount = context.GetAll<FinanceAccount>().FirstOrDefault(w => w.UUID == cart.FinanceAccountUUID);

                    if (financeAccount == null) {
                        return ServiceResponse.Error("You must select a payment method.");
                    }

                    #region old btc code
                    ////If btc I recall reading optimally you want a new btc address for each transaction, the send the btc to your main address after full payment is made.
                    ////Get the account the currency is going to be paid to.
                    ////   FinanceAccount payToAcct = new FinanceAccount();
                    ////      form.PayTypeUUID    get the finance account for paytype. so if payment type is btc, get the default or active account for the btc.
                    ////    string accountNumber = "";
                    ////    if (PayType.Symbol.EqualsIgnoreCase("BTC" || PayType.Symbol.EqualsIgnoreCase("BTCT")
                    ////    {
                    ////        payToAcct = CreateNewBtcAccount();
                    ////        if (payToAcct == null)
                    ////        {
                    ////            Message = "Could not create an account for Payment type:" + paymentTypeId.ToString() + " is test:" + Globals.UsePaymentTestnet.ToString();
                    ////            LogQueries.InsertError(Message, className, MethodInfo.GetCurrentMethod().Name);
                    ////            Debug.Assert(false, Message);
                    ////            return false;
                    ////        }
                    ////        accountNumber = payToAcct.AccountNumber;
                    ////        AccountImage = payToAcct.ImageUrl;   //Market.GetQrCodePath(accountNumber, PayType.Symbol), true);
                    ////    }
                    #endregion


                    #region Affiliate process. 
                    ////todo move to affiliate manager. Also this uses the parent id. we may want to use another refernce since we have accounts now that can establish a heirarchy
                    //////If the current user has a parent id (ie under a user) then get that users affiliate account info so they
                    //////get credit for the sale.
                    ////int affiliateId = -1;
                    ////string type = string.Empty;
                    ////if (currentUser.ParentId > 0)
                    ////{
                    ////    Omni.Models.Users.User parentUser = new UserManager().Get(currentUser.ParentId);
                    ////    if (parentUser != null && parentUser.IsBanned == false && parentUser.IsApproved == true)
                    ////    {
                    ////        Affiliate aff = UserQueries.GetAll<Affiliate>().FirstOrDefault(af => af.UserId == currentUser.ParentId);
                    ////        if (aff != null)
                    ////        {
                    ////            affiliateId = aff.Id;
                    ////            type = "affiliate";
                    ////        }
                    ////    }
                    ////}
                    #endregion

                    List<ShoppingCartItem> cartItems = context.GetAll<ShoppingCartItem>().Where(w => w.ShoppingCartUUID == cart.UUID).ToList();

                    order = context.GetAll<Order>().OrderByDescending(ob => ob.DateCreated)
                        .FirstOrDefault(w => w.UserUUID == cart.UserUUID && w.CartUUID == cart.UUID);

                    Debug.Assert(false, "TODO verify not getting duplicate rules.");

                    List<PriceRuleLog> priceRules = cart.PriceRules;
                   ////todo get mandatory price rules       // todo implement shipping/delivery and make sure it's tracked properly. cart.ShippingMethodUUID 
                   ////List<PriceRule> mandatoryRules = context.GetAll<PriceRule>()
                   ////                                    .Where( w => w.AccountUUID == user.AccountUUID && 
                   ////                                            w.Mandatory == true && w.Deleted == false  &&
                   ////                                            priceRules.Any( a => a.PriceRuleUUID != w.UUID)
                   ////                                            ).ToList();
                   ////priceRules.AddRange(mandatoryRules);

                    decimal subTotal = this.GetSubtotal(cartItems);
                    //todo validate the price rules (only one coupon, expiration date hasn't exceeded, max usage hasn't exceeded..)
                    decimal total =  MathHelper.CalcAdjustment(subTotal,ref priceRules);
                    
                    if (order == null)
                    {
                        order = new Order()
                        {
                            CartUUID = cart.UUID,
                            AddedBy = user.UUID,
                            Status = StoreFlag.OrderStatus.Recieved,
                            PayStatus = LedgerFlag.Status.PendingIncome,
                            UserUUID = user.UUID,
                            SubTotal = subTotal,
                            Total = total,
                            CurrencyUUID = financeAccount.CurrencyUUID,
                            DateCreated = DateTime.UtcNow,
                            AccountUUID =  user.AccountUUID,
                            Active = true,
                            FinancAccountUUID = financeAccount.UUID
                        };
                        context.Insert<Order>(order);
                    }

                    if (priceRules.Count > 0)
                    {
                        priceRules.ForEach(x =>
                        {
                            x.TrackingId = order.UUID;
                            x.TrackingType = "Order";

                            if (!context.Insert<PriceRuleLog>(x))
                            {
                                this._logger.InsertError("Failed to insert PriceRuleLog", "StoreManager", "ProcessPayment cart:" + cart.UUID + " PriceRuleLog:" + x.UUID);
                            }
                        });
                    }

                    #region todo Currency conversion. May not be needed or move to somewhere else.
                    ////// get payment type. then use symbol for conversion
                    //////paymentTypeId 
                    ////PayType = StoreQueries.GetPaymentTypes().FirstOrDefault(pt => pt.Id == paymentTypeId);
                    decimal orderTotalConverted = order.Total; ////PayTypeTotal = TODO => Market.ConvertCurrencyAmmount(Globals.CurrencySymbol, PayType.Symbol, Cart.Total);
                    ////decimal orderSubTotalConverted = 0;   //PayTypeSubTotal

                    ////if (order.Total == order.SubTotal)
                    ////    orderSubTotalConverted = orderTotalConverted;
                    ////else
                    ////    orderSubTotalConverted = order.SubTotal; ////TODO =>  // Market.ConvertCurrencyAmmount(Globals.CurrencySymbol, PayType.Symbol, Cart.SubTotal);
                    #endregion

                    //Clear order items and refresh with members selected items.
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@ORDERUUID", order.UUID);
                    context.Delete<OrderItem>("WHERE OrderUUID=@ORDERUUID", parameters);

                    foreach (ShoppingCartItem item in cartItems)
                    {
                        InventoryItem p = context.GetAll<InventoryItem>().FirstOrDefault(w => w.UUID == item.ItemUUID);

                        if (p == null)
                        {
                            Debug.Assert(false, "PRODUCT NOT FOUND");
                            _logger.InsertError("Product not found:" + item.UUID + " Name:" + item.Name,"StoreManager", "ProcessPayment");
                            continue;
                        }
                        OrderItem orderItem = new OrderItem()
                        {
                            OrderUUID = order.UUID,
                            UserUUID = user.UUID,
                            Quantity = item.Quantity,
                            Price = item.Price,
                            SKU = item.SKU,
                            Status = LedgerFlag.Status.PendingIncome, // PaymentStatusUUID
                            RoleWeight = item.RoleWeight,
                            RoleOperation = item.RoleOperation,
                            CreatedBy = item.UserUUID,
                            DateCreated = DateTime.UtcNow,
                            AccountUUID = user == null ? "" : user.AccountUUID,
                            Name = item.Name,
                            ProductUUID = item.ItemUUID,
                            ProductType = item.ItemType,
                            Image = item.Image,
                            UnitsInProduct = p.UnitsInProduct,
                            UnitsRemaining = p.UnitsInProduct * item.Quantity,
                            UnitType = p.UnitType,
                            IsVirtual = p.Virtual,
                            AccessGranted = false,
                            AccessExpires = DateTime.UtcNow.AddDays(120)//todo make configurable
                            //TODO  AffiliateUUID
                        };
                        if (!context.Insert<OrderItem>(orderItem))
                        {
                            this._logger.InsertError("Failed to insert OrderItem", "StoreManager", "ProcessPayment cart:" + cart.UUID + " PriceRuleLog:" + orderItem.UUID);
                        }
                    }
                    Currency currency = context.GetAll<Currency>().FirstOrDefault(w => w.UUID == financeAccount.CurrencyUUID);

                    //Add an initial payment record so we know who owes what
                    FinanceAccountTransaction payment = new FinanceAccountTransaction()
                    {
                        AccountUUID = user.AccountUUID,
                        AccountEmail = financeAccount.Email,
                        DateCreated = DateTime.UtcNow,
                        Image = financeAccount.Image,
                        CurrencyUUID = financeAccount.CurrencyUUID,
                        CustomerIp = ipAddress,
                        CreationDate = DateTime.Now,
                        LastPaymentStatusCheck = DateTime.UtcNow,
                        OrderUUID = order.UUID,
                        Balance = orderTotalConverted,
                        AmountTransferred = 0,
                        TransactionDate = DateTime.UtcNow,
                        TransactionType = LedgerFlag.TransactionTypes.Credit,
                        Status = LedgerFlag.Status.PendingIncome,
                       
                        SelectedPaymentTypeTotal = orderTotalConverted,
                        UserUUID = user.UUID,
                        //   PayFromAccountUUID = todo this is the customers account id. won't need it for now. we could also use it to set up accounts where users
                        //                          can order and be billed later.
                        FinanceAccountUUID = financeAccount.UUID,
                        PayToAccountUUID = financeAccount.AccountNumber, //todo this should be the store account", 
                        PaymentTypeUUID = cart.PaymentGateway,
                        SelectedPaymentTypeSymbol = currency?.Symbol//yes I used a null check operator here just in case. It's not critical piece of info and we don't want to stop operations because of it.
                        // = affiliateId,
                    };
                    if (!context.Insert<FinanceAccountTransaction>(payment))
                    {
                        this._logger.InsertError("Failed to insert FinanceAccountTransaction", "StoreManager", "ProcessPayment cart:" + cart.UUID + " PriceRuleLog:" + payment.UUID);
                    }
                   
                    //order has been placed so remove the cart and contents
                    DynamicParameters cartParams = new DynamicParameters();
                    parameters.Add("@UUID", cart.UUID);
                    context.Delete<ShoppingCart>("WHERE UUID=@UUID", cartParams);

                    DynamicParameters cartItemParams = new DynamicParameters();
                    parameters.Add("@CARTUUID", cart.UUID);
                    context.Delete<ShoppingCartItem>("WHERE ShoppingCartUUID=@CARTUUID", cartItemParams);
                    
                    transactionScope.Complete();
                }
            } catch (Exception ex) {
                Debug.Assert(false, ex.Message);
                _logger.InsertError(ex.Message, "StoreManager", "ProcessPayment");
                return ServiceResponse.Error("Failed to process payment.");   
            }

            //todo get app setting if pos system don't show this message, 
            return SendEmail(cart, order, financeAccount, user.Email, StoreFlag.OrderStatus.Recieved);
        }

        public decimal GetSubtotal(List<ShoppingCartItem> cartItems)
        {
            decimal subTotal = 0;
            for (var i = 0; i < cartItems.Count; i++)
            {
               subTotal += cartItems[i].Price * (decimal)cartItems[i].Quantity;
              
            }
            return subTotal;
        }

        private ServiceResult SendEmail(CartView cart, Order order, FinanceAccount account, string customerEmail, string status)
        {
            if (cart == null){
                return ServiceResponse.Error("Could not send email, cart was not set.");
            }

            AppManager am = new AppManager(this._connectionKey, "web", this._sessionKey);
            string domain = am.GetSetting("SiteDomain")?.Value;
            string emailSubject = "";
            string emailContent = "";

            //todo put this in another function 
            #region get email content function 

            switch (status)
            {
                case StoreFlag.OrderStatus.Recieved:
                    emailSubject = "Your " + domain + " order has been recieved.";

                    DocumentManager dm = new DocumentManager(this._connectionKey, SessionKey);
                    emailContent = dm.GetTemplate("EmailOrderReceived").Result?.ToString();

                    if (string.IsNullOrWhiteSpace(emailContent))
                        return ServiceResponse.Error("Failed to send email. Document not found.");

                    //use view cart for details
                    emailContent = emailContent.Replace("[Order.OrderID]", order.UUID);
                    emailContent = emailContent.Replace("[Order.AddedDate]", order.DateCreated.ToShortDateString());
                    //See below: emailContent = emailContent.Replace( "[Order.Total]"                  , 
                    emailContent = emailContent.Replace("[PaymentType.Title]", cart.PaidType); 
                    emailContent = emailContent.Replace("[StoreManager.PayType]", account.CurrencyName); 
                    emailContent = emailContent.Replace("[StoreManager.PayTypeTotal]", order.Total.ToString()); 
                    ////emailContent = emailContent.Replace( "                               ,PayTypeSubTotal);
                    //// emailContent = emailContent.Replace("[PayType.Address]", account. PayType.Address);
                    emailContent = emailContent.Replace("[PayType.PictureUrl]",account.Image );
                    emailContent = emailContent.Replace("[Settings.CurrencySymbol]",am.GetSetting("default.currency.symbol").Value);
                    emailContent = emailContent.Replace("[Settings.SiteDomain]", domain);

                    ////todo  paytype.address and qr code for btc.
                    ////todo bookmark latest currency symbol
                    //// string validationCode = Cipher.RandomString(12);  
                    ////   emailContent = emailContent.Replace("[Url.Unsubscribe]", "http://" + domain + "/FinanceAccount/ValidateEmail/?type=mbr&operation=mdel&code=" + validationCode);

                    StringBuilder ShoppingCartItemsList = new StringBuilder();

                    foreach (dynamic item in cart.CartItems)
                    {
                        ShoppingCartItemsList.Append("<tr id=\"[item-ShoppingCartItem.Product.Id]\">".Replace("[item-ShoppingCartItem.Product.ProductID]", item.ItemUUID.ToString()));
                        ShoppingCartItemsList.Append("<td align=\"center\">[ShoppingCartItem.Title]</td>".Replace("[ShoppingCartItem.Title]", item.Name.ToString()));
                        ShoppingCartItemsList.Append("<td align=\"center\">[ShoppingCartItem.Quantity]</td>".Replace("[ShoppingCartItem.Quantity]", item.Quantity.ToString()));
                        ShoppingCartItemsList.Append("<td align=\"center\">[ShoppingCartItem.Price]</td></tr>".Replace("[ShoppingCartItem.Price]", item.Price.ToString("c")));
                    }

                    emailContent = emailContent.Replace("[ShoppingCartItemsList]", ShoppingCartItemsList.ToString());
                    emailContent = emailContent.Replace("[Order.SubTotal]", order.SubTotal.ToString("c"));
                    emailContent = emailContent.Replace("[Order.Total]", order.Total.ToString("c"));
                    #endregion
                    break;
            }
            string appKey =am.GetSetting("AppKey")?.Value;
            string emailPassword = am.GetSetting("EmailHostPassword")?.Value;

            SMTP mailServer = new SMTP(this._connectionKey, new Models.Services.EmailSettings()
            {
                HostPassword = Cipher.Crypt(appKey, emailPassword, false),
                EncryptionKey = am.GetSetting("AppKey")?.Value,
                HostUser = am.GetSetting("EmailHostUser")?.Value,
                MailHost = am.GetSetting("MailHost")?.Value,
                MailPort = StringEx.ConvertTo<int>(am.GetSetting("MailPort")?.Value),
                SiteDomain = am.GetSetting("SiteDomain")?.Value,
                SiteEmail = am.GetSetting("SiteEmail")?.Value,
                UseSSL = StringEx.ConvertTo<bool>(am.GetSetting("UseSSL")?.Value)
            });
            MailMessage mail = new MailMessage();
            try
            {
                mail.From = new MailAddress(am.GetSetting("SiteEmail")?.Value);
                mail.ReplyToList.Add(mail.From);
                mail.To.Add(customerEmail);
                mail.Subject = emailSubject;
                mail.Body = emailContent;
                mail.IsBodyHtml = true;
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
                this._logger.InsertError(ex.Message, "StoreManager", "SendMail");
                return ServiceResponse.Error("Failed to send email. ");
            }

            ServiceResult res = mailServer.SendMail(mail);
            if (res.Code != 200)
            {
                Debug.Assert(false, mailServer.ErrorMessage);
                this._logger.InsertError(mailServer.ErrorMessage, "StoreManager", "SendMail");
                return ServiceResponse.Error("Failed to send email. ");
            }
            return ServiceResponse.OK();
        }

   
    }
}
