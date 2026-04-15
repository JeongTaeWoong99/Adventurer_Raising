using System.Collections;
using System.Diagnostics;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class LoginScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        SceneType = Define.SceneType.Login;
        
        StartCoroutine(WaitForAsync());
    }
    
    // ── 매니저 생성 순서 및 '@' 오브젝트 규칙 ─────────────────────────────────────
    // 생성 순서: DB → Client → Network
    //
    // '@' 접두사(예: @DBManager, @ClientManager, @NetworkManager)는
    // 인스펙터에 미리 배치된 오브젝트가 아니라, 각 매니저의 Init()이 처음 호출될 때
    // 코드로 직접 생성되는 오브젝트를 의미합니다.
    // → 씬 어디에도 해당 컴포넌트가 붙어있지 않아야 정상 동작합니다.
    // ───────────────────────────────────────────────────────────────────────────
    private IEnumerator WaitForAsync()
    {
        // 게임씬에서 넘어오는 경우, 이전 연결이 끊긴 @NetworkManager가 남아 있으므로 제거.
        NetworkManager existingNetworkManager = NetworkManager.GetExistingInstance();
        if (existingNetworkManager != null) Destroy(existingNetworkManager.gameObject);
        
        // DBManager 생성
        DBManager db = DBManager.Instance;
        
        yield return new WaitUntil(() => db.isDB_Init_Complete);        // 모든 비동기 다운로드 작업이 완료될 때까지 기다리기....
        
        // ClientManager 생성
        ClientManager client = ClientManager.Instance;
        
        yield return new WaitUntil(() => client.isClientInit_Complete); // 모든 비동기 다운로드 작업이 완료될 때까지 기다리기....
        
        // manageUI 없으면, 생성
        if (ClientManager.UI.manageUI == null)
            ClientManager.UI.ShowSceneUI<UI_Manage>("UI_Manage");
        yield return new WaitUntil(() => ClientManager.UI.manageUI); // 모든 비동기 다운로드 작업이 완료될 때까지 기다리기....
        ClientManager.UI.manageUI.LoadingPanelObject.SetActive(true); // manageUI 로딩 켜기
        
        // loginSceneUI 없으면, 생성
        if (ClientManager.UI.loginSceneUI == null)
            ClientManager.UI.ShowSceneUI<UI_LoginScene>("UI_LoginScene");
        yield return new WaitUntil(() => ClientManager.UI.loginSceneUI); // 모든 비동기 다운로드 작업이 완료될 때까지 기다리기....
        ClientManager.UI.loginSceneUI.gameObject.SetActive(true);
        
        gameObject.GetOrAddComponent<CursorController>();                                                                   // 커서 변경
        ClientManager.Sound.Play($"Sounds/BGM/{SceneManager.GetActiveScene().name}", Define.Sound.Bgm, 0.5f); // 씬 전용 BGM 재생
        
#if UNITY_EDITOR
        ClientManager.UI.loginSceneUI.EmailPlaceholderText.text    = "admin" + 123 + "@naver.com";
        ClientManager.UI.loginSceneUI.PasswordPlaceholderText.text = "admin" + 123;
        
        ClientManager.UI.loginSceneUI.MakeEmailPlaceholder.text    = "admin" + 123 + "@naver.com";
        ClientManager.UI.loginSceneUI.MakePasswordPlaceholder.text = "admin" + 123;     
#else
        ClientManager.UI.loginSceneUI.EmailPlaceholderText.text    = "admin" + 456 + "@naver.com";
        ClientManager.UI.loginSceneUI.PasswordPlaceholderText.text = "admin" + 456;
        
        ClientManager.UI.loginSceneUI.MakeEmailPlaceholder.text    = "admin" + 456 + "@naver.com";
        ClientManager.UI.loginSceneUI.MakePasswordPlaceholder.text = "admin" + 456;     
#endif 
        // NetworkManager 생성
        NetworkManager network = NetworkManager.Instance;
        
        yield return new WaitUntil(() => network.isNetworkInit_Complete); // 모든 비동기 다운로드 작업이 완료될 때까지 기다리기....
        
        ClientManager.UI.loginSceneUI.SceneLoadedSetting(); // 네트워크가 세팅되면, 화면 전환
    }
    public override void Clear()
    {   
        // Debug.Log("LoginScene 클리어!");
    }
    
    #region 게임 실행 전 작업
    
    private static Mutex mutex;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void DupleExeCheck()
    {
        bool createdNew = false;
        mutex = new Mutex(true, "MyUniqueGameMutexName", out createdNew);
    
        if (!createdNew)
        {
            // 다른 인스턴스가 정말 실행 중인지 체크
            string thisProcessName = Process.GetCurrentProcess().ProcessName;
            Process[] processes    = Process.GetProcessesByName(thisProcessName);
    
            if (processes.Length > 1)
            {
                Debug.LogWarning("이미 실행 중인 인스턴스가 감지되어 종료합니다.");
                Application.Quit();
            }
            else
            {
                Debug.LogWarning("Mutex 충돌이 감지되었지만, 실행 중인 다른 프로세스는 없음. 예외로 무시하고 진행.");
            }
        }
    }
    
    private void OnApplicationQuit()
    {
        mutex?.ReleaseMutex();
        mutex?.Dispose();
    }

    #endregion
}
