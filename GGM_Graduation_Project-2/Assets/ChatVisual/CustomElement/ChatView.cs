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

        private ChatContainer chatContainer;        // 쳇팅들 담겨있는 곳.

        public Action<NodeView> OnNodeSelected;     // 내가 눌렸다고 알려줌.

        private int firstChatEndIndex = 0;      // 처음으로 시작하는 쳇팅의 마지막. 질문이 시작되는 곳의 노드스에서 찾을 수 있는  인덱스
        private int askAndReplysCount = 0;        // 질문에 대한 대답의 개수
        private int lockAskAndReplysCount = 0;

        public ChatView()
        {
            firstChatEndIndex = 0;
            askAndReplysCount = 0;
            lockAskAndReplysCount = 0;

            Insert(0, new GridBackground());        // 그리드 넣기

            this.AddManipulator(new ContentZoomer());       // 줌기능 조작 추가
            this.AddManipulator(new ContentDragger());  // 컨탠츠 드래그 가능
            this.AddManipulator(new SelectionDragger());    // 선택해준거 움직이기
            this.AddManipulator(new RectangleSelector());   // 네모 만들어주기  조작들 추가
        }

        public void LoadChatSystem(ChatContainer chatContainer)     // 로드해주기
        {
            this.chatContainer = chatContainer;

            // 루트 노드가 없다면 에디터에서 다시 그리자
            if (this.chatContainer.nodes.Count == 0)
            {
                RootNode rootNode = chatContainer.CreateNode(typeof(RootNode)) as RootNode;
                EditorUtility.SetDirty(this.chatContainer);
                AssetDatabase.SaveAssets();
            }

            //Debug.Log($"채팅노드 개수 : {this.chatContainer.NowChapter.chat.Count}");
            // 채팅 노드 만들어주기
            for (int i = 0; i < this.chatContainer.NowChapter.chat.Count; ++i)
            {
                ++firstChatEndIndex;
                if (this.chatContainer.nodes.Count() - 1 > i) continue;     // 카운트가 i보다 크면 노드가 있는 것임. - 1하는 이유는 루트노드가 존재하니까.

                ChatNode chatNode = chatContainer.CreateNode(typeof(ChatNode)) as ChatNode;
                chatNode.state = this.chatContainer.NowChapter.chat[i].state;
                chatNode.text = this.chatContainer.NowChapter.chat[i].text;
                chatNode.face = this.chatContainer.NowChapter.chat[i].face;
                chatNode.textEvent = new List<EChatEvent>(this.chatContainer.NowChapter.chat[i].textEvent);

                if (i == 0)     // 루트노드랑 연결해주기
                {
                    RootNode rootNode = this.chatContainer.nodes[0] as RootNode;
                    rootNode.child = chatNode;
                }
                else        // 전꺼랑 연결해주기
                {
                    ChatNode chatParentNode = this.chatContainer.nodes[i] as ChatNode;
                    chatParentNode.child.Add(chatNode);
                }
            }

            // 질문 노드 만들어주기
            {
                ChatNode firstChatEndNode = this.chatContainer.nodes[firstChatEndIndex] as ChatNode;
                if (firstChatEndNode != null)
                {
                    //Debug.Log($"질문의 개수 : {this.chatContainer.NowChapter.askAndReply.Count}, 챗팅의 인덱스 {firstChatEndIndex}");
                    // 그냥 질문 만들어주기
                    for (int i = 0; i < this.chatContainer.NowChapter.askAndReply.Count; ++i)
                    {
                        if (this.chatContainer.nodes.Count() - 1 > i + firstChatEndIndex + askAndReplysCount) continue;
                        ++askAndReplysCount;

                        // 질문 노드 추가해주기
                        AskNode askNode = chatContainer.CreateNode(typeof(AskNode)) as AskNode;
                        askNode.ask = this.chatContainer.NowChapter.askAndReply[i].ask;
                        askNode.reply = new List<Chat>(this.chatContainer.NowChapter.askAndReply[i].reply);        // 값 복사, 깊은 복사
                        askNode.is_UseThis = this.chatContainer.NowChapter.askAndReply[i].is_UseThis;

                        // 쳇팅 노드랑 연결해주기
                        firstChatEndNode.child.Add(askNode);

                        // 대답(쳇팅)노드 추가해주기
                        for (int j = 0; j < this.chatContainer.NowChapter.askAndReply[i].reply.Count; ++j)
                        {
                            ChatNode replyNode = chatContainer.CreateNode(typeof(ChatNode)) as ChatNode;
                            replyNode.state = this.chatContainer.NowChapter.askAndReply[i].reply[j].state;
                            replyNode.text = this.chatContainer.NowChapter.askAndReply[i].reply[j].text;
                            replyNode.face = this.chatContainer.NowChapter.askAndReply[i].reply[j].face;
                            replyNode.textEvent = new List<EChatEvent>(this.chatContainer.NowChapter.askAndReply[i].reply[j].textEvent);

                            // 연결 해주기
                            if (j == 0)     // 질문노드랑 연결해야 하는 경우
                            {
                                askNode.child = replyNode;
                            }
                            else
                            {
                                ChatNode chatParentNode = this.chatContainer.nodes[(j + firstChatEndIndex + askAndReplysCount) - 1] as ChatNode;        // 루트노드 빼고 넣어주기
                                if (chatParentNode != null)
                                {
                                    chatParentNode.child.Add(replyNode);
                                }
                            }
                            ++askAndReplysCount;
                        }
                    }

                    // 잠김 질문 만들어주기
                    for (int i = 0; i < this.chatContainer.NowChapter.lockAskAndReply.Count; i++)
                    {
                        if (this.chatContainer.nodes.Count() - 1 > i + firstChatEndIndex + askAndReplysCount + askAndReplysCount) continue;
                        ++lockAskAndReplysCount;

                        // 락질문 노드 추가해주기
                        LockAskNode lockAskNode = chatContainer.CreateNode(typeof(LockAskNode)) as LockAskNode;
                        lockAskNode.evidence = new List<string>(this.chatContainer.NowChapter.lockAskAndReply[i].evidence);
                        lockAskNode.ask = this.chatContainer.NowChapter.lockAskAndReply[i].ask;
                        lockAskNode.reply = new List<Chat>(this.chatContainer.NowChapter.lockAskAndReply[i].reply);        // 값 복사, 깊은 복사
                        lockAskNode.is_UseThis = this.chatContainer.NowChapter.lockAskAndReply[i].is_UseThis;

                        // 쳇팅 노드랑 연결해주기
                        firstChatEndNode.child.Add(lockAskNode);

                        // 대답(쳇팅)노드 추가해주기
                        for (int j = 0; j < this.chatContainer.NowChapter.lockAskAndReply[i].reply.Count; ++j)
                        {
                            ChatNode replyNode = chatContainer.CreateNode(typeof(ChatNode)) as ChatNode;
                            replyNode.state = this.chatContainer.NowChapter.lockAskAndReply[i].reply[j].state;
                            replyNode.text = this.chatContainer.NowChapter.lockAskAndReply[i].reply[j].text;
                            replyNode.face = this.chatContainer.NowChapter.lockAskAndReply[i].reply[j].face;
                            replyNode.textEvent = new List<EChatEvent>(this.chatContainer.NowChapter.lockAskAndReply[i].reply[j].textEvent);

                            // 연결 해주기
                            if (j == 0)     // 질문노드랑 연결해야 하는 경우
                            {
                                lockAskNode.child = replyNode;
                            }
                            else
                            {
                                ChatNode chatParentNode = this.chatContainer.nodes[(j + firstChatEndIndex + askAndReplysCount + lockAskAndReplysCount) - 1] as ChatNode;        // 루트노드 빼고 넣어주기
                                if (chatParentNode != null)
                                {
                                    chatParentNode.child.Add(replyNode);
                                }
                            }
                            ++lockAskAndReplysCount;
                        }
                    }
                }
            }
        }

        public void SaveChatSystem()
        {
            chatContainer.NowChapter.chat.Clear();
            chatContainer.NowChapter.askAndReply.Clear();
            chatContainer.NowChapter.lockAskAndReply.Clear();

            int nowChatIndex = 0;       // 처음꺼로 가정하고
            int askIndex = 0;
            int lockAskIndex = 0;

            this.chatContainer.nodes.ForEach(n =>
            {
                var children = this.chatContainer.GetChildren(n);       // 자식들 가져오기
                children.ForEach(c =>
                {
                    ChatNode chatNode = c as ChatNode;      // 쳇팅 노드이면
                    if (chatNode != null && firstChatEndIndex > nowChatIndex)
                    {
                        // 새로운 클래스 만들어줌.
                        Chat chapter = new Chat();
                        chatContainer.NowChapter.chat.Add(chapter);
                        chatContainer.NowChapter.chat[nowChatIndex].state = chatNode.state;
                        chatContainer.NowChapter.chat[nowChatIndex].text = chatNode.text;
                        chatContainer.NowChapter.chat[nowChatIndex].face = chatNode.face;
                        chatContainer.NowChapter.chat[nowChatIndex].textEvent = chatNode.textEvent;
                        ++nowChatIndex;
                    }

/*                    AskNode askNode = c as AskNode;     // 질문 노드이면
                    if (askNode != null)
                    {
                        AskAndReply askAndReply = new AskAndReply();
                        chatContainer.NowChapter.askAndReply.Add(askAndReply);
                        chatContainer.NowChapter.askAndReply[askIndex].ask = askNode.ask;
                        chatContainer.NowChapter.askAndReply[askIndex].reply = askNode.reply;
                        chatContainer.NowChapter.askAndReply[askIndex].is_UseThis = askNode.is_UseThis;
                        ++askIndex;
                    }

                    LockAskNode lockAskNode = c as LockAskNode;*/


                });
            });
            chatContainer.ChangeNewChpater();
        }

        public void PopulateView()
        {
            graphViewChanged -= OnGraphViewChanged;

            DeleteElements(graphElements);       // 기존에 그려졌던 애들 모두 삭제

            graphViewChanged += OnGraphViewChanged;

            // 노드들 그려주기
            this.chatContainer.nodes.ForEach(n => CreateNodeView(n));

            // 엣지 그려주기
            this.chatContainer.nodes.ForEach(n =>
            {
                var children = this.chatContainer.GetChildren(n);
                NodeView parent = FindNodeView(n);
                children.ForEach(c =>
                {
                    NodeView child = FindNodeView(c);
                    Edge edge = parent.output.ConnectTo(child.input);
                    AddElement(edge);
                });
            });

            // 숫자 만들어주기
            this.chatContainer.SortIndex();
        }

        private NodeView FindNodeView(Node node)
        {
            return GetNodeByGuid(node.guid) as NodeView;
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove != null)       // 지워진 애들이 있으면
            {
                graphViewChange.elementsToRemove.ForEach(elem =>
                {
                    var nodeView = elem as NodeView;
                    if (nodeView != null)
                    {
                        chatContainer.DeleteNode(nodeView.node);        // 지워주기
                    }

                    var edge = elem as Edge;        // 연결선
                    if (edge != null)
                    {
                        NodeView parent = edge.output.node as NodeView;
                        NodeView child = edge.input.node as NodeView;

                        chatContainer.RemoveChild(parent.node, child.node);
                    }
                });
            }

            if (graphViewChange.edgesToCreate != null)      // 선 연결해주기
            {
                graphViewChange.edgesToCreate.ForEach(edge =>
                {
                    NodeView parent = edge.output.node as NodeView;
                    NodeView child = edge.input.node as NodeView;

                    chatContainer.AddChild(parent.node, child.node);
                });
            }

            if (graphViewChange.movedElements != null)      // 움직인 애들이 있으면 정렬해주기
            {
                nodes.ForEach(n =>
                {
                    var nodeView = n as NodeView;
                    nodeView?.SortChildren();
                });
            }

            return graphViewChange;
        }

        private void CreateNode(Type type, Vector2 position)
        {
            Node node = chatContainer.CreateNode(type);      // 노드 생성
            node.position = position;
            CreateNodeView(node);       // 보이는 걸 실제로 추가해줌.
        }

        private void CreateNodeView(Node n)
        {
            NodeView nodeView = new NodeView(n);
            nodeView.OnNodeSelected = OnNodeSelected;
            AddElement(nodeView);
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)       // 우클릭 했을 때 나올 메뉴들
        {
            if (chatContainer == null)      // 지금 눌러준. 트리가 없으면
            {
                Debug.Log("컨테이너 없어요!");
                evt.StopPropagation();      // 이벤트 전파 금지
                return;
            }

            Vector2 nodePosition = this.ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);      // 우클릭한 위치 가져오기, 그 위치에 노드 생성 예정

            var types = TypeCache.GetTypesDerivedFrom<Node>();      // 상속받은 애들 모두 가지고 오기
            foreach (var type in types)
            {
                if (type.Name == "RootNode") continue;
                evt.menu.AppendAction($"{type.Name}", (a) => { CreateNode(type, nodePosition); });
            }
        }

        // 드래깅이 시작될 때 연결가능한 포트를 가져오는 함수
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where(x => x.direction != startPort.direction).ToList();      // input output이 다르면 연결 가능
        }
    }
}