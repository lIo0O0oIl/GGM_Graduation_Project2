using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChatVisual
{
    /*
    ★ NextChat 로직
    쳇팅 노드일 때 : 대화 출력, 감정 변화, 대사 이벤트(카메라, 진동, 파일로드) 처리
    질문 노드일 때 : 질문 출력, 대사 이벤트(아마 다음 챗팅 넘어가기만 있을 예정)
    조건 노드일 때 : ???
    */

    public class ChatProgress : MonoBehaviour
    {
        public ChatContainer chatContainer;

        public float changeHumanTime = 1f;       // A time when humans change

        private List<Node> nowNodes = new List<Node>();
        private string nowHumanName;        // Name of the human you're talking to
        private int currentIndex = 0;

        public void InitChatProgress(string _nowHumanName)
        {
            nowHumanName = _nowHumanName;
            nowNodes = chatContainer.HumanAndChatDictionary[chatContainer.nowHumanName];
        }

        // Change human
        public void ChangeHuman(string _changeHumanName)
        {
            nowNodes = chatContainer.HumanAndChatDictionary[_changeHumanName];
            if (nowNodes[0] is RootNode rootNode)
            {
                currentIndex = rootNode.nowIndex;       // Change index
            }
        }

        // Proceed with chat
        public void NextChat()
        {
            var children = chatContainer.GetChild(nowNodes[currentIndex]);

            if (children.Count == 1 && children[0] is ChatNode)            // When a child is a ChatNode
            {
                // 대사 출력

                // 감정 변화

                // 대사 이벤트 처리 (카메라, 진동, 파일로드(아마 다 함수로 뺄 예정))

                currentIndex++;
            }
            else        // When child is not a ChatNode
            {
                // // 질문이나 조건이 담겨있을 때 담겨있을 때
                for (int i = 0; i < children.Count; i++)
                {
                    if (children[i] is AskNode askNode)
                    {
                        // 질문 처리
                        currentIndex++;
                    }
                    else if (children[i] is ConditionNode conditionNode)
                    {
                        if (conditionNode.is_UseThis)
                        {
                            // 조건노드일 때 조건이 완료된거라면 다음 노드 해줌.
                            if (conditionNode.checkClass.Check())
                            {
                                currentIndex++;
                                NextChat();
                            }
                        }
                    }
                }
            }
        }
    }
}
