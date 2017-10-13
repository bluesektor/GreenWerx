using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginInterface
{
    public interface IPluginHost
    {
        void Feedback(string Feedback, IPlugin Plugin);
        string Path();
      
    }
}
