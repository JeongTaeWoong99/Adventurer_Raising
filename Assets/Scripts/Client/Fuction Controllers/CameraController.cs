using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Define.CameraMode mode = Define.CameraMode.QuarterView;    // 카메라 각도 모드
    
    [SerializeField] Vector3 delta = new Vector3(0.0f, 6.0f, -5.0f);    
    
    [SerializeField] GameObject player = null;  // 추적하는 플레이어
    
    private void LateUpdate()
    {
        // mode 확인
        if (mode != Define.CameraMode.QuarterView) 
            return;
        
        // 플레이어가 유효한지 검사(Extension 함수)
        if (player.IsValid() == false)
            return;

        // 카메라의 위치 레이케스트에 Block이 들어오는지 체크
        // 있음
        if (Physics.Raycast(player.transform.position + Vector3.up * 2, delta, out var hit, delta.magnitude, 1 << (int)Define.Layer.Block))
        {
            float dist = (hit.point - player.transform.position).magnitude * 0.8f;
            transform.position = player.transform.position + delta.normalized * dist;
        }
        // 없음
        else
        {
            transform.position = player.transform.position + delta;
            transform.LookAt(player.transform);
        }
    }
    
    public void SetPlayer(GameObject player)
    {
        this.player = player;
    }
}
