# TreeMon Project Wiki #

  Version 0.1.0 

   TreeMon is a multi-project/solution built on C#, WebAPI, Angular and Windows forms. For more information see the [readme file](https://github.com/bluesektor/TreeMon/blob/master/README.md).

## Run the code.. ##
If you haven't done so already you'll need to download the following developer tools;

**Database:**  **[MS SQL Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)** is the database the WebAPI and client solutions use. I have the project running on 2014 and 2016 express and looks like they have 2017 out, it should work with this also. You may need the**[ management studio](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms)** if installing 2017. **Note** there was a version running in MySQL so it may still work with it by setting the correct connection string in the .config file. Not promising anything, but if you're feeling froggy.. Another version ran on SQLite. I can publish the old version of the database if your interested. 

**TreeMon.WebAPI:** API where web requests are sent. Developed in community edition of [**Visual Studio 2015 or 2017**](https://www.visualstudio.com/thank-you-downloading-visual-studio/?sku=Community&rel=15).  
1. Open solution  **\src\Services\TreeMon.WebAPI\TreeMon.WebAPI.sln** with Visual Studio.               

**TreeMon.NG:** Website built with [Angular 4](https://angular.io/) and [PrimeFaces Angular controls](https://www.primefaces.org/primeng/#/). 
 To run the website code you'll need **[VS Code](https://code.visualstudio.com/download)**, **[Node](https://nodejs.org/en/download/)**
  *You'll need to run TreeMon.WebAPI solution when running the website.*
1. Install NODE    
2. Open folder **\src\Client\TreeMon.NG** with Visual Studio Code.
3. You will need to run an install for node and update the packages. May look something like this *npm install -g npm-check-updates*
  
**TreeMon.Client:** This solution is a desktop application and does **NOT** require the WebAPI solution to run.
Developed in community edition of [**Visual Studio 2015 or 2017**](https://www.visualstudio.com/thank-you-downloading-visual-studio/?sku=Community&rel=15). This should be TreeMon.Forms but haven't got around to updating it yet.  
1. Open solution **\src\Client\TreeMon.Client\TreeMon.Client.sln** with Visual Studio.

               
## Architecture     
There are four main projects "core", the solutions utilize. 
WebAPI and client solutions revolve around the core projects. **The core projects should not reference or contain code that utilize client side technology. Like Session[], http request, Forms controls.**
Place those files in the client itself or create another folder inside the common and create a project representing the client side technology being used.

**Core**

- Models - POCO classes
- Utilites - Misc. classes to help get stuff done.
- Data   - Database context and other db/logging functionality
- Managers - Handle business logic and permissions

**Core projects dependency**  

- **Models:** No other projects required.
- **Utilities:** Requires: Models.
- **Data:**  Requires Models and Utilities.
- **Managers:** Requires Models, Utilities and Data.

**Solution dependency structure:**  
- **Core:** The core projects above are used int the other solutions to provide data access, security etc.

- **TreeMon.Client:** Requires Core projects.
- **TreeMon.WebAPI:** Requires Core projects.
- **TreeMon.NG:** Requires TreeMon.WebAPI to send requests to. 
- **TreeMon.Mobile:** Not implemented.

**Framework**        

- **Interfaces:** 
 -  INode: Foundation of the model classes. Most classes inherit the INode interface as a way to create  standard base models across applications.
 
 -  ICrud: Foundation of the manager classes. Most of the managers inherit this interface as a way to create standard data access functions across applications.
 
  
  					Delete(INode n, bool purge = false)
                            purge = false. The records Deleted flag is set to true.
                            purge = true. The record is removed from the database table.

                    Get( string name)
                            Retrieves the record by the Name field, case insensitive.

                    GetBy(string uuid);
                            Retrieves the record by the UUID field, case sensitive.

                    Insert(INode n, bool validateFirst = true);
                            Adds the object record to the database table.
                            validateFirst = true checks the table by name for a match. 
							If there is a match no record is inserted.

                    Update(INode n);
                            For web clients only specific fields are updated. 
							See the controller for each request to verify what will be updated.
                            This is because not all data is sent when the initial request is made.
                            The forms client updates the entire record.
 -  IDbContext: Foundation of the database context classes as a way to create standard database context functions across providers. 
 
 - ISessionManager: Not fully implemented. Meant as a way to create standard session managers across providers i.e. in memory as cache, database..
 
 - IPlugin: Forms project only. All plugins inherit this interface. Contains standard fields and methods for implementing a plugin.
 
 - IPluginHost: Forms project only. Implemented in IPlugin and inherited by PluginServices. Implementadion includes setting the path of the application in the plugin and feedback to the main form about the pluing.

- **Models**        
 - Node:  Most classes inherit directly from here. Some exceptions are classes that derive from the Item class, which derives from Node.
 
 - Item: Inherits Node. Classes inheriting Item are typically items that would used in buying or selling     transactions like a store.
 
  - Managers classes should Inherit ICrud interface.
  
 - ServiceResult: Returned by WebAPI controllers and some managers as a way to provide extended information about the result if needed. 


**Program Flow**

- **TreeMon.NG** --request--> **TreeMon.WebAPI** controller -->(enter the core) manager -> dbcontext -> database

- **TreeMon.Client** -->(enter the core) manager -> dbcontext -> database

                             
        
                      

          