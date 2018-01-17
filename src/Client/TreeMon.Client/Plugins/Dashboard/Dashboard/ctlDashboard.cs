using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PluginInterface;
using TreeMon.Models.App;

namespace Dashboard
{
    public partial class ctlDashboard: UserControl, IPlugin
    {
        public ctlDashboard()
        {
            InitializeComponent();
        }
        #region Plugin interface properties

        private IPluginHost myPluginHost = null;
        //  private string myPluginName = "Name";
        private string myPluginAuthor = "TreeMon.org";
        private string myPluginDescription = "Opensource dashboard.";
       
        private string myPluginVersion = "1.0.0";
        private string myPluginShortName = "Dashboard";

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

    }
}
