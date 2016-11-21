using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.IO;
using System.Xml.Linq;

namespace TerrariumMonitor
{
    public partial class Options : Window
    {
        public ObservableCollection<Terrarium_Group> TerrariumGroups { get; set; }
        public MainWindow ParentWindow { get; set; }

        private string[] str_PortNames;

        public Options()
        {
            InitializeComponent();
        }

        #region Public Methods
        public void PopulateUI()
        {
            cmbx_Groups.ItemsSource = TerrariumGroups;

            Get_COMPorts();
        }
        #endregion

        #region Private Methods
        private int Get_GroupIndex()
        {
            return cmbx_Groups.SelectedIndex;
        }

        private int Get_TerrariumIndex()
        {
            return cmbx_Terrariums.SelectedIndex;
        }

        private int Get_ProbesIndex()
        {
            return cmbx_Probes.SelectedIndex;
        }

        private void Get_COMPorts()
        {
            str_PortNames = SerialPort.GetPortNames();
            cmbx_COM.ItemsSource = str_PortNames;

            if (str_PortNames.Count() == 0)
            {
                cmbx_COM.IsEnabled = false;
            }
        }

        private void Save_Changes()
        {
            Backup_Config();
            Wright_XAML();
        }

        private void Backup_Config()
        {
            //Check that config exists if not skip
            if (File.Exists(ParentWindow.ConfigPath))
            {
                //Saves old config with Timestamp
                DateTime dt_Timestamp = DateTime.Now;

                string str_BackupFile = System.IO.Path.Combine(ParentWindow.ConfigBackupPath, dt_Timestamp.ToString("yyyy.MM.dd HH.mm") + " " + ParentWindow.ConfigName);

                if (!Directory.Exists(ParentWindow.ConfigBackupPath))
                {
                    Directory.CreateDirectory(ParentWindow.ConfigBackupPath);
                }
                File.Copy(ParentWindow.ConfigPath, str_BackupFile, true);
            }
        }

        private void Wright_XAML()
        {
            XDocument xD_NewINI = new XDocument();
            XElement xE_Groups = new XElement("Groups");
            xD_NewINI.Add(xE_Groups);


            foreach (Terrarium_Group cur_Group in TerrariumGroups)
            {
                //Creates new xElement and adds required info to as attrabutes

                XElement xE_Group = new XElement("Group");
                xE_Group.Add(new XAttribute("Name", cur_Group.ID));
                xE_Groups.Add(xE_Group);

                XElement xE_Port = new XElement("Port");
                xE_Port.Add(new XAttribute("Type", cur_Group.Mode));
                if (cur_Group.COM != null)
                {
                    xE_Port.Add(new XAttribute("Name", cur_Group.COM));
                }
                xE_Port.Add(new XAttribute("Baud", cur_Group.Baud));
                xE_Group.Add(xE_Port);

                XElement xE_Terrariums = new XElement("Terrariums");
                xE_Port.AddAfterSelf(xE_Terrariums);

                foreach (Terrarium cur_Terrarium in cur_Group.Get_Terrariums)
                {
                    //creates new terrarium element with attrabute name
                    XElement xE_Terrarium = new XElement("Terrarium");
                    xE_Terrarium.Add(new XAttribute("Name", cur_Terrarium.Name));
                    xE_Terrariums.Add(xE_Terrarium);

                    foreach (Probe cur_Probe in cur_Terrarium.Get_Probes())
                    {
                        XElement xE_Sensor = new XElement("Sensor");
                        xE_Sensor.Add(new XAttribute("Type", cur_Probe.Name));
                        xE_Sensor.Add(new XAttribute("Units", cur_Probe.Units.ToString()));
                        xE_Terrarium.Add(xE_Sensor);
                    }
                }
            }

            xD_NewINI.Save(ParentWindow.ConfigPath);
        }
        #endregion

        #region Events
        #region ComboBox Events
        private void cmbx_Groups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TerrariumGroups.Count >= 1)
            {
                cmbx_Terrariums.ItemsSource = TerrariumGroups[Get_GroupIndex()].Get_Terrariums;
                cmbx_Mode.SelectedValue = TerrariumGroups[Get_GroupIndex()].Mode;
                cmbx_COM.SelectedValue = TerrariumGroups[Get_GroupIndex()].COM;
                cmbx_BaudRate.SelectedValue = TerrariumGroups[Get_GroupIndex()].Baud;
            }
        }

        private void cmbx_Mode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbx_Mode.IsDropDownOpen)
            {
                cmbx_Mode.IsDropDownOpen = false;

                TerrariumGroups[Get_GroupIndex()].Mode = cmbx_Mode.SelectedValue.ToString();
            }
        }

        private void cmbx_COM_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbx_COM.IsDropDownOpen)
            {
                cmbx_COM.IsDropDownOpen = false;
                if (cmbx_COM.SelectedValue != null)
                {
                    TerrariumGroups[Get_GroupIndex()].COM = cmbx_COM.SelectedValue.ToString();
                }
            }
        }

        private void cmbx_BaudRate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbx_BaudRate.IsDropDownOpen)
            {
                cmbx_BaudRate.IsDropDownOpen = false;

                TerrariumGroups[Get_GroupIndex()].Baud = cmbx_BaudRate.SelectedValue.ToString();
            }
        }

        private void cmbx_Terrariums_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TerrariumGroups[Get_GroupIndex()].Get_Terrariums.Count >= 1)
            {
                cmbx_Probes.ItemsSource = TerrariumGroups[Get_GroupIndex()].Get_Terrariums[Get_TerrariumIndex()].Get_Probes();
                cmbx_Probes.SelectedIndex = 0;
            }
        }

        private void cmbx_Probes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TerrariumGroups[Get_GroupIndex()].Get_Terrariums[Get_TerrariumIndex()].Get_Probes().Count >= 1)
            {
                cmbx_Units.SelectedValue = TerrariumGroups[Get_GroupIndex()].Get_Terrariums[Get_TerrariumIndex()].Get_Probe(Get_ProbesIndex()).Units;
            }
        }

        private void cmbx_Units_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbx_Units.IsDropDownOpen)
            {
                cmbx_Units.IsDropDownOpen = false;

                TerrariumGroups[Get_GroupIndex()].Get_Terrariums[Get_TerrariumIndex()].Get_Probe(Get_ProbesIndex()).Units = cmbx_Units.SelectedValue.ToString();
            }
        }
        #endregion

        #region Button Events
        private void bttn_Apply_Click(object sender, RoutedEventArgs e)
        {
            //creates Backup and Writes out new config
            //Tells Parent window to purge old Terrarium groups and rebuild off new Config
            Save_Changes();
            ParentWindow.Toggle_Buttons();
            ParentWindow.Reload_Objects();
            this.Hide();
        }

        private void bttn_Close_Click(object sender, RoutedEventArgs e)
        {
            ParentWindow.Toggle_Buttons();
            this.Hide();
        }

        private void bttn_AddGroup_Click(object sender, RoutedEventArgs e)
        {
            if (tb_AddGroup.Text != "")
            {
                TerrariumGroups.Add(new Terrarium_Group());
                TerrariumGroups[TerrariumGroups.Count - 1].ID = tb_AddGroup.Text;

                TerrariumGroups[TerrariumGroups.Count - 1].Mode = "COM";
                TerrariumGroups[TerrariumGroups.Count - 1].COM = "COM99";
                TerrariumGroups[TerrariumGroups.Count - 1].Baud = "9600";

                tb_AddGroup.Text = "";
            }
        }

        private void bttn_AddTerrarium_Click(object sender, RoutedEventArgs e)
        {
            if (tb_AddTerrarium.Text != "")
            {
                TerrariumGroups[Get_GroupIndex()].Add_Terrarium(tb_AddTerrarium.Text);

                tb_AddTerrarium.Text = "";
            }
        }

        private void bttn_AddProbe_Click(object sender, RoutedEventArgs e)
        {
            if (tb_AddProbe.Text != "")
            {
                TerrariumGroups[Get_GroupIndex()].Add_Probe(tb_AddProbe.Text, "°C", Get_TerrariumIndex());

                tb_AddProbe.Text = "";
            }
        }

        #endregion
        private void PART_TITLEBAR_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        #endregion
    }
}
