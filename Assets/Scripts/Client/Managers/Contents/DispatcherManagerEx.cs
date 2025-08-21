using System;
using System.Collections.Generic;
using UnityEngine;

// S_ 패킷들 메인쓰레드에서 처리
// ★ using UnityEngine 작업들은 메인쓰레드에서만 처리 가능... 
public class DispatcherManagerEx
{
	private object _lock = new object();
	private Queue<Action> _actionQueue = new Queue<Action>();
	
	// 지연 실행 스케줄러용 항목
	private class DelayedItem
	{
		public float dueTime;
		public Action action;
	}
	private readonly List<DelayedItem> _delayedActions = new List<DelayedItem>();

	public void OnUpdate()
	{
		// 1) 지연 작업 중 기한이 지난 것들을 메인 큐로 이동
		ProcessDelayedActions();

		// 2) 모든 대기 중인 작업들을 실행
		List<Action> actions = PopAll();
		foreach (Action action in actions)
		{
			action?.Invoke();
		}
	}

	private void ProcessDelayedActions()
	{
		float now = Time.time;
		List<Action> ready = null;
		lock (_lock)
		{
			if (_delayedActions.Count == 0)
				return;

			for (int i = _delayedActions.Count - 1; i >= 0; i--)
			{
				if (_delayedActions[i].dueTime <= now)
				{
					if (ready == null) ready = new List<Action>();
					ready.Add(_delayedActions[i].action);
					_delayedActions.RemoveAt(i);
				}
			}

			if (ready != null)
			{
				foreach (var a in ready)
					_actionQueue.Enqueue(a);
			}
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

	// 지연 실행: 초 단위
	public void PushDelayedSeconds(Action action, float delaySeconds)
	{
		if (action == null) return;
		float due = Time.time + Mathf.Max(0f, delaySeconds);
		lock (_lock)
		{
			_delayedActions.Add(new DelayedItem { dueTime = due, action = action });
		}
	}

	// 지연 실행: 밀리초 단위
	public void PushDelayedMilliseconds(Action action, float delayMilliseconds)
	{
		float seconds = delayMilliseconds / 1000f;
		PushDelayedSeconds(action, seconds);
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