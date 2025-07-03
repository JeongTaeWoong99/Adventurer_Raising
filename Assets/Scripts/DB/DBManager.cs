using System;
using System.IO;
using Firebase;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[System.Serializable]
public class GoogleServicesConfig
{
    public ProjectInfo project_info;
    public Client[] client;
    
    [System.Serializable]
    public class ProjectInfo
    {
        public string project_number;
        public string firebase_url;
        public string project_id;
        public string storage_bucket;
    }
    
    [System.Serializable]
    public class Client
    {
        public ClientInfo client_info;
        public ApiKey[] api_key;
        
        [System.Serializable]
        public class ClientInfo
        {
            public string mobilesdk_app_id;
        }
        
        [System.Serializable]
        public class ApiKey
        {
            public string current_key;
        }
    }
}

public class DBManager : MonoBehaviour
{
    // 싱글톤
    private static DBManager s_instance;
    public static DBManager Instance { get { Init(); return s_instance; }}
    
    AuthManager      _auth      = new AuthManager();
    RealTimeManager  _realTime  = new RealTimeManager();
    FirestoreManager _firestore = new FirestoreManager();
    
    public static AuthManager      Auth      { get { return Instance._auth; } }
    public static RealTimeManager  RealTime  { get { return Instance._realTime; } }
    public static FirestoreManager Firestore { get { return Instance._firestore; } }
    
    private FirebaseApp customApp; // 인스턴스 변경용
    private GoogleServicesConfig firebaseConfig; // google-services.json 데이터

    public bool isDB_Init_Complete = false; // 완료 확인용
    
    static void Init()
    {
        if (s_instance != null) 
            return;
        
        GameObject go = GameObject.Find("@DBManager");
        if (go == null)
        {
            go = new GameObject { name = "@DBManager" };
            go.AddComponent<DBManager>();
        }

        DontDestroyOnLoad(go);
        s_instance = go.GetOrAddComponent<DBManager>();
        
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // 초기화는 Start에서 customApp이 만들어지고 나서 진행...
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    }
    
    private void Awake()
    {
        Init();
    }

    private async void Start()
    {
        try
        {
            // 로그 레벨을 Warning으로 설정하여 Info 및 Debug 메시지 숨기기
            FirebaseApp.LogLevel = LogLevel.Warning;
    
            try
            {
                var status = await FirebaseApp.CheckAndFixDependenciesAsync();
                if (status != DependencyStatus.Available)
                {
                    Debug.LogError($"Firebase 초기화 실패: {status}");
                    return;
                }

                // 🔧 google-services.json 파일에서 설정 로드
                bool configLoaded = await LoadFirebaseConfig();
                if (!configLoaded)
                {
                    Debug.LogError("Firebase 설정을 로드할 수 없습니다!");
                    return;
                }
                
                var options = new AppOptions()
                {
                    ProjectId   = GetProjectId(),
                    AppId       = GetAppId(),
                    ApiKey      = GetApiKey(),
                    DatabaseUrl = new Uri(GetDatabaseUrl())
                };

                // 설정 검증
                if (string.IsNullOrEmpty(options.AppId) || string.IsNullOrEmpty(options.ApiKey))
                {
                    Debug.LogError("Firebase AppId 또는 ApiKey가 설정되지 않았습니다!");
                    return;
                }

                Debug.Log($"✅ Firebase 설정 로드 완료 - ProjectId: {options.ProjectId}");

#if UNITY_EDITOR
                string appName = "EditorApp";
#else
                string appName = "RuntimeApp";
#endif
                // App 인스턴스 분리 및 초기화 진행
                customApp = FirebaseApp.Create(options, appName);
                await s_instance._firestore.Init(customApp);  // 커스텀 O
            
                // 초기화 진행
                s_instance._auth.Init();                      
                s_instance._realTime.Init();                 

                // 모든 작업이 완료되면, true
                isDB_Init_Complete = true;
                Debug.Log("✅ Firebase 초기화 완료");
            }
            catch (Exception e)
            {   
                Debug.LogError($"❌ Firebase 초기화 실패: {e.Message}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ DBManager 초기화 실패: {e.Message}");
        }
    }
    
    private async System.Threading.Tasks.Task<bool> LoadFirebaseConfig()
    {
        try
        {
            string filePath = Path.Combine(Application.streamingAssetsPath, "google-services.json");
            
            Debug.Log($"🔍 Firebase 설정 파일 경로: {filePath}");
            
            string jsonContent = "";
            
            // 플랫폼별 파일 읽기
            if (filePath.Contains("://") || filePath.Contains(":///"))
            {
                // Android, WebGL 등에서는 UnityWebRequest 사용
                UnityWebRequest www = UnityWebRequest.Get(filePath);
                var operation = www.SendWebRequest();
                
                while (!operation.isDone)
                {
                    await System.Threading.Tasks.Task.Yield();
                }
                
                if (www.result == UnityWebRequest.Result.Success)
                {
                    jsonContent = www.downloadHandler.text;
                }
                else
                {
                    Debug.LogError($"파일 읽기 실패: {www.error}");
                    return false;
                }
            }
            else
            {
                // PC, Editor에서는 File.ReadAllText 사용
                if (File.Exists(filePath))
                {
                    jsonContent = File.ReadAllText(filePath);
                }
                else
                {
                    Debug.LogError($"파일이 존재하지 않습니다: {filePath}");
                    return false;
                }
            }
            
            // JSON 파싱
            firebaseConfig = JsonUtility.FromJson<GoogleServicesConfig>(jsonContent);
            
            if (firebaseConfig?.project_info != null && firebaseConfig?.client?.Length > 0)
            {
                Debug.Log("✅ google-services.json 파일 로드 성공");
                return true;
            }
            else
            {
                Debug.LogError("❌ google-services.json 파일 형식이 올바르지 않습니다");
                return false;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Firebase 설정 로드 실패: {e.Message}");
            return false;
        }
    }
    
    // 환경변수 우선, 없으면 google-services.json에서 가져오기
    private string GetProjectId()
    {
        string envValue = System.Environment.GetEnvironmentVariable("FIREBASE_PROJECT_ID");
        if (!string.IsNullOrEmpty(envValue)) return envValue;
        return firebaseConfig?.project_info?.project_id ?? "d-rpg-server";
    }
    
    private string GetAppId()
    {
        string envValue = System.Environment.GetEnvironmentVariable("FIREBASE_APP_ID");
        if (!string.IsNullOrEmpty(envValue)) return envValue;
        return firebaseConfig?.client?[0]?.client_info?.mobilesdk_app_id ?? "";
    }
    
    private string GetApiKey()
    {
        string envValue = System.Environment.GetEnvironmentVariable("FIREBASE_API_KEY");
        if (!string.IsNullOrEmpty(envValue)) return envValue;
        return firebaseConfig?.client?[0]?.api_key?[0]?.current_key ?? "";
    }
    
    private string GetDatabaseUrl()
    {
        string envValue = System.Environment.GetEnvironmentVariable("FIREBASE_DATABASE_URL");
        if (!string.IsNullOrEmpty(envValue)) return envValue;
        return firebaseConfig?.project_info?.firebase_url ?? "https://d-rpg-server.firebaseio.com";
    }
}
