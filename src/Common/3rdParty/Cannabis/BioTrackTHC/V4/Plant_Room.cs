using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTrackTHC.V4
{
    //This  contains  plant  room  information  as
    //previously  submitted.  It  is  license 
    //specific and can be queried with all records or only active ones.
    //
    public class Plant_Room
    {
        public string Name { get; set; }//	variable length text field
        public string Location{ get; set; }//	license number of location value
        public int Id	{ get; set; }//integer value
        //from openthc locations
        //  public long Id  { get; set; }
        //public long OrgiId { get; set; } 
        //public long LocationId { get; set; }
        //public string Name { get; set; } 
        //public string Address1 { get; set; } 
        //public string Address2 { get; set; } 
        //public string City { get; set; } 
        //public string State { get; set; } 
        //public string Zip { get; set; } 
        //public int Deleted { get; set; }
        //public int LocationType { get; set; } 
        //public string LicenseNum { get; set; } 
        //public long     LocationExp { get; set; } 
        //public long    LocationIssue { get; set; } 
        //public string   Status { get; set; } 
        //public string  DistrictCode { get; set; } 
        //public decimal    LocLatitude { get; set; }
        //public decimal   LocLongitude { get; set; } 
        //public long     LocUbi { get; set; } 
        //public bool   Producer { get; set; } 
        //public bool   Processor { get; set; } 
        //public bool   Retail { get; set; } 
        //public long TransactionId { get; set; }
        //public long TransactionId_Original { get; set; }
        //public long  Fifteenday_End { get; set; } 
        //public DateTime Delete_Time { get; set; } 
        #region ideas
        //temperature
        //light source/ type during vegative, flowering
        //distance
        //we see great variability in the plant’s phenotypic expression: nutrients, temperature, the amount and angle of light, soil type, photoperiod length, time of harvest, and the distance between the plant and light source are among the many conditions that affect the plant’s characteristics. C
        //height measurements
        //harvest weight

        //projeted harvest date
        //flower density /compactness of buds
       // nutrient formula
       // keep track of your patients and their plant counts.
        //tags
        
    //Real Time - Encryption of data and pictures for the maximum protection of your information
    //Track an unlimited number of plants*
    //New - Grow Room Tracker - helps keep track of all your equipment

    //Attach and unlimited number of photos to any plant
    //Track All your finances in the Finance Tracker
    //Never loose track of your genetics, from clone all the way through enjoying your harvest!
    //unlimited notes about each plant
    //Ability to backup and import your data into versions
    //Optional ability to designate the days to flower your plant with a friendly reminder to harvest once it's time.
    //Multi Room setup - Run multiple operations from the same program
    //Printable dry tags to keep track of your plants during drying/curing
    //Track the heights of your plants as they grow
    //Final Yield / Final Height fields allowing with the daily gain of both
    //Track your seeds with the SeedBank
    //Printable Plant reports
        //Plant your seed — grow it — clone it — grow that — clone both — flower one — introduce a new mother — clone everything — eliminate males — harvest on time. Do all this at the same time with an unlimited number of mothers, clones, and plants without the fear of losing track.

        //germination, seed pre-soak time
        //sprout time
       // germ to harvest and curing

        // bluetoothing a usb scale, and making use of bt barcode readers to track indiv plants. 
        //Look at dog breeding programs, it's suprisingly the same as plants. You can do pedigrees, breeding program, schedules, every possible variable is in there to work with and develope a database of grow runs
        //pedigree layouts and dogs are bred about the same way as plants with linebreeding, inbreeding, cross breeding, outcrossing, trait calculations, lineage, parent stock tracking, 
//Crops
//Manage all your marijuana plants by age, growth stage, strain, and quickly record waterings. 

        
//Plants
//Manage your plants by strain or nickname. View the lifespan of your plant, the date it started each stage, its estimated harvest date, and a lot more. 

    
//Nutrients
//Create a virtual list of all your nutrients to quickly create custom nutrients schedules and easily tweak your feeding on the fly. 

    
//Nutrient Schedules
//Create custom nutrients schedules to account for variations in strains, grow mediums, and room conditions, allowing you to grow the dopest dope you’ve ever smoked. 

    
//Strains
//Get to know your plants better by keeping track of your strains, their descriptions, flowering lengths, and genetics all in one place.
//Mediums
//Keep a record of everything your plants are grown in; Soil, coco, rockwool, hydro corn, or your own custom blend.

//Notes
//Keep all of your notes in one place, no more scribbling on whiteboards or messy journals. GrowBuddy organizes all of the notes for your garden, crops, and plants in one place to easily find every note.
//Tasks
//Never forget a thing again. Create a single or repeating task for your garden, crop, or plant so you can stay on top of your grow.
//Journal
//Review all of your past grows in a day by day grow journal. View all the feedings, notes, and tasks to maximize your future yields and minimize your losses.


//Single crop/plant watering
//GrowBuddy will automatically pull the correct nutrient schedule and week for the plant or crop, whether its in clone, vegetative, or flowering stage.
//Multiple crop watering
//Quickly record waterings for multiple crops with just a few clicks using the muilti-crop watering feature.
//Calculations
//Never add the wrong amount of nutrients again, GrowBuddy will do all the calculations for you.
//Feedings
//Easily view past feedings while watering to keep your plants free of deficiencies. 

        //light
            //balast
            //spectrum
       // humidity

//        Multiple Grow Room Support
//Room lighting
//Room Size
//Room Temp (logged)
//Room Ventilation (logged)


//Hydro or Soil (Variations of certain logging/tracking aspects therein)

//-Hydro
//Multiple Controllers per grow room
//Plant Sites supported per controller
//Controller Volume (Gallons)

//Controller Temp (logged)
//Controller PH (logged)
//Controller TDS (logged)
//Controller Changes (Water, Nutes, Etc logged)

//-Soil
//Per Plant Logging

//-Plants
//Name - ID
//Picture (optional)
//Strain
//Born on Date
//Origination (Seed-Cutting)
//Mother Name-ID
//Type (Soil-Hydro)
//Stage (Veg/12s)
//Size (Logged)
//Yield (wet/dry/trimmed)
//1. Add mother plants or a "Clone" button option to make clones and just add an incremental number after the cutting, so it will sorta automate that process. Strain Clone #x. I hope that sorta makes sense.

//2. Copy. Being able to make copies for adding the 5 plants of the same strain you already have going.

//3. Events Reservor Seclection, Drop down selection of reservoir and how much was used, and let the program do the math.

//4. Reservoir. Drop down selection from your nutrients. I'm sure that one is being worked on already.
        //enviro data collection? I'm using lacrosse hardware and the wxweather package to gather data. expensive all around, but my understanding is the lacrosse people have some sort of exclusive license with them and would not provide their data structures to others. it's a suck arrangement, first you lay out $200- $300 for the hardware then it's $70 more to collect the data. 
//        Complete Climate Controller
//Hydroponics Controller
//Environmental Controller
//Temperature Controller
//Humidity Controller
//CO2 Controller
//pH and EC / TDS controller
//Reservoir Temperature Controller
//Reservoir Level Controller
//Security System
//Flood Detection and Alert System
//Fogger Controller
//Irrigation Controller
//Ebb and Flow table Controller
//Time lapse movie creator
//DWC Level Controller
//Deep Water Culture Controller
//Cycle Timer(s)
//Lighting Controller
//Atmospheric Controller
//Long Term data logger with advanced graphing features
//Control MULTIPLE growing areas from one system
//And much more, the system is extremely configurable
//    Growtronix can Monitor:
// Temperature Sensors
// Humidity Sensors
// CO2 Sensors
// pH Sensors
//chart23.jpg EC / TDS Sensors
// Reservoir Temp Sensors
// Light Detectors
// PAR Light Sensors
// Soil Temperature Sensors
// Flood Detectors   
// Smoke Detectors
// Float Sensors
// Door / Window Sensor
// Motion Sensors
// IP / Web Cams
//     Growtronix Controllable Power Outlets control the following devices within your grow room environment:
//• Grow Lighting	• Exhaust Systems 
//• Pumps	• CO2 Burners and Regulators
//• Air Conditioning 	• Heaters
//• Humidifiers	• Dehumidifiers 
//• Water Chillers	• Water heaters
//• Sulfur Burners	• Ozone Generators
//• Solenoid Valves	• Door Locks
//• Just about anything else	 
//        The controls can be setup to turn an outlet ON based on a sensor’s reading, the time of day, the day of the week, as a cycle (e.g. 10 minutes ON 2 hours 10 seconds OFF), another outlets on/off status or any combination of the above. This gives you the ability to control your growing area with the greatest degree of ease and sophistication

 

//iphone image

//Software Features:

//    Allows for easy automation of all aspects of the environment
//    Remote Monitoring and Control from any internet connection
//    Remote Monitoring and Control from any web enabled cell phone
//    Customizable alerts and notifications by Text Message or Email
//    Long Term data storage in built in database
//    Charts and Graphing
//    Play music and sound files in the growing environment automatically
//    Create Time Lapse videos

     

//Grow Analytics Features:

//    Min, Max and Average readings for sensors 
//    Total time ON and time OFF for all outlets 
//    Shows Total Kilowatt Hours (KWH) for outlets 
//    Calculates Electrical Costs for each outlet
//    Calculates total electrical cost 
//    Write and save notes and comments 
//    Store images for each day 
//    Builds and Display averages charts 
//    Builds and Display Charts 
//    Grow Room Timer (Keeps track of days in bloom or vegetative stages) 
        #endregion

      
    
    }
}
