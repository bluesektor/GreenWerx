 
# GreenWerx #  

***An open source software solution used for recording day to day operations of a cannabis related business.***


NOTE: The project as well as the website names have changed and will be updated the site soon. Code from this repository will be split amongst their respective project types.

 GreenWerx.org is an open source start-up project looking to disrupt the conventional "seed to sale" market with software specifically designed to the industry aka Cannabis Management System. By moving to open source and tailoring the software to the industry, we are working to create confidence in a community not only skeptical of outsiders, but help us move forward after so many failures from other companies. No longer will you have to depend on a third party for your information and be at the mercy of someone elses internet connection. You will have the power to keep, run and manage your own system and data as you see fit.



---=== API ===---

C# WebApi.

** Source code

https://github.com/bluesektor/Services

The service and website have code to run an installer similar
to WordPress. So if you run the service (it'll need the install.json in the
/App_Data/Install/ folder, then run the website, the website should start the installer screen to
setup the database and stuff. You can choose to seed the database with data from 
the app_data folder. This will mostly populate the strains and accounts tables.
Accounts represent businesses, so there's some data already in there for cannabis businesses.
The roles system is numeric based so the higher the number the more permissions you have, this
made it cleaner and easier to program for multiple roles requiring same permission.
The api currently runs on MS SQL, there were more providers (MySQL and SQLite),
but trying to maintain all of them when things were changing was too much work. I still have the old code I can upload.

There is also filter and screen classes. Screens are added to a filter to query data.
Filters use json against the class properties in the api. Because the api
utilizes the base node class every object can be filtered on those properties
and extended properties if they are added to the filtering function.

If you're interested in language translation, I started a way to select multiple languages.
So, if you speak English and Spanish you can select both of them and it'll select records
for English and Spanish and then filter duplicate subjects based on the primary language.

There is a lot of code not public but can be. Such as
bit coin/crypto currency, auctions etc... The bitcoin is in another
project but I will make it public if you want it. 
 
I had also started porting it to .Net Core. I had about 80% done I think, but refactored the permissions and
haven't revisited it yet.

---=== Website ===---

Angular 5

** Source Code

https://github.com/bluesektor/Client/tree/master/GreenWerx.NG

This is combination admin/website. It was my first real Angular project and
uses PrimeNG controls. The menus are all 'dynamically' generated in that
the site calls a dashboard endpoint to pull the data for the menus (hardcoded in api for now). 
The public site is simple, it has a store and plain ui. I'll get a public version running
again (switched hosts so I must redo it again).
Some of the features include a web installer that will create and seed your
database with strains, accounts, and roles/permissions. 

<a href="https://greenwerx.org/img/greenwerx.install.png">Install Screen</a>


 ** Admin Screenshots
 
 <a href="https://greenwerx.org/img/admin.strains.png">Strains Screen</a>

<a href="https://greenwerx.org/img/admin.currency.add.png">Currency  Screen</a>
<a href="https://greenwerx.org/img/admin.inventory.png">Inventory</a>
<a href="https://greenwerx.org/img/admin.leftmenu.png">Left Menu</a>
<a href="https://greenwerx.org/img/admin.pricerule.add.png"> Price Rules</a>
<a href="https://greenwerx.org/img/admin.utilities.png"> Utilities</a>




---=== Mobile ===---

Angular 7/Ionic 4
NOTE: I have an updated version in a private repo for another project. I'll
update it with that code (Angular 8/Ionic 5)
I'm thinking this will become an app for dispensaries. I ran across
<a href="https://github.com/7LPdWcaW"> another opensource app</a>
 by <a href="/u/7LPdWcaW">/u/7LPdWcaW</a>  <a href="https://www.reddit.com/r/GrowUtils/">Reddit</a> for growing thaty I may fork and port to run off
the GreenWerx api.


** Source code.
https://github.com/bluesektor/Client/tree/master/GreenWerx.Mobile

If you’re not familiar with Ionic, it will allow you to create
mobile apps for IOS, Android and PWA (progressive web app).   
This does not have an installer for the services and website. I don't want
to bloat this with un-nessary code.
 The code I need to port over has been refined a little more to reuse 
code that was once duplicated. The models mirror the service models so
there is consistency.  There is also code like LetGo, where
 tiled pictures scroll vertically (this was a drone project I worked on and the
guy flaked out). But the nice thing is creating a new "product" was simple because
of the simple consistency.
It was originally forked from the Ionic Conference app. https://github.com/ionic-team/ionic-conference-app
 There have been quite a few changes though lol.
 
For more ideas of what can be done see these pages...
https://ionicframework.com/docs/components/
https://showcase.ionicframework.com/apps/top




---=== WinForms ===---

C#, WinForms

** Source code.

https://github.com/bluesektor/Client/tree/master/GreenWerx.Client
For the application devs there is the WinForms repo. It uses an plugin
system I found somewhere on the web. 



---=== Basic quick start ===---

**Tools to get started.

-- Visual Studio: (Services, Winforms) Everything has been developed using the free version, no additional plugins or special tools.
-- Visual Studio Code: (Website, Mobile) 
-- MS SQL: I use the developer edition. https://www.microsoft.com/en-us/sql-server/sql-server-downloads
--- SQL Server Management Studio https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver15
Download common, webapi and GreenWerx.NG. Run the webapi solution,
then start the GreenWerx.NG website. There is a file in the App_Data/Install/install.json
that will put the service in an install state. When the website starts the api will return
this state which should redirect to the installer. It's a basic install, but it will create
your database, sead it with strains, accounts, and permissions.

---=== How you can help... ===---

	Development
	Design
	Bug Reports  
	Feature Requests
	Usability Feedback
	Bug Fixes
	Marketing
	Graphics
	Web Design
	Language Translations
	CI dev ops
	Helping Members, don't have any yet lol, but hey it's out there.
If you're a new developer feel free to reach out. I like
to mentor and help people along.  

** Spread the word, change doesn't happen in a vacuum!

Feel free 
to create bounties. Other developers can do the work and issue
a pull request for me to add it.

https://www.bountysource.com/
security oriented
https://www.bugcrowd.com
https://en.wikipedia.org/wiki/Open-source_bounty

If you contribute and want, I will be glad to add an attribution and link
etc. to help drive customers to you.

And I would also like to reward contributors, brainstorm
ideas, founding members. If this becomes a thing, I would love
to reward everyone involved. 

Thank you.


**Credits**: [WebAPI Throttle](https://github.com/stefanprodan/WebApiThrottle), [Angular](https://github.com/angular)

**License** [CPAL 1.0](https://opensource.org/licenses/CPAL-1.0)


