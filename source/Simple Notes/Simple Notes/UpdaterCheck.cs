using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Diagnostics;

namespace Simple_Notes
{
    public partial class UpdaterCheck : Form
    {
        public string currentversion { get; set; }
        public UpdaterCheck()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();
        }

        private void UpdaterCheck_Load(object sender, EventArgs e)
        {
            try
            {
                WebClient webClient = new WebClient();
                currentversion = Version.Parse(currentversion).ToString();
                string latestversion = Version.Parse(webClient.DownloadString("https://raw.githubusercontent.com/codename13/simplenotes/master/versionnumber")).ToString();
                label1.Text = "Current version " + currentversion;
                label2.Text = "Latest version " + latestversion;



                if (currentversion == latestversion)
                {
                    button1.Visible = false;
                    label2.Text = "No updates available at the moment.";
                } else
                {
                    button1.Visible = true;
                    label2.ForeColor = Color.Red;
                }
            } catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            WebClient webClient = new WebClient();
            webClient.DownloadFile("https://github.com/codename13/simplenotes/blob/master/updater.exe?raw=true", "C:\\simplenotes\\updater.exe");
            Process.Start("C:\\simplenotes\\updater.exe");
            Application.Exit();
            //Download latest exec from raw github url that is generic for main master - so that the url will always download the latest verison
        }
    }
}
