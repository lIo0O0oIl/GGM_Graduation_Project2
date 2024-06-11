using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System;
using System.Linq;
using Codice.Client.Common;

namespace ChatVisual
{
    public class ChatView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<ChatView, UxmlTraits> { }
        public class UmalTraits : GraphView.UxmlTraits { }

        private ChatContainer chatContainer;

        public Action<NodeView> OnNodeSelected;

        public ChatView()
        {
            Insert(0, new GridBackground());        // Add Grid

            this.AddManipulator(new ContentZoomer());       // Add zoom
            this.AddManipulator(new ContentDragger());      // Add Dragger
            this.AddManipulator(new SelectionDragger());    // Add SelectionDragger
            this.AddManipulator(new RectangleSelector());   // Add RectangleSelector
        }

        public void LoadChatData(ChatContainer _chatContainer)
        {
            chatContainer = _chatContainer;

            // Unconditional root nodes exist
            if (chatContainer.HumanAndChatDictionary[chatContainer.nowHumanName].Count == 0)
            {
                chatContainer.CreateNode(typeof(RootNode));
            }
        }

        public void SaveChatName()
        {
            if (chatContainer.HumanAndChatDictionary[chatContainer.nowHumanName][0] is RootNode rootNode)
            {

            }
        }

        public void PopulateView()
        {
            graphViewChanged -= OnGraphViewChanged;

            DeleteElements(graphElements);      // GraphElement's node delete

            graphViewChanged += OnGraphViewChanged;

            // node Create
            this.chatContainer.HumanAndChatDictionary[chatContainer.nowHumanName].ForEach(n => CreateNodeView(n));

            // Line Create
            this.chatContainer.HumanAndChatDictionary[chatContainer.nowHumanName].ForEach(n =>
            {
                var children = this.chatContainer.GetChild(n);
                NodeView parent = FindNodeView(n);
                children.ForEach(c =>
                {
                    NodeView child = FindNodeView(c);
                    Edge edge = parent.output.ConnectTo(child.input);
                    AddElement(edge);
                });
            });

            // Sort
            this.chatContainer.SortChildAndIndex();
        }


        private NodeView FindNodeView(Node node)
        {
            return GetNodeByGuid(node.guid) as NodeView;
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove != null)       // When you delete an element
            {
                graphViewChange.elementsToRemove.ForEach(elem =>
                {
                    var nodeView = elem as NodeView;
                    if (nodeView != null)
                    {
                        chatContainer.DeleteNode(nodeView.node); 
                    }

                    var edge = elem as Edge;        // RemoveEdge
                    if (edge != null)
                    {
                        NodeView parent = edge.output.node as NodeView;
                        NodeView child = edge.input.node as NodeView;

                        chatContainer.RemoveChild(parent.node, child.node);
                    }
                });
            }

            if (graphViewChange.edgesToCreate != null)
            {
                graphViewChange.edgesToCreate.ForEach(edge =>
                {
                    NodeView parent = edge.output.node as NodeView;
                    NodeView child = edge.input.node as NodeView;

                    chatContainer.AddChild(parent.node, child.node);
                });
            }

            /* if (graphViewChange.movedElements != null)      // If the element moved
             {
                 nodes.ForEach(n =>
                 {
                     var nodeView = n as NodeView;
                     nodeView?.SortChildren();
                 });
             }*/

            return graphViewChange;
        }

        private void CreateNode(Type type, Vector2 position)
        {
            Node node = chatContainer.CreateNode(type);      // Create node data
            node.position = position;
            CreateNodeView(node);       // Create a visible node in the editor
        }

        private void CreateNodeView(Node n)
        {
            NodeView nodeView = new NodeView(n);
            nodeView.OnNodeSelected = OnNodeSelected;
            AddElement(nodeView);
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)       // Mouse Button(0) Click
        {
            if (chatContainer == null)
            {
                Debug.Log("Chatcontainer is not selected");
                evt.StopPropagation();      // event Stop;
                return;
            }

            Vector2 nodePosition = this.ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);      // Get Mouse Position

            var types = TypeCache.GetTypesDerivedFrom<Node>();      // Get All Child Name
            foreach (var type in types)
            {
                if (type.Name == "RootNode") continue;
                evt.menu.AppendAction($"{type.Name}", (a) => { CreateNode(type, nodePosition); });
            }
        }

        // Get All Connectable
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where(x => x.direction != startPort.direction).ToList();      // Can only connect things with different direction
        }
    }
}