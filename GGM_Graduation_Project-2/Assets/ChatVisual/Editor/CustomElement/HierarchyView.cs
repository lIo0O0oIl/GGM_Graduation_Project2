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

            ScrollView scrollView = new ScrollView(ScrollViewMode.Vertical);
            scrollView.style.marginBottom = 5;
            scrollView.Add(new Label("Humans :"));
            for(int i = 0;  i < chatContainer.chatTrees.Count; i++)
            {
                int index = i;

                Button button = new Button(() => ChangeHuman(index));
                button.style.flexDirection = FlexDirection.Row;
                button.style.justifyContent = Justify.Center;
                button.style.flexGrow = 1;

                TextElement nameText = new TextElement();
                nameText.text = chatContainer.chatTrees[i].name;
                button.Add(nameText);

                TextElement indexText = new TextElement();
                indexText.text = " - " + index.ToString();
                indexText.style.color = Color.gray;
                button.Add(indexText);

                var deleteButton = new Button(() => DeleteHuman(index))
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

        private void ChangeHuman(int index)
        {
            chatView.PopulateView(chatContainer.chatTrees[index]);
            inspectorView.Clear();
        }

        private void DeleteHuman(int index)
        {
            if (chatContainer.chatTrees.Count <= 1) return;

            ChatTree removeChatTree = chatContainer.chatTrees[index];
            chatContainer.chatTrees.Remove(removeChatTree);
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(removeChatTree));      // Delete directly from memory
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            inspectorView.Clear();

            for (int i = 0; i < chatContainer.chatTrees.Count; i++)
            {
                chatView.PopulateView(chatContainer.chatTrees[i]);
                break;
            }

            UpdateHierarchy();
        }
    }
}
