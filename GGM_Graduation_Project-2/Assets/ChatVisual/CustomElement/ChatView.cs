using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace ChatVisual
{
    public class ChatView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<ChatView, UxmlTraits> { }
        public class UmalTraits : UxmlTraits { }

        public ChatView()
        {
            Insert(0, new GridBackground());        // 그리드 넣기

            this.AddManipulator(new ContentZoomer());       // 줌기능 조작 추가
            this.AddManipulator(new ContentDragger());  // 컨탠츠 드래그 가능
            this.AddManipulator(new SelectionDragger());    // 선택해준거 움직이기
            //this.AddManipulator(new RectangleSelector());   // 네모 만들어주기  조작들 추가
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)       // 우클릭 했을 때 나올 메뉴들
        {
            /*if (??? == null)
            {
                evt.StopPropagation();      // 이벤트 전파 금지
                return;
            }*/
            Debug.Log("우클릭함");
        }

    }
}