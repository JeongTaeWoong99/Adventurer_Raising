using DummyClient;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

// 모든 매니저 스크립트들을 만들어, 사용하는 총 관리자
public class NetworkManager : MonoBehaviour
{
    // 싱글톤
    private static NetworkManager s_instance;
    public static NetworkManager Instance { get { Init(); return s_instance; }}
    
    public ServerSession _session = new ServerSession();
    
    public bool isNetworkInit_Complete = false; // 네트워크가 연결되면, Connector->ServerSession에서 ture로 변경
    
    [Header("PP = Packet Processing")]
    ManagementPP _managementPP = new ManagementPP();
    OperationPP  _operationPP  = new OperationPP();
    DataBasePP   _dataBasePP   = new DataBasePP();
    
    public static ManagementPP Management { get { return Instance._managementPP; } }
    public static OperationPP  Operation  { get { return Instance._operationPP; } }
    public static DataBasePP   DataBase   { get { return Instance._dataBasePP; } }

    // LoginScene 스크립트에서 NetworkManager가 존재하는지 확인하고 싶을 때 사용
    public static NetworkManager GetExistingInstance()
    {
        return s_instance;
    }

    static void Init()
    {
        if (s_instance != null) 
            return;
        
        GameObject go = GameObject.Find("@NetworkManager");
        if (go == null)
        {
            go = new GameObject { name = "@NetworkManager" };
            go.AddComponent<NetworkManager>();
        }
        
        DontDestroyOnLoad(go);
        s_instance = go.GetOrAddComponent<NetworkManager>();
    }
    
    private void Awake()
    {
        Init();
        
        _managementPP.Init();
        _operationPP.Init();
        _dataBasePP.Init();
    }

    private void Start()
    {
        // DNS (Domain Name System)
        string      host     = Dns.GetHostName();
        IPHostEntry ipHost   = Dns.GetHostEntry(host);
        IPAddress   ipAddr   = ipHost.AddressList[0];
        IPEndPoint  endPoint = new IPEndPoint(ipAddr, 7777);
        
        Connector connector = new Connector();
        connector.Connect(endPoint, () => { return _session; }, 1);
    }

    private void Update()
    {
        // 모든 대기 중인 작업들을 실행
        List<IPacket> list = PacketQueueManager.Instance.PopAll();
        foreach(IPacket packet in list)
            PacketManager.Instance.HandlePacket(_session, packet);
    }

    public void Send(ArraySegment<byte> sendBuff)
    {
        _session.Send(sendBuff);
    }
    
    public static void Clear()
    {
        Management.Clear();
        Operation.Clear();
        DataBase.Clear();
    }
    
    // 게임 종료 시, 서버 연결 끊기
    private void OnApplicationQuit()
    {
        // 서버 연결 끊고, 나가기
        _session.Disconnect();
    }
}