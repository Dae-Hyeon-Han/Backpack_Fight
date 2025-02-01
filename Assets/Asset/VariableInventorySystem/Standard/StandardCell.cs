using UnityEngine;
using UnityEngine.UI;

namespace VariableInventorySystem
{
    public class StandardCell : VariableInventoryCell
    {
        [SerializeField] Vector2 defaultCellSize;
        [SerializeField] Vector2 margineSpace;

        [SerializeField] RectTransform sizeRoot;
        [SerializeField] RectTransform target;
        [SerializeField] Graphic background;
        [SerializeField] RawImage cellImage;
        [SerializeField] Graphic highlight;

        [SerializeField] StandardButton button;

        public override Vector2 DefaultCellSize => defaultCellSize;
        public override Vector2 MargineSpace => margineSpace;
        protected override IVariableInventoryCellActions ButtonActions => button;
        protected virtual StandardAssetLoader Loader { get; set; }

        protected bool isSelectable = true;
        protected IVariableInventoryAsset currentImageAsset;

        #region 내가 추가함
        //[SerializeField] Transform parentsTrf;
        #endregion

        public Vector2 GetCellSize()
        {
            var width = ((CellData?.Width ?? 1) * (defaultCellSize.x + margineSpace.x)) - margineSpace.x;
            var height = ((CellData?.Height ?? 1) * (defaultCellSize.y + margineSpace.y)) - margineSpace.y;
            return new Vector2(width, height);
        }

        public Vector2 GetRotateCellSize()
        {
            var isRotate = CellData?.IsRotate ?? false;
            var cellSize = GetCellSize();
            if (isRotate)
            {
                var tmp = cellSize.x;
                cellSize.x = cellSize.y;
                cellSize.y = tmp;
            }

            return cellSize;
        }

        public override void SetSelectable(bool value)
        {
            ButtonActions.SetActive(value);
            isSelectable = value;
        }

        public virtual void SetHighLight(bool value)
        {
            highlight.gameObject.SetActive(value);
        }

        // 인벤토리 상의 아이템 이동에 관한 부분
        protected override void OnApply()
        {
            SetHighLight(false);
            target.gameObject.SetActive(CellData != null);
            ApplySize();

            if (CellData == null)
            {
                // 이동 후
                cellImage.gameObject.SetActive(false);
                background.gameObject.SetActive(false);
                //Debug.Log("아이템 해제");
                ParentCheck(transform);
            }
            else
            {
                //Debug.Log("아이템 착용");
                ParentCheck(transform);
                // update cell image
                if (currentImageAsset != CellData.ImageAsset)
                {
                    currentImageAsset = CellData.ImageAsset;

                    cellImage.gameObject.SetActive(false);
                    if (Loader == null)
                    {
                        Loader = new StandardAssetLoader();
                    }

                    StartCoroutine(Loader.LoadAsync(CellData.ImageAsset, tex =>
                    {
                        cellImage.texture = tex;
                        cellImage.gameObject.SetActive(true);
                    }));
                }

                background.gameObject.SetActive(true && isSelectable);
            }
        }

        protected virtual void ApplySize()
        {
            sizeRoot.sizeDelta = GetRotateCellSize();
            target.sizeDelta = GetCellSize();
            target.localEulerAngles = Vector3.forward * (CellData?.IsRotate ?? false ? 90 : 0);
        }

        bool ParentCheck(Transform transform)
        {
            for(int i=0; i<4; i++)
            {
                transform = transform.parent;

                //if(i==3)
                    //Debug.Log($"이름: {transform.name}");
            }

            return true;
        }
    }
}
