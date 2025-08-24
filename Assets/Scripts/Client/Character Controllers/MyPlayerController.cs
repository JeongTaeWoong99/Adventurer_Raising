using System;
using System.Collections;
using UnityEngine;

// <상속구조>
// BaseController -> CommonController/MonsterController -> MyPlayerController 
public class MyPlayerController  : CommonPlayerController
{
	private const int MoveMask     = (1 << (int)Define.Layer.Ground);								     // 오른쪽 마우스를 눌렀을 때, 이동 반응 할 레이어(Ground)
	private const int RunBlockMast = (1 << (int)Define.Layer.Block)  | (1 << (int)Define.Layer.Object);  // UpdateRun에서 이동 중, 마우스를 땠을 때, 멈출 레이어(Block + Object)
	private const int AttackMask   = (1 << (int)Define.Layer.Ground) | (1 << (int)Define.Layer.Monster); // 일반 공격과 스킬 실행 가능 레이어(Ground + Monster)
	
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

			// // 애니메이션 동기화
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
			// //Debug.Log("[MyPlayer] Run 상태 진입 - 패킷 전송");
			previousAnime = Define.Anime.Run; // 현재 상태로 업데이트
		
			// // 애니메이션 동기화
			C_EntityAnimation animPacket = new C_EntityAnimation { animationID = (int)Define.Anime.Run };
			NetworkManager.Instance.Send(animPacket.Write());
		}
		
		// 캐릭터의 이동 경로 그리기
		Debug.DrawRay(transform.position + Vector3.up * 0.5f, dir.normalized, Color.green);
		// 캐릭터 앞 1.0f 거리 내에 Block 레이어의 장애물이 있는지 검사
		// 장애물이 있고, 마우스 오른쪽 버튼을 누르고 있으면    -> RUN 모션  + 이동 X
		// 장애물이 있고, 마우스 오른쪽 버튼을 안 누르고 있으면 -> IDLE 모션 + 이동 X
		if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, 1.0f, RunBlockMast))
		{
			if (Input.GetMouseButton(1) == false)
				Anime = Define.Anime.Idle;
		}
		
		float distance = Vector3.Distance(movePos, transform.position);
		if (distance < 0.25f)
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
		 bool raycastHit = Physics.Raycast(ray, out var hit, 100.0f, AttackMask);
		
		 if (raycastHit)
		 {
			 switch (evt)
			 {
				 // 콤보 처음 시작
				 case Define.MouseEvent.PointerDown: // 1회 작동
					 currentAttackComboNum = 1;						 // Idle Run에서 시작은 콤보 초기화
					 NormalAttackRoutine(hit,currentAttackComboNum); 
					 break;
			 }
		 }
	}

	private void OnMouseAttack_Attack(Define.MouseEvent evt)
	{
		switch (evt)
		{
			// 콤보 끔내기
			case Define.MouseEvent.PointerUp: // 1회 작동
				isComboAttack = false;
				break;
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
		
		// 스킬 (Q W E R)
		if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.R))
		{
			// 레벨 제한으로 잠금된 스킬은 입력 차단
			if      (Input.GetKeyDown(KeyCode.Q) && ClientManager.UI.gameSceneUI.IsSkillUnlocked("Q") == false) return;
			else if (Input.GetKeyDown(KeyCode.W) && ClientManager.UI.gameSceneUI.IsSkillUnlocked("W") == false) return;
			else if (Input.GetKeyDown(KeyCode.E) && ClientManager.UI.gameSceneUI.IsSkillUnlocked("E") == false) return;
			else if (Input.GetKeyDown(KeyCode.R) && ClientManager.UI.gameSceneUI.IsSkillUnlocked("R") == false) return;
			HandleSkillInput();
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
		bool raycastHit = Physics.Raycast(ray, out var hit, 100.0f, AttackMask);
		
		if (raycastHit)
		{ 
			// 내 클라 변경
			dir   = hit.point - transform.position; // 1회 설정
			dir.y = 0;
			Anime = Define.Anime.Dash;				// 실행
			
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
	
	// 스킬 입력(Q W E R)
	private void HandleSkillInput()
	{
		if (Anime != Define.Anime.Idle && Anime != Define.Anime.Run) 
			return;
		
		// 마우스 위치를 스킬 방향(+위치)으로 설정
		Ray  ray        = Camera.main.ScreenPointToRay(Input.mousePosition);
		bool raycastHit = Physics.Raycast(ray, out var hit, 100.0f, AttackMask);
		
		if (raycastHit)
		{
			// 어떤 스킬 키(Q/W/E/R)를 눌렀는지 기록
			if      (Input.GetKeyDown(KeyCode.Q)) currentSkillKey = "Q";
			else if (Input.GetKeyDown(KeyCode.W)) currentSkillKey = "W";
			else if (Input.GetKeyDown(KeyCode.E)) currentSkillKey = "E";
			else if (Input.GetKeyDown(KeyCode.R)) currentSkillKey = "R";
			else                                  currentSkillKey = "";

			// 유효 키가 아니면 리턴
			if (string.IsNullOrEmpty(currentSkillKey))
				return;

			// 쿨타임 중이면 사용 불가
			if (ClientManager.UI.gameSceneUI != null && ClientManager.UI.gameSceneUI.IsSkillOnCooldown(currentSkillKey))
				return;
			
			// 쿨타임 시작: 데이터에서 쿨타임을 읽고 UI에 전달
			try
			{
				string serial 		   = infoState.serialNumber;
				string atkKey 		   = "A_" + serial + "_" + currentSkillKey;
				AttackInfoData atkInfo = ClientManager.Data.AttackInfoDict[atkKey];
				float cool			   = float.Parse(atkInfo.coolTime);
				ClientManager.UI.gameSceneUI.StartSkillCooldown(currentSkillKey, cool);
			}
			catch (Exception e)
			{
				Debug.Log($"쿨타임 데이터 조회 실패: {e}");
			}
			
			// 내 클라 바라보는 방향 변경
			dir   		   = hit.point - transform.position; // 1회 설정
			dir.y 		   = 0;
			skillCreatePos = hit.point;			 			 // 스킬 생성 위치(고정된 위치 생성 스킬이 아닌 경우 사용 + 애니메이션이 끊기지 않고, OnSkillEvent()가 호출 된 경우 사용)
			Anime          = Define.Anime.Skill; 			 // 실행
	
			// 즉시 행동 무브 패킷(현재 내 위치 포지션 스냅핑)
			C_EntityMove movePacket = new C_EntityMove {isInstantAction = true, posX = transform.position.x, posY = transform.position.y, posZ = transform.position.z };
			NetworkManager.Instance.Send(movePacket.Write());
			
			// 공격 애니메이션 패킷(스킬도 C_EntityAttackAnimation 같이 사용... attackAnimeNumID에 10 20 30 40을 넣어서, Q W E R 구분)
			int skillAttackAnimeNumID = 0; // 10 = Q, 20 = W, 30 = E, 40 = R
			if		(currentSkillKey == "Q") skillAttackAnimeNumID = 10;
			else if (currentSkillKey == "W") skillAttackAnimeNumID = 20;
			else if (currentSkillKey == "E") skillAttackAnimeNumID = 30;
			else if (currentSkillKey == "R") skillAttackAnimeNumID = 40;
			
			C_EntityAttackAnimation attackAnimationPacket = new C_EntityAttackAnimation { animationID = (int)Define.Anime.Attack, attackAnimeNumID = skillAttackAnimeNumID, 
																						  dirX = dir.x, dirY = dir.y, dirZ = dir.z };
			NetworkManager.Instance.Send(attackAnimationPacket.Write());
		}
	}
	
	// 채팅 입력 처리
	private void HandleChatInput()
	{
		var chatField = ClientManager.UI.gameSceneUI.ChatInputField;
		bool isChatFocused = UnityEngine.EventSystems.EventSystem.current?.currentSelectedGameObject == chatField.gameObject; // 🔧 EventSystem 기준으로 포커스 상태 확인 (더 정확함)
		
		// 채팅창 포커스가 아닐 때, Enter => 채팅창 포커스 On 시키기
		if (!isChatFocused)
		{
			chatField.ActivateInputField(); // 채팅창 포커스 On
			return;						    // 포커스 활성화 후 즉시 리턴!
		}
		
		// 채팅창 포커스일 때, Enter => 채팅 전송 
		if (isChatFocused)
		{
			// 텍스트가 있음 => 서버로 전송
			if (!string.IsNullOrEmpty(chatField.text))
			{
				C_Chatting chat = new C_Chatting {
					contents = chatField.text
				};
				NetworkManager.Instance.Send(chat.Write());
				
				chatField.text = "";			  // 비우기
				chatField.DeactivateInputField(); // 포커스 해제
			}
			// 텍스트가 없음
			else
			{
				chatField.DeactivateInputField(); // 포커스 해제
			}
		}
	}

	private IEnumerator CoSendPacket()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.1f);
			
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
		// 내 캐릭터 movePos   -> 마우스 제어	   -> moveDirection 결정
		// 다른 캐릭터 movePos -> 패킷으로 받아옴 -> moveDirection 결정
		isComboAttack         = true;
		currentAttackComboNum = comboNum;           
		dir                   = hit.point - transform.position;
		dir.y                 = 0;
		Anime                 = Define.Anime.Attack;
    	
    	// 즉시 행동 무브 패킷(현재 내 위치 포지션 스냅핑)
	    C_EntityMove movePacket = new C_EntityMove {isInstantAction = true, posX = transform.position.x, posY = transform.position.y, posZ = transform.position.z };
	    NetworkManager.Instance.Send(movePacket.Write());
    	
		// 공격 애니메이션 패킷
		C_EntityAttackAnimation attackAnimationPacket = new C_EntityAttackAnimation { animationID = (int)Define.Anime.Attack, attackAnimeNumID = comboNum, 
																					  dirX = dir.x, dirY = dir.y, dirZ = dir.z };
		NetworkManager.Instance.Send(attackAnimationPacket.Write());
	}
}