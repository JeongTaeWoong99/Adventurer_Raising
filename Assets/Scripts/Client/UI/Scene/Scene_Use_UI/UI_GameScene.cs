using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_GameScene : UI_Scene
{
    [Header("TextMeshProUGUI")]
    public TextMeshProUGUI HpText       => GetText((int)Texts.HpText);
    public TextMeshProUGUI MpText       => GetText((int)Texts.MpText);
    public TextMeshProUGUI NickNameText => GetText((int)Texts.NickNameText);
    
    [Header("TMP_InputField")]
    public TMP_InputField ChatInputField => GetInputField((int)InputFields.ChatInputField);
    
    [Header("Sliders")]
    public Slider HpSlider       => GetSlider((int)Sliders.HpSlider);
    public Slider MpSlider       => GetSlider((int)Sliders.MpSlider);
    
    [Header("GameObjects")]
    public GameObject ChatContentGroup => GetObject((int)GameObjects.ChatContentGroup);

    enum Sliders
    {
        ExpSlider, HpSlider, MpSlider,
    }
    
    enum Texts
    {
        ExpText, NickNameText, HpText, MpText
    }
    
    enum Buttons
    {
        GoToLoginButton, GoToVillageButton, GoToVillageDeathButton, ExitLButton
    }
    
    enum GameObjects
    {
        DeathPanel, ChatContentGroup
    }
    
    enum InputFields
    {
        ChatInputField
    }

    public override void Init()
    {
        base.Init();

        // 슬라이더 바인드
        Bind<Slider>(typeof(Sliders));
        
        // 텍스트 바인딩
        Bind<TextMeshProUGUI>(typeof(Texts));
        
        // 버튼 바인드
        Bind<Button>(typeof(Buttons));
        GetButton((int)Buttons.GoToLoginButton).gameObject.BindEvent(GoToLoginButtonClicked);          // 로그아웃 및 로그인화면으로 돌아가기
        
        GetButton((int)Buttons.GoToVillageButton).gameObject.BindEvent(GoToVillageButtonClicked);      // 마을로 워프

        GetButton((int)Buttons.GoToVillageDeathButton).gameObject.BindEvent(GoToVillageButtonClicked); // 마을로 돌아가기
        
        GetButton((int)Buttons.ExitLButton).gameObject.BindEvent(OnExitGame);                          // 게임 종료
        
        // 게임오브젝트 바인드
        Bind<GameObject>(typeof(GameObjects));
        GetObject((int)GameObjects.DeathPanel).gameObject.SetActive(false);	// 마을로돌아아기 패널 끄기
        
        // TMP_텍스트 바인딩
        Bind<TMP_InputField>(typeof(InputFields));
    }

    public void OnStateChange(int hp, int maxHp, int mp, int maxMp, string nickName = null)
    {
        // 닉네임 텍스트 설정
        if(nickName != null)
            NickNameText.text = nickName;
        
        // Hp 설정 
        HpText.text       = hp + " / " + maxHp;
        HpSlider.maxValue = maxHp;
        HpSlider.value    = hp;
        
        // Mp 설정
        MpText.text       = mp + " / " + maxMp;
        MpSlider.maxValue = maxMp;
        MpSlider.value    = mp;
    }
    
    public void OnExpSliderChanged(int exp, int maxExp, string maxLevelTextSet = null)
    {
        if (maxLevelTextSet != null) GetText((int)Texts.ExpText).text = maxLevelTextSet;      // 최고 레벨 텍스트 설정  
        else                         GetText((int)Texts.ExpText).text = exp + " / " + maxExp; // 일반 레벨 텍스트 설정
        GetSlider((int)Sliders.ExpSlider).maxValue = maxExp;
        GetSlider((int)Sliders.ExpSlider).value    = exp;
    }
    
    private async void GoToLoginButtonClicked(PointerEventData data)
    {
        try
        {
            // 클라이언트 클리어
            ClientManager.Clear();
            
            ClientManager.UI.manageUI.LoadingPanelObject.gameObject.SetActive(true);
        
            // 로그인 화면으로 돌아가는 경우, 네트워크 끊어주기
            NetworkManager.Instance._session.Disconnect();
            
            // 1초 대기
            await Task.Delay(1000);
        
            ClientManager.Scene.LoadScene(Define.SceneName.Login);
        }
        catch (Exception e)
        {
            Debug.Log("예외 발생" + e);
        }
    }
    
    private void GoToVillageButtonClicked(PointerEventData data)
    {
        // 클라이언트 클리어
        ClientManager.Clear();
        
        // manageUI 로딩 켜기
        ClientManager.UI.manageUI.LoadingPanelObject.SetActive(true);
				
        // 포탈의 개인 MmNumber 넣어주기
        ClientManager.Scene.SceneChangeMmNumber = "N01";                    // 마을 버튼 이동 mmNumber
        ClientManager.Scene.PortalMoveSceneName = Define.SceneName.Village;
			
        // 방 떠나기
        C_EntityLeave leavePacket = new C_EntityLeave();
        NetworkManager.Instance.Send(leavePacket.Write());
        // ---> 내 캐릭터가 정상적으로 나가지면, ManagementPP의 EntityLeave를 통해, 씬 전환이 이루어짐....
    }

    public void DeathPanelSetting(bool isActive)
    {
        GetObject((int)GameObjects.DeathPanel).gameObject.SetActive(isActive);	// 마을로돌아아기 패널 켜기
        GetButton((int)Buttons.GoToVillageDeathButton).gameObject.SetActive(isActive);      // 마을로돌아가기 버튼 켜기
    }
    
    private void OnExitGame(PointerEventData data)
    {
        Debug.Log("게임 종료 버튼 클릭");
        Application.Quit();
    }
}