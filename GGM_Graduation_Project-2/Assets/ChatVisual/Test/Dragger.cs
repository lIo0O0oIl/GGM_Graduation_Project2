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
////////            Debug.Log("?앹꽦??);
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
////////            Debug.Log("?꾨Т嫄곕굹");
////////            if (CanStartManipulation(evt))
////////            {
////////                var x = target.layout.x;
////////                var y = target.layout.y;
////////                _beforeSlot = target.parent;
////////                var container = target.parent.parent; //諛깃렇?쇱슫??

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

////////            //?대깽?? ?쒕옒洹명븯怨좎엳????? ?댁쟾 遺紐?
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
//////        private float _clickThreshold = 0.2f; // ?대┃?쇰줈 ?몄떇??理쒕? ?쒓컙 (珥?
//////        private float _moveThreshold = 5.0f; // ?대┃?쇰줈 ?몄떇??理쒕? ?대룞 嫄곕━ (?쎌?)

//////        public Dragger(Action<MouseUpEvent, VisualElement, VisualElement> DropCallback)
//////        {
//////            Debug.Log("?앹꽦??);
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
//////                // 踰꾪듉 ?대┃?쇰줈 ?몄떇
//////                //_clickCallback?.Invoke();

//////                Debug.Log("?쇱씠 踰꾪듉 ?뚮━?⑤떎11");
//////            }
//////            else
//////            {
//////                // ?쒕옒洹몃줈 ?몄떇
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
////        private float _clickThreshold = 0.2f; // ?대┃?쇰줈 ?몄떇??理쒕? ?쒓컙 (珥?
////        private float _doubleClickThreshold = 0.3f; // ?붾툝 ?대┃?쇰줈 ?몄떇??理쒕? ?쒓컙 媛꾧꺽 (珥?
////        private float _moveThreshold = 5.0f; // ?대┃?쇰줈 ?몄떇??理쒕? ?대룞 嫄곕━ (?쎌?)

////        public Dragger(Action<MouseUpEvent, VisualElement, VisualElement> DropCallback)
////        {
////            Debug.Log("?앹꽦??);
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
////                    // ?붾툝 ?대┃?쇰줈 ?쒕옒洹??쒖옉
////                    StartDrag(evt);
////                }
////                else
////                {
////                    // ?⑥씪 ?대┃?쇰줈 踰꾪듉 ?대┃ ?몄떇
////                    //Debug.Log("?쒕컻");
////                    _isDrag = false;
////                }

////                _lastClickTime = Time.time;
////                _startPos = evt.localMousePosition;
////            }
////        }

////        private void StartDrag(MouseDownEvent evt)
////        {
////            Debug.Log("?닿쾶 ???쒖옉?섎냽");
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

////                // ?쒕옒洹????쒕∼ ?꾨즺
////                target.style.position = Position.Relative;
////                target.style.left = 0;
////                target.style.top = 0;
////                _dropCallback?.Invoke(evt, target, _beforeSlot);
////                _isDrag = false;
////            }
////            else
////            {
////                // ?⑥씪 ?대┃?쇰줈 踰꾪듉 ?대┃ ?몄떇
////                Debug.Log("踰꾪듉 ?묐떞??!!!");
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
//        private float _clickThreshold = 0.2f; // ?대┃?쇰줈 ?몄떇??理쒕? ?쒓컙 (珥?
//        private float _doubleClickThreshold = 0.3f; // ?붾툝 ?대┃?쇰줈 ?몄떇??理쒕? ?쒓컙 媛꾧꺽 (珥?
//        private float _moveThreshold = 5.0f; // ?대┃?쇰줈 ?몄떇??理쒕? ?대룞 嫄곕━ (?쎌?)
//        private bool _doubleClickInitiated = false;

//        public Dragger(Action<MouseUpEvent, VisualElement, VisualElement> DropCallback)
//        {
//            Debug.Log("?앹꽦??);
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
//                    // ?붾툝 ?대┃?쇰줈 ?쒕옒洹??쒖옉
//                    _doubleClickInitiated = true;
//                    StartDrag(evt);
//                }
//                else
//                {
//                    // ?⑥씪 ?대┃?쇰줈 踰꾪듉 ?대┃ ?몄떇
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

//                // ?쒕옒洹????쒕∼ ?꾨즺
//                target.style.position = Position.Relative;
//                target.style.left = 0;
//                target.style.top = 0;
//                _dropCallback?.Invoke(evt, target, _beforeSlot);
//                _isDrag = false;
//            }
//            else if (!_doubleClickInitiated)
//            {
//                // ?⑥씪 ?대┃?쇰줈 踰꾪듉 ?대┃ ?몄떇
//                Debug.Log("?щ뇬??!");
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
        private float _clickThreshold = 0.2f; // ?대┃?쇰줈 ?몄떇??理쒕? ?쒓컙 (珥?
        private float _doubleClickThreshold = 0.3f; // ?붾툝 ?대┃?쇰줈 ?몄떇??理쒕? ?쒓컙 媛꾧꺽 (珥?
        private float _moveThreshold = 5.0f; // ?대┃?쇰줈 ?몄떇??理쒕? ?대룞 嫄곕━ (?쎌?)
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
                    // ?붾툝 ?대┃?쇰줈 ?쒕옒洹??쒖옉
                    _doubleClickInitiated = true;
                    StartDrag(evt);
                }
                else
                {
                    // ?⑥씪 ?대┃?쇰줈 踰꾪듉 ?대┃ ?몄떇
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
                // ?대┃ 吏??泥섎━
                target.schedule.Execute(() =>
                {
                    if (!_doubleClickInitiated)
                    {
                        _clickCallback?.Invoke();
                    }
                }).StartingIn((int)(_doubleClickThreshold * 1000)); // ?붾툝 ?대┃ ?꾧퀎媛??쒓컙 ?숈븞 吏??
            }

            // ?붾툝 ?대┃ ?곹깭 珥덇린??
            _doubleClickInitiated = false;
        }
    }
}
