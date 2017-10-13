using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTrackTHC.V4
{
    class User
    {
        public string UserName { get; set; }
        public string Password { get; set; } 
        public string License_Number { get; set; }

        public long Id { get; set; }

        //public string display_name { get; set; }

        //public string user_email { get; set; }
        //public string user_url { get; set; }
        //public System.DateTime user_registered { get; set; }
        //public string user_activation_key { get; set; }
        //public int user_status { get; set; }
        

        //roles Admin => all permissions
        //      Store Manager
        //      Grow Room Manager

        #region permissions
        
        //permission_add
        //permission_add_modify
        //permission_remove
//inventory_convert
//sale_dispense
//sale_modify
//        sale_void
//sale_refund
//justauthenticate
//employee_add
//employee_modify
//employee_remove
//vehicle_add
//vehicle_modify
//vehicle_remove
//plant_room_add
//plant_room_modify
//plant_room_remove
//inventory_room_add
//inventory_room_modify
//inventory_room_remove
//plant_destroy_schedule
//plant_destroy_schedule_undo
//plant_destroy
//plant_harvest_schedule
//plant_harvest_schedule_undo
//plant_harvest
//plant_new 
//plant_new_undo
//        plant_convert_to_inventory
//plant_cure
//plant_yield_modify
//plant_waste_weigh
//inventory_new
//inventory_manifest_lookup
//inventory_transfer_inbound
//inventory_transfer_lookup
//inventory_transfer_outbound
//inventory_transfer_outbound_modify
//inventory_transfer_outbound_void
//plant_move
//plant_modify
//inventory_adjust
//inventory_sample
//inventory_qa_check
//inventory_qa_sample
//inventory_qa_sample_void
//inventory_qa_sample_results
//inventory_manifest
//inventory_manifest_void
//inventory_create_lot
//inventory_split
//        user_add
//user_modify
//user_remove
//inventory_move
//inventory_destroy_schedule
//inventory_destroy_schedule_undo
//inventory_destroy
//tax_obligation_file
//nonce_replay
//sync_vehicle
//sync_employee
//sync_plant_room
//sync_inventory_room
//sync_inventory
//sync_plant
//sync_plant_derivative
//sync_manifest
//sync_inventory_transfer
//sync_sale
//sync_tax_report
//sync_vendor
//sync_qa_lab
//sync_check 
//sync_inventory_adjust
//sync_inventory_qa_sample
        #endregion
    }
}
