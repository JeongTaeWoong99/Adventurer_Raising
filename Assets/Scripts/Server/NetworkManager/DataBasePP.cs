using System;
using UnityEngine;

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
		UI_Manage.LoadingPanelObject.SetActive(false);				 // 로딩 off
		UI_LoginScene.AccountResultText.gameObject.SetActive(true);  // 결과 텍스트 on
		UI_LoginScene.AccountResultText.text = p.resultText;		 // 결과 텍스트 변경
		
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
		Debug.Log("LoginResult" + p.isSuccess);
		UI_Manage.LoadingPanelObject.SetActive(false);		// 로딩 off
		UI_LoginScene.LoginResultText.text = p.resultText;  // 결과 텍스트 변경
		
		// 로그인 성공
		if (p.isSuccess)
		{
			UI_LoginScene.LoginResultText.gameObject.SetActive(false);  // 결과 텍스트 off
			UI_LoginScene.LoginPanelObject.SetActive(false);			// 로그인 패널 off
			UI_LoginScene.StartPanel.gameObject.SetActive(true);		// 시작 패널 on
			
			// 기본 정보 저장
			DBManager.RealTime.myDefaultData.email         = p.email;
			DBManager.RealTime.myDefaultData.creationDate  = p.creationDate;
			DBManager.RealTime.myDefaultData.nickname      = p.nickname;
			DBManager.RealTime.myDefaultData.serialNumber  = p.serialNumber;
			DBManager.RealTime.myDefaultData.currentLevel  = p.currentLevel.ToString();
			DBManager.RealTime.myDefaultData.currentHp     = p.currentHp.ToString();
			DBManager.RealTime.myDefaultData.currentExp    = p.currentExp.ToString();
			DBManager.RealTime.myDefaultData.currentGold   = p.currentGold.ToString();
			DBManager.RealTime.myDefaultData.savedScene    = p.savedScene;
			
			// 필요 시작 화면 정보 세팅
			UI_LoginScene.NickNameText.text = p.nickname;
			if  (p.serialNumber == "P000") UI_LoginScene.SerialNumberNameText.text = "검사";
			else						   UI_LoginScene.SerialNumberNameText.text = "직업 분류 없음";
			UI_LoginScene.CurrentHpText.text	= p.currentHp.ToString();
			UI_LoginScene.CurrentLevelText.text = p.currentLevel.ToString();
			UI_LoginScene.CurrentExpText.text   = p.currentExp.ToString();
			UI_LoginScene.CurrentGoldText.text  = p.currentGold.ToString();
			if      (p.savedScene == "Village") UI_LoginScene.SavedSceneText.text = "마을";
			else if (p.savedScene == "Stage1")  UI_LoginScene.SavedSceneText.text = "사냥터1";
			else if (p.savedScene == "Stage2")  UI_LoginScene.SavedSceneText.text = "사냥터2";
			else							    UI_LoginScene.SavedSceneText.text = "마을";
		}
		// 로그인 실패
		else
		{
			UI_LoginScene.LoginResultText.gameObject.SetActive(true); // 결과 텍스트 on
		}
	}
}
