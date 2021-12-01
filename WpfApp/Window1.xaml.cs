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
    public partial class Window1 : Window
    {
        public delegate void decision(string currentString);
        decision dec;
        public Window1(string[] input)
        {
            InitializeComponent();
            sign.Items.Add("<=");
            sign.Items.Add(">=");
            sign.Items.Add("==");
            sign.Items.Add("<");
            sign.Items.Add(">");
            if (input != null)
            {
                leftNum.Text = input[0];
                rightNum.Text = input[2];
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
            dec?.Invoke((leftNum.Text.Length <= 0 ? "0" : leftNum.Text) + (sign.SelectedItem == null ? "==" : sign.SelectedItem.ToString()) + (rightNum.Text.Length <= 0 ? "0" : rightNum.Text));
            this.Close();
        }
    }
}
