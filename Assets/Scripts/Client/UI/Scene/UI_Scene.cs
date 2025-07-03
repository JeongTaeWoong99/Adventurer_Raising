public class UI_Scene : UI_Base
{
	// 팝업 UI는 Sort 사용하지 않음.(씬에 계속 켜져있는 맨 뒤 UI들 이기 때문에)
	public override void Init()
	{	
		// UIManager
		ClientManager.UI.SetCanvas(gameObject, false);
	}
}