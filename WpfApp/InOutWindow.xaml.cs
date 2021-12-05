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
    /// Логика взаимодействия для Window2.xaml
    /// </summary>
    /// 
    public partial class InOutWindow : Window
    {
        public string members { get; set; }

        public delegate void inout(string input);
        inout InOut;
        string lastField;

        public InOutWindow(string type, string members)
        {
            InitializeComponent();
            TypeLabel.Content = type;
            ContentBox.Text = members;
            lastField = ContentBox.Text;
            this.Title = type.Replace(": ", "");
        }

        public void Reg_InOut_Del(inout io)
        {
            InOut = io;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (new String_Nodes_Methods_Container().Verifying_Declare_InOut_Format(TypeLabel.Content + ContentBox.Text) != "") InOut?.Invoke(TypeLabel.Content + ContentBox.Text);
            else InOut?.Invoke(TypeLabel.Content+lastField);
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (new String_Nodes_Methods_Container().Verifying_Declare_InOut_Format(TypeLabel.Content + ContentBox.Text) != "") InOut?.Invoke(TypeLabel.Content + ContentBox.Text);
            else InOut?.Invoke(TypeLabel.Content + lastField);
        }
    }
}
