using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryManipulator : PointerManipulator
{
    private bool isDragging = false;
    private VisualElement target;
    private VisualElement parentSlot;
    private VisualElement mainParent;
    private Vector2 startPosPointer;
    private Vector2 startPosElement;
    private Action<VisualElement, VisualElement> OnDrop;
    private string slotContainerName = "grid";

    public InventoryManipulator(VisualElement _taregt, VisualElement _parentSlot, VisualElement _mainParent, Action<VisualElement, VisualElement> _onDrop)
    {
        target = _taregt;
        parentSlot = _parentSlot;
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
        evt.StopPropagation();
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

        // mainParent.Remove(target);
        // parentSlot.Add(target);
        target.ReleasePointer(evt.pointerId);
        evt.StopPropagation();
    }

    private void OnPointerCaptureOut(PointerCaptureOutEvent evt)
    {
        if (!isDragging)
            return;

        VisualElement closestSlot = FindClosestSlot(true);
        if (closestSlot == null)
            closestSlot = target.parent;
        SnapToSlot(closestSlot);
        isDragging = false;
        OnDrop?.Invoke(target, closestSlot);
    }

    private void SnapToSlot(VisualElement slot)
    {
        target.RemoveFromHierarchy();
        slot.Add(target);

        target.style.position = Position.Relative;
        target.style.left = StyleKeyword.Auto;
        target.style.top = StyleKeyword.Auto;
        target.style.translate = new Translate(0, 0, 0);
        target.style.translate = Vector3.zero;
    }


    private VisualElement FindClosestSlot(bool overlap)
    {
        if (target.panel == null)
            return null;

        var root = target.panel.visualTree;
        var slotsRoots = root.Query(className: slotContainerName).ToList();
        if (slotsRoots == null || slotsRoots.Count == 0)
            return null;

        List<VisualElement> slots = new();
        foreach (var slotRoot in slotsRoots)
        {
            slots.AddRange(slotRoot.Query(className: "slot").ToList());
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
                minDistance = distance;
                closestSlot = slot;
            }
        }

        return closestSlot;
    }
}
