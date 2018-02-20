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
using System.Diagnostics;
using ClientCore.Models;
using AutoMapper;
using TreeMon.Managers;

namespace ClientCore.Controls
{
    public partial class ctlLocation : UserControl, IUserControl
    {
        private string _connectionKey;

        private string _sessionKey;

        Location _selectedLocation;
        ctlNode _nodeControl;
        ctlNodeTree _treeLocations;
    
        string _currentAccountUUID;

        public ctlLocation(string dbConnectionKey, string sessionKey)
        {
            InitializeComponent();
            _connectionKey = dbConnectionKey;
            _sessionKey = sessionKey;

            _treeLocations = new ctlNodeTree(_connectionKey, _sessionKey, this);
            _treeLocations.TreeNodesAferSelect += _treeLocations_TreeNodesAferSelect;
            pnlNodeList.Controls.Add(_treeLocations);

            _locationManager = new LocationManager(_connectionKey, _sessionKey);
        }

        public void Show(string accountUUID)
        {
            _currentAccountUUID = accountUUID;

            LocationManager lm = new LocationManager(_connectionKey, _sessionKey);
            List<Location> locations = lm.GetAccountLocations(accountUUID);

            List<INode> nodes = new List<INode>();
            if (locations.Count > 0)
            {
                nodes = locations.ConvertAll(new Converter<Location, INode>(NodeEx.ObjectToNode));
            }


            _nodeControl = new ctlNode(_connectionKey, _sessionKey);
            pnlNodeLocation.Controls.Add(_nodeControl);

            _treeLocations.Init(nodes, "");

            _treeLocations.Dock = DockStyle.Fill;
            _nodeControl.Dock = DockStyle.Fill;

            if (locations.Count == 0)
                return;

            _selectedLocation = locations[0];
            ShowDetails(locations[0]);
            LoadCountries();
            LoadStates(_selectedLocation.Country);
            LoadCities(_selectedLocation.State);
        }

        private void _treeLocations_TreeNodesAferSelect(object sender, System.EventArgs e)
        {
            if (e == null)
                return;
            TreeViewEventArgs arg = (TreeViewEventArgs)e;

            if (((INode)_treeLocations.SelectedTreeNode.Tag).UUID != ((INode)arg.Node.Tag).UUID)
            {
                _treeLocations.SelectedTreeNode.BackColor = Color.Transparent;
                _treeLocations.SelectedTreeNode.BackColor = Color.AliceBlue;
            }

            ShowDetails((Location)arg.Node.Tag);
        }


        protected void ShowDetails(Location l)
        {
            if (l == null)
                return;

            _nodeControl.Show(l);


            ////            //This is to link an account to a location.
            ////            //Set this to the account.UUID
            ////        public string AccountReference;

            ////        //this replaces the Id field on the insert. the ParentId will reference this.
            ////        public int RootId ;

            txtAbbr.Text = l.Abbr;
            txtCode.Text = l.Code;
            ////todo txtCurrencyUUID.Text = l.CurrencyUUID;
            ////todo txtLocationType.text l.LocationType;
            txtLatitude.Text = l.Latitude.ToString();
            txtLongitude.Text = l.Longitude.ToString();
            ////todo txtType.Text = l.Type;
            ////todo txtDescription.Text = l.Description;
            txtFirstName.Text = l.FirstName;
            txtLastName.Text = l.LastName;
            txtAddress1.Text = l.Address1;
            txtAddress2.Text = l.Address2;
            ////txtCity.Text = l.City;
            ////txtState.Text = l.State;
            ////txtCountry.Text = l.Country;
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

        protected Location GetLocationDetail()
        {
            _selectedLocation = (Location)_nodeControl.Get();
            _selectedLocation.Abbr = txtAbbr.Text; ;
            _selectedLocation.Code = txtCode.Text;
            ////_selectedLocation.CurrencyUUID = txtCurrencyUUID.Text;
            ////_selectedLocation.LocationType = txtLocationType.text;
            ////_selectedLocation.Latitude = txtLatitude.Text;
            ////_selectedLocation.Longitude = txtLongitude.Text;
            ////_selectedLocation.Longitude = txtLongitude.Text;
            ////_selectedLocation.Type = txtType.Text;
            ////_selectedLocation.Description = txtDescription.Text;
            _selectedLocation.FirstName = txtFirstName.Text;
            _selectedLocation.LastName = txtLastName.Text;
            _selectedLocation.Address1 = txtAddress1.Text;
            _selectedLocation.Address2 = txtAddress2.Text;
            ////_selectedLocation.City = txtCity.Text;
            ////_selectedLocation.State = txtState.Text;
            ////_selectedLocation.Country = txtCountry.Text;
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            lblError.Text = "";

            if (_selectedLocation == null)
                return;

            _selectedLocation = GetLocationDetail();

            Save(_selectedLocation);
        }

     
        LocationManager _locationManager;

        List<Location> _countries = new List<TreeMon.Models.Geo.Location>();

        private void LoadCountries()
        {
            if (_countries.Count == 0)
                _countries = _locationManager.GetCountries(_currentAccountUUID);

            this.cboCountry.DisplayMember = "Name";
            this.cboCountry.Items.AddRange(_countries.ToArray());
           
         
            if (_selectedLocation == null || string.IsNullOrWhiteSpace(_selectedLocation.Country))
                return;

           cboCountry.SelectedIndex = _countries.IndexOf(_countries.Single(w => w.UUID == _selectedLocation.Country));
        }

        List<Location> _states = new List<TreeMon.Models.Geo.Location>();

        private void LoadStates(string countryUUID)
        {
            if (_states.Count == 0)
                _states = _locationManager.GetStates(_currentAccountUUID, countryUUID);

            this.cboState.DisplayMember = "Name";
            this.cboState.Items.AddRange(_states.ToArray());


            if (_selectedLocation == null || string.IsNullOrWhiteSpace(_selectedLocation.State))
                return;

            cboState.SelectedIndex = _states.IndexOf(_states.Single(w => w.UUID == _selectedLocation.State));
        }

        List<Location> _cities = new List<TreeMon.Models.Geo.Location>();

        private void LoadCities(string stateUUID)
        {
            if(_cities.Count == 0)
                _cities = _locationManager.GetCities(_currentAccountUUID,stateUUID);

            this.cboCity.DisplayMember = "Name";
            this.cboCity.Items.AddRange(_cities.ToArray());


            if (_selectedLocation == null || string.IsNullOrWhiteSpace(_selectedLocation.City))
                return;

            cboCity.SelectedIndex = _cities.IndexOf(_cities.Single(w => w.UUID == _selectedLocation.City));
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

        public void Save(INode n)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<INode, Location>();
            });
            if (string.IsNullOrWhiteSpace(n.AccountUUID))
            {
                n.AccountUUID = new SessionManager(_connectionKey).GetAccountUUID(_sessionKey);
            }

            IMapper mapper = config.CreateMapper();
            Location location = mapper.Map<INode, Location>(n);

            LocationManager um = new LocationManager(this._connectionKey, this._sessionKey);
            ServiceResult res;

            if (string.IsNullOrWhiteSpace(location.UUID))
            {
                res = um.Insert(location);
               if(res.Code == 200)
                    _treeLocations.AddToTree(n);
            }
            else
            {
                res = um.Update(location);
                if (res.Code == 200)
                {
                    //_treeLocations.Update(n);//todo finish this function. remove the code below when done.
                    LocationManager lm = new LocationManager(_connectionKey, _sessionKey);
                    List<Location> locations = lm.GetAccountLocations(this._currentAccountUUID);

                    List<INode> nodes = new List<INode>();
                    if (locations.Count > 0)
                    {
                        nodes = locations.ConvertAll(new Converter<Location, INode>(NodeEx.ObjectToNode));
                    }
                    _treeLocations.Init(nodes, "", null);
                }
            }

            if (res.Code != 200)
                lblError.Text = res.Message;
        }

        public void Delete(INode n)
        {
            if (n == null)
                return;

            LocationManager lm = new LocationManager(this._connectionKey, this._sessionKey);

            lm.Delete(n);
            _treeLocations.DeleteFromTree(n);
            List<Location> subLocations = lm.GetLocations(n.AccountUUID).Where(x => x.UUParentID == n.UUID).ToList();

            foreach (Location l in subLocations)
            {
                lm.Delete(l);
                _treeLocations.DeleteFromTree(l);
                Delete(l);
            }
        }

        private void pnlNodeList_ClientSizeChanged(object sender, EventArgs e)
        {
            if (_treeLocations != null) {
                _treeLocations.Dock = DockStyle.Fill;
            }


            if(_nodeControl != null)
                _nodeControl.Dock = DockStyle.Fill;
        }

     
    }
}
