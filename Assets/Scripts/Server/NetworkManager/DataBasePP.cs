using System;

public class DataBasePP
{
	private static DataBasePP instance;	
	
	private UI_LoginScene UI_LoginScene;
	private UI_Manage     UI_Manage;

	public void Init()
	{
		UI_LoginScene = ClientManager.UI.loginSceneUI;
		UI_Manage     = ClientManager.UI.manageUI;
	}
	public void Clear()
	{
	
	}
	
	// 아이디 만들기 피드백
	public void MakeIdResult(S_MakeIdResult p)
	{
		UI_Manage.LoadingPanelObject.SetActive(false);	      // 로딩 off
		UI_LoginScene.ResultText.gameObject.SetActive(true); // 결과 텍스트 on
		UI_LoginScene.ResultText.text = p.resultText;		 // 결과 텍스트 변경
		
		// 아이디 만들기 성공
		if (p.isSuccess)
		{
			UI_LoginScene.AccountPanelObject.SetActive(false); // 아이디 생성 패널 닫기
		}
		// 아이디 만들기 실패
		else
		{
			
		}
	}
	
	// 로그인 피드백
	public void LoginResult(S_LoginResult p)
	{
		UI_Manage.LoadingPanelObject.SetActive(false); // 로딩 off
		UI_LoginScene.ResultText.text = p.resultText;  // 결과 텍스트 변경
		
		// 로그인 성공
		if (p.isSuccess)
		{
			UI_LoginScene.ResultText.gameObject.SetActive(false);  // 결과 텍스트 off
			UI_LoginScene.LoginPanelObject.SetActive(false);	   // 로그인 패널 off
			UI_LoginScene.StartButton.gameObject.SetActive(true); // 시작 버튼 켜기
			
			// 참고 정보 세팅
			DBManager.RealTime.myDefaultData.email         = p.email;
			DBManager.RealTime.myDefaultData.creationDate  = p.creationDate;
			DBManager.RealTime.myDefaultData.nickname      = p.nickname;
			DBManager.RealTime.myDefaultData.serialNumber  = p.serialNumber;
			DBManager.RealTime.myDefaultData.currentLevel  = p.currentLevel.ToString();
			DBManager.RealTime.myDefaultData.currentHp     = p.currentHp.ToString();
			DBManager.RealTime.myDefaultData.currentExp    = p.currentExp.ToString();
			DBManager.RealTime.myDefaultData.currentGold   = p.currentGold.ToString();
			DBManager.RealTime.myDefaultData.savedScene    = p.savedScene;
		}
		// 로그인 실패
		else
		{
			UI_LoginScene.ResultText.gameObject.SetActive(true); // 결과 텍스트 on
		}
	}
}
