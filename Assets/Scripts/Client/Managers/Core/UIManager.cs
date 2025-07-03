using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    int _order = 10; // 현재 팝업의 Sorting Order 번호

    Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();	// 팝업 UI는 여러개가 있을 수 있으니 스텍으로 저장...
    
    [HideInInspector] public UI_LoginScene loginSceneUI; // 로그인 씬 UI
	[HideInInspector] public UI_GameScene  gameSceneUI;  // 게임씬 씬 UI
	[HideInInspector] public UI_Manage     manageUI;     // 공통 관리 UI
    
    public GameObject Root
    {
        get
        {
			GameObject root = GameObject.Find("@UI_Root");
			if (root == null)
				root = new GameObject { name = "@UI_Root" };
				
			Object.DontDestroyOnLoad(root);
            return root;
		}
    }
	
	// 켄버스 sort 설정에 사용
    public void SetCanvas(GameObject go, bool sort = true)
    {
        Canvas canvas = Util.GetOrAddComponent<Canvas>(go);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;

        if (sort)
        {
            canvas.sortingOrder = _order;
            _order++;
        }
        else
        {
            canvas.sortingOrder = 0;
        }
    }
    
	// 팝업 UI 만드는데 사용.
	public T ShowPopupUI<T>(string name = null) where T : UI_Popup
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = ClientManager.Resource.R_Instantiate($"UI/Popup/{name}");
        T popup = Util.GetOrAddComponent<T>(go);
        _popupStack.Push(popup);

        go.transform.SetParent(Root.transform);

		return popup;
    }
	
	// 팝업의 닫기 버튼으로 삭제하는 메서드(Block으로 순서 보장)
    public void ClosePopupUI(UI_Popup popup)
    {
		if (_popupStack.Count == 0)
			return;

        if (_popupStack.Peek() != popup)
        {
            Debug.Log("Close Popup Failed!");
            return;
        }

        ClosePopupUI();
    }

	// 맨 나중에 띄운 팝업을 지워준다.(ESC에 넣어서 사용. Block으로 순서 보장)
    public void ClosePopupUI()
    {
        if (_popupStack.Count == 0)
            return;

        UI_Popup popup = _popupStack.Pop();
        ClientManager.Resource.R_Destroy(popup.gameObject);
        popup = null;
        _order--;
    }
	
	// 모두 닫기
    public void CloseAllPopupUI()
    {
        while (_popupStack.Count > 0)
            ClosePopupUI();
    }
    
    // 경험치 바, 지도, 시간, 상점 버튼 등등 만드는데 사용...(Sort상관 없이 맨 뒤에 있는 UI들)
	public T ShowSceneUI<T>(string name = null) where T : UI_Scene
	{
		if (string.IsNullOrEmpty(name))
			name = typeof(T).Name;

		GameObject go = ClientManager.Resource.R_Instantiate($"UI/Scene/{name}");
		T sceneUI = Util.GetOrAddComponent<T>(go);
		
		// 참조를 위해, 할당
		if(sceneUI.GetComponent<UI_LoginScene>())
			loginSceneUI = sceneUI.GetComponent<UI_LoginScene>();
		else if (sceneUI.GetComponent<UI_GameScene>())
			gameSceneUI = sceneUI.GetComponent<UI_GameScene>();
		else if (sceneUI.GetComponent<UI_Manage>())
			manageUI = sceneUI.GetComponent<UI_Manage>();

		go.transform.SetParent(Root.transform);

		return sceneUI;
	} 
	
	// UI_Scene로 받아서, 해당하는 게임 오브젝트 삭제해주기...
      public void CloseSceneUI(UI_Scene sceneUI)
      { 
	      ClientManager.Resource.R_Destroy(sceneUI.gameObject);
      }
	
	// 초기화 될 때, 켜져있는 UI 초기화
    
	// 인벤 속 아이템 만드는데 사용
	public T MakeSubItem<T>(Transform parent = null, string name = null) where T : UI_Base
	{
		if (string.IsNullOrEmpty(name))
			name = typeof(T).Name;

		GameObject go = ClientManager.Resource.R_Instantiate($"UI/SubItem/{name}");
		if (parent != null)
			go.transform.SetParent(parent);

		return Util.GetOrAddComponent<T>(go);
	}

	// 스테이터UI 등등 만드는데 사용
	public T MakeWorldSpaceUI<T>(Transform parent = null, string name = null) where T : UI_Base
	{
		if (string.IsNullOrEmpty(name))
			name = typeof(T).Name;

		GameObject go = ClientManager.Resource.R_Instantiate($"UI/WorldSpace/{name}");
		if (parent != null)
			go.transform.SetParent(parent);

        Canvas canvas = go.GetOrAddComponent<Canvas>();
        canvas.renderMode  = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;

		return Util.GetOrAddComponent<T>(go);
	}	
	
    public void Clear()
    {
        CloseAllPopupUI();
        if(gameSceneUI != null)
			gameSceneUI.gameObject.SetActive(false);
	    if (loginSceneUI != null)
			loginSceneUI.gameObject.SetActive(false);
	    // manageUI는 끄지 않는다...
    }
}
