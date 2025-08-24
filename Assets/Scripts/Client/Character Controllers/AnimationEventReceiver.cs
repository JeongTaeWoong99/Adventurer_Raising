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
        // 대시 완료 시 잠깐 dir을 고정하여 다음 프레임에서 올바른 방향으로 설정되도록 함
        if (_baseCon.Anime == Define.Anime.Dash)
        {
            StartCoroutine(DelayedDirReset());
        }
        else
        {
            _baseCon.dir = Vector3.zero;
        }
        
		if (ClientManager.Game.MyPlayerGameObject == gameObject)
		{
		    _baseCon.isAnimeMove = false;			  // 본인은 isAnimeMove로 바로 멈추고
		    _baseCon.Anime       = Define.Anime.Idle; // 다른 클라의 본인은 Define.Anime.Idle 상태를 보내서, EntityAnimation에서 isAnimeMove를 FALSE로 만들기.
		}
    }
    
    // 대시 완료 후 잠깐 지연하여 dir 리셋
    private System.Collections.IEnumerator DelayedDirReset()
    {
        yield return new WaitForFixedUpdate(); // 1 Physics Frame 대기
        _baseCon.dir = Vector3.zero;
    }
    
    // 공격 이벤트(일반 공격)
    public void OnHitEvent()
    {
		// 클라 일반 공격 이펙트
		// 클라쪽에서 애니메이션에 맞춰서, 재생하는 이펙트(해당 이펙트가 나오는 것과 서버에서 유효타가 되는 것은 상관 없음...)
		string clientMakeEffect = "Prefabs/" + "E" + _baseCon.infoState.SerialNumber + "NA" + _baseCon.currentAttackComboNum;
		GameObject original = ClientManager.Resource.R_Load<GameObject>(clientMakeEffect);
		if (original)
		{
			// Debug.Log("이팩트 존재");
			// x z 로테이션은 이펙트의 설정값 사용(y는 플레이어가 바라보는 방향 사용 )
			Quaternion spawnRot = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * original.transform.rotation;
			ClientManager.Resource.R_Instantiate("E" + _baseCon.infoState.SerialNumber + "NA" + _baseCon.currentAttackComboNum, null, transform.position + Vector3.up * 1.5f, spawnRot);
		}
		else
			Debug.Log("이팩트 없음");
		
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
		    
		    // 공격 요청
		    C_EntityAttack attackCheck = new C_EntityAttack {
			    createPosX   = _baseCon.skillCreatePos.x,
			    createPosY   = _baseCon.skillCreatePos.y,
			    createPosZ   = _baseCon.skillCreatePos.z,
			    attackSerial = "A_" + _baseCon.playerInfoState.SerialNumber + "_" + _baseCon.currentAttackComboNum // A_P000_1 또는 A_P000_2 또는 A_P000_3
		    };
		    NetworkManager.Instance.Send(attackCheck.Write());
		}
    }
    
    // 공격 이벤트(스킬 공격)
    public void OnSkillHitEvent()
    {
	    // 클라 스킬 공격 이펙트
	    // 클라쪽에서 애니메이션에 맞춰서, 재생하는 이펙트(해당 이펙트가 나오는 것과 서버에서 유효타가 되는 것은 상관 없음...)
	    // ~~~~~~~
		
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
		    
		    // 공격 요청
		    C_EntityAttack attackCheck = new C_EntityAttack {
			    createPosX   = _baseCon.skillCreatePos.x,
			    createPosY   = _baseCon.skillCreatePos.y,
			    createPosZ   = _baseCon.skillCreatePos.z,
			    attackSerial = "A_" + _baseCon.playerInfoState.SerialNumber + "_" + _baseCon.currentSkillKey // A_P000_Q 또는 A_P000_W
		    };
		    NetworkManager.Instance.Send(attackCheck.Write());
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

	// 대쉬 값 초기화
	public void ResetDash()
	{
		_baseCon.movePos = _baseCon.transform.position;
	}
}
