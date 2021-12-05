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
using MindFusion.Diagramming.Wpf;
using System.IO;
using System.Windows.Forms;

namespace WpfApp
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            Flowchart.AllowInplaceEdit = false;

            //shapeList.Items.Add(new ShapeNode { Shape = Shapes.Rectangle });
            StringFormat strFormat = new StringFormat();
            strFormat.LineAlignment = StringAlignment.Center;
            strFormat.Alignment = StringAlignment.Center;
            shapeList.Items.Add(ShapeFactory.CreateNode(NodeType.Begin));
            shapeList.Items.Add(ShapeFactory.CreateNode(NodeType.End));
            shapeList.Items.Add(ShapeFactory.CreateNode(NodeType.Assign));
            shapeList.Items.Add(ShapeFactory.CreateNode(NodeType.Declare));
            shapeList.Items.Add(ShapeFactory.CreateNode(NodeType.Print));
            shapeList.Items.Add(ShapeFactory.CreateNode(NodeType.Input));
            shapeList.Items.Add(ShapeFactory.CreateNode(NodeType.Decision));

        }
        List<string> AllPaths = new List<string>();
        OpenFileDialog ofd = new OpenFileDialog(); //Для открытия файлов
        FolderBrowserDialog fbd = new FolderBrowserDialog(); //Для выбора папки


        private void Save_Diagram_Click(object sender, RoutedEventArgs e)
        {
            Button_Click_Effect(ref sender);
            string filename = CurrentFileName.Text.Contains(".xml") ? CurrentFileName.Text : CurrentFileName.Text + ".xml";
            System.Windows.Forms.DialogResult result = System.Windows.Forms.DialogResult.No;
            if (DiagramListBox.SelectedItem == null || !DiagramListBox.SelectedItem.ToString().Contains(CurrentFileName.Text)) result = fbd.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK && !DiagramListBox.Items.Contains(filename))
            {
                string filepath = fbd.SelectedPath + "\\" + filename;
                Flowchart.SaveToXml(filepath);
                Flowchart.ClearAll();
                AllPaths.Add(filepath);
                DiagramListBox.Items.Add(Get_File_Name(filename));
                OutputLabel.Content = "Successfully saved";
            }
            else if (DiagramListBox.Items.Contains(filename))
            {
                string filepath = "";
                foreach (string s in AllPaths)
                {
                    if (s.Contains(filename)) filepath = s;
                }
                if (filepath.Length > 0)
                {
                    Flowchart.SaveToXml(filepath);
                    Flowchart.ClearAll();
                    CurrentFileName.Text = ".xml";
                    OutputLabel.Content = "Already in list\nSuccessfully changed";
                }
                else { OutputLabel.Content = "Error"; }
            }
            else
            {
                OutputLabel.Content = "First select path";
            }
        }

        private void Remove_Diagram_Click(object sender, RoutedEventArgs e)
        {
            Button_Click_Effect(ref sender);
            if (DiagramListBox.SelectedItem != null)
            {
                foreach (string s in AllPaths)
                {
                    if (s.Contains(DiagramListBox.SelectedItem.ToString()))
                    {
                        AllPaths.Remove(s);
                        DiagramListBox.Items.Remove(Get_File_Name(s));
                        break;
                    }
                }
            }
        }

        private void Generate_Code_Click(object sender, RoutedEventArgs e)
        {
            Button_Click_Effect(ref sender);
            var layout = new MindFusion.Diagramming.Wpf.Layout.DecisionLayout();
            layout.HorizontalPadding = 40;
            layout.VerticalPadding = 40;
            layout.StartNode = Flowchart.FindNode(NodeType.Begin);
            layout.Anchoring = Anchoring.Keep;
            layout.Arrange(Flowchart);

            if (WorkflowValidator.ValidateBlockDiagram(Flowchart))
            {
                OutputLabel.Content = "Good";
            }
            else
                OutputLabel.Content = "Bad";

            //string code = WorkflowAnalyzer.MakeProgram(Flowchart, DiagramListBox.SelectedItem.ToString() + "_program");
            List<Diagram> diags = new List<Diagram>()
            {
                Flowchart
            };
            string code = WorkflowAnalyzer.MakeProgram(diags);
            //FileName = FileName.Replace('\\', '_').Replace('/', '_');
            fbd.ShowDialog();
            string path = $"{fbd.SelectedPath}/{DiagramListBox.SelectedItem.ToString()}.txt";

            // The line below will create a text file, my_file.txt, in 
            // the Text_Files folder in D:\ drive.
            // The CreateText method that returns a StreamWriter object
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.Write(code);
            }

            var links = Flowchart.Links;
            List<int> inds = new List<int>();
            foreach (var link in links)
            {
                inds.Add(link.OriginIndex);
                inds.Add(link.DestinationIndex);

            }
            int a = 0;
        }

        private void Mouse_Double_Click(object sender, RoutedEventArgs e)
        {
            if (DiagramListBox.SelectedItem != null)
            {
                CurrentFileName.Text = DiagramListBox.SelectedItem.ToString();
                foreach (string s in AllPaths)
                {
                    if (s.Contains(DiagramListBox.SelectedItem.ToString()))
                    {
                        Flowchart.ClearAll();
                        Flowchart.LoadFromXml(s);
                    }
                }
            }
            else
            {
                Flowchart.ClearAll();
                CurrentFileName.Clear();
            }
        }

        private void Select_Files__Click(object sender, RoutedEventArgs e)
        {
            Button_Click_Effect(ref sender);
            ofd.Multiselect = true;
            ofd.Filter = "XML Files (*.xml)|*.xml";
            ofd.FilterIndex = 0;
            ofd.ShowDialog();
            foreach (string s in ofd.FileNames)
            {
                string filename = Get_File_Name(s);
                if (!DiagramListBox.Items.Contains(filename))
                {
                    DiagramListBox.Items.Add(filename);
                    AllPaths.Add(s);
                }
            }
        }

        private void Delete_File_Click(object sender, RoutedEventArgs e)
        {
            Button_Click_Effect(ref sender);
            foreach (string s in AllPaths)
            {
                if (s.Contains(DiagramListBox.SelectedItem.ToString()))
                {
                    File.Delete(s);
                    DiagramListBox.Items.Remove(DiagramListBox.SelectedItem.ToString());
                    AllPaths.Remove(s);
                    OutputLabel.Content = "Deleted";
                    break;
                }
            }
        }

        bool isDecisionOpened = false;

        private void Flowchart_NodeDoubleClicked(object sender, NodeEventArgs e)
        {

            String_Nodes_Methods_Container snmc = new String_Nodes_Methods_Container();
            NodeType type = WorkflowUtils.GetNodeTypeFromTag(e.Node.Tag);
            switch (type)
            {
                case NodeType.Decision:
                    if (isDecisionOpened == false)
                    {
                        DecisionWindow w = new DecisionWindow(new String_Nodes_Methods_Container().Get_Elements_For_Decision(e.Node.Text));
                        w.Reg_Decision_Del(set_node_string);
                        common_case_parameters((Window)w);
                    }
                    break;
                case NodeType.Input:
                    if (isDecisionOpened == false)
                    {
                        InOutWindow iow = new InOutWindow("Input: ", new String_Nodes_Methods_Container().Verifying_Declare_InOut_Format(e.Node.Text));
                        iow.Reg_InOut_Del(set_node_string);
                        common_case_parameters((Window)iow);
                    }
                    break;
                case NodeType.Print:
                    if (isDecisionOpened == false)
                    {
                        InOutWindow iow = new InOutWindow("Print: ", new String_Nodes_Methods_Container().Verifying_Declare_InOut_Format(e.Node.Text));
                        iow.Reg_InOut_Del(set_node_string);
                        common_case_parameters((Window)iow);
                    }
                    break;
                case NodeType.Declare:
                    if (isDecisionOpened==false)
                    {
                        InOutWindow iow = new InOutWindow("Declare: ", new String_Nodes_Methods_Container().Verifying_Declare_InOut_Format(e.Node.Text));
                        iow.Reg_InOut_Del(set_node_string);
                        common_case_parameters((Window)iow);
                    }
                    break;
                case NodeType.Assign:
                    if (isDecisionOpened == false)
                    {
                        AssignWindow aw = new AssignWindow(e.Node.Text);
                        aw.Reg_Assign_Del(set_node_string);
                        common_case_parameters((Window)aw);
                    }
                    break;
                default:
                    System.Windows.MessageBox.Show("Undefined node!");
                    break;
            }

            void set_node_string(string input)
            {
                isDecisionOpened = false;
                e.Node.Text = input;
            }

            void common_case_parameters(Window window)
            {
                isDecisionOpened = true;
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                window.Topmost = true;
                window.Show();
            }
        }

        private static void Button_Click_Effect(ref object sender) //Баловался с эффектом при нажатии, еще не разобрался
        {
            System.Windows.Controls.Button button = sender as System.Windows.Controls.Button;
            //button.Effect = new System.Windows.Media.Effects.DropShadowEffect()
            //{

            //};
        }

        private string Get_File_Name(string path) //Выделяет имя файла из строки пути к файлу
        {
            string filename = "";
            foreach (char c in path)
            {
                if (c == '\\')
                {
                    filename = "";
                    continue;
                }
                filename += c;
            }
            return filename;
        }
    }
}
