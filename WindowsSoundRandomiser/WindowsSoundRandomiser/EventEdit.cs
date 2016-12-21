using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace WindowsSoundRandomiser
{
    public partial class EventEdit : Form
    {
        private string eventName;

        public EventEdit(string name)
        {
            InitializeComponent();

            eventName = name;

            LoadSounds();
        }
        
        private void LoadSounds()
        {
            try
            {
                DataTable table = new DataTable();

                table.Columns.Add(new DataColumn("Filename"));

                string[] files = Directory.GetFiles(Config.GetBasePath() + eventName);

                foreach (string file in files)
                {
                    table.Rows.Add(table.NewRow());

                    string[] splitString = file.Split('\\');
                    //splitString = splitString[splitString.Length - 1].Split('.');

                    table.Rows[table.Rows.Count - 1]["Filename"] = splitString[splitString.Length - 1];
                }

                grdSounds.DataSource = table;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Log.WriteToLog("Exception when loading sounds. " + ex.ToString());
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Wave files(.wav)|*.wav";
            dialog.Multiselect = true;

            DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                foreach (string file in dialog.FileNames)
                {
                    try
                    {
                        string[] splitString = file.Split('\\');
                        string destination = Path.Combine(Config.GetBasePath(), eventName, splitString[splitString.Length-1]);

                        File.Copy(file, destination);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error when adding sound. " + ex.ToString());
                    }
                }
            }

            LoadSounds();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (grdSounds.SelectedRows.Count != 0)
            {
                DialogResult result = MessageBox.Show("Are you sure you want to remove this sound?", "Remove Sound?", MessageBoxButtons.YesNo);

                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    try
                    {
                        string path = Path.Combine(Config.GetBasePath(), eventName, grdSounds.SelectedRows[0].Cells["Filename"].Value.ToString());
                        File.Delete(path);

                        LoadSounds();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error when removing sound. " + ex.Message);
                    }
                }
            }
        }
    }
}
