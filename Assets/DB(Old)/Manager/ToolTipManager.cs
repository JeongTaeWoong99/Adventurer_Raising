using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToolTipManager : MonoBehaviour
{
    public static ToolTipManager instance;

    // 툴팁 화면 밖으로 나가는걸 방지
    private RectTransform tooltipRect;
    private float         halfwidth;
    private float         tooltipOffset = 10f; // 툴팁 오프셋 전역변수

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        tooltipRect = ClientManager.UI.manageUI.ToolTip.GetComponent<RectTransform>();
    }

    private void Update()
    {
        // 툴팁이 활성화되어 있을 때만 위치 업데이트
        if (ClientManager.UI.manageUI.ToolTip.activeInHierarchy)
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
}
