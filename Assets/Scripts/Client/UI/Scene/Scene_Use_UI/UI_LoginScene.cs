using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_LoginScene : UI_Scene
{
	[Header("Button")]
	public Button StartButton => GetButton((int)Buttons.StartButton);

	[Header("GameObject")]
	public GameObject LoginPanelObject   => GetObject((int)GameObjects.LoginPanel);
	public GameObject AccountPanelObject => GetObject((int)GameObjects.AccountPanel);
	public GameObject StartPanel         => GetObject((int)GameObjects.StartPanel);
	
	[Header("TMP_InputField")]
	public TMP_InputField EmailPlaceholderText    => GetInputField((int)InputFields.EmailPlaceholderText);
	public TMP_InputField PasswordPlaceholderText => GetInputField((int)InputFields.PasswordPlaceholderText);
	public TMP_InputField MakeEmailPlaceholder    => GetInputField((int)InputFields.MakeEmailPlaceholder);
	public TMP_InputField MakePasswordPlaceholder => GetInputField((int)InputFields.MakePasswordPlaceholder);
	public TMP_InputField MakeNickNamePlaceholder => GetInputField((int)InputFields.MakeNickNamePlaceholder);

	[Header("TextMeshProUGUI")]
	public TextMeshProUGUI LoginResultText   => GetText((int)Texts.LoginResultText);
	public TextMeshProUGUI AccountResultText => GetText((int)Texts.AccountResultText);
	public TextMeshProUGUI NickNameText                  => GetText((int)Texts.NickNameText);
	public TextMeshProUGUI SerialNumberNameText          => GetText((int)Texts.SerialNumberNameText);
	public TextMeshProUGUI CurrentHpText				 => GetText((int)Texts.CurrentHpText);
	public TextMeshProUGUI CurrentLevelText				 => GetText((int)Texts.CurrentLevelText);
	public TextMeshProUGUI CurrentExpText                => GetText((int)Texts.CurrentExpText);
	public TextMeshProUGUI CurrentGoldText               => GetText((int)Texts.CurrentGoldText);
	public TextMeshProUGUI SavedSceneText                => GetText((int)Texts.SavedSceneText);
	
	enum Buttons
	{
		StartButton, AccountPanelOpenButton, AccountCreateButton, CloseButton, LoginButton, ExitButton
	}
	
	enum GameObjects
	{
		LoginPanel, AccountPanel, StartPanel
	}
	
	enum InputFields
	{
		EmailPlaceholderText, PasswordPlaceholderText, MakeEmailPlaceholder, MakePasswordPlaceholder, MakeNickNamePlaceholder
	}

	enum Texts
	{
		LoginResultText, AccountResultText,
		NickNameText, SerialNumberNameText, CurrentLevelText, CurrentHpText, CurrentExpText, CurrentGoldText, SavedSceneText
	}

	public override void Init()
	{
		base.Init();

		// 버튼 바인드
		Bind<Button>(typeof(Buttons));
		
		GetButton((int)Buttons.StartButton).gameObject.BindEvent(OnStartButtonClicked);	// 스타트 버튼 이벤트 등록
		
		GetButton((int)Buttons.AccountPanelOpenButton).gameObject.BindEvent(OnAccountPanelOpenButtonClicked); // 계정 생성 버튼 이벤트 등록
																				
		GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnCloseButtonClicked);					      // 계정생성 창 닫기 이벤트 등록
		
		GetButton((int)Buttons.AccountCreateButton).gameObject.BindEvent(DBManager.Auth.OnRequestMakeId);     // 계정 생성 요청  이벤트 등록
		
		GetButton((int)Buttons.LoginButton).gameObject.BindEvent(DBManager.Auth.OnRequestLogin);	          // 로그인 요청 이벤트 등록
		
		GetButton((int)Buttons.ExitButton).gameObject.BindEvent(OnExitGame);								  // 게임 종료 버튼
		
		// 게임오브젝트 바인드
		Bind<GameObject>(typeof(GameObjects));
		
		GetObject((int)GameObjects.LoginPanel).gameObject.SetActive(false);	  // 로그인 패널 끄기

		GetObject((int)GameObjects.AccountPanel).gameObject.SetActive(false); // 계정생성 패널 끄기
		
		GetObject((int)GameObjects.StartPanel).gameObject.SetActive(false);   // 시작 패널 끄기		
		
		// TMP_텍스트 바인딩
		Bind<TMP_InputField>(typeof(InputFields));
		
		// 일반 텍스트
		Bind<TextMeshProUGUI>(typeof(Texts));
		
		GetText((int)Texts.LoginResultText).gameObject.SetActive(true);   // 텍스트 끄기
		GetText((int)Texts.AccountResultText).gameObject.SetActive(true); // 텍스트 끄기
	}
	
	// Managers를 통해, 게임씬 호출
	// 나중에는 DB에서 참고해서, 저장된 위치로 씬 불러오기
	private void OnStartButtonClicked(PointerEventData data)
	{
		StartPanel.SetActive(false);
	
		// 클라이언트 클리어
		ClientManager.Clear();
		
		// manageUI 로딩 켜기
		ClientManager.UI.manageUI.LoadingPanelObject.SetActive(true);
		
		// LoginToSave 타입의 mmNumber 저장
		ClientManager.Scene.SceneChangeMmNumber = "N00";
		
		// 이동 씬(로그인 때, 받아온 저장된 씬 이름으로 씬을 이동)
		// 저장된 씬으로 이동(저장된 씬 이름이 존재)
		string targetSceneName = DBManager.RealTime.myDefaultData.savedScene;
		if (System.Enum.TryParse(targetSceneName, out Define.SceneName sceneToLoad))
			ClientManager.Scene.LoadScene(sceneToLoad);
		// 마을로 이동(처음 생성하여, UnKnown이거나 or 방을 찾지 못함)
		else
			ClientManager.Scene.LoadScene(Define.SceneName.Village);
	}
	
	public void SceneLoadedSetting()
	{
		ClientManager.UI.manageUI.LoadingPanelObject.SetActive(false); // manageUI 로딩 끄기
		
		GetObject((int)GameObjects.LoginPanel).gameObject.SetActive(true);
		GetText((int)Texts.LoginResultText).gameObject.SetActive(true);
		GetText((int)Texts.LoginResultText).text = "로그인 정보를 입력하세요.";
	}

	private void OnAccountPanelOpenButtonClicked(PointerEventData data)
	{
		GetObject((int)GameObjects.AccountPanel).gameObject.SetActive(true); 
		GetText((int)Texts.AccountResultText).text = "가입 정보를 입력하세요.";	
	}

	private void OnCloseButtonClicked(PointerEventData data)
	{
		GetObject((int)GameObjects.AccountPanel).gameObject.SetActive(false);
		GetText((int)Texts.LoginResultText).text = "로그인 정보를 입력하세요.";
	}

	private void OnExitGame(PointerEventData data)
	{
		Debug.Log("게임 종료 버튼 클릭");
		Application.Quit();
	}
}