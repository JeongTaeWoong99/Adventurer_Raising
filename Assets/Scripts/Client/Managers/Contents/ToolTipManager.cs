using UnityEngine;

public class ToolTipManager
{
    // 툴팁 화면 밖으로 나가는걸 방지
    private RectTransform tooltipRect;
    private float         tooltipOffset = 10f; // 툴팁 오프셋 전역변수

    public void Init()
    {
        // manageUI가 먼저 생성되야 하기 때문에, 여기서 초기화 불가
    }

    public void OnUpdate()
    {
        // UI가 준비되었을 때 초기화
        if (tooltipRect == null && ClientManager.UI.manageUI != null && ClientManager.UI.manageUI.ToolTip != null)
        {
            tooltipRect = ClientManager.UI.manageUI.ToolTip.GetComponent<RectTransform>();
        }

        // 툴팁이 활성화되어 있을 때만 위치 업데이트
        if (tooltipRect != null && ClientManager.UI.manageUI.ToolTip.activeInHierarchy)
        {
            // 마우스 오른쪽에 툴팁이 나타날 때 화면 밖으로 나가는지 판단
            if (Input.mousePosition.x + tooltipRect.sizeDelta.x + tooltipOffset > Screen.width)
            {
                // 화면 밖으로 나가면 왼쪽으로 표시
                tooltipRect.pivot = new Vector2(1, 0.5f);
                ClientManager.UI.manageUI.ToolTip.transform.position = Input.mousePosition - new Vector3(tooltipOffset, 0, 0);
            }
            else
            {
                // 기본: 마우스 오른쪽에 표시
                tooltipRect.pivot = new Vector2(0, 0.5f);
                ClientManager.UI.manageUI.ToolTip.transform.position = Input.mousePosition + new Vector3(tooltipOffset, 0, 0);
            }
        }
    }

    public void SetupToolTip(string name)
    {
        ClientManager.UI.manageUI.ToolTipNameText.text = name;
        Debug.Log("이름변경 =>" + name);
    }

    public void Clear()
    {
        
    }
}
