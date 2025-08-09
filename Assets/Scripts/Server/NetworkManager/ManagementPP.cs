using System.Collections.Generic;
using UnityEngine;

public class ManagementPP
{
    private static ManagementPP instance;
    
    public MyPlayerController myPlayerCon;
    
    public Dictionary<int, CommonPlayerController> playerDic  = new Dictionary<int, CommonPlayerController>();
    public Dictionary<int, ObjectController>       objectDic  = new Dictionary<int, ObjectController>();
    public Dictionary<int, MonsterController>      monsterDic = new Dictionary<int, MonsterController>();

    public void Init()
    {
    
    }

    public void Clear()
    {
        // 내 캐릭터 비우기
        myPlayerCon = null;
        
        // 딕셔너리 모두 비우기
        playerDic.Clear();
        objectDic.Clear();
        monsterDic.Clear();
    }

    // 내 플레이어 처음 입장 시, 모든 엔티티의 목록을 받아서, 업데이트
    public void EntityList(S_BroadcastEntityList p)
    {
        foreach (S_BroadcastEntityList.Entity entity in p.entitys)
        {
            // 내 플레이어
            if (entity.entityType == (int)Define.Layer.Player && entity.isSelf)
            {
                GameObject myPlayer = ClientManager.Game.Spawn(Define.WorldObject.MyPlayer, $"{entity.serialNumber}");
                Camera.main.gameObject.GetOrAddComponent<CameraController>().SetPlayer(myPlayer);                        // 카메라 따라가기 on
                myPlayerCon = myPlayer.GetOrAddComponent<MyPlayerController>();                                          // 내 캐릭터에는 MyPlayerController 붙여주기
                playerDic.Add(entity.ID, myPlayerCon);
                myPlayerCon.ID = entity.ID;
                myPlayerCon.characterController.enabled            = false;                                              // CharacterController 일시 비활성화
                myPlayerCon.characterController.transform.position = new Vector3(entity.posX, entity.posY, entity.posZ); // 절대 위치 설정
                myPlayerCon.characterController.enabled            = true;                                               // CharacterController 재활성화     
                myPlayerCon.transform.rotation = Quaternion.Euler(0, entity.rotationY, 0);
                myPlayerCon.Anime = (Define.Anime)System.Enum.Parse(typeof(Define.Anime),entity.animationID.ToString());

                // [Header("서버 리얼타임 데이터베이스를 통해 세팅")] => 공통
                myPlayerCon.playerInfoState.SerialNumber  = entity.serialNumber;
                myPlayerCon.playerInfoState.Live          = entity.live;
                myPlayerCon.playerInfoState.Invincibility = entity.invincibility;
                myPlayerCon.playerInfoState.NickName      = entity.nickname;
                myPlayerCon.playerInfoState.Hp            = entity.currentHp;
                myPlayerCon.playerInfoState.Level         = entity.currentLevel;
                myPlayerCon.playerInfoState.Exp           = entity.currentExp;
                myPlayerCon.playerInfoState.Gold          = entity.currentGold;    
                
                // 정보들을 바탕으로, 디스플레이
                myPlayerCon.playerInfoState.SetStat(entity.currentLevel);
            }
            // 다른 플레이어, 오브젝트, 몬스터
            else
            {
                Vector3 savedPos = new Vector3(entity.posX, entity.posY, entity.posZ);
                CreateNewEntity(entity.ID, entity.entityType, entity.nickname, entity.currentLevel,entity.currentHp,savedPos, entity.rotationY,entity.serialNumber,entity.live,entity.invincibility,entity.animationID);
            }
        }
        
        // 모든 세팅이 완료되면, 로딩 패널 없애주기
        ClientManager.UI.manageUI.LoadingPanelObject.gameObject.SetActive(false);
    }
    
    // 새로 들어온 엔티티 생성
    public void EntityEnter(S_BroadcastEntityEnter p)
    {
         //내 캐릭터 중복 생성 방지
         if (p.entityType == (int)Define.Layer.Player && myPlayerCon != null && p.ID == myPlayerCon.ID)
             return;
         
         Vector3 savedPos = new Vector3(p.posX, p.posY, p.posZ);
         CreateNewEntity(p.ID, p.entityType, p.nickname, p.currentLevel,p.currentHp,savedPos, p.rotationY,p.serialNumber,p.live,p.invincibility, p.animationID);
    }
    
    // 공통
    private void CreateNewEntity(int ID,int entityType, string nickname, int currentLevel, int currentHp, Vector3 savedPos, float rotationY,string serialNumber,bool live,bool invincibility, int animID)
    {
        GameObject          entityGameObject = null;
        CharacterController characterCon     = null;
        
        // 생성 및 기본 세팅
        if (entityType == (int)Define.Layer.Player)
        {
            entityGameObject = ClientManager.Game.Spawn(Define.WorldObject.CommonPlayer, $"{serialNumber}");
            CommonPlayerController commonPlayerCon = entityGameObject.GetOrAddComponent<CommonPlayerController>();
            commonPlayerCon.ID = ID;
            characterCon = commonPlayerCon.characterController;
            playerDic.Add(ID, commonPlayerCon);
            commonPlayerCon.Anime = (Define.Anime)System.Enum.Parse(typeof(Define.Anime),animID.ToString());
        }
        else if (entityType == (int)Define.Layer.Object)
        {
            entityGameObject = ClientManager.Game.Spawn(Define.WorldObject.Object, $"{serialNumber}");
            ObjectController objectCon = entityGameObject.GetOrAddComponent<ObjectController>();
            objectDic.Add(ID, objectCon);
            objectCon.ID = ID;
            characterCon = objectCon.characterController;
            objectCon.Anime = (Define.Anime)System.Enum.Parse(typeof(Define.Anime),animID.ToString());
        }
        else if (entityType == (int)Define.Layer.Monster)
        {
            entityGameObject = ClientManager.Game.Spawn(Define.WorldObject.Monster, $"{serialNumber}");
            MonsterController monsterCon = entityGameObject.GetOrAddComponent<MonsterController>();
            monsterDic.Add(ID, monsterCon);
            monsterCon.ID = ID;
            characterCon = monsterCon.characterController;
            monsterCon.Anime = (Define.Anime)System.Enum.Parse(typeof(Define.Anime),animID.ToString());
        }

        // 리얼타임 세팅 및 파이어스토어 세팅
        if (entityGameObject != null)
        {
            if (characterCon)
            {
                characterCon.enabled            = false;    // CharacterController 일시 비활성화
                characterCon.transform.position = savedPos; // 절대 위치 설정
                characterCon.enabled            = true;     // CharacterController 재활성화
            }
            else
                entityGameObject.transform.position = savedPos;
            
            entityGameObject.transform.rotation = Quaternion.Euler(0, rotationY, 0);
            
            // [Header("서버 리얼타임 데이터베이스를 통해 세팅")] => 공통
            InfoState cloneInfoState = entityGameObject.GetOrAddComponent<InfoState>();
            cloneInfoState.SerialNumber  = serialNumber;
            cloneInfoState.Live          = live;        
            cloneInfoState.Invincibility = invincibility;
            cloneInfoState.NickName      = nickname;
            cloneInfoState.Hp            = currentHp;
            cloneInfoState.Level         = currentLevel;
            
            // [Header("서버 리얼타임 데이터베이스를 통해 세팅")]
            if (entityType == (int)Define.Layer.Player)
            {
                PlayerInfoState clonePlayerInfoState = entityGameObject.GetOrAddComponent<PlayerInfoState>();
                clonePlayerInfoState.SetStat(currentLevel);
            }
            else if (entityType == (int)Define.Layer.Object)
            {
                ObjectAndMonsterInfoState cloneMonsterAndMonsterInfoState = entityGameObject.GetOrAddComponent<ObjectAndMonsterInfoState>();
                cloneMonsterAndMonsterInfoState.SetStat(serialNumber);
            }
            else if (entityType == (int)Define.Layer.Monster)
            {
                ObjectAndMonsterInfoState cloneObjectAndMonsterInfoState = entityGameObject.GetOrAddComponent<ObjectAndMonsterInfoState>();
                cloneObjectAndMonsterInfoState.SetStat(serialNumber);
            }
        }
    }
    
    // 엔티티 정보 변경
    public void EntityInfoChange(S_BroadcastEntityInfoChange p)
    {
        // 내 캐릭터 + 다른 캐릭터(해당 영역은 변경에 따른 결과를 서버에서 브로드케스트 해주는 메서드이다...)
        if (p.entityType == (int)Define.Layer.Player)
        {
            //Debug.Log("P - EntityInfoChange 들어옴");
            if (playerDic.TryGetValue(p.ID, out var player))
            {
                // 체력 변경(두개의 체력이 다름)
                if (p.currentHp != player.playerInfoState.Hp)
                {
                    //Debug.Log(p.ID + "의 체력 변경됨." + player.playerInfoState.Hp + " => " + p.currentHp);
                    player.playerInfoState.Hp = p.currentHp;
                }
                else if (p.currentHp == 0)
                {
                    //Debug.Log(p.ID + "의 체력은 0. 사망 처리");
                    player.playerInfoState.Hp = p.currentHp;
                }
                        
                // 레벨 변경(두개의 레벨이 다름)
                if (p.currentLevel != player.playerInfoState.Level)
                {
                    player.playerInfoState.Level = p.currentLevel;
                    player.playerInfoState.SetStat(player.playerInfoState.Level);
                }
                else if (p.currentLevel == 0)
                {
                    Debug.Log(p.ID + "의 레벨은 0. 확인 필요");
                }
                
                // Exp변경
                if (p.currentExp != player.playerInfoState.Exp)
                    player.playerInfoState.Exp = p.currentExp;
                        
                // 무적 변경(두개의 무적이 다름)
                if (p.invincibility != player.playerInfoState.Invincibility)
                {
                    //Debug.Log(p.ID + "의 무적 변경됨." + player.playerInfoState.Invincibility + " => " + p.invincibility);
                    player.playerInfoState.Invincibility = p.invincibility;
                }
                else
                {
                    //Debug.Log(p.invincibility + " => " +  player.playerInfoState.Invincibility);
                }
                
                // 라이브 변경
                if (p.live != player.playerInfoState.Live)
                {
                    //Debug.Log(p.ID + "의 생존 변경됨." + player.playerInfoState.Live + " => " + p.live);
                    player.playerInfoState.Live = p.live;
                }
                
            }
        }
        else if (p.entityType ==  (int)Define.Layer.Object)
        {
            //Debug.Log("O - EntityInfoChange 들어옴");
            if (objectDic.TryGetValue(p.ID, out var _object))
            {
                // 체력 변경(두개의 체력이 다름)
                if (p.currentHp != _object.objectAndMonsterInfoState.Hp)
                {
                   //Debug.Log(p.ID + "의 체력 변경됨." + _object.objectAndMonsterInfoState.Hp + " => " + p.currentHp);
                    _object.objectAndMonsterInfoState.Hp = p.currentHp;
                }
                else if (p.currentHp == 0)
                {
                    //Debug.Log(p.ID + "의 체력은 0. 사망 처리");
                    _object.objectAndMonsterInfoState.Hp = p.currentHp;
                }
                
                if (p.live != _object.objectAndMonsterInfoState.Live)
                {
                    //Debug.Log(p.ID + "의 생존 변경됨." + _object.objectAndMonsterInfoState.Live + " => " + p.live);
                    _object.objectAndMonsterInfoState.Live = p.live;
                }
            }
        }
        else if (p.entityType ==  (int)Define.Layer.Monster)
        {
            //Debug.Log("M - EntityInfoChange 들어옴");
            if (monsterDic.TryGetValue(p.ID, out var monster))
            {
                // 체력 변경(두개의 체력이 다름)
                if (p.currentHp != monster.objectAndMonsterInfoState.Hp)
                {
                    //Debug.Log(p.ID + "의 체력 변경됨." + monster.objectAndMonsterInfoState.Hp + " => " + p.currentHp);
                    monster.objectAndMonsterInfoState.Hp = p.currentHp;
                }
                else if (p.currentHp == 0)
                {
                    //Debug.Log(p.ID + "의 체력은 0. 사망 처리");
                    monster.objectAndMonsterInfoState.Hp = p.currentHp;
                }
                
                if (p.live != monster.objectAndMonsterInfoState.Live)
                {
                    //Debug.Log(p.ID + "의 생존 변경됨." + monster.objectAndMonsterInfoState.Live + " => " + p.live);
                    monster.objectAndMonsterInfoState.Live = p.live;
                }
            }
        }
    }
    
    // 엔티티 방에서 나감
    public void EntityLeave(S_BroadcastEntityLeave p)
    {
        if (p.entityType == (int)Define.Layer.Player)
        {
            // 내 키릭터가 다른 게임룸으로 명시적(포탈같은)로 나간 경우 (게임씬 -> 게임씬)
            if (myPlayerCon != null && myPlayerCon.ID == p.ID) // _myPlayer != null를 통해, 중복 방지
            {
                // 네트워크 클리어
                NetworkManager.Clear();
                // 씬 이동(포탈 OnTriggerEnter에서 저장된, 이름)
                Define.SceneName toScene = ClientManager.Scene.PortalMoveSceneName;
                ClientManager.Scene.LoadScene(toScene);
            }
            else
            {
                if(playerDic.TryGetValue(p.ID, out var player))
                {
                    // 현재는 바로 삭제해 버리는데, 나중에는 쉐이더 같은걸 넣어서, 해당 작업이 끝나면 삭제되도록 해보기
                    ClientManager.Dispatcher.DestroyOnMainThread(player.gameObject);
                    playerDic.Remove(p.ID);  
                }
            }
        }
        else if (p.entityType ==  (int)Define.Layer.Object)
        {
            if(objectDic.TryGetValue(p.ID, out var _object))
            {
                // 현재는 바로 삭제해 버리는데, 나중에는 쉐이더 같은걸 넣어서, 해당 작업이 끝나면 삭제되도록 해보기
                ClientManager.Dispatcher.DestroyOnMainThread(_object.gameObject);
                objectDic.Remove(p.ID);  
            }
        }
        else if (p.entityType ==  (int)Define.Layer.Monster)
        {
            if(monsterDic.TryGetValue(p.ID, out var moster))
            {
                // 현재는 바로 삭제해 버리는데, 나중에는 쉐이더 같은걸 넣어서, 해당 작업이 끝나면 삭제되도록 해보기
                ClientManager.Dispatcher.DestroyOnMainThread(moster.gameObject);
                monsterDic.Remove(p.ID);  
            }
        }
    }
}