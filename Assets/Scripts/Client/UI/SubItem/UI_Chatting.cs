using System;
using TMPro;
using UnityEngine;

public class UI_Chatting : UI_Base
{
	[Header("TextMeshProUGUI")]
	public TextMeshProUGUI UI_ChattingText => GetText((int)Texts.UI_ChattingText);

	// 지연 적용 상태
	private bool   _isBound         = false;
	private bool   _hasData         = false;
	private string _pendingNickName = string.Empty;
	private string _pendingContents = string.Empty;

	enum Texts
	{
		UI_ChattingText
	}
	
	public override void Init()
	{
		Bind<TextMeshProUGUI>(typeof(Texts));
		_isBound = true;
		TryApply();
	}

	public void SetData(string nickname, string contents)
	{
		_pendingNickName = nickname;
		_pendingContents = contents;
		_hasData         = true;
		TryApply();
	}

	private void TryApply()
	{
		if (!_isBound || !_hasData)
			return;
		string timeText = DateTime.Now.ToString("HH:mm");
		UI_ChattingText.text = $"[{timeText}] {_pendingNickName} : {_pendingContents}";
	}
    
}
