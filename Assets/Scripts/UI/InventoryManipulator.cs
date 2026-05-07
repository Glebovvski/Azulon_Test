using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Controller
{
    public class InventoryManipulator : PointerManipulator
    {
        private bool isDragging = false;
        private VisualElement target;
        private VisualElement parentSlot;
        private VisualElement mainParent;
        private Vector2 startPosPointer;
        private Vector2 startPosElement;
        private Action<VisualElement, VisualElement, VisualElement> OnDrop;
        

        private VisualElement replaceDataInSlot;

        public InventoryManipulator(VisualElement _taregt, VisualElement _mainParent, Action<VisualElement, VisualElement, VisualElement> _onDrop)
        {
            target = _taregt;
            mainParent = _mainParent;
            OnDrop = _onDrop;
        }
        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerDownEvent>(OnPointerDown);
            target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            target.RegisterCallback<PointerUpEvent>(OnPointerUp);
            target.RegisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOut);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
            target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
            target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
            target.UnregisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOut);

        }

        private void OnPointerDown(PointerDownEvent evt)
        {
            if (evt.button != 0)
                return;

            parentSlot = target.parent;
            target.style.position = Position.Absolute;
            isDragging = true;

            startPosElement = target.worldBound.position;
            parentSlot.Remove(target);
            mainParent.Add(target);

            startPosPointer = evt.position;

            Rect worldRect = target.worldBound;
            Vector2 localMin = mainParent.WorldToLocal(worldRect.min);
            Vector2 localMax = mainParent.WorldToLocal(worldRect.max);

            target.style.width = Mathf.Abs(localMax.x - localMin.x);
            target.style.height = Mathf.Abs(localMax.y - localMin.y);

            target.style.left = localMin.x;
            target.style.top = localMin.y;

            target.style.marginLeft = 0;
            target.style.marginTop = 0;
            target.style.marginRight = 0;
            target.style.marginBottom = 0;

            ResolvePosition(evt.position);
            target.BringToFront();

            target.CapturePointer(evt.pointerId);
        }

        private void ResolvePosition(Vector2 position)
        {
            Vector2 pointerCurrentPos = position;
            Vector2 pointerDelta = pointerCurrentPos - startPosPointer;

            Vector2 currentPosWorld = startPosElement + pointerDelta;
            Vector2 currentPosLocal = mainParent.WorldToLocal(currentPosWorld);

            target.style.left = currentPosLocal.x;
            target.style.top = currentPosLocal.y;
        }

        private void OnPointerMove(PointerMoveEvent evt)
        {
            if (!isDragging || !target.HasPointerCapture(evt.pointerId))
                return;

            VisualElement parent = target.parent;
            if (parent == null)
                return;

            ResolvePosition(evt.position);

            evt.StopPropagation();
        }

        private void OnPointerUp(PointerUpEvent evt)
        {
            if (!isDragging || !target.HasPointerCapture(evt.pointerId))
                return;

            target.ReleasePointer(evt.pointerId);
            evt.StopPropagation();
        }

        private void OnPointerCaptureOut(PointerCaptureOutEvent evt)
        {
            if (!isDragging)
                return;

            VisualElement closestSlot = FindClosestSlot(true);
            if (closestSlot == null)
                closestSlot = parentSlot;
            SnapToSlot(closestSlot);
            if (replaceDataInSlot != null)
            {
                SnapToSlot(parentSlot, replaceDataInSlot);
                replaceDataInSlot = null;
            }
            isDragging = false;

        }

        private void SnapToSlot(VisualElement slot, VisualElement overrideTarget = null)
        {
            VisualElement item = target;
            VisualElement overrideTargetParentSlot = null;
            if (overrideTarget != null)
            {
                item = overrideTarget;
                overrideTargetParentSlot = overrideTarget.parent;
            }
            item.RemoveFromHierarchy();
            slot.Add(item);

            item.style.position = Position.Relative;
            item.style.left = StyleKeyword.Auto;
            item.style.top = StyleKeyword.Auto;
            item.style.translate = new Translate(0, 0, 0);
            item.style.translate = Vector3.zero;
            if (overrideTarget != null)
            {
                OnDrop?.Invoke(overrideTarget, overrideTargetParentSlot, parentSlot);
            }
            else
                OnDrop?.Invoke(target, parentSlot, slot);
        }


        private VisualElement FindClosestSlot(bool overlap, bool canReplace = true)
        {
            if (target.panel == null)
                return null;

            var root = target.panel.visualTree;
            var slotsRoots = root.Query(className: UIDocumentConsts.SlotContainerName).ToList();
            if (slotsRoots == null || slotsRoots.Count == 0)
                return null;

            List<VisualElement> slots = new();
            foreach (var slotRoot in slotsRoots)
            {
                slots.AddRange(slotRoot.Query(className: UIDocumentConsts.SlotClassName).ToList());
            }
            if (slots.Count == 0)
                return null;

            VisualElement closestSlot = null;
            float minDistance = float.MaxValue;
            foreach (var slot in slots)
            {
                if (overlap && !target.worldBound.Overlaps(slot.worldBound))
                    continue;

                float distance = (slot.worldBound.center - target.worldBound.center).sqrMagnitude;

                if (distance < minDistance)
                {
                    if (IsSlotEmpty(slot, out VisualElement itemInSlot))
                    {
                        minDistance = distance;
                        closestSlot = slot;
                    }
                    else
                    {
                        if (!canReplace)
                            continue;

                        replaceDataInSlot = itemInSlot;
                        minDistance = distance;
                        closestSlot = slot;
                    }
                }
            }

            return closestSlot;
        }

        private bool IsSlotEmpty(VisualElement slot, out VisualElement item)
        {
            item = slot.Q(className: UIDocumentConsts.ItemClassName);
            return item == null;
        }
    }
}