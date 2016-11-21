using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerrariumMonitor
{
    class Error
    {
        public int Catergory { get; set; }
        public string Node { get; set; }
        public string Attribute { get; set; }
        public int Position { get; set; }
        private string[] str_Catergories = { "Node out of position", "Missing Node", "Incorrect or Missing Attribute value", "Missing Attribute", "Duplicate Node", "Duplicate Attrabute" };

        public string Get_Report()
        {
            string str_Report = "";

            if (Catergory != 2 && Catergory != 5)
            {
                str_Report = str_Catergories[Catergory] + " (" + Node;

                if (Attribute == null)
                {
                    str_Report = str_Report + " At line: " + Position;
                }
                else
                {
                    str_Report = str_Report + ", Attribute: " + Attribute + "). At line: " + Position;
                }
            }
            else
            {
                str_Report = str_Catergories[Catergory] + " (" + Attribute + ") on Node (" + Node + ") at line: " + Position;
            }
            return str_Report;
        }
    }
}
