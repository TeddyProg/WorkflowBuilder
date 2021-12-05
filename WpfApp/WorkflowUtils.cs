using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MindFusion.Diagramming.Wpf;
namespace WpfApp
{
    class WorkflowUtils
    {

        private static bool CheckNodesConnectedStrictly(DiagramNode begNode, DiagramNode endNode, List<DiagramNode> cache)
        {
            cache.Add(begNode);
            var links = begNode.GetAllOutgoingLinks();
            
            if (links.Count == 0)
                return false;

            for (int i = 0; i < links.Count; ++i)
            {
                var link = links[i];
                if (ReferenceEquals(link.Destination, endNode))
                    continue;

                if (cache.Contains(link.Destination))
                    return false;

                if (CheckNodesConnectedStrictly(link.Destination, endNode, cache))
                    continue;
            }
            return true;
        }

        private static bool CheckNodesConnectedUnstrictly(DiagramNode begNode, DiagramNode endNode, List<DiagramNode> cache)
        {
            cache.Add(begNode);
            var links = begNode.GetAllOutgoingLinks();
            for (int i = 0; i < links.Count; ++i)
            {
                var link = links[i];
                if (ReferenceEquals(link.Destination, endNode))
                    return true;

                if (cache.Contains(link.Destination))
                    continue;

                if (CheckNodesConnectedUnstrictly(link.Destination, endNode, cache))
                    return true;
            }
            return false;
        }

        public static bool CheckNodesConnected(DiagramNode begNode, DiagramNode endNode, bool strict)
        {
            return strict ? CheckNodesConnectedStrictly(begNode, endNode, new List<DiagramNode>()) : CheckNodesConnectedUnstrictly(begNode, endNode, new List<DiagramNode>());
        }

        public static NodeType GetNodeTypeFromTag(object tag)
        {
            return (NodeType)(int)tag;
        }
    }
}
