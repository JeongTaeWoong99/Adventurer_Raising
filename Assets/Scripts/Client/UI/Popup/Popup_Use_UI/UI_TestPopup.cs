using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_TestPopup : UI_Popup
{
    enum Buttons
    {
        ScoreButton
    }

    enum Texts
    {
        ScoreText,
    }

    enum Images
    {
        ItemIcon,
    }

    public override void Init()
    {
        base.Init();
        
        // 버튼 바인딩
        Bind<Button>(typeof(Buttons));
        var scoreButton = GetButton((int)Buttons.ScoreButton);
        scoreButton.gameObject.BindEvent(OnButtonClicked);
        
        // 텍스트 바인딩
        Bind<TextMeshProUGUI>(typeof(Texts));
        var scoreText = GetText((int)Texts.ScoreText);
        scoreText.text = "Score : 0";
        
        // 이미지 바인딩
        Bind<Image>(typeof(Images));
        GameObject go = GetImage((int)Images.ItemIcon).gameObject;
        BindEvent(go, data => { go.transform.position = data.position; }, Define.UIEvent.Drag);
    }

    int _score = 0;
    public void OnButtonClicked(PointerEventData data)
    {
        _score++;
        var scoreText = GetText((int)Texts.ScoreText);
        scoreText.text = $"Score : {_score}";
    }

}
