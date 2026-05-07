using ScriptableData;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Controller
{
    public class TooltipController : MonoBehaviour
    {
        [SerializeField] private UIDocument uiDoc;
        [SerializeField] private Vector3 offset = new Vector3(20f, 50f, 0f);

        private VisualElement root;
        private VisualElement tooltip;
        private Label tooltipNameLabel;
        private Label tooltipDescriptionLabel;

        void Awake()
        {
            root = uiDoc.rootVisualElement;
            tooltip = root.Q(className: "tooltip");
            tooltipNameLabel = root.Q<Label>(className: "tooltip-name");
            tooltipDescriptionLabel = root.Q<Label>(className: "tooltip-description");
        }

        void OnEnable()
        {
            root.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            root.RegisterCallback<PointerLeaveEvent>(OnPointerLeave);
            root.RegisterCallback<PointerDownEvent>(OnPointerDown);
        }

        void OnDisable()
        {
            root.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
            root.UnregisterCallback<PointerLeaveEvent>(OnPointerLeave);
            root.UnregisterCallback<PointerDownEvent>(OnPointerDown);
        }

        private void OnPointerMove(PointerMoveEvent evt)
        {
            if (evt.target is not VisualElement item)
            {
                HideTooltip();
                return;
            }
            var data = item.dataSource as IInventoryData;

            if (data == null)
            {
                HideTooltip();
                return;
            }

            ShowTooltip(data, evt.position + offset);

        }

        private void ShowTooltip(IInventoryData data, Vector3 position)
        {
            tooltipNameLabel.text = data.Data.name;
            tooltipDescriptionLabel.text = data.Data.description;
            tooltip.style.left = position.x;
            tooltip.style.top = position.y;
            tooltip.AddToClassList("tooltip-active");
        }

        private void HideTooltip()
        {
            tooltipNameLabel.text = string.Empty;
            tooltipDescriptionLabel.text = string.Empty;
            tooltip.RemoveFromClassList("tooltip-active");
        }

        private void OnPointerLeave(PointerLeaveEvent evt)
        {
            HideTooltip();
        }

        private void OnPointerDown(PointerDownEvent evt)
        {
            HideTooltip();
        }
    }
}