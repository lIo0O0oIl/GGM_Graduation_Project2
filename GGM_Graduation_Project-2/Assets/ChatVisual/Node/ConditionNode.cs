using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChatVisual
{
    // Situations in which a ConditionNode will be disarmed
    // 1. if all question nodes associated with you are unlocked
    // 2. when a specific file is opened
    // 2-2. May have a question node immediately following.

    // Classes are used to categorize.

    [Serializable]
    public class ConditionNode : Node
    {
        public List<Node> parentList = new List<Node>();
        public List<Node> childList = new List<Node>();

        public ICheck checkClass;
        public bool is_AllQuestion;
        public List<AskNode> asks = new List<AskNode>();
        public bool is_SpecificFile;
        public string fileName;
        public bool is_LockQuestion;
        
        public void InitConditionNode()
        {
            if (is_AllQuestion)
            {
                checkClass = new AllQuestion();
                if (checkClass is AllQuestion allQuestion)
                {
                    allQuestion.conditionNode = this;
                }
            }

            if (is_SpecificFile)
            {
                checkClass = new SpecificFile();
                if (checkClass is SpecificFile specificFile)
                {
                    specificFile.conditionNode = this;
                }
            }
        }

        public bool Checkk()
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

    public interface ICheck
    {
        public bool Check();
    }

    [Serializable]
    public class AllQuestion : ICheck
    {
        public ConditionNode conditionNode;

        public void Init(ConditionNode myNode)
        {
            conditionNode.asks.Clear();
            //Debug.Log("tl");
            for (int i = 0; i < myNode.parentList.Count; i++)
            {
                Node nowNode = myNode.parentList[i];
                for (int j = 0; j < 50; j++)
                 {
                     if (nowNode is ChatNode chatNode)
                     {
                         nowNode = chatNode.parent;
                     }
                     if (nowNode is ConditionNode condition)
                     {   
                        break;
                     }
                     if (nowNode is AskNode askNode)
                     {
                        //Debug.Log(i + "_");
                        conditionNode.asks.Add(askNode);
                         break;
                     }
                 }
            }
            
        }

        public bool Check()
        {
            for (int i = 0; i < conditionNode.asks.Count; i++)
            {
                if (conditionNode.asks[i].is_UseThis == false)
                {
                    return false;
                }
            }
            return true;
        }
    }

     [Serializable]
    public class SpecificFile : ICheck
    {
        public ConditionNode conditionNode;

        public bool Check()
        {
            // Use Action or Dictionary
            /*if (dictionary[fileName] == true)
            {
                return true;
            }*/
            return false;
        }

    }

}
