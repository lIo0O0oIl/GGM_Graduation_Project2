using System;
using System.Collections;
using System.Runtime.CompilerServices;
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
        private float _lastClickTime;
        private float _doubleClickThreshold = 0.5f;
        private float _holdClickThreshold = 0.5f;
        public bool _doubleClickInitiated = false;

        private bool is_MouseDown = false;
        private MouseDownEvent _evt;

        private Charging _charging;

        private Charging charging
        {
            get
            {
                if (_charging == null)
                {
                    _charging = GameObject.Find("Game").GetComponent<Charging>();
                }
                return _charging;
            }
        }

        public Dragger(Action<MouseUpEvent, VisualElement, VisualElement> DropCallback, Action ClickCallback)
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
                charging.isFile = true;

                if (Time.time - _lastClickTime <= _doubleClickThreshold)
                {
                    _isDrag = false;
                    _doubleClickInitiated = false;
                }
                else
                {
                    GameManager.Instance.StartCoroutine(CheckMouseHold());
                    _evt = evt;
                    is_MouseDown = true;
                    _doubleClickInitiated = true;
                }

                _lastClickTime = Time.time;
                _startPos = evt.localMousePosition;
            }
        }

        private void StartDrag(MouseDownEvent evt)
        {
            //_beforeSlot = target.parent;
            _beforeSlot = GameManager.Instance.fileSystem.ui_fileGround;
            var container = target.parent.parent;

            target.RemoveFromHierarchy();
            container.Add(target);

            _isDrag = true;
            target.CaptureMouse();

            Vector2 offset = evt.mousePosition - container.worldBound.position - _startPos;

            target.style.position = Position.Absolute;

            Vector2 diff = evt.localMousePosition;

            var x = target.layout.x;
            var y = target.layout.y;

            target.style.left = x + diff.x;
            target.style.top = y + diff.y;
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
            charging.isFile = false;

            is_MouseDown = false;
            GameManager.Instance.StopCoroutine(CheckMouseHold());
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
            else if (_doubleClickInitiated == false)
            {
                //target.schedule.Execute(() =>
                //{
                    if (_doubleClickInitiated == false)
                    {
                        if (GameManager.Instance.fileSystem.isPathClick == false)
                        {
                            _clickCallback?.Invoke();
                        }
                        else
                            GameManager.Instance.fileSystem.isPathClick = false;
                    }
                //}).StartingIn((int)(_doubleClickThreshold * 1000));
            }

            _doubleClickInitiated = false;
        }

        IEnumerator CheckMouseHold()
        {
            yield return new WaitForSeconds(_holdClickThreshold);
            if (is_MouseDown)
            {
                StartDrag(_evt);
            }
        }
    }
}
