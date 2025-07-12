using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
	public string           portalSceneChangeMmNumber = "UnKnown";
	public Define.SceneName PortalMoveSceneName       = Define.SceneName.Unknown;

	private void OnTriggerEnter(Collider other)
	{
		// 포탈은 내 캐릭터만 충돌하도록
		if (other.gameObject.CompareTag("Player") && other.gameObject == ClientManager.Game.MyPlayerGameObject)
		{
			// 포탈과 충돌한 오브젝트가 플레이어인 경우에만 씬 전환
			if (other.CompareTag("Player"))
			{
				// 클라이언트 클리어
				ClientManager.Clear();
				
				// manageUI 로딩 켜기
				ClientManager.UI.manageUI.LoadingPanelObject.SetActive(true);
				
				// 포탈의 개인 MmNumber 넣어주기
				ClientManager.Scene.SceneChangeMmNumber = portalSceneChangeMmNumber;
				ClientManager.Scene.PortalMoveSceneName = PortalMoveSceneName;
			
				// 방 떠나기
				C_EntityLeave leavePacket = new C_EntityLeave();
				NetworkManager.Instance.Send(leavePacket.Write());
				// ---> 내 캐릭터가 정상적으로 나가지면, ManagementPP의 EntityLeave를 통해, 씬 전환이 이루어짐....
			}
		}
	}
}