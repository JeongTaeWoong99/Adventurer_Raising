using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager
{
	[Header("키보드")]
	public Action KeyAction         = null; // 키보드 입력이 발생했을 때 실행될 이벤트

	[Header("마우스")]
	public Action<Define.MouseEvent> MouseMoveAction   = null; // 이동용 마우스
	bool  _movePressed     = false;
	float _movePressedTime = 0;
	
	public Action<Define.MouseEvent> MouseAttackAction = null; // 공격용 마우스
	bool  _attackPressed     = false;
	float _attackPressedTime = 0;
	
	// Manager의 Update에서 매 프레임마다 호출되어 입력을 처리하는 메서드
	public void OnUpdate()
	{
		// UI 요소 위에 마우스가 있는 경우 입력 무시
		if (EventSystem.current.IsPointerOverGameObject())
			return;
		
		// 공격 (오른쪽 클릭)
		if (MouseAttackAction != null)
		{
			if (Input.GetMouseButton(0))
			{
				if (!_attackPressed)
				{
					MouseAttackAction.Invoke(Define.MouseEvent.PointerDown);
					_attackPressedTime = Time.time;
				}
				MouseAttackAction.Invoke(Define.MouseEvent.Press);
				_attackPressed = true;
			}
			else
			{
				if (_attackPressed)
				{
					if (Time.time < _attackPressedTime + 0.2f)
						MouseAttackAction.Invoke(Define.MouseEvent.Click);
					MouseAttackAction.Invoke(Define.MouseEvent.PointerUp);
				}
				_attackPressed     = false;
				_attackPressedTime = 0;
			}
		}
		
		// 이동 (왼쪽 클릭)
		if (MouseMoveAction != null)
		{
			if (Input.GetMouseButton(1))
			{
				if (!_movePressed)
				{
					MouseMoveAction.Invoke(Define.MouseEvent.PointerDown);
					_movePressedTime = Time.time;
				}
				MouseMoveAction.Invoke(Define.MouseEvent.Press);
				_movePressed = true;
			}
			else
			{
				if (_movePressed)
				{
					if (Time.time < _movePressedTime + 0.2f)
						MouseMoveAction.Invoke(Define.MouseEvent.Click);
					MouseMoveAction.Invoke(Define.MouseEvent.PointerUp);
				}
				_movePressed     = false;
				_movePressedTime = 0;
			}
		}
		
		// 키보드 입력 처리
		if (KeyAction != null)
		{
			if (Input.anyKey)
			{
				KeyAction.Invoke();
			}
		}
	}

	// 모든 이벤트를 초기화하는 메서드
	public void Clear()
	{
		KeyAction         = null;
		MouseMoveAction   = null;
		MouseAttackAction = null;
	}
}