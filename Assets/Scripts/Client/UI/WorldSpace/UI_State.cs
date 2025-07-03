using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_State : UI_Base
{
    private Slider          hpSlider;
    private TextMeshProUGUI nickNameText;
    private TextMeshProUGUI levelText;
    
    private InfoState infoState;
    private Camera    mainCamera;
    
    private Transform parentTransform;
    private Collider  parentCollider;
    private float     offsetY;
    
    enum Sliders
    {
        HPSlider
    }
    
    enum Texts
    {
        NickNameText, LevelText
    }
    
    public override void Init()
    {
        // 선 세팅
        mainCamera      = Camera.main;
        parentTransform = transform.parent;
        infoState       = parentTransform.GetComponent<InfoState>(); // 1회만 받아오기
        parentCollider  = parentTransform.GetComponent<Collider>();  // 1회만 받아오기
         
        // 후 세팅
        Bind<Slider>(typeof(Sliders));
        hpSlider = GetSlider((int)Sliders.HPSlider);
        
        Bind<TextMeshProUGUI>(typeof(Texts));
        
        nickNameText      = GetText((int)Texts.NickNameText);
        nickNameText.text = infoState.NickName;
        
        levelText      = GetText((int)Texts.LevelText);
        levelText.text = infoState.Level.ToString();
        
        // Y축 오프셋 미리 계산
        if (parentCollider != null)
            offsetY = parentCollider.bounds.size.y;
    }

    private void OnEnable()
    {
        // 재활성화 될 때, 켜기
        if (hpSlider)
            AllUI_Enable();
    }

    // 부드러운 따라가기 = Update
    private void Update()
    {
        // 위치 업데이트
        transform.position = parentTransform.position + Vector3.up * offsetY;
        transform.rotation = mainCamera.transform.rotation;

        // HP 비율 업데이트
        float ratio = infoState.Hp / (float)infoState.MaxHp;
        
        // 체력이 남아있다면, 표시 및 업데이트
        if(ratio > 0)
            SetHpRatio(ratio);
        // 체력이 없다면, 꺼버리기
        else if (ratio <= 0 && hpSlider.gameObject.activeInHierarchy)
            AllUI_Disable();
    }

    private void SetHpRatio(float ratio)
    { 
        hpSlider.value = ratio;
    }

    private void AllUI_Disable()
    {
        levelText.gameObject.SetActive(false);
        nickNameText.gameObject.SetActive(false);
        hpSlider.gameObject.SetActive(false);
    }

    private void AllUI_Enable()
    {
        levelText.gameObject.SetActive(true);
        nickNameText.gameObject.SetActive(true);
        hpSlider.gameObject.SetActive(true);
    }
}