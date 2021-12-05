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

namespace WpfApp
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class AssignWindow : Window
    {
        public delegate void assign(string input);
        assign output;
        string lastField;
        public AssignWindow(string input)
        {
            InitializeComponent();
            string[] els = input.Split(' ');
            lastField = input;
            if (els.Length>1)
            {
                VariableBox.Text = els[0];
                Sign.Content = els[1];
                AssignBox.Text = els[2];
            }
            else
            {
                VariableBox.Text = "";
                Sign.Content = "=";
                AssignBox.Text = "";
            }
        }

        public void Reg_Assign_Del(assign output)
        {
            this.output = output;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (new String_Nodes_Methods_Container().Verifying_Assign_Fromat(VariableBox.Text + " = " + AssignBox.Text) != "") output?.Invoke(VariableBox.Text + " = " + AssignBox.Text);
            else output?.Invoke(lastField);
            this.Close();
        }
    }
}
