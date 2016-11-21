using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace TerrariumMonitor.Res.Templates.Controls.Classes
{
    public partial class ComboBox_DelButton_Click : ResourceDictionary
    {
        void Del_bttn_Click(object sender, RoutedEventArgs e)
        {
            var parent = ((Button)sender).TemplatedParent;
            ComboBoxItem cmbx_SelectedItem = (ComboBoxItem)parent;
            ComboBox cmbx_Parent = ItemsControl.ItemsControlFromItemContainer(parent) as ComboBox;
            object obj_SelectedItem = cmbx_SelectedItem.Content;

            if (cmbx_Parent.ItemsSource is ObservableCollection<Terrarium_Group>)
            {
                ObservableCollection<Terrarium_Group> cmbx_Source = (ObservableCollection<Terrarium_Group>)cmbx_Parent.ItemsSource;

                Terrarium_Group cur_Group = (Terrarium_Group)obj_SelectedItem;

                for (int i = 0; i < cmbx_Source.Count; i++)
                {
                    if (cur_Group.ID == cmbx_Source[i].ID)
                    {
                        if (i == 0 && cmbx_Source.Count > 1)
                        {
                            cmbx_Parent.SelectedIndex = 1;
                        }
                        else if (cmbx_Source.Count > 1)
                        {
                            cmbx_Parent.SelectedIndex = 0;
                        }


                        cmbx_Source.RemoveAt(i);
                        cmbx_Parent.SelectedIndex = 0;
                    }
                }
            }

            if (cmbx_Parent.ItemsSource is ObservableCollection<Terrarium>)
            {
                ObservableCollection<Terrarium> cmbx_Source = (ObservableCollection<Terrarium>)cmbx_Parent.ItemsSource;

                Terrarium cur_Terrarium = (Terrarium)obj_SelectedItem;

                for (int i = 0; i < cmbx_Source.Count; i++)
                {
                    if (cur_Terrarium.Name == cmbx_Source[i].Name)
                    {
                        if (i == 0 && cmbx_Source.Count > 1)
                        {
                            cmbx_Parent.SelectedIndex = 1;
                        }
                        else if (cmbx_Source.Count > 1)
                        {
                            cmbx_Parent.SelectedIndex = 0;
                        }

                        cmbx_Source.RemoveAt(i);
                        cmbx_Parent.SelectedIndex = 0;
                    }
                }
            }

            if (cmbx_Parent.ItemsSource is ObservableCollection<Probe>)
            {
                ObservableCollection<Probe> cmbx_Source = (ObservableCollection<Probe>)cmbx_Parent.ItemsSource;

                Probe cur_Probe = (Probe)obj_SelectedItem;

                for (int i = 0; i < cmbx_Source.Count; i++)
                {
                    if (cur_Probe.Name == cmbx_Source[i].Name)
                    {
                        if (i == 0 && cmbx_Source.Count > 1)
                        {
                            cmbx_Parent.SelectedIndex = 1;
                        }
                        else if (cmbx_Source.Count > 1)
                        {
                            cmbx_Parent.SelectedIndex = 0;
                        }

                        cmbx_Source.RemoveAt(i);
                        cmbx_Parent.SelectedIndex = 0;
                    }
                }
            }
            //cmbx_Itemsource.RemoveAt(1);
        }
    }
}
