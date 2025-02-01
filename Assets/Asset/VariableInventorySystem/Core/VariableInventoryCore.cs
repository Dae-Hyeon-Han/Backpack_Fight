using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace VariableInventorySystem
{
    public abstract class VariableInventoryCore : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        protected List<IVariableInventoryView> InventoryViews { get; set; } = new List<IVariableInventoryView>();

        protected virtual GameObject CellPrefab { get; set; }
        protected virtual RectTransform EffectCellParent { get; set; }

        protected IVariableInventoryCell stareCell;
        protected IVariableInventoryCell effectCell;

        bool? originEffectCellRotate;
        Vector2 cursorPosition;

        public virtual void Initialize()
        {
            effectCell = Instantiate(CellPrefab, EffectCellParent).GetComponent<IVariableInventoryCell>();          // 아이템 오브젝트 생성
            effectCell.RectTransform.gameObject.SetActive(false);                                                   // ???
            effectCell.SetSelectable(false);                                                                        // 이거 없으면 드래그 & 드롭 막힘
        }

        public virtual void AddInventoryView(IVariableInventoryView variableInventoryView)
        {
            InventoryViews.Add(variableInventoryView);
            variableInventoryView.SetCellCallback(OnCellClick, OnCellOptionClick, OnCellEnter, OnCellExit);
        }

        public virtual void RemoveInventoryView(IVariableInventoryView variableInventoryView)
        {
            InventoryViews.Remove(variableInventoryView);
        }

        // 드래그 시작
        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            foreach (var inventoryViews in InventoryViews)
            {
                inventoryViews.OnPrePick(stareCell);
            }

            var stareData = stareCell?.CellData;
            var isHold = InventoryViews.Any(x => x.OnPick(stareCell));

            if (!isHold)
            {
                return;
            }

            effectCell.RectTransform.gameObject.SetActive(true);
            effectCell.Apply(stareData);
        }

        // 드래그 중
        public virtual void OnDrag(PointerEventData eventData)
        {
            if (effectCell?.CellData == null)
            {
                return;
            }

            foreach (var inventoryViews in InventoryViews)
            {
                inventoryViews.OnDrag(stareCell, effectCell, eventData);
            }

            RectTransformUtility.ScreenPointToLocalPointInRectangle(EffectCellParent, eventData.position, eventData.enterEventCamera, out cursorPosition);

            var (width, height) = GetRotateSize(effectCell.CellData);
            effectCell.RectTransform.localPosition = cursorPosition + new Vector2(
                 -(width - 1) * effectCell.DefaultCellSize.x * 0.5f,
                (height - 1) * effectCell.DefaultCellSize.y * 0.5f);
        }

        (int, int) GetRotateSize(IVariableInventoryCellData cell)
        {
            return (cell.IsRotate ? cell.Height : cell.Width, cell.IsRotate ? cell.Width : cell.Height);
        }

        // 드래그 완료
        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (effectCell.CellData == null)
            {
                return;
            }

            var isRelease = InventoryViews.Any(x => x.OnDrop(stareCell, effectCell));

            if (!isRelease && originEffectCellRotate.HasValue)
            {
                effectCell.CellData.IsRotate = originEffectCellRotate.Value;
                effectCell.Apply(effectCell.CellData);
                originEffectCellRotate = null;
            }

            foreach (var inventoryViews in InventoryViews)
            {
                inventoryViews.OnDroped(isRelease);
            }

            effectCell.RectTransform.gameObject.SetActive(false);
            effectCell.Apply(null);
        }

        public virtual void SwitchRotate()
        {
            if (effectCell.CellData == null)
            {
                return;
            }

            if (!originEffectCellRotate.HasValue)
            {
                originEffectCellRotate = effectCell.CellData.IsRotate;
            }

            effectCell.CellData.IsRotate = !effectCell.CellData.IsRotate;
            effectCell.Apply(effectCell.CellData);

            var (width, height) = GetRotateSize(effectCell.CellData);
            effectCell.RectTransform.localPosition = cursorPosition + new Vector2(
                 -(width - 1) * effectCell.DefaultCellSize.x * 0.5f,
                (height - 1) * effectCell.DefaultCellSize.y * 0.5f);

            foreach (var inventoryViews in InventoryViews)
            {
                inventoryViews.OnSwitchRotate(stareCell, effectCell);
            }
        }

        protected virtual void OnCellClick(IVariableInventoryCell cell)
        {
        }

        protected virtual void OnCellOptionClick(IVariableInventoryCell cell)
        {
        }

        protected virtual void OnCellEnter(IVariableInventoryCell cell)
        {
            stareCell = cell;

            foreach (var inventoryView in InventoryViews)
            {
                inventoryView.OnCellEnter(stareCell, effectCell);
            }
        }

        protected virtual void OnCellExit(IVariableInventoryCell cell)
        {
            foreach (var inventoryView in InventoryViews)
            {
                inventoryView.OnCellExit(stareCell);
            }

            stareCell = null;
        }
    }
}
