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
    public partial class OpenDB : Form
    {
        public string password { get; set; }
        public SQLiteConnection sqlConn;

        public OpenDB()
        {
            //this.Icon = new Icon("favicon.ico");
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            InitializeComponent();

            this.password = textBox1.Text;
            this.sqlConn = new SQLiteConnection("Data Source=C:\\simplenotes\\simplenotes.db;Version=3;Password=" + password);
        }

        private void OpenDB_Load(object sender, EventArgs e)
        {
            try
            {
                sqlConn = new SQLiteConnection("Data Source=C:\\simplenotes\\simplenotes.db;Version=3;");
                sqlConn.Open();
                using (SQLiteCommand command = new SQLiteCommand(sqlConn))
                {
                    command.CommandText = "PRAGMA database_list";
                    command.ExecuteNonQuery();
                }
                this.password = "";
                this.Close();
            } catch
            {
                return;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

                try
                {
                    SQLiteConnection sqlConn = new SQLiteConnection("Data Source=C:\\simplenotes\\simplenotes.db;Version=3;Password=" + textBox1.Text + ";");
                    sqlConn.Open();
                    using (SQLiteCommand command = new SQLiteCommand(sqlConn))
                    {
                        command.CommandText = "PRAGMA database_list";
                        command.ExecuteNonQuery();
                    }
                    sqlConn.Close();
                    this.password = textBox1.Text;

                    this.Close();
                }
                catch
                {
                    label2.ForeColor = Color.Red;
                    label2.Text = "Incorrect password entered.";
                }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            label2.ResetText();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
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
