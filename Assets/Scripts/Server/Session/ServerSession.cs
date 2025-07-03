using System;
using System.Net;
using ServerCore;
using UnityEngine;

namespace DummyClient
{
	// <클라이언트에서 서버 일을 하는 대리자>
	// 	⇒ (Dummy)Client.ServerSession에서 Packet을 수신  ⇒  Call OnRecvPacket                        
	// 	⇒ Call ServerSession에서.RecvPacket             ⇒  Call PacketManager.Instance.OnRecvPacket
	//  ⇒ Call PacketManager.MakePacket                ⇒  Call PacketHandler에서 해당하는 처리
	
	// <다른 메서드들이 분리된 이유>
	// OnConnected    : 연결 시 초기화 로직이 서버/클라이언트마다 다름
	// OnSend         : 데이터 전송 방식이 서버/클라이언트마다 다를 수 있음
	// OnDisconnected : 연결 해제 시 정리 작업이 서버/클라이언트마다 다름
	public class ServerSession : PacketSession
	{
		public override void OnConnected(EndPoint endPoint)
		{
			Debug.Log($"OnConnected!");
			NetworkManager.Instance.isNetworkInit_Complete = true;
		}
 
		public override void OnDisconnected(EndPoint endPoint)
		{
			// Debug.Log($"OnDisconnected!");
		}
		
		// Session을 상속하고 있는 PacketSession에서 사용
		public override void OnRecvPacket(ArraySegment<byte> buffer)
		{
			// PacketManager == ClientPacketManager.cs
			// ★ 콜백 == null으로 넘겨주고, PacketHandler처리함.(서버 or 더미클라에서는 따로 처리할 필요 없음.)
			//Debug.Log("OnRecvPacket...");
			PacketManager.Instance.OnRecvPacket(this, buffer);
		}

		public override void OnSend(int numOfBytes)
		{
			//Debug.Log($"OnSend : {numOfBytes}");
			//Console.WriteLine($"Transferred bytes: {numOfBytes}");
		}
	}
}

