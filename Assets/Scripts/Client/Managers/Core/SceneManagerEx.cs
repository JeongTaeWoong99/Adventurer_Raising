using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using Object = UnityEngine.Object;

public class SceneManagerEx
{
    public BaseScene CurrentScene => Object.FindFirstObjectByType<BaseScene>();

    public string           SceneChangeMmNumber = "UnKnown";
    public Define.SceneName PortalMoveSceneName = Define.SceneName.Unknown; // 포탈로 이동하는 씬 이름((ManagementPP의 EntityLeave에서 참조할 수 있게....))
    
    public async void LoadScene(Define.SceneName loadSceneName)
    {
        try
        {
            // 1초 대기(모든 작업들이 완료되는 것을 기다리기. 나중에는 연출로 시간 간격 조절)
            await Task.Delay(1000);
            
            SceneManager.LoadScene(loadSceneName.ToString());
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
    
    public void Clear()
    {
        CurrentScene.Clear(); // CurrentScene씬 비우기
    }
}