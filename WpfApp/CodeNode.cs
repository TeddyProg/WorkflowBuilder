using System.Collections.Generic;
using System.Windows;
using MindFusion.Diagramming.Wpf;
using System.IO;

namespace WpfApp
{

    class CodeNode
    {
        public virtual string SharpCode => "Unknown";

        public CodeNode nextNode = null;
    }

    class BeginNode : CodeNode
    {
        public override string SharpCode => $"//Beggining of program { _programName }";

        public void SetProgramName(string newName)
        {
            _programName = newName;
        }
        private string _programName = "unknown";
    }

    class PrintNode : CodeNode 
    {
        public override string SharpCode => $"Console.WriteLine({ _variableName });";

        public void SetVariableName(string newName)
        {
            _variableName = newName;
        }
        private string _variableName = "/*Here should be name of variable*/";
    }

    class DeclareNode : CodeNode
    {
        public DeclareNode()
        {

        }
        public DeclareNode(string varName)
        {
            _variableName = varName;
        }
        public override string SharpCode => $"int { _variableName };";

        public void SetVariableName(string newName)
        {
            _variableName = newName;
        }

        private string _variableName = "/*Here should be name of variable*/";
    }

    class AssignNode : CodeNode
    {
        public AssignNode()
        {

        }
        public AssignNode(string varName)
        {
            _variableName = varName;
        }

        public override string SharpCode => $"{ _variableName } = { _variableValue};";

        public void SetVariableName(string newName)
        {
            _variableName = newName;
        }

        public void SetVariableValue(int newValue)
        {
            _variableValue = newValue;
        }

        private string _variableName = "/*Here should be name of variable*/";
        private int _variableValue = 0;
    }

    class EndNode : CodeNode
    {
        public override string SharpCode => $"return;";
    }

    class CommentNode : CodeNode
    {
        public CommentNode()
        {
            Comment = "";
        }
        public CommentNode(string comment)
        {
            Comment = comment;
        }
        public override string SharpCode => $"//{ Comment };";
        
        public string Comment { get; set; }
    }

}
