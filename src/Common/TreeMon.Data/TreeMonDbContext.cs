// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

using Dapper;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.Annotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using TreeMon.Data.Logging;
using TreeMon.Data.Logging.Models;
using TreeMon.Models;
using TreeMon.Models.App;
using TreeMon.Models.Equipment;
using TreeMon.Models.Event;
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
using TreeMon.Utilites.Security;

namespace TreeMon.Data
{
    public class TreeMonDbContext : DbContext, IDbContext
    {
        #region Properties
        public bool Install { get; set; }
        public string ConnectionKey { get; set; }
        private ConnectionStringSettings ConnectionSettings { get; set; }
        public string _providerType { get; set; }

        /// <summary>
        /// Currently used for the file path of the sqlite database.
        /// </summary>
        private string PathToDatabase { get; set; }

        #endregion

        public static Dictionary<string, object> TypeTables = new Dictionary<string, object>();

        readonly SystemLogger _fileLogger = new SystemLogger(null, true);

        #region Initialization

        public TreeMonDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            Initialize(nameOrConnectionString);
        }

        //Installer constructor
        public TreeMonDbContext( bool install = false)
        {
            this.Install = install;
        }



        protected void Initialize(string nameOrConnectionString)
        {
            if (string.IsNullOrWhiteSpace(nameOrConnectionString))
                return;

            ConnectionKey = nameOrConnectionString;
            try
            {
                ConnectionStringSettingsCollection connStrings = ConfigurationManager.ConnectionStrings;

                foreach (ConnectionStringSettings settings in connStrings)
                {
                    if (settings.Name.EqualsIgnoreCase(nameOrConnectionString )|| settings.ConnectionString == nameOrConnectionString)
                    {
                        ConnectionSettings = settings;
                        break;
                    }
                }

                if (ConnectionSettings != null)
                {
                    if (ConnectionSettings.ProviderName.ToUpper().Contains("SQLITE"))
                    {
                        _providerType = "SQLITE";
                        string directory = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug", "") + "App_Data";

                        AppDomain.CurrentDomain.SetData("DataDirectory", directory);
                        string dataSource = this.ConnectionSettings.ConnectionString.Replace("data source=|DataDirectory|", "").Replace(";", "");
                        PathToDatabase = directory + dataSource;

                        //This gets rid of the provider doesn't support database creation errror.
                        Database.SetInitializer<TreeMonDbContext>(null);
                    }
                    else if (ConnectionSettings.ProviderName.ToUpper().Contains("MYSQL"))
                    {
                        _providerType = "MYSQL";
                       // Database.Connection.Disposed += new EventHandler(DisposedEvent);
                        //SetSqlGenerator(MySql.Data.Entity.MySqlProviderInvariantName.ProviderName, new MySql.Data.Entity.MySqlMigrationSqlGenerator());
                        //SetHistoryContextFactory(MySql.Data.Entity.MySqlProviderInvariantName.ProviderName, (connection, schema) => new MySql.Data.Entity.MySqlHistoryContext(connection, schema));
                        //class decorator 
                        //[DbConfigurationType(typeof(MySql.Data.Entity.MySqlEFConfiguration))]
                        //public class DataAccessBase<T> : DbContext where T : class
                    }
                    else if (ConnectionSettings.ProviderName.ToUpper().Contains("SQLCLIENT"))
                    {
                        _providerType = "MSSQL";
                      //  Database.Connection.Disposed += new EventHandler(DisposedEvent);
                    }
                }
            }
            catch (Exception ex)
            {
                _fileLogger.InsertError(ex.Message, "TreeMonDbContext", "Initialize");
            }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {


            if (!TypeTables.ContainsKey(this.GetTableName<Location>())) { TypeTables.Add(this.GetTableName<Location>(), new Location()); }

            
            if (!TypeTables.ContainsKey(this.GetTableName<TreeMon.Models.General.Attribute>())) { TypeTables.Add(this.GetTableName<TreeMon.Models.General.Attribute>(), new TreeMon.Models.General.Attribute()); }
            if (!TypeTables.ContainsKey(this.GetTableName<Category>())) { TypeTables.Add(this.GetTableName<Category>(), new Category()); }
            if (!TypeTables.ContainsKey(this.GetTableName<UnitOfMeasure>())) { TypeTables.Add(this.GetTableName<UnitOfMeasure>(), new UnitOfMeasure()); }
            if (!TypeTables.ContainsKey(this.GetTableName<StatusMessage>())) { TypeTables.Add(this.GetTableName<StatusMessage>(), new StatusMessage()); }

            if (!TypeTables.ContainsKey(this.GetTableName<AppInfo>())) { TypeTables.Add(this.GetTableName<AppInfo>(), new AppInfo()); }
            if (!TypeTables.ContainsKey(this.GetTableName<Setting>())) { TypeTables.Add(this.GetTableName<Setting>(), new Setting()); }
            if (!TypeTables.ContainsKey(this.GetTableName<UserSession>())) { TypeTables.Add(this.GetTableName<UserSession>(), new UserSession()); }
            if (!TypeTables.ContainsKey(this.GetTableName<AnatomyTag>())) { TypeTables.Add(this.GetTableName<AnatomyTag>(), new AnatomyTag()); }
            if (!TypeTables.ContainsKey(this.GetTableName<Anatomy>())) { TypeTables.Add(this.GetTableName<Anatomy>(), new Anatomy()); }

            if (!TypeTables.ContainsKey(this.GetTableName<SideAffect>())) { TypeTables.Add(this.GetTableName<SideAffect>(), new SideAffect()); }
            if (!TypeTables.ContainsKey(this.GetTableName<Symptom>())) { TypeTables.Add(this.GetTableName<Symptom>(), new Symptom()); }
            if (!TypeTables.ContainsKey(this.GetTableName<SymptomLog>())) { TypeTables.Add(this.GetTableName<SymptomLog>(), new SymptomLog()); }
            if (!TypeTables.ContainsKey(this.GetTableName<DoseLog>())) { TypeTables.Add(this.GetTableName<DoseLog>(), new DoseLog()); }

            if (!TypeTables.ContainsKey(this.GetTableName<Ballast>())) { TypeTables.Add(this.GetTableName<Ballast>(), new Ballast()); }
            if (!TypeTables.ContainsKey(this.GetTableName<Bulb>())) { TypeTables.Add(this.GetTableName<Bulb>(), new Bulb()); }
            if (!TypeTables.ContainsKey(this.GetTableName<Fan>())) { TypeTables.Add(this.GetTableName<Fan>(), new Fan()); }
            if (!TypeTables.ContainsKey(this.GetTableName<Filter>())) { TypeTables.Add(this.GetTableName<Filter>(), new Filter()); }
            if (!TypeTables.ContainsKey(this.GetTableName<Pump>())) { TypeTables.Add(this.GetTableName<Pump>(), new Pump()); }
            if (!TypeTables.ContainsKey(this.GetTableName<Vehicle>())) { TypeTables.Add(this.GetTableName<Vehicle>(), new Vehicle()); }


            if (!TypeTables.ContainsKey(this.GetTableName<Currency>())) { TypeTables.Add(this.GetTableName<Currency>(), new Currency()); }
            if (!TypeTables.ContainsKey(this.GetTableName<Fee>())) { TypeTables.Add(this.GetTableName<Fee>(), new Fee()); }
            if (!TypeTables.ContainsKey(this.GetTableName<FinanceAccount>())) { TypeTables.Add(this.GetTableName<FinanceAccount>(), new FinanceAccount()); }
            if (!TypeTables.ContainsKey(this.GetTableName<FinanceAccountTransaction>())) { TypeTables.Add(this.GetTableName<FinanceAccountTransaction>(), new FinanceAccountTransaction()); }
            if (!TypeTables.ContainsKey(this.GetTableName<PriceRule>())) { TypeTables.Add(this.GetTableName<PriceRule>(), new PriceRule()); }
            if (!TypeTables.ContainsKey(this.GetTableName<PaymentGatewayLog>())) { TypeTables.Add(this.GetTableName<PaymentGatewayLog>(), new PaymentGatewayLog()); }


            if (!TypeTables.ContainsKey(this.GetTableName<Notification>())) { TypeTables.Add(this.GetTableName<Notification>(), new Notification()); }
            if (!TypeTables.ContainsKey(this.GetTableName<Reminder>())) { TypeTables.Add(this.GetTableName<Reminder>(), new Reminder()); }
            if (!TypeTables.ContainsKey(this.GetTableName<ReminderRule>())) { TypeTables.Add(this.GetTableName<ReminderRule>(), new ReminderRule()); }

            if (!TypeTables.ContainsKey(this.GetTableName<AuthenticationLog>())) { TypeTables.Add(this.GetTableName<AuthenticationLog>(), new AuthenticationLog()); }
            if (!TypeTables.ContainsKey(this.GetTableName<LineItemLog>())) { TypeTables.Add(this.GetTableName<LineItemLog>(), new LineItemLog()); }
            if (!TypeTables.ContainsKey(this.GetTableName<MeasurementLog>())) { TypeTables.Add(this.GetTableName<MeasurementLog>(), new MeasurementLog()); }
            if (!TypeTables.ContainsKey(this.GetTableName<EmailLog>())) { TypeTables.Add(this.GetTableName<EmailLog>(), new EmailLog()); }
            if (!TypeTables.ContainsKey(this.GetTableName<LogEntry>())) { TypeTables.Add(this.GetTableName<LogEntry>(), new LogEntry()); }

            if (!TypeTables.ContainsKey(this.GetTableName<Account>())) { TypeTables.Add(this.GetTableName<Account>(), new Account()); }
            if (!TypeTables.ContainsKey(this.GetTableName<AccountMember>())) { TypeTables.Add(this.GetTableName<AccountMember>(), new AccountMember()); }
            if (!TypeTables.ContainsKey(this.GetTableName<Credential>())) { TypeTables.Add(this.GetTableName<Credential>(), new Credential()); }
            if (!TypeTables.ContainsKey(this.GetTableName<RolePermission>())) { TypeTables.Add(this.GetTableName<RolePermission>(), new RolePermission()); }
            if (!TypeTables.ContainsKey(this.GetTableName<Profile>())) { TypeTables.Add(this.GetTableName<Profile>(), new Profile()); }
            if (!TypeTables.ContainsKey(this.GetTableName<Role>())) { TypeTables.Add(this.GetTableName<Role>(), new Role()); }
            if (!TypeTables.ContainsKey(this.GetTableName<User>())) { TypeTables.Add(this.GetTableName<User>(), new User()); }
            if (!TypeTables.ContainsKey(this.GetTableName<UserRole>())) { TypeTables.Add(this.GetTableName<UserRole>(), new UserRole()); }
            if (!TypeTables.ContainsKey(this.GetTableName<Permission>())) { TypeTables.Add(this.GetTableName<Permission>(), new Permission()); }

            if (!TypeTables.ContainsKey(this.GetTableName<Product>())) { TypeTables.Add(this.GetTableName<Product>(), new Product()); }
            if (!TypeTables.ContainsKey(this.GetTableName<Vendor>())) { TypeTables.Add(this.GetTableName<Vendor>(), new Vendor()); }
            if (!TypeTables.ContainsKey(this.GetTableName<ShoppingCart>())) { TypeTables.Add(this.GetTableName<ShoppingCart>(), new ShoppingCart()); }
            if (!TypeTables.ContainsKey(this.GetTableName<ShoppingCartItem>())) { TypeTables.Add(this.GetTableName<ShoppingCartItem>(), new ShoppingCartItem()); }
            if (!TypeTables.ContainsKey(this.GetTableName<InventoryItem>())) { TypeTables.Add(this.GetTableName<InventoryItem>(), new InventoryItem()); }

            if (!TypeTables.ContainsKey(this.GetTableName<PriceRule>())) { TypeTables.Add(this.GetTableName<PriceRule>(), new PriceRule()); }
            if (!TypeTables.ContainsKey(this.GetTableName<PriceRuleLog>())) { TypeTables.Add(this.GetTableName<PriceRuleLog>(), new PriceRuleLog()); }
            if (!TypeTables.ContainsKey(this.GetTableName<Order>())) { TypeTables.Add(this.GetTableName<Order>(), new Order()); }
            if (!TypeTables.ContainsKey(this.GetTableName<OrderItem>())) { TypeTables.Add(this.GetTableName<OrderItem>(), new OrderItem()); }

            try
                {
                    //Note: if you get error 'The entity type XXXXX is not part of the model for the current context.' you need to add it below.

                    #region General
                    
                    modelBuilder.Entity<TreeMon.Models.General.Attribute>()
                          .ToTable(GetTableName<TreeMon.Models.General.Attribute>())
                          .HasKey(o => o.Id)
                          .Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                          {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                          }));

                    modelBuilder.Entity<Category>()
                            .ToTable(GetTableName<Category>())
                            .HasKey(o => o.Id)
                            .Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                            }));

                    modelBuilder.Entity<UnitOfMeasure>().ToTable(GetTableName<UnitOfMeasure>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                    modelBuilder.Entity<StatusMessage>().ToTable(GetTableName<StatusMessage>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                    #endregion

                    #region App

                    modelBuilder.Entity<AppInfo>().ToTable(GetTableName<AppInfo>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                    modelBuilder.Entity<Setting>().ToTable(GetTableName<Setting>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                    modelBuilder.Entity<UserSession>().ToTable(GetTableName<UserSession>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                    #endregion

                    #region Medical
                    modelBuilder.Entity<AnatomyTag>().ToTable(GetTableName<AnatomyTag>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                    modelBuilder.Entity<Anatomy>().ToTable(GetTableName<Anatomy>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                    modelBuilder.Entity<SideAffect>().ToTable(GetTableName<SideAffect>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                    modelBuilder.Entity<Symptom>().ToTable(GetTableName<Symptom>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                    modelBuilder.Entity<SymptomLog>().ToTable(GetTableName<SymptomLog>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));


                    modelBuilder.Entity<DoseLog>().ToTable(GetTableName<DoseLog>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));


                    #endregion

                    #region Equipment
                    modelBuilder.Entity<Ballast>().ToTable(GetTableName<Ballast>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                    modelBuilder.Entity<Bulb>().ToTable(GetTableName<Bulb>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                    modelBuilder.Entity<Fan>().ToTable(GetTableName<Fan>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));


                    modelBuilder.Entity<Filter>().ToTable(GetTableName<Filter>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                    modelBuilder.Entity<Pump>().ToTable(GetTableName<Pump>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                    modelBuilder.Entity<Vehicle>().ToTable(GetTableName<Vehicle>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                    modelBuilder.Entity<InventoryItem>().ToTable(GetTableName<InventoryItem>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                          {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));
                    #endregion

                    #region Events
                    modelBuilder.Entity<Notification>().ToTable(GetTableName<Notification>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                    modelBuilder.Entity<Reminder>().ToTable(GetTableName<Reminder>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));


                    modelBuilder.Entity<ReminderRule>().ToTable(GetTableName<ReminderRule>()).HasKey(o => o.UUID).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                #endregion

                    #region Finance
                    modelBuilder.Entity<Currency>().ToTable(GetTableName<Currency>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                          {
                                new IndexAttribute("IX_UUID") { IsUnique = true }
                            }));

                    modelBuilder.Entity<Fee>().ToTable(GetTableName<Fee>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                         {
                                new IndexAttribute("IX_UUID") { IsUnique = true }
                            }));
                    modelBuilder.Entity<FinanceAccount>().ToTable(GetTableName<FinanceAccount>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                         {
                                new IndexAttribute("IX_UUID") { IsUnique = true }
                            }));
                    modelBuilder.Entity<FinanceAccountTransaction>().ToTable(GetTableName<FinanceAccountTransaction>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                         { new IndexAttribute("IX_UUID") { IsUnique = true } }));


                modelBuilder.Entity<PriceRule>().ToTable(GetTableName<PriceRule>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                          { new IndexAttribute("IX_UUID") { IsUnique = true } }));

                modelBuilder.Entity<PaymentGatewayLog>().ToTable(GetTableName<PaymentGatewayLog>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            { new IndexAttribute("IX_UUID") { IsUnique = true }}));
                #endregion

                    #region Geo
                modelBuilder.Entity<Location>().ToTable(GetTableName<Location>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                          {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));
                    #endregion

                    #region Logging
                    modelBuilder.Entity<AuthenticationLog>().ToTable(GetTableName<AuthenticationLog>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));
                    modelBuilder.Entity<LineItemLog>().ToTable(GetTableName<LineItemLog>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                           {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                    modelBuilder.Entity<MeasurementLog>().ToTable(GetTableName<MeasurementLog>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));


                    modelBuilder.Entity<EmailLog>().ToTable(GetTableName<EmailLog>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                         {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));


                    //Data project...
                    modelBuilder.Entity<LogEntry>().ToTable(GetTableName<LogEntry>()).HasKey(o => o.Id);

                    #endregion

                    #region Membership
                    modelBuilder.Entity<Account>().ToTable(GetTableName<Account>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));


                    modelBuilder.Entity<AccountMember>().ToTable(GetTableName<AccountMember>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                    modelBuilder.Entity<Credential>().ToTable(GetTableName<Credential>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                    modelBuilder.Entity<RolePermission>().ToTable(GetTableName<RolePermission>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                    modelBuilder.Entity<Profile>().ToTable(GetTableName<Profile>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                    modelBuilder.Entity<Role>().ToTable(GetTableName<Role>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));


                    modelBuilder.Entity<User>().ToTable(GetTableName<User>()).HasKey(usr => usr.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                    modelBuilder.Entity<UserRole>().ToTable(GetTableName<UserRole>()).HasKey(usr => usr.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                    modelBuilder.Entity<Permission>().ToTable(GetTableName<Permission>()).HasKey(usr => usr.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                    #endregion

                    #region Plant
                    modelBuilder.Entity<Plant>().ToTable(GetTableName<Plant>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));
                    if (!TypeTables.ContainsKey(this.GetTableName<Plant>())) { TypeTables.Add(this.GetTableName<Plant>(), new Plant()); }

                    modelBuilder.Entity<Strain>().ToTable(GetTableName<Strain>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));
                    if (!TypeTables.ContainsKey(this.GetTableName<Strain>())) { TypeTables.Add(this.GetTableName<Strain>(), new Strain()); }

                    #endregion

                    #region Store    

                    modelBuilder.Entity<Product>().ToTable(GetTableName<Product>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                    modelBuilder.Entity<Vendor>().ToTable(GetTableName<Vendor>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                    modelBuilder.Entity<ShoppingCart>().ToTable(GetTableName<ShoppingCart>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                    {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));


                modelBuilder.Entity<ShoppingCartItem>().ToTable(GetTableName<ShoppingCartItem>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                        {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));
              

                modelBuilder.Entity<PriceRule>().ToTable(GetTableName<PriceRule>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                       {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));
                modelBuilder.Entity<PriceRuleLog>().ToTable(GetTableName<PriceRuleLog>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                       {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));
                modelBuilder.Entity<Order>().ToTable(GetTableName<Order>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                       {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));
                modelBuilder.Entity<OrderItem>().ToTable(GetTableName<OrderItem>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                       {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));
            

                #endregion
            }
            catch (Exception ex)
                {
                    _fileLogger.InsertError(ex.Message, "TreeMonDbContext", "OnModelCreating");
                    Debug.Assert(false, ex.Message);
                }
         
        }
        #endregion

   
      


        #region  ---===  CRUD Async Functions  ===---

        public async Task<T> GetAsync<T>(int id) where T : class
        {
            T res = null;
            try
            {
                using (var connection = base.Database.Connection)
                {
                    connection.Open();
                    res = await connection.GetAsync<T>(id);
                }
            }
            catch (Exception ex)
            {
                _fileLogger.InsertError(ex.Message, "TreeMonDbContext", "GetAsync:" + typeof(T).ToString());
                Debug.Assert(false, ex.Message);
            }
            return res;
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>() where T : class
        {
            IEnumerable<T> res = null;
            try
            {
                using (var connection = base.Database.Connection)
                {
                    if (connection.State != ConnectionState.Open)
                        connection.Open();

                    res = await connection.GetListAsync<T>();
                }
            }
            catch (Exception ex)
            {
                _fileLogger.InsertError(ex.Message, "TreeMonDbContext", "GetAllAsync:" + typeof(T).ToString());
                Debug.Assert(false, ex.Message);
            }
            return res;
        }

        public async Task<IEnumerable<T>> SelectAsync<T>(string sql, object parameters) where T : class
        {
            IEnumerable<T> res = null;

            if (Validator.HasSqlCommand(sql))
            {
                this._fileLogger.InsertSecurity("illegal statement:" + sql, "TreeMonDbContext", "SelectAsync");

                Debug.Assert(false, "Illegal statement.");
                return res;
            }
           
            try
            {
                using (var connection = base.Database.Connection)
                {
                    if (connection.State != ConnectionState.Open)
                        connection.Open();

                    res = await connection.QueryAsync<T>(sql, parameters);
                }
            }
            catch (Exception ex)
            {
                _fileLogger.InsertError(ex.Message, "TreeMonDbContext", "SelectAsync:" + typeof(T).ToString());
                Debug.Assert(false, ex.Message);
            }

            return res;
        }

        public async Task<int> DeleteAsync<T>(T entity) where T : class
        {
            int res = 0;
            try {
                if (Database.Connection.State != ConnectionState.Open)
                    Database.Connection.Open();

                res = await Database.Connection.DeleteAsync<T>(entity);
                await SaveChangesAsync();
            }
            catch(Exception ex)
            {
                _fileLogger.InsertError(ex.Message, "TreeMonDbContext", "DeleteAsync:" + typeof(T).ToString());
            }

            return res;
        }

        public async Task<int> ExecuteNonQueryAsync(string sql, object parameters)
        {
            if (Validator.HasSqlCommand(sql))
            {
                this._fileLogger.InsertSecurity("illegal statement:" + sql, "TreeMonDbContext", "ExecuteNonQueryAsync");

                Debug.Assert(false, "Illegal statement.");
                return 0;
            }
            int res = 0;
            
            try
            {
                if (Database.Connection.State != ConnectionState.Open)
                    Database.Connection.Open();
                res = await Database.Connection.ExecuteAsync(sql, parameters);

            }
            catch (Exception ex)
            {
                res = -1;
                _fileLogger.InsertError(ex.Message, "TreeMonDbContext", "ExecuteNonQuery:" + sql);
            }

            return res;
        }

        public async Task<int> InsertAsync<T>(T entity) where T : class
        {
            int res = 0;
            try
            {
                base.Set<T>().Add(entity);
                res = await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _fileLogger.InsertError(ex.Message, "TreeMonDbContext", "InsertAsync:" + typeof(T).ToString());
                return res;
            }
            return res;
        }

        public async Task<int> SaveAsync()
        {
            try {
                return await base.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                _fileLogger.InsertError(ex.Message, "TreeMonDbContext", "SaveAsync");
            }
            return 0;
        }

        public async Task<int> UpdateAsync<T>(T entity) where T : class
        {
            int res = 0;
            try
            {
                Entry<T>(entity).State = EntityState.Modified;
                res = await  SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _fileLogger.InsertError(ex.Message, "TreeMonDbContext", "UpdateAsync:" + typeof(T).ToString());
                return res;
            }
         

            return res;
        }

        public async Task<object> ExecuteScalarAsync(string sql, object parameters)
        {
            if (Validator.HasSqlCommand(sql))
            {
                this._fileLogger.InsertSecurity("illegal statement:" + sql, "TreeMonDbContext", "ExecuteScalarAsync");

                Debug.Assert(false, "Illegal statement.");
                return 0;
            }
            object res = null;

            try
            {
                if (Database.Connection.State != ConnectionState.Open)
                    Database.Connection.Open();

                res =  await Database.Connection.ExecuteScalarAsync(sql, parameters);
            }
            catch (Exception ex)
            {
                _fileLogger.InsertError(ex.Message, "TreeMonDbContext", "ExecuteNonQuery:" + sql);
            }
            return res;
        }

        #endregion

        #region     ---=== CRUD Functions ===---

        public int ExecuteNonQuery(string sql, object parameters) 
        {
            if (Validator.HasSqlCommand(sql))
            {
                this._fileLogger.InsertSecurity("illegal statement:" + sql, "TreeMonDbContext", "ExecuteNonQuery");

                Debug.Assert(false, "Illegal statement.");
                return 0;
            }
            int res = 0;
         
            try {
                if (Database.Connection.State != ConnectionState.Open)
                    Database.Connection.Open();
                res = Database.Connection.Execute(sql, parameters);
            }
            catch(Exception ex)
            {
                _fileLogger.InsertError(ex.Message, "TreeMonDbContext", "ExecuteNonQuery:" + sql);
            }

            return res;
        }

        public string Message { get; set; }

        public IEnumerable<T> GetAll<T>() where T : class
        {
            IEnumerable<T> res = null;
            try {

                if (Database.Connection.State != ConnectionState.Open)
                    Database.Connection.Open();

                res = Database.Connection?.GetList<T>();
            }
            catch(Exception ex)
            {
                Message = ex.Message;
                _fileLogger.InsertError(ex.Message, "TreeMonDbContext", "GetAll:" + typeof(T).ToString());
            }
            return res;
        }

        public T Get<T>(int id) where T: class
        {
            T res = null;
            try {
                if (Database.Connection.State != ConnectionState.Open)
                    Database.Connection.Open();
                
                res = Database.Connection.Get<T>(id);
            }
            catch(Exception ex)
            {
                _fileLogger.InsertError(ex.Message, "TreeMonDbContext", "Get:" + typeof(T).ToString());
            }
 
            return res;
        }

        public IEnumerable<T> Select<T>(string sql, object parameters) where T : class
        {
            IEnumerable<T> res = null;

            try {
                if (Database.Connection.State != ConnectionState.Open)
                    Database.Connection.Open();
   
                res = Database.Connection.Query<T>(sql, parameters);
            }
            catch (Exception ex)
            {
                _fileLogger.InsertError(ex.Message, "TreeMonDbContext", "Select:" + typeof(T).ToString() + " " + sql);
                
            }
 
            return res;
        }

        public IDataReader Execute(string sql, object parameters) 
        {
            if (Validator.HasSqlCommand(sql))
            {
                this._fileLogger.InsertSecurity("illegal statement:" + sql, "TreeMonDbContext", "Execute");
                Debug.Assert(false, "Illegal statement.");
                return null;
            }
            IDataReader res = null;

            try
            {
                if (Database.Connection.State != ConnectionState.Open)
                    Database.Connection.Open();
  
                res = Database.Connection.ExecuteReader(sql, parameters);
            }
            catch (Exception ex)
            {
                _fileLogger.InsertError(ex.Message, "TreeMonDbContext", "Select:" + sql);

            }
            return res;
        }

        public new int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
           
            catch (DbUpdateConcurrencyException cex)
            {
                // Client wins update
                var entry = cex.Entries.Single();
                entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                _fileLogger.InsertError(cex.Message, "TreeMonDbContext", "SaveChanges:" );
            }
            catch (Exception ex)
            {
                _fileLogger.InsertError(ex.Message, "TreeMonDbContext", "SaveChanges:");
            }
            return 0;
        }

        public object ExecuteScalar(string sql, object parameters)
        {
            if (Validator.HasSqlCommand(sql))
            {
                Debug.Assert(false, "Illegal statement.");
                return 0;
            }
            object res = null;

            try
            {
                if (Database.Connection.State != ConnectionState.Open)
                    Database.Connection.Open();

                res = Database.Connection.ExecuteScalar(sql, parameters);
            }
            catch (Exception ex)
            {
                _fileLogger.InsertError(ex.Message, "TreeMonDbContext", "ExecuteNonQuery:" + sql);
            }
            return res;
        }

        /// <summary>
        /// If this is not updating, check the Id property, it may need to be 
        /// set.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Update<T>(T entity) where T : class
        {
            int res = 0;

            bool saveFailed = false;
            do
            {
                try
                {
                    if (Database.Connection.State != ConnectionState.Open)
                        Database.Connection.Open();

                    res =  Database.Connection.Update(entity);
                }
                catch (DbUpdateConcurrencyException cex)
                {
                    saveFailed = true;
                    res = 0;

                    // Database wins update
                    cex.Entries.Single().Reload();

                    //    Client wins update
                   var entry = cex.Entries.Single();
                   entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                    _fileLogger.InsertError(cex.Message, "TreeMonDbContext", "Update.1:" + typeof(T).ToString());
                }
                catch (Exception ex)
                {
                    Debug.Assert(false, ex.Message);
                    _fileLogger.InsertError(ex.Message, "TreeMonDbContext", "Update.2:" + typeof(T).ToString());
                    res = 0;
                }
 
            } while (saveFailed);
        
            return res;
        }

    
        public int Delete<T>(T entity) where T : class
        {
            int res = 0;
            try {
                if (Database.Connection.State != ConnectionState.Open)
                    Database.Connection.Open();
                res =   Database.Connection.Delete<T>(entity);
                SaveChanges();
            }
            catch(Exception ex)
            {
                _fileLogger.InsertError(ex.Message, "TreeMonDbContext", "Delete:" + typeof(T).ToString());
            }
 
            return res;
        }

        /// <summary>
        ///object[] paramters = new object[] { userUUID, roleUUID, domain.ToUpper() };
        /// da.Delete("WHERE UserUUID=? RoleUUID=? AND ApplicationName=?", paramters);/
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="whereStatement"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int Delete<T>(string whereStatement, object parameters) where T : class
        {
            if (Validator.HasSqlCommand(whereStatement))
            {
                this._fileLogger.InsertSecurity("illegal statement:" + whereStatement, "TreeMonDbContext", "Delete");
                Debug.Assert(false, "Illegal statement.");
                return 0;
            }

            int res = -1;
            try
            {
                if (Database.Connection.State != ConnectionState.Open)
                   Database.Connection.Open();
                string sql = "DELETE FROM " + GetTableName<T>() + " " + whereStatement;
                res = Database.Connection.Execute(sql, parameters);

            }
            catch (Exception ex)
            {
                _fileLogger.InsertError(ex.Message, "TreeMonDbContext", "Delete:" + typeof(T).ToString() + " " + "DELETE FROM " + GetTableName<T>() + " " + whereStatement);
                Debug.Assert(false, ex.Message);

            }

            return res;
        }

        public bool Insert<T>(T entity) where T : class
        {
            try {
               var test =  base.Set<T>().Add(entity);

                int res = SaveChanges();
                return res > 0 ? true : false;

            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
                _fileLogger.InsertError(ex.Message + ex.InnerException?.ToString(), "TreeMonDbContext", "Insert:" + typeof(T).ToString());
                return false;
            }
        }

        public void Add<T>(T entity) where T : class
        {
            try
            {
             
                var test = base.Set<T>().Add(entity);
            }
            catch (Exception ex)
            {
                _fileLogger.InsertError(ex.Message + ex.InnerException?.ToString(), "TreeMonDbContext", "Add:" + typeof(T).ToString());
            }
        }

        public void AddRange<T>(List<T> entities) where T : class
        {
            try
            {
                var test = base.Set<T>().AddRange(entities);
            }
            catch (Exception ex)
            {
                _fileLogger.InsertError(ex.Message + ex.InnerException?.ToString(), "TreeMonDbContext", "Add:" + typeof(T).ToString());
            }
        }

        #endregion

        #region     ---=== Installer Code ===--- 

        public ServiceResult CreateDatabase(AppInfo appSettings, string connectionString)
        {
            if (string.IsNullOrWhiteSpace(appSettings.ActiveDbConnectionKey))
                return ServiceResponse.Error("Failed to create databse, ActiveDbConnectionKey is not set.");
            ServiceResult res = null;
            switch (appSettings.ActiveDbProvider.ToUpper())
            {
                case "SQLITE":
                    //This actually works, but the schema is so out of sync I removed it for now.
                    // res = CopyDefaultSQLiteDatabase(appSettings);
                    res = ServiceResponse.Error(appSettings.ActiveDbProvider + " NOT IMPLEMENTED");

                    break;
                case "MSSQL":
                    #region sql database creation
                    try
                    {
                        this.Install = true;
                        this.Initialize(appSettings.ActiveDbConnectionKey);
                        ConnectionSettings = new ConnectionStringSettings(appSettings.ActiveDbConnectionKey, connectionString);
                        this.Database.Connection.ConnectionString = connectionString;
                        Database.SetInitializer<TreeMonDbContext>(new CreateDatabaseIfNotExists<TreeMonDbContext>());
                        Database.Initialize(true);
                       // this.Insert<LogEntry>(new LogEntry() { LogDate = DateTime.UtcNow, Level = SystemFlag.Level.Info, Source = "TreeMonDbContext.Initialize", Type = "LogEntry" });
                        SystemLogger sl = new SystemLogger(this.ConnectionKey, false);
                        sl.Insert(new LogEntry() { LogDate = DateTime.UtcNow, Level = SystemFlag.Level.Info, Source = "TreeMonDbContext.Initialize", Type = "LogEntry" });
                    }
                    catch (Exception ex)
                    {
                        _fileLogger.InsertError(ex.Message, "TreeMonDbContext", "InstallDatabase:");
                        return new ServiceResult() { Code = 500, Status = "ERROR", Message = ex.Message };
                    }
                    #endregion
                    res = ServiceResponse.OK( );
                    break;
                case "MYSSQL":
                    res= ServiceResponse.Error(appSettings.ActiveDbProvider + " NOT IMPLEMENTED" );
                    break;
                default:
                    res = ServiceResponse.Error("UNSUPORTED PROVIDER:" + appSettings.ActiveDbProvider);
                    break;
            }

            return res;
        }
      
        /// <summary>
        /// Copies a sqlite boilerplate database to the App_Data folder for initial use.
        /// </summary>
        /// <param name="appSettings"></param>
        /// <returns></returns>
        protected ServiceResult CopyDefaultSQLiteDatabase(AppInfo appSettings)
        {
            string directory = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug", "");

            string pathToDefaults = Path.Combine(directory, "App_Data\\Install\\TreeMon.sqlite");

            if (!File.Exists(pathToDefaults))
                return new ServiceResult() { Code = 500, Status = "ERROR", Message = "Default database file missing in " + pathToDefaults };

         
            try
            {    //Connection.DataSource is null so hack away at it :/
                 //BACKLOG: Warn user and let them choose to overwrite
                if (File.Exists(PathToDatabase))
                {
                    File.Move(PathToDatabase, PathToDatabase + "_BACKUP_" + string.Format("text-{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.UtcNow));
                }
                File.Copy(pathToDefaults, PathToDatabase);
            }
            catch (Exception ex)
            {
                _fileLogger.InsertError(ex.Message, "TreeMonDbContext", "CopyDefaultSQLiteDatabase:" + PathToDatabase);
                return new ServiceResult() { Code = 500, Status = "ERROR", Message = "Error creating database: " + ex.Message };
            }
            return new ServiceResult() { Code = 200, Status = "OK" };
        }

        #endregion

        #region Table Functions 
        public List<string> GetDataTypes()
        {
            if (TypeTables.Count == 0)
                this.LoadTableNames();
            List<string> res = new List<string>();
            foreach (var typeEntry in TypeTables)
            {
                dynamic table = typeEntry.Value;
                try
                {
                    res.Add(table?.UUIDType);
                }
                catch {//
                }
            }
            #region future
            //if (ConnectionSettings == null)
            //    return res;
            //string sql = "";
            //switch (_providerType)
            //{
            //    case "SQLITE":
            //        Debug.Assert(false, "NOT IMPLEMENTED");
            //        //sql = "SELECT tbl_name FROM sqlite_master WHERE type='table'";
            //        //res = _sqliteContext.Table<sqlite_master>().AsQueryable().Select(s => s.tbl_name).ToList();
            //        ////using (SQLiteConnection conn = new SQLiteConnection(PathToDatabase))
            //        ////{ 
            //        ////    res = conn.Table<sqlite_master>().AsQueryable().Select(s => s.tbl_name).ToList();
            //        ////}
            //        break;
            //    case "MYSQL":
            //        Debug.Assert(false, "NOT IMPLEMENTED");
            //        //sql = "SELECT * FROM information_schema.tables";
            //        //res =  Select<string>(sql, new object[] { }).ToList();
            //        break;
            //    case "MSSQL":
            //        // Debug.Assert(false, "NOT IMPLEMENTED");
            //        //SQL Server 2005, 2008, 2012 or 2014:
            //        // sql = "SELECT * FROM information_schema.tables";                //SQL Server 2000: SELECT* FROM sysobjects WHERE xtype = 'U'
            //        //res = Select<string>(sql, new object[] { }).ToList();
            //        break;
            //}
            #endregion

            return res;
        }

        public List<string> GetTableNames()
        {
            List<string> res = TypeTables.Select(t => t.Key).ToList();
            return res;
        }

        public string GetTableName<T>() where T : class
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

        public string GetTableName(string typeName)
        {
            string res = string.Empty;
            if (string.IsNullOrWhiteSpace(typeName))
                return res;

            if (TypeTables.Count == 0)
                this.LoadTableNames();

            foreach(dynamic typeEntry in TypeTables)
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

        public object GetTableObject(string tableName)
        {
            if (TypeTables.Count == 0)
                LoadTableNames();

            if (TypeTables.ContainsKey(tableName))
                    return TypeTables[tableName];

            return null;
        }

        public string GetTableType(string tableName)
        {
            if (TypeTables.Count == 0)
                LoadTableNames();

            if (TypeTables.ContainsKey(tableName))
                return ((dynamic) TypeTables[tableName])?.UUIDType.ToUpper();

            return string.Empty;
        }

        public void LoadTableNames()
        {
            #region General
          
            if (!TypeTables.ContainsKey(this.GetTableName<TreeMon.Models.General.Attribute>())) { TypeTables.Add(this.GetTableName<TreeMon.Models.General.Attribute>(), new TreeMon.Models.General.Attribute()); }

            if (!TypeTables.ContainsKey(this.GetTableName<Category>())) { TypeTables.Add(this.GetTableName<Category>(), new Category()); }

            if (!TypeTables.ContainsKey(this.GetTableName<UnitOfMeasure>())) { TypeTables.Add(this.GetTableName<UnitOfMeasure>(), new UnitOfMeasure()); }

            if (!TypeTables.ContainsKey(this.GetTableName<StatusMessage>())) { TypeTables.Add(this.GetTableName<StatusMessage>(), new StatusMessage()); }

            #endregion

            #region App

            if (!TypeTables.ContainsKey(this.GetTableName<AppInfo>())) { TypeTables.Add(this.GetTableName<AppInfo>(), new AppInfo()); }

            if (!TypeTables.ContainsKey(this.GetTableName<Setting>())) { TypeTables.Add(this.GetTableName<Setting>(), new Setting()); }

            if (!TypeTables.ContainsKey(this.GetTableName<UserSession>())) { TypeTables.Add(this.GetTableName<UserSession>(), new UserSession()); }

            #endregion

            #region Medical

            if (!TypeTables.ContainsKey(this.GetTableName<AnatomyTag>())) { TypeTables.Add(this.GetTableName<AnatomyTag>(), new AnatomyTag()); }

            if (!TypeTables.ContainsKey(this.GetTableName<Anatomy>())) { TypeTables.Add(this.GetTableName<Anatomy>(), new Anatomy()); }

            if (!TypeTables.ContainsKey(this.GetTableName<SideAffect>())) { TypeTables.Add(this.GetTableName<SideAffect>(), new SideAffect()); }

            if (!TypeTables.ContainsKey(this.GetTableName<Symptom>())) { TypeTables.Add(this.GetTableName<Symptom>(), new Symptom()); }

            if (!TypeTables.ContainsKey(this.GetTableName<SymptomLog>())) { TypeTables.Add(this.GetTableName<SymptomLog>(), new SymptomLog()); }

            if (!TypeTables.ContainsKey(this.GetTableName<DoseLog>())) { TypeTables.Add(this.GetTableName<DoseLog>(), new DoseLog()); }

            #endregion

            #region Equipment
            if (!TypeTables.ContainsKey(this.GetTableName<Ballast>())) { TypeTables.Add(this.GetTableName<Ballast>(), new Ballast()); }

            if (!TypeTables.ContainsKey(this.GetTableName<Bulb>())) { TypeTables.Add(this.GetTableName<Bulb>(), new Bulb()); }

            if (!TypeTables.ContainsKey(this.GetTableName<Fan>())) { TypeTables.Add(this.GetTableName<Fan>(), new Fan()); }

            if (!TypeTables.ContainsKey(this.GetTableName<Filter>())) { TypeTables.Add(this.GetTableName<Filter>(), new Filter()); }

            if (!TypeTables.ContainsKey(this.GetTableName<Pump>())) { TypeTables.Add(this.GetTableName<Pump>(), new Pump()); }

            if (!TypeTables.ContainsKey(this.GetTableName<Vehicle>())) { TypeTables.Add(this.GetTableName<Vehicle>(), new Vehicle()); }

            if (!TypeTables.ContainsKey(this.GetTableName<InventoryItem>())) { TypeTables.Add(this.GetTableName<InventoryItem>(), new InventoryItem()); }
            #endregion

            #region Events

            if (!TypeTables.ContainsKey(this.GetTableName<Notification>())) { TypeTables.Add(this.GetTableName<Notification>(), new Notification()); }

            if (!TypeTables.ContainsKey(this.GetTableName<Reminder>())) { TypeTables.Add(this.GetTableName<Reminder>(), new Reminder()); }

            if (!TypeTables.ContainsKey(this.GetTableName<ReminderRule>())) { TypeTables.Add(this.GetTableName<ReminderRule>(), new ReminderRule()); }

            #endregion

            #region Finance

            if (!TypeTables.ContainsKey(this.GetTableName<Currency>())) { TypeTables.Add(this.GetTableName<Currency>(), new Currency()); }

            if (!TypeTables.ContainsKey(this.GetTableName<Fee>())) { TypeTables.Add(this.GetTableName<Fee>(), new Fee()); }

            if (!TypeTables.ContainsKey(this.GetTableName<FinanceAccount>())) { TypeTables.Add(this.GetTableName<FinanceAccount>(), new FinanceAccount()); }

            if (!TypeTables.ContainsKey(this.GetTableName<FinanceAccountTransaction>())) { TypeTables.Add(this.GetTableName<FinanceAccountTransaction>(), new FinanceAccountTransaction()); }

            if (!TypeTables.ContainsKey(this.GetTableName<PriceRule>())) { TypeTables.Add(this.GetTableName<PriceRule>(), new PriceRule()); }

            if (!TypeTables.ContainsKey(this.GetTableName<PaymentGatewayLog>())) { TypeTables.Add(this.GetTableName<PaymentGatewayLog>(), new PaymentGatewayLog()); }

            #endregion

            #region Geo

            if (!TypeTables.ContainsKey(this.GetTableName<Location>())) { TypeTables.Add(this.GetTableName<Location>(), new Location()); }

            #endregion

            #region Logging

            if (!TypeTables.ContainsKey(this.GetTableName<AuthenticationLog>())) { TypeTables.Add(this.GetTableName<AuthenticationLog>(), new AuthenticationLog()); }

            if (!TypeTables.ContainsKey(this.GetTableName<LineItemLog>())) { TypeTables.Add(this.GetTableName<LineItemLog>(), new LineItemLog()); }

            if (!TypeTables.ContainsKey(this.GetTableName<MeasurementLog>())) { TypeTables.Add(this.GetTableName<MeasurementLog>(), new MeasurementLog()); }

            if (!TypeTables.ContainsKey(this.GetTableName<LogEntry>())) { TypeTables.Add(this.GetTableName<LogEntry>(), new LogEntry()); }

            #endregion

            #region Membership

            if (!TypeTables.ContainsKey(this.GetTableName<Account>())) { TypeTables.Add(this.GetTableName<Account>(), new Account()); }

            if (!TypeTables.ContainsKey(this.GetTableName<AccountMember>())) { TypeTables.Add(this.GetTableName<AccountMember>(), new AccountMember()); }

            if (!TypeTables.ContainsKey(this.GetTableName<Credential>())) { TypeTables.Add(this.GetTableName<Credential>(), new Credential()); }

            if (!TypeTables.ContainsKey(this.GetTableName<RolePermission>())) { TypeTables.Add(this.GetTableName<RolePermission>(), new RolePermission()); }
      
            if (!TypeTables.ContainsKey(this.GetTableName<Profile>())) { TypeTables.Add(this.GetTableName<Profile>(), new Profile()); }
            
            if (!TypeTables.ContainsKey(this.GetTableName<Role>())) { TypeTables.Add(this.GetTableName<Role>(), new Role()); }
            
            if (!TypeTables.ContainsKey(this.GetTableName<User>())) { TypeTables.Add(this.GetTableName<User>(), new User()); }

            if (!TypeTables.ContainsKey(this.GetTableName<UserRole>())) { TypeTables.Add(this.GetTableName<UserRole>(), new UserRole()); }
            
            if (!TypeTables.ContainsKey(this.GetTableName<Permission>())) { TypeTables.Add(this.GetTableName<Permission>(), new Permission()); }

            #endregion

            #region Plant
            
            if (!TypeTables.ContainsKey(this.GetTableName<Plant>())) { TypeTables.Add(this.GetTableName<Plant>(), new Plant()); }
            
            if (!TypeTables.ContainsKey(this.GetTableName<Strain>())) { TypeTables.Add(this.GetTableName<Strain>(), new Strain()); }

            #endregion

            #region Store
            
            if (!TypeTables.ContainsKey(this.GetTableName<Product>())) { TypeTables.Add(this.GetTableName<Product>(), new Product()); }
            
            if (!TypeTables.ContainsKey(this.GetTableName<Vendor>())) { TypeTables.Add(this.GetTableName<Vendor>(), new Vendor()); }

            if (!TypeTables.ContainsKey(this.GetTableName<ShoppingCart>())) { TypeTables.Add(this.GetTableName<ShoppingCart>(), new ShoppingCart()); }

            if (!TypeTables.ContainsKey(this.GetTableName<ShoppingCartItem>())) { TypeTables.Add(this.GetTableName<ShoppingCartItem>(), new ShoppingCartItem()); }

            if (!TypeTables.ContainsKey(this.GetTableName<PriceRule>())) { TypeTables.Add(this.GetTableName<PriceRule>(), new PriceRule()); }

            if (!TypeTables.ContainsKey(this.GetTableName<PriceRuleLog>())) { TypeTables.Add(this.GetTableName<PriceRuleLog>(), new PriceRuleLog()); }

            if (!TypeTables.ContainsKey(this.GetTableName<Order>())) { TypeTables.Add(this.GetTableName<Order>(), new Order()); }

            if (!TypeTables.ContainsKey(this.GetTableName<OrderItem>())) { TypeTables.Add(this.GetTableName<OrderItem>(), new OrderItem()); }
       
            #endregion
        }

        #endregion

    }
}
