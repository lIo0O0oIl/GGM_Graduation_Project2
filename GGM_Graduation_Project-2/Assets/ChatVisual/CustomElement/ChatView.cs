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

        private ChatContainer chatContainer;        // 쳇팅들 담겨있는 곳.

        public Action<NodeView> OnNodeSelected;     // 내가 눌렸다고 알려줌.

        public ChatView()
        {
            Insert(0, new GridBackground());        // 그리드 넣기

            this.AddManipulator(new ContentZoomer());       // 줌기능 조작 추가
            this.AddManipulator(new ContentDragger());  // 컨탠츠 드래그 가능
            this.AddManipulator(new SelectionDragger());    // 선택해준거 움직이기
            this.AddManipulator(new RectangleSelector());   // 네모 만들어주기  조작들 추가
        }

        public void LoadChatSystem(ChatContainer _chatContainer)     // 로드해주기
        {
            chatContainer = _chatContainer;

            chatContainer.nodes.Clear();

            int firstChatEndIndex = 0;      // 처음으로 시작하는 쳇팅의 마지막. 질문이 시작되는 곳의 노드스에서 찾을 수 있는  인덱스
            int askAndReplysCount = 0;        // 질문에 대한 대답의 개수          // 이게 2개가 되고 있었음. 바꿔주기
            int lockAskAndReplysCount = 0;

            // 루트 노드부터 다시 그려주기
            {
                RootNode rootNode = chatContainer.CreateNode(typeof(RootNode)) as RootNode;
                rootNode.showName = chatContainer.NowChapter.showName;
                rootNode.saveLocation = chatContainer.NowChapter.saveLocation;
                rootNode.round = new List<string>(chatContainer.NowChapter.round);
                EditorUtility.SetDirty(chatContainer);
                AssetDatabase.SaveAssets();
            }

            // 채팅 노드 만들어주기
            for (int i = 0; i < chatContainer.NowChapter.chat.Count; ++i)
            {
                ++firstChatEndIndex;

                // 데이터 연결해주기
                ChatNode chatNode = chatContainer.CreateNode(typeof(ChatNode)) as ChatNode;
                chatNode.state = chatContainer.NowChapter.chat[i].state;
                chatNode.text = chatContainer.NowChapter.chat[i].text;
                chatNode.face = chatContainer.NowChapter.chat[i].face;
                chatNode.textEvent = new List<EChatEvent>(chatContainer.NowChapter.chat[i].textEvent);

                // 위치 잡아주기
                if (chatNode.state == EChatState.Other)
                {
                    chatNode.position = new Vector2(-125, 150 * (i + 1));
                }
                else
                {
                    chatNode.position = new Vector2(125, 150 * (i + 1));
                }

                if (i == 0)     // 루트노드랑 연결해주기
                {
                    RootNode rootNode = chatContainer.nodes[0] as RootNode;
                    rootNode.child = chatNode;
                }
                else        // 전꺼랑 연결해주기
                {
                    ChatNode chatParentNode = chatContainer.nodes[i] as ChatNode;
                    chatParentNode.child.Add(chatNode);
                }
            }

            // 질문, 잠김 질문 노드 만들어주기
            {
                ChatNode firstChatEndNode = chatContainer.nodes[firstChatEndIndex] as ChatNode;
                if (firstChatEndNode != null)
                {
                    Vector2 endChatNodePosition = firstChatEndNode.position;
                    //Debug.Log($"질문의 개수 : {this.chatContainer.NowChapter.askAndReply.Count}, 챗팅의 인덱스 {firstChatEndIndex}");

                    // 그냥 질문 만들어주기
                    for (int i = 0; i < chatContainer.NowChapter.askAndReply.Count; ++i)
                    {
                        ++askAndReplysCount;

                        // 질문 노드 추가해서 데이터 추가
                        AskNode askNode = chatContainer.CreateNode(typeof(AskNode)) as AskNode;
                        askNode.ask = this.chatContainer.NowChapter.askAndReply[i].ask;
                        askNode.reply = new List<Chat>(this.chatContainer.NowChapter.askAndReply[i].reply);        // 값 복사, 깊은 복사
                        askNode.is_UseThis = this.chatContainer.NowChapter.askAndReply[i].is_UseThis;

                        // 쳇팅 노드랑 연결해주기
                        firstChatEndNode.child.Add(askNode);

                        // 위치 설정해주기
                        askNode.position = new Vector2(endChatNodePosition.x + i * 400, endChatNodePosition.y + 200);

                        // 대답(쳇팅)노드 추가해주기
                        for (int j = 0; j < chatContainer.NowChapter.askAndReply[i].reply.Count; ++j)
                        {
                            // 쳇팅 노드 추가해서 데이터 추가
                            ChatNode replyNode = chatContainer.CreateNode(typeof(ChatNode)) as ChatNode;
                            replyNode.state = this.chatContainer.NowChapter.askAndReply[i].reply[j].state;
                            replyNode.text = this.chatContainer.NowChapter.askAndReply[i].reply[j].text;
                            replyNode.face = this.chatContainer.NowChapter.askAndReply[i].reply[j].face;
                            replyNode.textEvent = new List<EChatEvent>(this.chatContainer.NowChapter.askAndReply[i].reply[j].textEvent);

                            // 위치 잡아주기
                            if (replyNode.state == EChatState.Other)
                            {
                                replyNode.position = new Vector2(askNode.position.x - 100, askNode.position.y + 140 * (j + 1));
                            }
                            else
                            {
                                replyNode.position = new Vector2(askNode.position.x + 100, askNode.position.y + 140 * (j + 1));
                            }

                            // 연결 해주기
                            if (j == 0)     // 질문노드랑 연결해야 하는 경우
                            {
                                askNode.child = replyNode;
                            }
                            else
                            {
                                ChatNode chatParentNode = this.chatContainer.nodes[j + firstChatEndIndex + askAndReplysCount] as ChatNode;        // 루트노드 빼고 넣어주기
                                if (chatParentNode != null)
                                {
                                    chatParentNode.child.Add(replyNode);
                                }
                                if (j == chatContainer.NowChapter.askAndReply[i].reply.Count - 1)      // 이게 마지막 반복이면
                                {
                                    askAndReplysCount += j + 1;
                                }
                            }
                        }
                    }

                    // 잠김 질문 만들어주기
                    for (int i = 0; i < chatContainer.NowChapter.lockAskAndReply.Count; i++)
                    {
                        ++lockAskAndReplysCount;

                        // 락질문 노드 추가해주기
                        LockAskNode lockAskNode = chatContainer.CreateNode(typeof(LockAskNode)) as LockAskNode;
                        lockAskNode.evidence = new List<string>(this.chatContainer.NowChapter.lockAskAndReply[i].evidence);
                        lockAskNode.ask = this.chatContainer.NowChapter.lockAskAndReply[i].ask;
                        lockAskNode.reply = new List<Chat>(this.chatContainer.NowChapter.lockAskAndReply[i].reply);        // 값 복사, 깊은 복사
                        lockAskNode.is_UseThis = this.chatContainer.NowChapter.lockAskAndReply[i].is_UseThis;

                        // 쳇팅 노드랑 연결해주기
                        firstChatEndNode.child.Add(lockAskNode);

                        // 위치 설정해주기
                        lockAskNode.position = new Vector2(endChatNodePosition.x + (chatContainer.NowChapter.askAndReply.Count * 400) + (i * 400), endChatNodePosition.y + 200);

                        // 대답(쳇팅)노드 추가해주기
                        for (int j = 0; j < this.chatContainer.NowChapter.lockAskAndReply[i].reply.Count; ++j)
                        {
                            ChatNode replyNode = chatContainer.CreateNode(typeof(ChatNode)) as ChatNode;
                            replyNode.state = this.chatContainer.NowChapter.lockAskAndReply[i].reply[j].state;
                            replyNode.text = this.chatContainer.NowChapter.lockAskAndReply[i].reply[j].text;
                            replyNode.face = this.chatContainer.NowChapter.lockAskAndReply[i].reply[j].face;
                            replyNode.textEvent = new List<EChatEvent>(this.chatContainer.NowChapter.lockAskAndReply[i].reply[j].textEvent);

                            // 위치 잡아주기
                            if (replyNode.state == EChatState.Other)
                            {
                                replyNode.position = new Vector2(lockAskNode.position.x - 100, lockAskNode.position.y + 140 * (j + 1));
                            }
                            else
                            {
                                replyNode.position = new Vector2(lockAskNode.position.x + 100, lockAskNode.position.y + 140 * (j + 1));
                            }

                            // 연결 해주기
                            if (j == 0)     // 질문노드랑 연결해야 하는 경우
                            {
                                lockAskNode.child = replyNode;
                            }
                            else
                            {
                                ChatNode chatParentNode = this.chatContainer.nodes[j + firstChatEndIndex + askAndReplysCount + lockAskAndReplysCount] as ChatNode;        // 루트노드 빼고 넣어주기
                                if (chatParentNode != null)
                                {
                                    chatParentNode.child.Add(replyNode);
                                }
                                if (j == chatContainer.NowChapter.lockAskAndReply[i].reply.Count - 1)      // 이게 마지막 반복이면
                                {
                                    lockAskAndReplysCount += j + 1;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void SaveChatSystem()        // 값들을 저장해줌.
        {
            chatContainer.NowChapter.chat.Clear();
            chatContainer.NowChapter.askAndReply.Clear();
            chatContainer.NowChapter.lockAskAndReply.Clear();

            // nodes 리스트에서 현재 인덱스를 표시해줄 것.
            int chatIndex = 0;       // 가장처음에 있는 쳇팅들의 인덱스
            int askIndex = 0;       // 다음 있을 질문들의 인덱스
            int lockAskIndex = 0;       // 질문 다음에 나올 잠김 질문들
            bool firstChatEnd = false;

            int nowAskIndex = 0;        // 지금 질문의 인덱스
            int nowReplyIndex = 0;      // 지금 질문에 들어갈 대답 인덱스
            int nowReplysCountIndex = 0;      // 지금 질문의 대답 개수 인덱스, 아래 리스트의 인덱스 값
            bool lockAskStart = false;      // 잠김 질문이 시작되었다면

            // 질문의 대답 개수를 저장해주는 리스트
            List<int> replysCount = new List<int>();            // -1은 

            if (chatContainer.nodes[0] != null)     // 루트노드가 있다면
            {
                RootNode rootNode = chatContainer.nodes[0] as RootNode;
                if (rootNode != null)
                {
                    chatContainer.NowChapter.showName = rootNode.showName;
                    chatContainer.NowChapter.saveLocation = rootNode.saveLocation;
                    chatContainer.NowChapter.round = new List<string>(rootNode.round);
                }
            }

            chatContainer.nodes.ForEach(n =>
            {
                var children = chatContainer.GetChildren(n);       // 자식들 가져오기
                children.ForEach(c =>
                {
                    ChatNode chatNode = c as ChatNode;      // 쳇팅 노드이면
                    if (chatNode != null)
                    {
                        if (firstChatEnd == false)
                        {
                            // 새로운 클래스 만들어줌.
                            Debug.Log("그냥쳇팅");
                            Chat chat = new Chat();
                            chatContainer.NowChapter.chat.Add(chat);
                            chatContainer.NowChapter.chat[chatIndex].state = chatNode.state;
                            chatContainer.NowChapter.chat[chatIndex].text = chatNode.text;
                            chatContainer.NowChapter.chat[chatIndex].face = chatNode.face;
                            chatContainer.NowChapter.chat[chatIndex].textEvent = chatNode.textEvent;
                            ++chatIndex;
                        }
                        else
                        {
                            // 대답들 저장해주기
                            if (lockAskStart == false)
                            {
                                if (replysCount[nowReplysCountIndex] == -1)
                                {
                                    Debug.Log("잠김 질문 시작");
                                    nowReplysCountIndex++;

                                    nowAskIndex = 0;
                                    nowReplyIndex = 0;

                                    Chat lockReply = new Chat();
                                    chatContainer.NowChapter.lockAskAndReply[nowAskIndex].reply.Add(lockReply);
                                    chatContainer.NowChapter.lockAskAndReply[nowAskIndex].reply[nowReplyIndex].state = chatNode.state;
                                    chatContainer.NowChapter.lockAskAndReply[nowAskIndex].reply[nowReplyIndex].text = chatNode.text;
                                    chatContainer.NowChapter.lockAskAndReply[nowAskIndex].reply[nowReplyIndex].face = chatNode.face;
                                    chatContainer.NowChapter.lockAskAndReply[nowAskIndex].reply[nowReplyIndex].textEvent = chatNode.textEvent;
                                    
                                    lockAskStart = true;
                                }

                                if (lockAskStart == false)
                                {
                                    Debug.Log("그냥질문대답쳇팅저장");
                                    Chat reply = new Chat();
                                    chatContainer.NowChapter.askAndReply[nowAskIndex].reply.Add(reply);
                                    chatContainer.NowChapter.askAndReply[nowAskIndex].reply[nowReplyIndex].state = chatNode.state;
                                    chatContainer.NowChapter.askAndReply[nowAskIndex].reply[nowReplyIndex].text = chatNode.text;
                                    chatContainer.NowChapter.askAndReply[nowAskIndex].reply[nowReplyIndex].face = chatNode.face;
                                    chatContainer.NowChapter.askAndReply[nowAskIndex].reply[nowReplyIndex].textEvent = chatNode.textEvent;
                                    nowReplyIndex++;

                                    if (nowReplyIndex + 1 > replysCount[nowReplysCountIndex])      // 대답 개수를 넘었다면
                                    {
                                        nowAskIndex++;
                                        nowReplysCountIndex++;
                                        nowReplyIndex = 0;
                                    }
                                }
                            }
                            else
                            {
                                Debug.Log("잠김 질문 2번째 대답");

                                nowReplyIndex++;
                                Chat chat = new Chat();
                                chatContainer.NowChapter.lockAskAndReply[nowAskIndex].reply.Add(chat);
                                chatContainer.NowChapter.lockAskAndReply[nowAskIndex].reply[nowReplyIndex].state = chatNode.state;
                                chatContainer.NowChapter.lockAskAndReply[nowAskIndex].reply[nowReplyIndex].text = chatNode.text;
                                chatContainer.NowChapter.lockAskAndReply[nowAskIndex].reply[nowReplyIndex].face = chatNode.face;
                                chatContainer.NowChapter.lockAskAndReply[nowAskIndex].reply[nowReplyIndex].textEvent = chatNode.textEvent;

                                if (nowReplyIndex + 1 > replysCount[nowReplysCountIndex])      // 대답 개수를 넘었다면
                                {
                                    nowAskIndex++;
                                    nowReplysCountIndex++;
                                    nowReplyIndex = 0;
                                }
                            }
                        }
                    }

                    AskNode askNode = c as AskNode;     // 질문 노드이면
                    if (askNode != null)
                    {
                        AskAndReply askAndReply = new AskAndReply();
                        chatContainer.NowChapter.askAndReply.Add(askAndReply);
                        chatContainer.NowChapter.askAndReply[askIndex].ask = askNode.ask;
                        replysCount.Add(askNode.reply.Count);
                        Debug.Log($"내 자식 개수 : {askNode.reply.Count}");
                        chatContainer.NowChapter.askAndReply[askIndex].is_UseThis = askNode.is_UseThis;
                        ++askIndex;
                        firstChatEnd = true;
                    }

                    LockAskNode lockAskNode = c as LockAskNode;
                    if (lockAskNode != null)
                    {
                        if (replysCount.Find(n => n == -1) == 0)
                        {
                            replysCount.Add(-1);        // 락걸린 쳇팅이 시작했다고
                        }

                        LockAskAndReply lockAskAndReply = new LockAskAndReply();
                        chatContainer.NowChapter.lockAskAndReply.Add(lockAskAndReply);
                        chatContainer.NowChapter.lockAskAndReply[lockAskIndex].evidence = lockAskNode.evidence;
                        chatContainer.NowChapter.lockAskAndReply[lockAskIndex].ask = lockAskNode.ask;
                        replysCount.Add(lockAskNode.reply.Count);
                        chatContainer.NowChapter.lockAskAndReply[lockAskIndex].is_UseThis = lockAskNode.is_UseThis;
                        ++lockAskIndex;
                        firstChatEnd = true;
                    }
                });
            });
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