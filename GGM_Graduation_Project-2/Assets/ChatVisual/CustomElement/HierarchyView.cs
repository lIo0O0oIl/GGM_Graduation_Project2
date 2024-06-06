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

            // IMGUI ?????곴퐣 ??륁뵠?????쇨갯 筌띾슢諭??곻폒疫?
            Clear();

            ScrollView scrollView = new ScrollView(ScrollViewMode.Vertical);
            scrollView.style.marginBottom = 5;
            scrollView.Add(new Label("Chapters :"));
            /*for (int i = 0; i < chatContainer.MainChapter.Count; ++i)
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
            Add(scrollView);*/
        }

/*        private void ChangeChapter(int index)
        {
            //chatView.SaveChatSystem();      // 筌왖疫?筌?벤苑????館鍮먧틠?⑤┛ 
            chatContainer.ChangeNowChapter(index);      // 筌?벤苑???띾┛疫?
            //chatView.LoadChatSystem(chatContainer);         // 筌?벤苑?嚥≪뮆諭??곻폒疫?
            chatView.PopulateView();        // 癰귣똻???野?域밸챶??틠?⑤┛
        }

        private void DeleteChapter(int index)
        {
            chatContainer.MainChapter.RemoveAt(index);
            UpdateHierarchy(chatContainer, chatView);
        }*/
    }
}
