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
			
		    // 플레이어의 전방 위치 계산 (앞 + 위) => xyz와 회전y는 스킬에서도 사용될 듯. => 냠겨두기
		    Vector3 attackCenter = transform.position + transform.forward * 0.5f + Vector3.up * 1f;
		    C_EntityAttackCheck attackCheck = new C_EntityAttackCheck {
			    attackCenterX = attackCenter.x,
			    attackCenterY = attackCenter.y,
			    attackCenterZ = attackCenter.z,
			    rotationY     = transform.rotation.eulerAngles.y,
			    attackSerial  = "A" + _baseCon.playerInfoState.SerialNumber + "_" + _baseCon.currentAttackComboNum // AP000_1 또는 AP000_3
		    };
		    NetworkManager.Instance.Send(attackCheck.Write());
		}
    }

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
} 