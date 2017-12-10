using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTrackTHC.V4
{
//    This contains all active vendor information (sans phone numbers). This function 
//will only return active entries.
    public class Vendor
    {
        //Quickbooks Online Vendor C# source code
        //PM> Install-Package IppDotNetSdkForQuickBooksApiV3 
        #region qb vendor fields
        public string    ContactName{ get; set; } // Contact Name -- Name of the contact within the vendor. Used by QBD only
        public string    AltContactName{ get; set; } // Alt Contact Name -- Name of the Alternate contact within the vendor. Used by QBD only
        public string Notes { get; set; }
        public string BillAddr{ get; set; } // Bill Addr -- Default billing address.
        public string ShipAddr{ get; set; } // Ship Addr -- Default shipping address.
        public string OtherAddr{ get; set; } // Other Addr -- An address other than default billing or shipping.
        public string TaxCountry{ get; set; } // Tax Country -- Country of Vendor.
        public string TaxIdentifier{ get; set; } // Tax Identifier -- Specifies the Tax ID of the Person or Organization
        public string BusinessNumber{ get; set; } // Business Number -- Business Number of the Vendor. Applicable for CA/UK versions of QuickBooks.
        public string ParentRef{ get; set; } // Parent Ref -- Vendor.ParentRef
        public string VendorTypeRef{ get; set; } // Vendor Type Ref -- Reference to the VendorType.
        public string TermRef{ get; set; } // Term Ref -- Vendor.TermRef
        public string PrefillAccountRef{ get; set; } // Prefill Account Ref -- Reference to the PrefillAccount.
        public string Balance{ get; set; } // Balance -- Specifies the open balance amount or the amount unpaid by the vendor. For the create operation, this represents the opening balance for the vendor. When returned in response to the query request it represents the current open balance (unpaid amount) for that vendor. Filterable{ get; set; } // QBW Sortable{ get; set; } // QBW
        public string BalanceSpecified{ get; set; } // Balance Specified -- Vendor.BalanceSpecified
        public string OpenBalanceDate{ get; set; } // Open Balance Date -- Specifies the date of the Open Balance. Non QB-writable.
        public string OpenBalanceDateSpecified{ get; set; } // Open Balance Date Specified -- Vendor.OpenBalanceDateSpecified
        public string CreditLimit{ get; set; } // Credit Limit -- Specifies the maximum amount of an unpaid vendor balance.
        public string CreditLimitSpecified{ get; set; } // Credit Limit Specified -- Vendor.CreditLimitSpecified
        public string AcctNum{ get; set; } // Acct Num -- Name or number of the account associated with this vendor. Length Restriction{ get; set; }
        public string Vendor1099{ get; set; } // Vendor 1 0 9 9 -- The Vendor is an independent contractor, someone who is given a 1099-MISC form at the end of the year. The "1099 Vendor" is paid with regular checks, and taxes are not withhold on their behalf.
        public string Vendor1099Specified{ get; set; } // Vendor 1 0 9 9 Specified -- Vendor.Vendor1099Specified
        public bool T4AEligible { get; set; } // T 4 A Eligible -- True if vendor is T4A eligible. Applicable for CA/UK versions of quickbooks.
        public string T4AEligibleSpecified{ get; set; } // T 4 A Eligible Specified -- Vendor.T4AEligibleSpecified
        public bool T5018Eligible { get; set; } // T 5 0 1 8 Eligible -- True if vendor is T5018 eligible. Applicable for CA/UK versions of quickbooks.
        public string T5018EligibleSpecified{ get; set; } // T 5 0 1 8 Eligible Specified -- Vendor.T5018EligibleSpecified
        public string CurrencyRef{ get; set; } // Currency Ref -- Reference to the currency all the business transactions created for or received from that vendor are created in.
        public string VendorEx{ get; set; } // Vendor Ex -- Internal use only{ get; set; } // extension place holder for Vendor.
        public string IntuitId { get; set; } // Intuit Id -- IntuitId represents the realm id, authid or an entity id. An entity is a new type of IAM identity that represents a person or a business which has no Intuit authentication context
        public bool Organization { get; set; } // Organization -- True if the entity represents an organization; otherwise the entity represents a person. Default is NULL or False, representing a person.
        public string OrganizationSpecified{ get; set; } // Organization Specified -- Vendor.OrganizationSpecified
        public string Title{ get; set; } // Title -- QBW{ get; set; } // Title of the person. The person can have zero or more titles. Description{ get; set; } // QBO{ get; set; } // Title of the person. The person can have zero or more titles. InputType{ get; set; } // ReadWrite ValidRange{ get; set; } // QBW{ get; set; } // Min=0, Max=15 ValidationRules{ get; set; } // QBW{ get; set; } // At least one of the name elements is required{ get; set; } // Title, GivenName, MiddleName, or FamilyName. ValidationRules{ get; set; } // QBO{ get; set; } // At least one of the name elements is required{ get; set; } // Title, GivenName, MiddleName, FamilyName, or Suffix. I18n{ get; set; } // ALL
        public string GivenName{ get; set; } // Given Name -- Given name or first name of a person.Max. length{ get; set; } // 25 characters.At least one of the name elements is required{ get; set; } // Title, GivenName, MiddleName, or FamilyName. Product{ get; set; } // QBO Description{ get; set; } // Given name or first name of a person.Max. length{ get; set; } // 25 characters.At least one of the name elements is required{ get; set; } // Title, GivenName, MiddleName, FamilyName, or Suffix. Filterable{ get; set; } // ALL Sortable{ get; set; } // ALL
        public string MiddleName{ get; set; } // Middle Name -- Middle name of the person. The person can have zero or more middle names.Max. length{ get; set; } // 5 characters.At least one of the name elements is required{ get; set; } // Title, GivenName, MiddleName, or FamilyName. Product{ get; set; } // QBO Description{ get; set; } // Middle name of the person. The person can have zero or more middle names.Max. length{ get; set; } // 15 characters.At least one of the name elements is required{ get; set; } // Title, GivenName, MiddleName, FamilyName, or Suffix. Filterable{ get; set; } // ALL Sortable{ get; set; } // ALL
        public string FamilyName{ get; set; } // Family Name -- Family name or the last name of the person.Max. length{ get; set; } // 25 characters.At least one of the name elements is required{ get; set; } // Title, GivenName, MiddleName, or FamilyName. Product{ get; set; } // QBO Description{ get; set; } // Family name or the last name of the person.Max. length{ get; set; } // 15 characters.At least one of the name elements is required{ get; set; } // Title, GivenName, MiddleName, FamilyName, or Suffix. Filterable{ get; set; } // ALL Sortable{ get; set; } // ALL
        public string Suffix{ get; set; } // Suffix -- Suffix appended to the name of a person. For example, Senior, Junior, etc. QBO only field.Max. length{ get; set; } // 15 characters.At least one of the name elements is required{ get; set; } // Title, GivenName, MiddleName, FamilyName, or Suffix.
        public string  Product { get; set; } 
        public string FullyQualifiedName{ get; set; } // Fully Qualified Name -- Fully qualified name of the entity. The fully qualified name prepends the topmost parent, followed by each sub element separated by colons. Takes the form of Parent{ get; set; } //Customer{ get; set; } //Job{ get; set; } //Sub-job. Limited to 5 levels.Max. length{ get; set; } // 41 characters (single name) or 209 characters (fully qualified name).
        public string CompanyName{ get; set; } // Company Name -- The name of the company associated with the person or organization.
        public string DisplayName{ get; set; } // Display Name -- The name of the person or organization as displayed. If not provided, this is populated from FullName. Product{ get; set; } // QBW Description{ get; set; } // The name of the person or organization as displayed. Required{ get; set; } // ALL Filterable{ get; set; } // QBW
        public string PrintOnCheckName{ get; set; } // Print On Check Name -- Name of the person or organization as printed on a check. If not provided, this is populated from FullName.
        public string UserId{ get; set; } // User Id -- The ID of the Intuit user associated with this name. Note{ get; set; } // this is NOT the Intuit AuthID of the user.
        public bool Active { get; set; } // Active -- If true, this entity is currently enabled for use by QuickBooks. The default value is true. Filterable{ get; set; } // QBW
        public string ActiveSpecified{ get; set; } // Active Specified -- Vendor.ActiveSpecified
        public string PrimaryPhone{ get; set; } // Primary Phone -- Primary phone number.
        public string AlternatePhone{ get; set; } // Alternate Phone -- Alternate phone number.
        public string Mobile{ get; set; } // Mobile -- Mobile phone number.
        public string Fax{ get; set; } // Fax -- Fax number.
        public string PrimaryEmailAddr{ get; set; } // Primary Email Addr -- Primary email address.
        public string WebAddr{ get; set; } // Web Addr -- Website address (URI).
        public string OtherContactInfo{ get; set; } // Other Contact Info -- List of ContactInfo entities of any contact info type. The ContactInfo Type values are defined in the ContactTypeEnum.
        public string DefaultTaxCodeRef{ get; set; } // Default Tax Code Ref -- Reference to the tax code associated with the Customer or Vendor by default for sales or purchase taxes.
        public string SyncToken{ get; set; } // Sync Token -- Version number of the entity. The SyncToken is used to lock the entity for use by one application at a time. As soon as an application modifies an entity, its SyncToken is incremented; another application's request to modify the entity with the same SyncToken will fail. Only the latest version of the entity is maintained by Data Services. An attempt to modify an entity specifying an older SyncToken will fail. Required for the update operation. Required{ get; set; } // ALL
        public string MetaData{ get; set; } // Meta Data -- Descriptive information about the entity. The MetaData values are set by Data Services and are read only for all applications.
        public string CustomField{ get; set; } // Custom Field -- Custom field (or data extension). Filterable{ get; set; } // QBW
        public string AttachableRef{ get; set; } // Attachable Ref -- Specifies entity name of the attachment from where the attachment was requested
        public string domain{ get; set; } // domain -- Domain in which the entity belongs.
        public string status{ get; set; } // status -- System status of the entity. Output only field. Filterable{ get; set; } // ALL

        //status Enumerated Values
        //    Deleted
        //    Voided
        //    Draft
        //    Pending
        //    InTransit
        //    Synchronized
        //    SyncError


        public string statusSpecified{ get; set; } // status Specified -- Vendor.statusSpecified
        public bool sparse{ get; set; } // sparse -- True if the entity representation has a partial set of elements. Output only field.
        public string sparseSpecified{ get; set; } // sparse Specified -- Vendor.sparseSpecified
        public string NameAndId{ get; set; } // Name And Id -- Property used for Select clauses. This property is not used for entity operation and Where and orderBy clauses.
        public string Overview{ get; set; } // Overview -- Property used for Select clauses. This property is not used for entity operation and Where and orderBy clauses.
        public string HeaderLite{ get; set; } // Header Lite -- Property used for Select clauses. This property is not used for entity operation and Where and orderBy clauses.
        public string HeaderFull{ get; set; } // Header Full -- Property used for Select clauses. This property is not used for entity operation and Where and orderBy clauses.
        #endregion
   
    }
}
