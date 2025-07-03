using UnityEngine;

public class ObjectController : BaseController
{
	protected override void Init()
	{
		// 오브젝트 세팅
		WorldObjectType = Define.WorldObject.Object;
	}
	
	protected override void UpdateIdle()
	{				
		base.UpdateIdle();
	}
	
	protected override void UpdateRun()
	{
		base.UpdateRun();
	
		if(dir != Vector3.zero)
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);	// 로테이션 회전
	}
	
	protected override void UpdateHit()
	{
		base.UpdateHit();
	
		if(dir != Vector3.zero)	// 회전(이동 방향과 반대로 바라보기 -> 히트 위치를 바라보도록)
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(-dir), 40 * Time.deltaTime);
	}
}