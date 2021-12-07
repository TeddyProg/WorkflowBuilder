using System;
using System.Collections.Generic;
using System.Linq;
using MindFusion.Diagramming.Wpf;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp
{
    class WorkflowValidator
    {

        /// <summary>
        /// Check whether all nodes of given type has exact needed number of incoming and outgoing links
        /// </summary>
        /// <param name="diag">Diagram to analyze</param>
        /// <param name="typeToCheck">Type to check in diagram</param>
        /// <param name="numIncLinks">Num of links which should be incoming to node</param>
        /// <param name="numOutLinks">Num of links which should be outgoing from node</param>
        /// <returns>If all nodes of given type has correct number of links - true, false otherwise</returns>
        private static bool CheckNumOutgoingLinksOfNode(Diagram diag, NodeType typeToCheck, int numOutLinks)
        {
            var nodes = diag.Nodes;
            foreach (var node in nodes)
            {
                if (node.Tag != null)
                {
                    NodeType nodeType = WorkflowUtils.GetNodeTypeFromTag(node.Tag);
                    if (nodeType == typeToCheck)
                    {
                        if (node.OutgoingLinks.Count != numOutLinks)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }


        /// <summary>
        /// Check whether all nodes of given type has exact needed number of incoming and outgoing links
        /// </summary>
        /// <param name="diag">Diagram to analyze</param>
        /// <param name="typeToCheck">Type to check in diagram</param>
        /// <param name="numIncLinks">Num of links which should be incoming to node</param>
        /// <returns>If all nodes of given type has correct number of links - true, false otherwise</returns>
        private static bool CheckNumIncomingLinksOfNode(Diagram diag, NodeType typeToCheck, int numIncLinks)
        {
            var nodes = diag.Nodes;
            foreach (var node in nodes)
            {
                if (node.Tag != null)
                {
                    NodeType nodeType = WorkflowUtils.GetNodeTypeFromTag(node.Tag);
                    if (nodeType == typeToCheck)
                    {
                        if (node.IncomingLinks.Count != numIncLinks)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
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
            if (!CheckNumIncomingLinksOfNode(diag, typeToCheck, numIncLinks) ||
                !CheckNumOutgoingLinksOfNode(diag, typeToCheck, numOutLinks))
                return false;

            return true;
        }

        public static bool ValidateBlockDiagram(Diagram diag)
        {
            return ValidateBlockDiagram(diag, out _);
        }

        public static bool ValidateBlockDiagram(Diagram diag, out string message)
        {
            message = "OK";
            ///Checking that number of begin and end nodes equals 1
            {
                var nodes = diag.Nodes;
                uint numBegs = 0;
                uint numEnds = 0;
                foreach (var node in nodes)
                {
                    if (node.Tag != null)
                    {
                        NodeType tagStr = WorkflowUtils.GetNodeTypeFromTag(node.Tag);
                        if (tagStr == NodeType.End)
                            numEnds++;
                        else if (tagStr == NodeType.Begin)
                            numBegs++;
                    }
                }
                if (numBegs != 1 || numEnds != 1)
                {
                    message = "Diagram has no beggining and ending nodes";
                    return false;
                }
            }
            ////////////////////////////////////////////////////

            ///Checking that there is only one link from starting node and no link into it
            {
                if (!CheckNumLinksOfNode(diag, NodeType.Begin, 0, 1))
                {
                    message = "Diagram has wrong linking of beggining node";
                    return false;
                }

            }
            ////////////////////////////////////////////////////////

            ///Checking that there is no link from ending node
            {
                if (!CheckNumOutgoingLinksOfNode(diag, NodeType.End, 0))
                {
                    message = "Ending node has output link!";
                    return false;
                }
            }
            ////////////////////////////////////////////////////////

            ///Checking that num of incoming and outgoing links of print nodes equals 1
            {
                if (!CheckNumOutgoingLinksOfNode(diag, NodeType.Print, 1))
                {
                    message = "Some of print nodes have too many or no one outgoing link";
                    return false;
                }
            }
            ////////////////////////////////////////////////////

            ///Checking that num of incoming and outgoing links of assign nodes equals 1
            {
                if (!CheckNumOutgoingLinksOfNode(diag, NodeType.Assign, 1))
                {
                    message = "Some of assign nodes have too many or no one outgoing link";
                    return false;
                }
            }
            ////////////////////////////////////////////////////

            ///Checking that num of incoming and outgoing links of declare nodes equals 1
            {
                if (!CheckNumLinksOfNode(diag, NodeType.Declare, 1, 1))
                {
                    message = "Some of declare nodes have too many or no one outgoing and incoming link";
                    return false;
                }
            }
            ////////////////////////////////////////////////////

            ///Checking that num of incoming and outgoing links of declare nodes equals 1
            {
                if (!CheckNumOutgoingLinksOfNode(diag, NodeType.Decision, 2))
                {
                    message = "Some of decision nodes have too many or no one outgoing links";
                    return false;
                }
            }
            ////////////////////////////////////////////////////

            ///Checking that beggining and ending nodes are connected in any way
            {
                var begNode = diag.FindNode(NodeType.Begin);
                var endNode = diag.FindNode(NodeType.End);
                if (!WorkflowUtils.CheckNodesConnected(begNode, endNode, false))
                {
                    message = "Begin and end nodes on diagram are not connected!";
                    return false;
                }
            }
            ////////////////////////////////////////////////////

            /////Checking that num of incoming and outgoing links of declare nodes equals 1
            //{
            //    var begNode = diag.FindNode(NodeType.Begin);
            //    var endNode = diag.FindNode(NodeType.End);
            //    if (!WorkflowUtils.CheckNodesConnected(begNode, endNode, false))
            //    {
            //        return false;
            //    }
            //}
            //////////////////////////////////////////////////////

            return true;
        }

    }
}
