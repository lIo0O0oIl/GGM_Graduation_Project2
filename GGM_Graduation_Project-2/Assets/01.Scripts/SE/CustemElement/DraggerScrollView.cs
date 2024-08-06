using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DraggerScrollView : MonoBehaviour
{
    private ScrollView scrollView;

    void Start()
    {
        // UXML에서 ScrollView를 가져옵니다.
        scrollView = UIReader_Main.Instance.root.Q<ScrollView>("TestScrollView");

        // 마우스 이벤트를 처리하기 위해 Manipulator를 추가합니다.
        var manipulator = new MouseManipulator(scrollView);
        var zoomManipulator = new ZoomManipulator(scrollView, scrollView.contentContainer);
        scrollView.AddManipulator(manipulator);
        scrollView.AddManipulator(zoomManipulator);
    }

    private class MouseManipulator : PointerManipulator
    {
        private ScrollView scrollView;
        private Vector3 startMousePosition;
        private Vector2 startScrollPosition;
        private bool isDragging = false;

        public MouseManipulator(ScrollView scrollView)
        {
            this.scrollView = scrollView;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerDownEvent>(OnPointerDown);
            target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            target.RegisterCallback<PointerUpEvent>(OnPointerUp);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
            target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
            target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
        }

        private void OnPointerDown(PointerDownEvent evt)
        {
            if (evt.button == (int)MouseButton.LeftMouse)
            {
                isDragging = true;
                startMousePosition = evt.localPosition;
                startScrollPosition = scrollView.scrollOffset;
                scrollView.CapturePointer(evt.pointerId);
                evt.StopImmediatePropagation();
            }
        }

        private void OnPointerMove(PointerMoveEvent evt)
        {
            if (isDragging)
            {
                Vector2 delta = evt.localPosition - startMousePosition;
                scrollView.scrollOffset = startScrollPosition - delta;      // Lerp 적용해주기
                evt.StopImmediatePropagation();
            }
        }

        private void OnPointerUp(PointerUpEvent evt)
        {
            if (isDragging && evt.button == (int)MouseButton.LeftMouse)
            {
                isDragging = false;
                scrollView.ReleasePointer(evt.pointerId);
                evt.StopImmediatePropagation();
            }
        }
    }

    private class ZoomManipulator : PointerManipulator
    {
        private ScrollView scrollView;
        private VisualElement contentContainer;
        private Vector3 startMousePosition;
        private Vector2 startScrollPosition;
        private bool isDragging = false;
        private float zoomScale = 1.0f;
        private const float zoomStep = 0.1f;
        private const float minZoom = 0.5f;
        private const float maxZoom = 3.0f;

        public ZoomManipulator(ScrollView scrollView, VisualElement contentContainer)
        {
            this.scrollView = scrollView;
            this.contentContainer = contentContainer;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerDownEvent>(OnPointerDown);
            target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            target.RegisterCallback<PointerUpEvent>(OnPointerUp);
            target.RegisterCallback<WheelEvent>(OnWheel);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
            target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
            target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
            target.UnregisterCallback<WheelEvent>(OnWheel);
        }

        private void OnPointerDown(PointerDownEvent evt)
        {
            if (evt.button == (int)MouseButton.LeftMouse)
            {
                isDragging = true;
                startMousePosition = evt.localPosition;
                startScrollPosition = scrollView.scrollOffset;
                scrollView.CapturePointer(evt.pointerId);
                evt.StopImmediatePropagation();
            }
        }

        private void OnPointerMove(PointerMoveEvent evt)
        {
            if (isDragging)
            {
                Vector2 delta = evt.localPosition - startMousePosition;
                scrollView.scrollOffset = startScrollPosition - delta;
                evt.StopImmediatePropagation();
            }
        }

        private void OnPointerUp(PointerUpEvent evt)
        {
            if (isDragging && evt.button == (int)MouseButton.LeftMouse)
            {
                isDragging = false;
                scrollView.ReleasePointer(evt.pointerId);
                evt.StopImmediatePropagation();
            }
        }

        private void OnWheel(WheelEvent evt)
        {
            // 줌 인/줌 아웃
            float delta = evt.delta.y > 0 ? -zoomStep : zoomStep;
            zoomScale = Mathf.Clamp(zoomScale + delta, minZoom, maxZoom);

            // 콘텐츠의 스케일을 조정합니다.
            contentContainer.transform.scale = new Vector3(zoomScale, zoomScale, 1.0f);

            // 스크롤 오프셋을 조정합니다.
            scrollView.scrollOffset = new Vector2(
                scrollView.scrollOffset.x * zoomScale,
                scrollView.scrollOffset.y * zoomScale
            );

            evt.StopImmediatePropagation();
        }
    }
}