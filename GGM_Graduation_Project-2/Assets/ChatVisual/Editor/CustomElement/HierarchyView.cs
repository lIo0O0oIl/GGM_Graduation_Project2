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
        private InspectorView inspectorView;

        private int index = 0;

        public void InitHierarchy(ChatContainer _chatContainer, ChatView _chatView, InspectorView _inspectorView)
        {
            chatContainer = _chatContainer;
            chatView = _chatView;
            inspectorView = _inspectorView;
        }

        public void UpdateHierarchy()
        {
            // IMGUI
            Clear();

            Debug.Log("Update Hierarchy");

            ScrollView scrollView = new ScrollView(ScrollViewMode.Vertical);
            scrollView.style.marginBottom = 5;
            scrollView.Add(new Label("Chapters :"));
            foreach(string name in chatContainer.HumanAndChatDictionary.Keys)
            {

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
            chatView.SaveChatName();   
            chatContainer.ChangeNowChapter(key);    
            chatView.LoadChatData(chatContainer);       
            chatView.PopulateView();
            inspectorView.Clear();
        }

        private void DeleteChapter(string key)
        {
            chatContainer.HumanAndChatDictionary.Remove(key);
            UpdateHierarchy();
        }
    }
}
