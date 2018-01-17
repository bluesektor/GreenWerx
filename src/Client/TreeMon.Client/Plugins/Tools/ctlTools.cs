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
using TreeMon.Models.App;
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
        private string myPluginAuthor = "TreeMon.org";
        private string myPluginDescription = "Opensource tools.";
        private string myPluginVersion = "1.0.0";
        private string myPluginShortName = "Tools";

        private UserSession _session;
        private AppInfo _appSettings;

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

        public void Initialize(UserSession session, AppInfo appSettings)
        {
            _session = session;
            _appSettings = appSettings;
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
            // AppManager am = new AppManager(Globals.DBConnectionKey, "web", Request.Headers?.Authorization?.Parameter);
            //  am.DataTypes();
            //MainInterface.
        }

        private void ctlTools_Load(object sender, EventArgs e)
        {

        }
    }
}
