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

            // Delete what was drawn before
            //chatContainer.nowChatNodeList.Clear();

            Debug.Log(chatContainer.HumanAndChatDictionary[chatContainer.nowHumanName].Count);

            // Unconditional root nodes exist
            RootNode makeRootNode = chatContainer.CreateNode(typeof(RootNode)) as RootNode;
            chatContainer.HumanAndChatDictionary[chatContainer.nowHumanName].Add(makeRootNode);

            /*       foreach (Node now in chatContainer.HumanAndChatDictionary[chatContainer.nowHumanName])
                   {
                       if (now is RootNode rootNode)
                       {
                           RootNode makeRootNode = chatContainer.CreateNode(typeof(RootNode)) as RootNode;
                           makeRootNode.showName = rootNode.showName;
                           makeRootNode.loadFileNameList = rootNode.loadFileNameList;
                       }
                   }*/

            /*          // ???????饔낅떽?????怨뚮옩鴉딅퀫???耀붾굝???????????欲꼲????
                      for (int i = 0; i < chatContainer.NowChapter.chat.Count; ++i)
                      {
                          ++firstChatEndIndex;

                          // ???????????????⑤벡瑜?????欲꼲????
                          ChatNode chatNode = chatContainer.CreateNode(typeof(ChatNode)) as ChatNode;
                          chatNode.state = chatContainer.NowChapter.chat[i].state;
                          chatNode.type = chatContainer.NowChapter.chat[i].type;
                          chatNode.text = chatContainer.NowChapter.chat[i].text;
                          chatNode.face = chatContainer.NowChapter.chat[i].face;
                          chatNode.textEvent = new List<EChatEvent>(chatContainer.NowChapter.chat[i].textEvent);

                          // ?????獄쏅챶留??????????????밸쫫?????욱룏??
                          if (chatNode.state == EChatState.Other)
                          {
                              chatNode.position = new Vector2(-125, 150 * (i + 1));
                          }
                          else
                          {
                              chatNode.position = new Vector2(125, 150 * (i + 1));
                          }

                          if (i == 0)     // ????룸ħ瑗?????饔낅떽?????怨뚮옩鴉딅퀫?????????⑤벡瑜?????欲꼲????
                          {
                              RootNode rootNode = chatContainer.nodes[0] as RootNode;
                              rootNode.child = chatNode;
                          }
                          else        // ?????ш낄????????????⑤벡瑜?????欲꼲????
                          {
                              ChatNode chatParentNode = chatContainer.nodes[i] as ChatNode;
                              chatParentNode.child.Add(chatNode);
                          }
                      }

                      // ?耀붾굝??????????壤? ??? ?耀붾굝??????????壤???饔낅떽?????怨뚮옩鴉딅퀫???耀붾굝???????????欲꼲????
                      {
                          ChatNode firstChatEndNode = chatContainer.nodes[firstChatEndIndex] as ChatNode;
                          if (firstChatEndNode != null)
                          {
                              Vector2 endChatNodePosition = firstChatEndNode.position;
                              //Debug.Log($"?耀붾굝??????????壤???????ル뒌????: {this.chatContainer.NowChapter.askAndReply.Count}, ?????????饔낅떽???????{firstChatEndIndex}");

                              // ?????醫딇떍??????耀붾굝??????????壤??耀붾굝???????????欲꼲????
                              for (int i = 0; i < chatContainer.NowChapter.askAndReply.Count; ++i)
                              {
                                  ++askAndReplysCount;

                                  // ?耀붾굝??????????壤???饔낅떽?????怨뚮옩鴉딅퀫????????꾨굴?????????욱뒅?????????????????꾨굴??
                                  AskNode askNode = chatContainer.CreateNode(typeof(AskNode)) as AskNode;
                                  askNode.ask = this.chatContainer.NowChapter.askAndReply[i].ask;
                                  askNode.reply = new List<Chat>(this.chatContainer.NowChapter.askAndReply[i].reply);        // ???????곕츥?嶺뚮?爰????? ????濡?씀?濾?? ?????곕츥?嶺뚮?爰?????
                                  askNode.is_UseThis = this.chatContainer.NowChapter.askAndReply[i].is_UseThis;

                                  // ???癲ル슢???????饔낅떽?????怨뚮옩鴉딅퀫?????????⑤벡瑜?????欲꼲????
                                  firstChatEndNode.child.Add(askNode);

                                  // ?????獄쏅챶留??????μ떜媛?걫??????欲꼲????
                                  askNode.position = new Vector2(endChatNodePosition.x + i * 400, endChatNodePosition.y + 200);

                                  // ???????癲ル슢???????饔낅떽?????怨뚮옩鴉딅퀫????????꾨굴??????欲꼲????
                                  for (int j = 0; j < chatContainer.NowChapter.askAndReply[i].reply.Count; ++j)
                                  {
                                      // ???癲ル슢???????饔낅떽?????怨뚮옩鴉딅퀫????????꾨굴?????????욱뒅?????????????????꾨굴??
                                      ChatNode replyNode = chatContainer.CreateNode(typeof(ChatNode)) as ChatNode;
                                      replyNode.state = this.chatContainer.NowChapter.askAndReply[i].reply[j].state;
                                      replyNode.type = this.chatContainer.NowChapter.askAndReply[i].reply[j].type;
                                      replyNode.text = this.chatContainer.NowChapter.askAndReply[i].reply[j].text;
                                      replyNode.face = this.chatContainer.NowChapter.askAndReply[i].reply[j].face;
                                      replyNode.textEvent = new List<EChatEvent>(this.chatContainer.NowChapter.askAndReply[i].reply[j].textEvent);

                                      // ?????獄쏅챶留??????????????밸쫫?????욱룏??
                                      if (replyNode.state == EChatState.Other)
                                      {
                                          replyNode.position = new Vector2(askNode.position.x - 100, askNode.position.y + 140 * (j + 1));
                                      }
                                      else
                                      {
                                          replyNode.position = new Vector2(askNode.position.x + 100, askNode.position.y + 140 * (j + 1));
                                      }

                                      // ??????⑤벡瑜??????欲꼲????
                                      if (j == 0)     // ?耀붾굝??????????壤??饔낅떽?????怨뚮옩鴉딅퀫?????????⑤벡瑜???????源낆쭍?????黎앸럽????룸돥??????汝뷴젆?琉????
                                      {
                                          askNode.child = replyNode;
                                      }
                                      else
                                      {
                                          ChatNode chatParentNode = this.chatContainer.nodes[j + firstChatEndIndex + askAndReplysCount] as ChatNode;        // ????룸ħ瑗?????饔낅떽?????怨뚮옩鴉딅퀫??????????????????????밸쫫?????욱룏??
                                          if (chatParentNode != null)
                                          {
                                              chatParentNode.child.Add(replyNode);
                                          }
                                      }
                                      if (j == chatContainer.NowChapter.askAndReply[i].reply.Count - 1)      // ?????????獄??耀붾굝?????????????밸븶筌믩끃????????
                                      {
                                          askAndReplysCount += j + 1;
                                      }
                                  }
                              }

                              // ??? ?耀붾굝??????????壤??耀붾굝???????????欲꼲????
                              for (int i = 0; i < chatContainer.NowChapter.lockAskAndReply.Count; i++)
                              {
                                  ++lockAskAndReplysCount;

                                  // ?????????饔낅떽?????怨뚮옩鴉딅퀫????????꾨굴??????欲꼲????
                                  LockAskNode lockAskNode = chatContainer.CreateNode(typeof(LockAskNode)) as LockAskNode;
                                  lockAskNode.evidence = new List<string>(this.chatContainer.NowChapter.lockAskAndReply[i].evidence);
                                  lockAskNode.ask = this.chatContainer.NowChapter.lockAskAndReply[i].ask;
                                  lockAskNode.reply = new List<Chat>(this.chatContainer.NowChapter.lockAskAndReply[i].reply);        // ???????곕츥?嶺뚮?爰????? ????濡?씀?濾?? ?????곕츥?嶺뚮?爰?????
                                  lockAskNode.is_UseThis = this.chatContainer.NowChapter.lockAskAndReply[i].is_UseThis;

                                  // ???癲ル슢???????饔낅떽?????怨뚮옩鴉딅퀫?????????⑤벡瑜?????欲꼲????
                                  firstChatEndNode.child.Add(lockAskNode);

                                  // ?????獄쏅챶留??????μ떜媛?걫??????欲꼲????
                                  lockAskNode.position = new Vector2(endChatNodePosition.x + (chatContainer.NowChapter.askAndReply.Count * 400) + (i * 400), endChatNodePosition.y + 200);

                                  // ???????癲ル슢???????饔낅떽?????怨뚮옩鴉딅퀫????????꾨굴??????欲꼲????
                                  for (int j = 0; j < this.chatContainer.NowChapter.lockAskAndReply[i].reply.Count; ++j)
                                  {
                                      ChatNode replyNode = chatContainer.CreateNode(typeof(ChatNode)) as ChatNode;
                                      replyNode.state = this.chatContainer.NowChapter.lockAskAndReply[i].reply[j].state;
                                      replyNode.type = this.chatContainer.NowChapter.lockAskAndReply[i].reply[j].type;
                                      replyNode.text = this.chatContainer.NowChapter.lockAskAndReply[i].reply[j].text;
                                      replyNode.face = this.chatContainer.NowChapter.lockAskAndReply[i].reply[j].face;
                                      replyNode.textEvent = new List<EChatEvent>(this.chatContainer.NowChapter.lockAskAndReply[i].reply[j].textEvent);

                                      // ?????獄쏅챶留??????????????밸쫫?????욱룏??
                                      if (replyNode.state == EChatState.Other)
                                      {
                                          replyNode.position = new Vector2(lockAskNode.position.x - 100, lockAskNode.position.y + 140 * (j + 1));
                                      }
                                      else
                                      {
                                          replyNode.position = new Vector2(lockAskNode.position.x + 100, lockAskNode.position.y + 140 * (j + 1));
                                      }

                                      // ??????⑤벡瑜??????欲꼲????
                                      if (j == 0)     // ?耀붾굝??????????壤??饔낅떽?????怨뚮옩鴉딅퀫?????????⑤벡瑜???????源낆쭍?????黎앸럽????룸돥??????汝뷴젆?琉????
                                      {
                                          lockAskNode.child = replyNode;
                                      }
                                      else
                                     {
                                          ChatNode chatParentNode = this.chatContainer.nodes[j + firstChatEndIndex + askAndReplysCount + lockAskAndReplysCount] as ChatNode;        // ????룸ħ瑗?????饔낅떽?????怨뚮옩鴉딅퀫??????????????????????밸쫫?????욱룏??
                                          if (chatParentNode != null)
                                          {
                                              chatParentNode.child.Add(replyNode);
                                          }
                                      }
                                      if (j == chatContainer.NowChapter.lockAskAndReply[i].reply.Count - 1)      // ?????????獄??耀붾굝?????????????밸븶筌믩끃????????
                                      {
                                          lockAskAndReplysCount += j + 1;
                                      }
                                  }
                              }
                          }
                      }*/
        }

        /*
        public void SaveChatSystem()        // ?????ル뒌????????????嚥〓끃異?????怨뚮뼺?怨뺥닠?
        {
            chatContainer.NowChapter.chat.Clear();
            chatContainer.NowChapter.askAndReply.Clear();
            chatContainer.NowChapter.lockAskAndReply.Clear();

            // nodes ?????얠뺏?????깅젿癲??饔낅떽????????????獄쏅챶留????饔낅떽???????? ???????????紐꾩몷???
            int chatIndex = 0;       // ?????ル뒌????????⑸뵃彛??????????????????癲ル슢????????μ떜媛?걫?????饔낅떽???????
            int askIndex = 0;       // ????μ떜媛?걫???????μ떜媛?걫????耀붾굝??????????壤????μ떜媛?걫?????饔낅떽???????
            int lockAskIndex = 0;       // ?耀붾굝??????????壤?????μ떜媛?걫?????????곌떽釉붾????? ?耀붾굝??????????壤??
            bool firstChatEnd = false;

            int nowAskIndex = 0;        // ?耀붾굝????????耀붾굝??????????壤????饔낅떽???????
            int nowReplyIndex = 0;      // ?耀붾굝????????耀붾굝??????????壤?????????????????????饔낅떽???????
            int nowReplysCountIndex = 0;      // ?耀붾굝????????耀붾굝??????????壤???????????ル뒌??????饔낅떽??????? ?????獄쏅챶留???????얠뺏?????깅젿癲??饔낅떽???????곗뵚????饔낅떽?????????
            bool lockAskStart = false;      // ??? ?耀붾굝??????????壤?????轅붽틓???壤굿??걜???????????ル뭸??

            // ?耀붾굝??????????壤???????????ル뒌??????怨쀫엥??????????嚥〓끃異???????ㅻ쿋???????꾨굴???????얠뺏?????깅젿癲??
            List<int> replysCount = new List<int>();            // -1?? 

            if (chatContainer.nodes[0] != null)     // ????룸ħ瑗?????饔낅떽?????怨뚮옩鴉딅퀫??雅?퍔瑗?땟?ъ???????쎛 ????????곸죩??
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
                var children = chatContainer.GetChild(n);       // ???????????ル뒌????饔낅떽?????嶺뚮ㅎ?닺짆?汝뷴젆?????
                children.ForEach(c =>
                {
                    ChatNode chatNode = c as ChatNode;      // ???癲ル슢???????饔낅떽?????怨뚮옩鴉딅퀫??????
                    if (chatNode != null)
                    {
                        if (firstChatEnd == false)
                        {
                            // ??????????????耀붾굝??????????????
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
                            // ??????????????嚥〓끃異???????ㅻ쿋????????
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

                                    if (nowReplyIndex + 1 > replysCount[nowReplysCountIndex])      // ?????????ル뒌??????怨쀫엥??????????????ル뭸??
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

                                if (nowReplyIndex + 1 > replysCount[nowReplysCountIndex])      // ?????????ル뒌??????怨쀫엥??????????????ル뭸??
                                {
                                    nowAskIndex++;
                                    nowReplysCountIndex++;
                                    nowReplyIndex = 0;
                                }
                            }
                        }
                    }

                    AskNode askNode = c as AskNode;     // ?耀붾굝??????????壤???饔낅떽?????怨뚮옩鴉딅퀫??????
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
                            replysCount.Add(-1);        // ?????熬곥끇???????癲ル슢?????????轅붽틓???壤굿??걜????????곸죩??
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
        */

        public void PopulateView()
        {
            graphViewChanged -= OnGraphViewChanged;

            DeleteElements(graphElements);      // GraphElement's node delete

            graphViewChanged += OnGraphViewChanged;

            // node Create
            this.chatContainer.HumanAndChatDictionary[chatContainer.nowHumanName].ForEach(n => CreateNodeView(n));

            // Line Create
            //this.chatContainer.HumanAndChatDictionary[chatContainer.nowHumanName].ForEach(n =>
            //{
            //    var children = this.chatContainer.GetChild(n);
            //    NodeView parent = FindNodeView(n);
            //    children.ForEach(c =>
            //    {
            //        NodeView child = FindNodeView(c);
            //        Edge edge = parent.output.ConnectTo(child.input);
            //        AddElement(edge);
            //    });
            //});

            // ??????耀붾굝???????????欲꼲????
            //this.chatContainer.SortChildAndIndex();
        }

        
        private NodeView FindNodeView(Node node)
        {
            return GetNodeByGuid(node.guid) as NodeView;
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove != null)       // ?耀붾굝?????????????????????????μ떜媛?걫?롪퍊?붺댚???
            {
                graphViewChange.elementsToRemove.ForEach(elem =>
                {
                    var nodeView = elem as NodeView;
                    if (nodeView != null)
                    {
                        chatContainer.DeleteNode(nodeView.node);        // ?耀붾굝????????????붺댆????
                    }

                    var edge = elem as Edge;        // ??????⑤벡瑜???
                    if (edge != null)
                    {
                        NodeView parent = edge.output.node as NodeView;
                        NodeView child = edge.input.node as NodeView;

                        //chatContainer.RemoveChild(parent.node, child.node);
                    }
                });
            }

            if (graphViewChange.edgesToCreate != null)      // ????????⑤벡瑜?????欲꼲????
            {
                graphViewChange.edgesToCreate.ForEach(edge =>
                {
                    NodeView parent = edge.output.node as NodeView;
                    NodeView child = edge.input.node as NodeView;

                    //chatContainer.AddChild(parent.node, child.node);
                });
            }

           /* if (graphViewChange.movedElements != null)      // ???耀붾굝??????????????????????μ떜媛?걫?롪퍊?붺댚?????饔낅떽????鶯ㅺ동??????欲꼲????
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
            Node node = chatContainer.CreateNode(type);      // ??饔낅떽?????怨뚮옩鴉딅퀫??????ш끽維뽳쭩???
            node.position = position;
            CreateNodeView(node);       // ?????곕츥???????????μ떜媛?걫?繹먃?????????꾨굴?????????
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
            return ports.ToList().Where(x => x.direction != startPort.direction).ToList();      // input output?????????낇뀘??????????⑤벡瑜???????ル뒌????
        }
    }
}