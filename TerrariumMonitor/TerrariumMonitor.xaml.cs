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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace TerrariumMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string ConfigName { get; set; }
        public string ConfigPath { get; set; }
        public string ConfigBackupPath { get; set; }
        public string ConfigFolder { get; set; }

        DispatcherTimer ti_Update = new DispatcherTimer();
        Options w_Options = new Options();
        ErrorReporter ctrl_ErrorReporter = new ErrorReporter();


        ObservableCollection<Terrarium_Group> lst_TerrariumGroups = new ObservableCollection<Terrarium_Group>();

        ObservableCollection<string> lst_Units = new ObservableCollection<string>();
        int i_YPos = 5;
        int i_Spacing = 5;

        public MainWindow()
        {
            InitializeComponent();

            ti_Update.Tick += new EventHandler(TI_Update_OnTick);
            ti_Update.Interval = new TimeSpan(0, 0, 0, 0, 250);

            this.ConfigName = "TerrariumSettings.xml";
            //this.ConfigPath = @"C:\Users\David\TerrariumMonitor\Terrarium Settings.xaml";
            this.ConfigPath = System.IO.Path.Combine(Environment.CurrentDirectory, @"Config\", ConfigName);
            this.ConfigBackupPath = System.IO.Path.Combine(Environment.CurrentDirectory, @"Config\Backups\");
            this.ConfigFolder = System.IO.Path.Combine(Environment.CurrentDirectory, @"Config\");

            ti_Update.Start();

            //Load_Objects();
            Load_Interface();
        }

        #region Public Methods
        public double Get_Width()
        {
            return this.Width;
        }

        public void Reload_Objects()
        {
            ClosePorts();

            foreach (Terrarium_Group cur_Group in lst_TerrariumGroups)
            {
                Main_Grid.Children.Remove(cur_Group);
            }

            lst_TerrariumGroups.Clear();

            i_YPos = 0;

            Load_Objects();
        }

        public void Toggle_Buttons()
        {
            this.IsEnabled = !this.IsEnabled;
        }
        #endregion

        #region Private Methods
        private void Load_Objects()
        {
            ReadXML();
            Set_ColumnInfo();
            ShowGroups();
            OpenPorts();
        }

        private void Load_Interface()
        {
            if (Check_XMLExists())
            {
                if (Check_XMLFormat())
                {
                    ReadXML();
                    Set_ColumnInfo();
                    ShowGroups();
                    OpenPorts();
                }
                else
                {
                    ctrl_ErrorReporter.Report_Errors();
                    ctrl_ErrorReporter.Margin = new Thickness(0, i_YPos, 0, 0);
                    ctrl_ErrorReporter.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    ctrl_ErrorReporter.Arrange(new Rect(0, 0, ctrl_ErrorReporter.DesiredSize.Width, ctrl_ErrorReporter.DesiredSize.Height));
                    Main_Grid.Children.Add(ctrl_ErrorReporter);
                }
            }
        }

        #region XML
        #region Error Reporting
        private bool Check_XMLExists()
        {
            //Checks if .xaml Exists
            if (File.Exists(this.ConfigPath))
            {
                string spath = this.ConfigPath;
                return true;
            }

            return false;
        }

        private bool Check_XMLFormat() //check that children exists
        {
            bool b_IsValid = true;
            Match rx_Match;
            XDocument xd_Config = XDocument.Load(ConfigPath, LoadOptions.SetLineInfo);
            
            //checks formatting of .xaml

            //!!!Check that nodes exist!!!

            IEnumerable<XElement> lst_Elements = from xe_Elements in xd_Config.Descendants() select xe_Elements;

            foreach (XElement cur_Element in lst_Elements)
            {
                IXmlLineInfo li_LineInf = cur_Element;
                int i_LineNo = li_LineInf.LineNumber;

                if (cur_Element.Name != "Groups") //Checks if node is expected root
                {
                    if (cur_Element.Name == "Group")
                    {
                        //Checks Positioning of Node
                        if (cur_Element.Parent.Name != null && cur_Element.Parent.Name != "Groups")
                        {
                            b_IsValid = false;
                            ctrl_ErrorReporter.Add_Error(0, cur_Element.Name.ToString(), i_LineNo);
                        }
                        //checks Attrabutes of Node
                        if (cur_Element.Attribute("Name") == null)
                        {
                            b_IsValid = false;
                            ctrl_ErrorReporter.Add_Error(3, cur_Element.Name.ToString(), "Name", i_LineNo);
                        }
                        if (cur_Element.Attribute("Name").Value == "")
                        {
                            b_IsValid = false;
                            ctrl_ErrorReporter.Add_Error(3, cur_Element.Name.ToString(), "Name", i_LineNo);
                        }
                        if (cur_Element.Attributes("Name").Count() > 1)
                        {
                            b_IsValid = false;
                            ctrl_ErrorReporter.Add_Error(5, cur_Element.Name.ToString(), "Name", i_LineNo);
                        }
                        //Basic Port Checks
                        if (cur_Element.Descendants("Port") == null)
                        {
                            b_IsValid = false;
                            ctrl_ErrorReporter.Add_Error(1, "Port", i_LineNo);
                        }
                        if (cur_Element.Descendants("Port").Count() > 1)
                        {
                            b_IsValid = false;
                            ctrl_ErrorReporter.Add_Error(4, "Port", i_LineNo);
                        }
                    }
                }

                if (cur_Element.Name == "Port")
                {
                    //Checks Parent
                    if (cur_Element.Parent.Name != "Group")
                    {
                        b_IsValid = false;
                        ctrl_ErrorReporter.Add_Error(0, cur_Element.Name.ToString(), i_LineNo);
                    }
                    //Checks Attrabutes
                    if (cur_Element.Attribute("Type") == null)
                    {
                        b_IsValid = false;
                        ctrl_ErrorReporter.Add_Error(3, cur_Element.Name.ToString(), "Type", i_LineNo);
                    }
                    if (cur_Element.Attribute("Name") == null)
                    {
                        b_IsValid = false;
                        ctrl_ErrorReporter.Add_Error(3, cur_Element.Name.ToString(), "Name", i_LineNo);
                    }
                    if (cur_Element.Attribute("Baud") == null)
                    {
                        b_IsValid = false;
                        ctrl_ErrorReporter.Add_Error(3, cur_Element.Name.ToString(), "Baud", i_LineNo);
                    }


                    if (cur_Element.Attribute("Type").Value != "COM" && cur_Element.Attribute("Type").Value != "Network" && cur_Element.Attribute("Type").Value != "Simulation")
                    {
                        b_IsValid = false;
                        ctrl_ErrorReporter.Add_Error(2, cur_Element.Name.ToString(), "Type", i_LineNo);
                    }

                    if (cur_Element.Attribute("Name") != null)
                    {
                        rx_Match = Regex.Match(cur_Element.Attribute("Name").Value, @"^COM(\d+?)$");
                        if (!rx_Match.Success)
                        {
                            b_IsValid = false;
                            ctrl_ErrorReporter.Add_Error(2, cur_Element.Name.ToString(), "Name", i_LineNo);
                        }
                    }
                    
                    if (cur_Element.Attribute("Baud").Value == "") //check for valid baud rate
                    {
                        b_IsValid = false;
                    }


                }

                if (cur_Element.Name == "Terrarium")
                {
                    if (cur_Element.Parent.Name != "Group" && cur_Element.Parent.Name != "Terrariums")
                    {
                        b_IsValid = false;
                        ctrl_ErrorReporter.Add_Error(0, cur_Element.Name.ToString(), i_LineNo);
                    }

                    if (cur_Element.Attribute("Name") == null)
                    {
                        b_IsValid = false;
                        ctrl_ErrorReporter.Add_Error(3, cur_Element.Name.ToString(), "Name", i_LineNo);
                    }

                    if (cur_Element.Attribute("Name").Value == "")
                    {
                        b_IsValid = false;
                        ctrl_ErrorReporter.Add_Error(2, cur_Element.Name.ToString(), "Name", i_LineNo);
                    }
                }

                if (cur_Element.Name == "Sensor")
                {
                    if (cur_Element.Parent.Name != "Terrarium")
                    {
                        b_IsValid = false;
                    }

                    if (cur_Element.Attribute("Type") == null)
                    {
                        b_IsValid = false;
                    }

                    if (cur_Element.Attribute("Units") == null)
                    {
                        b_IsValid = false;
                    }
                }

                if (cur_Element.Name == "Groups" && cur_Element.Parent != null)
                {
                    b_IsValid = false;
                }
            }
            return b_IsValid;
        }
        #endregion

        private void ReadXML()
        {
            using (XmlReader xml_Reader = XmlReader.Create(ConfigPath))
            {
                while (xml_Reader.Read())
                {
                    if (xml_Reader.NodeType == XmlNodeType.Element)
                    {
                        if (xml_Reader.Name == "Group")
                        {
                            lst_TerrariumGroups.Add(new Terrarium_Group());
                            lst_TerrariumGroups[lst_TerrariumGroups.Count - 1].ID = xml_Reader.GetAttribute("Name");
                            lst_TerrariumGroups[lst_TerrariumGroups.Count - 1].Spacing = 15;
                        }

                        if (xml_Reader.Name == "Port")
                        {
                            lst_TerrariumGroups[lst_TerrariumGroups.Count - 1].Mode = xml_Reader.GetAttribute("Type");
                            lst_TerrariumGroups[lst_TerrariumGroups.Count - 1].COM = xml_Reader.GetAttribute("Name");
                            lst_TerrariumGroups[lst_TerrariumGroups.Count - 1].Baud = xml_Reader.GetAttribute("Baud");
                        }

                        if (xml_Reader.Name == "Terrarium")
                        {
                            lst_TerrariumGroups[lst_TerrariumGroups.Count - 1].Add_Terrarium(xml_Reader.GetAttribute("Name"));
                        }

                        if (xml_Reader.Name == "Sensor")
                        {
                            lst_TerrariumGroups[lst_TerrariumGroups.Count - 1].Add_Probe(xml_Reader.GetAttribute("Type"), xml_Reader.GetAttribute("Units"));
                            lst_Units.Add(xml_Reader.GetAttribute("Units"));
                        }
                    }
                }
            }
        }
        #endregion

        private void Set_ColumnInfo()
        {
            List<string> lst_ColumnTags = new List<string>();

            foreach (Terrarium_Group cur_Group in lst_TerrariumGroups)
            {
                lst_ColumnTags.AddRange(cur_Group.Get_ProbeNames());
            }

            lst_ColumnTags = new List<string>(lst_ColumnTags.Distinct());

            foreach (Terrarium_Group cur_Group in lst_TerrariumGroups)
            {
                cur_Group.ColTitles = lst_ColumnTags.ToArray();
                cur_Group.Columns = lst_ColumnTags.Count;
                cur_Group.Set_Width(85 * (lst_ColumnTags.Count + 1));
            }
        }

        private void ShowGroups()
        {
            foreach (Terrarium_Group cur_Group in lst_TerrariumGroups)
            {
                cur_Group.Gen_Controls();
                cur_Group.Gen_UnitsList();
                cur_Group.Width = Main_Grid.Width;

                cur_Group.Margin = new Thickness(0, i_YPos, 5, 0);
                cur_Group.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                cur_Group.Arrange(new Rect(0, 0, cur_Group.DesiredSize.Width, cur_Group.DesiredSize.Height));

                Main_Grid.Children.Add(cur_Group);

                i_YPos = i_YPos + (int)cur_Group.ActualHeight + i_Spacing;
            }
        }

        private void OpenPorts()
        {
            foreach (Terrarium_Group cur_Group in lst_TerrariumGroups)
            {
                cur_Group.Open_Port();
            }
        }

        private void ClosePorts()
        {
            foreach (Terrarium_Group cur_Group in lst_TerrariumGroups)
            {
                cur_Group.Close_Port();
            }
        }

        private void Open_Options()
        {
            w_Options.TerrariumGroups = Clone_Groups(); //
            Toggle_Buttons();
            w_Options.ParentWindow = this;
            w_Options.PopulateUI();
            w_Options.Show();
        }

        private ObservableCollection<Terrarium_Group> Clone_Groups()
        {
            ObservableCollection<Terrarium_Group> clone_Groups = new ObservableCollection<Terrarium_Group>();

            foreach (Terrarium_Group cur_Group in lst_TerrariumGroups)
            {
                clone_Groups.Add(cur_Group.Clone_Group());
            }

            return clone_Groups;
        }
        #endregion

        #region Events
        private void TI_Update_OnTick(object sender, EventArgs e)
        {
            foreach (Terrarium_Group cur_Group in lst_TerrariumGroups)
            {
                cur_Group.Update();
            }
        }

        private void PART_TITLEBAR_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void PART_CLOSE_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void bttn_Options_Show(object sender, RoutedEventArgs e)
        {
            Open_Options();
        }
        #endregion
    }
}
