using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using System.Windows;
using System.IO;
using Microsoft.Win32;

namespace WindowsSoundRandomiser
{
    public partial class Form1 : Form
    {
        //device detection
        private const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        private const int DBT_DEVICEARRIVAL = 0x8000;
        private const int WM_DEVICECHANGE = 0x0219;

        //user session
        private const uint ENDSESSION_LOGOFF = 0x80000000;
        private const int WM_QUERYENDSESSION = 0x11;
        private const int WM_USERCHANGED = 0x0054;

        private const int WM_PRINT = 0x791;

        private bool close = false;

        public enum SoundEvents 
        { 
            DeviceConnect = 0
            ,DeviceDisconnect
            ,WindowsLogoff
            ,WindowsLogon
        };

        //private const string basePath = @"C:\Users\Frempt\Dropbox\Personal\sounds\";
        //private const string registryBasePath = @"AppEvents\Schemes\Apps\.Default\";

        public Form1()
        {
            InitializeComponent();

            Config.CreateConfigFile();

            //ensure the application runs at startup
            /*try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                key.SetValue("WSR", Application.ExecutablePath.ToString());
                key.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Log.WriteToLog("Exception when creating startup registry key. " +  ex.ToString());
            */

            //create the directories for the sounds
            try
            {
                CreateDirectories();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Log.WriteToLog("Exception when creating directories. " + ex.ToString());
            }

            LoadEvents();
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            switch(m.Msg)
            {
                case WM_DEVICECHANGE:
                    if (m.WParam.ToInt32() == DBT_DEVICEARRIVAL)
                    {
                        ChangeEventSound(SoundEvents.DeviceConnect);
                    }

                    if (m.WParam.ToInt32() == DBT_DEVICEREMOVECOMPLETE)
                    {
                        ChangeEventSound(SoundEvents.DeviceDisconnect);
                    }
                    break;

                case WM_QUERYENDSESSION:
                    if (m.WParam.ToInt64() == ENDSESSION_LOGOFF || m.WParam.ToInt64() == 0)
                    {
                        ChangeEventSound(SoundEvents.WindowsLogoff);
                        ChangeEventSound(SoundEvents.WindowsLogon);
                        close = true;
                        this.Close();
                    }
                    break;

                case WM_USERCHANGED:
                    ChangeEventSound(SoundEvents.WindowsLogon);
                    ChangeEventSound(SoundEvents.WindowsLogoff);
                    break;

                default:
                    break;
            }
        }

        public void CreateDirectories() 
        {
            string connectPath = Path.Combine(Config.GetBasePath(), SoundEvents.DeviceConnect.ToString());

            if(!Directory.Exists(connectPath))
            {
                Directory.CreateDirectory(connectPath);
            }

            string disconnectPath = Path.Combine(Config.GetBasePath(), SoundEvents.DeviceDisconnect.ToString());

            if (!Directory.Exists(disconnectPath))
            {
                Directory.CreateDirectory(disconnectPath);
            }

            string logonPath = Path.Combine(Config.GetBasePath(), SoundEvents.WindowsLogon.ToString());

            if (!Directory.Exists(logonPath))
            {
                Directory.CreateDirectory(logonPath);
            }

            string logoffPath = Path.Combine(Config.GetBasePath(), SoundEvents.WindowsLogoff.ToString());

            if (!Directory.Exists(logoffPath))
            {
                Directory.CreateDirectory(logoffPath);
            }
        }

        public void ChangeEventSound(SoundEvents eventType)
        {
            try
            {
                //choose a new sound
                string[] files = Directory.GetFiles(Config.GetBasePath() + eventType.ToString(), "*.wav");

                if (files.Length != 0)
                {
                    string fileName = files[GetRandomNumber(0, files.Length - 1)];

                    //update the registry
                    RegistryKey key = Registry.CurrentUser.OpenSubKey(Config.GetRegistryBasePath() + eventType.ToString() + @"\.Current", true);
                    if (key != null)
                    {
                        key.SetValue(null, fileName);
                        key.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Log.WriteToLog("Exception when changing event sound. " + ex.ToString());
            }
        }

        //get a random number between min and max
        public int GetRandomNumber(int min, int max)
        {
            Random rng = new Random();
            return rng.Next(min, max + 1);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (grdEvents.SelectedRows.Count != 0)
            {
                EventEdit dialog = new EventEdit(grdEvents.SelectedRows[0].Cells["Event Name"].Value.ToString());
                dialog.Show();
            }
        }

        private void LoadEvents()
        {
            try
            {
                DataTable table = new DataTable();

                table.Columns.Add(new DataColumn("Event Name"));
                table.Columns.Add(new DataColumn("Number of Sounds"));

                table.Rows.Add(table.NewRow());
                table.Rows[table.Rows.Count - 1]["Event Name"] = SoundEvents.DeviceConnect.ToString();
                table.Rows[table.Rows.Count - 1]["Number of Sounds"] = GetNumberOfSounds(SoundEvents.DeviceConnect);

                table.Rows.Add(table.NewRow());
                table.Rows[table.Rows.Count - 1]["Event Name"] = SoundEvents.DeviceDisconnect.ToString();
                table.Rows[table.Rows.Count - 1]["Number of Sounds"] = GetNumberOfSounds(SoundEvents.DeviceDisconnect);

                table.Rows.Add(table.NewRow());
                table.Rows[table.Rows.Count - 1]["Event Name"] = SoundEvents.WindowsLogon.ToString();
                table.Rows[table.Rows.Count - 1]["Number of Sounds"] = GetNumberOfSounds(SoundEvents.WindowsLogon);

                table.Rows.Add(table.NewRow());
                table.Rows[table.Rows.Count - 1]["Event Name"] = SoundEvents.WindowsLogoff.ToString();
                table.Rows[table.Rows.Count - 1]["Number of Sounds"] = GetNumberOfSounds(SoundEvents.WindowsLogoff);

                grdEvents.DataSource = table;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Log.WriteToLog("Exception when finding events. " + ex.ToString());
            }
        }

        private int GetNumberOfSounds(SoundEvents eventName)
        {
            string[] files = Directory.GetFiles(Config.GetBasePath() + eventName.ToString());
            return files.Length;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                WSR.Visible = true;
                WSR.ShowBalloonTip(3000);
                this.ShowInTaskbar = false;
            }
            else
            {
                this.ShowInTaskbar = true;
                WSR.Visible = false;
            }
        }

        private void notifyIcon1_DoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            WSR.Visible = false;
        }

        private void Form1_Closing(object sender, FormClosingEventArgs e)
        {
            if(!close)
            {
                e.Cancel = true;
                WindowState = FormWindowState.Minimized;
            }
        }

        private void contextMenuStrip1_Click(object sender, EventArgs e)
        {
            close = true;
            this.Close();
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            ChangeEventSound((SoundEvents)Enum.Parse(typeof(SoundEvents), grdEvents.SelectedRows[0].Cells["Event Name"].Value.ToString()));

            LoadEvents();
        }
    }
}
