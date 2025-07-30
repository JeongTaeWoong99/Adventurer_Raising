using TMPro;
using UnityEngine;

public class UI_Manage : UI_Scene
{
	[Header("GameObject")]
	public GameObject LoadingPanelObject => GetObject((int)GameObjects.LoadingPanel);
	public GameObject ToolTip			 => GetObject((int)GameObjects.ToolTip);
		
	[Header("TextMeshProUGUI")]
	public TextMeshProUGUI ToolTipNameText => GetText((int)Texts.ToolTipNameText);
	public TextMeshProUGUI PktInfoText     => GetText((int)Texts.PktInfoText);
	
	enum GameObjects
	{
		LoadingPanel, ToolTip
	}
	
	enum Texts
	{
		ToolTipNameText, PktInfoText
	}

	public override void Init()
	{
		// 게임오브젝트 바인드
		Bind<GameObject>(typeof(GameObjects));
		
		GetObject((int)GameObjects.LoadingPanel).gameObject.SetActive(true); // 로딩 패널 켜기	
		
		GetObject((int)GameObjects.ToolTip).gameObject.SetActive(false);	 // 툴팁 끄기
		
		// 일반 텍스트
		Bind<TextMeshProUGUI>(typeof(Texts));
	}
}