using System.Collections.Generic;
using UnityEngine;

namespace VariableInventorySystem
{
    public class StandardCore : VariableInventoryCore
    {
        [SerializeField] GameObject cellPrefab;
        [SerializeField] GameObject casePopupPrefab;
        [SerializeField] RectTransform effectCellParent;
        [SerializeField] RectTransform caseParent;

        protected override GameObject CellPrefab => cellPrefab;
        protected override RectTransform EffectCellParent => effectCellParent;

        protected List<IStandardCaseCellData> popupList = new List<IStandardCaseCellData>();

        private void Start()
        {
            // 외않데?
            //var standardCaseViewPopup = Instantiate(casePopupPrefab, caseParent).GetComponent<StandardCaseViewPopup>();
        }

        protected override void OnCellClick(IVariableInventoryCell cell)
        {
            if (cell.CellData is IStandardCaseCellData caseData)
            {
                if (popupList.Contains(caseData))
                {
                    return;
                }

                //Debug.Log("Chest 열기");

                popupList.Add(caseData);

                var standardCaseViewPopup = Instantiate(casePopupPrefab, caseParent).GetComponent<StandardCaseViewPopup>();
                AddInventoryView(standardCaseViewPopup.StandardCaseView);

                standardCaseViewPopup.Open(
                    caseData,
                    () =>
                    {
                        RemoveInventoryView(standardCaseViewPopup.StandardCaseView);
                        Destroy(standardCaseViewPopup.gameObject);
                        popupList.Remove(caseData);
                    });
            }
        }
    }
}
