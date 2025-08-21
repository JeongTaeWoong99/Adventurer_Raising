using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_GameScene : UI_Scene
{
    // 스킬 쿨타임 상태
    private readonly Dictionary<string, float> _skillRemain = new Dictionary<string, float>();   // 남은 시간
    private readonly Dictionary<string, float> _skillDur    = new Dictionary<string, float>();   // 총 쿨타임

    [Header("TextMeshProUGUI")]
    public TextMeshProUGUI HpText       => GetText((int)Texts.HpText);
    public TextMeshProUGUI MpText       => GetText((int)Texts.MpText);
    public TextMeshProUGUI NickNameText => GetText((int)Texts.NickNameText);
    public TextMeshProUGUI MapNameText  => GetText((int)Texts.MapNameText);
    public TextMeshProUGUI RemainQText  => GetText((int)Texts.RemainQText);
    public TextMeshProUGUI RemainWText  => GetText((int)Texts.RemainWText);
    public TextMeshProUGUI RemainEText  => GetText((int)Texts.RemainEText);
    public TextMeshProUGUI RemainRText  => GetText((int)Texts.RemainRText);
    
    [Header("TMP_InputField")]
    public TMP_InputField ChatInputField => GetInputField((int)InputFields.ChatInputField);
    
    [Header("Sliders")]
    public Slider HpSlider       => GetSlider((int)Sliders.HpSlider);
    public Slider MpSlider       => GetSlider((int)Sliders.MpSlider);
    
    [Header("GameObjects")]
    public GameObject ChatContentGroup => GetObject((int)GameObjects.ChatContentGroup);
    
    [Header("Image")]
    public Image SkillQ_Image => GetImage((int)Images.SkillQ_Image);
    public Image SkillW_Image => GetImage((int)Images.SkillW_Image);
    public Image SkillE_Image => GetImage((int)Images.SkillE_Image);
    public Image SkillR_Image => GetImage((int)Images.SkillR_Image);
    public Image HideQImage => GetImage((int)Images.HideQImage);
    public Image HideWImage => GetImage((int)Images.HideWImage);
    public Image HideEImage => GetImage((int)Images.HideEImage);
    public Image HideRImage => GetImage((int)Images.HideRImage);

    enum Sliders
    {
        ExpSlider, HpSlider, MpSlider,
    }
    
    enum Texts
    {
        ExpText, NickNameText, HpText, MpText, MapNameText, RemainQText, RemainWText, RemainEText, RemainRText
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

    enum Images
    {
        SkillQ_Image, SkillW_Image, SkillE_Image, SkillR_Image, HideQImage, HideWImage, HideEImage, HideRImage
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
        
        // 이미지 바인딩
        Bind<Image>(typeof(Images));
    }

    // 게임씬에 들어오고, 생성이 완료된 후, UI 세팅해
    public void GameSecneUiSetting()
    {
        // 사망 UI 끄기
        DeathPanelSetting(false);                
        
        // 텍스트 설정     
        if      (SceneManager.GetActiveScene().name == "Village") MapNameText.text = "마을";
        else if (SceneManager.GetActiveScene().name == "Stage1")  MapNameText.text = "사냥터1";
        else if (SceneManager.GetActiveScene().name == "Stage2")  MapNameText.text = "사냥터2";

        // 스킬 쿨타임 초기화
        _skillRemain.Clear();
        _skillDur.Clear();

        RemainQText.gameObject.SetActive(false);
        RemainWText.gameObject.SetActive(false);
        RemainEText.gameObject.SetActive(false);
        RemainRText.gameObject.SetActive(false);
    }
    
    // 플레이어 세팅 후, UI 세팅
    public void OnStateChange(int hp, int maxHp, int mp, int maxMp, int level ,string nickName = null)
    {
        // 닉네임 텍스트 설정
        if(nickName != null)
            NickNameText.text = "LV. " + level + " " + nickName;
        
        // Hp 설정 
        HpText.text       = hp + " / " + maxHp;
        HpSlider.maxValue = maxHp;
        HpSlider.value    = hp;
        
        // Mp 설정
        MpText.text       = mp + " / " + maxMp;
        MpSlider.maxValue = maxMp;
        MpSlider.value    = mp;
    }
    
    // 플레이어 세팅 후, UI 세팅
    public void SkillSetting()
    {
        string serialNumber = ClientManager.Game.MyPlayerGameObject.GetComponent<BaseController>().infoState.serialNumber;
        // 스킬 세팅(Q W E R)
        var attackDict = ClientManager.Data.AttackInfoDict;
        // Q
        string ui_imageKeyQ = "S" + serialNumber + "_Q";
        AttackInfoData attackInfoQ = attackDict[ui_imageKeyQ];
        ClientManager.UI.gameSceneUI.SkillQ_Image.sprite   = ClientManager.Resource.R_Load<Sprite>("UI/" + attackInfoQ.image);
        ClientManager.UI.gameSceneUI.HideQImage.fillAmount = 0f;
        
        // W
        string ui_imageKeyW = "S" + serialNumber + "_W";
        AttackInfoData attackInfoW = attackDict[ui_imageKeyW];
        ClientManager.UI.gameSceneUI.SkillW_Image.sprite   = ClientManager.Resource.R_Load<Sprite>("UI/" + attackInfoW.image);
        ClientManager.UI.gameSceneUI.HideWImage.fillAmount = 0f;
        
        // E
        string ui_imageKeyE = "S" + serialNumber + "_E";
        AttackInfoData attackInfoE = attackDict[ui_imageKeyE];
        ClientManager.UI.gameSceneUI.SkillE_Image.sprite   = ClientManager.Resource.R_Load<Sprite>("UI/" + attackInfoE.image);
        ClientManager.UI.gameSceneUI.HideEImage.fillAmount = 0f;
        
        // R
        string ui_imageKeyR = "S" + serialNumber + "_R";
        AttackInfoData attackInfoR = attackDict[ui_imageKeyR];
        ClientManager.UI.gameSceneUI.SkillR_Image.sprite   = ClientManager.Resource.R_Load<Sprite>("UI/" + attackInfoR.image);
        ClientManager.UI.gameSceneUI.HideRImage.fillAmount = 0f;
    }

    // 레벨에 따른 스킬 잠금/해제 표시 및 초기화
    public void UpdateSkillUnlockByLevel(int level)
    {
        // 잠금 시: 아이콘 반투명 + 가리개 100%
        // 해제 시: 아이콘 원색 + 가리개 0%
        SetSkillUnlockVisual("Q", level >= 1);
        SetSkillUnlockVisual("W", level >= 2);
        SetSkillUnlockVisual("E", level >= 3);
        SetSkillUnlockVisual("R", level >= 4);
    }

    private void SetSkillUnlockVisual(string key, bool unlocked)
    {
        Image icon  = null;
        Image cover = GetHideImageByKey(key);

        switch (key)
        {
            case "Q": icon = SkillQ_Image; break;
            case "W": icon = SkillW_Image; break;
            case "E": icon = SkillE_Image; break;
            case "R": icon = SkillR_Image; break;
        }

        if (icon != null)
            icon.color = unlocked ? Color.white : new Color(1f, 1f, 1f, 0.35f);

        if (cover != null)
            cover.fillAmount = unlocked ? 0f : 1f;

        // 레벨로 잠금된 경우 남은시간 텍스트는 숨김 유지
        var text = GetRemainTextByKey(key);
        if (!unlocked && text != null)
            text.gameObject.SetActive(false);
    }

    // 쿨타임 시작 (key: "Q"/"W"/"E"/"R")
    public void StartSkillCooldown(string key, float duration)
    {
        if (string.IsNullOrEmpty(key) || duration <= 0f)
            return;

        _skillDur[key]    = duration;
        _skillRemain[key] = duration;

        var text  = GetRemainTextByKey(key);
        var image = GetHideImageByKey(key);
        if (text != null)
        {
            text.gameObject.SetActive(true);
            text.text = Mathf.CeilToInt(duration).ToString();
        }
        if (image != null)
            image.fillAmount = 1f; // 가득 채움에서 0으로 감소
    }

    // 매 프레임 갱신 (UIManager에서 호출)
    public void UpdateSkillCooldownUI(float deltaTime)
    {
        if (_skillRemain.Count == 0)
            return;

        UpdateOneCooldown("Q", deltaTime);
        UpdateOneCooldown("W", deltaTime);
        UpdateOneCooldown("E", deltaTime);
        UpdateOneCooldown("R", deltaTime);
    }

    private void UpdateOneCooldown(string key, float deltaTime)
    {
        if (!_skillRemain.TryGetValue(key, out float remain) || remain <= 0f)
            return;

        remain -= deltaTime;
        if (remain < 0f) remain = 0f;
        _skillRemain[key] = remain;

        float duration = _skillDur.TryGetValue(key, out float d) ? d : 0f;
        var text  = GetRemainTextByKey(key);
        var image = GetHideImageByKey(key);
        if (duration > 0f)
        {
            if (image != null)
                image.fillAmount = remain / duration; // 1->0
            if (text != null)
                text.text = Mathf.CeilToInt(remain).ToString();
        }

        if (remain <= 0f)
        {
            if (image != null)
                image.fillAmount = 0f;
            if (text != null)
                text.gameObject.SetActive(false);
        }
    }

    // 현재 쿨타임 여부 확인
    public bool IsSkillOnCooldown(string key)
    {
        return _skillRemain.TryGetValue(key, out float remain) && remain > 0f;
    }

    // 레벨 해제 여부 질의
    public bool IsSkillUnlocked(string key)
    {
        switch (key)
        {
            case "Q": return true;  // 최소 1레벨 이상만 게임 시작 가정
            case "W": return ClientManager.Game.MyPlayerGameObject.GetComponent<PlayerInfoState>().Level >= 2;
            case "E": return ClientManager.Game.MyPlayerGameObject.GetComponent<PlayerInfoState>().Level >= 3;
            case "R": return ClientManager.Game.MyPlayerGameObject.GetComponent<PlayerInfoState>().Level >= 4;
            default:   return false;
        }
    }

    private TextMeshProUGUI GetRemainTextByKey(string key)
    {
        switch (key)
        {
            case "Q": return RemainQText;
            case "W": return RemainWText;
            case "E": return RemainEText;
            case "R": return RemainRText;
            default:   return null;
        }
    }

    private Image GetHideImageByKey(string key)
    {
        switch (key)
        {
            case "Q": return HideQImage;
            case "W": return HideWImage;
            case "E": return HideEImage;
            case "R": return HideRImage;
            default:   return null;
        }
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
        GetObject((int)GameObjects.DeathPanel).gameObject.SetActive(isActive);	       // 마을로돌아아기 패널 켜기
        GetButton((int)Buttons.GoToVillageDeathButton).gameObject.SetActive(isActive); // 마을로돌아가기 버튼 켜기
    }
    
    private void OnExitGame(PointerEventData data)
    {
        Debug.Log("게임 종료 버튼 클릭");
        Application.Quit();
    }
}