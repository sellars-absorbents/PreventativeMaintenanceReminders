using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestPreventativeMaintenanceReminder
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            NameValueCollection parms = new NameValueCollection();
            PreventativeMaintenanceReminders.Reminder rmnd = new PreventativeMaintenanceReminders.Reminder();
            rmnd.Run(parms);     
        }
    }
}
