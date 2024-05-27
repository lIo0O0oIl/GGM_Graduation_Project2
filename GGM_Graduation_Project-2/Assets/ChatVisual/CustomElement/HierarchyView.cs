using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ChatVisual
{
    public class HierarchyView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<HierarchyView, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits { }

        private ChatContainer chatContainer;
        private ChatView chatView;

        public void MakeHierarchy(ChatContainer _chatContainer, ChatView _chatView)
        {
            chatContainer = _chatContainer;
            chatView = _chatView;

            // IMGUI 사용해서 하이어라키창 만들어주기
            Clear();

            ScrollView scrollView = new ScrollView(ScrollViewMode.Vertical);
            scrollView.Add(new Label("Chapters :"));
            for (int i = 0; i < chatContainer.MainChapter.Count; ++i)
            {
                int index = i;
                var button = new Button(() => ChangeChapter(index))
                {
                    text = chatContainer.MainChapter[i].showName
                };
                scrollView.Add(button);
            }
            Add(scrollView);
        }

        private void ChangeChapter(int index)
        {
            chatView.SaveChatSystem();
            chatContainer.ChangeNowChapter(index);
            chatView.LoadChatSystem(chatContainer); 
            chatView.PopulateView();
        }
    }
}
