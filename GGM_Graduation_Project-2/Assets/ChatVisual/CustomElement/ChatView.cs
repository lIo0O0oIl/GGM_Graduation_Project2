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

        private ChatContainer chatContainer;        // ??쒕룊??????㏓낵???덈츎 ??

        public Action<NodeView> OnNodeSelected;     // ??? ???二??釉붋?????삯쨹?

        public ChatView()
        {
            Insert(0, new GridBackground());        // ?잙갭梨????影?끸뵛

            this.AddManipulator(new ContentZoomer());       // 繞벿살뒦????브퀗????怨뺣뼺?
            this.AddManipulator(new ContentDragger());  // ???쳜繹먮먦럹???類ㅼ굥???띠럾???
            this.AddManipulator(new SelectionDragger());    // ??ルㅎ臾???濾???嶺뚯쉳??議용Ь?
            this.AddManipulator(new RectangleSelector());   // ???좉콫 嶺뚮씭??キ??怨삵룖?? ?브퀗?????怨뺣뼺?
        }

        public void LoadChatSystem(ChatContainer _chatContainer)     // ?β돦裕녻キ??怨삵룖??
        {
            chatContainer = _chatContainer;

            chatContainer.nodes.Clear();

            int firstChatEndIndex = 0;      // 嶺뚳퐣瑗???怨쀬Ŧ ??戮곗굚??濡ル츎 ??쒕룊???嶺뚮씭??嶺? 嶺뚯쉶?꾣룇????戮곗굚??濡ル츎 ??ㅻ걙???筌뤾퍓援???고뱺??嶺뚢돦堉???????덈츎  ?筌뤾퍓???
            int askAndReplysCount = 0;        // 嶺뚯쉶?꾣룇???????????踰??띠룇裕??         // ???곗벟 2?띠룇裕? ???겶??????? ?꾩룆??????ㅲ뵛
            int lockAskAndReplysCount = 0;

            // ?猷먮쳜???筌뤾퍓援?遊붋?????곕뻣 ?잙갭梨?????ㅲ뵛
            {
                RootNode rootNode = chatContainer.CreateNode(typeof(RootNode)) as RootNode;
                rootNode.showName = chatContainer.NowChapter.showName;
                rootNode.saveLocation = chatContainer.NowChapter.saveLocation;
                rootNode.round = new List<string>(chatContainer.NowChapter.round);
                EditorUtility.SetDirty(chatContainer);
                AssetDatabase.SaveAssets();
            }

            // 嶺?????筌뤾퍓援?嶺뚮씭??キ??怨삵룖??
            for (int i = 0; i < chatContainer.NowChapter.chat.Count; ++i)
            {
                ++firstChatEndIndex;

                // ??⑥щ턄????⑤슡???怨삵룖??
                ChatNode chatNode = chatContainer.CreateNode(typeof(ChatNode)) as ChatNode;
                chatNode.state = chatContainer.NowChapter.chat[i].state;
                chatNode.type = chatContainer.NowChapter.chat[i].type;
                chatNode.text = chatContainer.NowChapter.chat[i].text;
                chatNode.face = chatContainer.NowChapter.chat[i].face;
                chatNode.textEvent = new List<EChatEvent>(chatContainer.NowChapter.chat[i].textEvent);

                // ?熬곣뫚?????뗮닡?낅슣?딁뵳?
                if (chatNode.state == EChatState.Other)
                {
                    chatNode.position = new Vector2(-125, 150 * (i + 1));
                }
                else
                {
                    chatNode.position = new Vector2(125, 150 * (i + 1));
                }

                if (i == 0)     // ?猷먮쳜??筌뤾퍓援????⑤슡???怨삵룖??
                {
                    RootNode rootNode = chatContainer.nodes[0] as RootNode;
                    rootNode.child = chatNode;
                }
                else        // ?熬곥룗?????⑤슡???怨삵룖??
                {
                    ChatNode chatParentNode = chatContainer.nodes[i] as ChatNode;
                    chatParentNode.child.Add(chatNode);
                }
            }

            // 嶺뚯쉶?꾣룇, ??? 嶺뚯쉶?꾣룇 ?筌뤾퍓援?嶺뚮씭??キ??怨삵룖??
            {
                ChatNode firstChatEndNode = chatContainer.nodes[firstChatEndIndex] as ChatNode;
                if (firstChatEndNode != null)
                {
                    Vector2 endChatNodePosition = firstChatEndNode.position;
                    //Debug.Log($"嶺뚯쉶?꾣룇???띠룇裕??: {this.chatContainer.NowChapter.askAndReply.Count}, 嶺???????筌뤾퍓???{firstChatEndIndex}");

                    // ?잙갭梨뜻틦?嶺뚯쉶?꾣룇 嶺뚮씭??キ??怨삵룖??
                    for (int i = 0; i < chatContainer.NowChapter.askAndReply.Count; ++i)
                    {
                        ++askAndReplysCount;

                        // 嶺뚯쉶?꾣룇 ?筌뤾퍓援??怨뺣뼺???怨댄맋 ??⑥щ턄???怨뺣뼺?
                        AskNode askNode = chatContainer.CreateNode(typeof(AskNode)) as AskNode;
                        askNode.ask = this.chatContainer.NowChapter.askAndReply[i].ask;
                        askNode.reply = new List<Chat>(this.chatContainer.NowChapter.askAndReply[i].reply);        // ???곌랜踰딀쾮? 濚밸Ŧ遊? ?곌랜踰딀쾮?
                        askNode.is_UseThis = this.chatContainer.NowChapter.askAndReply[i].is_UseThis;

                        // ??쒕룊???筌뤾퍓援????⑤슡???怨삵룖??
                        firstChatEndNode.child.Add(askNode);

                        // ?熬곣뫚?????깆젧??怨삵룖??
                        askNode.position = new Vector2(endChatNodePosition.x + i * 400, endChatNodePosition.y + 200);

                        // ??????쒕룊???筌뤾퍓援??怨뺣뼺???怨삵룖??
                        for (int j = 0; j < chatContainer.NowChapter.askAndReply[i].reply.Count; ++j)
                        {
                            // ??쒕룊???筌뤾퍓援??怨뺣뼺???怨댄맋 ??⑥щ턄???怨뺣뼺?
                            ChatNode replyNode = chatContainer.CreateNode(typeof(ChatNode)) as ChatNode;
                            replyNode.state = this.chatContainer.NowChapter.askAndReply[i].reply[j].state;
                            replyNode.type = this.chatContainer.NowChapter.askAndReply[i].reply[j].type;
                            replyNode.text = this.chatContainer.NowChapter.askAndReply[i].reply[j].text;
                            replyNode.face = this.chatContainer.NowChapter.askAndReply[i].reply[j].face;
                            replyNode.textEvent = new List<EChatEvent>(this.chatContainer.NowChapter.askAndReply[i].reply[j].textEvent);

                            // ?熬곣뫚?????뗮닡?낅슣?딁뵳?
                            if (replyNode.state == EChatState.Other)
                            {
                                replyNode.position = new Vector2(askNode.position.x - 100, askNode.position.y + 140 * (j + 1));
                            }
                            else
                            {
                                replyNode.position = new Vector2(askNode.position.x + 100, askNode.position.y + 140 * (j + 1));
                            }

                            // ??⑤슡????怨삵룖??
                            if (j == 0)     // 嶺뚯쉶?꾣룇?筌뤾퍓援????⑤슡???怨룻뒍 ??濡ル츎 ?롪퍔???
                            {
                                askNode.child = replyNode;
                            }
                            else
                            {
                                ChatNode chatParentNode = this.chatContainer.nodes[j + firstChatEndIndex + askAndReplysCount] as ChatNode;        // ?猷먮쳜??筌뤾퍓援???????影??꽑?낅슣?딁뵳?
                                if (chatParentNode != null)
                                {
                                    chatParentNode.child.Add(replyNode);
                                }
                            }
                            if (j == chatContainer.NowChapter.askAndReply[i].reply.Count - 1)      // ???곗벟 嶺뚮씭??嶺??꾩룇瑗??????
                            {
                                askAndReplysCount += j + 1;
                            }
                        }
                    }

                    // ??? 嶺뚯쉶?꾣룇 嶺뚮씭??キ??怨삵룖??
                    for (int i = 0; i < chatContainer.NowChapter.lockAskAndReply.Count; i++)
                    {
                        ++lockAskAndReplysCount;

                        // ????떨???筌뤾퍓援??怨뺣뼺???怨삵룖??
                        LockAskNode lockAskNode = chatContainer.CreateNode(typeof(LockAskNode)) as LockAskNode;
                        lockAskNode.evidence = new List<string>(this.chatContainer.NowChapter.lockAskAndReply[i].evidence);
                        lockAskNode.ask = this.chatContainer.NowChapter.lockAskAndReply[i].ask;
                        lockAskNode.reply = new List<Chat>(this.chatContainer.NowChapter.lockAskAndReply[i].reply);        // ???곌랜踰딀쾮? 濚밸Ŧ遊? ?곌랜踰딀쾮?
                        lockAskNode.is_UseThis = this.chatContainer.NowChapter.lockAskAndReply[i].is_UseThis;

                        // ??쒕룊???筌뤾퍓援????⑤슡???怨삵룖??
                        firstChatEndNode.child.Add(lockAskNode);

                        // ?熬곣뫚?????깆젧??怨삵룖??
                        lockAskNode.position = new Vector2(endChatNodePosition.x + (chatContainer.NowChapter.askAndReply.Count * 400) + (i * 400), endChatNodePosition.y + 200);

                        // ??????쒕룊???筌뤾퍓援??怨뺣뼺???怨삵룖??
                        for (int j = 0; j < this.chatContainer.NowChapter.lockAskAndReply[i].reply.Count; ++j)
                        {
                            ChatNode replyNode = chatContainer.CreateNode(typeof(ChatNode)) as ChatNode;
                            replyNode.state = this.chatContainer.NowChapter.lockAskAndReply[i].reply[j].state;
                            replyNode.type = this.chatContainer.NowChapter.lockAskAndReply[i].reply[j].type;
                            replyNode.text = this.chatContainer.NowChapter.lockAskAndReply[i].reply[j].text;
                            replyNode.face = this.chatContainer.NowChapter.lockAskAndReply[i].reply[j].face;
                            replyNode.textEvent = new List<EChatEvent>(this.chatContainer.NowChapter.lockAskAndReply[i].reply[j].textEvent);

                            // ?熬곣뫚?????뗮닡?낅슣?딁뵳?
                            if (replyNode.state == EChatState.Other)
                            {
                                replyNode.position = new Vector2(lockAskNode.position.x - 100, lockAskNode.position.y + 140 * (j + 1));
                            }
                            else
                            {
                                replyNode.position = new Vector2(lockAskNode.position.x + 100, lockAskNode.position.y + 140 * (j + 1));
                            }

                            // ??⑤슡????怨삵룖??
                            if (j == 0)     // 嶺뚯쉶?꾣룇?筌뤾퍓援????⑤슡???怨룻뒍 ??濡ル츎 ?롪퍔???
                            {
                                lockAskNode.child = replyNode;
                            }
                            else
                           {
                                ChatNode chatParentNode = this.chatContainer.nodes[j + firstChatEndIndex + askAndReplysCount + lockAskAndReplysCount] as ChatNode;        // ?猷먮쳜??筌뤾퍓援???????影??꽑?낅슣?딁뵳?
                                if (chatParentNode != null)
                                {
                                    chatParentNode.child.Add(replyNode);
                                }
                            }
                            if (j == chatContainer.NowChapter.lockAskAndReply[i].reply.Count - 1)      // ???곗벟 嶺뚮씭??嶺??꾩룇瑗??????
                            {
                                lockAskAndReplysCount += j + 1;
                            }
                        }
                    }
                }
            }
        }

        public void SaveChatSystem()        // ?띠룆?獄?????繞③뜮癒㏓뭄?
        {
            chatContainer.NowChapter.chat.Clear();
            chatContainer.NowChapter.askAndReply.Clear();
            chatContainer.NowChapter.lockAskAndReply.Clear();

            // nodes ?洹먮봾裕?筌뤾쑬????熬곣뫗???筌뤾퍓???? ??戮?뻣??怨맡???
            int chatIndex = 0;       // ?띠럾???쒑뜎????????덈츎 ??쒕룊????깅꺄 ?筌뤾퍓???
            int askIndex = 0;       // ???깅쾳 ???깅굵 嶺뚯쉶?꾣룇???깅꺄 ?筌뤾퍓???
            int lockAskIndex = 0;       // 嶺뚯쉶?꾣룇 ???깅쾳????瑜곴텪 ??? 嶺뚯쉶?꾣룇??
            bool firstChatEnd = false;

            int nowAskIndex = 0;        // 嶺뚯솘???嶺뚯쉶?꾣룇???筌뤾퍓???
            int nowReplyIndex = 0;      // 嶺뚯솘???嶺뚯쉶?꾣룇?????곗꽑???????筌뤾퍓???
            int nowReplysCountIndex = 0;      // 嶺뚯솘???嶺뚯쉶?꾣룇???????띠룇裕???筌뤾퍓??? ?熬곣뫁???洹먮봾裕?筌뤾쑴踰??筌뤾퍓?????
            bool lockAskStart = false;      // ??? 嶺뚯쉶?꾣룇????戮곗굚??琉????좊듆

            // 嶺뚯쉶?꾣룇???????띠룇裕?遺븍ご????繞③뜮癒㏉떊?怨뺣츎 ?洹먮봾裕??
            List<int> replysCount = new List<int>();            // -1?? 

            if (chatContainer.nodes[0] != null)     // ?猷먮쳜??筌뤾퍓援→뤆?쎛 ???덈펲嶺?
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
                var children = chatContainer.GetChildren(n);       // ???六???띠럾??筌뤾쑴沅롧뼨?
                children.ForEach(c =>
                {
                    ChatNode chatNode = c as ChatNode;      // ??쒕룊???筌뤾퍓援?????
                    if (chatNode != null)
                    {
                        if (firstChatEnd == false)
                        {
                            // ???됱Ŧ????????嶺뚮씭??キ??怨멥돘.
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
                            // ?????援????繞③뜮癒㏉떊??ㅲ뵛
                            if (lockAskStart == false)
                            {
                                if (replysCount[nowReplysCountIndex] == -1)
                                {
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
                                    Chat reply = new Chat();
                                    chatContainer.NowChapter.askAndReply[nowAskIndex].reply.Add(reply);
                                    chatContainer.NowChapter.askAndReply[nowAskIndex].reply[nowReplyIndex].state = chatNode.state;
                                    chatContainer.NowChapter.askAndReply[nowAskIndex].reply[nowReplyIndex].text = chatNode.text;
                                    chatContainer.NowChapter.askAndReply[nowAskIndex].reply[nowReplyIndex].face = chatNode.face;
                                    chatContainer.NowChapter.askAndReply[nowAskIndex].reply[nowReplyIndex].textEvent = chatNode.textEvent;
                                    nowReplyIndex++;

                                    if (nowReplyIndex + 1 > replysCount[nowReplysCountIndex])      // ?????띠룇裕?遺븍ご???琉????좊듆
                                    {
                                        nowAskIndex++;
                                        nowReplysCountIndex++;
                                        nowReplyIndex = 0;
                                    }
                                }
                            }
                            else
                            {
                                nowReplyIndex++;
                                Chat chat = new Chat();
                                chatContainer.NowChapter.lockAskAndReply[nowAskIndex].reply.Add(chat);
                                chatContainer.NowChapter.lockAskAndReply[nowAskIndex].reply[nowReplyIndex].state = chatNode.state;
                                chatContainer.NowChapter.lockAskAndReply[nowAskIndex].reply[nowReplyIndex].text = chatNode.text;
                                chatContainer.NowChapter.lockAskAndReply[nowAskIndex].reply[nowReplyIndex].face = chatNode.face;
                                chatContainer.NowChapter.lockAskAndReply[nowAskIndex].reply[nowReplyIndex].textEvent = chatNode.textEvent;

                                if (nowReplyIndex + 1 > replysCount[nowReplysCountIndex])      // ?????띠룇裕?遺븍ご???琉????좊듆
                                {
                                    nowAskIndex++;
                                    nowReplysCountIndex++;
                                    nowReplyIndex = 0;
                                }
                            }
                        }
                    }

                    AskNode askNode = c as AskNode;     // 嶺뚯쉶?꾣룇 ?筌뤾퍓援?????
                    if (askNode != null)
                    {
                        AskAndReply askAndReply = new AskAndReply();
                        chatContainer.NowChapter.askAndReply.Add(askAndReply);
                        chatContainer.NowChapter.askAndReply[askIndex].ask = askNode.ask;
                        replysCount.Add(askNode.reply.Count);
                        chatContainer.NowChapter.askAndReply[askIndex].is_UseThis = askNode.is_UseThis;
                        ++askIndex;
                        firstChatEnd = true;
                    }

                    LockAskNode lockAskNode = c as LockAskNode;
                    if (lockAskNode != null)
                    {
                        if (replysCount.Find(n => n == -1) == 0)
                        {
                            replysCount.Add(-1);        // ??袁㏓럡????쒕룊?????戮곗굚???덈펲??
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

            DeleteElements(graphElements);       // ?リ옇?????잙갭梨?????????ル봾援?嶺뚮ㅄ維筌?????

            graphViewChanged += OnGraphViewChanged;

            // ?筌뤾퍓援???잙갭梨?????ㅲ뵛
            this.chatContainer.nodes.ForEach(n => CreateNodeView(n));

            // ?影? ?잙갭梨?????ㅲ뵛
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

            // ?????嶺뚮씭??キ??怨삵룖??
            this.chatContainer.SortChildAndIndex();
        }

        private NodeView FindNodeView(Node node)
        {
            return GetNodeByGuid(node.guid) as NodeView;
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove != null)       // 嶺뚯솘????異???ル봾援?????깅さ嶺?
            {
                graphViewChange.elementsToRemove.ForEach(elem =>
                {
                    var nodeView = elem as NodeView;
                    if (nodeView != null)
                    {
                        chatContainer.DeleteNode(nodeView.node);        // 嶺뚯솘?????믥뼨?
                    }

                    var edge = elem as Edge;        // ??⑤슡???
                    if (edge != null)
                    {
                        NodeView parent = edge.output.node as NodeView;
                        NodeView child = edge.input.node as NodeView;

                        chatContainer.RemoveChild(parent.node, child.node);
                    }
                });
            }

            if (graphViewChange.edgesToCreate != null)      // ????⑤슡???怨삵룖??
            {
                graphViewChange.edgesToCreate.ForEach(edge =>
                {
                    NodeView parent = edge.output.node as NodeView;
                    NodeView child = edge.input.node as NodeView;

                    chatContainer.AddChild(parent.node, child.node);
                });
            }

           /* if (graphViewChange.movedElements != null)      // ??嶺뚯쉳?????ル봾援?????깅さ嶺??筌먲퐣議??怨삵룖??
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
            Node node = chatContainer.CreateNode(type);      // ?筌뤾퍓援???諛댁뎽
            node.position = position;
            CreateNodeView(node);       // ?곌랜????濾????깆젷???怨뺣뼺???怨멥돘.
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
                Debug.Log("???쳜????????怨룹꽑??");
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
            return ports.ToList().Where(x => x.direction != startPort.direction).ToList();      // input output?????섎?춯???⑤슡???띠럾???
        }
    }
}