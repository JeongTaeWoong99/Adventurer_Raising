using System.Collections.Generic;
using UnityEngine;

public class ObjectAndMonsterInfoState : InfoState
{
	[Header("오브젝트와 몬스터 - 서버에서 받은 serialNumber를 통해, 제이슨 정보로 디스플레이 세팅")]
	[SerializeField] protected int findRadius;
	[SerializeField] protected int dropExp;      
	
	public int FindRadius
	{
		get => findRadius;
		set => findRadius = value;
	}
	
	public int DropExp
	{
		get => dropExp;
		set => dropExp = value;
	}
	
	// 레벨에 따른 스탯을 설정
	public void SetStat(string serialNumber)
	{
		var    dict = ClientManager.Data.CharacterInfoDict;
		string key = $"{serialNumber}_1";
		CharacterInfoData info = dict[key];

		// 공통
		maxHp              = int.Parse(info.maxHp);
		normalAttackDamage = int.Parse(info.normalAttackDamage);
		moveSpeed          = float.Parse(info.moveSpeed);
		//normalAttackRange  = Extension.ParseVector3(info.normalAttackRange);
		// 오 + 몬 전용
		findRadius         = int.Parse(info.findRadius);
		dropExp            = int.Parse(info.dropExp);

		// 체력바 만들기(+ 무적이 아닌 오브젝트만)
		if (gameObject.GetComponentInChildren<UI_State>() == null && !Invincibility)
			ClientManager.UI.MakeWorldSpaceUI<UI_State>(transform);
	}
	
	public override void OnAttacked(GameObject attacker, Vector3 attackCenterVec, int damage, string effectSerial)
	{
		base.OnAttacked(attacker, attackCenterVec,damage,effectSerial);
		
		// 몬스터는 Decimals(White) 넘버
		ClientManager.Resource.R_Instantiate("Decimals(White)", null,transform.position + Vector3.up * 4f, damage);
	}
}