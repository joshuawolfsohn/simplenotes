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
using System.Drawing.Text;
using System.Windows.Documents;
using System.Diagnostics;

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
        public string latestversion { get; set; }
        public bool ignore;
        public string param;

        public Main()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            OpenDB openDB = new OpenDB();
            openDB.ShowDialog();

            this.password = openDB.password;
            this.sqlConn = new SQLiteConnection("Data Source=C:\\simplenotes\\simplenotes.db;Version=3;Password=" + password);
            this.comm = new SQLiteCommand(sqlConn);
            this.currentversion = "2.0.0";
            this.latestversion = currentversion;


            comm.CommandText = "SELECT [value] FROM [config] WHERE [parameter] = 'autoupdate'";
            ExecuteReadQuery();
            if (param == "true")
            {

                WebClient webClient = new WebClient();
                currentversion = Version.Parse(currentversion).ToString();
                this.latestversion = Version.Parse(webClient.DownloadString("https://raw.githubusercontent.com/joshuawolfsohn/simplenotes/master/versionnumber")).ToString();

                if (!(currentversion == latestversion))
                {
                    MessageBox.Show("Update available. To install update, go to Settings => Updater => Install Now.", "Simple Notes");
                }
            }

            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            comm.CommandText = "SELECT [value] FROM [config] WHERE [parameter] = 'theme'";
            ExecuteReadQuery();
            if (param == "dark")
            {
                lightToolStripMenuItem.Text = "Disable dark theme";

                foreach (Control control in this.Controls)
                {
                    control.BackColor = Color.MidnightBlue;
                    control.ForeColor = Color.White;
                }
            }

            richTextBox1.ReadOnly = true;
            textBox2.Visible = false;
            textBox2.Clear();
            label1.Visible = false;
            closeToolStripMenuItem.Enabled = false;
            saveCtrlSToolStripMenuItem.Enabled = false;
            newToolStripMenuItem.Enabled = true;
            openCtrlOToolStripMenuItem.Enabled = false;
            comboBox1.Visible = false;
            comboBox2.Visible = false;

            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(new SQLiteCommand("SELECT * FROM [notes]", sqlConn));
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            dataAdapter.Dispose();
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
            ExecuteReadQuery();
            if (param == "true")
            {
                richTextBox1.ShortcutsEnabled = false;
                disableCopyingFromNotesToolStripMenuItem.Checked = true;
            }
            FontStuff();

            if (String.IsNullOrWhiteSpace(this.password))
            {
                removePasswordToolStripMenuItem.Enabled = false;
            }
            else
            {
                removePasswordToolStripMenuItem.Enabled = true;
            }

            comm.CommandText = "SELECT [value] FROM [config] WHERE [parameter] = 'autoupdate'";
            ExecuteReadQuery();
            if (param == "true")
            {
                automaticallyCheckForUpdatesToolStripMenuItem.Checked = true;
            } else
            {
                automaticallyCheckForUpdatesToolStripMenuItem.Checked = false;
            }

            button1.Visible = false;

            if (!(currentversion == latestversion))
            {
                installUpdateToolStripMenuItem.Visible = true;
            }

        }

        private void saveCtrlSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenDB();
            try
            {
                if (String.IsNullOrWhiteSpace(textBox2.Text))
                {
                    MessageBox.Show("Please enter note title.");
                    return;
                }
                richTextBox1.SaveFile("C:\\simplenotes\\cache.rtf", RichTextBoxStreamType.RichText);
                FileStream fileStream = new FileStream("C:\\simplenotes\\cache.rtf", FileMode.Open);


                using (StreamReader textReader = new StreamReader(fileStream))
                {
                    if (listBox1.SelectedItems.Count == 1)
                    {
                        SQLiteParameter parameter = new SQLiteParameter();
                        parameter.DbType = DbType.String;
                        parameter.Value = textReader.ReadToEnd();

                        SQLiteCommand command = new SQLiteCommand("UPDATE [notes] SET [content] = ?" + textReader.ReadToEnd().Replace("'", "''") + ", [title] = '" + textBox2.Text.Replace("'", "''") + "', [lastmodified] = '" + DateTime.Now + "' WHERE [index] = " + listBox1.SelectedIndex, sqlConn);
                        command.Parameters.Add(parameter);
                        command.ExecuteNonQuery();
                    }
                    else
                    {
                        SQLiteParameter parameter = new SQLiteParameter();
                        parameter.DbType = DbType.String;
                        parameter.Value = textReader.ReadToEnd();

                        SQLiteCommand command = new SQLiteCommand("INSERT INTO [notes] ([index],[title],[content],[creation],[lastmodified]) VALUES (" + (listBox1.Items.Count) + ",'" + textBox2.Text.Replace("'", "''") + "',?,'" + DateTime.Now + "','" + DateTime.Now + "')", sqlConn);
                        command.Parameters.Add(parameter);
                        command.ExecuteNonQuery();
                    }
                }
                Main_Load(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
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

        private void button_delete_Click(object sender, EventArgs e)
        {
            //dialog are you sure?
            if (listBox1.SelectedItems.Count == 1)
            {
                if (MessageBox.Show("Deleting a note is irreversible. Are you sure you'd like to proceed?", "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    SQLiteCommand command = new SQLiteCommand("DELETE FROM [notes] where [index] = '" + listBox1.SelectedIndex + "'", sqlConn);
                    command.ExecuteNonQuery();
                    command.CommandText = "UPDATE [notes] set [index] = ([index] - 1) where [index] > " + listBox1.SelectedIndex;
                    command.ExecuteNonQuery();
                    Main_Load(sender, e);

                    richTextBox1.Clear();
                    listBox1.ClearSelected();
                    label2.ResetText();
                    label3.ResetText();
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (listBox1.SelectedItems.Count == 1)
                {

                    SQLiteCommand command = new SQLiteCommand("SELECT [content] FROM [notes] where [index] = " + listBox1.SelectedIndex, sqlConn);


                    FileStream fileStream = new FileStream("C:\\simplenotes\\cache.rtf", FileMode.Open);
                    using (StreamWriter textWriter = new StreamWriter(fileStream))
                    {
                        textWriter.Write(command.ExecuteScalar().ToString());
                    }

                    richTextBox1.LoadFile("C:\\simplenotes\\cache.rtf", RichTextBoxStreamType.RichText);
                    command.CommandText = "SELECT [creation] FROM [notes] where [index] = " + listBox1.SelectedIndex;
                    label2.Text = "Created: " + command.ExecuteScalar().ToString();
                    label2.Visible = true;
                    command.CommandText = "SELECT [lastmodified] FROM [notes] where [index] = " + listBox1.SelectedIndex;
                    label3.Text = "Last modified: " + command.ExecuteScalar().ToString();
                    label3.Visible = true;

                    deleteCtrlDToolStripMenuItem.Enabled = true;
                    openCtrlOToolStripMenuItem.Enabled = true;

                }
                deleteCtrlDToolStripMenuItem.Enabled = true;
                openCtrlOToolStripMenuItem.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void newToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            richTextBox1.ReadOnly = false;
            listBox1.ClearSelected();
            listBox1.Enabled = false;

            deleteCtrlDToolStripMenuItem.Enabled = false;
            newToolStripMenuItem.Enabled = false;
            openCtrlOToolStripMenuItem.Enabled = false;
            deleteCtrlDToolStripMenuItem.Enabled = false;
            saveCtrlSToolStripMenuItem.Enabled = true;
            closeToolStripMenuItem.Enabled = true;

            label1.Visible = true;
            label2.Visible = false;
            label3.Visible = false;
            textBox2.Visible = true;

            button1.Visible = true;

            comboBox1.Visible = true;
            comboBox2.Visible = true;


        }

        private void saveToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Please enter note title.");
                return;
            }
            if (listBox1.SelectedItems.Count == 1)
            {
                SQLiteCommand command = new SQLiteCommand("UPDATE [notes] SET [content] = '" + richTextBox1.Text.Replace("'", "''") + "', [title] = '" + textBox2.Text.Replace("'", "''") + "', [lastmodified] = '" + DateTime.Now + "' WHERE [index] = " + listBox1.SelectedIndex, sqlConn);
                command.ExecuteNonQuery();
            }
            else
            {
                SQLiteCommand command = new SQLiteCommand("INSERT INTO [notes] ([index],[title],[content],[creation],[lastmodified]) VALUES (" + (listBox1.Items.Count) + ",'" + textBox2.Text.Replace("'", "''") + "'" + ",'" + richTextBox1.Text.Replace("'", "''") + "', + '" + DateTime.Now + "', + '" + DateTime.Now + "')", sqlConn);
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

            /*button_new.Enabled = false;
            button_edit.Enabled = false;
            button_delete.Enabled = false;
            button_save.Enabled = true;
            button_cancel.Enabled = true;*/
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button_cancel_Click(object sender, EventArgs e)
        {

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

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void deleteCtrlDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button_delete_Click(sender, null);
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*Settings settings = new Settings();
            this.Enabled = false;
            settings.password = this.password;
            settings.sqlConn = this.sqlConn;
            settings.comm = this.comm;
            settings.currentversion = currentversion;
            settings.ShowDialog();
            this.Enabled = true;

            this.password = settings.password;*/
        }

        private void aboutToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            About about = new About();
            this.Enabled = false;
            about.currentversion = currentversion;
            about.ShowDialog();
            this.Enabled = true;
        }

        private void importNotesToolStripMenuItem_Click(object sender, EventArgs e)
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
                        FileStream fileStream = new FileStream(file, FileMode.Open);
                        using (StreamReader textReader = new StreamReader(fileStream))
                        {

                            comm.CommandText = "SELECT COUNT(*) FROM [notes]";
                            int currindex = int.Parse(comm.ExecuteScalar().ToString());
                            comm.CommandText = "INSERT INTO [notes] ([index],[title],[content],[creation],[lastmodified]) VALUES ('" + (currindex) + "','" + Path.GetFileNameWithoutExtension(file).Replace("'", "''") + "',@notecontent,'" + File.GetCreationTime(file) + "','" + DateTime.Now + "')";

                            comm.Parameters.Add(new SQLiteParameter("@notecontent"));
                            comm.Parameters["@notecontent"].DbType = DbType.String;
                            comm.Parameters["@notecontent"].Value = textReader.ReadToEnd();
                            comm.ExecuteNonQuery();
                        }

                    }
                    MessageBox.Show("Success!");
                    Application.Restart();
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToString());
            }
        }

        private void exportNotesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NoteExport noteExport = new NoteExport();
            noteExport.sqlConn = this.sqlConn;
            noteExport.password = this.password;
            this.Enabled = false;
            noteExport.ShowDialog();
            this.Enabled = true;
        }

        private void openCtrlOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.ReadOnly = false;
            listBox1.Enabled = false;

            label1.Visible = true;
            label2.Visible = false;
            label3.Visible = false;
            textBox2.Visible = true;

            saveCtrlSToolStripMenuItem.Enabled = true;
            comboBox1.Visible = true;
            comboBox2.Visible = true;
            closeToolStripMenuItem.Enabled = true;
            textBox2.Text = listBox1.GetItemText(listBox1.SelectedItem);

            button1.Visible = true;
        }

        private void fontDialog1_Apply(object sender, EventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void FontStuff()
        {
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();
            InstalledFontCollection installedFontCollection = new InstalledFontCollection();

            foreach (var font in installedFontCollection.Families)
            {
                comboBox1.Items.Add(font.Name);
            }

            object[] size = new object[] { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };
            comboBox2.Items.AddRange(size);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {

                richTextBox1.SelectionFont = new Font(comboBox1.GetItemText(comboBox1.SelectedItem), richTextBox1.SelectionFont.Size, new FontStyle());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                richTextBox1.SelectionFont = new Font(richTextBox1.SelectionFont.FontFamily, float.Parse(comboBox2.GetItemText(comboBox2.SelectedItem)), new FontStyle());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void richTextBox1_SelectionChanged(object sender, EventArgs e)
        {
            if (richTextBox1.SelectionLength > 1)
            {
                ignore = true;
            }
            if (ignore)
            {

            }
            else
            {
                comboBox1.SelectedIndex = comboBox1.Items.IndexOf(richTextBox1.SelectionFont.FontFamily.Name.ToString());
                comboBox2.SelectedIndex = comboBox2.Items.IndexOf(Convert.ToInt32(richTextBox1.SelectionFont.SizeInPoints));
            }

            ignore = false;
        }

        private void richTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                ignore = true;
            }
        }

        private void resetDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("All notes will be deleted and all settings will be reset to default. Would you like to continue?", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                sqlConn.Close();
                File.Delete("C:\\simplenotes\\simplenotes.db");
            }
        }

        private void removePasswordToolStripMenuItem_Click(object sender, EventArgs e)
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

            Main_Load(sender, e);
        }

        private void changePasswordToolStripMenuItem_Click(object sender, EventArgs e)
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

            Main_Load(sender, e);
        }

        private void lightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lightToolStripMenuItem.Text == "Enable dark theme")
            {
                comm.CommandText = "UPDATE [config] set [value] = 'dark' WHERE [parameter] = 'theme'";
            }
            else if (lightToolStripMenuItem.Text == "Disable dark theme")
            {
                comm.CommandText = "UPDATE [config] set [value] = 'light' WHERE [parameter] = 'theme'";
            }
            comm.ExecuteNonQuery();
            Application.Restart();
        }

        private void themeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void automaticallyCheckForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (automaticallyCheckForUpdatesToolStripMenuItem.Checked)
            {
                comm.CommandText = "UPDATE [config] SET [value] = 'false' WHERE [parameter] = 'autoupdate'";
                comm.ExecuteNonQuery();
                automaticallyCheckForUpdatesToolStripMenuItem.Checked = false;
            }
            else
            {
                comm.CommandText = "UPDATE [config] SET [value] = 'true' WHERE [parameter] = 'autoupdate'";
                comm.ExecuteNonQuery();
                automaticallyCheckForUpdatesToolStripMenuItem.Checked = true;
            }
        }

        private void checkForUpdatesNowToolStripMenuItem_Click(object sender, EventArgs e)
        {
                WebClient webClient = new WebClient();
                currentversion = Version.Parse(currentversion).ToString();
                string latestversion = Version.Parse(webClient.DownloadString("https://raw.githubusercontent.com/joshuawolfsohn/simplenotes/master/versionnumber")).ToString();

                if (!(currentversion == latestversion))
                {
                    MessageBox.Show("Update available. To install update, go to Settings => Updater => Install Now.");
                installUpdateToolStripMenuItem.Visible = true;
                }
        }

        private void installUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WebClient webClient = new WebClient();
            webClient.DownloadFile("https://github.com/joshuawolfsohn/simplenotes/blob/master/updater.exe?raw=true", "C:\\simplenotes\\updater.exe");
            Process.Start("C:\\simplenotes\\updater.exe");
            Application.Exit();
            //Download latest exec from raw github url that is generic for main master - so that the url will always download the latest verison
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                richTextBox1.SelectionColor = colorDialog1.Color;
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listBox1_SelectedIndexChanged(sender, e);
            Main_Load(sender, e);
            if (!(listBox1.Items.Count == 1))
            {
                richTextBox1.Clear();
            }
            textBox2.Clear();
            listBox1_SelectedIndexChanged(sender, e);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void moreToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void backupDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "Select a location to back up the database.";
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string output = Path.Combine(folderBrowserDialog.SelectedPath, "simplenotes.db." + DateTime.Now.ToFileTime() + ".bak");
                File.Copy("C:\\simplenotes\\simplenotes.db", output);
                MessageBox.Show("Database backup complete.");
            }
        }

        private void generalSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void updatesToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void securityToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void disableCopyingFromNotesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (disableCopyingFromNotesToolStripMenuItem.Checked)
            {
                comm.CommandText = "UPDATE [config] SET [value] = 'false' WHERE [parameter] = 'disableclipboard'";
                disableCopyingFromNotesToolStripMenuItem.Checked = false;

            }
            else
            {
                comm.CommandText = "UPDATE [config] SET [value] = 'true' WHERE [parameter] = 'disableclipboard'";
                disableCopyingFromNotesToolStripMenuItem.Checked = true;
            }
            comm.ExecuteNonQuery();
            Main_Load(sender, e);
        }

        private void OpenDB()
        {
            if (sqlConn.State == ConnectionState.Closed)
            {
                sqlConn.Open();
            }
        }

        private void CloseDB()
        {
            if (sqlConn.State == ConnectionState.Open)
            {
                sqlConn.Close();
            }
        }

        private void ExecuteReadQuery()
        {
            OpenDB();
            comm.ExecuteScalar(); // if selecting
            param = comm.ExecuteScalar().ToString();
        }

        private void ExecuteWriteQuery()
        {
            OpenDB();
            comm.ExecuteNonQuery(); // if modifying
            CloseDB();
        }

        private void restoreDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
           OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog.SafeFileName.Contains("simplenotes.db")) {
                    File.Delete("C:\\simplenotes\\simplenotes.db");
                    File.Copy(openFileDialog.FileName, "C:\\simplenotes\\simplenotes.db");
                    MessageBox.Show("Restored database backup.");
                }
            }
        }
    }
}