public class UI_Popup : UI_Base
{
    // 팝업 UI는 Sort사용(나중에 실행된 UI가 앞으로 오도록)
    public override void Init()
    {   // UIManager
        ClientManager.UI.SetCanvas(gameObject, true);
    }
    
    // 맨 앞에 있는 UI부터 차례로 삭제
    public virtual void ClosePopupUI()
    {   // UIManager
        ClientManager.UI.ClosePopupUI(this);
    }
}