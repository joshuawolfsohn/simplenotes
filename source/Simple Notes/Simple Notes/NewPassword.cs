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
    public partial class NewPassword : Form
    {
        public string password { get; set; }
        public SQLiteConnection sqlConn;

        public NewPassword()
        {
            this.StartPosition = FormStartPosition.CenterScreen;

            InitializeComponent();
        }

        private void NewPassword_Load(object sender, EventArgs e)
        {
            try
            {
                using (SQLiteCommand command = new SQLiteCommand(sqlConn))
                {
                    command.CommandText = "PRAGMA database_list";
                    command.ExecuteNonQuery();
                }
                sqlConn.Close();
                label1.Visible = false;
                textBox1.Visible = false;
            }
            catch
            {
                return;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
                {
                if (textBox1.Visible)
                {
                    this.password = textBox1.Text;
                    sqlConn.Open();
                    using (SQLiteCommand command = new SQLiteCommand(sqlConn))
                    {
                        command.CommandText = "PRAGMA database_list";
                        command.ExecuteNonQuery();
                    }
            } else
            {
                this.password = "";
            }
            }
            catch
            {
                label3.Visible = true;
                label3.Text = "Current password incorrect";
            }

            if ((textBox2.Text == textBox3.Text) && !(String.IsNullOrEmpty(textBox2.Text)) && !(String.IsNullOrEmpty(textBox3.Text)))

            {
                try
                {
                    sqlConn.Open();
                    sqlConn.ChangePassword(textBox2.Text);
                    sqlConn.Close();
                    MessageBox.Show("Success!");
                    
                    password = textBox2.Text;
                    this.DialogResult = DialogResult.OK;
                    Application.Restart();
                } catch
                {
                    label3.Visible = true;
                    label3.Text = "Current password incorrect";
                }
            } else
            {
                label3.Visible = true;
                label3.Text = "New passwords don't match";
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

        private void textBox3_TextChanged(object sender, EventArgs e)
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

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }
    }
}
