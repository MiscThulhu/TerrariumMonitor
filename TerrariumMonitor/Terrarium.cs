using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace TerrariumMonitor
{
    public class Terrarium
    {
        public string Name { get; set; }

        ObservableCollection<Probe> lst_Probes = new ObservableCollection<Probe>();

        #region Public Methods
        #region Cone
        public Terrarium Clone_Terrarium()
        {
            Terrarium t_Clone = new Terrarium();

            t_Clone.Name = this.Name;

            foreach (Probe cur_Probe in lst_Probes)
            {
                t_Clone.Add_Probe(cur_Probe.Name, cur_Probe.Units);
            }

            return t_Clone;
        }
        #endregion
        #region Probes
        public void Add_Probe(string str_Name, string str_Units)
        {
            lst_Probes.Add(new Probe());
            lst_Probes[lst_Probes.Count - 1].Name = str_Name;
            lst_Probes[lst_Probes.Count - 1].Units = str_Units;
        }

        public ObservableCollection<Probe> Get_Probes()
        {
            return lst_Probes;
        }

        public void Set_Probes(ObservableCollection<Probe> new_List)
        {
            lst_Probes = new_List;
        }

        public Probe Get_Probe(int i_Index)
        {
            return lst_Probes[i_Index];
        }
        #endregion
        #endregion
    }
}
