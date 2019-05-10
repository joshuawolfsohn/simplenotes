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
using System.Text.RegularExpressions;
using System.Net;

namespace Simple_Notes
{
    public partial class Main : Form
    {
        public SQLiteConnection sqlConn;
        public SQLiteCommand comm;
        public string password { get; set; }
        public string[] fontarray { get; set; }
        public bool darktheme;
        public string currentversion { get; set; }

        public Main()
        {
            try
            {
             this.Icon = new Icon("favicon.ico");
            } catch { }
            this.StartPosition = FormStartPosition.CenterScreen;
            OpenDB openDB = new OpenDB();
            openDB.ShowDialog();

            this.password = openDB.password;
            this.sqlConn = new SQLiteConnection("Data Source=C:\\simplenotes\\simplenotes.db;Version=3;Password=" + password);
            this.comm = new SQLiteCommand(sqlConn);
            this.currentversion = "1.0.1";


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
                WebClient webClient = new WebClient();
                currentversion = Version.Parse(currentversion).ToString();
                string latestversion = Version.Parse(webClient.DownloadString("https://raw.githubusercontent.com/codename13/simplenotes/master/versionnumber")).ToString();

                if (!(currentversion == latestversion))
                {
                    MessageBox.Show("Update available. To install update, go to More => Settings => Update.");
                        }
            }

                InitializeComponent();
}

private void Main_Load(object sender, EventArgs e)
{


            textBox1.ReadOnly = true;
            textBox2.Visible = false;
            textBox2.Clear();
            label1.Visible = false;
            button_cancel.Enabled = false;
            button_save.Enabled = false;
            button_new.Enabled = true;
            button_edit.Enabled = false;

            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(new SQLiteCommand("SELECT * FROM [notes]", sqlConn));
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            listBox1.DataSource = dataTable;
            listBox1.DisplayMember = "title";
            listBox1.Enabled = true;

            //  comboBox2.Items.Clear();
            /* List<int> numbers = Enumerable.Range(1, 40).ToList();
             foreach (var num in numbers)
             {
                 if (num % 2 == 0)
                 {
                     comboBox2.Items.Add(num);
                 }
             }*/




            //FontLoad();

            //open open, set selected index to db
            //comboBox1.SelectedIndex = comboBox1.FindStringExact(textBox1.Font.FontFamily.Name);
            //comboBox2.Text = textBox1.Font.Size.ToString();

            comm.CommandText = "SELECT [value] FROM [config] WHERE [parameter] = 'disableclipboard'";
            if (comm.ExecuteScalar().ToString() == "true")
            {
                textBox1.ShortcutsEnabled = false;
            }
        }


        /*private void FontLoad()
        {
           // comboBox1.Items.Clear();
            if (sqlConn.State == ConnectionState.Open)
            {
                sqlConn.Close();
            }
            if (sqlConn.State == ConnectionState.Closed)
            {
                sqlConn.Open();
            }

            comm.CommandText = "SELECT [value] FROM [config] where [parameter] = 'font'";
            string fontcombinedstring = comm.ExecuteScalar().ToString();
            string[] fontarray = fontcombinedstring.Split(',');

            //set font and size from db
            textBox1.Font = new Font(new FontFamily(fontarray[0]), Convert.ToSingle(fontarray[1]));



            //populate combobox1
            FontFamily[] fontFam = FontFamily.Families;
            foreach (var fonts in fontFam)
            {
                comboBox1.Items.Add(fonts.Name);
            }


        }*/

        private void button_more_Click(object sender, EventArgs e)
        {

            Dialog_More more = new Dialog_More();
            this.Enabled = false;
            more.password = password;
            more.currentversion = currentversion;
            more.sqlConn = this.sqlConn;
            more.comm = this.comm;
            more.ShowDialog();
            this.Enabled = true;

            if (more.DialogResult == DialogResult.OK)
            {
                this.password = more.password;
            }
            more.Dispose();
            if (sqlConn.State == ConnectionState.Open)
            {
                sqlConn.Close();
            }
            if (sqlConn.State == ConnectionState.Closed)
            {
                sqlConn.Open();
            }
            Main_Load(sender, e);
        }

        private void button_delete_Click(object sender, EventArgs e)
        {
            //dialog are you sure?
            if (listBox1.SelectedItems.Count == 1)
            {
                if (MessageBox.Show("Deleting a note is irreversible. Are you sure you'd like to proceed?", "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    SQLiteCommand command = new SQLiteCommand("DELETE FROM [notes] where [index] = '" + listBox1.SelectedIndex + "' AND [content] = '" + textBox1.Text.Replace("'", "''") + "'", sqlConn);
                    command.ExecuteNonQuery();
                    command.CommandText = "UPDATE [notes] set [index] = ([index] - 1) where [index] > " + listBox1.SelectedIndex;
                    command.ExecuteNonQuery();
                    Main_Load(sender, e);

                    textBox1.Clear();
                    listBox1.ClearSelected();
                    label2.ResetText();
                    label3.ResetText();
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count == 1)
            {
                SQLiteCommand command = new SQLiteCommand("SELECT [content] FROM [notes] where [index] = " + listBox1.SelectedIndex, sqlConn);
                textBox1.Text = command.ExecuteScalar().ToString();
                command.CommandText = "SELECT [creation] FROM [notes] where [index] = " + listBox1.SelectedIndex;
                label2.Text = "Created: " + command.ExecuteScalar().ToString();
                label2.Visible = true;
                command.CommandText = "SELECT [lastmodified] FROM [notes] where [index] = " + listBox1.SelectedIndex;
                label3.Text = "Last modified: " + command.ExecuteScalar().ToString();
                label3.Visible = true;

                button_delete.Enabled = true;
                button_edit.Enabled = true;

            }
            else
            {
                button_delete.Enabled = false;
                button_edit.Enabled = false;
            }
        }

        private void button_new_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox1.ReadOnly = false;
            listBox1.ClearSelected();
            listBox1.Enabled = false;

            button_new.Enabled = false;
            button_edit.Enabled = false;
            button_delete.Enabled = false;
            button_save.Enabled = true;
            button_cancel.Enabled = true;

            label1.Visible = true;
            label2.Visible = false;
            label3.Visible = false;
            textBox2.Visible = true;


        }

        private void button_save_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Please enter note title.");
                return;
            }
            if (listBox1.SelectedItems.Count == 1)
            {
                SQLiteCommand command = new SQLiteCommand("UPDATE [notes] SET [content] = '" + textBox1.Text.Replace("'", "''") + "', [title] = '" + textBox2.Text.Replace("'", "''") + "', [lastmodified] = '" + DateTime.Now + "' WHERE [index] = " + listBox1.SelectedIndex, sqlConn);
                command.ExecuteNonQuery();
            }
            else
            {
                SQLiteCommand command = new SQLiteCommand("INSERT INTO [notes] ([index],[title],[content],[creation],[lastmodified]) VALUES (" + (listBox1.Items.Count) + ",'" + textBox2.Text.Replace("'", "''") + "'" + ",'" + textBox1.Text.Replace("'", "''") + "', + '" + DateTime.Now + "', + '" + DateTime.Now + "')", sqlConn);
                command.ExecuteNonQuery();
                //code for dialog to name new note title
                // string title = yadayadayada
                //add new item to index
                // add textbox contents
            }
            Main_Load(sender, e);
        }

        private void button_edit_Click(object sender, EventArgs e)
        {

            button_new.Enabled = false;
            button_edit.Enabled = false;
            button_delete.Enabled = false;
            button_save.Enabled = true;
            button_cancel.Enabled = true;
            textBox1.ReadOnly = false;
            listBox1.Enabled = false;

            label1.Visible = true;
            label2.Visible = false;
            label3.Visible = false;
            textBox2.Visible = true;
            textBox2.Text = listBox1.GetItemText(listBox1.SelectedItem);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            listBox1_SelectedIndexChanged(sender, e);
            Main_Load(sender, e);
            if (!(listBox1.Items.Count == 1))
            {
                textBox1.Clear();
            }
            textBox2.Clear();
            listBox1_SelectedIndexChanged(sender, e);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            


            /* if (textBox1.Text.Contains(a + "+" + b + "=")) {
                 textBox1.Text = (a + b).ToString();
             }*/
        }


        /*private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comm.CommandText = "SELECT [value] FROM [config] where [parameter] = 'font'";
            string fontcombinedstring = comm.ExecuteScalar().ToString();
            string[] fontarray = fontcombinedstring.Split(',');

            comm.CommandText = "UPDATE [config] SET [value] = '" + comboBox1.GetItemText(comboBox1.SelectedItem) + "," + fontarray[1] + "' where [parameter] = 'font'";
            comm.ExecuteNonQuery();
            FontLoad();

        }*/

        /*private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            comm.CommandText = "SELECT [value] FROM [config] where [parameter] = 'font'";
            string fontcombinedstring = comm.ExecuteScalar().ToString();
            string[] fontarray = fontcombinedstring.Split(',');

            comm.CommandText = "UPDATE [config] SET [value] = '" + fontarray[0] + "," + comboBox2.Text + "' where [parameter] = 'font'";
            comm.ExecuteNonQuery();
            FontLoad();

        }*/

        /*private void comboenter(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter) && !(String.IsNullOrEmpty(comboBox2.Text)))
            {
                comboBox2_SelectedIndexChanged(sender, e);
            }
        }*/

        private void EqualsKey(object sender, KeyEventArgs e)
        {
            /*if ((e.KeyCode == Keys.Oemplus && e.Modifiers == Keys.Alt))
            {
                try
                {
                    try
                    {
                        string[] firstnum = textBox1.Text.Split('+');
                        int a = int.Parse(firstnum[0]);
                        string[] secondnum = firstnum[1].Split('=');
                        int b = int.Parse(secondnum[0]);

                        textBox1.Text = Environment.NewLine + a + "+" + b + "=" + (a + b) + textBox1.Text.Replace(a + "+" + b + "=", "");
                    }
                    catch
                    {
                        string[] firstnum = textBox1.Text.Split('*');
                        int a = int.Parse(firstnum[0]);
                        string[] secondnum = firstnum[1].Split('=');
                        int b = int.Parse(secondnum[0]);

                        textBox1.Text = Environment.NewLine + a + "*" + b + "=" + (a * b) + textBox1.Text.Replace(a + "*" + b + "=", "");
                    }
                } catch
                {
                    try
                    {
                        string[] firstnum = textBox1.Text.Split('-');
                        int a = int.Parse(firstnum[0]);
                        string[] secondnum = firstnum[1].Split('=');
                        int b = int.Parse(secondnum[0]);

                        textBox1.Text = Environment.NewLine + a + "-" + b + "=" + (a - b) + textBox1.Text.Replace(a + "-" + b + "=", "");
                    } catch
                    {
                        string[] firstnum = textBox1.Text.Split('/');
                        int a = int.Parse(firstnum[0]);
                        string[] secondnum = firstnum[1].Split('=');
                        int b = int.Parse(secondnum[0]);

                        textBox1.Text = Environment.NewLine + a + "/" + b + "=" + (a / b) + textBox1.Text.Replace(a + "/" + b + "=", "");
                    }
                }
            }*/
        }

        private void KeyCombos(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                if (e.KeyCode == Keys.N && button_new.Enabled)
                {
                    button_new_Click(sender, e);
                }
                if (e.KeyCode == Keys.D && button_delete.Enabled)
                {
                    button_delete_Click(sender, e);
                }
                if (e.KeyCode == Keys.E && button_edit.Enabled)
                {
                    button_edit_Click(sender, e);
                }
                if (e.KeyCode == Keys.S && button_save.Enabled)
                {
                    button_save_Click(sender, e);
                }
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
;