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
            Button button = invenElem.Q<Button>();
            baseArea.Add(invenElem);
            button.AddManipulator(new Dragger((evt, target, beforeSlot) =>
            {
                //Debug.Log(beforeSlot.name);
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
            },
            () => { }
            ));

        }

        private VisualElement FindMoveArea(Vector2 position)
        {
            //筌뤴뫀諭??????筌≪뼚釉??域밸챷夷?癒?퐣 worldBound ??position????곷릭??????뱀뱽 筌≪뼚釉??삠늺 
            if (moveArea.worldBound.Contains(position)) //????RECT??됰퓠 ?????륁뵠 ??덈뮉筌왖 野꺜??鍮?
            {
                return moveArea;
            }
            return null;
        }
    }
}
