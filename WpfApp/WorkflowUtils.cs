using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp
{
    class WorkflowUtils
    {
        public static NodeType GetNodeTypeFromTag(object tag)
        {
            return (NodeType)(int)tag;
        }
    }
}
