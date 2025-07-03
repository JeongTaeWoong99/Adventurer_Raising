using System;
using System.Collections.Generic;
using UnityEngine;

// 모든 매니저 스크립트들을 만들어, 사용하는 총 관리자
// Core와 Contents가 포함되어 있음.
public class ClientManager : MonoBehaviour
{
    // 싱글톤
    private static ClientManager s_instance;
    public static ClientManager Instance { get { Init(); return s_instance; }}

    public bool isClientInit_Complete = false;
    
	[Header("Contents 매니저")]
	GameManagerEx       _game       = new GameManagerEx();
    DispatcherManagerEx _dispatcher = new DispatcherManagerEx();
    public static GameManagerEx       Game       { get { return Instance._game; } }
    public static DispatcherManagerEx Dispatcher { get { return Instance._dispatcher; } }

    [Header("Core 매니저")]
	DataManager         _data = new DataManager();
    InputManager       _input = new InputManager();
    PoolManager         _pool = new PoolManager();
    ResourceManager _resource = new ResourceManager();
    SceneManagerEx     _scene = new SceneManagerEx();
    SoundManager       _sound = new SoundManager();
    UIManager             _ui = new UIManager();

    public static DataManager     Data     { get { return Instance._data; } }
    public static InputManager    Input    { get { return Instance._input; } }
    public static PoolManager     Pool     { get { return Instance._pool; } }
    public static ResourceManager Resource { get { return Instance._resource; } }
    public static SceneManagerEx  Scene    { get { return Instance._scene; } }
    public static SoundManager    Sound    { get { return Instance._sound; } }
    public static UIManager       UI       { get { return Instance._ui; } }

    static void Init()
    {
        if (s_instance != null) 
            return;
        
        GameObject go = GameObject.Find("@ClientManager");
        if (go == null)
        {
            go = new GameObject { name = "@ClientManager" };
            go.AddComponent<ClientManager>();
        }

        DontDestroyOnLoad(go);
        s_instance = go.GetOrAddComponent<ClientManager>();
    }
    
    private void Awake()
    {
        Init();
        
        // 백그라운드 실행 설정 (멀티플레이어 테스트용)
        Application.runInBackground = true;      // 백그라운드에서도 실행 유지
        Application.targetFrameRate = 144;       // 목표 프레임률 고정 (포커스 상태)
        QualitySettings.vSyncCount  = 0;         // V-Sync 비활성화 (프레임률 제한 방지)
        
        s_instance._data.Init();
        s_instance._pool.Init();
        s_instance._sound.Init();
        
        isClientInit_Complete = true;
        //Debug.Log("isClientInit_Complete 완료");
    }
    
    // 포커스 상태에 따른 프레임률 조정
    private void OnApplicationFocus(bool hasFocus)
    {
        // 포커스가 있을 때: 144fps
        // 포커스가 없을 때: 30fps (멀티플레이어 테스트를 위해 완전히 멈추지 않음)
        Application.targetFrameRate = hasFocus ? 144 : 60;
    }
    
    private float fpsTimer = 0f;
    
    // 플레이어의 마우스와 키 입력을 매니저에서 관리.
    // 인풋 조건을 만족할 때에만, _input.OnUpdate();가 실행된다.
    private void Update()
    {
        _input.OnUpdate();
        
        // 모든 대기 중인 작업들을 실행
        List<Action> actions = Dispatcher.PopAll();
        foreach (Action action in actions)
        {
            action?.Invoke();
        }
        
        // FPS 디버그 (5초마다)
        fpsTimer += Time.deltaTime;
        if (fpsTimer >= 5f)
        {
            fpsTimer = 0f;
            float currentFPS = 1.0f / Time.deltaTime;
            //Debug.Log($"[FPS Debug] 실제 FPS: {currentFPS:F1} | 목표: {Application.targetFrameRate} | V-Sync: {QualitySettings.vSyncCount}");
        }
    }
    
    public static void Clear()
    {
        Input.Clear();
        Sound.Clear();
        Scene.Clear();
        UI.Clear();
        Pool.Clear();
        
        Dispatcher.Clear();
        Game.Clear();
    }
}
