using System;
using UnityEngine;
using UnityEngine.Serialization;

// <상속구조>
// BaseController -> CommonController/MonsterController -> MyPlayerController 
public abstract class BaseController : MonoBehaviour
{
	[Header("괸리 정보")]
	public int ID;

	[Header("공통 상속 사용")]
	public Define.Anime anime   = Define.Anime.Idle;
	public Vector3	    movePos = Vector3.zero;	// 마우스 클릭 위치(이동,공격), 플레이어 위치, 패킷으로 받아오는 다른 플레이어의 이동 위치
	public Vector3      dir     = Vector3.zero; 
	
	public bool isAnimeMove = false; // 공격 or 대쉬 같은 애니메이션 중, 이동 상태 체크
	
	public int    maxAttackComboNum     = 0; // 어택 최대 수(-> CountAttackAnimations 메서드로 확인)
	public int    currentAttackComboNum = 1; // 현재 콤보
	
	public string  currentSkillKey = "QWER";		// 스킬 키
	public Vector3 skillCreatePos  = Vector3.zero;	// 스킬 생성 위치(고정된 위치 생성 스킬이 아닌 경우 사용)

	private Vector3 gravityDirection = Vector3.zero; // 독립적인 중력 이동
	
	[Header("오브젝트 속성")]
	public Define.WorldObject WorldObjectType { get; protected set; } = Define.WorldObject.Unknown;
	
	[Header("초기화")]
	[HideInInspector] public CharacterController       characterController;      
	[HideInInspector] public Animator                  animator;                 
	[HideInInspector] public InfoState				   infoState;		           // 공통   
	[HideInInspector] public PlayerInfoState           playerInfoState;			   // 플레이어
	[HideInInspector] public ObjectAndMonsterInfoState objectAndMonsterInfoState;  // 오+몬	 
	
	// 상태가 변경될 때, 애니메이션 State를 1회 호출함.
	public Define.Anime Anime
	{
		get => anime;
		set
		{
			anime = value;
			
			if (animator == null)
				return;
				
			// 애니메이션 재생 중, 동일한 애니메이션 animator.CrossFade가 들어올 시.강제로 처음부터 재생하도록 함.(※ 원래는 같은 상태면, 크로스 페이드를 무시함.)
			switch (anime)
			{
				case Define.Anime.Idle: // 이어서 재생
					animator.CrossFade("IDLE", 0.1f, -1);
					break;
				case Define.Anime.Run:  // 이어서 재생
					animator.CrossFade("RUN", 0.1f, -1);
					break;
				case Define.Anime.Dash: // 초기화 재생
					isAnimeMove = true;
					animator.CrossFade("DASH", 0.1f, -1, 0f);
					break;
				case Define.Anime.Attack: // 초기화 재생
					isAnimeMove = true;
					animator.CrossFade("ATTACK" + currentAttackComboNum, 0.1f, -1, 0f);
					SoundCheckAndPlay();
					break;
				case Define.Anime.Skill: // 초기화 재생
					animator.CrossFade("SKILL" + currentSkillKey,        0.1f, -1, 0f);
					break;
				case Define.Anime.Hit: // 초기화 재생
					animator.CrossFade("HIT", 0.1f, -1, 0f);
					break;
				case Define.Anime.Death: // 초기화 재생
					animator.CrossFade("DEATH", 0.1f, -1, 0f);
					break;
			}
		}
	}
	
	// BaseController에서만 Awake 사용
	private void Awake()
	{
		// 스크립트 초기화
		animator                  = GetComponentInChildren<Animator>();
		characterController       = GetComponent<CharacterController>();
		infoState				  = GetComponent<InfoState>();
		playerInfoState           = GetComponent<PlayerInfoState>();
		objectAndMonsterInfoState = GetComponent<ObjectAndMonsterInfoState>();
		
		// 메서드 초기화
		maxAttackComboNum = CountAttackAnimations();
		
		// AnimationEventReceiver를 찾거나 추가하고, 자신을 등록합니다.
		// 이를 통해 애니메이션 이벤트가 Controller와 통신할 수 있습니다.
		AnimationEventReceiver receiver = gameObject.GetOrAddComponent<AnimationEventReceiver>();
		receiver.Initialize(this);
	}

	// BaseController에서만 Start 사용
	private void Start()
	{
		// 상속 스크립트에서 실행하도록 넣어줘야 함.
		Init();
	}

	private void Update()
	{
		switch (Anime)
		{
			case Define.Anime.Idle:
				UpdateIdle();
				break;
			case Define.Anime.Run:
				UpdateRun();
				break;
			case Define.Anime.Dash:
				UpdateDash();
				break;
			case Define.Anime.Attack:
				UpdateAttack();
				break;
			case Define.Anime.Skill:
				UpdateSkill();
				break;
			case Define.Anime.Hit:
				UpdateHit();
				break;
			case Define.Anime.Death:
				UpdateDie();
				break;
		}
		
		// 충력(캐릭터 컨트롤러가 없는 캐릭터는 중력 제외)
		if (characterController) GroundGravityDirection();
		else					 return;
		characterController.Move(gravityDirection * Time.deltaTime);
	}
	
	// 반드시 구현해야 하는 메서드 -> 모든 컨트롤러가 반드시 초기화 로직을 가져야 하므로 abstract
	protected abstract void Init();
	
	// 선택적으로 재정의할 수 있는 메서드 -> 상태 업데이트 메서드들은 모든 컨트롤러가 반드시 구현할 필요가 없으므로 virtual
	protected virtual void UpdateIdle()   { }
	protected virtual void UpdateRun()    { }
	protected virtual void UpdateDash()   { }
	protected virtual void UpdateAttack() { }
	protected virtual void UpdateSkill()  { }
	protected virtual void UpdateHit()    { }
	protected virtual void UpdateDie()    { }
	
	// 캐릭터 당, 기본 공격 애니메이션이 몇개인지 확인
	private int CountAttackAnimations()
	{
		int count = 0;
		AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;

		foreach (AnimationClip clip in clips)
		{
			//Debug.Log(clip.name);
			if (clip.name.Contains("ATTACK") || clip.name.Contains("Attack"))
				count++;
		}
		return count;
	}

	// 사운드 재생
	// TODO : 사운드 매니저로 옮기고, 시리얼넘버를 매개변수로 받아서, 해당하는 사운드 재생 
	private void SoundCheckAndPlay()
	{
		if(WorldObjectType is Define.WorldObject.MyPlayer or Define.WorldObject.CommonPlayer)
			ClientManager.Sound.Play("Sounds/Player" + "/P_Attack" + currentAttackComboNum,Define.Sound.Effect,1,transform);
	}
	
	// 중력
	private void GroundGravityDirection()
	{
		if (!characterController.isGrounded)    // 공중
			gravityDirection.y += -9.81f * Time.deltaTime * 2f;
		else if (characterController.isGrounded) // 바닥
			gravityDirection.y = -0.1f;		     // 움직일 때, 살짝 뜨는 문제로 인해, isGrounded가 false가 나오는 것을 방지하고자 -0.1f를 넣어줌...
	}
	
	private void OnDrawGizmos()
	{
		// 런타임에서만 작동하도록 체크
		if (!Application.isPlaying) return;

		// infoState가 NULL이면 리턴
		if (playerInfoState == null && objectAndMonsterInfoState == null) return;
		
		// 공격 범위를 시각화(공통)
		// Vector3 attackCenter = transform.position + transform.forward * 0.5f + Vector3.up * 1f;
		// Gizmos.color = new Color(1, 0, 0, 1f);
		// if(WorldObjectType is Define.WorldObject.MyPlayer or Define.WorldObject.CommonPlayer) 
		// 	Gizmos.matrix = Matrix4x4.TRS(attackCenter, transform.rotation, new Vector3(playerInfoState.NormalAttackRange.x * 2, playerInfoState.NormalAttackRange.y * 2, playerInfoState.NormalAttackRange.z * 2));
		// else 
		// 	Gizmos.matrix = Matrix4x4.TRS(attackCenter, transform.rotation, new Vector3(objectAndMonsterInfoState.NormalAttackRange.x * 2, objectAndMonsterInfoState.NormalAttackRange.y * 2, objectAndMonsterInfoState.NormalAttackRange.z * 2));
		// Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
		
		// 몬스터 찾기 범위를 시각화(몬스터)
		if (WorldObjectType is Define.WorldObject.Monster)
		{
			Gizmos.matrix = Matrix4x4.identity; // 매트릭스 리셋!
			Gizmos.color = new Color(0.92f, 0f, 1f);
			Gizmos.DrawWireSphere(transform.position, objectAndMonsterInfoState.FindRadius);
		}
	}
}