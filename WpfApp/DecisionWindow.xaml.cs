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
    public partial class DecisionWindow : Window
    {
        public delegate void decision(string currentString);
        decision dec;
        public DecisionWindow(string[] input)
        {
            InitializeComponent();
            sign.Items.Add("<=");
            sign.Items.Add(">=");
            sign.Items.Add("==");
            sign.Items.Add("<");
            sign.Items.Add(">");
            if (input != null)
            {
                leftNum.Text = input[0].Replace(" ", "");
                rightNum.Text = input[2].Replace(" ", "");
                sign.Text = input[1];
            }
            else
            {
                leftNum.Text = "";
                rightNum.Text = "";
                sign.Text = "";
            }
        }

        public void Reg_Decision_Del(decision dec)
        {
            this.dec = dec;
        }

        private void Ok_Button_Click(object sender, RoutedEventArgs e)
        {
            string[] output = new String_Nodes_Methods_Container().Get_Elements_For_Decision(leftNum.Text + sign.Text + rightNum.Text);
            dec?.Invoke(output[0] + " " + output[1] + " " + output[2]);
            this.Close();
        }
    }
}