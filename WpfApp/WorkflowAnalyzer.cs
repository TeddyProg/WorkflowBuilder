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

        private static bool ParseAssignText(string assgnNodeText, out string varName, out string varValue)
        {
            var args = assgnNodeText.Split(' ');
            varValue = "/*incorrect variable value*/";
            varName = "/*incorrect variable name*/";
            if (args.Length != 3)
                return false;
            if (args[1] != "=")
                return false;
            varName = args[0];
            varValue = args[2];
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

        private static string MakeTextFromCodeNodes(CodeNode startNode, string prefix = "")
        {
            string res = "";

            var curNode = startNode;
            while(curNode != null)
            {
                if (curNode.Type == CodeType.Condition)
                {
                    var condNode = curNode as ConditionNode;
                    condNode.Prefix = prefix;
                }
                res += prefix + curNode.SharpCode + '\n';
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
                        string varValue;
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
                                int ind = link.OriginAnchor; // 3 - false, 2 - true
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

        public static string MakeProgram(Diagram diag, string progName = "program", string prefix = "")
        {
            if (!WorkflowValidator.ValidateBlockDiagram(diag))
            {
                return "//Incorrect diagram!";
            }

            CodeNode startNode = MakeCodeSequence(diag.FindNode(NodeType.Begin));

            string res = MakeTextFromCodeNodes(startNode, prefix);

            return res;
        }

        private static HashSet<string> GetVariablesFromDiagram(Diagram d)
        {
            HashSet<string> result = new HashSet<string>();
            var nodes = d.Nodes;

            bool IsNum(string v) 
            {
                return int.TryParse(v, out _);
            }

            foreach (var node in nodes)
            {
                var type = WorkflowUtils.GetNodeTypeFromTag(node.Tag);
                switch (type)
                {
                    case NodeType.Declare:
                        {
                            string varName;
                            ParseDeclareText(node.Text, out varName);
                            result.Add(varName);
                            break;
                        }
                    case NodeType.Assign:
                        {
                            string varName;
                            string varValue;
                            
                            ParseAssignText(node.Text, out varName, out varValue);
                            result.Add(varName);
                            if (!IsNum(varValue))
                            {
                                result.Add(varValue);
                            }
                            break;
                        }
                    case NodeType.Print:
                        {
                            string varName;

                            ParsePrintText(node.Text, out varName);
                            if (!IsNum(varName))
                            {
                                result.Add(varName);
                            }
                            break;
                        }
                    case NodeType.Input:
                        {
                            string varName;
                            ParseInputText(node.Text, out varName);
                            result.Add(varName);
                            break;
                        }
                    case NodeType.Decision:
                        {
                            string condText;
                            ParseDecisionText(node.Text, out condText);
                            var arghs = condText.Split(' ');
                            result.Add(arghs[0]);
                            if (!IsNum(arghs[2]) )
                            {
                                result.Add(arghs[2]);
                            }
                            break;
                        }
                    default:
                        continue;
                }
            }
            return result;
        }

        public static string MakeProgram(List<Diagram> diags)
        {

            string res = header;

            HashSet<string> variables = new HashSet<string>();
            foreach (var d in diags)
            {
                var newVars = GetVariablesFromDiagram(d);
                variables.UnionWith(newVars);
            }

            foreach(string var in variables)
            {
                res += "\t\tprivate static volatile int V;\n";
            }

            res +=
            "\t\tstatic void Main(string[] args)\n" +
            "\t\t{\n" +
            "\n" +
            "\t\t\tTask[] tasks = {\n";

            foreach (var d in diags)
            {
                res += MakeProgram(d, "program", "\t\t\t\t");
            }

            res += footer;
            return res;
        }

        static string header =
            "using System;\n" +
            "using System.Threading.Tasks;\n" +
            "\n" +
            "namespace Work\n" +
            "{\n" +
            "\tclass Program\n" +
            "\t{\n";

        static string footer = 
            "\t\t\t};\n" + 
            "\n" +
            "\t\t\tfor(int i = 0; i < tasks.Length; ++i)\n" +
            "\t\t\t{\n" + 
            "\t\t\t\ttasks[i].Start();\n" + 
            "\t\t\t}\n" + 
            "\n" + 
            "\t\t\tTask.WaitAll(tasks);\n" + 
            "\n" + 
            "\t\t}\n" + 
            "\t}\n" +
            "}\n";


    }
}
