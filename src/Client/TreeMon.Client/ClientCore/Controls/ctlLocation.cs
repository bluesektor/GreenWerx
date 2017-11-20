using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TreeMon.Managers.Membership;
using TreeMon.Managers.Store;
using TreeMon.Models.Geo;
using TreeMon.Models;
using ClientCore.Extensions;
using TreeMon.Utilites.Extensions;
using TreeMon.Models.App;
using TreeMon.Data.Logging.Models;

namespace ClientCore.Controls
{
    public partial class ctlLocation : UserControl
    {
        private string _connectionKey;

        private string _sessionKey;

        Location _selectedLocation;
        ctlNode _nodeControl;

        TreeNode _currentNode;
        string _currentAccountUUID;

        public ctlLocation(string dbConnectionKey, string sessionKey)
        {
            InitializeComponent();
            _connectionKey = dbConnectionKey;
            _sessionKey = sessionKey;



            _locationManager = new LocationManager(_connectionKey, _sessionKey);
        }

        public void Show(string accountUUID)
        {
            _currentAccountUUID = accountUUID;
            trvLocations.Nodes.Clear();
            
            LocationManager lm = new LocationManager(_connectionKey, _sessionKey);
            List<Location> locations = lm.GetAccountLocations(accountUUID);

            if (locations.Count == 0)
                return;

            List<INode> nodes = locations.ConvertAll(new Converter<Location, INode>(NodeEx.ObjectToNode));
            trvLocations.Load(nodes, null);
            trvLocations.SelectedNode = this.trvLocations.Nodes[0];
            _currentNode = trvLocations.SelectedNode;
            _currentNode.BackColor = Color.LightSteelBlue;

            _nodeControl = new ctlNode(_connectionKey, _sessionKey);

      
            pnlNodeLocation.Controls.Add(_nodeControl);
          
            _selectedLocation = locations[0];
            ShowDetails(locations[0]);
            LoadCountries();
            LoadStates(_selectedLocation.Country);
            LoadCities(_selectedLocation.State);
        }

        protected void ShowDetails(Location l)
        {
            if (l == null)
                return;

            _nodeControl.Show(l);


            //            //This is to link an account to a location.
            //            //Set this to the account.UUID
            //        public string AccountReference;

            //        //this replaces the Id field on the insert. the ParentId will reference this.
            //        public int RootId ;

            txtAbbr.Text = l.Abbr;
            txtCode.Text = l.Code;
            //todo txtCurrencyUUID.Text = l.CurrencyUUID;
            //todo txtLocationType.text l.LocationType;
            txtLatitude.Text = l.Latitude.ToString();
            txtLongitude.Text = l.Longitude.ToString();
            //todo txtType.Text = l.Type;
            //todo txtDescription.Text = l.Description;
            txtFirstName.Text = l.FirstName;
            txtLastName.Text = l.LastName;
            txtAddress1.Text = l.Address1;
            txtAddress2.Text = l.Address2;
            //txtCity.Text = l.City;
            //txtState.Text = l.State;
            //txtCountry.Text = l.Country;
            txtPostal.Text = l.Postal;
            chkIsBillingAddress.Checked = l.IsBillingAddress;
            chkDispensary.Checked = l.Dispensary;
            chkCultivation.Checked = l.Cultivation;
            chkManufacturing.Checked = l.Manufacturing;
            chkLab.Checked = l.Lab;
            chkProcessor.Checked = l.Processor;
            chkRetailer.Checked = l.Retailer;
            chkVirtual.Checked = l.Virtual;
            chkisDefault.Checked = l.isDefault;
            chkOnlineStore.Checked = l.OnlineStore;


        }

        private void ctlLocation_Load(object sender, EventArgs e)
        {

        }
        protected Location GetLocationDetail()
        {
            _selectedLocation = (Location)_nodeControl.Get();
            _selectedLocation.Abbr = txtAbbr.Text; ;
            _selectedLocation.Code = txtCode.Text;
            //_selectedLocation.CurrencyUUID = txtCurrencyUUID.Text;
            //_selectedLocation.LocationType = txtLocationType.text;
            //_selectedLocation.Latitude = txtLatitude.Text;
            //_selectedLocation.Longitude = txtLongitude.Text;
            //_selectedLocation.Longitude = txtLongitude.Text;
            //_selectedLocation.Type = txtType.Text;
            //_selectedLocation.Description = txtDescription.Text;
            _selectedLocation.FirstName = txtFirstName.Text;
            _selectedLocation.LastName = txtLastName.Text;
            _selectedLocation.Address1 = txtAddress1.Text;
            _selectedLocation.Address2 = txtAddress2.Text;
            //_selectedLocation.City = txtCity.Text;
            //_selectedLocation.State = txtState.Text;
            //_selectedLocation.Country = txtCountry.Text;
            _selectedLocation.Postal = txtPostal.Text;
            _selectedLocation.IsBillingAddress = chkIsBillingAddress.Checked;
            _selectedLocation.Dispensary = chkDispensary.Checked;
            _selectedLocation.Cultivation = chkCultivation.Checked;
            _selectedLocation.Manufacturing = chkManufacturing.Checked;
            _selectedLocation.Lab = chkLab.Checked;
            _selectedLocation.Processor = chkProcessor.Checked;
            _selectedLocation.Retailer = chkRetailer.Checked;
            _selectedLocation.Virtual = chkVirtual.Checked;
            _selectedLocation.isDefault = chkisDefault.Checked;
            _selectedLocation.OnlineStore = chkOnlineStore.Checked;
            return _selectedLocation;
        }

        private void trvLocations_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (trvLocations.SelectedNode == null || trvLocations.SelectedNode.Tag == null)
                return;

            Location l = (Location)this.trvLocations.SelectedNode.Tag;

            ShowDetails(l);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            lblError.Text = "";

            if (this.trvLocations.SelectedNode == null )
                _selectedLocation = (Location)this.trvLocations.SelectedNode.Tag;

            if (_selectedLocation == null)
                return;

            _selectedLocation = GetLocationDetail();

            LocationManager um = new LocationManager(this._connectionKey, this._sessionKey);
            ServiceResult res;

            if (string.IsNullOrWhiteSpace(_selectedLocation.UUID))
                res = um.Insert(_selectedLocation);
            else
                res = um.Update(_selectedLocation);

            if (res.Code != 200)
                lblError.Text = res.Message;

        }

        LocationManager _locationManager;

        private void LoadCountries()
        {

            List<Location> countries = _locationManager.GetCountries(_currentAccountUUID);
            this.cboCountry.DisplayMember = "Name";
            this.cboCountry.Items.AddRange(countries.ToArray());
           
         
            if (_selectedLocation == null || string.IsNullOrWhiteSpace(_selectedLocation.Country))
                return;

           cboCountry.SelectedIndex = countries.IndexOf(countries.Single(w => w.UUID == _selectedLocation.Country));
        }

        private void LoadStates(string countryUUID)
        {
            List<Location> states = _locationManager.GetStates(_currentAccountUUID, countryUUID);
            this.cboState.DisplayMember = "Name";
            this.cboState.Items.AddRange(states.ToArray());


            if (_selectedLocation == null || string.IsNullOrWhiteSpace(_selectedLocation.State))
                return;

            cboState.SelectedIndex = states.IndexOf(states.Single(w => w.UUID == _selectedLocation.State));
        }

        private void LoadCities(string stateUUID)
        {
            List<Location> cities = _locationManager.GetCities(_currentAccountUUID,stateUUID);
            this.cboCity.DisplayMember = "Name";
            this.cboCity.Items.AddRange(cities.ToArray());


            if (_selectedLocation == null || string.IsNullOrWhiteSpace(_selectedLocation.City))
                return;

            cboCity.SelectedIndex = cities.IndexOf(cities.Single(w => w.UUID == _selectedLocation.City));
        }

        private void cboCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedLocation.Country = ((Location)cboCountry.SelectedItem).UUID;
            _selectedLocation.State = "";
            _selectedLocation.City = "";
            LoadStates(_selectedLocation.Country);
        }

        private void cboState_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedLocation.State =    ((Location)cboState.SelectedItem).UUID;
            _selectedLocation.City = "";
           LoadCities(_selectedLocation.State);
        }

        private void cboCity_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedLocation.City = ((Location)cboCity.SelectedItem).UUID;
        }
    }
}
