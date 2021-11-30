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


    class WorkflowAnalyzer
    {

        private static NodeType GetNodeTypeFromTag(object tag)
        {
            return (NodeType)(int)tag;
        }

        /// <summary>
        /// Check whether all nodes of given type has exact needed number of incoming and outgoing links
        /// </summary>
        /// <param name="diag">Diagram to analyze</param>
        /// <param name="typeToCheck">Type to check in diagram</param>
        /// <param name="numIncLinks">Num of links which should be incoming to node</param>
        /// <param name="numOutLinks">Num of links which should be outgoing from node</param>
        /// <returns>If all nodes of given type has correct number of links - true, false otherwise</returns>
        private static bool CheckNumLinksOfNode(Diagram diag, NodeType typeToCheck, int numIncLinks, int numOutLinks)
        {
            var nodes = diag.Nodes;
            foreach (var node in nodes)
            {
                if (node.Tag != null)
                {
                    NodeType nodeType = GetNodeTypeFromTag(node.Tag);
                    if (nodeType == typeToCheck)
                    {
                        if (node.IncomingLinks.Count != numIncLinks || node.OutgoingLinks.Count != numOutLinks)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public static bool ValidateBlockDiagram(Diagram diag)
        {
            ///Checking that number of begin and end nodes equals 1
            {
                var nodes = diag.Nodes;
                uint numBegs = 0;
                uint numEnds = 0;
                foreach (var node in nodes)
                {
                    if (node.Tag != null)
                    {
                        NodeType tagStr = GetNodeTypeFromTag(node.Tag);
                        if (tagStr == NodeType.End)
                            numEnds++;
                        else if (tagStr == NodeType.Begin)
                            numBegs++;
                    }
                }
                if (numBegs != 1 || numEnds != 1)
                    return false;
            }
            ////////////////////////////////////////////////////

            ///Checking that there is only one link from starting node and no link into it
            { 
                if (!CheckNumLinksOfNode(diag, NodeType.Begin, 0, 1))
                    return false;

            }
            ////////////////////////////////////////////////////////

            ///Checking that there is only one link into ending node and no link from it
            {
                if (!CheckNumLinksOfNode(diag, NodeType.End, 1, 0))
                    return false;
            }
            ////////////////////////////////////////////////////////

            ///Checking that num of incoming and outgoing links of print nodes equals 1
            {
                if (!CheckNumLinksOfNode(diag, NodeType.Print, 1, 1))
                    return false;
            }
            ////////////////////////////////////////////////////

            ///Checking that num of incoming and outgoing links of assign nodes equals 1
            {
                if (!CheckNumLinksOfNode(diag, NodeType.Assign, 1, 1))
                    return false;
            }
            ////////////////////////////////////////////////////

            ///Checking that num of incoming and outgoing links of declare nodes equals 1
            {
                if (!CheckNumLinksOfNode(diag, NodeType.Declare, 1, 1))
                    return false;
            }
            ////////////////////////////////////////////////////

            return true;
        }

        private static bool ParseDeclareText(string decNodeText, out string varName)
        {
            var args = decNodeText.Split(' ');
            varName = "/*incorrect variable name*/";
            if (args.Length != 2)
                return false;
            if (args[0].ToLower() != "declare:")
                return false;
            varName = args[1];
            return true;
        }

        private static bool ParseInputText(string decNodeText, out string varName)
        {
            var args = decNodeText.Split(' ');
            varName = "/*incorrect variable name*/";
            if (args.Length != 2)
                return false;
            if (args[0].ToLower() != "input:")
                return false;
            varName = args[1];
            return true;
        }

        private static bool ParseAssignText(string decNodeText, out string varName)
        {
            var args = decNodeText.Split(' ');
            varName = "/*incorrect variable name*/";
            if (args.Length != 3)
                return false;
            if (args[1] != "=")
                return false;
            varName = args[1];
            return true;
        }

        public string MakeProgram(Diagram diag, string progName = "program")
        {
            if (!ValidateBlockDiagram(diag))
            {
                return "//Incorrect diagram!";
            }

            BeginNode startNode = new BeginNode();
            startNode.SetProgramName(progName);
            
            var curDiagNode = diag.FindNode(NodeType.Begin);
            curDiagNode = curDiagNode.OutgoingLinks[0].Destination; // going to next node
            CodeNode curCodeNode = startNode;
            string tmpString;
            while (curDiagNode != null)
            {
                NodeType type = GetNodeTypeFromTag(curDiagNode.Tag);
                switch (type)
                {
                    // it means that diagram is wrong, should show it some way
                    case NodeType.Begin:
                        curCodeNode.nextNode = new CommentNode("Unexpected begin node");
                        break;
                    case NodeType.End:
                        curCodeNode.nextNode = new EndNode();
                        break;
                    case NodeType.Declare:
                        ParseDeclareText(curDiagNode.Text, out tmpString);
                        curCodeNode.nextNode = new DeclareNode()
                        break;
                    case NodeType.Assign:
                        break;
                    case NodeType.Print:
                        break;
                    case NodeType.Unknown:
                        break;
                    default:
                        break;
                }
                curCodeNode = curCodeNode.nextNode;
            }

            return "";
        }
    }
}
