// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeMon.Data.Logging.Models;
using TreeMon.Models;
using TreeMon.Models.App;
using TreeMon.Models.Equipment;
using TreeMon.Models.Events;
using TreeMon.Models.Finance;
using TreeMon.Models.Finance.PaymentGateways;
using TreeMon.Models.General;
using TreeMon.Models.Geo;
using TreeMon.Models.Inventory;
using TreeMon.Models.Logging;
using TreeMon.Models.Medical;
using TreeMon.Models.Membership;
using TreeMon.Models.Plant;
using TreeMon.Models.Store;
using TreeMon.Utilites.Extensions;

namespace TreeMon.Data.Helpers
{
    public static class DatabaseEx
    {
        public static Dictionary<string, object> TypeTables = new Dictionary<string, object>();
        #region Table Functions 
        public static List<string> GetDataTypes()
        {
            if (TypeTables.Count == 0)
                LoadTableNames();
            List<string> res = new List<string>();
            foreach (var typeEntry in TypeTables)
            {
                dynamic table = typeEntry.Value;
                try
                {
                    res.Add(table?.UUIDType);
                }
                catch
                {//
                }
            }
            #region future
            ////if (ConnectionSettings == null)
            ////    return res;
            ////string sql = "";
            ////switch (_providerType)
            ////{
            ////    case "SQLITE":
            ////        Debug.Assert(false, "NOT IMPLEMENTED");
            ////        //sql = "SELECT tbl_name FROM sqlite_master WHERE type='table'";
            ////        //res = _sqliteContext.Table<sqlite_master>().AsQueryable().Select(s => s.tbl_name).ToList();
            ////        ////using (SQLiteConnection conn = new SQLiteConnection(PathToDatabase))
            ////        ////{ 
            ////        ////    res = conn.Table<sqlite_master>().AsQueryable().Select(s => s.tbl_name).ToList();
            ////        ////}
            ////        break;
            ////    case "MYSQL":
            ////        Debug.Assert(false, "NOT IMPLEMENTED");
            ////        //sql = "SELECT * FROM information_schema.tables";
            ////        //res =  Select<string>(sql, new object[] { }).ToList();
            ////        break;
            ////    case "MSSQL":
            ////        // Debug.Assert(false, "NOT IMPLEMENTED");
            ////        //SQL Server 2005, 2008, 2012 or 2014:
            ////        // sql = "SELECT * FROM information_schema.tables";                //SQL Server 2000: SELECT* FROM sysobjects WHERE xtype = 'U'
            ////        //res = Select<string>(sql, new object[] { }).ToList();
            ////        break;
            ////}
            #endregion

            return res;
        }

        public static List<string> GetTableNames()
        {
            if (TypeTables.Count == 0)
                LoadTableNames();

            List<string> res = TypeTables.Select(t => t.Key).ToList();
            return res;
        }

        public static string GetTableName<T>() where T : class
        {
            string name = "";
            object[] attributes = typeof(T).GetCustomAttributes(true);

            foreach (object attribute in attributes)
            {
                string typeName = attribute.GetType().Name;

                if (typeName == "TableAttribute") //this is the [Table("Accounts")] at the top of the class, it tells what table to look at.
                {
                    System.ComponentModel.DataAnnotations.Schema.TableAttribute ta = (System.ComponentModel.DataAnnotations.Schema.TableAttribute)attribute;
                    name = ta.Name;
                }
            }

            //if no table attribute was found then create the table based on the name of the class.
            //
            if (string.IsNullOrEmpty(name))
            {
                name = typeof(T).Name;

                //If it's namespaced then get the very last element in the namespace (class name).
                if (name.Contains("."))
                {
                    string[] tokens = name.Split('.');
                    name = tokens[tokens.Length - 1];
                }
            }
            return name;
        }

        public static string GetTableName(string typeName)
        {
            string res = string.Empty;
            if (string.IsNullOrWhiteSpace(typeName))
                return res;

            if (TypeTables.Count == 0)
                LoadTableNames();

            foreach (dynamic typeEntry in TypeTables)
            {
                try
                {
                    string type = typeEntry.Value.UUIDType?.ToUpper();
                    if (string.IsNullOrWhiteSpace(type))
                        continue;

                    //inside try cause not all objects use the uuidtype
                    if (type.EqualsIgnoreCase(typeName))
                    {
                        res = typeEntry.Key;
                        break;
                    }
                }
                catch
                {
                    continue;
                }
            }

            return res;
        }

        public static object GetTableObject(string tableName)
        {
            if (TypeTables.Count == 0)
                LoadTableNames();

            if (TypeTables.ContainsKey(tableName))
                return TypeTables[tableName];

            return null;
        }

        public static string GetTableType(string tableName)
        {
            if (TypeTables.Count == 0)
                LoadTableNames();

            if (TypeTables.ContainsKey(tableName))
                return ((dynamic)TypeTables[tableName])?.UUIDType.ToUpper();

            return string.Empty;
        }

        public static void LoadTableNames()
        {
            #region General

            if (!TypeTables.ContainsKey(GetTableName<TreeMon.Models.General.Attribute>())) { TypeTables.Add(GetTableName<TreeMon.Models.General.Attribute>(), new TreeMon.Models.General.Attribute()); }

            if (!TypeTables.ContainsKey(GetTableName<Category>())) { TypeTables.Add(GetTableName<Category>(), new Category()); }

            if (!TypeTables.ContainsKey(GetTableName<UnitOfMeasure>())) { TypeTables.Add(GetTableName<UnitOfMeasure>(), new UnitOfMeasure()); }

            if (!TypeTables.ContainsKey(GetTableName<StatusMessage>())) { TypeTables.Add(GetTableName<StatusMessage>(), new StatusMessage()); }

            #endregion

            #region App

            if (!TypeTables.ContainsKey(GetTableName<AppInfo>())) { TypeTables.Add(GetTableName<AppInfo>(), new AppInfo()); }

            if (!TypeTables.ContainsKey(GetTableName<Setting>())) { TypeTables.Add(GetTableName<Setting>(), new Setting()); }

            if (!TypeTables.ContainsKey(GetTableName<UserSession>())) { TypeTables.Add(GetTableName<UserSession>(), new UserSession()); }

            #endregion

            #region Medical

            if (!TypeTables.ContainsKey(GetTableName<AnatomyTag>())) { TypeTables.Add(GetTableName<AnatomyTag>(), new AnatomyTag()); }

            if (!TypeTables.ContainsKey(GetTableName<Anatomy>())) { TypeTables.Add(GetTableName<Anatomy>(), new Anatomy()); }

            if (!TypeTables.ContainsKey(GetTableName<SideAffect>())) { TypeTables.Add(GetTableName<SideAffect>(), new SideAffect()); }

            if (!TypeTables.ContainsKey(GetTableName<Symptom>())) { TypeTables.Add(GetTableName<Symptom>(), new Symptom()); }

            if (!TypeTables.ContainsKey(GetTableName<SymptomLog>())) { TypeTables.Add(GetTableName<SymptomLog>(), new SymptomLog()); }

            if (!TypeTables.ContainsKey(GetTableName<DoseLog>())) { TypeTables.Add(GetTableName<DoseLog>(), new DoseLog()); }

            #endregion

            #region Equipment
            if (!TypeTables.ContainsKey(GetTableName<Ballast>())) { TypeTables.Add(GetTableName<Ballast>(), new Ballast()); }

            if (!TypeTables.ContainsKey(GetTableName<Bulb>())) { TypeTables.Add(GetTableName<Bulb>(), new Bulb()); }

            if (!TypeTables.ContainsKey(GetTableName<Fan>())) { TypeTables.Add(GetTableName<Fan>(), new Fan()); }

            if (!TypeTables.ContainsKey(GetTableName<Filter>())) { TypeTables.Add(GetTableName<Filter>(), new Filter()); }

            if (!TypeTables.ContainsKey(GetTableName<Pump>())) { TypeTables.Add(GetTableName<Pump>(), new Pump()); }

            if (!TypeTables.ContainsKey(GetTableName<Vehicle>())) { TypeTables.Add(GetTableName<Vehicle>(), new Vehicle()); }

            if (!TypeTables.ContainsKey(GetTableName<InventoryItem>())) { TypeTables.Add(GetTableName<InventoryItem>(), new InventoryItem()); }
            #endregion

            #region Events

            if (!TypeTables.ContainsKey(GetTableName<Notification>())) { TypeTables.Add(GetTableName<Notification>(), new Notification()); }

            if (!TypeTables.ContainsKey(GetTableName<Reminder>())) { TypeTables.Add(GetTableName<Reminder>(), new Reminder()); }

            if (!TypeTables.ContainsKey(GetTableName<ReminderRule>())) { TypeTables.Add(GetTableName<ReminderRule>(), new ReminderRule()); }

            #endregion

            #region Finance

            if (!TypeTables.ContainsKey(GetTableName<Currency>())) { TypeTables.Add(GetTableName<Currency>(), new Currency()); }

            if (!TypeTables.ContainsKey(GetTableName<Fee>())) { TypeTables.Add(GetTableName<Fee>(), new Fee()); }

            if (!TypeTables.ContainsKey(GetTableName<FinanceAccount>())) { TypeTables.Add(GetTableName<FinanceAccount>(), new FinanceAccount()); }

            if (!TypeTables.ContainsKey(GetTableName<FinanceAccountTransaction>())) { TypeTables.Add(GetTableName<FinanceAccountTransaction>(), new FinanceAccountTransaction()); }

            if (!TypeTables.ContainsKey(GetTableName<PriceRule>())) { TypeTables.Add(GetTableName<PriceRule>(), new PriceRule()); }

            if (!TypeTables.ContainsKey(GetTableName<PaymentGatewayLog>())) { TypeTables.Add(GetTableName<PaymentGatewayLog>(), new PaymentGatewayLog()); }

            #endregion

            #region Geo

            if (!TypeTables.ContainsKey(GetTableName<Location>())) { TypeTables.Add(GetTableName<Location>(), new Location()); }

            #endregion

            #region Logging

            if (!TypeTables.ContainsKey(GetTableName<AccessLog>())) { TypeTables.Add(GetTableName<AccessLog>(), new AccessLog()); }

            if (!TypeTables.ContainsKey(GetTableName<LineItemLog>())) { TypeTables.Add(GetTableName<LineItemLog>(), new LineItemLog()); }

            if (!TypeTables.ContainsKey(GetTableName<MeasurementLog>())) { TypeTables.Add(GetTableName<MeasurementLog>(), new MeasurementLog()); }

            if (!TypeTables.ContainsKey(GetTableName<LogEntry>())) { TypeTables.Add(GetTableName<LogEntry>(), new LogEntry()); }

            #endregion

            #region Membership

            if (!TypeTables.ContainsKey(GetTableName<Account>())) { TypeTables.Add(GetTableName<Account>(), new Account()); }

            if (!TypeTables.ContainsKey(GetTableName<AccountMember>())) { TypeTables.Add(GetTableName<AccountMember>(), new AccountMember()); }

         

            if (!TypeTables.ContainsKey(GetTableName<Credential>())) { TypeTables.Add(GetTableName<Credential>(), new Credential()); }

            if (!TypeTables.ContainsKey(GetTableName<RolePermission>())) { TypeTables.Add(GetTableName<RolePermission>(), new RolePermission()); }

            if (!TypeTables.ContainsKey(GetTableName<PersonalProfile>())) { TypeTables.Add(GetTableName<PersonalProfile>(), new PersonalProfile()); }

            if (!TypeTables.ContainsKey(GetTableName<Profile>())) { TypeTables.Add(GetTableName<Profile>(), new Profile()); }

            if (!TypeTables.ContainsKey(GetTableName<Role>())) { TypeTables.Add(GetTableName<Role>(), new Role()); }

            if (!TypeTables.ContainsKey(GetTableName<User>())) { TypeTables.Add(GetTableName<User>(), new User()); }

            if (!TypeTables.ContainsKey(GetTableName<UserRole>())) { TypeTables.Add(GetTableName<UserRole>(), new UserRole()); }

            if (!TypeTables.ContainsKey(GetTableName<Permission>())) { TypeTables.Add(GetTableName<Permission>(), new Permission()); }

            #endregion

            #region Plant

            if (!TypeTables.ContainsKey(GetTableName<Plant>())) { TypeTables.Add(GetTableName<Plant>(), new Plant()); }

            if (!TypeTables.ContainsKey(GetTableName<Strain>())) { TypeTables.Add(GetTableName<Strain>(), new Strain()); }

            #endregion

            #region Store

            if (!TypeTables.ContainsKey(GetTableName<Product>())) { TypeTables.Add(GetTableName<Product>(), new Product()); }

            if (!TypeTables.ContainsKey(GetTableName<Vendor>())) { TypeTables.Add(GetTableName<Vendor>(), new Vendor()); }

            if (!TypeTables.ContainsKey(GetTableName<ShoppingCart>())) { TypeTables.Add(GetTableName<ShoppingCart>(), new ShoppingCart()); }

            if (!TypeTables.ContainsKey(GetTableName<ShoppingCartItem>())) { TypeTables.Add(GetTableName<ShoppingCartItem>(), new ShoppingCartItem()); }

            if (!TypeTables.ContainsKey(GetTableName<PriceRule>())) { TypeTables.Add(GetTableName<PriceRule>(), new PriceRule()); }

            if (!TypeTables.ContainsKey(GetTableName<PriceRuleLog>())) { TypeTables.Add(GetTableName<PriceRuleLog>(), new PriceRuleLog()); }

            if (!TypeTables.ContainsKey(GetTableName<Order>())) { TypeTables.Add(GetTableName<Order>(), new Order()); }

            if (!TypeTables.ContainsKey(GetTableName<OrderItem>())) { TypeTables.Add(GetTableName<OrderItem>(), new OrderItem()); }

            #endregion

            #region Enterprise

            if (!TypeTables.ContainsKey(GetTableName<ApiKey>())) { TypeTables.Add(GetTableName<ApiKey>(), new ApiKey()); }
            #endregion
        }

        #endregion

    }
}
