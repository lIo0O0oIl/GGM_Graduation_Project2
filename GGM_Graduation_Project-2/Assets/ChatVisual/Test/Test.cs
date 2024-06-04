using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ChatVisual
{
    public class Test : MonoBehaviour
    {
        private UIDocument uiDocument;

        private VisualElement moveArea;
        private VisualElement baseArea;

        public VisualTreeAsset item;

        private void Awake()
        {
            uiDocument = GetComponent<UIDocument>();
        }

        private void OnEnable()
        {
            var root = uiDocument.rootVisualElement;
            moveArea = root.Q<VisualElement>("MoveArea");
            baseArea = root.Q<VisualElement>("BaseArea");
            
            VisualElement invenElem = item.Instantiate().Q<VisualElement>("Item");
            baseArea.Add(invenElem);
            invenElem.AddManipulator(new Dragger((evt, target, beforeSlot) =>
            {
                Debug.Log(beforeSlot.name);
                var area = FindMoveArea(evt.mousePosition);

                target.RemoveFromHierarchy();
                if (area == null)
                {
                    beforeSlot.Add(target);
                }
                else
                {
                    moveArea.Add(target);
                }
            }));

        }

        private VisualElement FindMoveArea(Vector2 position)
        {
            //모든 슬롯을 찾아서 그중에서 worldBound 에 position이 속하는 녀석을 찾아오면 
            if (moveArea.worldBound.Contains(position)) //해당 RECT안에 포지션이 있는지 검사해
            {
                return moveArea;
            }
            return null;
        }
    }
}
