using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System;

namespace ChatVisual
{
    public class ChatView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<ChatView, UxmlTraits> { }
        public class UmalTraits : UxmlTraits { }

        private ChatContainer chatContainer;        // 쳇팅들 담겨있는 곳.

        public Action<NodeView> OnNodeSelected;     // 내가 눌렸다고 알려줌.

        public ChatView()
        {
            Insert(0, new GridBackground());        // 그리드 넣기

            this.AddManipulator(new ContentZoomer());       // 줌기능 조작 추가
            this.AddManipulator(new ContentDragger());  // 컨탠츠 드래그 가능
            this.AddManipulator(new SelectionDragger());    // 선택해준거 움직이기
            //this.AddManipulator(new RectangleSelector());   // 네모 만들어주기  조작들 추가
        }

/*        private void CreateNode(Type type, Vector2 position)
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
        }*/

        public void PopulateView(ChatContainer chatContainer)
        {
            this.chatContainer = chatContainer;

            // 쳇 컨테이너 눌러줌.
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
                //evt.menu.AppendAction("ChatNode", (a) => { CreateNode(type, nodePosition); });
            }
        }

    }
}