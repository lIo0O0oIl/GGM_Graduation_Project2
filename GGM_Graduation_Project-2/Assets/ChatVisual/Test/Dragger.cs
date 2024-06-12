////////using System;
////////using System.Collections;
////////using System.Collections.Generic;
////////using UnityEngine;
////////using UnityEngine.UIElements;

////////namespace ChatVisual
////////{
////////    public class Dragger : MouseManipulator
////////    {
////////        private Action<MouseUpEvent, VisualElement, VisualElement> _dropCallback;

////////        private bool _isDrag = false;
////////        private Vector2 _startPos;
////////        private VisualElement _beforeSlot;
////////        public Dragger(Action<MouseUpEvent, VisualElement, VisualElement> DropCallback)
////////        {
////////            Debug.Log("생성됨");
////////            activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
////////            _dropCallback = DropCallback;
////////        }

////////        protected override void RegisterCallbacksOnTarget()
////////        {
////////            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
////////            target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
////////            target.RegisterCallback<MouseUpEvent>(OnMouseUp);
////////        }

////////        protected override void UnregisterCallbacksFromTarget()
////////        {
////////            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
////////            target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
////////            target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
////////        }

////////        protected void OnMouseDown(MouseDownEvent evt)
////////        {
////////            Debug.Log("아무거나");
////////            if (CanStartManipulation(evt))
////////            {
////////                var x = target.layout.x;
////////                var y = target.layout.y;
////////                _beforeSlot = target.parent;
////////                var container = target.parent.parent; //백그라운드

////////                target.RemoveFromHierarchy();
////////                container.Add(target);

////////                _isDrag = true;
////////                target.CaptureMouse();
////////                _startPos = evt.localMousePosition;

////////                Vector2 offset = evt.mousePosition - container.worldBound.position - _startPos;

////////                target.style.position = Position.Absolute;
////////                target.style.left = offset.x;
////////                target.style.top = offset.y;
////////            }
////////        }

////////        protected void OnMouseMove(MouseMoveEvent evt)
////////        {
////////            if (!_isDrag || !CanStartManipulation(evt) || !target.HasMouseCapture())
////////                return;

////////            Vector2 diff = evt.localMousePosition - _startPos;
////////            var x = target.layout.x;
////////            var y = target.layout.y;

////////            target.style.left = x + diff.x;
////////            target.style.top = y + diff.y;
////////        }

////////        protected void OnMouseUp(MouseUpEvent evt)
////////        {
////////            if (!_isDrag || !target.HasMouseCapture())
////////                return;

////////            _isDrag = false;
////////            target.ReleaseMouse();

////////            target.style.position = Position.Relative;
////////            target.style.left = 0;
////////            target.style.top = 0;

////////            //이벤트, 드래그하고있는 녀석, 이전 부모
////////            _dropCallback?.Invoke(evt, target, _beforeSlot);
////////        }
////////    }
////////}

//////using System;
//////using UnityEngine;
//////using UnityEngine.UIElements;

//////namespace ChatVisual
//////{
//////    public class Dragger : MouseManipulator
//////    {
//////        private Action<MouseUpEvent, VisualElement, VisualElement> _dropCallback;
//////        //private Action _clickCallback;

//////        private bool _isDrag = false;
//////        private Vector2 _startPos;
//////        private VisualElement _beforeSlot;
//////        private float _clickStartTime;
//////        private float _clickThreshold = 0.2f; // 클릭으로 인식할 최대 시간 (초)
//////        private float _moveThreshold = 5.0f; // 클릭으로 인식할 최대 이동 거리 (픽셀)

//////        public Dragger(Action<MouseUpEvent, VisualElement, VisualElement> DropCallback)
//////        {
//////            Debug.Log("생성됨");
//////            activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
//////            _dropCallback = DropCallback;
//////            //_clickCallback = ClickCallback;
//////        }

//////        protected override void RegisterCallbacksOnTarget()
//////        {
//////            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
//////            target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
//////            target.RegisterCallback<MouseUpEvent>(OnMouseUp);
//////        }

//////        protected override void UnregisterCallbacksFromTarget()
//////        {
//////            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
//////            target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
//////            target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
//////        }

//////        protected void OnMouseDown(MouseDownEvent evt)
//////        {
//////            if (CanStartManipulation(evt))
//////            {
//////                _clickStartTime = Time.time;
//////                _startPos = evt.localMousePosition;
//////                _beforeSlot = target.parent;
//////                var container = target.parent.parent;

//////                target.RemoveFromHierarchy();
//////                container.Add(target);

//////                _isDrag = true;
//////                target.CaptureMouse();

//////                Vector2 offset = evt.mousePosition - container.worldBound.position - _startPos;

//////                target.style.position = Position.Absolute;
//////                target.style.left = offset.x;
//////                target.style.top = offset.y;
//////            }
//////        }

//////        protected void OnMouseMove(MouseMoveEvent evt)
//////        {
//////            if (!_isDrag || !CanStartManipulation(evt) || !target.HasMouseCapture())
//////                return;

//////            Vector2 diff = evt.localMousePosition - _startPos;
//////            if (diff.magnitude > _moveThreshold)
//////            {
//////                _isDrag = true;
//////            }

//////            var x = target.layout.x;
//////            var y = target.layout.y;

//////            target.style.left = x + diff.x;
//////            target.style.top = y + diff.y;
//////        }

//////        protected void OnMouseUp(MouseUpEvent evt)
//////        {
//////            if (!_isDrag || !target.HasMouseCapture())
//////                return;

//////            target.ReleaseMouse();

//////            float clickDuration = Time.time - _clickStartTime;
//////            Vector2 diff = evt.localMousePosition - _startPos;

//////            if (clickDuration <= _clickThreshold && diff.magnitude <= _moveThreshold)
//////            {
//////                // 버튼 클릭으로 인식
//////                //_clickCallback?.Invoke();

//////                Debug.Log("야이 버튼 눌리셨다11");
//////            }
//////            else
//////            {
//////                // 드래그로 인식
//////                target.style.position = Position.Relative;
//////                target.style.left = 0;
//////                target.style.top = 0;
//////                _dropCallback?.Invoke(evt, target, _beforeSlot);
//////            }   

//////            _isDrag = false;
//////        }
//////    }
//////}

////using System;
////using UnityEngine;
////using UnityEngine.UIElements;

////namespace ChatVisual
////{
////    public class Dragger : MouseManipulator
////    {
////        private Action<MouseUpEvent, VisualElement, VisualElement> _dropCallback;

////        private bool _isDrag = false;
////        private Vector2 _startPos;
////        private VisualElement _beforeSlot;
////        private float _clickStartTime;
////        private float _lastClickTime;
////        private float _clickThreshold = 0.2f; // 클릭으로 인식할 최대 시간 (초)
////        private float _doubleClickThreshold = 0.3f; // 더블 클릭으로 인식할 최대 시간 간격 (초)
////        private float _moveThreshold = 5.0f; // 클릭으로 인식할 최대 이동 거리 (픽셀)

////        public Dragger(Action<MouseUpEvent, VisualElement, VisualElement> DropCallback)
////        {
////            Debug.Log("생성됨");
////            activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
////            _dropCallback = DropCallback;
////        }

////        protected override void RegisterCallbacksOnTarget()
////        {
////            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
////            target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
////            target.RegisterCallback<MouseUpEvent>(OnMouseUp);
////        }

////        protected override void UnregisterCallbacksFromTarget()
////        {
////            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
////            target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
////            target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
////        }

////        protected void OnMouseDown(MouseDownEvent evt)
////        {
////            if (CanStartManipulation(evt))
////            {
////                _clickStartTime = Time.time;

////                if (Time.time - _lastClickTime <= _doubleClickThreshold)
////                {
////                    // 더블 클릭으로 드래그 시작
////                    StartDrag(evt);
////                }
////                else
////                {
////                    // 단일 클릭으로 버튼 클릭 인식
////                    //Debug.Log("시발");
////                    _isDrag = false;
////                }

////                _lastClickTime = Time.time;
////                _startPos = evt.localMousePosition;
////            }
////        }

////        private void StartDrag(MouseDownEvent evt)
////        {
////            Debug.Log("이게 왜 시작되농");
////            _beforeSlot = target.parent;
////            var container = target.parent.parent;

////            target.RemoveFromHierarchy();
////            container.Add(target);

////            _isDrag = true;
////            target.CaptureMouse();

////            Vector2 offset = evt.mousePosition - container.worldBound.position - _startPos;

////            target.style.position = Position.Absolute;
////            target.style.left = offset.x;
////            target.style.top = offset.y;
////        }

////        protected void OnMouseMove(MouseMoveEvent evt)
////        {
////            if (!_isDrag || !CanStartManipulation(evt) || !target.HasMouseCapture())
////                return;

////            Vector2 diff = evt.localMousePosition - _startPos;

////            var x = target.layout.x;
////            var y = target.layout.y;

////            target.style.left = x + diff.x;
////            target.style.top = y + diff.y;
////        }

////        protected void OnMouseUp(MouseUpEvent evt)
////        {
////            if (_isDrag)
////            {
////                if (!target.HasMouseCapture())
////                    return;

////                target.ReleaseMouse();

////                // 드래그 앤 드롭 완료
////                target.style.position = Position.Relative;
////                target.style.left = 0;
////                target.style.top = 0;
////                _dropCallback?.Invoke(evt, target, _beforeSlot);
////                _isDrag = false;
////            }
////            else
////            {
////                // 단일 클릭으로 버튼 클릭 인식
////                Debug.Log("버튼 응담함!!!!");
////            }
////        }
////    }
////}

//using PlasticPipe.PlasticProtocol.Messages;
//using System;
//using UnityEngine;
//using UnityEngine.UIElements;

//namespace ChatVisual
//{
//    public class Dragger : MouseManipulator
//    {
//        private Action<MouseUpEvent, VisualElement, VisualElement> _dropCallback;

//        private bool _isDrag = false;
//        private Vector2 _startPos;
//        private VisualElement _beforeSlot;
//        private float _clickStartTime;
//        private float _lastClickTime;
//        private float _clickThreshold = 0.2f; // 클릭으로 인식할 최대 시간 (초)
//        private float _doubleClickThreshold = 0.3f; // 더블 클릭으로 인식할 최대 시간 간격 (초)
//        private float _moveThreshold = 5.0f; // 클릭으로 인식할 최대 이동 거리 (픽셀)
//        private bool _doubleClickInitiated = false;

//        public Dragger(Action<MouseUpEvent, VisualElement, VisualElement> DropCallback)
//        {
//            Debug.Log("생성됨");
//            activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
//            _dropCallback = DropCallback;
//        }

//        protected override void RegisterCallbacksOnTarget()
//        {
//            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
//            target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
//            target.RegisterCallback<MouseUpEvent>(OnMouseUp);
//        }

//        protected override void UnregisterCallbacksFromTarget()
//        {
//            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
//            target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
//            target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
//        }

//        protected void OnMouseDown(MouseDownEvent evt)
//        {
//            if (CanStartManipulation(evt))
//            {
//                _clickStartTime = Time.time;

//                if (Time.time - _lastClickTime <= _doubleClickThreshold)
//                {
//                    // 더블 클릭으로 드래그 시작
//                    _doubleClickInitiated = true;
//                    StartDrag(evt);
//                }
//                else
//                {
//                    // 단일 클릭으로 버튼 클릭 인식
//                    _isDrag = false;
//                    _doubleClickInitiated = false;
//                }

//                _lastClickTime = Time.time;
//                _startPos = evt.localMousePosition;
//            }
//        }

//        private void StartDrag(MouseDownEvent evt)
//        {
//            _beforeSlot = target.parent;
//            var container = target.parent.parent;

//            target.RemoveFromHierarchy();
//            container.Add(target);

//            _isDrag = true;
//            target.CaptureMouse();

//            Vector2 offset = evt.mousePosition - container.worldBound.position - _startPos;

//            target.style.position = Position.Absolute;
//            target.style.left = offset.x;
//            target.style.top = offset.y;
//        }

//        protected void OnMouseMove(MouseMoveEvent evt)
//        {
//            if (!_isDrag || !CanStartManipulation(evt) || !target.HasMouseCapture())
//                return;

//            Vector2 diff = evt.localMousePosition - _startPos;

//            var x = target.layout.x;
//            var y = target.layout.y;

//            target.style.left = x + diff.x;
//            target.style.top = y + diff.y;
//        }

//        protected void OnMouseUp(MouseUpEvent evt)
//        {


//            if (_isDrag)
//            {
//                if (!target.HasMouseCapture())
//                    return;

//                target.ReleaseMouse();

//                // 드래그 앤 드롭 완료
//                target.style.position = Position.Relative;
//                target.style.left = 0;
//                target.style.top = 0;
//                _dropCallback?.Invoke(evt, target, _beforeSlot);
//                _isDrag = false;
//            }
//            else if (!_doubleClickInitiated)
//            {
//                // 단일 클릭으로 버튼 클릭 인식
//                Debug.Log("심봤다!!");
//            }
//        }
//    }
//}

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace ChatVisual
{
    public class Dragger : MouseManipulator
    {
        private Action<MouseUpEvent, VisualElement, VisualElement> _dropCallback;
        private Action _clickCallback;

        private bool _isDrag = false;
        private Vector2 _startPos;
        private VisualElement _beforeSlot;
        private float _clickStartTime;
        private float _lastClickTime;
        private float _clickThreshold = 0.2f; // 클릭으로 인식할 최대 시간 (초)
        private float _doubleClickThreshold = 0.3f; // 더블 클릭으로 인식할 최대 시간 간격 (초)
        private float _moveThreshold = 5.0f; // 클릭으로 인식할 최대 이동 거리 (픽셀)
        private bool _doubleClickInitiated = false;

        public Dragger(Action<MouseUpEvent, VisualElement, VisualElement> DropCallback, Action ClickCallback    )
        {
            activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
            _dropCallback = DropCallback;
            _clickCallback = ClickCallback;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
            target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
            target.RegisterCallback<MouseUpEvent>(OnMouseUp);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
            target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
            target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
        }

        protected void OnMouseDown(MouseDownEvent evt)
        {
            if (CanStartManipulation(evt))
            {
                _clickStartTime = Time.time;

                if (Time.time - _lastClickTime <= _doubleClickThreshold)
                {
                    // 더블 클릭으로 드래그 시작
                    _doubleClickInitiated = true;
                    StartDrag(evt);
                }
                else
                {
                    // 단일 클릭으로 버튼 클릭 인식
                    _isDrag = false;
                    _doubleClickInitiated = false;
                }

                _lastClickTime = Time.time;
                _startPos = evt.localMousePosition;
            }
        }

        private void StartDrag(MouseDownEvent evt)
        {
            _beforeSlot = target.parent;
            var container = target.parent.parent;

            target.RemoveFromHierarchy();
            container.Add(target);

            _isDrag = true;
            target.CaptureMouse();

            Vector2 offset = evt.mousePosition - container.worldBound.position - _startPos;

            target.style.position = Position.Absolute;
            target.style.left = offset.x;
            target.style.top = offset.y;
        }

        protected void OnMouseMove(MouseMoveEvent evt)
        {
            if (!_isDrag || !CanStartManipulation(evt) || !target.HasMouseCapture())
                return;

            Vector2 diff = evt.localMousePosition - _startPos;

            var x = target.layout.x;
            var y = target.layout.y;

            target.style.left = x + diff.x;
            target.style.top = y + diff.y;
        }

        protected void OnMouseUp(MouseUpEvent evt)
        {
            if (_isDrag)
            {
                if (!target.HasMouseCapture())
                    return;

                target.ReleaseMouse();

                target.style.position = Position.Relative;
                target.style.left = 0;
                target.style.top = 0;
                _dropCallback?.Invoke(evt, target, _beforeSlot);
                _isDrag = false;
            }
            else if (!_doubleClickInitiated)
            {
                // 클릭 지연 처리
                target.schedule.Execute(() =>
                {
                    if (!_doubleClickInitiated)
                    {
                        Debug.Log("버튼 실행됨");
                        _clickCallback?.Invoke();
                    }
                }).StartingIn((int)(_doubleClickThreshold * 1000)); // 더블 클릭 임계값 시간 동안 지연
            }

            // 더블 클릭 상태 초기화
            _doubleClickInitiated = false;
        }
    }
}
