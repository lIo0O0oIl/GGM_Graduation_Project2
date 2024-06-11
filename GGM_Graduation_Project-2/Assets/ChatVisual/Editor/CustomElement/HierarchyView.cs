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

        int index = 0;

        public void UpdateHierarchy(ChatContainer _chatContainer, ChatView _chatView)
        {
            chatContainer = _chatContainer;
            chatView = _chatView;

            // IMGUI
            Clear();

            Debug.Log("Update Hierarchy");

            ScrollView scrollView = new ScrollView(ScrollViewMode.Vertical);
            scrollView.style.marginBottom = 5;
            scrollView.Add(new Label("Chapters :"));
            foreach(string name in chatContainer.HumanAndChatDictionary.Keys)
            {
                /*if (chatContainer.MainChapter[i].showName == null || chatContainer.MainChapter[i].showName == "")
                {
                    name = "???";
                }
                else
                {
                    name = chatContainer.MainChapter[i].showName;
                }*/

                Button button = new Button(() => ChangeChapter(name));
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

                var deleteButton = new Button(() => DeleteChapter(name))
                {
                    text = "Delete"
                };
                var set = new VisualElement();
                set.style.flexDirection = FlexDirection.Row;
                set.Add(button);
                set.Add(deleteButton);
                scrollView.Add(set);

                index++;
            }
            Add(scrollView);
        }

        private void ChangeChapter(string key)
        {
            //chatView.SaveChatSystem();      // 嶺뚯솘???嶺?踰ㅸ땻????繞③뜮癒㏉떊??ㅲ뵛 
            chatContainer.ChangeNowChapter(key);      // 嶺?踰ㅸ땻????얄뵛??
            //chatView.LoadChatSystem(chatContainer);         // 嶺?踰ㅸ땻??β돦裕녻キ??怨삵룖??
            chatView.PopulateView();        // ?곌랜???????잙갭梨?????ㅲ뵛
        }

        private void DeleteChapter(string key)
        {
            chatContainer.HumanAndChatDictionary.Remove(key);
            UpdateHierarchy(chatContainer, chatView);
        }
    }
}
