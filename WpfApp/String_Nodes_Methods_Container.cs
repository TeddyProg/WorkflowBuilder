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
        string letters = "abcdefghijklmnopqrstuvwxyzабвгдеёжзийклмнопрстуфхцчшщъыьэюяії";
        string numbers = "1234567890.,";
        string symbols = "<>=";
        public string[] Get_Elements_For_Decision(string input)
        {
            if (input.Length > 0)
            {

                string[] a_sign_b = new string[3];
                a_sign_b[0] = "";
                a_sign_b[1] = "";
                a_sign_b[2] = "";
                foreach (char c in input)
                {
                    if (!(letters.Contains(c) || letters.Contains(c.ToString().ToLower())) && !(symbols.Contains(c) || symbols.Contains(c.ToString().ToLower())) && !(numbers.Contains(c) || numbers.Contains(c.ToString().ToLower())))
                    {
                        if (input != "Decision") MessageBox.Show("Wrong format!");
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
                if (a_sign_b[2].Length > 0 && a_sign_b[0].Length > 0) return a_sign_b;
                else if (a_sign_b[0] == "Decision") return new string[] { "0", "==", "0" };
                else return new string[] { "", "", "" };
            }
            else
            {
                return new string[] { "", "", "" };
            }
        }

        public string Verifying_Declare_InOut_Format(string input)
        {
            string filtInput = "";
            if (input.Contains("Input: ")) filtInput = input.Replace("Input: ", "");
            else if (input.Contains("Print: ")) filtInput = input.Replace("Print: ", "");
            else if (input.Contains("Declare: ")) filtInput = input.Replace("Declare: ", "");
            if (filtInput.Length > 0)
            {
                for (int i = 0; i < input.Length; i++)
                {
                    if ((i < input.Length - 1 && input[i] == ',' && input[i + 1] != ' ') || (input[input.Length - 1] == ',' || input[input.Length - 1] == ' '))
                    {
                        System.Windows.MessageBox.Show("Wrong format!");
                        return "";
                    }
                }
                string[] a = filtInput.Split(',');
                if (a.Length > 1)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        if (a[i][0] != ' ')
                        {
                            a[i] = " " + a[i];
                        }
                    }
                }
                for (int i = 0; i < a.Length; i++)
                {
                    for (int j = i + 1; j < a.Length; j++)
                    {
                        if (a[i] == a[j])
                        {
                            System.Windows.MessageBox.Show("Variables must have different names");
                            return "";
                        }
                    }
                }
                return filtInput;
            }
            else
            {
                return "";
            }
        }

        public string Verifying_Assign_Fromat(string input)
        {
            string[] splitted = input.Split(' ');
            if (splitted.Length > 3)
            {
                MessageBox.Show("Incorrect variable name");
                return "";
            }
            if (input.Length > 0)
            {
                if (splitted[0].Contains(' ') || splitted[2].Contains(' '))
                {
                    MessageBox.Show("Incorrect variable name");
                    return "";
                }
                for (int i = 0; i < splitted[0].Length; i++)
                {
                    if (isInList(splitted[0][i], letters)) return (splitted[0] + " " + splitted[1] + " " + splitted[2]);
                    else { MessageBox.Show("Incorrect variable name"); return ""; };
                }
            }
            return "";
        }

        private bool isInList(char c, string input)
        {
            foreach (char ch in input)
            {
                if (ch == c) return true;
                else if (ch == Convert.ToChar(c.ToString().ToLower())) return true;
            }
            return false;
        }
    }
}