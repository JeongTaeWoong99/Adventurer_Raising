using UnityEngine;

// 스킬 이펙트 단순 이동 (패킷 기반 초기화)
public class SkillMove : MonoBehaviour
{
	[SerializeField] private float _speed = 0f;
	[SerializeField] public bool lookAtDirection = true; // 스킬이 날아가는 방향을 바라볼지 여부
	private Vector3 _direction;

	public void InitializeFromPacket(S_BroadcastEntityAttackEffectCreate p, Transform attacker)
	{
		if (p == null) return;
		_speed = p.moveSpeed;
		_direction = (attacker != null ? attacker.forward : transform.forward);
		
		// 스킬이 날아가는 방향을 바라보도록 회전 (옵션에 따라)
		if (lookAtDirection && _direction != Vector3.zero)
		{
			transform.rotation = Quaternion.LookRotation(_direction);
			//Debug.Log("화살의 _direction은 " + _direction);
		}
	}
	
	// 오버로드: 공격자 트랜스폼을 넘기지 않는 경우, 현재 forward 사용
	public void InitializeFromPacket(S_BroadcastEntityAttackEffectCreate p)
	{
		if (p == null) return;
		_speed = p.moveSpeed;
		_direction = transform.forward;
		
		// 스킬이 날아가는 방향을 바라보도록 회전 (옵션에 따라)
		if (lookAtDirection && _direction != Vector3.zero)
		{
			transform.rotation = Quaternion.LookRotation(_direction);
			//Debug.Log("화살의 _direction은 " + _direction);
		}
	}

	private void Update()
	{
		if (_speed == 0f)
			return;

		transform.position += _direction * (_speed * Time.deltaTime);
		
		Debug.Log("디렉션 더하기 = " + _direction);
	}
}


