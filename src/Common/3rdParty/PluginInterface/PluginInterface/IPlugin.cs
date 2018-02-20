
using System;
using TreeMon.Models.App;
/// <summary>
/// This is for forms app only because the use of System.Windows.Forms.UserControl for the
/// MainInterface
/// </summary>
namespace PluginInterface
{
    public interface IPlugin
    {
        IPluginHost Host { get; set; }

        string Name { get; }          //this is what is shown in the info box.
        string ShortName { get; } //this is the name that will show up in the list, and used to find when selected.
        string Description { get; }
        string Author { get; }
        string Version { get; }

        System.Windows.Forms.UserControl MainInterface { get; }

        void Initialize(UserSession session, AppInfo appSettings );

        void Dispose();

        void ResizeControl();

    }

}
