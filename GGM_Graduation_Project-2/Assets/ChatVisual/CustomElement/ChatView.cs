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

        private ChatContainer chatContainer;        // ???類ｌ┯???????蹂κ땁?????됲닓 ??

        public Action<NodeView> OnNodeSelected;     // ??? ????????고뀘??????????

        public ChatView()
        {
            Insert(0, new GridBackground());        // ???녾컯嶺????鶯ㅺ동???紐껎꺙

            this.AddManipulator(new ContentZoomer());       // 嚥싳쉶瑗??裕?????됰슦???????ㅻ쿋??
            this.AddManipulator(new ContentDragger());  // ????얠뮁萸먪솒????????嶺뚮Ĳ?됪뤃????醫딆쓧???
            this.AddManipulator(new SelectionDragger());    // ????ｋ?????????꿔꺂?????브퀣?쒐낼?
            this.AddManipulator(new RectangleSelector());   // ????ル맩???꿔꺂?????????κ땁??? ??됰슦????????ㅻ쿋??
        }

        public void LoadChatSystem(ChatContainer _chatContainer)     // ?汝??吏??좉텣????κ땁???
        {
            chatContainer = _chatContainer;

            chatContainer.nodes.Clear();

            int firstChatEndIndex = 0;      // ?꿔꺂??節뉖き?????Β????嶺뚮??ｆ뤃???β뼯爰귨㎘????類ｌ┯????꿔꺂?????? ?꿔꺂???熬곻퐢利????嶺뚮??ｆ뤃???β뼯爰귨㎘?????곗숯???癲ル슢??蹂좊쨨????⑥쥓援???꿔꺂?????????????됲닓  ?癲ル슢?????
            int askAndReplysCount = 0;        // ?꿔꺂???熬곻퐢利???????????????醫딆┻???         // ????⑥レ퓡 2??醫딆┻?? ???野껊뿈???????? ?熬곣뫖利??????????
            int lockAskAndReplysCount = 0;

            // ??猷매?댚???癲ル슢??蹂좊쨨???낇뀘???????⑤베鍮????녾컯嶺?????????
            {
                RootNode rootNode = chatContainer.CreateNode(typeof(RootNode)) as RootNode;
                rootNode.showName = chatContainer.NowChapter.showName;
                rootNode.saveLocation = chatContainer.NowChapter.saveLocation;
                rootNode.round = new List<string>(chatContainer.NowChapter.round);
                EditorUtility.SetDirty(chatContainer);
                AssetDatabase.SaveAssets();
            }

            // ??????癲ル슢??蹂좊쨨??꿔꺂?????????κ땁???
            for (int i = 0; i < chatContainer.NowChapter.chat.Count; ++i)
            {
                ++firstChatEndIndex;

                // ?????????????쇰뮚?????κ땁???
                ChatNode chatNode = chatContainer.CreateNode(typeof(ChatNode)) as ChatNode;
                chatNode.state = chatContainer.NowChapter.chat[i].state;
                chatNode.type = chatContainer.NowChapter.chat[i].type;
                chatNode.text = chatContainer.NowChapter.chat[i].text;
                chatNode.face = chatContainer.NowChapter.chat[i].face;
                chatNode.textEvent = new List<EChatEvent>(chatContainer.NowChapter.chat[i].textEvent);

                // ????썹땟???????????녿뮝??怨룸렓?
                if (chatNode.state == EChatState.Other)
                {
                    chatNode.position = new Vector2(-125, 150 * (i + 1));
                }
                else
                {
                    chatNode.position = new Vector2(125, 150 * (i + 1));
                }

                if (i == 0)     // ??猷매?댚??癲ル슢??蹂좊쨨??????쇰뮚?????κ땁???
                {
                    RootNode rootNode = chatContainer.nodes[0] as RootNode;
                    rootNode.child = chatNode;
                }
                else        // ????꾤뙴???????쇰뮚?????κ땁???
                {
                    ChatNode chatParentNode = chatContainer.nodes[i] as ChatNode;
                    chatParentNode.child.Add(chatNode);
                }
            }

            // ?꿔꺂???熬곻퐢利? ??? ?꿔꺂???熬곻퐢利??癲ル슢??蹂좊쨨??꿔꺂?????????κ땁???
            {
                ChatNode firstChatEndNode = chatContainer.nodes[firstChatEndIndex] as ChatNode;
                if (firstChatEndNode != null)
                {
                    Vector2 endChatNodePosition = firstChatEndNode.position;
                    //Debug.Log($"?꿔꺂???熬곻퐢利????醫딆┻???: {this.chatContainer.NowChapter.askAndReply.Count}, ????????癲ル슢?????{firstChatEndIndex}");

                    // ???녾컯嶺?????꿔꺂???熬곻퐢利??꿔꺂?????????κ땁???
                    for (int i = 0; i < chatContainer.NowChapter.askAndReply.Count; ++i)
                    {
                        ++askAndReplysCount;

                        // ?꿔꺂???熬곻퐢利??癲ル슢??蹂좊쨨????ㅻ쿋??????ㅻ샑筌?????????????ㅻ쿋??
                        AskNode askNode = chatContainer.CreateNode(typeof(AskNode)) as AskNode;
                        askNode.ask = this.chatContainer.NowChapter.askAndReply[i].ask;
                        askNode.reply = new List<Chat>(this.chatContainer.NowChapter.askAndReply[i].reply);        // ????⑤슢?뽫뵓怨??? ?μ떜媛?걫?? ??⑤슢?뽫뵓怨???
                        askNode.is_UseThis = this.chatContainer.NowChapter.askAndReply[i].is_UseThis;

                        // ???類ｌ┯???癲ル슢??蹂좊쨨??????쇰뮚?????κ땁???
                        firstChatEndNode.child.Add(askNode);

                        // ????썹땟?????繹먮냱?????κ땁???
                        askNode.position = new Vector2(endChatNodePosition.x + i * 400, endChatNodePosition.y + 200);

                        // ???????類ｌ┯???癲ル슢??蹂좊쨨????ㅻ쿋??????κ땁???
                        for (int j = 0; j < chatContainer.NowChapter.askAndReply[i].reply.Count; ++j)
                        {
                            // ???類ｌ┯???癲ル슢??蹂좊쨨????ㅻ쿋??????ㅻ샑筌?????????????ㅻ쿋??
                            ChatNode replyNode = chatContainer.CreateNode(typeof(ChatNode)) as ChatNode;
                            replyNode.state = this.chatContainer.NowChapter.askAndReply[i].reply[j].state;
                            replyNode.type = this.chatContainer.NowChapter.askAndReply[i].reply[j].type;
                            replyNode.text = this.chatContainer.NowChapter.askAndReply[i].reply[j].text;
                            replyNode.face = this.chatContainer.NowChapter.askAndReply[i].reply[j].face;
                            replyNode.textEvent = new List<EChatEvent>(this.chatContainer.NowChapter.askAndReply[i].reply[j].textEvent);

                            // ????썹땟???????????녿뮝??怨룸렓?
                            if (replyNode.state == EChatState.Other)
                            {
                                replyNode.position = new Vector2(askNode.position.x - 100, askNode.position.y + 140 * (j + 1));
                            }
                            else
                            {
                                replyNode.position = new Vector2(askNode.position.x + 100, askNode.position.y + 140 * (j + 1));
                            }

                            // ????쇰뮚??????κ땁???
                            if (j == 0)     // ?꿔꺂???熬곻퐢利?癲ル슢??蹂좊쨨??????쇰뮚?????ㅿ폑?????β뼯爰귨㎘??嚥▲굧????
                            {
                                askNode.child = replyNode;
                            }
                            else
                            {
                                ChatNode chatParentNode = this.chatContainer.nodes[j + firstChatEndIndex + askAndReplysCount] as ChatNode;        // ??猷매?댚??癲ル슢??蹂좊쨨???????鶯ㅺ동???????녿뮝??怨룸렓?
                                if (chatParentNode != null)
                                {
                                    chatParentNode.child.Add(replyNode);
                                }
                            }
                            if (j == chatContainer.NowChapter.askAndReply[i].reply.Count - 1)      // ????⑥レ퓡 ?꿔꺂???????熬곣뫖利???????
                            {
                                askAndReplysCount += j + 1;
                            }
                        }
                    }

                    // ??? ?꿔꺂???熬곻퐢利??꿔꺂?????????κ땁???
                    for (int i = 0; i < chatContainer.NowChapter.lockAskAndReply.Count; i++)
                    {
                        ++lockAskAndReplysCount;

                        // ????????癲ル슢??蹂좊쨨????ㅻ쿋??????κ땁???
                        LockAskNode lockAskNode = chatContainer.CreateNode(typeof(LockAskNode)) as LockAskNode;
                        lockAskNode.evidence = new List<string>(this.chatContainer.NowChapter.lockAskAndReply[i].evidence);
                        lockAskNode.ask = this.chatContainer.NowChapter.lockAskAndReply[i].ask;
                        lockAskNode.reply = new List<Chat>(this.chatContainer.NowChapter.lockAskAndReply[i].reply);        // ????⑤슢?뽫뵓怨??? ?μ떜媛?걫?? ??⑤슢?뽫뵓怨???
                        lockAskNode.is_UseThis = this.chatContainer.NowChapter.lockAskAndReply[i].is_UseThis;

                        // ???類ｌ┯???癲ル슢??蹂좊쨨??????쇰뮚?????κ땁???
                        firstChatEndNode.child.Add(lockAskNode);

                        // ????썹땟?????繹먮냱?????κ땁???
                        lockAskNode.position = new Vector2(endChatNodePosition.x + (chatContainer.NowChapter.askAndReply.Count * 400) + (i * 400), endChatNodePosition.y + 200);

                        // ???????類ｌ┯???癲ル슢??蹂좊쨨????ㅻ쿋??????κ땁???
                        for (int j = 0; j < this.chatContainer.NowChapter.lockAskAndReply[i].reply.Count; ++j)
                        {
                            ChatNode replyNode = chatContainer.CreateNode(typeof(ChatNode)) as ChatNode;
                            replyNode.state = this.chatContainer.NowChapter.lockAskAndReply[i].reply[j].state;
                            replyNode.type = this.chatContainer.NowChapter.lockAskAndReply[i].reply[j].type;
                            replyNode.text = this.chatContainer.NowChapter.lockAskAndReply[i].reply[j].text;
                            replyNode.face = this.chatContainer.NowChapter.lockAskAndReply[i].reply[j].face;
                            replyNode.textEvent = new List<EChatEvent>(this.chatContainer.NowChapter.lockAskAndReply[i].reply[j].textEvent);

                            // ????썹땟???????????녿뮝??怨룸렓?
                            if (replyNode.state == EChatState.Other)
                            {
                                replyNode.position = new Vector2(lockAskNode.position.x - 100, lockAskNode.position.y + 140 * (j + 1));
                            }
                            else
                            {
                                replyNode.position = new Vector2(lockAskNode.position.x + 100, lockAskNode.position.y + 140 * (j + 1));
                            }

                            // ????쇰뮚??????κ땁???
                            if (j == 0)     // ?꿔꺂???熬곻퐢利?癲ル슢??蹂좊쨨??????쇰뮚?????ㅿ폑?????β뼯爰귨㎘??嚥▲굧????
                            {
                                lockAskNode.child = replyNode;
                            }
                            else
                           {
                                ChatNode chatParentNode = this.chatContainer.nodes[j + firstChatEndIndex + askAndReplysCount + lockAskAndReplysCount] as ChatNode;        // ??猷매?댚??癲ル슢??蹂좊쨨???????鶯ㅺ동???????녿뮝??怨룸렓?
                                if (chatParentNode != null)
                                {
                                    chatParentNode.child.Add(replyNode);
                                }
                            }
                            if (j == chatContainer.NowChapter.lockAskAndReply[i].reply.Count - 1)      // ????⑥レ퓡 ?꿔꺂???????熬곣뫖利???????
                            {
                                lockAskAndReplysCount += j + 1;
                            }
                        }
                    }
                }
            }
        }

        public void SaveChatSystem()        // ??醫딆┫???????嚥싳쇎紐????蹂μ땃?
        {
            chatContainer.NowChapter.chat.Clear();
            chatContainer.NowChapter.askAndReply.Clear();
            chatContainer.NowChapter.lockAskAndReply.Clear();

            // nodes ??잙갭큔?딆뼍吏?癲ル슢?????????썹땟???癲ル슢?????? ??嶺?筌????ㅳ럶???
            int chatIndex = 0;       // ??醫딆쓧????臾먯맗??????????됲닓 ???類ｌ┯????繹먮굛???癲ル슢?????
            int askIndex = 0;       // ???繹먮굞?????繹먮굛???꿔꺂???熬곻퐢利???繹먮굛???癲ル슢?????
            int lockAskIndex = 0;       // ?꿔꺂???熬곻퐢利????繹먮굞???????볥궖????? ?꿔꺂???熬곻퐢利??
            bool firstChatEnd = false;

            int nowAskIndex = 0;        // ?꿔꺂??????꿔꺂???熬곻퐢利???癲ル슢?????
            int nowReplyIndex = 0;      // ?꿔꺂??????꿔꺂???熬곻퐢利??????⑥ろ맖???????癲ル슢?????
            int nowReplysCountIndex = 0;      // ?꿔꺂??????꿔꺂???熬곻퐢利????????醫딆┻????癲ル슢????? ????썹땟????잙갭큔?딆뼍吏?癲ル슢???븍┛??癲ル슢???????
            bool lockAskStart = false;      // ??? ?꿔꺂???熬곻퐢利????嶺뚮??ｆ뤃??嶺?????ル봾諭?

            // ?꿔꺂???熬곻퐢利????????醫딆┻???釉랁닑?????嚥싳쇎紐?????곕뼁???ㅻ쿋筌???잙갭큔?딆뼍吏??
            List<int> replysCount = new List<int>();            // -1?? 

            if (chatContainer.nodes[0] != null)     // ??猷매?댚??癲ル슢??蹂좊쨨?誘⑹º???쎛 ?????딅젩??
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
                var children = chatContainer.GetChild(n);       // ???嶺????醫딆쓧??癲ル슢???몄쒜嚥▲룗??
                children.ForEach(c =>
                {
                    ChatNode chatNode = c as ChatNode;      // ???類ｌ┯???癲ル슢??蹂좊쨨?????
                    if (chatNode != null)
                    {
                        if (firstChatEnd == false)
                        {
                            // ????沅?????????꿔꺂?????????ㅔ??
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
                            // ??????????嚥싳쇎紐?????곕뼁??????
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

                                    if (nowReplyIndex + 1 > replysCount[nowReplysCountIndex])      // ??????醫딆┻???釉랁닑????嶺?????ル봾諭?
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

                                if (nowReplyIndex + 1 > replysCount[nowReplysCountIndex])      // ??????醫딆┻???釉랁닑????嶺?????ル봾諭?
                                {
                                    nowAskIndex++;
                                    nowReplysCountIndex++;
                                    nowReplyIndex = 0;
                                }
                            }
                        }
                    }

                    AskNode askNode = c as AskNode;     // ?꿔꺂???熬곻퐢利??癲ル슢??蹂좊쨨?????
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
                            replysCount.Add(-1);        // ???ш낄猷??????類ｌ┯?????嶺뚮??ｆ뤃?????딅젩??
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

            DeleteElements(graphElements);      // GraphElement's node delete

            graphViewChanged += OnGraphViewChanged;

            // node Create
            this.chatContainer.nodes.ForEach(n => CreateNodeView(n));

            // Line Create
            this.chatContainer.nodes.ForEach(n =>
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

            // ??????꿔꺂?????????κ땁???
            this.chatContainer.SortChildAndIndex();
        }

        private NodeView FindNodeView(Node node)
        {
            return GetNodeByGuid(node.guid) as NodeView;
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove != null)       // ?꿔꺂????????????ル뒇??????繹먮겧嫄х솾?
            {
                graphViewChange.elementsToRemove.ForEach(elem =>
                {
                    var nodeView = elem as NodeView;
                    if (nodeView != null)
                    {
                        chatContainer.DeleteNode(nodeView.node);        // ?꿔꺂???????沃샩살??
                    }

                    var edge = elem as Edge;        // ????쇰뮚???
                    if (edge != null)
                    {
                        NodeView parent = edge.output.node as NodeView;
                        NodeView child = edge.input.node as NodeView;

                        chatContainer.RemoveChild(parent.node, child.node);
                    }
                });
            }

            if (graphViewChange.edgesToCreate != null)      // ??????쇰뮚?????κ땁???
            {
                graphViewChange.edgesToCreate.ForEach(edge =>
                {
                    NodeView parent = edge.output.node as NodeView;
                    NodeView child = edge.input.node as NodeView;

                    chatContainer.AddChild(parent.node, child.node);
                });
            }

           /* if (graphViewChange.movedElements != null)      // ???꿔꺂?????????ル뒇??????繹먮겧嫄х솾??癲ル슢??節?????κ땁???
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
            Node node = chatContainer.CreateNode(type);      // ?癲ル슢??蹂좊쨨????꾩룆???
            node.position = position;
            CreateNodeView(node);       // ??⑤슢??????????繹먮냱議?????ㅻ쿋??????ㅔ??
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
            return ports.ToList().Where(x => x.direction != startPort.direction).ToList();      // input output???????봔???????쇰뮚????醫딆쓧???
        }
    }
}