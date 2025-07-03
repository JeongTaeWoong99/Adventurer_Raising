using UnityEngine;
using UnityEngine.EventSystems;

public class AuthManager
{
	public void Init()
	{
    }

	public void OnRequestMakeId(PointerEventData data)
	{
		UI_LoginScene UI_LoginScene = ClientManager.UI.loginSceneUI;
	
		if (string.IsNullOrEmpty(UI_LoginScene.MakeEmailPlaceholder.text) || string.IsNullOrEmpty(UI_LoginScene.MakePasswordPlaceholder.text) ||
			string.IsNullOrEmpty(UI_LoginScene.MakeNickNamePlaceholder.text))
		{
		    Debug.Log("이메일 또는 비밀번호 또는 닉네임이 비어있습니다.");
		    return;
		}
		ClientManager.UI.manageUI.LoadingPanelObject.SetActive(true); // manageUI 로딩 켜기
		
		// 서버에 생성 요청 보내기
		C_RequestMakeId makeId = new C_RequestMakeId
		{
			email        = UI_LoginScene.MakeEmailPlaceholder.text,
			password     = UI_LoginScene.MakePasswordPlaceholder.text,
			nickName     = UI_LoginScene.MakeNickNamePlaceholder.text,
			serialNumber = "P000"										// TODO : 추후, 캐릭터를 늘린다면, 아이디 생성에서 남캐 여캐 선택할 수 있도록 함...
		};
		NetworkManager.Instance.Send(makeId.Write());
		// --> 결과는 핸들러에서 받아옴....
	}
	
	public void OnRequestLogin(PointerEventData data)
	{
		UI_LoginScene UI_LoginScene = ClientManager.UI.loginSceneUI;
	
		if (string.IsNullOrEmpty(UI_LoginScene.EmailPlaceholderText.text) || string.IsNullOrEmpty(UI_LoginScene.PasswordPlaceholderText.text))
		{
			Debug.Log("이메일 또는 비밀번호가 비어있습니다.");
			return;
		}
		ClientManager.UI.manageUI.LoadingPanelObject.SetActive(true); // manageUI 로딩 켜기
		
		// 서버에 생성 요청 보내기
		C_RequestLogin login = new C_RequestLogin
		{
			email    = UI_LoginScene.EmailPlaceholderText.text,
			password = UI_LoginScene.PasswordPlaceholderText.text
		};
		NetworkManager.Instance.Send(login.Write());
		// --> 결과는 핸들러에서 받아옴....
	}
}