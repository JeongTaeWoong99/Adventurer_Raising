using UnityEngine;

// <상속 구조>
// InfoState -> PlayerInfoState
// 공통으로 상속되는 부분은 몬스터도 사용.
// 각자, 따로 사용하는 부분은 PlayerState같이 만들어서 구현.
public class InfoState : MonoBehaviour
{
    [Header("공통 - 서버 리얼타임 데이터베이스를 통해 세팅")]
    [SerializeField] protected string serialNumber; 
    [SerializeField] protected bool   live;         
    [SerializeField] protected bool   invincibility;
    [SerializeField] protected int    level;        
    [SerializeField] protected string nickName;     
    [SerializeField] protected int    currentHp;    
    [SerializeField] protected int    currentExp;   
    [SerializeField] protected int    currentGold; 
    
    [Header("서버에서 받은 serialNumber를 통해, 제이슨 정보로 디스플레이 세팅")]
    [SerializeField] protected int     maxHp;               
    [SerializeField] protected Vector3 normalAttackRange;   
    [SerializeField] protected int     normalAttackDamage;  
    [SerializeField] protected float   moveSpeed;           
    
    protected BaseController controller;
    
    #region Properties
    public bool Live
    {
        get => live;
        set => live = value;
    }   
    
    public bool Invincibility
    {
        get => invincibility;
        set => invincibility = value;
    }
    
    public int Level
    {
        get => level;
        set => level = value;
    }
    
    public string NickName
    {
        get => nickName;
        set => nickName = value;
    }
    
    public int Hp
    {
        get => currentHp;
        set
        {
            currentHp = value;
            
            // 내 캐릭터의 HP가 변경될 때 게임씬 UI 갱신 (MP는 임시 고정값 9999)
            if (ClientManager.Game.MyPlayerGameObject == gameObject && ClientManager.UI.gameSceneUI != null && maxHp > 0)
                ClientManager.UI.gameSceneUI.OnStateChange(currentHp, maxHp, 9999, 9999);
        }
    }
    public int MaxHp
    {
        get => maxHp;
        set
        {
            maxHp = value;
            // MaxHp 세팅 시에도 최신 HP/MaxHp를 반영하도록 UI 갱신
            if (ClientManager.Game.MyPlayerGameObject == gameObject && ClientManager.UI.gameSceneUI != null)
            {
                ClientManager.UI.gameSceneUI.OnStateChange(currentHp, maxHp, 9999, 9999, nickName);
            }
        }
    }
    public Vector3 NormalAttackRange
    {
        get => normalAttackRange;
        set => normalAttackRange = value;
    }
    public int NormalAttackDamage
    {
        get => normalAttackDamage;
        set => normalAttackDamage = value;
    }
    
    public string SerialNumber
    {
        get => serialNumber;
        set => serialNumber = value;
    }
    
    public float MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = value;
    }
    
    #endregion
    
    private void Start()
    {
        controller = GetComponent<BaseController>();
    }

    // 히트 공통
    public virtual void OnAttacked(GameObject attacker, Vector3 moveDir, int damage, string effectSerial)
    {
        // 사망
        if (Hp == 0)
        {
            controller.Anime = Define.Anime.Death;
        }
        // 히트
        else
        {
            // 오브젝트 히트 동작
            controller.dir     = moveDir;           // 서버에서 방향을 계산
            controller.Anime   = Define.Anime.Hit;
        }
    }
}
