using UnityEngine;

public class MonsterController : BaseController
{
	protected override void Init()
    {
	    // 몬스터 세팅
		WorldObjectType = Define.WorldObject.Monster;
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
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);	// 로테이션 회전
		
		if (!characterController)
			return;	
		characterController.Move(dir.normalized * (infoState.MoveSpeed * Time.deltaTime));
	}
	
	protected override void UpdateAttack()
	{
		base.UpdateAttack();
		
		if(dir != Vector3.zero)
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
	}
	
	protected override void UpdateHit()
	{
		base.UpdateHit();
	
		if(dir != Vector3.zero)	// 회전(이동 방향과 반대로 바라보기 -> 히트 위치를 바라보도록)
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(-dir), 40 * Time.deltaTime);
	}
}
