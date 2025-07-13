using UnityEngine;

namespace Client.Managers.Contents
{
	public class FPSManager
	{
		private float fpsTimer = 0f;
		
		public void Init()
		{
			// 백그라운드 실행 설정 (멀티플레이어 테스트용)
			Application.runInBackground = true;      // 백그라운드에서도 실행 유지
			Application.targetFrameRate = 144;       // 목표 프레임률 고정 (포커스 상태)
			QualitySettings.vSyncCount  = 0;         // V-Sync 비활성화 (프레임률 제한 방지)
		}
	
		// 플레이어의 마우스와 키 입력을 매니저에서 관리.
		// 인풋 조건을 만족할 때에만, _input.OnUpdate();가 실행된다.
		public void OnUpdate()
		{
			// FPS 디버그 (5초마다)
			fpsTimer += Time.deltaTime;
			if (fpsTimer >= 5f)
			{
				fpsTimer = 0f;
				float currentFPS = 1.0f / Time.deltaTime;
				//Debug.Log($"[FPS Debug] 실제 FPS: {currentFPS:F1} | 목표: {Application.targetFrameRate} | V-Sync: {QualitySettings.vSyncCount}");
			}
		}
		
		public void Clear()
		{
			
		}
	}
}