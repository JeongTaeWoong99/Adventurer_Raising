using System;
using System.Collections.Generic;
using UnityEngine;

// S_ 패킷들 메인쓰레드에서 처리
// ★ using UnityEngine 작업들은 메인쓰레드에서만 처리 가능... 
public class DispatcherManagerEx
{
	private object _lock = new object();
	private Queue<Action> _actionQueue = new Queue<Action>();

	public void OnUpdate()
	{
		// 모든 대기 중인 작업들을 실행
		List<Action> actions = PopAll();
		foreach (Action action in actions)
		{
			action?.Invoke();
		}
	}

	// 액션을 큐에 추가 (PacketQueueManager.Push와 유사)
	public void Push(Action action)
	{
		lock (_lock)
		{
			_actionQueue.Enqueue(action);
		}
	}

	// 하나의 액션을 꺼내서 반환 (PacketQueueManager.Pop과 유사)
	public Action Pop()
	{
		lock (_lock)
		{
			if (_actionQueue.Count == 0)
				return null;

			return _actionQueue.Dequeue();
		}
	}

	// 모든 액션을 꺼내서 리스트로 반환 (PacketQueueManager.PopAll과 유사)
	public List<Action> PopAll()
	{
		List<Action> list = new List<Action>();
		lock (_lock)
		{
			while (_actionQueue.Count > 0)
				list.Add(_actionQueue.Dequeue());
		}
		return list;
	}

	// 삭제 작업을 위한 편의 메서드
	public void DestroyOnMainThread(GameObject go)
	{
		if (go == null) return;
		Push(() => { ClientManager.Resource.R_Destroy(go); });
	}
	
	public void Clear()
	{
		// 남은 작업을 할 필요가 없음...
		lock (_lock)
		{
			_actionQueue.Clear();
		}
	}
}