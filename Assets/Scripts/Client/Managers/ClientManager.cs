using System;
using System.Collections.Generic;
using Client.Managers.Contents;
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
    ToolTipManager      _toolTip    = new ToolTipManager();
    FPSManager          _fps        = new FPSManager();
    public static GameManagerEx       Game       { get { return Instance._game; } }
    public static DispatcherManagerEx Dispatcher { get { return Instance._dispatcher; } }
    public static ToolTipManager      ToolTip    { get { return Instance._toolTip; } }
    public static FPSManager          FPS        { get { return Instance._fps; } }
    
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
        
        // 코어
        s_instance._data.Init();
        s_instance._pool.Init();
        s_instance._sound.Init();
        
        // 컨텐츠
        s_instance._toolTip.Init();
        s_instance._fps.Init();
        
        // 완료
        isClientInit_Complete = true;
        //Debug.Log("isClientInit_Complete 완료");
    }
    
    private void Update()
    {
        // 코어
        _input.OnUpdate();
        _ui.OnUpdate();
        
        // 컨텐츠
        _toolTip.OnUpdate();
        _fps.OnUpdate();
        _dispatcher.OnUpdate();
    }
    
    public static void Clear()
    {
        // 코어
        Input.Clear();
        Sound.Clear();
        Scene.Clear();
        UI.Clear();
        Pool.Clear();
        
        // 컨텐츠
        Dispatcher.Clear();
        Game.Clear();
        FPS.Clear();
    }
    
    // 포커스 상태에 따른 프레임률 조정(내장 메서드)
    private void OnApplicationFocus(bool hasFocus)
    {
        // 포커스가 있을 때: 144fps
        // 포커스가 없을 때: 30fps (멀티플레이어 테스트를 위해 완전히 멈추지 않음)
        Application.targetFrameRate = hasFocus ? 144 : 60;
        Debug.Log("OnApplicationFocus => " + Application.targetFrameRate);
    }
}
