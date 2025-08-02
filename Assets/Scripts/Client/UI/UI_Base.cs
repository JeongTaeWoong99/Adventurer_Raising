using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class UI_Base : MonoBehaviour
{
	// UI 요소들을 저장하는 딕셔너리(각 UI_Base의 Objects 딕셔너리에 Bind를 때, 저장 되었다가, Get으로 찾아서 사용.)
	protected Dictionary<Type, UnityEngine.Object[]> Objects = new Dictionary<Type, UnityEngine.Object[]>();
	
	// UI 초기화를 위한 추상 메서드
	public abstract void Init();
	
	// 자주 사용하는 UI 컴포넌트 접근 메서드들
	protected GameObject      GetObject(int idx) { return Get<GameObject>(idx); }
	protected TextMeshProUGUI GetText(int idx)   { return Get<TextMeshProUGUI>(idx); }
	protected TMP_InputField  GetInputField(int idx) { return Get<TMP_InputField>(idx); }
	protected Button          GetButton(int idx) { return Get<Button>(idx); }
	protected Image           GetImage(int idx)  { return Get<Image>(idx); }
	protected Slider          GetSlider(int idx) { return Get<Slider>(idx); }

	private void Start()
	{
		Init();
	}

	// UI 요소들을 자동으로 찾아서 딕셔너리에 저장하는 메서드
	protected void Bind<T>(Type type) where T : UnityEngine.Object
	{
		string[] names = Enum.GetNames(type);								 // Enum 타입의 모든 이름을 가져옴
		UnityEngine.Object[] objects = new UnityEngine.Object[names.Length]; // 해당 타입의 객체들을 저장할 배열 생성
		Objects.Add(typeof(T), objects);									 // 딕셔너리에 타입과 배열을 추가
		
		// Enum의 모든 이름에 대해 반복
		for (int i = 0; i < names.Length; i++)
		{
			// GameObject 타입인 경우와 그 외의 경우를 구분하여 처리
			if (typeof(T) == typeof(GameObject))
				objects[i] = Util.FindChild(gameObject, names[i], true);
			else
				objects[i] = Util.FindChild<T>(gameObject, names[i], true);

			// 찾지 못한 경우 로그 출력
			if (objects[i] == null)
				Debug.LogError($"UI 바인드 실패 - 타입: {typeof(T).Name}, 이름: {names[i]}, 부모: {gameObject.name}");
		}
	}

	// 바인딩된 UI 요소를 가져오는 메서드
	protected T Get<T>(int idx) where T : UnityEngine.Object
	{
		UnityEngine.Object[] objects = null;
		if (Objects.TryGetValue(typeof(T), out objects) == false)
			return null;

		return objects[idx] as T;
	}
	
	// UI 요소에 이벤트를 바인딩하는 메서드
	public static void BindEvent(GameObject go, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click)
	{
		UI_EventHandler evt = Util.GetOrAddComponent<UI_EventHandler>(go);

		switch (type)
		{
			case Define.UIEvent.Click:
				evt.OnClickHandler -= action;
				evt.OnClickHandler += action;
				break;
			case Define.UIEvent.Drag:
				evt.OnDragHandler -= action;
				evt.OnDragHandler += action;
				break;
		}
	}
}
