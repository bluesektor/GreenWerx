using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTrackTHC.V4
{
      #region plant_destroy_schedule
        //The plant_destroy_schedule function will allow a licensee to schedule for destruction a plant or set of plants. This event will begin a 72-hour waiting period before a plant_destroy function may be called on the plant(s). The optional override parameter can be used in instances where a user successfully initiated a scheduled destruction across one or more plants but, e.g. they failed to commit locally to a user’s platform. Essentially, it will suppress the error message that indicates an item has already been scheduled and will handle any additional items within the list. It will NOT suppress any other error messages.
//    reason	variable length text field
 // public string barcodeid { get; set; }//	Array	of	1	or	more	text	fields
//representing the plants

//override	Optional,	0	or	1	Boolean	value
//(defaults to 0 if omitted)

#endregion

    #region plant_destroy
//        The plant_destroy function will allow a licensee to destroy 
//a plant or set of plants. Plants may   only   be destroyed
//after   the   waiting   period   has   expired. Please   see   the 
//plant_destroy_schedule function for an explanation on the optional override parameter.

//    action	variable length text field
// // public string barcodeid { get; set; }	Array	of	1	or	more	text	fields
//representing the plants

#endregion

    #region plant_harvest_schedule
//        The plant_harvest_schedule function will notify the traceability system of intent to begin 
//harvesting a plant or set of plants. This notification must occur before the plant_harvest 
//is called on these plants.

    //        barcodeid	Array  of  1  or  more  text  fields
    //representing the plants
#endregion

    #region plant_harvest

//        The plant_harvest function will begin the process of harvesting a plant. This will move 
//said plant from the “growing” phase to the “drying” phase. During this process, a cultivator must take, at a minimum, a wet weight of the plant. In addition, a cultivator 
//may also gather two additional derivatives defined by their inventory type.
//Specifically, the system requires inventory type 6 (Flower) and optionally allows type 9 (Ot
//her Plant Material) and type 27 (Waste).Harvests can be partial, as well. In other words, if part of the plant is harvested and 
//the rest of the plant will be processed later (commonly known as re-flowering), then 
//the collectadditional parameter should be 1. This will inform the traceability system 
//to expect another additional wet weight.
//Each  harvest  event  should  be  on  a  per-plant  basis. 
//So  every  individual  plant  will need its own wet weight reported.
//Both Other Plant Material and Waste collected 
//during this process will receive random unique identifiers. For Other Plant Material, 
//this will facilitate the process of creating a lot. For Waste, this will allow a user to 
//accumulate waste in a traceable manner
//and schedule a destruction event at a later point. 

//        action	variable length text field
//collectiontime	Optional,Unix    32-bitinteger
//    timestamp, defaults to current time
 // public string barcodeid { get; set; }	Array of one or more unique plant
//    identifiers
//weights	Array of 1 or more nodes containing
//    weight information
//amount	decimal value
//invtype	integer    value    representing    the
//    derivative type
//uom	variable length text field. Valid values
//    are: g, mg, kg, oz, lb. These represent:
//    grams, milligrams, kilograms, ounces
//    and pounds.
//collectadditional	Keeps the plant in the growing phase
//    and allows the user to take another
//    wet weight of the plant(s) at a later
//    point  that  will  compound  to  the
//    original wet weight.
//new_room	Optional, will move the now drying
//    plant(s) to another plant room.


#endregion


    #region      plant_waste_weigh
//        The plant_waste_weigh function will allow a cultivator
//to take a general waste weight for destruction  accountability  at  a  later  point.
//General  leaf,  stem,  veg  trimming,  etc. collection can thus be facilitated in a more generalized fashion without unduly burdening 
//a licensee. The return inventory will be typed as 27 and must be scheduled for destruction at a later point.
//            collectiontime	Optional,	Unix    32-bit	integer
//    timestamp, defaults to current time
//weight	decimal value	
//uom	variable length text field. Valid values
//    are: g, mg, kg, oz, lb. These represent:
//    grams, milligrams, kilograms, ounces
//    and pounds.		
//location	license number of location	

#endregion

    #region plant_cure
//        The plant_cure function will begin the process of curing a plant. This will move said 
//plant from the drying phase to inventory. During this process, a cultivator must take, at 
//a minimum, a dry weight of the plant. In addition, a cultivator may also gather additional 
//derivatives  defined  by  their  inventory  type. Specifically,  the  system  requires  inventory 
//type 6 (Flower) and optionally allows type 9 (Other Plant Material) and type 27 (Waste).
//If  the  cultivator  is  doing  a  partial  harvest/cure,  the  plant
//can  pass  through  this function  again  to  accumulate an  additional  dry  weight. 
//        If  the  cultivator  is  re-flowering, ensure the collectadditional field is set to 1.
//    collectiontime	Optional,	Unix
//    timestamp, defaults to current time
 //// public string barcodeid { get; set; }	Array of one or more unique plant
//    identifiers	
//        weights	Array of 1 or more nodes containing
//    weight information
//amount	decimal value
//invtype	integer    value    representing    the
//    derivative type
//uom	variable length text field. Valid values
//    are: g, mg, kg, oz, lb. These represent:
//    grams, milligrams, kilograms, ounces
//    and pounds.
//collectadditional	Keeps the plant in the growing phase
//    and allows the user to take another
//    wet weight of the plant(s) at a later
//    point  that  will  compound  to  the
//    original wet weight.
//room	integer, room the collection occurred
//    in
//location	license number of location

#endregion

    #region plant_convert_to_inventory
//        The plant_convert_to_inventory 
//function will allow a licensee to convert a plant that is 
//growing (but not flowering) into an inventory item that can then be transferred and sold. 
//Once converted, the new item will keep its identifier but will now have an inventory type 
//of 12(Mature Plant).
//        barcodeid	Array	of	1	or	more	text	fields

//representing the plants to convert

        #endregion

    #region plant_yield_modify
//        The  plant_yield_modify function  will
//allow  direct  access  to  modify  previously  stored 
//values for harvest and cure  collections.
//The user will need to specify one transaction at a 
//time. The integrator is, of course, free to hide this from the end
//calls behind the scenes if they display the capability to modify collected values in a unique 
//or innovative way. The user can, however, specify all values that would have been specifiable at the time of 
//the original transaction. That is, if the transaction relates to the plant_harvest, wet weight 
//and  any  derivative  can  be  specified.  If  the  original  transaction  was  a  p
//lant_cure,  dry weight could be specified, instead. Only values that are included will be modified. If a 
//user wishes to zero out a value, it must be declared. Null or absent values will retain their 
//previous values.

//        collectiontime	Optional,	Unix	32-bit	integer
//timestamp, defaults to current time
//transactionid	integer, the transaction to correct
//weights	Array of 1 or more nodes containing

//weight information
//amount	Optional, decimal value
//invtype	integer	value	representing	the
//derivative type

//uom	variable length text field. Valid values
//are: g, mg, kg, oz, lb. These represent:
//grams, milligrams, kilograms, ounces
//and pounds.


        #endregion 
        
    #region plant_modify   
//        The plant_modify function will allow direct access to modify previously stored values 
//for a plant. The user will need to specify one plant at a time. The 
//integrator is, of course, free  to  hide  this  from  the  end-user  with  multiple API  calls  behind  the  scenes  if  they 
//display the capability to modify collected values in a unique or innovative way.
//The user will need to specify the barcode id and, optionally th
//e new strain, new mother flag or new room.

//        strain	Optional, variable length text field of
//the new strain name
//room	Optional, integer value that will move

//the plant to another plant room.
//mother	Optional,  integer  value  indicating if
//the plant is a mother plant
//birthdate	Optional, 8 character birthdate in the

//following format: YYYYMMDD. If
//not provided, the system will default
//to the current date.

#endregion

    #region inventory_adjust
//        The inventory_adjust function will allow a licensee to adjust the amount or quantity of
//an  inventory  item.  The  type  field  can  represent  one  of  the  following:  1  (General 
//Inventory  Audit),  2  (Theft),  3,  (Seizure  by  Federal,  State,  Local  or  Tribal  Law Enforcement), 4 (Correcting a mistake), 5 (Moisture loss, e.g. wet other plant material), 
//6 (Depletion, e.g. inventory type 11). For backward compatibility, reason and type can 
//be provided outside of the data array as a fallback default. The integrator can also choose 
//whether to provide the new quantity to adjust to (with the quantity parameter) or can 
//simply provide the remove_quantity parameter. It is recommended to only provide one 
//or the other. The system will look for remove_quantity first and fallback to quantity if not found.
//    data	Array of 1 or more nodes containing

//inventory information
 // public string barcodeid { get; set; }	inventory identifier
//quantity	Decimal value, optional if
//remove_quantity is provided, new

//quantity to adjust to.

//quantity_uom	variable length text field. Valid values
//are: g, mg, kg, oz, lb, each. These
//represent: grams, milligrams,

//kilograms, ounces, pounds, each.
//remove_quantity	Decimal value, optional if quantity is
//provided, quantity to remove. Does
//not need to be remaining quantity

//(can be a partial removal).
//remove_quantity_uom	variable length text field. Valid values
//are: g, mg, kg, oz, lb, each. These
//represent: grams, milligrams,
//kilograms, ounces, pounds, each.

//reason	variable length text field explaining in
//greater detail the reason for the
//removal or addition of inventory
//type	Integer value representing the type of

//adjustment.

#endregion

    #region inventory_destroy_schedule
    //The inventory_destroy_schedule function will notify the traceability system of intent to destroy an inventory item. Per current rules, this function can only (currently) be called by producers and processors. Please see the plant_destroy_schedule function for an explanation on the optional override parameter.
//    barcodeid	Array	of	1	or	more	text	fields
//representing the inventory
//reason	reason for the destruction
//override	Optional,	0	or	1	Boolean	value

//(defaults to 0 if omitted)

#endregion

   #region inventory_destroy
    //The inventory_destroy function will allow a licensee to destroy an item that has been previously scheduled for destruction. Please see the plant_destroy_schedule function for an explanation on the optional override parameter.
//    barcodeid	inventory identifier
//reason	reason for the removal of inventory
//override	Optional,	0	or	1	Boolean	value

//(defaults to 0 if omitted)

#endregion

   #region inventory_move
    //The inventory_move function will update the current room for the specified inventory items. Essentially, it allows a user to move inventory from one room to another.
//    data	Array of 1 or more nodes containing
//inventory information

 // public string barcodeid { get; set; }	inventory identifier
//room	Integer	value,	represents	the
//identification number of a room

#endregion

    #region inventory_check
    //The inventory_check function can be used to perform a cursory lookup on an item before an inbound inventory_transfer from an outside licensee. It will pull various pieces of inventory on the inventory identifiers specified in the request. This information can include: strain, quantity available, usable weight (if applicable), product (if applicable) and inventory type.
//    barcodeid	Array	of	1	or	more	text	fields
//representing the inventory to lookup

#endregion

    #region inventory_new
 //   The inventory_new function can be used to create new inventory not previously entered into the system. This function is ONLY accessible to a licensee that has been designated as a producer. It may be used for the first 15 days of operation without a source_id. Subsequent calls to this function will require a source_id of a plant in cultivation that has been designated as a mother plant. Only four types may be provided to this function without a source_id: Seed, Clone, Mature Plant
//and Plant Tissue. After the 15 day period, only three types may be provided: Seed, Clone and Plant Tissue.
//    location	license number of location
//data	Array of 1 or more nodes containing

//new inventory information
//strain	variable length text field
//quantity	integer value
//invtype	integer, corresponds to the inventory

//type system
//source_id	text field, optional when within the
//15 day period

#endregion

    #region inventory_manifest
//The inventory_manifest function will notify the traceability system of intent to transfer an inventory item. This function will need to be called in instances of transfers from one licensee to another. It will also need to be called for licensees which possess multiples licenses (e.g. Producer + Processor) that possess different license numbers. For internal transfers (e.g. from one part of a facility to another), there is no need to quarantine and schedule a transfer. In previous versions, this function did not require a location or a stop_overview and assumed a single stop. The previous syntax, although deprecated, is still supported.
//    employee_id	variable length text field
//vehicle_id	integer value
//location	license number of origin location

//stop_overview	Array of 1 or more nodes containing
//stop information
//approximate_departure	Unix	32-bit	integer	timestamp,
//approximate departure time

//approximate_arrival	Unix	32-bit	integer	timestamp,
//approximate arrival time
//approximate_route	variable length text field, route that
//will be used

//vendor_license	license number of vendor the item(s)

//are being transferred to
//stop_number	stop number of the overview, integer
//greater than or equal to 1

 // public string barcodeid { get; set; }	Array	of	1	or	more	text	fields
//representing	the	items	to	be

//transferred on the specific stop
//new_room	Optional, can specify the item(s)
//have been placed into e.g. a
//quarantine room.

#endregion

    #region inventory_manifest_lookup
    //The inventory_manifest_lookup function can be used to offer a heads up of shipments that have been both manifested and transferred out of one licensee and are ready to be transferred into the receiver’s inventory.
    //location	license number of location
#endregion

    #region inventory_manifest_void
    //The inventory_manifest_void function will cancel a manifest that has been previously filed.
        //manifest_id	manifest identifier


#endregion

    #region inventory_transfer_lookup
//    //The inventory_transfer_lookup function can be after the inventory_manifest_lookup function, or, alternatively, after having the manifest identifier in hand to retrieve specific details on the receiving items.
//    //location	license number of location
    #endregion

    #region inventory_transfer_outbound
//    The inventory_transfer_outbound function can be used to transfer inventory that already exists in the system. A manifest must be filed prior to all transfers.
//        manifest_id	manifest identifier obtained from
//previously filed manifest
//data	Array of 1 or more nodes containing
//inventory information

 // public string barcodeid { get; set; }	inventory identifier
//price	Optional if inter-UBI transfer,
//decimal value that indicates how
//much the item was sold for before

//any applicable taxes.

#endregion

    #region inventory_transfer_outbound_modify

//The inventory_transfer_outbound_modify function will allow a user to modify the price recorded for an inventory transfer sale. This can be used before filing a monthly report if a line item mistake is noticed and needs to be corrected.
//transactionid	integer value
 // public string barcodeid { get; set; }	inventory identifier

//price	Decimal value representing the price
//paid before any applicable taxes.
//item_number	Optional, integer, should be provided
//if multiple line items of the same

//barcode were included in one sale. 0
//would represent the first item (in the
//order submitted to the system), 1 the
//next, etc.

#endregion

    #region inventory_transfer_outbound_void

//The inventory_transfer_outbound_void function will allow a user to void an inventory transfer that has been completed but not yet received by the recipient. This can be used for instances where a sale has been reported complete on the sender end; but was made in error. The transfer can then be made again; or the manifest voided, if necessary.
 

//transactionid	integer value

#endregion

      #region inventory_transfer_inbound

//The inventory_transfer_inbound function can be used to officially receive inventory from another licensee.

//Parameters:
//action	variable length text field

//location	license number of location
//data	Array of 1 or more nodes containing
//inventory information
 // public string barcodeid { get; set; }	inventory identifier

//quantity	Quantity or amount received
//uom	variable length text field. Valid values
//are: g, mg, kg, oz, lb, each. These
//represent: grams, milligrams,

//kilograms, ounces, pounds, each.

#endregion

    #region inventory_create_lot

//The inventory_create_lot function will allow a user to combine inventory types 6 (Flower) and 9 (Other Plant Material) into lots as mandated by rules. The return types will be 13 (Flower Lot) and 14 (Other Plant Material Lot), respectively.

//Parameters:
//action	variable length text field

//lot_quantity	decimal value, new quantity of
//combined items
//lot_quantity_uom	variable length text field. Valid values
//are: g, mg, kg, oz, lb, each. These

//represent: grams, milligrams,
//kilograms, ounces, pounds, each.

//data	Array of 1 or more nodes containing
//inventory information

 // public string barcodeid { get; set; }	inventory identifier
//remove_quantity	integer value, quantity to remove.
//Does not need to be remaining
//quantity (can be a partial

//combination).
 

//remove_quantity_uom	variable length text field. Valid values
//are: g, mg, kg, oz, lb, each. These

//represent: grams, milligrams,
//kilograms, ounces, pounds, each.

#endregion

    #region inventory_split

//The inventory_split function will allow a user to split inventory items into sub lots or sub batches. For example, if a user has a lot of Flower and only wishes to sell half of it, they would need to first create a sub lot using this function. Then, with the new lot number, they can sell the desired amount. Multiple lots or batches can be specified at a time, however, keep in mind they will not be combined. Rather, each one will receive a new sub-lot or sub-batch number.



//Parameters:
//action	variable length text field
//data	Array of 1 or more nodes containing
//inventory information

 // public string barcodeid { get; set; }	inventory identifier
//remove_quantity	integer value, quantity to remove.
//Does not need to be remaining
//quantity (can be a partial

//combination).
//remove_quantity_uom	variable length text field. Valid values
//are: g, mg, kg, oz, lb, each. These
//represent: grams, milligrams,

//kilograms, ounces, pounds, each.

#endregion

    #region inventory_convert

//The inventory_convert function will allow a user to convert one type of item to another. The system allows for multiple sources. So, for example, a processor may use part of various Other Plant Material Lots in producing a batch of hash oil. Certain derivatives may not be strain specific, so entering a strain is optional under those circumstances. Product name is optional when it is not the end product. If the derivative item will be sold to a consumer (that is, inventory types 22,23,24,25) and is not regular usable marijuana (type 28), then a product will be required (e.g. Cookie, Brownie, etc).

//Parameters:
//action	variable length text field
//data	Array of 1 or more nodes containing
//inventory information

 // public string barcodeid { get; set; }	inventory identifier
 


//remove_quantity	integer value, quantity to remove.
//Does not need to be remaining

//quantity (can be a partial
//combination).
//remove_quantity_uom	variable length text field. Valid values
//are: g, mg, kg, oz, lb, each. These

//represent: grams, milligrams,
//kilograms, ounces, pounds, each.
//waste	decimal value, amount of waste
//produced by the process, if any

//waste_uom	Valid values are: g, mg, kg, oz, lb.
//These represent: grams, milligrams,
//kilograms, ounces, pounds.

//derivative_type	Inventory type of derivative item

//derivative_quantity	decimal value, quantity of new
//derivative after conversion
//derivative_quantity_uom	Valid values are: g, mg, kg, oz, lb,
//each. These represent: grams,

//milligrams, kilograms, ounces,
//pounds, each.
//derivative_usable	decimal value, quantity of usable
//marijuana in new product after

//conversion
//derivative_usable_uom	Valid values are: g, mg, kg, oz, lb,
//each. These represent: grams,
//milligrams, kilograms, ounces,

//pounds, each.
//derivative_strain	Optional, variable length text field
//derivative_product	Optional, variable length text field

#endregion

     #region inventory_sample

//The inventory_sample function will allow a user to provide samples as allowed by law. Specifically, samples can be provided to employees for quality assurance purposes or to vendors for the purposes of negotiating a sale. Either employee_id or vendor_license should be provided; but not both.
 

//Parameters:
//action	variable length text field

 // public string barcodeid { get; set; }	inventory identifier
//employee_id	Optional, variable length text field
//vendor_license	Optional, variable length text field
//representing license number of

//receiving entity
//quantity	decimal value, quantity of old
//product before conversion
//quantity_uom	Valid values are: g, mg, kg, oz, lb,

//each. These represent: grams,
//milligrams, kilograms, ounces,
//pounds, each.

#endregion

    #region inventory_qa_sample

//The inventory_qa_sample function will allow a user to provide QA samples to qualified testing facilities as allowed by law.

//Parameters:
//action	variable length text field
 // public string barcodeid { get; set; }	inventory identifier
//lab_id	variable length text field, license

//number of the QA facility
//quantity	decimal value, quantity of old
//product before conversion
//quantity_uom	Valid values are: g, mg, kg, oz, lb,

//each. These represent: grams,
//milligrams, kilograms, ounces,
//pounds, each.
//use	Optional. If the inventory type is 13

//(flower lot), this field should be 1 to
//indicate the lot will be used to
//convert to usable marijuana (type 28,
//e.g. pre-packs), or 0 to indicate it will

//be used for an extract. Converting
//directly to type 28 will trigger more
//rigorous QA test requirements.

#endregion

      #region inventory_qa_sample_void

//The inventory_qa_sample_void function will void a sample that has been sent out (from the traceability system’s perspective), but not tested yet.

//Parameters:
//action	variable length text field
//transactionid	integer value


#endregion

    #region inventory_qa_sample_results

//The inventory_qa_sample_results function will allow a user or laboratory to provide QA results as allowed by law. As QA facilities will be reporting directly, most licensed facilities will not need to report the results themselves.

//Parameters:

//action	variable length text field

//sample_id	sample identifier
 
//test	Array of 1 or more nodes containing
//test details

//The parameters to expect for each test can be found in both the example and tables below.
//       QA Test Types

//1	Moisture Content
	
//2	Potency Analysis
	
//3	Foreign Matter Inspection
	
//4	Microbiological Screening
	
//5	Residual Solvent
	
//Moisture Content Details	
	
//Parameter	Details
	
//moisture	Moisture Content, whole number only
	
//Potency Analysis Details	
	
//Parameter	Details
	
//THC	THC Content
	
//THCA	THCA Content
	
//CBD	CBD Content
	
//Total	Total Cannabinoid Profile
	
 
//    Foreign Matter Types			
				
//    Parameter		Details	
				
//    Stems		Content of the aforementioned matter, as	
//            a percentage	
				
//    Other		Content of the aforementioned matter, as	
//            a percentage	
				
//    Microbial and Fungal Counts (Colony Forming Units [CFU]/g)
				
//    Parameter		Details	
				
//    aerobic_bacteria		Total viable aerobic bacteria count	
				
//    yeast_and_mold		Total yeast and mold count	
				
//    coliforms		Total coliforms count	
				
//    bile_tolerant		Bile-tolerant gram-negative bacteria	
				
//    e_coli_and_salmonella		E. coli and Salmonella	
				
//    Residual Solvent Details			
				
//    Parameter		Details	
				
//    residual_solvent		Residual Solvents	
				
#endregion

    #region inventory_qa_check

//The inventory_qa_check function will pull down lab results that have been submitted to the traceability system by a certified QA lab.

//Parameters:
//action	variable length text field
//sample_id	sample identifier

#endregion

    #region inventory_modify

//The inventory_modify function will allow a producer to modify the strain on inventory that can be used as a plant source (inventory types 7, 10, 11, 12) or inventory that was incorrectly classified but not yet grouped (inventory types 6, 9, 27).

//Parameters:
//action	variable length text field

 // public string barcodeid { get; set; }	16 digit barcode identifier

//strain	variable length text field


#endregion

     #region sale_dispense

//The sale_dispense function will allow a user to deduct items from inventory through the sales process. Since all items sold must be pre-packaged, units will be assumed to be

//“each”.

//Parameters:

//action	variable length text field
//data	Array of 1 or more nodes containing
//inventory information
 // public string barcodeid { get; set; }	inventory identifier

//quantity	integer value, quantity to remove
//price	Decimal value representing the price
//paid before any applicable taxes.
//item_number	Optional, integer, should be provided

//if multiple line items of the same
//barcode were included in one sale. 0
//would represent the first item (in the
//order submitted to the system), 1 the

//next, etc.
//sale_time	Optional, unix 32-bit integer
//timestamp of when the sale occurred.
//If not used, will default to current

//time. Otherwise, the time must not
//be in the future and, also, must not

//be in a locked tax period.

#endregion

    #region sale_void

//The sale_void function will reverse items that have been sold to a customer and return the items to inventory. A refund should be used, instead, when the return is not being used to simply fix a mistake.

//Parameters:
//action	variable length text field
//transactionid	integer value

#endregion

      #region sale_modify

//The sale_modify function will allow a user to modify the price recorded for a sale. This can be used before filing a monthly report if a line item mistake is noticed and needs to be corrected.

//Parameters:
//action	variable length text field
//transactionid	integer value

 // public string barcodeid { get; set; }	inventory identifier
//price	Decimal value representing the price
//paid before any applicable taxes.
//item_number	Optional, integer, should be provided

//if multiple line items of the same

//barcode were included in one sale. 0
//would represent the first item (in the
 
//order submitted to the system), 1 the next, etc.

//sale_time Optional, unix 32-bit integer timestamp of when the sale occurred. If not used, will default to current time. Otherwise, the time must not be in the future and, also, must not be in a locked tax period.

#endregion

    #region sale_refund

//The sale_refund function is nearly identical to sale_dispense except that it for items to selectively come back into inventory from a sale. This can take place at any time period after the original sale and will reflect on current sales as opposed to affecting previously reported data. You must specify both a transactionid and one or more identifiers. Retailers are not currently allowed by rule to destroy product, so if an open item is received it must be scheduled for transfer back to the processor for destruction.

//Parameters:
//action	variable length text field

//data	Array of 1 or more nodes containing
//inventory information
 // public string barcodeid { get; set; }	inventory identifier
//quantity	integer value, quantity to bring in.


//price	Negative decimal value representing
//the price paid before any applicable

//taxes.
//item_number	Optional, integer, should be provided
//if multiple line items of the same
//barcode were included in one sale. 0

//would represent the first item (in the
//order submitted to the system), 1 the
//next, etc.
//sale_time	Optional, unix 32-bit integer

//timestamp of when the sale occurred.
//If not used, will default to current
//time. Otherwise, the time must not

//be in the future and, also, must not

//be in a locked tax period.

#endregion

    #region tax_obligation_file

//After the system collects sales information over the course of a month, a licensee will be able to confirm their records with what is stored in the traceability system and track down any discrepancies.

//Parameters:
//action	variable length text field

//gross_sales	decimal value representing all sales
//excise_tax	decimal value representing amount

//believed to be owed
//month	integer value, 1 (Jan) – 12 (Dec)

//year	integer value
//verify	Boolean value. If set to true, the
//system will kick back an error instead
//of proceeding.

#endregion

    #region nonce_replay

//The system allows for a nonce value to be embedded in any request in which data is being saved. This is a user-defined value that should be unique for every request. It is the integrator’s responsibility, should they choose to utilize this functionality, to ensure this. Should the integrator re-use a token, and later request a replay of the results; the system will only return the last result for said token. For simplicity, a user may include a nonce value in non-transactional requests; they will be silently ignored. The system will only store data for which a transaction id is returned. Therefore, if the submitted data was non-transactional or produced an error, replay data would be unavailable and a request for said nonce would simply return a not found error.

//To embed a nonce value, simply encode said value into a standard request. For example, one might call the inventory_new function as:

//{
//"API": "4.0",

//"action": "inventory_new", "data": {

//"invtype": "12", "quantity": "50", "strain": "Blueberry"

//},

//"location": "12345",
//"nonce": "2ebf8a5981651d7403a40a3a4f710551afab"
//}


#endregion

     #region sync_check

//The sync_check function is the canonical function for synchronization. As indicated throughout this text; the system uses identification numbers for all transactional data received (via the transactionid). This function allows an integrator to determine if the summation of the transactions they have recorded what is currently stored within the traceability system. It can be used to either compare local value to remote values; or it can be used to simultaneously compare and download data that does not match. As these functions are comparing raw data tables the integrator should expect them returned as such.

//The data tables can be queried on their own via a specific call directly without doing a summation check or through this function. The direct calls will be detailed later in the chapter.

//The consistency check involves, at a minimum, providing a table. An integrator can also provide a start transaction (inclusive), an end transaction (inclusive), a sum value and whether or not only active data points are considered. More on this below.

//There are currently 15 tables which can be queried: vehicle, employee, plant_room, inventory_room, inventory, plant, plant_derivative, manifest, inventory_transfer, sale, tax_report, vendor, qa_lab, inventory_adjust and inventory_qa_sample.


#endregion

    #region inventory_qa_sample

//This contains basic quality assurance sample information as previously submitted. It is license specific and can be queried with all records or only active ones. As QA derived samples receive their own identifier; this list can be used to cross-reference said samples currently (or previously) in inventory.


//Parameters:

//action	variable length text field
//download	integer value of 0 or 1

//data	Array of 1 or more nodes containing
//synchronization information

//table	data table to be queried
//transaction_start	Optional, minimum transactionid
//(inclusive) to compare sums with.
//transaction_end	Optional, maximum transactionid

//(inclusive) to compare sums with.
//sum	Optional, summation of
//transactionid values the client side
//possesses.

//active	Optional, indicates only active
//records should be returned.


#endregion
    
      #region sync_vehicle

//The sync_vehicle function will allow a user to synchronize vehicle data as previously submitted.

//Parameters:
//action	variable length text field
//transaction_start	Optional, integer that indicates the

//first transactionid of interest
//transaction_end	Optional, integer that indicates the
//last transactionid of interest
//active	Optional, boolean value that

//indicates whether or not to only
//return non-deleted records

#endregion

    #region sync_employee

//The sync_employee function will allow a user to synchronize employee data as previously submitted.

//Parameters:
//action	variable length text field
//transaction_start	Optional, integer that indicates the
//first transactionid of interest

//transaction_end	Optional, integer that indicates the
//last transactionid of interest
//active	Optional, boolean value that
//indicates whether or not to only

//return non-deleted records

#endregion

    #region sync_plant_room

//The sync_plant_room function will allow a user to synchronize cultivation room data as previously submitted.

//Parameters:
//action	variable length text field
//transaction_start	Optional, integer that indicates the

//first transactionid of interest
//transaction_end	Optional, integer that indicates the

//last transactionid of interest
//active	Optional, boolean value that

//indicates whether or not to only
//return non-deleted records

#endregion

    #region sync_inventory_room

//The sync_inventory_room function will allow a user to synchronize inventory room data as previously submitted.

//Parameters:
//action	variable length text field
//transaction_start	Optional, integer that indicates the
//first transactionid of interest

//transaction_end	Optional, integer that indicates the
//last transactionid of interest
//active	Optional, boolean value that
//indicates whether or not to only

//return non-deleted records

#endregion

     #region sync_inventory

//The sync_inventory function will allow a user to synchronize inventory data as previously submitted.

//Parameters:

//action	variable length text field
//transaction_start	Optional, integer that indicates the
//first transactionid of interest
//transaction_end	Optional, integer that indicates the

//last transactionid of interest
//active	Optional, boolean value that
//indicates whether or not to only
//return non-deleted records

#endregion

    #region sync_plant

//The sync_plant function will allow a user to synchronize plant data as previously submitted.

//Parameters:
//action	variable length text field

//transaction_start	Optional, integer that indicates the
//first transactionid of interest
//transaction_end	Optional, integer that indicates the
//last transactionid of interest

//active	Optional, boolean value that
//indicates whether or not to only
//return non-deleted records

#endregion

      #region sync_plant_derivative

//The sync_plant_derivative function will allow a user to synchronize plant derivative data (wet and dry weights) as previously submitted.

//Parameters:
//action	variable length text field
//transaction_start	Optional, integer that indicates the

//first transactionid of interest
//transaction_end	Optional, integer that indicates the
//last transactionid of interest
//active	Optional, boolean value that

//indicates whether or not to only
//return non-deleted records

#endregion

    #region sync_manifest

//The sync_manifest function will allow a user to synchronize manifest data as previously submitted.

//Parameters:
//action	variable length text field

//transaction_start	Optional, integer that indicates the
//first transactionid of interest
//transaction_end	Optional, integer that indicates the
//last transactionid of interest

//active	Optional, boolean value that
//indicates whether or not to only
//return non-deleted records

#endregion

    #region sync_sale

//The sync_sale function will allow a user to synchronize sale data as previously submitted.

//Parameters:
//action	variable length text field

//transaction_start	Optional, integer that indicates the
//first transactionid of interest

//transaction_end	Optional, integer that indicates the
//last transactionid of interest
//active	Optional, boolean value that
//indicates whether or not to only

//return non-deleted records

#endregion

    #region sync_tax_report

//The sync_tax_report function will allow a user to synchronize tax obligation report data as previously submitted.

//Parameters:

//action	variable length text field
//transaction_start	Optional, integer that indicates the
//first transactionid of interest
//transaction_end	Optional, integer that indicates the

//last transactionid of interest
//active	Optional, boolean value that
//indicates whether or not to only
//return non-deleted records


#endregion
    
    #region sync_inventory_adjust

//The sync_inventory_adjust function will allow a user to synchronize inventory adjustment report data as previously submitted.

//Parameters:

//action	variable length text field
//transaction_start	Optional, integer that indicates the
//first transactionid of interest
//transaction_end	Optional, integer that indicates the

//last transactionid of interest

#endregion

    #region sync_inventory_qa_sample

//The sync_inventory_qa_sample function will allow a user to synchronize inventory quality assurance samples as previously submitted.

//Parameters:
//action	variable length text field

//transaction_start	Optional, integer that indicates the
//first transactionid of interest
//transaction_end	Optional, integer that indicates the
//last transactionid of interest

//active	Optional, boolean value that
//indicates whether or not to only
//return non-deleted records


#endregion

    #region sync_vendor

//The sync_vendor function will allow a user to synchronize official vendor data.
 
//Parameters:
//action	variable length text field

//transaction_start	Optional, integer that indicates the
//first transactionid of interest
//transaction_end	Optional, integer that indicates the
//last transactionid of interest

#endregion

     #region sync_qa_lab

//The sync_qa_lab function will allow a user to synchronize official QA labs.

//Parameters:
//action	variable length text field
//transaction_start	Optional, integer that indicates the
//first transactionid of interest

//transaction_end	Optional, integer that indicates the
//last transactionid of interest

#endregion

//_____________________________________Chapter 1: Authentication	7

//login	7

//user_add	11
//user_modify	15
   //user_remove	16

    //_____________________________________Chapter 2: Employees & Vehicles	17

//employee_add	17

//employee_modify	18

//employee_remove	19

//vehicle_add	19

//vehicle_modify	20

//vehicle_remove	21
    //_____________________________________Chapter 3: Rooms	22

//plant_room_add	22

//plant_room_modify	22

//plant_room_remove	23

//inventory_room_add	24

//inventory_room_modify	24

//inventory_room_remove	25
    ////_____________________________________Chapter 4: Plants	26

//plant_new	26

//plant_new_undo	27

//plant_move	28

//plant_destroy_schedule	29

//plant_destroy	30

//plant_harvest_schedule	30

//plant_harvest	31

//plant_waste_weigh	34

//plant_cure	35

//plant_convert_to_inventory	38

//plant_yield_modify	38

//plant_modify	40
    ///_____________________________________/Chapter 5: Inventory	42

//inventory_adjust	42
 


//inventory_destroy_schedule	43

//inventory_destroy	44

//inventory_move	45

//inventory_check	46

//inventory_new	47

//inventory_manifest	49

//inventory_manifest_lookup	50

//inventory_manifest_void	52

//inventory_transfer_lookup	52

//inventory_transfer_outbound	53

//inventory_transfer_outbound_modify	55

//inventory_transfer_outbound_void	55

//inventory_transfer_inbound	56

//inventory_create_lot	57

//inventory_split	59

//inventory_convert	60

//inventory_sample	62

//inventory_qa_sample	64

//inventory_qa_sample_void	65

//inventory_qa_sample_results	65

//QA Test Types	67

//Moisture Content Details	67

//Potency Analysis Details	67

//Foreign Matter Types	68

//Microbial and Fungal Counts (Colony Forming Units [CFU]/g)	68

//Residual Solvent Details	68

//inventory_qa_check	68

//inventory_modify	69
    ////_____________________________________Chapter 6: Sales	71

//sale_dispense	71

//sale_void	73

//sale_modify	73

//sale_refund	74
    ////_____________________________________Chapter 7: Finance	77

//tax_obligation_file	77
    ////_____________________________________Chapter 8: Synchronization	79

//nonce_replay	79

//sync_check	81

//Data Tables	81


//sync_vehicle	85
//sync_employee	87
//sync_plant_room	89
//sync_inventory_room	91
//sync_inventory	93
//sync_plant	96
//sync_plant_derivative	99
//sync_manifest	101
//sync_inventory_transfer	106
//sync_sale	108
//sync_tax_report	110
//sync_inventory_adjust	112
//sync_inventory_qa_sample	113
//sync_vendor	115
//sync_qa_lab	118
 

}
