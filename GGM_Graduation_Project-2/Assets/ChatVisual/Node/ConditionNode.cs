using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChatVisual
{
    // Situations in which a ConditionNode will be disarmed
    // 1. if all question nodes associated with you are unlocked
    // 2. when a specific file is opened

    // Classes are used to categorize.

    public class ConditionNode : Node
    {
        public List<Node> parentList = new List<Node>();
        public Node child;

        public ICheck checkClass;
        public bool is_AllQuestion;
        public bool is_SpecificFile;
        
        public void InitConditionNode()
        {
            if (is_AllQuestion)
            {
                checkClass = new AllQuestion();
            }

            if (is_SpecificFile)
            {
                checkClass = new SpecificFile();
            }
        }
    }

    public interface ICheck
    {
        public bool Check();
    }

    public class AllQuestion : ICheck
    {
        public List<AskNode> asks = new List<AskNode>();

        public void Init(ConditionNode myNode)
        {
            Debug.Log(myNode.parentList.Count);
            for (int i = 0; i < myNode.parentList.Count; i++)
            {
                Node nowNode = myNode.parentList[i];
                while(!(nowNode is AskNode))
                {
                    if (nowNode is ChatNode chatNode)
                    {
                        nowNode = chatNode.parent;
                    }
                }
                asks.Add(nowNode as AskNode);
            }
        }

        public bool Check()
        {
            for (int i = 0; i < asks.Count; i++)
            {
                if (asks[i].is_UseThis == false)
                {
                    return false;
                }
            }
            return true;
        }
    }

    public class SpecificFile : ICheck
    {
        public string fileName;

        public bool Check()
        {
            // 액션 혹은 딕셔너리로 만들기
            /*if (dictionary[fileName] == true)
            {
                return true;
            }*/
            return false;
        }

    }

}
