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
using System.IO;

namespace Simple_Notes
{
    public partial class CreateDB : Form
    {
        public CreateDB()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            InitializeComponent();
        }

        private void CreateDB_Load(object sender, EventArgs e)
        {
            MessageBox.Show("On the following screen, set a password to protect your notes.", "Welcome to Simple Notes!", MessageBoxButtons.OK);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(textBox1.Text))
            {
                if (!(textBox1.Text == textBox2.Text))
                {
                    label3.Text = "Passwords don't match.";
                    label3.ForeColor = Color.Red;
                }
                else
                {
                    Directory.CreateDirectory("C:\\simplenotes");
                    SQLiteConnection.CreateFile("C:\\simplenotes\\simplenotes.db");
                    SQLiteConnection sqlConn = new SQLiteConnection("Data Source=C:\\simplenotes\\simplenotes.db;Version=3;");
                    sqlConn.SetPassword(textBox1.Text);
                    sqlConn.Open();
                    SQLiteCommand command = new SQLiteCommand(sqlConn);
                    command.CommandText = "CREATE TABLE [notes] ([index] INT(3), [title] TEXT, [content] TEXT, [creation] TEXT, [lastmodified] TEXT)";
                    command.ExecuteNonQuery();
                    command.CommandText = "CREATE TABLE [config] ([index] INT(3), [parameter] TEXT, [value] TEXT)";
                    command.ExecuteNonQuery();
                    command.CommandText = "INSERT INTO [config] ([index],[parameter],[value]) VALUES (0,'font','Arial,8,FontStyle.Bold')";
                    command.ExecuteNonQuery();
                    command.CommandText = "INSERT INTO [config] ([index],[parameter],[value]) VALUES ('1','autoupdate','false')";
                    command.ExecuteNonQuery();
                    command.CommandText = "INSERT INTO [config] ([index],[parameter],[value]) VALUES ('2','disableclipboard','false')";
                    command.ExecuteNonQuery();
                    sqlConn.Close();

                    Application.Restart();
                }
            } else
            {
                label3.Text = "Choose a password.";
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            label3.ResetText();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            label3.ResetText();
        }

        private void enter_key(object sender, KeyEventArgs e)
        {
                if (e.KeyCode == Keys.Enter)
                {
                    button1_Click(sender, e);
                }
        }
    }
}
