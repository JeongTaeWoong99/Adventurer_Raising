using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class BaseScene : MonoBehaviour
{
	[HideInInspector] public Define.SceneType SceneType { get; protected set; } = Define.SceneType.Unknown;
    
	// [HideInInspector] public UI_Scene loginSceneUI; // 로그인 씬 UI
	// [HideInInspector] public UI_Scene gameSceneUI;  // 게임씬 씬 UI
	
	void Awake()
	{
		Init();
	}

	protected virtual void Init()
    {
		
    }

    public abstract void Clear();
}
