using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        // 씬 타입 설정
        SceneType = Define.SceneType.Game;
        
        StartCoroutine(WaitForAsync());
    }
    
    private IEnumerator WaitForAsync()
    {
        // 인게임 UI 생성(생성하고, UI_GameScene 추출해서 gameSceneUI 넣어주기)
        if(ClientManager.UI.gameSceneUI == null)
            ClientManager.UI.ShowSceneUI<UI_Scene>("UI_GameScene");
        yield return new WaitUntil(() => ClientManager.UI.gameSceneUI); // gameSceneUI가 생성되는 것을 기다리기...
        ClientManager.UI.gameSceneUI.gameObject.SetActive(true); // 꺼져 있는 경우 켜주기                      
        ClientManager.UI.gameSceneUI.GameSecneUiSetting();       // 세팅 작업
        
        gameObject.GetOrAddComponent<CursorController>();                                                     // 커서 변경
        ClientManager.Sound.Play($"Sounds/BGM/{SceneManager.GetActiveScene().name}", Define.Sound.Bgm, 0.5f); // 씬 전용 BGM 재생
        
        // mmNumber를 보내서, 세팅 피드백 받기
        C_SceneChange sceneChange = new C_SceneChange {
            mmNumber = ClientManager.Scene.SceneChangeMmNumber
        };
        NetworkManager.Instance.Send(sceneChange.Write());
    }
    
    public override void Clear()
    {
        Debug.Log("GameScene 클리어!");
    }
}