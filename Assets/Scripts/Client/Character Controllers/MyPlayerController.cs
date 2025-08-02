using System;
using System.Collections;
using UnityEngine;

// <상속구조>
// BaseController -> CommonController/MonsterController -> MyPlayerController 
public class MyPlayerController  : CommonPlayerController
{
	private const int MoveMask           = (1 << (int)Define.Layer.Ground);									 // Ground만 해당
	private const int BlockAndObjectMask = (1 << (int)Define.Layer.Block) | (1 << (int)Define.Layer.Object); // Block + Object 해당
	
	public bool isComboAttack = false;  // 콤보 어택 상태
	
	private Define.Anime previousAnime = Define.Anime.Idle;  // 이전 애니메이션 상태
	
	private Coroutine _coSendPacket;
	
	protected override void Init()
	{
		base.Init();
		WorldObjectType = Define.WorldObject.MyPlayer;	// 재지정
		
		// 이동 이벤트 구독
		ClientManager.Input.MouseMoveAction -= OnMouseMoveEvent;
		ClientManager.Input.MouseMoveAction += OnMouseMoveEvent;
        
		// 공격 이벤트 구독
		ClientManager.Input.MouseAttackAction -= OnMouseAttackEvent;
		ClientManager.Input.MouseAttackAction += OnMouseAttackEvent;

		// 키 이벤트
		ClientManager.Input.KeyAction -= OnKeyEvent;
		ClientManager.Input.KeyAction += OnKeyEvent;
		
		_coSendPacket = StartCoroutine(CoSendPacket());
	}

	protected override void UpdateIdle()
	{
		base.UpdateIdle();
		
		// Idle 상태 최초 진입 시만 동기화 (이전 상태와 다를 때만)
		if (previousAnime != Define.Anime.Idle)
		{
			//Debug.Log("[MyPlayer] Idle 상태 진입 - 패킷 전송");
			previousAnime = Define.Anime.Idle; // 현재 상태로 업데이트
			
			// 애니메이션 동기화
			C_EntityAnimation animPacket = new C_EntityAnimation { animationID = (int)Define.Anime.Idle };
			NetworkManager.Instance.Send(animPacket.Write());
			
			// 로테이션 동기화
			C_EntityRotation locationPacket = new C_EntityRotation { rotationY = transform.rotation.eulerAngles.y };
			NetworkManager.Instance.Send(locationPacket.Write());
		}
	}
	
	protected override void UpdateRun()
	{
		base.UpdateRun();
		
		// 내 캐릭터 movePos   => 마우스를 통한 처리(Update만큰 갱신)
		// 다른 캐릭터 movePos => EntityMove에서 갱신(일정 주기 갱신)
		dir   = movePos - transform.position;
		dir.y = 0;
		
		// Run 상태 최초 진입 시만 동기화 (이전 상태와 다를 때만)
		if (previousAnime != Define.Anime.Run)
		{
			//Debug.Log("[MyPlayer] Run 상태 진입 - 패킷 전송");
			previousAnime = Define.Anime.Run; // 현재 상태로 업데이트
		
			// 애니메이션 동기화
			C_EntityAnimation animPacket = new C_EntityAnimation { animationID = (int)Define.Anime.Run };
			NetworkManager.Instance.Send(animPacket.Write());
		}
		
		// 캐릭터의 이동 경로 그리기
		Debug.DrawRay(transform.position + Vector3.up * 0.5f, dir.normalized, Color.green);
		// 캐릭터 앞 1.0f 거리 내에 Block 레이어의 장애물이 있는지 검사
		// 장애물이 있고, 마우스 오른쪽 버튼을 누르고 있으면    -> RUN 모션  + 이동 X
		// 장애물이 있고, 마우스 오른쪽 버튼을 안 누르고 있으면 -> IDLE 모션 + 이동 X
		if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, 1.0f, BlockAndObjectMask))
		{
			if (Input.GetMouseButton(1) == false)
				Anime = Define.Anime.Idle;
		}
		
		float distance = Vector3.Distance(movePos, transform.position);
		if (distance < 0.2f)
		{
			Anime = Define.Anime.Idle;
		}
	}

	protected override void UpdateDash()
	{
		base.UpdateDash();
	
		if (previousAnime != Define.Anime.Dash)
		{
			previousAnime = Define.Anime.Dash; // 현재 상태로 업데이트
			//Debug.Log("[MyPlayer] Dash 상태 진입");
		}
	}
	
	protected override void UpdateAttack()
	{
		base.UpdateAttack();
		
		if (previousAnime != Define.Anime.Attack)
		{
			previousAnime = Define.Anime.Attack; // 현재 상태로 업데이트
			//Debug.Log("[MyPlayer] Attack 상태 진입");
		}
	}
	
	protected override void UpdateSkill()
	{
		base.UpdateSkill();
	
		if (previousAnime != Define.Anime.Skill)
		{
			previousAnime = Define.Anime.Skill; // 현재 상태로 업데이트
			//Debug.Log("[MyPlayer] Skill 상태 진입");
		}
	}
	
	protected override void UpdateHit()
	{
		base.UpdateHit();
	
		if (previousAnime != Define.Anime.Hit)
			previousAnime = Define.Anime.Hit; // 현재 상태로 업데이트
		{
			//Debug.Log("[MyPlayer] Hit 상태 진입 - 패킷 전송");
		}
	}
	
	protected override void UpdateDie()
	{
		base.UpdateDie();
	
		if (previousAnime != Define.Anime.Death)
		{
			previousAnime = Define.Anime.Death; // 현재 상태로 업데이트
			//Debug.Log("[MyPlayer] Death 상태 진입 - 패킷 전송");
		}
	}
	
	// 마우스 오른쪽 이동 
	private void OnMouseMoveEvent(Define.MouseEvent evt)
	{
		switch (Anime)
		{
			case Define.Anime.Idle: case Define.Anime.Run:
				OnMouseMove_IdleRun(evt);
				break;
		}
	}
	private void OnMouseMove_IdleRun(Define.MouseEvent evt)
	{
		Ray     ray        = Camera.main.ScreenPointToRay(Input.mousePosition);
		bool    raycastHit = Physics.Raycast(ray, out var hit, 100.0f, MoveMask);
		Vector3 checkDir   = Vector3.zero;

		if (raycastHit)
		{
			switch (evt)
			{
				case Define.MouseEvent.PointerDown: case Define.MouseEvent.Press: // 1회 작동
					checkDir = hit.point - transform.position;
					checkDir.y = 0;
					// 너무 가까우면 무시
					if (checkDir.magnitude < 0.2f)
						return;
					// 멀면, 정상 작동
					movePos = hit.point;		  // movePos를 저장해 놔야, 도착 여부를 확인할 수 있음
					if(Anime != Define.Anime.Run) // 런이 아니면, 애니메이션 실행
						Anime = Define.Anime.Run;
					break;
			}
		}
	}

	// 마우스 왼쪽 공격 
	private void OnMouseAttackEvent(Define.MouseEvent evt)
	{
		switch (Anime)
		{
			case Define.Anime.Idle: case Define.Anime.Run:
				OnMouseAttack_IdleRun(evt);
				break;
			case Define.Anime.Attack:
				OnMouseAttack_Attack(evt);
				break;
		}
	}
	
	private void OnMouseAttack_IdleRun(Define.MouseEvent evt)
	{
		 Ray  ray        = Camera.main.ScreenPointToRay(Input.mousePosition);
		 bool raycastHit = Physics.Raycast(ray, out var hit, 100.0f);
		
		 if (raycastHit)
		 {
			 switch (evt)
			 {
				 case Define.MouseEvent.PointerDown:  // 1회 작동
					 currentAttackComboNum = 1; // Idle Run에서 시작은 콤보 초기화
					 NormalAttackRoutine(hit,currentAttackComboNum);
					 break;
			 }
		 }
	}

	private void OnMouseAttack_Attack(Define.MouseEvent evt)
	{
		Ray  ray        = Camera.main.ScreenPointToRay(Input.mousePosition);
		bool raycastHit = Physics.Raycast(ray, out var hit, 100.0f); 
		if (raycastHit)
		{
			switch (evt)
			{
				case Define.MouseEvent.PointerUp: // 1회 작동
					isComboAttack = false; 
					break;
			}
		}
	}

	// 키보드 이벤트 (통합 처리)
	private void OnKeyEvent()
	{
		// 대시 (Space)
		if (Input.GetKeyDown(KeyCode.Space))
		{
			HandleDashInput();
		}
		
		// 채팅 (Enter)
		if (Input.GetKeyDown(KeyCode.Return))
		{
			HandleChatInput();
		}
		
		// 여기에 추가 키 입력들을 넣을 수 있음
		// 예: 인벤토리 (I키), 스킬 (Q,W,E,R) 등
	}
	
	// 대시 입력 처리
	private void HandleDashInput()
	{
		if (Anime != Define.Anime.Idle && Anime != Define.Anime.Run) 
			return;
		
		// 마우스 위치를 이동 방향으로 설정
		Ray  ray        = Camera.main.ScreenPointToRay(Input.mousePosition);
		bool raycastHit = Physics.Raycast(ray, out var hit, 100.0f);
		
		dir   = movePos - transform.position;	
		dir.y = 0;
		if (raycastHit)
		{
			// 내 클라 변경
			dir   = hit.point - transform.position; // 1회 설정
			dir.y = 0;
			Anime = Define.Anime.Dash;			    // 실행
				
			// 즉시 행동 무브 패킷(현재 내 위치 포지션 스냅핑)
			C_EntityMove movePacket = new C_EntityMove {
				isInstantAction = true,
				posX            = transform.position.x, 
				posY            = transform.position.y, 
				posZ            = transform.position.z
			};
			NetworkManager.Instance.Send(movePacket.Write());
			
			// 대쉬 패킷
			C_EntityDash dashPtk = new C_EntityDash {
				animationID = (int)Define.Anime.Dash,
				dirX        = dir.x, 
				dirY        = dir.y, 
				dirZ        = dir.z
			};
			NetworkManager.Instance.Send(dashPtk.Write());
		}
	}
	
	// 채팅 입력 처리
	private void HandleChatInput()
	{
		var chatField = ClientManager.UI.gameSceneUI.ChatInputField;
		// 🔧 EventSystem 기준으로 포커스 상태 확인 (더 정확함)
		bool isChatFocused = UnityEngine.EventSystems.EventSystem.current?.currentSelectedGameObject == chatField.gameObject;
		
		// 채팅창 포커스가 아닐 때, Enter => 채팅창 포커스 On 시키기
		if (!isChatFocused)
		{
			//Debug.Log("채팅창 포커스 off => on");
			chatField.ActivateInputField(); // 채팅창 포커스 On
			return;						    // ✅ 핵심! 포커스 활성화 후 즉시 리턴
		}
		
		// 채팅창 포커스일 때, Enter => 채팅 전송 
		if (isChatFocused)
		{
			//Debug.Log("채팅창 포커스 on => off");
			// 텍스트가 있음 => 서버로 전송
			if (!string.IsNullOrEmpty(chatField.text))
			{
				string messageToSend = chatField.text;
				chatField.text = ""; // 비우기
				Debug.Log($"메시지 전송: {messageToSend}");
				
				// 포커스 해제 (선택사항)
				chatField.DeactivateInputField();
			}
			// 텍스트가 없음
			else
			{
				// Debug.Log("텍스트 입력창이 비어 있습니다. 포커스를 해제합니다.");
				// 포커스 해제 (빈 Enter 시)
				chatField.DeactivateInputField();
			}
		}
	}

	private IEnumerator CoSendPacket()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.05f);
			
			try
			{
				// 상시 위치 동기화
				// 연속 행동 무브 패킷(현재 내 위치 갱신)
				C_EntityMove movePacket = new C_EntityMove {isInstantAction = false, posX = transform.position.x, posY = transform.position.y, posZ = transform.position.z };
				NetworkManager.Instance.Send(movePacket.Write());
			}
			catch (NullReferenceException ex)
			{ Debug.Log($"MyPlayer not found: {ex}"); }
		}
	}
	
	public void StopSendPacketCoroutine()
	{
		if (_coSendPacket != null)
		{
			StopCoroutine(_coSendPacket);
			_coSendPacket = null;
		}
	}
	
	public void NormalAttackRoutine(RaycastHit hit, int comboNum)
	{
		// 이동 방향 계산(애니메이션 전환 시, 1회 갱신)
		// 내 캐릭터 movePos   -> 마우스 제어	  -> moveDirection 결정
		// 다른 캐릭터 movePos -> 패킷으로 받아옴 -> moveDirection 결정
		isComboAttack         = true;
		currentAttackComboNum = comboNum;            // 1
		dir                   = hit.point - transform.position;
		dir.y                 = 0;
		Anime                 = Define.Anime.Attack;
    	
    	// 즉시 행동 무브 패킷(현재 내 위치 포지션 스냅핑)
	    C_EntityMove movePacket = new C_EntityMove {isInstantAction = true, posX = transform.position.x, posY = transform.position.y, posZ = transform.position.z };
	    NetworkManager.Instance.Send(movePacket.Write());
    	
		// 공격 패킷
		C_EntityAttackAnimation attackAnimationPacket = new C_EntityAttackAnimation { animationID = (int)Define.Anime.Attack, attackAnimeNumID = comboNum, 
																					  dirX = dir.x, dirY = dir.y, dirZ = dir.z };
		NetworkManager.Instance.Send(attackAnimationPacket.Write()); // 자동 애니 확정
	}
}