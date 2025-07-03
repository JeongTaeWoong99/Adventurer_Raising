using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerEx
{
    public GameObject MyPlayerGameObject { get; set; }

    public List<GameObject> allPlayerList = new List<GameObject>(); // 모든 플레이어 리스트

    HashSet<GameObject> monsters = new HashSet<GameObject>();

    public Action<int> OnSpawnEvent;

    public GameObject Spawn(Define.WorldObject type, string path, Transform parent = null)
    {
        GameObject go = ClientManager.Resource.R_Instantiate(path, parent);

        switch (type)
        {
            case Define.WorldObject.MyPlayer:
                MyPlayerGameObject = go;
                allPlayerList.Add(go);
                break;
            case Define.WorldObject.CommonPlayer:
                //Debug.Log("다른 플레이어 스폰");
                allPlayerList.Add(go);
                break;
            case Define.WorldObject.Monster:
                monsters.Add(go);
                if (OnSpawnEvent != null)
                    OnSpawnEvent.Invoke(1); // 1마리 증가
                break;
        }

        return go;
    }

    public void Despawn(GameObject go)
    {
        Define.WorldObject type = Extension.GetWorldObjectType(go);

        switch (type)
        {
            case Define.WorldObject.MyPlayer:
            {

                if (MyPlayerGameObject == go)
                {
                    MyPlayerGameObject = null;
                    allPlayerList.Remove(go);
                }
            }
                break;
            case Define.WorldObject.CommonPlayer:
            {
                //Debug.Log("다른 플레이어 디스폰");
                allPlayerList.Remove(go);
            }
                break;
            case Define.WorldObject.Monster:
            {
                if (monsters.Contains(go))
                {
                    monsters.Remove(go);
                    if (OnSpawnEvent != null)
                        OnSpawnEvent.Invoke(-1); // 1마리 감소
                }
            }
                break;
        }

        ClientManager.Resource.R_Destroy(go);
    }

    public void Clear()
    {
        if (MyPlayerGameObject == null)
            return;

        // 주기적인 체크 종료
        MyPlayerController mpc = MyPlayerGameObject.GetComponent<MyPlayerController>();
        if (mpc != null) 
            mpc.StopSendPacketCoroutine();
    }
}
