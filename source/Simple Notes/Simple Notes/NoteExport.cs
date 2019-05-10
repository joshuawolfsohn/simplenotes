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

    public partial class NoteExport : Form
    {
        public SQLiteConnection sqlConn;
        public SQLiteCommand comm;
        List<int> mylist = new List<int>();
        public string password { get; set; }
        public NoteExport()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (checkedListBox1.CheckedItems.Count == 0)
            {
                button3.Enabled = false;
            } else
            {
                SQLiteCommand command = new SQLiteCommand("SELECT [content] FROM [notes] where [index] = " + checkedListBox1.SelectedIndex, sqlConn);
                textBox1.Text = command.ExecuteScalar().ToString();
                button3.Enabled = true;
            }

        }

        private void NoteExport_Load(object sender, EventArgs e)
        {
            textBox1.ReadOnly = true;
            textBox1.ShortcutsEnabled = false;
            textBox1.ScrollBars = ScrollBars.Vertical;

            button3.Enabled = false;



            checkedListBox1.CheckOnClick = true;

            this.comm = new SQLiteCommand(sqlConn);
            comm.CommandText = "SELECT [title],[content],[creation] FROM [notes]";
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(comm);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            foreach (DataRow dataRow in dataTable.Rows)
            {
                checkedListBox1.Items.Add(dataRow["title"].ToString());
                
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mylist.Clear();
            foreach (var item in checkedListBox1.Items)
            {
                if (!(checkedListBox1.GetItemChecked(checkedListBox1.Items.IndexOf(item))))
                {
                    mylist.Add(checkedListBox1.Items.IndexOf(item));
                }
            }
            foreach (int thing in mylist)
            {
                checkedListBox1.SetItemChecked(thing, true);
            }
            button3.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            mylist.Clear();
            foreach (var item in checkedListBox1.Items)
            {
                if ((checkedListBox1.GetItemChecked(checkedListBox1.Items.IndexOf(item))))
                {
                    mylist.Add(checkedListBox1.Items.IndexOf(item));
                }
            }
            foreach (int thing in mylist)
            {
                checkedListBox1.SetItemChecked(thing, false);
            }
            mylist.Clear();
            button3.Enabled = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                fbd.Description = "Choose a folder to export your note files";
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    foreach (var item in checkedListBox1.Items)
                    {
                        if (checkedListBox1.GetItemChecked(checkedListBox1.Items.IndexOf(item))) {
                            comm.CommandText = "SELECT [title],[content],[creation] FROM [notes] WHERE [title] = '" + checkedListBox1.GetItemText(item) + "'";
                            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(comm);
                            DataTable dataTable = new DataTable();
                            dataAdapter.Fill(dataTable);
                            foreach (DataRow dataRow in dataTable.Rows)
                            {
                                File.WriteAllText(fbd.SelectedPath + "\\" + dataRow["title"].ToString() + ".txt", "Original creation date " + dataRow["creation"].ToString() + Environment.NewLine + dataRow["content"].ToString());
                            }
                        }
                    }
                    MessageBox.Show("Success!");

                }
            }
            catch
            {
                MessageBox.Show("Error!");
            }
        }
    }
}
