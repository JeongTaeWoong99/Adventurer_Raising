using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

// <Connector는 클라이언트와의 연결만 하는데, ServerCore 폴더에 있는 이유>
// 클라이언트 전용 기능 : Connector가 클라이언트 전용 기능이라면 DummyClient 폴더에 위치하는 것이 맞습니다.
// 공용 네트워크 기능  : Connector가 서버와 클라이언트 모두에서 사용될 수 있는 기능이라면 ServerCore에 위치하는 것이 더 적절할 수 있습니다.
namespace ServerCore
{
	public class Connector
	{
		Func<Session> _sessionFactory;
		
		public void Connect(IPEndPoint endPoint, Func<Session> sessionFactory, int count = 1)
		{
			for (int i = 0; i < count; i++)
			{
				// 휴대폰 설정
				Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
				_sessionFactory = sessionFactory;

				SocketAsyncEventArgs args = new SocketAsyncEventArgs();
				args.Completed      += OnConnectCompleted;
				args.RemoteEndPoint  = endPoint;
				args.UserToken       = socket;

				RegisterConnect(args);
			}
		}

		void RegisterConnect(SocketAsyncEventArgs args)
		{
			Socket socket = args.UserToken as Socket;
			if (socket == null)
				return;
			
			bool pending = socket.ConnectAsync(args);
			if (pending == false)
				OnConnectCompleted(null, args);
		}
		
		void OnConnectCompleted(object sender, SocketAsyncEventArgs args)
		{
			if (args.SocketError == SocketError.Success)
			{
				Session session = _sessionFactory.Invoke();
				session.Start(args.ConnectSocket);			// 서버와 진행할 작업 등록
				session.OnConnected(args.RemoteEndPoint);	// 잘 연결 되었다고, 콘솔에 출력. // ServerSession <- PacketSession <- Session
				// ServerSession에서 출력해줌.
			}
			else
			{
				Debug.Log($"OnConnectCompleted Fail: {args.SocketError}");
			}
		}
	}
}