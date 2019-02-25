using PluginInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using TreeMon.Managers;
using TreeMon.Models.App;
using static System.Windows.Forms.ListView;
using static System.Windows.Forms.ListViewItem;

namespace Settings
{
    public partial class ctlSettings : UserControl, IPlugin
    {
        public ctlSettings()
        {
            InitializeComponent();


        }
        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            if (lstPlugins.SelectedIndices.Count == 0)
                return;

            int index = lstPlugins.SelectedIndices[0];

            if (index == 0)
                return;

            ListViewItem selectedItem = lstPlugins.Items[index];

            ListViewItem itemAbove = lstPlugins.Items[index - 1];

            if (itemAbove == null)
                return;

            lstPlugins.Items.Remove(selectedItem);
            lstPlugins.Items.Insert(itemAbove.Index, selectedItem);
        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            if (lstPlugins.SelectedIndices.Count == 0)
                return;

            int index = lstPlugins.SelectedIndices[0];

            if (index >= lstPlugins.Items.Count - 1)
                return;

            ListViewItem selectedItem = lstPlugins.Items[index];

            ListViewItem itemBelow = lstPlugins.Items[index + 1];

            if (itemBelow == null)
                return;

            lstPlugins.Items.Remove(selectedItem);
            lstPlugins.Items.Insert(itemBelow.Index + 1, selectedItem);
        }

        private void btnSaveAppearance_Click(object sender, EventArgs e)
        {
            IEnumerable<ListViewItem> lv = lstPlugins.Items.Cast<ListViewItem>();
            string plugins = lv.Select(s => s.Text).Aggregate((current, next) => current + "," + next);

            ClientCore.Application.SaveConfigSetting(_session.UserName + "PluginOrder", plugins);
        }

        #region Plugin interface properties

        
        private string myPluginAuthor = "bluesektor@hotmail.com";
        private string myPluginDescription = "descriptions";
        private string myPluginVersion = "1.0.0";

        private string myPluginShortName = "Settings";

        private UserSession _session;
        private AppInfo _appSettings;

        void PluginInterface.IPlugin.Dispose()
        {
            //add cleanup here.
        }
        

        public string Description
        { get { return myPluginDescription; } }

        public string Author
        { get { return myPluginAuthor; } }

        public IPluginHost Host { get; set; }

        public void Initialize(UserSession session, AppInfo appSettings)
        {
            _session = session;
            _appSettings = appSettings;
            string pluginDirectory = Application.StartupPath + @"\Plugins";

            AppManager am = new AppManager(_appSettings.ActiveDbConnectionKey, "FORMS", _session.AuthToken);
            string pluginOrder = am.GetSetting(_session.UserName + "PluginOrder", false)?.Value;
            List<string> plugins = new List<string>();

            if (!string.IsNullOrEmpty(pluginOrder))
            {
                plugins.AddRange(pluginOrder.Split(',').ToList());
            }
            else if (Directory.Exists(pluginDirectory))
            {
                PluginServices svcPlugins = new PluginServices();
                //Call the find plugins routine, to search in our Plugins Folder
                List<string> AvailablePlugins = svcPlugins.FindPlugins(pluginDirectory);
                plugins.AddRange(AvailablePlugins);
            }

            foreach (string plugin in plugins)
            {
                lstPlugins.Items.Add(new ListViewItem(plugin));
            }
            lstPlugins.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);


        }

        protected void Run()
        {
            //not mandatory, but good for a standard interface to main.
        }

        public void ResizeControl()
        {
            //adjust windows here
        }

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
