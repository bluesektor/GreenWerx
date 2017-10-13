using PluginInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TreeMon.Data;
using TreeMon.Data.Logging;
using TreeMon.Data.Logging.Models;
using TreeMon.Managers.Membership;
using TreeMon.Managers.Plant;
using TreeMon.Managers.Store;
using TreeMon.Models.Membership;
using TreeMon.Models.Plant;
using TreeMon.Models.Store;

namespace Tools
{
    public partial class ctlTools : UserControl, IPlugin
    {
        #region Plugin interface properties

        private IPluginHost myPluginHost = null;
        //  private string myPluginName = "Name";
        private string myPluginAuthor = "bluesektor.com";
        private string myPluginDescription = "descriptions";
        private string myPluginVersion = "1.0.0";

        private string myPluginShortName = "Tools";
        #region google geocode xml example
        string _googleGeoXml = @"<GeocodeResponse>
  <status>OK</status>
  <result>
    <type>street_address</type>
    <formatted_address>11259 W Lily McKinley Dr, Surprise, AZ 85378, USA</formatted_address>
    <address_component>
      <long_name>11259</long_name>
      <short_name>11259</short_name>
      <type>street_number</type>
    </address_component>
    <address_component>
      <long_name>West Lily McKinley Drive</long_name>
      <short_name>W Lily McKinley Dr</short_name>
      <type>route</type>
    </address_component>
    <address_component>
      <long_name>Canyon Ridge West</long_name>
      <short_name>Canyon Ridge West</short_name>
      <type>neighborhood</type>
      <type>political</type>
    </address_component>
    <address_component>
      <long_name>Surprise</long_name>
      <short_name>Surprise</short_name>
      <type>locality</type>
      <type>political</type>
    </address_component>
    <address_component>
      <long_name>Maricopa County</long_name>
      <short_name>Maricopa County</short_name>
      <type>administrative_area_level_2</type>
      <type>political</type>
    </address_component>
    <address_component>
      <long_name>Arizona</long_name>
      <short_name>AZ</short_name>
      <type>administrative_area_level_1</type>
      <type>political</type>
    </address_component>
    <address_component>
      <long_name>United States</long_name>
      <short_name>US</short_name>
      <type>country</type>
      <type>political</type>
    </address_component>
    <address_component>
      <long_name>85378</long_name>
      <short_name>85378</short_name>
      <type>postal_code</type>
    </address_component>
    <address_component>
      <long_name>6902</long_name>
      <short_name>6902</short_name>
      <type>postal_code_suffix</type>
    </address_component>
    <geometry>
      <location>
        <lat>33.6455000</lat>
        <lng>-112.3019530</lng>
      </location>
      <location_type>ROOFTOP</location_type>
      <viewport>
        <southwest>
          <lat>33.6441510</lat>
          <lng>-112.3033020</lng>
        </southwest>
        <northeast>
          <lat>33.6468490</lat>
          <lng>-112.3006040</lng>
        </northeast>
      </viewport>
    </geometry>
    <partial_match>true</partial_match>
    <place_id>ChIJuYrbdjpDK4cRel0GZ6uZGog</place_id>
  </result>
</GeocodeResponse>";
        #endregion

        #region google geocode json example
//        string _googleGeoJson = "{   \"results\" : [{\"address_components\" : [
//            {
//               \"long_name\" : \"1600\",
//               \"short_name\" : \"1600\",
//               \"types\" : [ \"street_number\" ]
//    },
//            {
//               \"long_name\" : \"Amphitheatre Pkwy\",
//               \"short_name\" : \"Amphitheatre Pkwy\",
//               \"types\" : [ \"route\" ]
//},
//            {
//               \"long_name\" : \"Mountain View\",
//               \"short_name\" : \"Mountain View\",
//               \"types\" : [ \"locality\", \"political\" ]
//            },
//            {
//               \"long_name\" : \"Santa Clara County\",
//               \"short_name\" : \"Santa Clara County\",
//               \"types\" : [ \"administrative_area_level_2\", \"political\" ]
//            },
//            {
//               \"long_name\" : \"California\",
//               \"short_name\" : \"CA\",
//               \"types\" : [ \"administrative_area_level_1\", \"political\" ]
//            },
//            {
//               \"long_name\" : \"United States\",
//               \"short_name\" : \"US\",
//               \"types\" : [ \"country\", \"political\" ]
//            },
//            {
//               \"long_name\" : \"94043\",
//               \"short_name\" : \"94043\",
//               \"types\" : [ \"postal_code\" ]
//            }
//         ],
//         \"formatted_address\" : \"1600 Amphitheatre Parkway, Mountain View, CA 94043, USA\",
//         \"geometry\" : {
//            \"location\" : {
//               \"lat\" : 37.4224764,
//               \"lng\" : -122.0842499
//            },
//            \"location_type\" : \"ROOFTOP\",
//            \"viewport\" : {
//               \"northeast\" : {
//                  \"lat\" : 37.4238253802915,
//                  \"lng\" : -122.0829009197085
//               },
//               \"southwest\" : {
//                  \"lat\" : 37.4211274197085,
//                  \"lng\" : -122.0855988802915
//               }
//            }
//         },
//         \"place_id\" : \"ChIJ2eUgeAK6j4ARbn5u_wAGqWA\",
//         \"types\" : [ \"street_address\" ]
//      }
//   ],
//   \"status\" : \"OK\"
//}";
        #endregion

        void PluginInterface.IPlugin.Dispose()
        { }

        public string Description
        { get { return myPluginDescription; } }

        public string Author
        { get { return myPluginAuthor; } }

        public IPluginHost Host
        {
            get { return myPluginHost; }
            set { myPluginHost = value; }
        }

        public void Initialize(string[] args)
        {   //if (args == null)return; //uncomment if args are mandetory
        }

        protected void Run() //not mandatory, but good for a standard interface to main.
        { }

        public void ResizeControl()
        { }

        public UserControl MainInterface
        { get { return this; } }

        public string Version
        { get { return myPluginVersion; } }

        public string ShortName
        {
            get
            {
                return myPluginShortName;
            }
        }

      

        #endregion
        public ctlTools()
        {
            InitializeComponent();
        }


        public delegate void SetStatusBarBoundariedDelegate(int min, int max);

        public void SetProgressBarBoundaries(int min, int max)
        {

            if (prgStatus.InvokeRequired)
            {
                Invoke(new SetStatusBarBoundariedDelegate(SetProgressBarBoundaries), new object[] { min, max });
            }
            else
            {
                prgStatus.Minimum = min;
                prgStatus.Maximum = max;

                Application.DoEvents();
            }
        }

        public delegate void UpdateStatusDelegate(int index, string msg);
        public void SetProgressBar(int nCompleted, string msg)
        {
            if (prgStatus.InvokeRequired)
            {
                Invoke(new UpdateStatusDelegate(SetProgressBar), new object[] { nCompleted, msg });
                return; //if you don't return, it'll cause a double entry (i.e. execute UpdateStatusMessage(..) twice.
            }
            else
            {
                if (nCompleted > prgStatus.Minimum && nCompleted < prgStatus.Maximum)
                    prgStatus.Value = nCompleted;

                Application.DoEvents();
            }
            UpdateStatusMessage(msg);
        }

        public delegate void UpdateStatusMessageDelegate(string msg);

        public void UpdateStatusMessage(string msg)
        {
            if (this.txtStatus.InvokeRequired)
                Invoke(new UpdateStatusMessageDelegate(UpdateStatusMessage), new object[] { msg });
            else
            {
                if (msg.Length > 0)
                    txtStatus.Text += msg + "\r\n";

                Application.DoEvents();
            }

        }

        public string ErrorLog { get; set; }

        private void btnProcessStrains_Click(object sender, EventArgs e)
        {
            ErrorLog = "";
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "G:\\Dev\\Projects\\_OpenSource\\Web\\TreeMon.Web\\TreeMon.Web\\App_Data";
            openFileDialog1.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            //this.openFileDialog1.Multiselect = true;
            //foreach (String file in openFileDialog1.FileNames)
            //{   MessageBox.Show(file);}
            
            string[] tokens = System.IO.File.ReadAllLines(openFileDialog1.FileName);

        
            ErrorLog = "";
        }

        private void btnGetCooridnates_Click(object sender, EventArgs e)
        {

        }
    }
}
