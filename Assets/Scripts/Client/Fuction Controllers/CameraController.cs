using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Define.CameraMode mode   = Define.CameraMode.QuarterView; // 카메라 각도 모드
    [SerializeField] GameObject        player = null;                          // 추적하는 플레이어
    
    [Header("쿼터뷰 카메라")]
    [SerializeField] Vector3 delta  = new Vector3(0.0f, 6.0f, -5.0f);    
    
    // [Header("미니맵 카메라")]
    
    
    private void LateUpdate()
    {
        // 플레이어가 유효한지 검사(Extension 함수)
        if (player.IsValid() == false)
            return;
        
        // 화면 디스플레이 카메라 
        if (mode == Define.CameraMode.QuarterView)
        {
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
        // 미니맵 카메라
        else if (mode == Define.CameraMode.MiniMap)
        {   
            // Y깊이는 내껄 사용 // x z는 플레이어 머리 위
            transform.position = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        }
    }
    
    public void SetPlayer(GameObject player)
    {
        // 디스플레이 카메라 플레이어 설정
        this.player = player;
    }
}
