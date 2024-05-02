using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditorInternal.VR;

namespace ChatVisual
{
    public class InspectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits { }

        private Editor editor;

        public InspectorView()
        {

        }

        public void UpdateSelection(NodeView node)      // 누른 노드가 다른거면
        {
            Clear();        // 엘리먼트 모두 없애고
            Object.DestroyImmediate(editor);        // 에디터를 지워주고

            //editor = Editor.CreateEditor(node.node);     // 유니티 기본 인스펙터뷰를 만들어준다.

            var container = new IMGUIContainer(() => {
                if (editor.target)      // 내가 지금 편집중인, 선택중인 얘가 있다면
                {
                    editor.OnInspectorGUI();     // 컨테이너로 넣어줌
                }
            });

            Add(container);     // UI 컨테이너에 넣어줌, 실제로 보이게 해줌.
        }
    }
}
