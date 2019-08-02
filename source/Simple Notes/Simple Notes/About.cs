using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Simple_Notes
{
    public partial class About : Form
    {
        public string currentversion { get; set; }

        public About()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();

        }

        private void About_Load(object sender, EventArgs e)
        {
            label2.Text = "Version " + currentversion;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/joshuawolfsohn/simplenotes");
        }
    }
}
