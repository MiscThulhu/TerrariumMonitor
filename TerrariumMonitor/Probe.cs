using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerrariumMonitor
{
    public class Probe
    {
        public string Name { get; set; }
        public string Units { get; set; }

        public Probe Clone_Probe()
        {
            Probe p_Clone = (Probe)this.MemberwiseClone();

            return p_Clone;
        }
    }
}
