using System;
using UnityEngine;
using UnityEngine.EventSystems;

// 편리하게 사용하기 위한, 메서드들 미리 만들어 두고 사용
public static class Extension
{
	// 컴포넌트 확인하거나, 새로 붙여주기
	public static T GetOrAddComponent<T>(this GameObject go) where T : Component
	{
		return Util.GetOrAddComponent<T>(go);
	}

	// UI 바인드
	public static void BindEvent(this GameObject go, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click)
	{
		UI_Base.BindEvent(go, action, type);
	}
	
	// 유효한 상태인지 체크
	public static bool IsValid(this GameObject go)
	{
		return go != null && go.activeSelf;
	}
	
	// 타입 찾기
	public static Define.WorldObject GetWorldObjectType(GameObject go)
	{
		BaseController bc = go.GetComponent<BaseController>();
		return bc == null ? Define.WorldObject.Unknown : bc.WorldObjectType;
	}
	
	public static Vector3 ParseVector3(string value)
	{
		var tokens = value.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
		if (tokens.Length != 3) 
			return Vector3.zero;
        
		return new Vector3(float.Parse(tokens[0].Trim()), float.Parse(tokens[1].Trim()), float.Parse(tokens[2].Trim()));
	}
}