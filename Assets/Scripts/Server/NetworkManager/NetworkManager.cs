using DummyClient;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
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
    ManagementPP   _managementPP   = new ManagementPP();
    OperationPP    _operationPP    = new OperationPP();
    DataBasePP     _dataBasePP     = new DataBasePP();
    NetworkStatsPP _networkStatsPP = new NetworkStatsPP();
    
    public static ManagementPP   Management  { get { return Instance._managementPP; } }
    public static OperationPP    Operation   { get { return Instance._operationPP; } }
    public static DataBasePP     DataBase    { get { return Instance._dataBasePP; } }
    public static NetworkStatsPP NetworkStats { get { return Instance._networkStatsPP; } }

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
        _networkStatsPP.Init();
    }

    private void Start()
    {
        // 서버 IP 주소로 직접 연결 (다른 컴퓨터에서 접속하기 위해)
        IPAddress  ipAddr   = IPAddress.Parse("211.237.180.244"); // 서버 컴퓨터의 실제 IP 주소
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);
        
        Connector connector = new Connector();
        connector.Connect(endPoint, () => { return _session; }, 1);
        
        // 핑 측정용 IP도 서버 IP와 동일하게 설정
        string pingIP = "211.237.180.244";
        _networkStatsPP.SetServerIP(pingIP);
    }

    private void Update()
    {
        // 모든 대기 중인 작업들을 실행
        List<IPacket> list = PacketQueueManager.Instance.PopAll();
        foreach(IPacket packet in list)
        {
            // 큐에 있는 패킷들을 처리
            PacketManager.Instance.HandlePacket(_session, packet);
        }
        
        // 네트워크 통계 업데이트
        _networkStatsPP.UpdateStats();
    }
    
    public void Send(ArraySegment<byte> sendBuff)
    {
        // 서버로 패킷 전송
        _session.Send(sendBuff);
    }
    
    public static void Clear()
    {
        Management.Clear();
        Operation.Clear();
        DataBase.Clear();
        NetworkStats.Clear();
    }
    
    // 게임 종료 시, 서버 연결 끊기
    private void OnApplicationQuit()
    {
        // 서버 연결 끊고, 나가기
        _session.Disconnect();
    }
}