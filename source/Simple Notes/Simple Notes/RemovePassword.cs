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
    public partial class RemovePassword : Form
    {
        public string password { get; set; }
        public RemovePassword()
        {

            this.StartPosition = FormStartPosition.CenterScreen;

            InitializeComponent();

        }

        private void RemovePassword_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == textBox2.Text)
            {
                SQLiteConnection sqlConn = new SQLiteConnection("Data Source=C:\\simplenotes\\simplenotes.db;Version=3;Password=" + textBox1.Text);
                try
                {
                    sqlConn.Open();
                    using (SQLiteCommand command = new SQLiteCommand(sqlConn))
                    {
                        command.CommandText = "PRAGMA database_list";
                        command.ExecuteNonQuery();
                    }


                    if (MessageBox.Show("Are you sure you'd like to remove password protection?", "Confirm password removal", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        sqlConn.ChangePassword("");
                        sqlConn.Close();
                        MessageBox.Show("Success!");
                        password = null;
                        this.DialogResult = DialogResult.OK;
                        Application.Restart();
                    }

                } catch
                {
                    label3.Visible = true;
                    label3.Text = "Incorrect password entered";
                }

            } else
            {
                label3.Visible = true;
                label3.Text = "Passwords don't match";
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            label3.Visible = false;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            label3.Visible = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void enterkey(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1_Click(sender, e);
            }
        }
    }
}
