using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEditorInternal;
using Codice.Client.Common;

namespace ChatVisual
{
    public class InspectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits { }

        private int enumValue;
        private int enumValue2;

        private List<EChatEvent> chatEventList = new List<EChatEvent>();     // ???類ｌ┯???
        private List<string> evidenceList = new List<string>();     // ?꿔꺂?ｉ뜮?뚮쑏癰귥쥓夷??
        private List<string> roundList = new List<string>();        // ??⑤슢??????鶯???濚밸Ŧ?뤺짆???????쀫굞?←춯?????????

        private bool is_Expand = false;
        private bool is_LoadList = false;

        public void UpdateInspector(NodeView node)      // ????썼キ???癲ル슢??蹂좊쨨?誘⑹º???쎛 ?????봔饔끸뮧??ш낄猷???
        {
            Clear();        // ???β뼯?蹂λ뤀?亦껋꼨援????꿔꺂??袁ㅻ븶?癲?????ㅿ폍壤??

            is_Expand = false;
            is_LoadList = false;

            var container = new IMGUIContainer();
           /* container.onGUIHandler = () =>
            {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.fontSize = 20;
                GUILayout.Label($"{node.node.GetType().Name}", style);

                bool is_ChildExist = false;
                switch (node.node)
                {
                    case RootNode:
                        {
                            RootNode rootNode = node.node as RootNode;
                            if (rootNode.child != null) is_ChildExist = true;
                            GUILayout.Space(15);

                            rootNode.showName = EditorGUILayout.TextField("ShowName", rootNode.showName, EditorStyles.textArea);     // ????꾤뙴??醫롫븸??됱삩?
                            GUILayout.Space(5);

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("SaveLocation");
                            GUILayout.Space(50);
                            rootNode.saveLocation = (ESaveLocation)EditorGUILayout.EnumPopup(rootNode.saveLocation);        // ?????얜Ŧ?嶺??熬곣뫖利?????
                            GUILayout.EndHorizontal();
                            GUILayout.Space(10);

                            // round ???ㅻ쿋??
                            if (!is_LoadList)
                            {
                                roundList = new List<string>(rootNode.round);
                                is_LoadList = true;
                            }
                            GUIStyle boxStyle = EditorStyles.helpBox;
                            GUILayout.BeginVertical(boxStyle);
                            is_Expand = EditorGUILayout.BeginFoldoutHeaderGroup(is_Expand, "Round List", menuAction: ShowHeaderContextMenu);
                            if (is_Expand)
                            {
                                for (int i = 0; i < roundList.Count; ++i)
                                {
                                    roundList[i] = EditorGUILayout.TextArea(roundList[i]);
                                }
                                if (GUILayout.Button("Add Round"))
                                {
                                    roundList.Add("");
                                }
                            }
                            rootNode.round = new List<string>(roundList);
                            EditorGUILayout.EndFoldoutHeaderGroup();
                            GUILayout.EndVertical();
                            GUILayout.Space(5);

                            // ?????뺢껸??????곸?? ??嶺뚮∥梨띄뙴濡년뵾????繹먮굞?????뺢껸?????怨룻뱺?????????怨뺤퓡 ???β뼯爰귨㎘??嚥▲굧?????????뉖?
                            rootNode.is_nextChapter = EditorGUILayout.Toggle("is_nextChapter", rootNode.is_nextChapter);
                            if (rootNode.is_nextChapter)
                            {
                                rootNode.nextChapterIndex = EditorGUILayout.TextField("nextChapterIndex", rootNode.nextChapterIndex);     // ???繹먮굞?????Β????嶺뚮슣??땻?????類ｌ┯??
                            }
                        }
                        break;
                    case ChatNode:
                        {
                            GUILayout.Space(15);
                            ChatNode chatNode = node.node as ChatNode;
                            if (chatNode.child.Count != 0) is_ChildExist = true;        // ???嶺?????β뼯援η뙴????ㅻ깹?????繹먮겧嫄х솾?

                            enumValue = (int)chatNode.state;
                            enumValue = GUILayout.Toolbar(enumValue, System.Enum.GetNames(typeof(EChatState)));     // ?꿔꺂????????????
                            chatNode.state = (EChatState)enumValue;
                            GUILayout.Space(5);

                            GUILayout.Label("Chat");
                            chatNode.text = EditorGUILayout.TextArea(chatNode.text, EditorStyles.textArea);     // ????紐꾨쫯??
                            GUILayout.Space(10);

                            enumValue2 = (int)chatNode.face;
                            enumValue2 = GUILayout.Toolbar(enumValue2, System.Enum.GetNames(typeof(EFace)));        // ?꿔꺂????????????嶺뚮????
                            chatNode.face = (EFace)enumValue2;
                            GUILayout.Space(10);

                            // ???類ｌ┯?????嚥?????ㅻ쿋??
                            if (!is_LoadList)
                            {
                                chatEventList = new List<EChatEvent>(chatNode.textEvent);
                                is_LoadList = true;
                            }
                            GUIStyle boxStyle = EditorStyles.helpBox;
                            GUILayout.BeginVertical(boxStyle);
                            is_Expand = EditorGUILayout.BeginFoldoutHeaderGroup(is_Expand, "Chat Event List", menuAction: ShowHeaderContextMenu);
                            if (is_Expand)
                            {
                                for (int i = 0; i < chatEventList.Count; ++i)
                                {
                                    chatEventList[i] = (EChatEvent)EditorGUILayout.EnumPopup(chatEventList[i]);
                                }
                                if (GUILayout.Button("Add Chat Event"))
                                {
                                    chatEventList.Add(EChatEvent.Vibration);
                                }
                            }
                            chatNode.textEvent = new List<EChatEvent>(chatEventList);
                            EditorGUILayout.EndFoldoutHeaderGroup();
                            GUILayout.EndVertical();
                        }
                        break;
                    case AskNode:
                        {
                            GUILayout.Space(15);
                            AskNode askNode = node.node as AskNode;
                            if (askNode.child != null) is_ChildExist = true;
                            GUILayout.Space(10);

                            GUILayout.Label("Ask");
                            askNode.ask = EditorGUILayout.TextArea(askNode.ask, EditorStyles.textArea);     // ?꿔꺂???熬곻퐢利?
                            EditorGUI.BeginDisabledGroup(true);
                            EditorGUILayout.Toggle("is_UseThie", askNode.is_UseThis);
                        }
                        break;
                    case LockAskNode:
                        {
                            GUILayout.Space(15);
                            LockAskNode lockAskNode = node.node as LockAskNode;
                            if (lockAskNode.child != null) is_ChildExist = true;

                            GUILayout.Label("Ask");
                            lockAskNode.ask = EditorGUILayout.TextArea(lockAskNode.ask, EditorStyles.textArea);     // ?꿔꺂???熬곻퐢利?
                            GUILayout.Space(10);

                            // ?꿔꺂?ｉ뜮?뚮쑏癰귥쥓夷?
                            if (!is_LoadList)
                            {
                                evidenceList = new List<string>(lockAskNode.evidence);
                                is_LoadList = true;
                            }
                            GUIStyle boxStyle = EditorStyles.helpBox;
                            GUILayout.BeginVertical(boxStyle);
                            is_Expand = EditorGUILayout.BeginFoldoutHeaderGroup(is_Expand, "Evidence List", menuAction: ShowHeaderContextMenu);
                            if (is_Expand)
                            {
                                for (int i = 0; i < evidenceList.Count; ++i)
                                {
                                    evidenceList[i] = GUILayout.TextField(evidenceList[i]);
                                }
                                if (GUILayout.Button("Add Evidence"))
                                {
                                    evidenceList.Add(null);
                                }
                            }
                            lockAskNode.evidence = new List<string>(evidenceList);
                            EditorGUILayout.EndFoldoutHeaderGroup();
                            GUILayout.EndVertical();
                        }
                        break;
                }

                GUILayout.Space(5);
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.Toggle("child", is_ChildExist);
                EditorGUI.EndDisabledGroup();
            };
*/
            Add(container);     // UI ????????????鶯ㅺ동????묐쵈?? ???繹먮냱議????⑤슢????브퀣堉싪눧?????ㅔ??
        }

        private void ShowHeaderContextMenu(Rect position)       // ?꿔꺂??????????ㅿ폑獄????????꿔꺂?????????κ땁???
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Clear"), false, () =>
            {
                Debug.Log("dl");
                chatEventList.Clear();
                evidenceList.Clear();
                roundList.Clear();
            });
            menu.DropDown(position);
        }
    }
}
