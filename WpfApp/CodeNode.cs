using System.Collections.Generic;
using System.Windows;
using MindFusion.Diagramming.Wpf;
using System.IO;
using System;

namespace WpfApp
{

    enum CodeType
    {
        Begin,
        End,
        Print,
        Declare,
        Input,
        Cycle,
        Assign,
        Comment,
        Condition,
        Unknown
    }

    class CodeNode
    {
        public virtual string SharpCode => "Unknown";
        public virtual CodeType Type => CodeType.Unknown;

        public CodeNode nextNode = null;
    }

    class BeginNode : CodeNode
    {
        public override string SharpCode => $"//Beggining of program { _programName }";
        private string _programName = "unknown";
        public override CodeType Type => CodeType.Begin;

        public BeginNode()
        {

        }
        public BeginNode(string progName)
        {
            _programName = progName;
        }


        public void SetProgramName(string newName)
        {
            _programName = newName;
        }
    }

    class PrintNode : CodeNode 
    {
        public override string SharpCode => $"Console.WriteLine({ _variableName });";
        private string _variableName = "/*Here should be name of variable*/";
        public override CodeType Type => CodeType.Print;

        public PrintNode()
        {

        }
        public PrintNode(string varName)
        {
            _variableName = varName;
        }


        public void SetVariableName(string newName)
        {
            _variableName = newName;
        }
    }

    class InputNode : CodeNode
    {
        public override string SharpCode => $"{_variableName} = Convert.ToInt32(Console.ReadLine());";
        private string _variableName = "/*Here should be name of variable*/";
        public override CodeType Type => CodeType.Input;

        public InputNode()
        {

        }
        public InputNode(string varName)
        {
            _variableName = varName;
        }

        public void SetVariableName(string newName)
        {
            _variableName = newName;
        }
    }

    class DeclareNode : CodeNode
    {
        public override string SharpCode => $"int { _variableName };";
        private string _variableName = "/*Here should be name of variable*/";
        public override CodeType Type => CodeType.Declare;

        public DeclareNode()
        {

        }
        public DeclareNode(string varName)
        {
            _variableName = varName;
        }

        public void SetVariableName(string newName)
        {
            _variableName = newName;
        }

    }

    class AssignNode : CodeNode
    {
        private string _variableName = "/*Here should be name of variable*/";
        private string _variableValue = "/*Here should be value of variable*/";
        public override string SharpCode => $"{ _variableName } = { _variableValue};";
        public override CodeType Type => CodeType.Assign;

        public AssignNode()
        {

        }
        public AssignNode(string varName, string varValue)
        {
            _variableName = varName;
            _variableValue = varValue;
        }
        public void SetVariableName(string newName)
        {
            _variableName = newName;
        }
        public void SetVariableValue(string newValue)
        {
            _variableValue = newValue;
        }

    }

    class EndNode : CodeNode
    {
        public override CodeType Type => CodeType.End;
        public override string SharpCode => $"return;";
    }

    class CommentNode : CodeNode
    {
        public override string SharpCode => $"//{ Comment };";
        public string Comment { get; set; }
        public override CodeType Type => CodeType.Comment;

        public CommentNode()
        {
            Comment = "";
        }
        public CommentNode(string comment)
        {
            Comment = comment;
        }
    }

    class ConditionNode : CodeNode
    {
        public string Condition { get; set; }
        public override CodeType Type => CodeType.Condition;
        public string Prefix { get; set; }

        public CodeNode TrueNodeSeq { get; set; }
        public CodeNode FalseNodeSeq { get; set; }

        public ConditionNode()
        {
            Condition = "false";
        }
        public ConditionNode(string condition)
        {
            Condition = condition;
        }

        private string GetSeqCode(CodeNode begin)
        {
            CodeNode curNode = begin;
            string res = "";
            while (curNode != null)
            {
                if (curNode.Type == CodeType.Condition)
                {
                    ConditionNode condNode = curNode as ConditionNode;
                    condNode.Prefix = '\t' + Prefix;
                }
                res += '\t' + Prefix + curNode.SharpCode + '\n';
                curNode = curNode.nextNode;
            }
            return res;
        }

        public override string SharpCode
        {
            get
            {
                string res = 
                    $"if ({Condition})\n" +
                    $"{Prefix}{{\n";
                res += GetSeqCode(TrueNodeSeq);
                res += 
                    $"{Prefix}}}\n" +
                    $"{Prefix}else\n" +
                    $"{Prefix}{{\n";
                res += GetSeqCode(FalseNodeSeq);
                res += $"{Prefix}}}";
                return res;
            }
        }
    }
}
