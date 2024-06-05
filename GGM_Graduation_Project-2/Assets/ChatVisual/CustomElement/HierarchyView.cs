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

        public void UpdateHierarchy(ChatContainer _chatContainer, ChatView _chatView)
        {
            chatContainer = _chatContainer;
            chatView = _chatView;

            // IMGUI 사용해서 하이어라키창 만들어주기
            Clear();

            ScrollView scrollView = new ScrollView(ScrollViewMode.Vertical);
            scrollView.style.marginBottom = 5;
            scrollView.Add(new Label("Chapters :"));
            for (int i = 0; i < chatContainer.MainChapter.Count; ++i)
            {
                int index = i;
                string name = "";
                if (chatContainer.MainChapter[i].showName == null || chatContainer.MainChapter[i].showName == "")
                {
                    name = "???";
                }
                else
                {
                    name = chatContainer.MainChapter[i].showName;
                }

                Button button = new Button(() => ChangeChapter(index));
                button.style.flexDirection = FlexDirection.Row;
                button.style.justifyContent = Justify.Center;
                button.style.flexGrow = 1;

                TextElement nameText = new TextElement();
                nameText.text = name;
                button.Add(nameText);

                TextElement indexText = new TextElement();
                indexText.text = " - " + index.ToString();
                indexText.style.color = Color.gray;
                button.Add(indexText);

                var deleteButton = new Button(() => DeleteChapter(index))
                {
                    text = "Delete"
                };
                var set = new VisualElement();
                set.style.flexDirection = FlexDirection.Row;
                set.Add(button);
                set.Add(deleteButton);
                scrollView.Add(set);
            }
            Add(scrollView);
        }

        private void ChangeChapter(int index)
        {
            //chatView.SaveChatSystem();      // 지금 챕터 저장해주기 
            chatContainer.ChangeNowChapter(index);      // 챕터 넘기기
            //chatView.LoadChatSystem(chatContainer);         // 챕터 로드해주기
            chatView.PopulateView();        // 보이는 것 그려주기
        }

        private void DeleteChapter(int index)
        {
            chatContainer.MainChapter.RemoveAt(index);
            UpdateHierarchy(chatContainer, chatView);
        }
    }
}
