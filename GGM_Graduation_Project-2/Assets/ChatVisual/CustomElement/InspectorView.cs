using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace ChatVisual
{
    public class InspectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits { }

        private SerializedObject inspectorObject;
        private SerializedProperty inspectorProperty;

        public InspectorView()
        {

        }

        public void UpdateSelection(NodeView node)      // 누른 노드가 다른거면
        {
            Clear();        // 엘리먼트 모두 없애고

            var container = new IMGUIContainer();
            /*container.onGUIHandler = () =>
            {
                inspectorObject = new SerializedObject(node.node.nodeContainer.GetComponent<Node.NodeContainer>());
                inspectorProperty = inspectorObject.FindProperty("no");
                Debug.Log($"{inspectorObject}, {inspectorProperty}");

                if (inspectorObject != null && inspectorObject.targetObject != null)
                {
                    inspectorObject.Update();
                    EditorGUILayout.PropertyField(inspectorProperty);
                    inspectorObject.ApplyModifiedProperties();
                }
            };*/

            Add(container);     // UI 컨테이너에 넣어줌, 실제로 보이게 해줌.
        }
    }
}
