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
using System.Collections.ObjectModel;

namespace TerrariumMonitor
{
    /// <summary>
    /// Interaction logic for Terrarium_Group.xaml
    /// </summary>
    public partial class Terrarium_Group : UserControl
    {
        public string ID { get; set; }
        public string Mode { get; set; }
        public string COM { get; set; }
        public string Baud { get; set; }

        public SerialReader s_Reader = new SerialReader();

        ObservableCollection<Terrarium> lst_Terrariums = new ObservableCollection<Terrarium>();

        //Formatting
        public int Spacing { get; set; }
        public int Columns { get; set; }
        public string[] ColTitles { get; set; }

        public SolidColorBrush brsh_Foreground = new SolidColorBrush(Color.FromArgb(255, 195, 195, 195));//

        private string[] str_Units;

        List<Label> lst_ColTags = new List<Label>();
        List<Label> lst_TerrariumTags = new List<Label>();
        List<Label> lst_DataTags = new List<Label>();

        public Terrarium_Group()
        {
            InitializeComponent();
        }

        #region Public Methods
        #region Clone
        public Terrarium_Group Clone_Group() //=========================================================================
        {
            Terrarium_Group tg_Clone = new Terrarium_Group();
            tg_Clone.ID = this.ID;
            tg_Clone.Mode = this.Mode;
            tg_Clone.COM = this.COM;
            tg_Clone.Baud = this.Baud;

            //Clone Terrarium List
            foreach (Terrarium cur_Terrarium in lst_Terrariums)
            {
                tg_Clone.Add_Terrarium(cur_Terrarium.Clone_Terrarium());
            }

            //Clone Port
            //tg_Clone.s_Reader = this.s_Reader.Clone_Reader();

            return tg_Clone;

            //Clone label Lists


        }
        #endregion

        #region Ports
        public void Open_Port()
        {
            if (Mode == "COM")
            {
                s_Reader.COM = COM;

                int i_Baud;

                if (Int32.TryParse(Baud, out i_Baud))
                {
                    s_Reader.Boud = i_Baud;
                }
                else
                {
                    s_Reader.Boud = 9600;
                }

                s_Reader.OpenPort();
            }
        }

        public void Close_Port()
        {
            if (Mode == "Simulation")
            {

            }
            else
            {
                s_Reader.Close_Port();
            }
        }
        #endregion

        #region Terrarium
        public void Add_Terrarium(string str_Name)
        {
            lst_Terrariums.Add(new Terrarium());
            lst_Terrariums[lst_Terrariums.Count - 1].Name = str_Name;
        }

        public void Add_Terrarium(Terrarium new_Terrarium)
        {
            lst_Terrariums.Add(new_Terrarium);
        }

        public ObservableCollection<Terrarium> Get_Terrariums
        {
            get { return lst_Terrariums; }
        }

        #region Terrarium: Probes
        public void Add_Probe(string str_Name, string str_Units)
        {
            lst_Terrariums[lst_Terrariums.Count - 1].Add_Probe(str_Name, str_Units);
        }

        public void Add_Probe(string str_Name, string str_Units, int i_Index)
        {
            lst_Terrariums[i_Index].Add_Probe(str_Name, str_Units);
        }

        public string[] Get_Units()
        {
            List<string> lst_Units = new List<string>();

            foreach (Terrarium cur_Terrarium in lst_Terrariums)
            {
                foreach (Probe cur_Probe in cur_Terrarium.Get_Probes())
                {
                    lst_Units.Add(cur_Probe.Units);
                }
            }

            lst_Units = new List<string>(lst_Units.Distinct());

            return lst_Units.ToArray();
        }

        public void Gen_UnitsList()
        {
            List<string> lst_Units = new List<string>();

            foreach (Terrarium cur_Terrarium in lst_Terrariums)
            {
                foreach (Probe cur_Probe in cur_Terrarium.Get_Probes())
                {
                    lst_Units.Add(cur_Probe.Units);
                }
            }

            str_Units = lst_Units.ToArray();
        }

        public string[] Get_ProbeNames()
        {
            List<string> lst_ProbeNames = new List<string>();

            foreach (Terrarium cur_terrarium in lst_Terrariums)
            {
                foreach (Probe cur_Probe in cur_terrarium.Get_Probes())
                {
                    lst_ProbeNames.Add(cur_Probe.Name);
                }
            }

            return lst_ProbeNames.ToArray();
        }
        #endregion
        #endregion

        #region Formatting
        public void Gen_Controls()
        {
            int i_YPos = 0;
            Spacing = 15;

            Columns = Columns + 1;

            //Adds Columns to Terrarium Group
            //Adds Labels to each Column
            for (int i = 0; i < Columns; i++)
            {
                TG_Grid.ColumnDefinitions.Add(new ColumnDefinition());
                
                lst_ColTags.Add(new Label());

                if (i == 0)
                {
                    lst_ColTags[i].Content = ID;
                    lst_ColTags[i].Margin = new Thickness(5, i_YPos, 0, 0);
                    lst_ColTags[i].HorizontalAlignment = HorizontalAlignment.Left;

                    TG_Grid.ColumnDefinitions[i].Tag = Name;
                }
                else
                {
                    lst_ColTags[i].Content = ColTitles[i - 1];
                    TG_Grid.ColumnDefinitions[i].Tag = ColTitles[i - 1];

                    if (i == Columns - 1)
                    {
                        lst_ColTags[i].Margin = new Thickness(5, i_YPos, 0, 0);
                        lst_ColTags[i].HorizontalAlignment = HorizontalAlignment.Right;
                    }
                    else
                    {
                        lst_ColTags[i].Margin = new Thickness(0, i_YPos, 0, 0);
                        lst_ColTags[i].HorizontalAlignment = HorizontalAlignment.Center;
                    }
                }

                Grid.SetColumn(lst_ColTags[i], i);
                TG_Grid.Children.Add(lst_ColTags[i]);
            }

            i_YPos = Spacing;

            //Adds Labels for each Terrarium object in the leftmost Column.
            for (int i = 0; i < lst_Terrariums.Count; i++)
            {
                lst_TerrariumTags.Add(new Label());
                lst_TerrariumTags[i].Margin = new Thickness(5, i_YPos, 0, 0);
                lst_TerrariumTags[i].HorizontalAlignment = HorizontalAlignment.Left;
                lst_TerrariumTags[i].Content = lst_Terrariums[i].Name;
                Grid.SetColumn(lst_TerrariumTags[i], 0);
                TG_Grid.Children.Add(lst_TerrariumTags[i]);

                i_YPos = i_YPos + Spacing;
            }

            //Returns vertical positioning to Spacing so DataTags can be added below ColTags
            i_YPos = Spacing;

            for (int i = 0; i < lst_Terrariums.Count; i++)
            {
                foreach (Probe cur_Probe in lst_Terrariums[i].Get_Probes())
                {
                    for (int x = 1; x < TG_Grid.ColumnDefinitions.Count; x++)
                    {
                        if (cur_Probe.Name == TG_Grid.ColumnDefinitions[x].Tag.ToString())
                        {
                            lst_DataTags.Add(new Label());
                            lst_DataTags[lst_DataTags.Count - 1].Content = "No Data";

                            if (x == 1)
                            {
                                lst_DataTags[lst_DataTags.Count - 1].Margin = new Thickness(5, i_YPos, 0, 0);
                                lst_DataTags[lst_DataTags.Count - 1].HorizontalAlignment = HorizontalAlignment.Center;
                            }
                            else if (x == TG_Grid.ColumnDefinitions.Count - 1)
                            {
                                lst_DataTags[lst_DataTags.Count - 1].Margin = new Thickness(5, i_YPos, 0, 0);
                                lst_DataTags[lst_DataTags.Count - 1].HorizontalAlignment = HorizontalAlignment.Right;
                            }
                            else
                            {
                                lst_DataTags[lst_DataTags.Count - 1].Margin = new Thickness(0, i_YPos, 0, 0);
                                lst_DataTags[lst_DataTags.Count - 1].HorizontalAlignment = HorizontalAlignment.Center;
                            }

                            Grid.SetColumn(lst_DataTags[lst_DataTags.Count - 1], x);
                            TG_Grid.Children.Add(lst_DataTags[lst_DataTags.Count - 1]);
                        }
                    }
                }
                i_YPos = i_YPos + Spacing;
            }
        }

        public void Set_Width(int newWidth)
        {
            TG_Grid.Width = newWidth;
        }
        #endregion

        #region Update
        public void Update()
        {
            string[] str_Data;
            
            if (Mode == "COM")
            {
                if (s_Reader.Readable)
                {
                    //Add Check for Mode
                    str_Data = Get_SerialOut();

                    if (str_Data.Count() == lst_DataTags.Count)
                    {
                        for (int i = 0; i < lst_DataTags.Count; i++)
                        {
                            lst_DataTags[i].Content = str_Data[i] + str_Units[i];
                        }
                    }
                }
            }

            else
            {
                Random r_Num = new Random();

                for (int i = 0; i < lst_DataTags.Count; i++)
                {
                    if (lst_DataTags[i].Content.ToString() == "No Data")
                    {
                        lst_DataTags[i].Content = r_Num.Next(16, 32) + str_Units[i];
                    }
                    else if (lst_DataTags[i].Content.ToString().Substring(0, lst_DataTags[i].Content.ToString().IndexOf("°")) == "16")
                    {
                        lst_DataTags[i].Content = 16 + r_Num.Next(0, 2) + str_Units[i];
                    }
                    else if (lst_DataTags[i].Content.ToString().Substring(0, lst_DataTags[i].Content.ToString().IndexOf("°")) == "32")
                    {
                        lst_DataTags[i].Content = 32 - r_Num.Next(0, 2) + str_Units[i];
                    }
                    else
                    {
                        int i_CurNum;
                        if (Int32.TryParse(lst_DataTags[i].Content.ToString().Substring(0, lst_DataTags[i].Content.ToString().IndexOf("°")), out i_CurNum))
                        {
                            if (i_CurNum < 23)
                            {
                                lst_DataTags[i].Content = i_CurNum + r_Num.Next(-1, 2) + str_Units[i];
                            }
                            else
                            {
                                lst_DataTags[i].Content = i_CurNum + r_Num.Next(-1, 1) + str_Units[i];
                            }
                        }
                    }
                }
            }
        }
        #endregion
        #endregion

        #region Private Methods
        #region Update
        private string[] Get_SerialOut()
        {
            string str_RawData = s_Reader.Get_SerialOut();
            str_RawData = str_RawData.Trim(new char[] {'<', '>' });
            string[] str_Split = str_RawData.Split(new Char[] { ':' });

            return str_Split;
        }
        #endregion
        #endregion
    }
}
