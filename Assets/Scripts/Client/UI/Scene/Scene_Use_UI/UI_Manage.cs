using UnityEngine;

public class UI_Manage : UI_Scene
{
	[Header("GameObject")]
	public GameObject LoadingPanelObject => GetObject((int)GameObjects.LoadingPanel);
		
	enum GameObjects
	{
		LoadingPanel
	}

	public override void Init()
	{
		// 게임오브젝트 바인드
		Bind<GameObject>(typeof(GameObjects));
		
		GetObject((int)GameObjects.LoadingPanel).gameObject.SetActive(true); // 로딩 패널 켜기		
	}

}