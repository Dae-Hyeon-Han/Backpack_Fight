using System.Collections;
using UnityEngine;
using VariableInventorySystem;
using VariableInventorySystem.Sample;

public class SampleScene : MonoBehaviour
{
    [SerializeField] StandardCore standardCore;                         // 컨트롤 등 핵심 기능
    [SerializeField] StandardStashView standardStashView;               // 실제 보여지고 상호작용 할 오브젝트
    [SerializeField] UnityEngine.UI.Button rotateButton;                // 무엇을 위한 버튼인지?

    void Awake()
    {
        standardCore.Initialize();
        standardCore.AddInventoryView(standardStashView);

        rotateButton.onClick.AddListener(standardCore.SwitchRotate);

        StartCoroutine(InsertCoroutine());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            standardCore.SwitchRotate();
        }
    }

    IEnumerator InsertCoroutine()
    {
        var stashData = new StandardStashViewData(8, 16);

        var caseItem = new CaseCellData(0);
        stashData.InsertInventoryItem(stashData.GetInsertableId(caseItem).Value, caseItem);
        standardStashView.Apply(stashData);

        // 디폴트로 아이템 생성하는 코드
        for (var i = 0; i < 20; i++)
        {
            var item = new ItemCellData(i % 6);
            stashData.InsertInventoryItem(stashData.GetInsertableId(item).Value, item);
           standardStashView.Apply(stashData);

            yield return null;
        }
    }
}
