using UnityEngine;

// 애니메이션에 들어가는 플레이어 전용 펑션들을 사용하기 위한, 스크립트
public class AnimationEventReceiver : MonoBehaviour
{
    // 플레이어 전용
    private BaseController     _baseCon;
    private MyPlayerController _playerCon;

    // 런타임에 생성된 Controller가 자신을 등록하기 위해 호출하는 함수
    public void Initialize(BaseController myController)
    {
        _baseCon   = myController;
        _playerCon = _baseCon.GetComponent<MyPlayerController>();
    }
    
    public void ReturnToIdle()
    {
		if (ClientManager.Game.MyPlayerGameObject == gameObject)
		{
			_baseCon.isAnimeMove = false;				// 본인은 isAnimeMove로 바로 멈추고
			_baseCon.Anime       = Define.Anime.Idle;	// 다른 클라의 본인은 Define.Anime.Idle 상태를 보내서, EntityAnimation에서 isAnimeMove를 FALSE로 만들기.
		}
    }
    
    // 노멀 공격
    public void OnHitEvent()
    {
		// 공통 영역
		// 히트 이펙트(E 이펙트 N 노말 A 어택)
		// 프리팹 있는지 확인
		string normalAttackEffectPath = "Prefabs/" + "E" + _baseCon.infoState.SerialNumber + "NA" + _baseCon.currentAttackComboNum;
		GameObject original = ClientManager.Resource.R_Load<GameObject>(normalAttackEffectPath);
		if (original)
		{
			// Debug.Log("이팩트 존재");
			// x z 로테이션은 이펙트의 설정값 사용
			// y는 플레이어가 바라보는 방향 사용 
			Quaternion spawnRot = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * original.transform.rotation;
			ClientManager.Resource.R_Instantiate("E" + _baseCon.infoState.SerialNumber + "NA" + _baseCon.currentAttackComboNum, null, transform.position + Vector3.up * 1.5f, spawnRot);
		}
		else
		{
			//Debug.Log("이팩트 없음");
		}
		
	    // 플레이어는 자신 클라에서 판단하고 보내줌...
	    if (ClientManager.Game.MyPlayerGameObject == gameObject)
	    {
		    _baseCon.isAnimeMove = false;
			
		    // 내 위치 서버 동기화(현재 내 위치 갱신 + 스냅핑을 통한, isAnimeMove == false 만들기)
		    C_EntityMove movePacket = new C_EntityMove {isInstantAction = true, posX = transform.position.x, posY = transform.position.y, posZ = transform.position.z };
		    NetworkManager.Instance.Send(movePacket.Write());
			
			// 로테이션 스냅핑
		    C_EntityRotation rotation = new C_EntityRotation { rotationY = transform.rotation.eulerAngles.y };
		    NetworkManager.Instance.Send(rotation.Write());
		    
		    C_EntityAttackCheck attackCheck = new C_EntityAttackCheck {
			    createPosX = _baseCon.skillCreatePos.x,
			    createPosY = _baseCon.skillCreatePos.y,
			    createPosZ = _baseCon.skillCreatePos.z,
			    attackSerial = "A" + _baseCon.playerInfoState.SerialNumber + "_" + _baseCon.currentAttackComboNum // AP000_1 또는 AP000_3
		    };
		    NetworkManager.Instance.Send(attackCheck.Write());
		    
		    Debug.Log("movePacket을 통한 내 위치 스냅핑 : " + transform.position.x + " / " + transform.position.y + " / " + transform.position.z);
		    Debug.Log("C_EntityRotation을 통한 내 로테이션 스냅핑 : " + transform.rotation.eulerAngles.y);
		}
    }
	
	// 노멀 공격 콤보 체크
    public void OnComboAttackCheck()
    {
	    if (ClientManager.Game.MyPlayerGameObject == gameObject)
	    {
		    // 애니메이션 중, 공격 상태가 아니라면, 콤보 체크가 발동하지 않도록 한다.
		    if (_baseCon.Anime != Define.Anime.Attack)
			    return;
		
		    if (_playerCon.isComboAttack)
		    {
			    Ray  ray        = Camera.main.ScreenPointToRay(Input.mousePosition);
			    bool raycastHit = Physics.Raycast(ray, out var hit, 100.0f);
		
			    if (raycastHit)
			    {
				    // 콥보 프레스 연속 체크
				    _baseCon.currentAttackComboNum++;
				    if (_baseCon.currentAttackComboNum > _baseCon.maxAttackComboNum)
					    _baseCon.currentAttackComboNum = 1;
				    _playerCon.NormalAttackRoutine(hit,_baseCon.currentAttackComboNum);
			    }
		    }
	    }
    }
    
    // 스킬 요청
    public void OnSkillEvent()
    {
	    // 공통 영역
	    // 히트 이펙트(E 이펙트 N 노말 A 어택)
	    // 프리팹 있는지 확인
	    // string normalAttackEffectPath = "Prefabs/" + "E" + _baseCon.infoState.SerialNumber + "NA" + _baseCon.currentAttackComboNum;
	    // GameObject original = ClientManager.Resource.R_Load<GameObject>(normalAttackEffectPath);
	    // if (original)
	    // {
		   //  // Debug.Log("이팩트 존재");
		   //  // x z 로테이션은 이펙트의 설정값 사용
		   //  // y는 플레이어가 바라보는 방향 사용 
		   //  Quaternion spawnRot = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * original.transform.rotation;
		   //  ClientManager.Resource.R_Instantiate("E" + _baseCon.infoState.SerialNumber + "NA" + _baseCon.currentAttackComboNum, null, transform.position + Vector3.up * 1.5f, spawnRot);
	    // }
	    // else
	    // {
		   //  //Debug.Log("이팩트 없음");
	    // }
	    
	    
		
	    // 플레이어는 자신 클라에서 판단하고 보내줌...
	    if (ClientManager.Game.MyPlayerGameObject == gameObject)
	    {
		    // 내 위치 서버 동기화(현재 내 위치 갱신 + 스냅핑을 통한, isAnimeMove == false 만들기)
		    C_EntityMove movePacket = new C_EntityMove {isInstantAction = true, posX = transform.position.x, posY = transform.position.y, posZ = transform.position.z };
		    NetworkManager.Instance.Send(movePacket.Write());
		    
		    // 로테이션 스냅핑
		    C_EntityRotation rotation = new C_EntityRotation { rotationY = transform.rotation.eulerAngles.y };
		    NetworkManager.Instance.Send(rotation.Write());
		    
		    // 스킬 그래픽 생성 요청(+ 그래픽도 생성에 맞춰서, 서버에서는 스킬 히트 체크가 진행됨) 
		    C_EntitySkillCreate skillCreate = new C_EntitySkillCreate {
				createPosX = _baseCon.skillCreatePos.x,
				createPosY = _baseCon.skillCreatePos.y,
				createPosZ = _baseCon.skillCreatePos.z,
			    attackSerial = "S" + _baseCon.playerInfoState.SerialNumber + "_" + _baseCon.currentSkillKey // SP000_Q 또는 SP000_W
		    };
		    NetworkManager.Instance.Send(skillCreate.Write());
		    Debug.Log(_baseCon.skillCreatePos.x + " / " + _baseCon.skillCreatePos.y + " / " + _baseCon.skillCreatePos.z);
	    }
    }
} 