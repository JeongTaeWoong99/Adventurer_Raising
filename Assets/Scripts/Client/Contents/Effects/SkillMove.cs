using UnityEngine;

// 스킬 이펙트 단순 이동 (패킷 기반 초기화)
public class SkillMove : MonoBehaviour
{
	[SerializeField] private float _speed = 0f;
	private Vector3 _direction;

	public void InitializeFromPacket(S_BroadcastEntitySkillCreate p, Transform attacker)
	{
		if (p == null) return;
		_speed = p.moveSpeed;
		_direction = (attacker != null ? attacker.forward : transform.forward);
	}

	// 오버로드: 공격자 트랜스폼을 넘기지 않는 경우, 현재 forward 사용
	public void InitializeFromPacket(S_BroadcastEntitySkillCreate p)
	{
		if (p == null) return;
		_speed = p.moveSpeed;
		_direction = transform.forward;
	}

	private void Update()
	{
		if (_speed == 0f)
			return;

		transform.position += _direction * _speed * Time.deltaTime;
	}
}


