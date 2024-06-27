using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System;
using System.Linq;

namespace ChatVisual
{
    public class ChatView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<ChatView, UxmlTraits> { }
        public class UmalTraits : GraphView.UxmlTraits { }

        private ChatTree chatTree;
        private Label whoLabel;

        public Action<NodeView> OnNodeSelected;

        public ChatView()
        {
            Insert(0, new GridBackground());        // Add Grid

            this.AddManipulator(new ContentZoomer());       // Add zoom
            this.AddManipulator(new ContentDragger());      // Add Dragger
            this.AddManipulator(new SelectionDragger());    // Add SelectionDragger
            this.AddManipulator(new RectangleSelector());   // Add RectangleSelector
        }

        public void InitChatView(Label label)
        {
            whoLabel = label;
        }

        public void PopulateView(ChatTree _chatContainer)
        {
            chatTree = _chatContainer;
            whoLabel.text = "ChatView - " + chatTree.name;
            graphViewChanged -= OnGraphViewChanged;

            DeleteElements(graphElements);      // GraphElement's node delete

            graphViewChanged += OnGraphViewChanged;

            if (chatTree.rootNode == null)     
            {
                chatTree.rootNode = chatTree.CreateNode(typeof(RootNode)) as RootNode;
                chatTree.rootNode.parent = chatTree;
                chatTree.rootNode.showName = chatTree.name;
                EditorUtility.SetDirty(chatTree);
                AssetDatabase.SaveAssets();
            }

            // node Create
            chatTree.nodeList.ForEach(n => CreateNodeView(n));

            // Line Create
            chatTree.nodeList.ForEach(n =>
            {
                var children = chatTree.GetChild(n);
                NodeView parent = FindNodeView(n);
                children.ForEach(c =>
                {
                    NodeView child = FindNodeView(c);
                    if (child != null && parent != null)
                    {
                        Edge edge = parent.output.ConnectTo(child.input);
                        AddElement(edge);
                    }
                });
            });
        }


        private NodeView FindNodeView(Node node)
        {
            if (node == null) return null;
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
                        chatTree.DeleteNode(nodeView.node); 
                    }

                    var edge = elem as Edge;        // RemoveEdge
                    if (edge != null)
                    {
                        NodeView parent = edge.output.node as NodeView;
                        NodeView child = edge.input.node as NodeView;

                        chatTree.RemoveChild(parent.node, child.node);
                    }
                });
            }

            if (graphViewChange.edgesToCreate != null)
            {
                graphViewChange.edgesToCreate.ForEach(edge =>
                {
                    NodeView parent = edge.output.node as NodeView;
                    NodeView child = edge.input.node as NodeView;

                    chatTree.AddChild(parent.node, child.node);
                });
            }

            if (graphViewChange.movedElements != null)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            return graphViewChange;
        }

        private void CreateNode(Type type, Vector2 position)
        {
            Node node = chatTree.CreateNode(type);      // Create node data
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
            if (chatTree == null)
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