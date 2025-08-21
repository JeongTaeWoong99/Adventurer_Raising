using UnityEngine;

// <상속구조>
// BaseController -> CommonController/MonsterController -> MyPlayerController 
public class CommonPlayerController : BaseController
{
	protected override void Init()
	{
		// 플레이어 세팅
		WorldObjectType = Define.WorldObject.CommonPlayer;
	}
	
	protected override void UpdateIdle()
	{
		base.UpdateIdle();

		dir = Vector3.zero;
	}
	
	protected override void UpdateRun()
	{
		base.UpdateRun();
		
		if(dir != Vector3.zero)
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
		
		if (!characterController)
			return;
		characterController.Move(dir.normalized * (infoState.MoveSpeed * Time.deltaTime));
	}

	protected override void UpdateDash()
	{
		base.UpdateDash();
		
		if(dir != Vector3.zero)
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
		
		if (!isAnimeMove || !characterController)
			return;
		characterController.Move(dir.normalized * (2f * infoState.MoveSpeed * Time.deltaTime));
	}
	
	protected override void UpdateAttack()
	{
		base.UpdateAttack();
		
		if(dir != Vector3.zero)
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 40 * Time.deltaTime);
		
		if (!isAnimeMove || !characterController)
			return;
		characterController.Move(dir.normalized * (infoState.MoveSpeed * Time.deltaTime)); 
	}
	
	protected override void UpdateSkill()
	{
		base.UpdateSkill();	
		
		if(dir != Vector3.zero)
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 40 * Time.deltaTime);
	}
	
	protected override void UpdateHit()
	{
		base.UpdateHit();
	
		if(dir != Vector3.zero)	// 회전(이동 방향과 반대로 바라보기 -> 히트 위치를 바라보도록)
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(-dir), 20 * Time.deltaTime);
	}
	
	protected override void UpdateDie()
	{
		base.UpdateDie();
	}
}
