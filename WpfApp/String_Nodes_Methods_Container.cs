using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp
{
    class String_Nodes_Methods_Container
    {
        public string[] Get_Elements_For_Decision(string input)
        {
            if (input.Length > 0)
            {
                string numbers = "1234567890.,";
                string symbols = "<>=";
                string[] a_sign_b = new string[3];
                a_sign_b[0] = "";
                a_sign_b[1] = "";
                a_sign_b[2] = "";
                foreach (char c in input)
                {
                    if (!numbers.Contains(c) && !symbols.Contains(c))
                    {
                        MessageBox.Show("Wrong format!");
                        return new string[] { "0", "==", "0" };
                    }
                }
                bool isAfterSymbol = false;
                foreach (char c in input)
                {
                    if (symbols.Contains(c)) isAfterSymbol = true;
                    if (!isAfterSymbol) a_sign_b[0] += c;
                    if (isAfterSymbol && symbols.Contains(c)) a_sign_b[1] += c;
                    if (isAfterSymbol && !symbols.Contains(c)) a_sign_b[2] += c;
                }
                return a_sign_b;
            }
            else
            {
                return new string[] { "", "", "" };
            }
        }
    }
}

