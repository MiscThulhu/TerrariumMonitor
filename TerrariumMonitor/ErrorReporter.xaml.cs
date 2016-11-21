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

namespace TerrariumMonitor
{
    /// <summary>
    /// Interaction logic for ErrorReporter.xaml
    /// </summary>
    public partial class ErrorReporter : UserControl
    {
        List<Error> lst_Errors = new List<Error>();

        public ErrorReporter()
        {
            InitializeComponent();
        }

        public void Add_Error(int i_Catrgory, string str_Node, string str_Attribute, int i_Position)
        {
            lst_Errors.Add(new Error());
            lst_Errors[lst_Errors.Count - 1].Catergory = i_Catrgory;
            lst_Errors[lst_Errors.Count - 1].Node = str_Node;
            lst_Errors[lst_Errors.Count - 1].Attribute = str_Attribute;
            lst_Errors[lst_Errors.Count - 1].Position = i_Position;
        }

        public void Add_Error(int i_Catrgory, string str_Node, int i_Position)
        {
            lst_Errors.Add(new Error());
            lst_Errors[lst_Errors.Count - 1].Catergory = i_Catrgory;
            lst_Errors[lst_Errors.Count - 1].Node = str_Node;
            lst_Errors[lst_Errors.Count - 1].Position = i_Position;
        }

        public void Report_Errors()
        {
            foreach (Error cur_Error in lst_Errors)
            {
                tB_ErrorReport.Text += System.Environment.NewLine + cur_Error.Get_Report();
            }
        }
    }
}
