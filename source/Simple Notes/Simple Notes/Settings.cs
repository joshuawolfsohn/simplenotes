using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Simple_Notes
{
    public partial class Settings : Form
    {
        public string password { get; set; }
        public SQLiteConnection sqlConn { get; set; }
        public bool darktheme;
        public string currentversion { get; set; }
        public SQLiteCommand comm { get; set; }
        public Settings()
        {
            this.StartPosition = FormStartPosition.CenterScreen;



            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RemovePassword removePassword = new RemovePassword();
            this.Enabled = false;
            removePassword.ShowDialog();
            this.Enabled = true;

            if (removePassword.DialogResult == DialogResult.OK)
            {
                this.password = removePassword.password;
            }
            removePassword.Dispose();

            Settings_Load(sender, e);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            NewPassword newPassword = new NewPassword();
            this.Enabled = false;
            newPassword.sqlConn = this.sqlConn;
            newPassword.password = this.password;
            newPassword.ShowDialog();
            this.Enabled = true;

            if (newPassword.DialogResult == DialogResult.OK)
            {
                this.password = newPassword.password;
            }
            newPassword.Dispose();

            Settings_Load(sender, e);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Dispose();
        }

        private void Settings_Load(object sender, EventArgs e)
        {

            if (String.IsNullOrWhiteSpace(this.password))
            {
                button1.Enabled = false;
            } else
            {
                button1.Enabled = true;
            }

            comm.CommandText = "SELECT [value] FROM [config] WHERE [parameter] = 'autoupdate'";
            if (sqlConn.State == ConnectionState.Open)
            {
                sqlConn.Close();
            }
            if (sqlConn.State == ConnectionState.Closed)
            {
                sqlConn.Open();
            }
            if (comm.ExecuteScalar().ToString() == "true")
            {
                checkBox1.Checked = true;
            } else
            {
                checkBox1.Checked = false;
            }

            comm.CommandText = "SELECT [value] FROM [config] WHERE [parameter] = 'disableclipboard'";
            if (comm.ExecuteScalar().ToString() == "true")
            {
                checkBox2.Checked = true;
            }
            else
            {
                checkBox2.Checked = false;
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.BackColor = Color.Black;
            button4.Text = "Switch to light theme";
            
            //Color theme - have it set a bool that is saved to db for reloading, main form checks the db and passes the bool to the other forms This bool also changes the button text depending on current theme.
        }

        private void button5_Click(object sender, EventArgs e)
        {
            UpdaterCheck updaterCheck = new UpdaterCheck();
            updaterCheck.currentversion = currentversion;
            this.Enabled = false;
            updaterCheck.ShowDialog();
            this.Enabled = true;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                //change flag in db and have main form read this flag on launch - check for updates auto on start;
                comm.CommandText = "UPDATE [config] SET [value] = 'true' WHERE [parameter] = 'autoupdate'";
                comm.ExecuteNonQuery();

            } else
            {
                comm.CommandText = "UPDATE [config] SET [value] = 'false' WHERE [parameter] = 'autoupdate'";
                comm.ExecuteNonQuery();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            NoteExport noteExport = new NoteExport();
            noteExport.sqlConn = this.sqlConn;
            noteExport.password = this.password;
            this.Enabled = false;
            noteExport.ShowDialog();
            this.Enabled = true;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = true;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    this.comm = new SQLiteCommand(sqlConn);

                    foreach (var file in ofd.FileNames)
                    {
                        comm.CommandText = "SELECT COUNT(*) FROM [notes]";
                        int currindex = int.Parse(comm.ExecuteScalar().ToString());
                        comm.CommandText = "INSERT INTO [notes] ([index],[title],[content],[creation],[lastmodified]) VALUES ('" + (currindex) + "','" + Path.GetFileNameWithoutExtension(file).Replace("'", "''") + "','" + File.ReadAllText(file).Replace("'", "''") + "','IMPORTED','" + DateTime.Now + "')";
                        comm.ExecuteNonQuery();
                    }
                    MessageBox.Show("Success!");
                    Application.Restart();
                }
            } 
            catch
            {
                MessageBox.Show("Error!");
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                comm.CommandText = "UPDATE [config] SET [value] = 'true' WHERE [parameter] = 'disableclipboard'";

            } else
            {
                comm.CommandText = "UPDATE [config] SET [value] = 'false' WHERE [parameter] = 'disableclipboard'";
            }
            comm.ExecuteNonQuery();
        }
    }
}
