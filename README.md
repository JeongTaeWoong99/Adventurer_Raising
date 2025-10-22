# 모험가 키우기 - 클라이언트 리포지토리

## 📑 목차
- [📋 개요](#-개요)
- [🎬 인게임 사진](#-인게임-사진)
- [🔗 관련 링크](#-관련-링크)
- [🔧 시스템 구조도](#-시스템-구조도)
- [✨ 주요 기능](#-주요-기능)
- [📂 프로젝트 구조](#-프로젝트-구조)
- [🛠 기술 스택](#-기술-스택)
- [🏗 아키텍처](#-아키텍처)
- [🚀 성능 특성](#-성능-특성)

## 📋 개요

| 항목 | 내용 |
|------|------|
| 기간 | 2025.05 ~ 2025.08 |
| 인원 | 1인 개발 |
| 역할 | 클라이언트, 서버, DB |
| 도구 | UNITY, C#, TCP SOCKET, FIREBASE |
| 타겟 기기 | PC |

판타지 배경의 쿼터뷰 3D 멀티 RPG 게임입니다. 

데디케이트 서버를 구축하여, 멀티플레이가 가능합니다.

3D 게임 개발 및 네트워크 시스템에 대한 전반적인 이해를 목적으로, 클라이언트부터 서버와 DB까지 직접 설계하고 구현했습니다.

## 🎬 인게임 사진

<table>
  <tr>
    <td align="center">
      <img width="640" height="360" alt="로그인 화면" src="https://github.com/user-attachments/assets/eae1f5d1-66f9-45ab-9c95-a243a4f3d9ee" />
      <br/>
      <b>클라이언트 로그인 화면</b>
    </td>
    <td align="center">
      <img width="640" height="360" alt="인게임 전투 화면" src="https://github.com/user-attachments/assets/f667d625-3be4-4b9f-8a9b-3d58ff999d94" />
      <br/>
      <b>클라이언트 인게임 전투 화면</b>
    </td>
  </tr>
  <tr>
    <td align="center">
      <img width="640" height="360" alt="C# 데디케이트 서버" src="https://github.com/user-attachments/assets/77bc055d-b986-42cb-89b3-5b50de9f131a" />
      <br/>
      <b>C# 데디케이트 서버</b>
    </td>
    <td align="center">
      <img width="640" height="360" alt="FIREBASE DB" src="https://github.com/user-attachments/assets/b238cf89-7f3f-4615-bd19-860c3556ad9e" />
      <br/>
      <b>FIREBASE DB</b>
    </td>
  </tr>
</table>

## 🔗 관련 링크

| 항목 | 링크                                                                   |
|------|----------------------------------------------------------------------|
| 시연 영상 | [바로가기](https://www.youtube.com/watch?v=bL4QaUiaqw4&feature=youtu.be) |
| 서버 GitHub | [바로가기](https://github.com/JeongTaeWoong99/Adventurer_Raising_Server) |

## 🔧 시스템 구조도

<p align="center">
  <img width="1056" height="501" alt="image" src="https://github.com/user-attachments/assets/717bd5d7-3ba2-43a3-a8f7-1881c426f9b8" />
</p>

## ✨ 주요 기능

### 실시간 멀티플레이어
- **TCP 소켓 통신** : 서버와 실시간 양방향 통신
- **이중 큐 시스템** : 네트워크 스레드와 메인 스레드 간 안전한 패킷 처리
- **Dispatcher 패턴** : Unity 메인 스레드 제약 해결
- **동기화 전략** : 주기적 위치 업데이트 + 즉각 액션 패킷

### 패킷 통신 시스템
- **패킷 구조** : `[2바이트:크기][2바이트:패킷ID][N바이트:데이터]`
- **서버와 동일한 패킷 스펙** : 자동 생성 시스템으로 일관성 보장
- **비동기 소켓** : `SocketAsyncEventArgs` 기반 고성능 I/O
- **패킷 배칭** : 송신 큐로 여러 패킷 묶어서 전송

### 게임 시스템
- **전투 시스템** : 콤보 공격, QWER 스킬, 히트 판정
- **캐릭터 시스템** : 레벨, 경험치, 스탯 관리
- **UI 시스템** : Scene/Popup/SubItem/WorldSpace UI 계층 구조
- **엔티티 관리** : 플레이어, 몬스터, 오브젝트 Dictionary 기반 관리
- **채팅 시스템** : 실시간 채팅 메시지 브로드캐스트

### 인증 및 데이터 관리
- **Firebase Authentication** : 계정 생성 및 로그인
- **Firebase Firestore** : 플레이어 데이터 영구 저장
- **클라이언트 캐싱** : JSON 기반 게임 데이터 관리


## 📂 프로젝트 구조

```
Assets/Scripts/
├── Client/                         # 클라이언트 게임 로직
│   ├── Managers/
│   │   ├── ClientManager.cs        # 중앙 매니저 허브 (싱글톤)
│   │   ├── Core/                   # 핵심 매니저들
│   │   │   ├── DataManager.cs      # JSON 데이터 파싱 및 관리
│   │   │   ├── InputManager.cs     # 이벤트 기반 입력 처리
│   │   │   ├── PoolManager.cs      # 오브젝트 풀링
│   │   │   ├── ResourceManager.cs  # 리소스 로드/인스턴스화
│   │   │   ├── SceneManagerEx.cs   # 씬 전환 관리
│   │   │   ├── SoundManager.cs     # 사운드 재생
│   │   │   └── UIManager.cs        # UI 생성 및 관리
│   │   └── Contents/               # 게임 컨텐츠 매니저들
│   │       ├── GameManagerEx.cs    # 게임 상태 관리
│   │       ├── DispatcherManagerEx.cs  # 스레드 안전 액션 큐
│   │       ├── ToolTipManager.cs   # 툴팁 표시
│   │       └── FPSManager.cs       # 프레임 모니터링
│   │
│   ├── Character Controllers/      # 엔티티 컨트롤러 계층
│   │   ├── BaseController.cs       # 추상 기반 클래스
│   │   ├── CommonPlayerController.cs  # 다른 플레이어 (서버 동기화)
│   │   ├── MyPlayerController.cs   # 로컬 플레이어 (입력 처리)
│   │   ├── MonsterController.cs    # 몬스터 AI
│   │   └── ObjectController.cs     # 상호작용 오브젝트
│   │
│   ├── Contents/
│   │   ├── InfoState/              # 엔티티 상태 관리
│   │   │   ├── InfoState.cs        # 공통 상태 (HP, 레벨, 경험치)
│   │   │   ├── PlayerInfoState.cs  # 플레이어 전용 상태
│   │   │   └── ObjectAndMonsterInfoState.cs  # 오브젝트/몬스터 상태
│   │   ├── Effects/
│   │   │   └── SkillMove.cs        # 스킬 이펙트 이동 로직
│   │   └── Object/
│   │       └── Portal.cs           # 포털 전환
│   │
│   ├── UI/                         # UI 시스템
│   │   ├── UI_Base.cs              # UI 기반 클래스
│   │   ├── UI_EventHandler.cs      # UI 이벤트 처리
│   │   ├── Scene/                  # 씬 UI (HUD, 채팅, 스킬바)
│   │   ├── Popup/                  # 팝업 UI (인벤토리, 설정)
│   │   ├── SubItem/                # 동적 아이템 (채팅 메시지, 슬롯)
│   │   └── WorldSpace/             # 3D UI (체력바, 데미지 텍스트)
│   │
│   ├── Scenes/                     # 씬 관리
│   │   ├── BaseScene.cs            # 씬 기반 클래스
│   │   ├── Content/
│   │   │   ├── LoginScene.cs       # 로그인 씬
│   │   │   └── GameScene.cs        # 게임 씬
│   │
│   ├── Fuction Controllers/
│   │   ├── CameraController.cs     # 카메라 추적
│   │   └── CursorController.cs     # 커서 표시
│   │
│   ├── Data/
│   │   └── Data.Contents.cs        # 데이터 클래스 정의
│   │
│   └── Utils/                      # 유틸리티
│       ├── Define.cs               # Enum 정의
│       ├── Extension.cs            # 확장 메서드
│       └── Util.cs                 # 헬퍼 함수
│
├── Server/                         # 네트워크 통신 레이어
│   ├── NetworkManager/
│   │   ├── NetworkManager.cs       # 네트워크 매니저 허브
│   │   ├── PacketQueueManager.cs   # 패킷 큐 (스레드 안전)
│   │   ├── ManagementPP.cs         # 엔티티 생명주기 패킷 처리
│   │   ├── OperationPP.cs          # 조작 패킷 처리
│   │   ├── DataBasePP.cs           # DB 패킷 처리
│   │   └── NetworkStatsPP.cs       # 네트워크 통계
│   │
│   ├── Packet/
│   │   ├── PacketHandler.cs        # 패킷 핸들러 등록
│   │   ├── ClientPacketManager.cs  # 패킷 매니저
│   │   └── GenPackets.cs           # 자동 생성 패킷 클래스
│   │
│   ├── ServerCore/                 # 소켓 통신 코어
│   │   ├── Session.cs              # 비동기 소켓 세션
│   │   ├── Connector.cs            # 서버 연결
│   │   ├── RecvBuffer.cs           # 수신 버퍼
│   │   └── SendBuffer.cs           # 송신 버퍼
│   │
│   └── Session/
│       └── ServerSession.cs        # 서버 세션 구현
│
└── DB/                             # Firebase 연동
    ├── AuthManager.cs              # 인증 관리
    ├── DBManager.cs                # DB 초기화
    ├── FirestoreManager.cs         # Firestore 읽기/쓰기
    └── RealTimeManager.cs          # 실시간 DB
```

## 🛠 기술 스택

### 클라이언트
- **Unity 6000.0.44f1** - 3D 게임 엔진
- **C# (.NET Framework)** - 프로그래밍 언어
- **Unity Addressables** - 리소스 관리 시스템
- **Unity UI (uGUI)** - UI 시스템

### 네트워킹
- **TCP Socket** - 실시간 통신 프로토콜
- **SocketAsyncEventArgs** - 비동기 소켓 I/O
- **Custom Binary Protocol** - 자체 제작 패킷 직렬화

### 데이터베이스
- **Firebase Authentication** - 계정 인증
- **Firebase Firestore** - NoSQL 플레이어 데이터 저장
- **Firebase Realtime Database** - 실시간 데이터 동기화

### 디자인 패턴
- **Singleton Pattern** - 매니저 클래스
- **Factory Pattern** - UI 생성
- **Observer Pattern** - 이벤트 시스템
- **Object Pooling** - 성능 최적화
- **Dispatcher Pattern** - 스레드 안전성

## 🏗 아키텍처

### 1️⃣ 패킷 처리 흐름

클라이언트는 **이중 큐 시스템**으로 네트워크 스레드와 Unity 메인 스레드를 분리합니다.

```
[네트워크 스레드 - 백그라운드]
Socket.ReceiveAsync()
  ↓
Session.OnRecvCompleted()
  ↓
PacketSession.OnRecv()               # 패킷 헤더 파싱 [크기][ID]
  ↓
ServerSession.OnRecvPacket()
  ↓
PacketQueueManager.Push(packet)      # 🔒 Lock으로 큐에 저장

──────────────────────────────────────────────

[Unity 메인 스레드 - Update Loop]

NetworkManager.Update()
  ↓
PacketQueueManager.PopAll()          # 🔒 Lock으로 큐에서 꺼냄
  ↓
PacketManager.HandlePacket()
  ↓
PacketHandler.S_*Handler()           # 패킷 타입별 핸들러 호출
  ↓
Dispatcher.Push(lambda)              # Unity API 호출 액션 큐잉

ClientManager.Update()
  ↓
Dispatcher.OnUpdate()                # 큐잉된 액션 실행
  ↓
NetworkManager.Operation.EntityMove() # 실제 게임 로직 실행
```

**핵심 설계** :
1. **PacketQueueManager** : 네트워크 스레드에서 수신한 패킷을 큐에 저장
2. **Dispatcher Pattern** : Unity API는 메인 스레드에서만 호출 가능하므로 액션 큐로 마샬링
3. **Thread Safety** : Lock 기반 동기화로 데이터 레이스 방지

### 2️⃣ 핵심 디자인 패턴

#### 이중 매니저 구조

클라이언트는 **ClientManager**와 **NetworkManager**로 관심사를 분리합니다.

**ClientManager** - Unity 게임 로직
```csharp
public class ClientManager : MonoBehaviour
{
    // Core 매니저
    DataManager     _data;      // JSON 데이터 관리
    InputManager    _input;     // 입력 이벤트
    PoolManager     _pool;      // 오브젝트 풀링
    ResourceManager _resource;  // 리소스 로드
    SceneManagerEx  _scene;     // 씬 전환
    SoundManager    _sound;     // 사운드 재생
    UIManager       _ui;        // UI 생성/관리

    // Contents 매니저
    GameManagerEx       _game;       // 게임 상태
    DispatcherManagerEx _dispatcher; // 스레드 안전 큐
    ToolTipManager      _toolTip;    // 툴팁
    FPSManager          _fps;        // FPS 모니터

    void Update()
    {
        _input.OnUpdate();
        _ui.OnUpdate();
        _toolTip.OnUpdate();
        _fps.OnUpdate();
        _dispatcher.OnUpdate();  // 네트워크 액션 실행
    }
}
```

**NetworkManager** - 네트워크 통신
```csharp
public class NetworkManager : MonoBehaviour
{
    ServerSession _session;  // TCP 연결

    // 패킷 처리 모듈 (PP = Packet Processing)
    ManagementPP   _managementPP;    // 엔티티 입장/퇴장/정보변경
    OperationPP    _operationPP;     // 이동/공격/스킬/채팅
    DataBasePP     _dataBasePP;      // 로그인/회원가입
    NetworkStatsPP _networkStatsPP;  // 핑/패킷수/바이트

    void Update()
    {
        // 패킷 큐에서 꺼내서 처리
        List<IPacket> list = PacketQueueManager.Instance.PopAll();
        foreach(IPacket packet in list)
            PacketManager.Instance.HandlePacket(_session, packet);

        _networkStatsPP.UpdateStats();
    }
}
```

**분리 이유** :
- ClientManager : Unity 게임 로직 (리소스, UI, 씬, 사운드)
- NetworkManager : 네트워크 I/O 및 패킷 처리
- 장점: 모듈화, 테스트 용이, 명확한 책임 분리

#### Dispatcher 패턴 (스레드 안전성)

Unity API는 메인 스레드에서만 호출 가능하므로, 네트워크 스레드에서 받은 패킷을 메인 스레드로 마샬링합니다.

```csharp
public class DispatcherManagerEx
{
    Queue<Action> _actionQueue = new Queue<Action>();
    object _lock = new object();

    // 네트워크 스레드에서 호출
    public void Push(Action action)
    {
        lock (_lock)
        {
            _actionQueue.Enqueue(action);
        }
    }

    // 메인 스레드 (Update)에서 호출
    public void OnUpdate()
    {
        lock (_lock)
        {
            while (_actionQueue.Count > 0)
            {
                Action action = _actionQueue.Dequeue();
                action.Invoke();  // Unity API 호출
            }
        }
    }
}
```

**사용 예시** :
```csharp
// PacketHandler.cs (네트워크 스레드에서 실행)
public static void S_BroadcastEntityMoveHandler(PacketSession session, IPacket packet)
{
    S_BroadcastEntityMove pkt = packet as S_BroadcastEntityMove;

    // Unity API 호출을 메인 스레드로 마샬링
    ClientManager.Dispatcher.Push(() => {
        NetworkManager.Operation.EntityMove(pkt);
    });
}
```

#### 캐릭터 컨트롤러 계층 구조

```
BaseController (Abstract)
├── CommonPlayerController (다른 플레이어)
│   └── MyPlayerController (내 플레이어)
├── MonsterController (몬스터)
└── ObjectController (상호작용 오브젝트)
```

**BaseController.cs** - 모든 엔티티의 공통 로직
```csharp
public abstract class BaseController : MonoBehaviour
{
    // 공통 컴포넌트
    public Animator animator;
    public CharacterController characterController;
    public InfoState infoState;

    // 공통 상태
    public Define.Anime anime;
    public Vector3 movePos;
    public int currentAttackComboNum;

    // 애니메이션 상태 머신
    public Define.Anime Anime
    {
        set
        {
            anime = value;
            switch (anime)
            {
                case Define.Anime.Attack:
                    isAnimeMove = true;
                    animator.CrossFade("ATTACK" + currentAttackComboNum, 0.1f, -1, 0f);
                    break;
                // ...
            }
        }
    }

    void Update()
    {
        switch (Anime)
        {
            case Define.Anime.Idle:   UpdateIdle();   break;
            case Define.Anime.Run:    UpdateRun();    break;
            case Define.Anime.Attack: UpdateAttack(); break;
            // ...
        }

        // 중력 적용
        if (characterController) GroundGravityDirection();
    }

    // 하위 클래스에서 구현
    protected abstract void Init();
    protected virtual void UpdateIdle() { }
    protected virtual void UpdateRun() { }
    protected virtual void UpdateAttack() { }
}
```

**MyPlayerController.cs** - 로컬 플레이어 (입력 처리 + 패킷 전송)
```csharp
public class MyPlayerController : CommonPlayerController
{
    protected override void Init()
    {
        // 입력 이벤트 등록
        ClientManager.Input.MouseMoveAction -= OnMouseMoveEvent;
        ClientManager.Input.MouseMoveAction += OnMouseMoveEvent;
        ClientManager.Input.KeyAction -= OnKeyEvent;
        ClientManager.Input.KeyAction += OnKeyEvent;

        // 주기적 위치 동기화 (100ms)
        StartCoroutine(CoSendPacket());
    }

    // 우클릭 이동
    void OnMouseMoveEvent(Define.MouseEvent evt)
    {
        if (evt == Define.MouseEvent.Click)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100.0f, groundLayer))
            {
                movePos = hit.point;
                Anime = Define.Anime.Run;

                // 서버에 이동 패킷 전송
                C_EntityMove movePkt = new C_EntityMove();
                movePkt.posInfo = new PositionInfo { ... };
                NetworkManager.Instance.Send(movePkt.Write());
            }
        }
    }

    // 좌클릭 콤보 공격
    void OnMouseAttackEvent(Define.MouseEvent evt)
    {
        if (evt == Define.MouseEvent.Click && !isAttacking)
        {
            StartCoroutine(NormalAttackRoutine());
        }
    }

    IEnumerator NormalAttackRoutine()
    {
        // 공격 애니메이션 패킷
        C_EntityAttackAnimation attackPkt = new C_EntityAttackAnimation();
        attackPkt.comboNum = currentAttackComboNum;
        NetworkManager.Instance.Send(attackPkt.Write());

        yield return new WaitForSeconds(0.5f);
        currentAttackComboNum = (currentAttackComboNum % maxAttackComboNum) + 1;
    }
}
```

**CommonPlayerController.cs** - 다른 플레이어 (서버 동기화)
```csharp
public class CommonPlayerController : BaseController
{
    protected override void UpdateRun()
    {
        // 서버에서 받은 dir 벡터로 이동
        characterController.Move(dir * infoState.moveSpeed * Time.deltaTime);

        // 부드러운 회전
        if (dir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }
    }
}
```

#### UI 시스템 계층 구조

```
UI_Base (추상 클래스)
├── UI_Scene      # 씬 UI (HUD, 채팅, 스킬바)
├── UI_Popup      # 팝업 UI (인벤토리, 설정)
├── UI_SubItem    # 동적 아이템 (채팅 메시지, 슬롯)
└── UI_WorldSpace # 3D UI (체력바, 데미지 텍스트)
```

**UIManager.cs** - Factory 패턴
```csharp
public class UIManager
{
    int _order = 10;
    Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();

    public T ShowPopupUI<T>(string name = null) where T : UI_Popup
    {
        // 프리팹에서 UI 생성
        GameObject go = ClientManager.Resource.R_Instantiate($"UI/Popup/{name}");
        T popup = Util.GetOrAddComponent<T>(go);

        // 스택에 추가 및 sortingOrder 증가
        _popupStack.Push(popup);
        popup.GetComponent<Canvas>().sortingOrder = _order++;

        return popup;
    }
}
```

**UI_Base.cs** - Enum 기반 자동 바인딩
```csharp
public abstract class UI_Base : MonoBehaviour
{
    Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();

    protected void Bind<T>(Type type) where T : UnityEngine.Object
    {
        string[] names = Enum.GetNames(type);
        UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];

        for (int i = 0; i < names.Length; i++)
        {
            objects[i] = Util.FindChild<T>(gameObject, names[i], true);
        }

        _objects.Add(typeof(T), objects);
    }

    protected T Get<T>(int idx) where T : UnityEngine.Object
    {
        return _objects[typeof(T)][idx] as T;
    }
}
```

**사용 예시** :
```csharp
public class UI_GameScene : UI_Scene
{
    enum Texts { PlayerName, Level, HP, EXP }
    enum Buttons { SkillQ, SkillW, SkillE, SkillR }

    public override void Init()
    {
        Bind<Text>(typeof(Texts));
        Bind<Button>(typeof(Buttons));

        Get<Text>((int)Texts.PlayerName).text = "Player1";
        Get<Button>((int)Buttons.SkillQ).onClick.AddListener(OnSkillQ);
    }
}
```

#### 오브젝트 풀링

**PoolManager.cs**
```csharp
public class PoolManager
{
    class Pool
    {
        public GameObject Original { get; private set; }
        public Stack<GameObject> _poolStack = new Stack<GameObject>();

        public void Push(GameObject go)
        {
            go.SetActive(false);
            _poolStack.Push(go);
        }

        public GameObject Pop(Transform parent)
        {
            GameObject go;
            if (_poolStack.Count > 0)
                go = _poolStack.Pop();
            else
                go = GameObject.Instantiate(Original, parent);

            go.SetActive(true);
            return go;
        }
    }

    Dictionary<string, Pool> _pools = new Dictionary<string, Pool>();
}
```

**ResourceManager.cs 통합**
```csharp
public GameObject R_Instantiate(string path, Transform parent = null)
{
    GameObject original = R_Load<GameObject>(path);

    // Poolable 컴포넌트가 있으면 풀에서 가져옴
    if (original.GetComponent<Poolable>() != null)
        return ClientManager.Pool.Pop(original, parent);

    return GameObject.Instantiate(original, parent);
}

public void R_Destroy(GameObject go)
{
    if (go.GetComponent<Poolable>() != null)
    {
        ClientManager.Pool.Push(go);  // 풀로 반환
        return;
    }

    GameObject.Destroy(go);
}
```

### 3️⃣ 네트워크 동기화

#### 패킷 카테고리

**Database (DB)**
- `C_MakeId` / `S_MakeIdResult` - 회원가입
- `C_Login` / `S_LoginResult` - 로그인

**Management (관리)**
- `S_BroadcastEntityList` - 초기 동기화 (씬 진입 시 모든 엔티티)
- `S_BroadcastEntityEnter` - 새 엔티티 입장
- `S_BroadcastEntityLeave` - 엔티티 퇴장
- `S_BroadcastEntityInfoChange` - HP/레벨/경험치 업데이트

**Operation (조작)**
- `C_EntityMove` / `S_BroadcastEntityMove` - 이동
- `C_EntityRotation` / `S_BroadcastEntityRotation` - 회전
- `C_EntityAnimation` / `S_BroadcastEntityAnimation` - 애니메이션
- `C_EntityDash` / `S_BroadcastEntityDash` - 대쉬
- `C_EntityAttackAnimation` / `S_BroadcastEntityAttackAnimation` - 공격 애니메이션
- `S_BroadcastEntityAttackResult` - 공격 결과 (데미지, 히트)
- `S_BroadcastEntityAttackEffectCreate` - 스킬 이펙트 생성
- `C_Chatting` / `S_BroadcastChatting` - 채팅

#### 동기화 전략

**서버 권한 (Server Authority)**
- 엔티티 위치 (몬스터, 다른 플레이어)
- 체력, 데미지, 전투 결과
- 엔티티 스폰/디스폰

**클라이언트 권한 (Client Authority)**
- 로컬 플레이어 입력 및 예측
- 애니메이션 재생 (시각 효과)
- 이펙트 생성 (시각 효과)

**업데이트 빈도**
- **위치** : 100ms (10 Hz) - 주기적 업데이트
- **애니메이션 상태** : 변화 시에만 - 대역폭 절약
- **전투 액션** : 즉각 (0ms 지연) - 반응성
- **엔티티 정보** : 서버가 변경 시 브로드캐스트

**전투 시스템 흐름**

1. **클라이언트 측** :
   ```csharp
   // 공격 시작
   C_EntityAttackAnimation attackPkt = new C_EntityAttackAnimation();
   attackPkt.comboNum = currentAttackComboNum;
   attackPkt.dir = transform.forward;
   NetworkManager.Instance.Send(attackPkt.Write());
   ```

2. **서버 측** :
   - 공격 검증 (쿨다운, 거리, 상태)
   - 모든 클라이언트에 `S_BroadcastEntityAttackAnimation` 브로드캐스트
   - 히트 판정 계산 (공격 범위, 충돌 체크)
   - `S_BroadcastEntityAttackResult` 브로드캐스트 (데미지, 타겟 ID)

3. **클라이언트 응답** :
   ```csharp
   public void EntityAttackAnimation(S_BroadcastEntityAttackAnimation pkt)
   {
       if (playerDic.TryGetValue(pkt.id, out CommonPlayerController player))
       {
           player.currentAttackComboNum = pkt.comboNum;
           player.Anime = Define.Anime.Attack;
       }
   }

   public void EntityAttackResult(S_BroadcastEntityAttackResult pkt)
   {
       // 데미지 적용
       targetInfoState.hp -= pkt.damage;

       // 히트 이펙트 생성
       ClientManager.Resource.R_Instantiate("Effect/HitEffect", targetPosition);
   }
   ```

#### 엔티티 관리

**ManagementPP.cs**
```csharp
public class ManagementPP
{
    public MyPlayerController myPlayerCon;
    public Dictionary<int, CommonPlayerController> playerDic = new Dictionary<int, CommonPlayerController>();
    public Dictionary<int, ObjectController> objectDic = new Dictionary<int, ObjectController>();
    public Dictionary<int, MonsterController> monsterDic = new Dictionary<int, MonsterController>();

    // 초기 동기화 (씬 진입 시)
    public void EntityList(S_BroadcastEntityList pkt)
    {
        foreach (var entity in pkt.players)
        {
            if (entity.isSelf)
            {
                // 내 플레이어 생성
                GameObject go = ClientManager.Resource.R_Instantiate("Player/MyPlayer");
                myPlayerCon = go.GetComponent<MyPlayerController>();
                myPlayerCon.ID = entity.id;
            }
            else
            {
                // 다른 플레이어 생성
                GameObject go = ClientManager.Resource.R_Instantiate("Player/CommonPlayer");
                CommonPlayerController player = go.GetComponent<CommonPlayerController>();
                player.ID = entity.id;
                playerDic.Add(entity.id, player);
            }
        }
    }

    // 새 엔티티 입장
    public void EntityEnter(S_BroadcastEntityEnter pkt)
    {
        GameObject go = ClientManager.Resource.R_Instantiate("Player/CommonPlayer");
        CommonPlayerController player = go.GetComponent<CommonPlayerController>();
        player.ID = pkt.id;
        playerDic.Add(pkt.id, player);
    }

    // 엔티티 퇴장
    public void EntityLeave(S_BroadcastEntityLeave pkt)
    {
        if (playerDic.TryGetValue(pkt.id, out CommonPlayerController player))
        {
            ClientManager.Resource.R_Destroy(player.gameObject);
            playerDic.Remove(pkt.id);
        }
    }
}
```

## 🚀 성능 특성

### 네트워크 최적화
- **패킷 배칭** : 송신 큐로 여러 패킷을 묶어서 한 번에 전송
- **선택적 업데이트** : 상태 변화 시에만 패킷 전송 (애니메이션, 회전)
- **주기적 위치 동기화** : 100ms (10 Hz) - 네트워크 부하와 반응성의 균형

### 메모리 최적화
- **오브젝트 풀링** : 이펙트, 투사체 등 자주 생성/파괴되는 오브젝트 재사용
- **리소스 캐싱** : ResourceManager에서 로드한 리소스 캐싱
- **GC 압력 감소** : 오브젝트 풀링으로 메모리 할당 최소화

### 렌더링 최적화
- **동적 프레임 조정** : 포커스 시 144 FPS, 비포커스 시 60 FPS
- **LOD 시스템** : 거리에 따른 모델 디테일 조정
- **Occlusion Culling** : 보이지 않는 오브젝트 렌더링 제외

### 스레드 안전성
- **Dispatcher 패턴** : Unity API 호출을 메인 스레드로 마샬링
- **Lock 기반 동기화** : 패킷 큐 접근 시 데이터 레이스 방지
- **단일 스레드 게임 로직** : Unity 메인 스레드에서만 게임 상태 변경
