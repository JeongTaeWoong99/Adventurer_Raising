using UnityEngine;

// 공통 packet processing
public class OperationPP
{
	private static OperationPP _instance;
	
	public void Init()
	{

	}
	
	public void Clear()
	{
		
	}
	
	// 엔티티 이동
	public void EntityMove(S_BroadcastEntityMove p)
	{
		// 다른 플레이어 이동
		if (p.entityType == (int)Define.Layer.Player)
		{
			if (NetworkManager.Management.myPlayerCon != null & NetworkManager.Management.myPlayerCon.ID == p.ID) { }
			else
			{
				if (NetworkManager.Management.playerDic.TryGetValue(p.ID, out var player))
				{
					// 포지션 스냅핑 해야하는 이동인지 체크
					// 포지션 스냅핑(순간이동을 통해, 정확한 위치로 이옹)
					if (p.isInstantAction)
					{
						player.isAnimeMove                 = false;									
						
						player.characterController.enabled = false;									// CharacterController 일시 비활성화
						player.transform.position          = new Vector3(p.posX, p.posY, p.posZ);   // 절대 위치 설정
						player.characterController.enabled = true;									// CharacterController 재활성화
					}
					// 이동 방향 갱신
					else if(player.Anime == Define.Anime.Idle || player.anime == Define.Anime.Run)
					{
						// 다른 플레이어 캐릭터는 '최신 위치' - '클라 위치'로 방향을 구함...
						player.dir   = new Vector3(p.posX, p.posY, p.posZ) - player.transform.position;
						player.dir.y = 0f;	
					}
				}
			}
		}
		// 오브젝트 이동
		else if (p.entityType == (int)Define.Layer.Object)
		{
			if (NetworkManager.Management.objectDic.TryGetValue(p.ID, out var _object))
			{ }
		}
		// 몬스터 공격 애니메이션
		else if (p.entityType == (int)Define.Layer.Monster)
		{
			if (NetworkManager.Management.monsterDic.TryGetValue(p.ID, out var monster))
			{
				// 포지션 스냅핑 해야하는 이동인지 체크
				// 포지션 스냅핑(순간이동을 통해, 정확한 위치로 이옹)
				if (p.isInstantAction)
				{	
					monster.isAnimeMove                 = false;			
				
					monster.characterController.enabled = false;									// CharacterController 일시 비활성화
					monster.transform.position          = new Vector3(p.posX, p.posY, p.posZ);	// 절대 위치 설정
					monster.characterController.enabled = true;									// CharacterController 재활성화
				}
				// 이동 방향 갱신
				else if(monster.Anime == Define.Anime.Idle || monster.anime == Define.Anime.Run)
				{
					//monster.isAnimeMove = false;
					// 서버에서 계산한 디렉션을 바로 넣어줌
					monster.dir   = new Vector3(p.posX, p.posY, p.posZ);
					monster.dir.y = 0f;
				}
			}
		}
	}
	
	// 엔티티 회전
	public void EntityRotation(S_BroadcastEntityRotation p)
	{
		// 플레이어 회전
		if (p.entityType == (int)Define.Layer.Player)
		{
			if (NetworkManager.Management.myPlayerCon != null & NetworkManager.Management.myPlayerCon.ID == p.ID) { }
			else
			{
				if (NetworkManager.Management.playerDic.TryGetValue(p.ID, out var player))
				{
					// 회전 
					//Debug.Log(p.ID + "의 로테이션이 " + + p.rotationY + "으로 고정됨...");
					player.gameObject.transform.rotation = Quaternion.Euler(0, p.rotationY, 0);
				}
			}
		}	
	}
	
	// 엔티티 애니메이션 변경
	public void EntityAnimation(S_BroadcastEntityAnimation p)
	{
		// 플레이어 애니메이션
		if (p.entityType == (int)Define.Layer.Player)
		{
			if (NetworkManager.Management.myPlayerCon != null & NetworkManager.Management.myPlayerCon.ID == p.ID) { }
			else
			{
				if (NetworkManager.Management.playerDic.TryGetValue(p.ID, out var player))
				{
					player.isAnimeMove = false; // isAnimeMove를 이용하지 않는 애니메이션들 = false
					if(player.Anime != (Define.Anime)p.animationID)	// 동인한 상태이면, 중복 실행 방지
						player.Anime = (Define.Anime)p.animationID;
				}
			}
		}
		// 오브젝트 애니메이션
		else if (p.entityType == (int)Define.Layer.Object)
		{
			if (NetworkManager.Management.objectDic.TryGetValue(p.ID, out var _object))
			{
				_object.isAnimeMove = false; // isAnimeMove를 이용하지 않는 애니메이션들 = false
				if(_object.Anime != (Define.Anime)p.animationID) // 동인한 상태이면, 중복 실행 방지
					_object.Anime = (Define.Anime)p.animationID;
			}
		}
		// 몬스터 애니메이션
		else if (p.entityType == (int)Define.Layer.Monster)
		{
			if (NetworkManager.Management.monsterDic.TryGetValue(p.ID, out var monster))
			{
				monster.isAnimeMove = false; // isAnimeMove를 이용하지 않는 애니메이션들 = false
				if(monster.Anime != (Define.Anime)p.animationID) // 동인한 상태이면, 중복 실행 방지
					monster.Anime = (Define.Anime)p.animationID;
			}
		}
	}
	
	// 플레이어 대쉬 변경
	public void Dash(S_BroadcastEntityDash p)
	{
		// 플레이어 대쉬
		if (p.entityType == (int)Define.Layer.Player)
		{
			if (NetworkManager.Management.myPlayerCon != null & NetworkManager.Management.myPlayerCon.ID == p.ID) { }
			else
			{
				if (NetworkManager.Management.playerDic.TryGetValue(p.ID, out var player))
				{	
					// 각자의 클라에서 실행하는 작업과 동일
					// Debug.Log("대쉬 방향 => " + new Vector3(p.dirX, p.dirY, p.dirZ));
					player.dir   = new Vector3(p.dirX, p.dirY, p.dirZ);
					player.Anime = (Define.Anime)p.animationID;
				}
			}
		}
	}
	
	// 엔티티 공격 애니메이션 
	public void EntityAttackAnimation(S_BroadcastEntityAttackAnimation p)
	{
		// 플레이어 공격 애니메이션
		if (p.entityType == (int)Define.Layer.Player)
		{
			//Debug.Log("Player의 PlayAttackAnim이 들어옴...");
			if (NetworkManager.Management.myPlayerCon != null & NetworkManager.Management.myPlayerCon.ID == p.ID) { }
			else
			{
				if (NetworkManager.Management.playerDic.TryGetValue(p.ID, out var player))
			    {
			        player.dir                   = new Vector3(p.dirX, p.dirY, p.dirZ); // 공격 이동 방향
			        player.currentAttackComboNum = p.attackAnimeNumID;                  // 콤보 번호
			        player.Anime                 = Define.Anime.Attack;                 // 공격 애니메이션 재생(자동)
			    }
			}
		}
		// 오브젝트 공격 애니메이션
		else if (p.entityType == (int)Define.Layer.Object)
		{
			if (NetworkManager.Management.objectDic.TryGetValue(p.ID, out var _object))
			{
				_object.dir                   = new Vector3(p.dirX, p.dirY, p.dirZ); // 공격 이동 방향
				_object.currentAttackComboNum = p.attackAnimeNumID;                  // 콤보 번호
				_object.Anime                 = Define.Anime.Attack;                 // 공격 애니메이션 재생(자동)
			}
		}
		// 몬스터 공격 애니메이션
		else if (p.entityType == (int)Define.Layer.Monster)
		{
			if (NetworkManager.Management.monsterDic.TryGetValue(p.ID, out var monster))
			{
				monster.dir                   = new Vector3(p.dirX, p.dirY, p.dirZ); // 공격 이동 방향
				monster.currentAttackComboNum = p.attackAnimeNumID;                  // 콤보 번호
				monster.Anime                 = Define.Anime.Attack;                 // 공격 애니메이션 재생(자동)
			}
		}
	}
	
	// 엔티티 공격 결과
	public void EntityAttackResult(S_BroadcastEntityAttackResult p)
	{	
		int     attackerID   = p.attackerID;
		int     damage       = p.damage;
		string  effectSerial = p.effectSerial;
		
		// 모든 결과 확인
		if (p.attackerEntityType == (int)Define.Layer.Player)
		{
			//Debug.Log("EntityAttackResult => " + p.attackerEntityType);
			// 공격자 찾기
			if (NetworkManager.Management.playerDic.TryGetValue(attackerID, out var attackerPlayer))
			{
				// 타겟 처리(플레이어는 오브젝트 or 몬스터)
				foreach (var targetEntity in p.entitys)
				{
					// 오브젝트트의 경우
					if (targetEntity.targetEntityType== (int)Define.Layer.Object)
					{
						if (NetworkManager.Management.objectDic.TryGetValue(targetEntity.targetID, out var targetObject) && targetObject != null)
						{
							InfoState targetInfoState = targetObject.GetComponent<InfoState>();
							if (targetInfoState != null) 
							{
								targetInfoState.OnAttacked(attackerPlayer.gameObject, 
									new Vector3(targetEntity.hitMoveDirX, targetEntity.hitMoveDirY, targetEntity.hitMoveDirZ) , damage, effectSerial);
							}
							else
							{
								Debug.Log($"[Client] 오브젝트 {targetEntity.targetID}의 InfoState가 null입니다.");
							}
						}
						else
						{
							Debug.Log($"[Client] 오브젝트 {targetEntity.targetID}가 클라이언트에서 찾을 수 없습니다.");
						}
					}
					// 몬스터의 경우
					else if (targetEntity.targetEntityType== (int)Define.Layer.Monster)
					{
						if (NetworkManager.Management.monsterDic.TryGetValue(targetEntity.targetID, out var targetObject) && targetObject != null)
						{
							InfoState targetInfoState = targetObject.GetComponent<InfoState>();
							if (targetInfoState != null) 
							{
								targetInfoState.OnAttacked(attackerPlayer.gameObject,
									new Vector3(targetEntity.hitMoveDirX, targetEntity.hitMoveDirY, targetEntity.hitMoveDirZ) , damage, effectSerial);
							}
							else
							{
								Debug.Log($"[Client] 몬스터 {targetEntity.targetID}의 InfoState가 null입니다.");
							}
						}
						else
						{
							Debug.Log($"[Client] 몬스터 {targetEntity.targetID}가 클라이언트에서 찾을 수 없습니다.");
						}
					}
				}
			}
		}
		else if (p.attackerEntityType is (int)Define.Layer.Object)
		{
			//Debug.Log("EntityAttackResult => " + p.attackerEntityType);
			// 공격자 찾기
			if (NetworkManager.Management.objectDic.TryGetValue(attackerID, out var attackerObject))
			{
				// 타겟 처리(오브젝트는 플레이어)
				foreach (var targetEntity in p.entitys)
				{
					if (targetEntity.targetEntityType == (int)Define.Layer.Player)
					{
						// 플레이어
						if (NetworkManager.Management.playerDic.TryGetValue(targetEntity.targetID, out var targetObject) && targetObject != null)
						{
							InfoState targetInfoState = targetObject.GetComponent<InfoState>();
							if (targetInfoState != null) 
							{
								targetInfoState.OnAttacked(attackerObject.gameObject,
									new Vector3(targetEntity.hitMoveDirX, targetEntity.hitMoveDirY, targetEntity.hitMoveDirZ) , damage, effectSerial);
							}
							else
							{
								Debug.Log($"[Client] 플레이어 {targetEntity.targetID}의 InfoState가 null입니다.");
							}
						}
						else
						{
							Debug.Log($"[Client] 플레이어 {targetEntity.targetID}가 클라이언트에서 찾을 수 없습니다.");
						}
					}
				}
			}
		}
		else if (p.attackerEntityType is (int)Define.Layer.Monster)
		{
			//Debug.Log("EntityAttackResult => " + p.attackerEntityType);
			// 공격자 찾기
			if (NetworkManager.Management.monsterDic.TryGetValue(attackerID, out var attackerMonster))
			{
				// 타겟 처리(몬스터는 플레이어)
				foreach (var targetEntity in p.entitys)
				{
					if (targetEntity.targetEntityType == (int)Define.Layer.Player)
					{
						// 플레이어
						if (NetworkManager.Management.playerDic.TryGetValue(targetEntity.targetID, out var targetObject) && targetObject != null)
						{
							InfoState targetInfoState = targetObject.GetComponent<InfoState>();
							if (targetInfoState != null) 
							{
								targetInfoState.OnAttacked(attackerMonster.gameObject,
									new Vector3(targetEntity.hitMoveDirX, targetEntity.hitMoveDirY, targetEntity.hitMoveDirZ) , damage, effectSerial);
							}
							else
							{
								Debug.Log($"[Client] 플레이어 {targetEntity.targetID}의 InfoState가 null입니다.");
							}
						}
						else
						{
							Debug.Log($"[Client] 플레이어 {targetEntity.targetID}가 클라이언트에서 찾을 수 없습니다.");
						}
					}
				}
			}
		}
	}
	
	// 채팅(플레이어전용)
	public void Chatting(S_BroadcastChatting p)
	{	
		int    id       = p.ID;		  // 채팅을 보낸 사람 ID
		string contents = p.contents; // 채팅 내용
		
		// 프리팹 생성 및 컴포넌트 받아오기
		UI_Chatting chatClone = ClientManager.UI.MakeSubItem<UI_Chatting>(ClientManager.UI.gameSceneUI.ChatContentGroup.transform,"UI_ChattingText");
		// 레이아웃 강제 리빌드(빌드 환경에서 초기 프레임에 레이아웃이 늦게 적용되는 문제 방지)
		UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(ClientManager.UI.gameSceneUI.ChatContentGroup.GetComponent<RectTransform>());
		
		// 닉네임 확인
		string nickName = $"Player_{id}";
		if (NetworkManager.Management.playerDic.TryGetValue(id, out var chatPlayer) && chatPlayer != null && chatPlayer.infoState != null)
			nickName = chatPlayer.infoState.NickName;
		
		// 데이터 전달(지연 적용: 내부에서 바인딩 완료 시점과 결합하여 안전 적용)
		chatClone.SetData(nickName, contents);
	}
}