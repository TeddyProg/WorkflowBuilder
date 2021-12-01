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

        private static bool ParseInputText(string inptNodeText, out string varName)
        {
            var args = inptNodeText.Split(' ');
            varName = "/*incorrect variable name*/";
            if (args.Length != 2)
                return false;
            if (args[0].ToLower() != "input:")
                return false;
            varName = args[1];
            return true;
        }

        private static bool ParseAssignText(string assgnNodeText, out string varName, out int varValue)
        {
            var args = assgnNodeText.Split(' ');
            varValue = -1;
            varName = "/*incorrect variable name*/";
            if (args.Length != 3)
                return false;
            if (args[1] != "=")
                return false;
            varName = args[0];
            varValue = Convert.ToInt32(args[2]);
            return true;
        }

        private static bool ParsePrintText(string prntNodeText, out string varName)
        {
            var args = prntNodeText.Split(' ');
            varName = "/*incorrect variable name*/";
            if (args.Length != 2)
                return false;
            if (args[0].ToLower() != "print:")
                return false;
            varName = args[1];
            return true;
        }

        private static bool ParseDecisionText(string cndNodeText, out string condText)
        {
            condText = cndNodeText;
            return true;
        }

        private static string MakeTextFromCodeNodes(CodeNode startNode)
        {
            string res = "";

            var curNode = startNode;
            while(curNode != null)
            {
                res += curNode.SharpCode + '\n';
                curNode = curNode.nextNode;
            }

            return res;
        }

        private static CodeNode GetCodeNode(DiagramNode dNode)
        {
            NodeType type = WorkflowUtils.GetNodeTypeFromTag(dNode.Tag);
            CodeNode resNode;
            switch (type)
            {
                case NodeType.Begin:
                    resNode = new BeginNode();
                    break;
                case NodeType.End:
                    resNode = new EndNode();
                    break;
                case NodeType.Declare:
                    {
                        string decVar;
                        ParseDeclareText(dNode.Text, out decVar);
                        resNode = new DeclareNode(decVar);
                        break;
                    }
                case NodeType.Assign:
                    {
                        string assignVar;
                        int varValue;
                        ParseAssignText(dNode.Text, out assignVar, out varValue);
                        resNode = new AssignNode(assignVar, varValue);
                        break;
                    }
                case NodeType.Print:
                    {
                        string outputVar;
                        ParsePrintText(dNode.Text, out outputVar);
                        resNode = new PrintNode(outputVar);
                        break;
                    }
                case NodeType.Input:
                    {
                        string inputVar;
                        ParseInputText(dNode.Text, out inputVar);
                        resNode = new InputNode(inputVar);
                        break;

                    }
                case NodeType.Decision:
                    {
                        string cond;
                        ParseDecisionText(dNode.Text, out cond);
                        resNode = new ConditionNode(cond);
                        break;
                    }
                case NodeType.Unknown:
                default:
                    resNode = new CommentNode("Unknown node");
                    break;
            }
            return resNode;
        }

        private static CodeNode MakeCodeSequence(DiagramNode dNode)
        {
            CodeNode resNode = GetCodeNode(dNode);
            CodeNode curCodeNode = resNode; 
            while(dNode != null)
            {
                switch (curCodeNode.Type)
                {
                    case CodeType.Begin:
                        dNode = dNode.OutgoingLinks[0].Destination;
                        curCodeNode.nextNode = GetCodeNode(dNode);
                        break;
                    case CodeType.End:
                        dNode = null;
                        break;
                    case CodeType.Print:
                        dNode = dNode.OutgoingLinks[0].Destination;
                        curCodeNode.nextNode = GetCodeNode(dNode);
                        break;
                    case CodeType.Declare:
                        dNode = dNode.OutgoingLinks[0].Destination;
                        curCodeNode.nextNode = GetCodeNode(dNode);
                        break;
                    case CodeType.Input:
                        dNode = dNode.OutgoingLinks[0].Destination;
                        curCodeNode.nextNode = GetCodeNode(dNode);
                        break;
                    case CodeType.Cycle:
                        //TODO
                        break;
                    case CodeType.Condition:
                        {
                            ConditionNode condNode = curCodeNode as ConditionNode;
                            var links = dNode.GetAllOutgoingLinks();
                            List<int> inds = new List<int>();
                            // TODO: Check if it`s only 1 outgoing link for each variant in condition
                            for (int i = 0; i < links.Count; ++i)
                            {
                                var link = links[i];
                                int ind = link.OriginIndex; // 3 - false, 2 - true
                                inds.Add(ind);
                                if (ind == 2) // true sequence
                                {
                                    condNode.TrueNodeSeq = MakeCodeSequence(link.Destination);
                                }
                                else if (ind == 3) // false sequence
                                {
                                    condNode.FalseNodeSeq = MakeCodeSequence(link.Destination);
                                }
                            }
                            dNode = null;
                            break;
                        }
                    case CodeType.Assign:
                        dNode = dNode.OutgoingLinks[0].Destination;
                        curCodeNode.nextNode = GetCodeNode(dNode);
                        break;
                    case CodeType.Comment:
                        dNode = dNode.OutgoingLinks[0].Destination;
                        curCodeNode.nextNode = GetCodeNode(dNode);
                        break;
                    case CodeType.Unknown:
                    default:
                        return null;
                }
                curCodeNode = curCodeNode.nextNode;
            }
            return resNode;
        }

        public static string MakeProgram(Diagram diag, string progName = "program")
        {
            if (!WorkflowValidator.ValidateBlockDiagram(diag))
            {
                return "//Incorrect diagram!";
            }

            CodeNode startNode = MakeCodeSequence(diag.FindNode(NodeType.Begin));
            //startNode.SetProgramName(progName);
            
            //var curDiagNode = diag.FindNode(NodeType.Begin);
            //curDiagNode = curDiagNode.OutgoingLinks[0].Destination; // going to next node
            //CodeNode curCodeNode = startNode;
            //string tmpString;
            //int tmpVal;
            //while (curDiagNode != null)
            //{
            //    NodeType type = GetNodeTypeFromTag(curDiagNode.Tag);
            //    switch (type)
            //    {
            //        // it means that diagram is wrong, should show it some way
            //        case NodeType.Begin:
            //            curCodeNode.nextNode = new CommentNode("Unexpected begin node");
            //            curDiagNode = curDiagNode.OutgoingLinks[0].Destination; // going to next node
            //            break;
            //        case NodeType.End:
            //            curCodeNode.nextNode = new EndNode();
            //            curDiagNode = null;
            //            break;
            //        case NodeType.Declare:
            //            ParseDeclareText(curDiagNode.Text, out tmpString);
            //            curCodeNode.nextNode = new DeclareNode(tmpString);
            //            curDiagNode = curDiagNode.OutgoingLinks[0].Destination; // going to next node
            //            break;
            //        case NodeType.Assign:
            //            ParseAssignText(curDiagNode.Text, out tmpString, out tmpVal);
            //            curCodeNode.nextNode = new AssignNode(tmpString, tmpVal);
            //            curDiagNode = curDiagNode.OutgoingLinks[0].Destination; // going to next node
            //            break;
            //        case NodeType.Print:
            //            ParsePrintText(curDiagNode.Text, out tmpString);
            //            curCodeNode.nextNode = new PrintNode(tmpString);
            //            curDiagNode = curDiagNode.OutgoingLinks[0].Destination; // going to next node
            //            break;
            //        case NodeType.Input:
            //            ParseInputText(curDiagNode.Text, out tmpString);
            //            curCodeNode.nextNode = new InputNode(tmpString);
            //            curDiagNode = curDiagNode.OutgoingLinks[0].Destination; // going to next node
            //            break;
            //        case NodeType.Unknown:
            //        default:
            //            curCodeNode.nextNode = new CommentNode("Unknown node");
            //            curDiagNode = curDiagNode.OutgoingLinks[0].Destination; // going to next node
            //            break;
            //    }
            //    curCodeNode = curCodeNode.nextNode;
            //}

            string res = MakeTextFromCodeNodes(startNode);

            return res;
        }
    }
}
