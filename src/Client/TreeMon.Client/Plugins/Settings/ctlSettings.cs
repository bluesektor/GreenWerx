using PluginInterface;
using System;
using System.Windows.Forms;

namespace Settings
{
    public partial class ctlSettings :  UserControl, IPlugin
    {
        private void button1_Click(object sender, EventArgs e)
        {
          
        }

        public ctlSettings()
        {
            InitializeComponent();
        }

        #region Plugin interface properties

        private IPluginHost myPluginHost = null;
      //  private string myPluginName = "Name";
        private string myPluginAuthor = "bluesektor@hotmail.com";
        private string myPluginDescription = "descriptions";
        private string myPluginVersion = "1.0.0";

        private string myPluginShortName = "Settings";


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

        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void btnImportDataset_Click(object sender, EventArgs e)
        {
            ErrorLog = "";
             OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "G:\\Dev\\Projects\\_OpenSource\\Web\\TreeMon.Web\\TreeMon.Web\\App_Data";
            openFileDialog1.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            openFileDialog1.Multiselect = true;
            foreach (String file in openFileDialog1.FileNames)
            { MessageBox.Show(file); }


            ErrorLog = "";
        }

        public string ErrorLog { get; set; }

   
    }
}
