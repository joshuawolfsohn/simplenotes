using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
namespace Simple_Notes
{
    public partial class Dialog_More : Form
    {
        public string password { get; set; }
        public string currentversion { get; set; }
        public SQLiteConnection sqlConn;
        public SQLiteCommand comm;

        public Dialog_More()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();
        }

        private void button_about_Click(object sender, EventArgs e)
        {
            About about = new About();
            this.Enabled = false;
            about.currentversion = currentversion;
            about.ShowDialog();
            this.Enabled = true;
        }

        private void Dialog_More_Load(object sender, EventArgs e)
        {
        }

        private void button_settings_Click(object sender, EventArgs e)
        {

            Settings settings = new Settings();
            this.Enabled = false;
            settings.password = this.password;
            settings.sqlConn = this.sqlConn;
            settings.comm = this.comm;
            settings.currentversion = currentversion;
            settings.ShowDialog();
            this.Enabled = true;

            this.password = settings.password;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
