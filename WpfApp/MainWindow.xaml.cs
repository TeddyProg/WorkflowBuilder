using System;
using System.Collections.Generic;
using System.Windows;
using MindFusion.Diagramming.Wpf;
using System.IO;
using System.Windows.Forms;
using MindFusion.Diagramming.Wpf.Layout;


namespace WpfApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //MindFusion.Licensing.LicenseManager.AddLicense()
            //shapeList.Items.Add(new ShapeNode { Shape = Shapes.RoundRect, Bounds = new Rect(0, 0, 40, 40) });
            StringFormat strFormat = new StringFormat();
            strFormat.LineAlignment = StringAlignment.Center;
            strFormat.Alignment = StringAlignment.Center;
            shapeList.Items.Add(ShapeFactory.CreateNode(NodeType.Begin));
            shapeList.Items.Add(ShapeFactory.CreateNode(NodeType.End));
            shapeList.Items.Add(ShapeFactory.CreateNode(NodeType.Assign));
            shapeList.Items.Add(ShapeFactory.CreateNode(NodeType.Declare));
            shapeList.Items.Add(ShapeFactory.CreateNode(NodeType.Print));


            if (!System.IO.Directory.Exists(DirPath))
            {
                System.IO.Directory.CreateDirectory(DirPath);
            }
            
        }

        string FileName = "";
        string DirPath = "Diagrams/";
        List<string> paths = new List<string>();

        private void Add_Diagram_Click(object sender, RoutedEventArgs e)
        {
            FileName = CurrentFileName.Text.Contains(".xml") ? CurrentFileName.Text : CurrentFileName.Text + ".xml";
            Flowchart.SaveToXml(DirPath + FileName);
            if (!DiagramListBox.Items.Contains(FileName))
            {
                OutputLabel.Content = $"Successfully saved as {FileName}.xml";
                DiagramListBox.Items.Add(FileName);
            }
            else OutputLabel.Content = $"{FileName} was successfully edited";
            Flowchart.ClearAll();
        }

        private void Remove_Diagram_Click(object sender, RoutedEventArgs e)
        {
            if (DiagramListBox.SelectedItem != null)
            {
                FileName = DiagramListBox.SelectedItem.ToString().Contains(".xml") ? DiagramListBox.SelectedItem.ToString() : DiagramListBox.SelectedItem.ToString() + ".xml";
                File.Delete(DirPath + FileName);
                DiagramListBox.Items.Remove(FileName);
                foreach (string s in paths)
                {
                    if (s.Contains(FileName))
                    {
                        File.Delete(s);
                        paths.Remove(s);
                        break;
                    }
                }
                Flowchart.ClearAll();
            }
            else
            {
                OutputLabel.Content = "First select item";
            }
        }
        private void Generate_Code_Click(object sender, RoutedEventArgs e)
        { 
            var layout = new MindFusion.Diagramming.Wpf.Layout.DecisionLayout();
            layout.HorizontalPadding = 40;
            layout.VerticalPadding = 40;
            layout.StartNode = Flowchart.FindNode("Begin");
            layout.Anchoring = Anchoring.Keep;
            layout.Arrange(Flowchart);

            if (WorkflowAnalyzer.ValidateBlockDiagram(Flowchart))
            {
                OutputLabel.Content = "Good";
            }
            else
                OutputLabel.Content = "Hui";

            //var node = Flowchart.FindNode("Decision");
            //if (node != null)
            //{
            //    var links = node.GetAllOutgoingLinks();
            //    for (int i = 0; i < links.Count; ++i)
            //    {
            //        var link = links[i];
            //        int ind = link.OriginIndex;
            //        var anchorPos = link.OriginConnection.GetAnchorPos(ind);
            //        var type = anchorPos.GetType();
            //        //node.GetAnchorPos(i).GetType();
            //    }
            //    node.GetAllLinks();
            //}
            //int index = Flowchart.Links[0].OriginIndex;
            //Flowchart.Links[0].OriginConnection.GetAnchorPos(index).GetType();
        }

        private void Mouse_Double_Click(object sender, RoutedEventArgs e)
        {
            if (DiagramListBox.SelectedItem != null)
            {
                string filepath = "";
                bool isinList = false;
                FileName = DiagramListBox.SelectedItem.ToString().Contains(".xml") ? DiagramListBox.SelectedItem.ToString() : DiagramListBox.SelectedItem.ToString() + ".xml";
                foreach (string s in paths)
                {
                    if (s.Contains(FileName))
                    {
                        isinList = true;
                        filepath = s;
                        break;
                    }
                }
                if (!isinList)
                {
                    Flowchart.LoadFromXml(FileName);
                    CurrentFileName.Text = FileName;
                }
                else
                {
                    Flowchart.LoadFromXml(filepath);
                }
            }
        }

        private void Change_Dir_Click(object sender, RoutedEventArgs e)
        {
            Get_Dir_Path();
        }

        private void Load_All__Click(object sender, RoutedEventArgs e)
        {
            string[] files = { };
            if (DirPath != "") files = Directory.GetFiles(DirPath);
            else OutputLabel.Content = "Error";
            DiagramListBox.Items.Clear();
            foreach (string f in files)
            {
                if (f.Contains(".xml"))
                {
                    string line = "";
                    for (int i = 0; i < f.Length; i++)
                    {
                        if (f[i] == '\\')
                        {
                            line = "";
                            continue;
                        }
                        line += f[i];
                    }
                    DiagramListBox.Items.Add(line);
                }
            }
        }

        private void Select_File__Click(object sender, RoutedEventArgs e)
        {
            string filepath = Get_File_Path();
            if (!paths.Contains(filepath)) paths.Add(filepath);
            string filename = "";
            foreach (char c in filepath)
            {
                if (c == '\\')
                {
                    filename = "";
                    continue;
                }
                filename += c;
            }
            if (!DiagramListBox.Items.Contains(filename))
            {
                DiagramListBox.Items.Add(filename);
                Flowchart.LoadFromXml(filepath);
            }
            else
            {
                OutputLabel.Content = "Already in list";
            }
        }

        private string Get_File_Path()
        {
            System.Windows.Forms.OpenFileDialog openFileDlg = new System.Windows.Forms.OpenFileDialog();
            var result = openFileDlg.ShowDialog();
            //System.Windows.MessageBox.Show(openFileDlg.FileName);
            if (result.ToString() != string.Empty && openFileDlg.FileName.Contains(".xml"))
            {
                return openFileDlg.FileName;
            }
            else;
            {
                return "None";
                OutputLabel.Content = "Error";
            }
        }

        private void Get_Dir_Path()
        {
            System.Windows.Forms.FolderBrowserDialog openFileDlg = new System.Windows.Forms.FolderBrowserDialog();
            var result = openFileDlg.ShowDialog();
            if (result.ToString() != string.Empty)
            {
                DirPath = openFileDlg.SelectedPath + "\\";
            }
            if (DirPath == "\\") DirPath = "";
        }
    }
}
