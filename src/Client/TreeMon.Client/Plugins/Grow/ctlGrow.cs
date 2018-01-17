using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClientCore.Controls;

//rooms 
//inventory - plants, pots etc
//-  batch
// stage

namespace Grow
{
    public partial class ctlGrow : UserControl
    {
#pragma warning disable CS0169 // The field 'ctlGrow._nodeTree' is never used
        ctlNodeTree _nodeTree;
#pragma warning restore CS0169 // The field 'ctlGrow._nodeTree' is never used

        public ctlGrow()
        {
            InitializeComponent();
            //_nodeControl = new ctlNode(_connectionKey, _sessionKey);
            //pnlNodeGrowLocations.Controls.Add(_nodeControl);
        }
    }
}
