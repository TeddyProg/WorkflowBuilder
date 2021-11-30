using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MindFusion.Diagramming.Wpf;
using System.IO;

namespace WpfApp
{
    enum NodeType
    {
        Begin,
        End,
        Declare,
        Assign,
        Print,
        Unknown
    }



    class ShapeFactory
    {
        private static ShapeNode CreateNode(Shapes s, string tag, string text, TextAlignment txtAl, StringFormat strFormat)
        {
            return null;
        }

        public static ShapeNode CreateNode(NodeType type)
        { 
            StringFormat strFormat = new StringFormat();
            strFormat.LineAlignment = StringAlignment.Center;
            strFormat.Alignment = StringAlignment.Center;

            switch (type)
            {
                case NodeType.Begin:
                    return new ShapeNode {
                        Shape = Shapes.Terminator,
                        TextFormat = strFormat,
                        Tag = NodeType.Begin,
                        Text = "Begin",
                        ToolTip = NodeType.Begin.ToString()
                    };
                case NodeType.Declare:
                    return new ShapeNode
                    {
                        Shape = Shapes.RSave,
                        TextFormat = strFormat,
                        Tag = NodeType.Declare,
                        Text = "varName"
                    };
                case NodeType.Assign:
                    return new ShapeNode
                    {
                        Shape = Shapes.Rectangle,
                        TextFormat = strFormat,
                        Tag = NodeType.Assign,
                        Text = "varName = 0"
                    };
                case NodeType.Print:
                    return new ShapeNode
                    {
                        Shape = Shapes.RoundRect,
                        TextFormat = strFormat,
                        Tag = NodeType.Print,
                        Text = "varName"
                    };
                case NodeType.End:
                    return new ShapeNode
                    {
                        Shape = Shapes.Terminator,
                        TextFormat = strFormat,
                        Tag = NodeType.End,
                        Text = "End"
                    };
                case NodeType.Unknown:
                default:
                    return null;
            }

        }
    }
}
