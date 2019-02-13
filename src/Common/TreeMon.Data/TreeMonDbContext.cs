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
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TreeMon.Data.Helpers;
using TreeMon.Data.Logging;
using TreeMon.Data.Logging.Models;
using TreeMon.Models;
using TreeMon.Models.App;
using TreeMon.Models.Datasets;
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
using TreeMon.Utilites.Security;


namespace TreeMon.Data
{
    public partial class TreeMonDbContext : DbContext, IDbContext
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
             Database.SetInitializer<TreeMonDbContext>(null); // dev try context has changed since the database was created


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
                    }
                    else if (ConnectionSettings.ProviderName.ToUpper().Contains("SQLCLIENT"))
                    {
                        _providerType = "MSSQL";
                    }
                }
            }
            catch (Exception ex)
            {
                _fileLogger.InsertError(ex.DeserializeException(true), "TreeMonDbContext", "Initialize");
            }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<Location>())) { TypeTables.Add(DatabaseEx.GetTableName<Location>(), new Location()); }

            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<Tag>())) { TypeTables.Add(DatabaseEx.GetTableName<Tag>(), new Tag()); }
            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<TreeMon.Models.General.Attribute>())) { TypeTables.Add(DatabaseEx.GetTableName<TreeMon.Models.General.Attribute>(), new TreeMon.Models.General.Attribute()); }
            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<Category>())) { TypeTables.Add(DatabaseEx.GetTableName<Category>(), new Category()); }
            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<UnitOfMeasure>())) { TypeTables.Add(DatabaseEx.GetTableName<UnitOfMeasure>(), new UnitOfMeasure()); }
            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<StatusMessage>())) { TypeTables.Add(DatabaseEx.GetTableName<StatusMessage>(), new StatusMessage()); }

            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<AppInfo>())) { TypeTables.Add(DatabaseEx.GetTableName<AppInfo>(), new AppInfo()); }
            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<Setting>())) { TypeTables.Add(DatabaseEx.GetTableName<Setting>(), new Setting()); }
            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<UserSession>())) { TypeTables.Add(DatabaseEx.GetTableName<UserSession>(), new UserSession()); }
            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<AnatomyTag>())) { TypeTables.Add(DatabaseEx.GetTableName<AnatomyTag>(), new AnatomyTag()); }
            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<Anatomy>())) { TypeTables.Add(DatabaseEx.GetTableName<Anatomy>(), new Anatomy()); }

            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<SideAffect>())) { TypeTables.Add(DatabaseEx.GetTableName<SideAffect>(), new SideAffect()); }
            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<Symptom>())) { TypeTables.Add(DatabaseEx.GetTableName<Symptom>(), new Symptom()); }
            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<SymptomLog>())) { TypeTables.Add(DatabaseEx.GetTableName<SymptomLog>(), new SymptomLog()); }
            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<DoseLog>())) { TypeTables.Add(DatabaseEx.GetTableName<DoseLog>(), new DoseLog()); }


            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<Ballast>())) { TypeTables.Add(DatabaseEx.GetTableName<Ballast>(), new Ballast()); }
            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<Bulb>())) { TypeTables.Add(DatabaseEx.GetTableName<Bulb>(), new Bulb()); }
            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<Fan>())) { TypeTables.Add(DatabaseEx.GetTableName<Fan>(), new Fan()); }
            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<Filter>())) { TypeTables.Add(DatabaseEx.GetTableName<Filter>(), new Filter()); }
            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<Pump>())) { TypeTables.Add(DatabaseEx.GetTableName<Pump>(), new Pump()); }
            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<Vehicle>())) { TypeTables.Add(DatabaseEx.GetTableName<Vehicle>(), new Vehicle()); }
           

            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<Currency>())) { TypeTables.Add(DatabaseEx.GetTableName<Currency>(), new Currency()); }
            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<Fee>())) { TypeTables.Add(DatabaseEx.GetTableName<Fee>(), new Fee()); }
            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<FinanceAccount>())) { TypeTables.Add(DatabaseEx.GetTableName<FinanceAccount>(), new FinanceAccount()); }
            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<FinanceAccountTransaction>())) { TypeTables.Add(DatabaseEx.GetTableName<FinanceAccountTransaction>(), new FinanceAccountTransaction()); }
            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<PriceRule>())) { TypeTables.Add(DatabaseEx.GetTableName<PriceRule>(), new PriceRule()); }
            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<PaymentGatewayLog>())) { TypeTables.Add(DatabaseEx.GetTableName<PaymentGatewayLog>(), new PaymentGatewayLog()); }

            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<Event>())) { TypeTables.Add(DatabaseEx.GetTableName<Event>(), new Event()); }
            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<EventMember>())) { TypeTables.Add(DatabaseEx.GetTableName<EventMember>(), new EventMember()); }
            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<EventGroup>())) { TypeTables.Add(DatabaseEx.GetTableName<EventGroup>(), new EventGroup()); }

            
            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<EventItem>())) { TypeTables.Add(DatabaseEx.GetTableName<EventItem>(), new EventItem()); }
            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<EventLocation>())) { TypeTables.Add(DatabaseEx.GetTableName<EventLocation>(), new EventLocation()); }

            
            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<Notification>())) { TypeTables.Add(DatabaseEx.GetTableName<Notification>(), new Notification()); }
            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<Reminder>())) { TypeTables.Add(DatabaseEx.GetTableName<Reminder>(), new Reminder()); }
            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<ReminderRule>())) { TypeTables.Add(DatabaseEx.GetTableName<ReminderRule>(), new ReminderRule()); }

            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<AccessLog>())) { TypeTables.Add(DatabaseEx.GetTableName<AccessLog>(), new AccessLog()); }
            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<LineItemLog>())) { TypeTables.Add(DatabaseEx.GetTableName<LineItemLog>(), new LineItemLog()); }
            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<MeasurementLog>())) { TypeTables.Add(DatabaseEx.GetTableName<MeasurementLog>(), new MeasurementLog()); }
            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<EmailLog>())) { TypeTables.Add(DatabaseEx.GetTableName<EmailLog>(), new EmailLog()); }
            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<LogEntry>())) { TypeTables.Add(DatabaseEx.GetTableName<LogEntry>(), new LogEntry()); }

            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<Account>())) { TypeTables.Add(DatabaseEx.GetTableName<Account>(), new Account()); }
            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<AccountMember>())) { TypeTables.Add(DatabaseEx.GetTableName<AccountMember>(), new AccountMember()); }
            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<ApiKey>())) { TypeTables.Add(DatabaseEx.GetTableName<ApiKey>(), new ApiKey()); }
            
            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<Credential>())) { TypeTables.Add(DatabaseEx.GetTableName<Credential>(), new Credential()); }
            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<RolePermission>())) { TypeTables.Add(DatabaseEx.GetTableName<RolePermission>(), new RolePermission()); }
            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<PersonalProfile>())) { TypeTables.Add(DatabaseEx.GetTableName<PersonalProfile>(), new PersonalProfile()); }
            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<Profile>())) { TypeTables.Add(DatabaseEx.GetTableName<Profile>(), new Profile()); }
                if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<Role>())) { TypeTables.Add(DatabaseEx.GetTableName<Role>(), new Role()); }
            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<User>())) { TypeTables.Add(DatabaseEx.GetTableName<User>(), new User()); }
            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<UserRole>())) { TypeTables.Add(DatabaseEx.GetTableName<UserRole>(), new UserRole()); }
            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<Permission>())) { TypeTables.Add(DatabaseEx.GetTableName<Permission>(), new Permission()); }

            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<Product>())) { TypeTables.Add(DatabaseEx.GetTableName<Product>(), new Product()); }
            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<Vendor>())) { TypeTables.Add(DatabaseEx.GetTableName<Vendor>(), new Vendor()); }
            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<ShoppingCart>())) { TypeTables.Add(DatabaseEx.GetTableName<ShoppingCart>(), new ShoppingCart()); }
            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<ShoppingCartItem>())) { TypeTables.Add(DatabaseEx.GetTableName<ShoppingCartItem>(), new ShoppingCartItem()); }
            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<InventoryItem>())) { TypeTables.Add(DatabaseEx.GetTableName<InventoryItem>(), new InventoryItem()); }

            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<PriceRule>())) { TypeTables.Add(DatabaseEx.GetTableName<PriceRule>(), new PriceRule()); }
            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<PriceRuleLog>())) { TypeTables.Add(DatabaseEx.GetTableName<PriceRuleLog>(), new PriceRuleLog()); }
            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<Order>())) { TypeTables.Add(DatabaseEx.GetTableName<Order>(), new Order()); }
            if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<OrderItem>())) { TypeTables.Add(DatabaseEx.GetTableName<OrderItem>(), new OrderItem()); }

            try
                {
                //Note: if you get error 'The entity type XXXXX is not part of the model for the current context.' you need to add it below.

                #region General

                modelBuilder.Entity<Tag>()
                     .ToTable(DatabaseEx.GetTableName<Tag>())
                     .HasKey(o => o.UUID)
                     .Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                     {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                     }));

                modelBuilder.Entity<TreeMon.Models.General.Attribute>()
                          .ToTable(DatabaseEx.GetTableName<TreeMon.Models.General.Attribute>())
                          .HasKey(o => o.Id)
                          .Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                          {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                          }));

                    modelBuilder.Entity<Category>()
                            .ToTable(DatabaseEx.GetTableName<Category>())
                            .HasKey(o => o.Id)
                            .Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                            }));

                    modelBuilder.Entity<UnitOfMeasure>().ToTable(DatabaseEx.GetTableName<UnitOfMeasure>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                    modelBuilder.Entity<StatusMessage>().ToTable(DatabaseEx.GetTableName<StatusMessage>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                    #endregion

                    #region App

                    modelBuilder.Entity<AppInfo>().ToTable(DatabaseEx.GetTableName<AppInfo>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                    modelBuilder.Entity<Setting>().ToTable(DatabaseEx.GetTableName<Setting>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                    modelBuilder.Entity<UserSession>().ToTable(DatabaseEx.GetTableName<UserSession>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                    #endregion

                    #region Medical
                    modelBuilder.Entity<AnatomyTag>().ToTable(DatabaseEx.GetTableName<AnatomyTag>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                    modelBuilder.Entity<Anatomy>().ToTable(DatabaseEx.GetTableName<Anatomy>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                    modelBuilder.Entity<SideAffect>().ToTable(DatabaseEx.GetTableName<SideAffect>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                    modelBuilder.Entity<Symptom>().ToTable(DatabaseEx.GetTableName<Symptom>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                    modelBuilder.Entity<SymptomLog>().ToTable(DatabaseEx.GetTableName<SymptomLog>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));


                    modelBuilder.Entity<DoseLog>().ToTable(DatabaseEx.GetTableName<DoseLog>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));               

                #endregion

                #region Equipment
                modelBuilder.Entity<Ballast>().ToTable(DatabaseEx.GetTableName<Ballast>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                    modelBuilder.Entity<Bulb>().ToTable(DatabaseEx.GetTableName<Bulb>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                    modelBuilder.Entity<Fan>().ToTable(DatabaseEx.GetTableName<Fan>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));


                    modelBuilder.Entity<Filter>().ToTable(DatabaseEx.GetTableName<Filter>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                    modelBuilder.Entity<Pump>().ToTable(DatabaseEx.GetTableName<Pump>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                    modelBuilder.Entity<Vehicle>().ToTable(DatabaseEx.GetTableName<Vehicle>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                    modelBuilder.Entity<InventoryItem>().ToTable(DatabaseEx.GetTableName<InventoryItem>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                          {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));
                #endregion

                #region Events
               modelBuilder.Entity<Event>().ToTable(DatabaseEx.GetTableName<Event>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                modelBuilder.Entity<EventMember>().ToTable(DatabaseEx.GetTableName<EventMember>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));
                modelBuilder.Entity<EventGroup>().ToTable(DatabaseEx.GetTableName<EventGroup>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                          {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                
                modelBuilder.Entity<EventItem>().ToTable(DatabaseEx.GetTableName<EventItem>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                modelBuilder.Entity<EventLocation>().ToTable(DatabaseEx.GetTableName<EventLocation>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                
                modelBuilder.Entity<Notification>().ToTable(DatabaseEx.GetTableName<Notification>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                    modelBuilder.Entity<Reminder>().ToTable(DatabaseEx.GetTableName<Reminder>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));


                    modelBuilder.Entity<ReminderRule>().ToTable(DatabaseEx.GetTableName<ReminderRule>()).HasKey(o => o.UUID).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                #endregion

                    #region Finance
                    modelBuilder.Entity<Currency>().ToTable(DatabaseEx.GetTableName<Currency>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                          {
                                new IndexAttribute("IX_UUID") { IsUnique = true }
                            }));

                    modelBuilder.Entity<Fee>().ToTable(DatabaseEx.GetTableName<Fee>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                         {
                                new IndexAttribute("IX_UUID") { IsUnique = true }
                            }));
                    modelBuilder.Entity<FinanceAccount>().ToTable(DatabaseEx.GetTableName<FinanceAccount>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                         {
                                new IndexAttribute("IX_UUID") { IsUnique = true }
                            }));
                    modelBuilder.Entity<FinanceAccountTransaction>().ToTable(DatabaseEx.GetTableName<FinanceAccountTransaction>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                         { new IndexAttribute("IX_UUID") { IsUnique = true } }));


                modelBuilder.Entity<PriceRule>().ToTable(DatabaseEx.GetTableName<PriceRule>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                          { new IndexAttribute("IX_UUID") { IsUnique = true } }));

                modelBuilder.Entity<PaymentGatewayLog>().ToTable(DatabaseEx.GetTableName<PaymentGatewayLog>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            { new IndexAttribute("IX_UUID") { IsUnique = true }}));
                #endregion

                    #region Geo
                modelBuilder.Entity<Location>().ToTable(DatabaseEx.GetTableName<Location>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                          {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));
                    #endregion

                    #region Logging
                    modelBuilder.Entity<AccessLog>().ToTable(DatabaseEx.GetTableName<AccessLog>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));
                    modelBuilder.Entity<LineItemLog>().ToTable(DatabaseEx.GetTableName<LineItemLog>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                           {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                    modelBuilder.Entity<MeasurementLog>().ToTable(DatabaseEx.GetTableName<MeasurementLog>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));


                    modelBuilder.Entity<EmailLog>().ToTable(DatabaseEx.GetTableName<EmailLog>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                         {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));


                    //Data project...
                    modelBuilder.Entity<LogEntry>().ToTable(DatabaseEx.GetTableName<LogEntry>()).HasKey(o => o.Id);

                    #endregion

                    #region Membership
                    modelBuilder.Entity<Account>().ToTable(DatabaseEx.GetTableName<Account>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));


                    modelBuilder.Entity<AccountMember>().ToTable(DatabaseEx.GetTableName<AccountMember>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));


                modelBuilder.Entity<ApiKey>().ToTable(DatabaseEx.GetTableName<ApiKey>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                       {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                modelBuilder.Entity<Credential>().ToTable(DatabaseEx.GetTableName<Credential>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                    modelBuilder.Entity<RolePermission>().ToTable(DatabaseEx.GetTableName<RolePermission>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                    modelBuilder.Entity<PersonalProfile>().ToTable(DatabaseEx.GetTableName<PersonalProfile>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                    modelBuilder.Entity<Role>().ToTable(DatabaseEx.GetTableName<Role>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));


                    modelBuilder.Entity<User>().ToTable(DatabaseEx.GetTableName<User>()).HasKey(usr => usr.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                    modelBuilder.Entity<UserRole>().ToTable(DatabaseEx.GetTableName<UserRole>()).HasKey(usr => usr.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                    modelBuilder.Entity<Permission>().ToTable(DatabaseEx.GetTableName<Permission>()).HasKey(usr => usr.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                modelBuilder.Entity<Profile>().ToTable(DatabaseEx.GetTableName<Profile>()).HasKey(usr => usr.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                       {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));
                #endregion

                #region Plant
                modelBuilder.Entity<Plant>().ToTable(DatabaseEx.GetTableName<Plant>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));
                    if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<Plant>())) { TypeTables.Add(DatabaseEx.GetTableName<Plant>(), new Plant()); }

                    modelBuilder.Entity<Strain>().ToTable(DatabaseEx.GetTableName<Strain>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));
                    if (!TypeTables.ContainsKey(DatabaseEx.GetTableName<Strain>())) { TypeTables.Add(DatabaseEx.GetTableName<Strain>(), new Strain()); }

                    #endregion

                    #region Store    

                    modelBuilder.Entity<Product>().ToTable(DatabaseEx.GetTableName<Product>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                    modelBuilder.Entity<Vendor>().ToTable(DatabaseEx.GetTableName<Vendor>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                            {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));

                    modelBuilder.Entity<ShoppingCart>().ToTable(DatabaseEx.GetTableName<ShoppingCart>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                    {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));


                modelBuilder.Entity<ShoppingCartItem>().ToTable(DatabaseEx.GetTableName<ShoppingCartItem>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                        {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));
              

                modelBuilder.Entity<PriceRule>().ToTable(DatabaseEx.GetTableName<PriceRule>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                       {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));
                modelBuilder.Entity<PriceRuleLog>().ToTable(DatabaseEx.GetTableName<PriceRuleLog>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                       {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));
                modelBuilder.Entity<Order>().ToTable(DatabaseEx.GetTableName<Order>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                       {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));
                modelBuilder.Entity<OrderItem>().ToTable(DatabaseEx.GetTableName<OrderItem>()).HasKey(o => o.Id).Property(p => p.UUID).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
                       {
                            new IndexAttribute("IX_UUID") { IsUnique = true }
                        }));
            

                #endregion
            }
            catch (Exception ex)
                {
                    _fileLogger.InsertError(ex.DeserializeException(true), "TreeMonDbContext", "OnModelCreating");
                    Debug.Assert(false, ex.DeserializeException(true));
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
                _fileLogger.InsertError(ex.DeserializeException(true), "TreeMonDbContext", "GetAsync:" + typeof(T).ToString());
                Debug.Assert(false, ex.DeserializeException(true));
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
                _fileLogger.InsertError(ex.DeserializeException(true), "TreeMonDbContext", "GetAllAsync:" + typeof(T).ToString());
                Debug.Assert(false, ex.DeserializeException(true));
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
                _fileLogger.InsertError(ex.DeserializeException(true), "TreeMonDbContext", "SelectAsync:" + typeof(T).ToString());
                Debug.Assert(false, ex.DeserializeException(true));
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
                _fileLogger.InsertError(ex.DeserializeException(true), "TreeMonDbContext", "DeleteAsync:" + typeof(T).ToString());
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
                _fileLogger.InsertError(ex.DeserializeException(true), "TreeMonDbContext", "ExecuteNonQuery:" + sql);
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
                _fileLogger.InsertError(ex.DeserializeException(true), "TreeMonDbContext", "InsertAsync:" + typeof(T).ToString());
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
                _fileLogger.InsertError(ex.DeserializeException(true), "TreeMonDbContext", "SaveAsync");
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
                _fileLogger.InsertError(ex.DeserializeException(true), "TreeMonDbContext", "UpdateAsync:" + typeof(T).ToString());
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
                _fileLogger.InsertError(ex.DeserializeException(true), "TreeMonDbContext", "ExecuteNonQuery:" + sql);
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
                _fileLogger.InsertError(ex.DeserializeException(true), "TreeMonDbContext", "ExecuteNonQuery:" + sql);
            }

            return res;
        }

        public string Message { get; set; }

        public IEnumerable<dynamic> GetAllOf(string type){

            if (string.IsNullOrWhiteSpace(type))
                return new List<object>();

            IEnumerable<object> res = null;
            switch (type.ToUpper()) {
                #region General
                case "TAG": return GetAll<Tag>();
                case "ATTRIBUTE": return GetAll<TreeMon.Models.General.Attribute>();
                case "CATEGORY": return GetAll<Category>();
                case "UNITOFMEASURE": return GetAll<UnitOfMeasure>();
                case "STATUSMESSAGE": return GetAll<StatusMessage>();
                #endregion

                #region App
                case "APPINFO": return GetAll<AppInfo>();
                case "SETTING": return GetAll<Setting>();
                case "USERSESSION": return GetAll<UserSession>();
                #endregion

                #region Medical
                case "ANATOMYTAG": return GetAll<AnatomyTag>();
                case "ANATOMY": return GetAll<Anatomy>();
                case "SIDEAFFECT": return GetAll<SideAffect>();
                case "SYMPTOM": return GetAll<Symptom>();
                case "SYMPTOMLOG": return GetAll<SymptomLog>();
                case "DOSELOG": return GetAll<DoseLog>();
                #endregion


                #region Equipment
                case "BALLAST": return GetAll<Ballast>();
                case "BULB": return GetAll<Bulb>();
                case "FAN": return GetAll<Fan>();
                case "FILTER": return GetAll<Filter>();
                case "PUMP": return GetAll<Pump>();
                case "VEHICLE": return GetAll<Vehicle>();
                case "INVENTORYITEM": return GetAll<InventoryItem>();
                #endregion

                #region Events
                case "NOTIFICATION": return GetAll<Notification>();
                case "REMINDER": return GetAll<Reminder>();
                case "REMINDERRULE": return GetAll<ReminderRule>();
                case "EVENT": return GetAll<Event>();
                case "EVENTMEMBER": return GetAll<EventMember>();
                case "EVENTITEM": return GetAll<EventItem>();
                case "EVENTGROUP": return GetAll<EventGroup>();
                case "EVENTLOCATION": return GetAll<EventLocation>();
                    
                #endregion

                #region Finance
                case "CURRENCY": return GetAll<Currency>();
                case "FEE": return GetAll<Fee>();
                case "FINANCEACCOUNT": return GetAll<FinanceAccount>();
                case "FINANCEACCOUNTTRANSACTION": return GetAll<FinanceAccountTransaction>();
                case "PAYMENTGATEWAYLOG": return GetAll<PaymentGatewayLog>();
                #endregion

                #region Geo
                case "LOCATION": return GetAll<Location>();
                #endregion

                #region Logging
                case "ACCESSLOG": return GetAll<AccessLog>();
                case "LINEITEMLOG": return GetAll<LineItemLog>();
                case "MEASUREMENTLOG": return GetAll<MeasurementLog>();
                case "EMAILLOG": return GetAll<EmailLog>();
                case "LOGENTRY": return GetAll<LogEntry>();
                #endregion

                #region Membership
                case "ACCOUNT": return GetAll<Account>();
                case "ACCOUNTMEMBER": return GetAll<AccountMember>();
                case "APIKEY": return GetAll<ApiKey>();
                case "CREDENTIAL": return GetAll<Credential>();
                case "ROLEPERMISSION": return GetAll<RolePermission>();
                case "PERSONALPROFILE": return GetAll<PersonalProfile>();
                case "PROFILE": return GetAll<Profile>();
                case "ROLE": return GetAll<Role>();
                case "USER": return GetAll<User>();
                case "USERROLE": return GetAll<UserRole>();
                case "PERMISSION": return GetAll<Permission>();
                #endregion

                #region Plant
                case "PLANT": return GetAll<Plant>();
                case "STRAIN": return GetAll<Strain>();
                #endregion

                #region Store    
                case "PRODUCT": return GetAll<Product>();
                case "VENDOR": return GetAll<Vendor>();
                case "SHOPPINGCART": return GetAll<ShoppingCart>();
                case "SHOPPINGCARTITEM": return GetAll<ShoppingCartItem>();
                case "PRICERULE": return GetAll<PriceRule>();
                case "PRICERULELOG": return GetAll<PriceRuleLog>();
                case "ORDER": return GetAll<Order>();
                case "ORDERITEM": return GetAll<OrderItem>();
                #endregion

            }

            return res;
        }

        public IEnumerable<T> GetAll<T>() where T : class
        {
            try {

                if (Database.Connection.State != ConnectionState.Open)
                    Database.Connection.Open();

                return Database.Connection?.GetList<T>();
            }
            catch(Exception ex)
            {
                Message = ex.DeserializeException(true);
                _fileLogger.InsertError(ex.DeserializeException(true), "TreeMonDbContext", "GetAll:" + typeof(T).ToString());
            }
            return new List<T>();
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
                _fileLogger.InsertError(ex.DeserializeException(true), "TreeMonDbContext", "Get:" + typeof(T).ToString());
            }
 
            return res;
        }

        public IEnumerable<T> Select<T>(string sql, object parameters) where T : class
        {
            IEnumerable<T> res = new List<T>();

            try {
                if (Database.Connection.State != ConnectionState.Open)
                    Database.Connection.Open();
   
                res = Database.Connection.Query<T>(sql, parameters);
            }
            catch (Exception ex)
            {
                _fileLogger.InsertError(ex.DeserializeException(true), "TreeMonDbContext", "Select:" + typeof(T).ToString() + " " + sql);
                
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
                _fileLogger.InsertError(ex.DeserializeException(true), "TreeMonDbContext", "Select:" + sql);

            }
            return res;
        }

        public new int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    this.Message += "Entity of type \"" + eve.Entry.Entity.GetType().Name +
                        "\" in state \"" + eve.Entry.State + "\" has the following validation errors:";
                    foreach (var ve in eve.ValidationErrors)
                    {
                        this.Message += "- Property: \"" + ve.PropertyName + "\", Error: \"" + ve.ErrorMessage + "\"";
                    }
                }
            }
            catch (DbUpdateConcurrencyException cex)
            {
                // Client wins update
                var entry = cex.Entries.Single();
                entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                _fileLogger.InsertError(cex.DeserializeException(true) + Environment.NewLine + this.Message, "TreeMonDbContext", "SaveChanges:");
            }
            
            catch (Exception ex)
            {
                _fileLogger.InsertError(ex.DeserializeException(true), "TreeMonDbContext", "SaveChanges:");
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
                _fileLogger.InsertError(ex.DeserializeException(true), "TreeMonDbContext", "ExecuteNonQuery:" + sql);
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
                    _fileLogger.InsertError(cex.DeserializeException(true), "TreeMonDbContext", "Update.1:" + typeof(T).ToString());
                }
                catch (Exception ex)
                {
                    Debug.Assert(false, ex.DeserializeException(true));
                    _fileLogger.InsertError(ex.DeserializeException(true), "TreeMonDbContext", "Update.2:" + typeof(T).ToString());
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
                _fileLogger.InsertError(ex.DeserializeException(true), "TreeMonDbContext", "Delete:" + typeof(T).ToString());
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
                string sql = "DELETE FROM " + DatabaseEx.GetTableName<T>() + " " + whereStatement;
                res = Database.Connection.Execute(sql, parameters);

            }
            catch (Exception ex)
            {
                _fileLogger.InsertError(ex.DeserializeException(true), "TreeMonDbContext", "Delete:" + typeof(T).ToString() + " " + "DELETE FROM " + DatabaseEx.GetTableName<T>() + " " + whereStatement);
                Debug.Assert(false, ex.DeserializeException(true));

            }

            return res;
        }

        public bool Insert<T>(T entity) where T : class
        {
            this.Message = "";
            try {
                base.Set<T>().Add(entity);

                int res = SaveChanges();
                return res > 0 ? true : false;

            }
            catch (Exception ex)
            {

                Debug.Assert(false, ex.DeserializeException(true));
                this.Message += ex.DeserializeException(true);
                _fileLogger.InsertError(this.Message  , "TreeMonDbContext", "Insert:" + typeof(T).ToString());
                return false;
            }
        }

        public void Add<T>(T entity) where T : class
        {
            try
            {
               base.Set<T>().Add(entity);
            }
            catch (Exception ex)
            {
                
                _fileLogger.InsertError(ex.DeserializeException(true), "TreeMonDbContext", "Add:" + typeof(T).ToString());
            }
        }

        public void AddRange<T>(List<T> entities) where T : class
        {
            try
            {
                base.Set<T>().AddRange(entities);
            }
            catch (Exception ex)
            {
                _fileLogger.InsertError(ex.DeserializeException(true), "TreeMonDbContext", "Add:" + typeof(T).ToString());
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
                    //// res = CopyDefaultSQLiteDatabase(appSettings);
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
                       //// this.Insert<LogEntry>(new LogEntry() { LogDate = DateTime.UtcNow, Level = SystemFlag.Level.Info, Source = "TreeMonDbContext.Initialize", Type = "LogEntry" });
                        SystemLogger sl = new SystemLogger(this.ConnectionKey, false);
                        sl.Insert(new LogEntry() { LogDate = DateTime.UtcNow, Level = SystemFlag.Level.Info, Source = "TreeMonDbContext.Initialize", Type = "LogEntry" });
                    }
                    catch (Exception ex)
                    {
                        _fileLogger.InsertError(ex.DeserializeException(true), "TreeMonDbContext", "InstallDatabase:");
                        return new ServiceResult() { Code = 500, Status = "ERROR", Message = ex.DeserializeException(true) };
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
                _fileLogger.InsertError(ex.DeserializeException(true), "TreeMonDbContext", "CopyDefaultSQLiteDatabase:" + PathToDatabase);
                return new ServiceResult() { Code = 500, Status = "ERROR", Message = "Error creating database: " + ex.DeserializeException(true) };
            }
            return new ServiceResult() { Code = 200, Status = "OK" };
        }

        #endregion
    }
}
