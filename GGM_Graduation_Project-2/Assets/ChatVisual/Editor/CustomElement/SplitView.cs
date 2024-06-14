using UnityEngine.UIElements;

namespace ChatVisual
{
    public class SplitView : TwoPaneSplitView       // 자식을 두 개의 수평 또는 수직 창에 배열하는 컨테이너
    {
        public new class UxmlFactory : UxmlFactory<SplitView, UxmlTraits> { }
        public new class UxmlTraits : TwoPaneSplitView.UxmlTraits { }
    }
}
