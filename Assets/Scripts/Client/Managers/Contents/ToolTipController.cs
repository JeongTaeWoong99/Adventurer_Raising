using UnityEngine;
using UnityEngine.EventSystems;

public class ToolTipController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea]
    public string iconName;

    // 마우스가 ToolTipController가 들어간 UI에 닿으면, 발동
    public void OnPointerEnter(PointerEventData eventData)
    {
        ClientManager.ToolTip.SetupToolTip(iconName);      // 툴팁 텍스트 설정 
        ClientManager.UI.manageUI.ToolTip.SetActive(true); // 켜기
    }

    // 마우스가 ToolTipController가 들어간 UI에서 나가면, 발동
    public void OnPointerExit(PointerEventData eventData)
    {
        ClientManager.UI.manageUI.ToolTip.SetActive(false);  // 끄기
    }
}