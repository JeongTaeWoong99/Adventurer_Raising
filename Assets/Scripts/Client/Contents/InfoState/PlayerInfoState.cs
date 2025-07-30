using UnityEngine;

// 플레이어의 상태를 관리하는 클래스
// State 클래스를 상속받아 플레이어만의 고유한 상태(경험치, 골드 등)를 관리
public class PlayerInfoState : InfoState
{
	[Header("플레이어 - 서버에서 받은 serialNumber를 통해, 제이슨 정보로 디스플레이 세팅")]
	[SerializeField] protected int needExp;

	#region Properties
	// 플레이어의 경험치를 관리하는 프로퍼티
	// 경험치가 변경될 때마다 레벨업 체크를 수행
	public int Exp
	{
		get => currentExp;
		set
		{
			currentExp = value;

			var dict = ClientManager.Data.CharacterInfoDict;
			string key = $"{serialNumber}_{level}";
			CharacterInfoData playerInfo = dict[key];

			// 레벨업 경험치 부족 -> 슬라이더만 변화
			if (currentExp < int.Parse(playerInfo.needExp))
			{
				Debug.Log("1번");
				ClientManager.UI.gameSceneUI.GetComponent<UI_GameScene>().OnExpSliderChanged(currentExp, int.Parse(playerInfo.needExp));
			}
			// 경험치 충분
			else
			{
				string nextKey = $"{serialNumber}_{level + 1}";
				// 최고레벨 도달하면, 슬라이더 고정 설정
				if (dict.TryGetValue(nextKey, out playerInfo) == false)
				{
					Debug.Log("2번");
					ClientManager.UI.gameSceneUI.GetComponent<UI_GameScene>().OnExpSliderChanged(1, 1,"MaxLevel");
				}
				// 다음 레벨 있음 -> 레벨업 후 세팅
				else
				{
					currentExp = 0;   // exp 초기화
					level++;          // 레벨 증가
					SetStat(level);   // 증가된 레벨로 스탯 설정
				}
			}
		}
	}

	// 플레이어의 골드를 관리하는 프로퍼티
	public int Gold
	{
		get => currentGold;
		set => currentGold = value;
	}

	public int NeedExp
	{
		get => needExp;
		set => needExp = value;
	}

	# endregion

	// 레벨에 따른 스탯을 설정
	public void SetStat(int level)
	{
		var dict = ClientManager.Data.CharacterInfoDict;
		string key = $"{serialNumber}_{level}";
		CharacterInfoData info = dict[key];

		// 공통
		this.level         = level;
		maxHp              = int.Parse(info.maxHp);
		normalAttackDamage = int.Parse(info.normalAttackDamage);
		moveSpeed          = float.Parse(info.moveSpeed);
		normalAttackRange  = Extension.ParseVector3(info.normalAttackRange);
		// 플레이어 전용
		needExp            = int.Parse(info.needExp);

		// 체력바 만들기(앞에 값들이 설정되고 나서,....)
		if (!gameObject.GetComponentInChildren<UI_State>())
			ClientManager.UI.MakeWorldSpaceUI<UI_State>(transform);
		else
		{
			// 레벨 ui 변경(SetStat가 호출되는 경우는 레벨이 변경될 경우에 호출됨...)
			if (gameObject.GetComponentInChildren<UI_State>() != null)
				gameObject.GetComponentInChildren<UI_State>().levelText.text = level.ToString();
		}
	}

	public override void OnAttacked(GameObject attacker,Vector3 attackCenterVec, int damage, string effectSerial)
	{
		base.OnAttacked(attacker, attackCenterVec, damage, effectSerial);
		
		// 플레이어는 Decimals(Red) 넘버
		ClientManager.Resource.R_Instantiate("Decimals(Red)", null, transform.position + Vector3.up * 4f, damage);
		
		// 사망 및 내 캐릭터인 경우
		if (Hp == 0 && ClientManager.Game.MyPlayerGameObject == gameObject)
		{
			// 사망 시, 주기적인 체크 종료(+씬 넘어갈 때)
			MyPlayerController mpc = controller.GetComponent<MyPlayerController>();
			if (controller != null) 
				mpc.StopSendPacketCoroutine();
				
			// 사망 UI 세팅
			ClientManager.UI.gameSceneUI.DeathPanelSetting(true);
		}
	}
}