using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using TreeMon.Models.App;

namespace PluginInterface
{
    public class PluginServices : IPluginHost   //<--- Notice how it inherits IPluginHost interface!
    {
        /// <summary>
        /// Constructor of the Class
        /// </summary>
        public PluginServices()
        {
        }

        private AvailablePlugins colAvailablePlugins = new AvailablePlugins();

        /// <summary>
        /// A Collection of all Plugins Found and Loaded by the FindPlugins() Method
        /// </summary>
        public AvailablePlugins AvailablePlugins
        {
            get { return colAvailablePlugins; }
            set { colAvailablePlugins = value; }
        }

        /// <summary>
        /// Searches the Application's Startup Directory for Plugins
        /// </summary>
        //public void FindPlugins()
        //{
        //    //FindPlugins(AppDomain.CurrentDomain.BaseDirectory);
        //}

        

        /// <summary>
        /// Searches the passed Path for Plugins
        /// </summary>
        /// <param name="Path">Directory to search for Plugins in</param>
        public List<string> FindPlugins(string Path)
        {
            List<string> plugins = new List<string>();
            try
            {
                //Go through all the files in the plugin directory
                foreach (string fileOn in Directory.GetFiles(Path))
                {
                    FileInfo file = new FileInfo(fileOn);

                    //Preliminary check, must be .dll
                    if (!file.Extension.Equals(".dll"))
                        continue;

                    Assembly pluginAssembly = Assembly.LoadFrom(Path + "\\" +file.Name);

                    //Next we'll loop through all the Types found in the assembly
                    foreach (Type pluginType in pluginAssembly.GetTypes())
                    {
                        if (!pluginType.IsPublic && pluginType.IsAbstract)
                            continue;

                        //Gets a type object of the interface we need the plugins to match
                        Type typeInterface = pluginType.GetInterface("PluginInterface.IPlugin", true);

                        if (typeInterface == null)
                            continue;

                        AvailablePlugin newPlugin = new AvailablePlugin();
                        newPlugin.AssemblyPath = Path;
                        newPlugin.Instance = (IPlugin)Activator.CreateInstance(pluginAssembly.GetType(pluginType.ToString()));

                        
                        //Set the Plugin's host to this class which inherited IPluginHost
                        //newPlugin.Instance.Host = this;

                        plugins.Add(newPlugin.Instance.ShortName);
                        //cleanup a bit
                        newPlugin = null;
                        typeInterface = null; //Mr. Clean			
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
            }
            return plugins;
        }

        /// <summary>
        /// Searches the passed Path for Plugins
        /// </summary>
        /// <param name="Path">Directory to search for Plugins in</param>
        public string LoadPlugins(string Path, UserSession session, AppInfo settings)
        {
            try
            {
                //First empty the collection, we're reloading them all
                colAvailablePlugins.Clear();

                //Go through all the files in the plugin directory
                foreach (string fileOn in Directory.GetFiles(Path))
                {
                    FileInfo file = new FileInfo(fileOn);

                    //Preliminary check, must be .dll
                    if (file.Extension.Equals(".dll"))
                    {
                        //Add the 'plugin'
                        this.AddPlugin(fileOn, session,settings);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
                return ex.Message;
            }
            return "Plugins loaded.";
        }

        //This is similiar to FindPlugins(..), except it loads one/specified plugin.
        //
        public string LoadPlugin(string Path, UserSession session, AppInfo settings)
        {
            try
            {
                //First empty the collection, we're reloading them all
                colAvailablePlugins.Clear();

                FileInfo file = new FileInfo(Path);

                //Preliminary check, must be .dll
                if (file.Extension.Equals(".dll"))
                {
                    this.AddPlugin(Path, session, settings);                        //Add the 'plugin'
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "Plugin loaded.";
        }


        // Unloads and Closes all AvailablePlugins
        //	
        public void ClosePlugins()
        {
            foreach (AvailablePlugin pluginOn in colAvailablePlugins)
            {
                //Close all plugin instances
                //We call the plugins Dispose sub first incase it has to do 
                //Its own cleanup stuff
                pluginOn.Instance.Dispose();

                //After we give the plugin a chance to tidy up, get rid of it
                pluginOn.Instance = null;
            }

            //Finally, clear our collection of available plugins
            colAvailablePlugins.Clear();
        }

        private void AddPlugin(string FileName, UserSession session, AppInfo settings)
        {
            //Create a new assembly from the plugin file we're adding..
            Assembly pluginAssembly = Assembly.LoadFrom(FileName);

            //Next we'll loop through all the Types found in the assembly
            foreach (Type pluginType in pluginAssembly.GetTypes())
            {
                if (pluginType.IsPublic) //Only look at public types
                {
                    if (!pluginType.IsAbstract)  //Only look at non-abstract types
                    {
                        //Gets a type object of the interface we need the plugins to match
                        Type typeInterface = pluginType.GetInterface("PluginInterface.IPlugin", true);

                        //Make sure the interface we want to use actually exists
                        if (typeInterface != null)
                        {
                            //Create a new available plugin since the type implements the IPlugin interface
                            AvailablePlugin newPlugin = new AvailablePlugin();

                            //Set the filename where we found it
                            newPlugin.AssemblyPath = FileName;

                            //Create a new instance and store the instance in the collection for later use
                            //We could change this later on to not load an instance.. we have 2 options
                            //1- Make one instance, and use it whenever we need it.. it's always there
                            //2- Don't make an instance, and instead make an instance whenever we use it, then close it
                            //For now we'll just make an instance of all the plugins
                            newPlugin.Instance = (IPlugin)Activator.CreateInstance(pluginAssembly.GetType(pluginType.ToString()));

                            //Set the Plugin's host to this class which inherited IPluginHost
                            newPlugin.Instance.Host = this;
                            try
                            {
                                //Call the initialization sub of the plugin
                                newPlugin.Instance.Initialize(session, settings);
                            }
                            catch { }


                            //Add the new plugin to our collection here
                            this.colAvailablePlugins.Add(newPlugin);

                            //cleanup a bit
                            newPlugin = null;
                        }

                        typeInterface = null; //Mr. Clean			
                    }
                }
            }

            pluginAssembly = null; //more cleanup
        }

        /// <summary>
        /// Displays a feedback dialog from the plugin
        /// </summary>
        /// <param name="Feedback">String message for feedback</param>
        /// <param name="Plugin">The plugin that called the feedback</param>
        public void Feedback(string Feedback, IPlugin Plugin)
        {
            //This sub makes a new feedback form and fills it out
            //With the appropriate information
            //This method can be called from the actual plugin with its Host Property

            System.Windows.Forms.Form newForm = null;
            frmFeedback newFeedbackForm = new frmFeedback();

            //Here we set the frmFeedback's properties that i made custom
            newFeedbackForm.PluginAuthor = "Programmer: " + Plugin.Author;
            newFeedbackForm.PluginDesc = Plugin.Description;
            newFeedbackForm.PluginName = Plugin.Name;
            newFeedbackForm.PluginVersion = Plugin.Version;
            newFeedbackForm.Feedback = Feedback;

            //We also made a Form object to hold the frmFeedback instance
            //If we were to declare if not as  frmFeedback object at first,
            //We wouldn't have access to the properties we need on it
            newForm = newFeedbackForm;
            newForm.ShowDialog();

            newFeedbackForm = null;
            newForm = null;

        }


        // public void FunctionTemplate(){ }//for adding access from plugin to main form.
        public string Path()
        {
           
            string appPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            appPath = appPath.Replace("file:\\", "");
            return appPath;
        }


      


    }
}
