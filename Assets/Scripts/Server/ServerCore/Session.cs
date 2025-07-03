using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ServerCore
{
	// 받아온 데이터를 파싱하는 부분.
	// <상속구조> ☆ ServerSession/ClientSession이 각각의 Session을 가지고 있음!!
	// ServerSession/ClientSession <- PacketSession <- Session
	// <패킷 처리의 공통 로직>
	// OnRecv는 모든 세션(서버/클라이언트)에서 동일한 방식으로 패킷을 처리해야 함
	// 패킷의 크기 확인, 헤더 파싱, 데이터 분리 등의 로직이 공통적
	// 이 로직을 각 세션마다 구현하면 코드 중복이 발생
	// <패킷 구조의 일관성>
	// 모든 패킷은 [size(2)][packetId(2)][data...] 형식을 따름.
	// 이 구조를 파싱하는 로직은 모든 세션에서 동일해야 함.
	// PacketSession에서 이 공통 로직을 구현하고, 실제 패킷 처리(OnRecvPacket)는 추상 메서드로 남김.
	public abstract class PacketSession : Session
	{
		public static readonly int HeaderSize = 2;

		// [size(2)][packetId(2)][ ... ][size(2)][packetId(2)][ ... ]
		// sealed은 더이상 상속하지 않을 것 이라는 의미
		public sealed override int OnRecv(ArraySegment<byte> buffer)
		{
			int processLen  = 0;
			int packetCount = 0;

			while (true)
			{
				// 최소한 헤더는 파싱할 수 있는지 확인.
				if (buffer.Count < HeaderSize)
					break;

				// 패킷이 완전체로 도착했는지 확인.
				// 완전체가 되지 않았으면, break...
				ushort dataSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
				if (buffer.Count < dataSize)
					break;
			
				// 여기까지 왔으면 패킷 조립 가능.
				// ★ ServerSession의 override void OnRecvPacket에서 실행됨.
				OnRecvPacket(new ArraySegment<byte>(buffer.Array, buffer.Offset, dataSize));
				packetCount++;	// 조립된 패킷 수
				
				// 리턴값 변경
				processLen += dataSize;
				
				// 처리한 패킷 버퍼 사이즈만큼 빼준 후, 다시 루프
				buffer = new ArraySegment<byte>(buffer.Array, buffer.Offset + dataSize, buffer.Count - dataSize);
			}
			
			// 조립된 패킷 수 확인...
			if (packetCount > 1)
			{
				//Console.WriteLine($"패킷 모아보내기 : {packetCount}");
			}
			
			// 처리 결과 리턴
			return processLen;
		}
		
		// PacketSession를 상속하고 있는, ServerSession의 OnRecvPacket를 사용하기 위해, 선언
		public abstract void OnRecvPacket(ArraySegment<byte> buffer);
	}

	public abstract class Session
	{
		Socket _socket;
		int    _disconnected = 0;
		
		RecvBuffer _recvBuffer = new RecvBuffer(65535); // 64KB 버퍼 생성

		object _lock = new object();  // 하나의 스레드만 sendQueue 및 _pendingList에 접근 할 수 있도록 보호함.
		
		Queue<ArraySegment<byte>> _sendQueue   = new Queue<ArraySegment<byte>>(); // 서버에 송신할 데이터를 모아두는 큐
		List<ArraySegment<byte>>  _pendingList = new List<ArraySegment<byte>>();  // 진행중인, 작업 리스트
		
		SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs(); // 비동기 송신 이벤트 생성
		SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs(); // 비동기 수신 이벤트 생성

		public abstract void OnConnected(EndPoint endPoint);		
		public abstract int  OnRecv(ArraySegment<byte> buffer);
		public abstract void OnSend(int numOfBytes);
		public abstract void OnDisconnected(EndPoint endPoint);
		
		// 커넥트 완료후, 초기화
		public void Start(Socket socket)
		{
			_socket = socket;

			// 소켓이 연결되면, recv와 send에서 할 작업을 등록 해준다.
			_recvArgs.Completed += OnRecvCompleted;	// recv는 계속 뺑뺑 돌며 받아주기.
			_sendArgs.Completed += OnSendCompleted; // send는 한번에 모아서 보내기.
			
			// 서버에서 정보를 계속 받아줘야 하니, 바로 뺑뺑이 시작.
			RegisterRecv();
		}
		
		public void Disconnect()
		{
			if (Interlocked.Exchange(ref _disconnected, 1) == 1)
				return;

			OnDisconnected(_socket.RemoteEndPoint);
			_socket.Shutdown(SocketShutdown.Both);
			_socket.Close();
			Clear();
		}
		
		#region Send구역(모아보내기)
		// 여러 개의 ArraySegment<byte>(=데이터 버퍼의 조각들)을 네트워크 송신 큐에 추가하고 비동기 송신 작업을 등록.
		public void Send(List<ArraySegment<byte>> sendBuffList)
		{
			if (sendBuffList.Count == 0)
				return;

			lock (_lock)
			{
				foreach (ArraySegment<byte> sendBuff in sendBuffList)
					_sendQueue.Enqueue(sendBuff);

				if (_pendingList.Count == 0)
					RegisterSend();
			}
		}

		// 단일 데이터 세그먼트를 처리하여 송신 큐에 추가.
		// 단일 데이터 세그먼트기 때문에, 여기서는 입장같은 경우에 사용.
		public void Send(ArraySegment<byte> sendBuff)
		{
			lock (_lock)
			{
				_sendQueue.Enqueue(sendBuff);
				
				// 현재 대기 중인(_pendingList) 송신 작업이 없음 -> RegisterSend() 실행.
				// ☆ RegisterSend의 _socket.SendAsync(_sendArgs);은 비동기 작업이라, 이때, _lock이 해제됨. ☆
				// 현재 대기 중인(_pendingList) 송신 작업이 있음 -> 큐에 등록만 하고, _lock을 나감.
				if (_pendingList.Count == 0)
					RegisterSend();
			}
		}
		
		void RegisterSend()
		{
			if (_disconnected == 1)
				return;

			// 송신 큐에서 데이터를 꺼내, 대기 목록(_pendingList)에 추가.
			while (_sendQueue.Count > 0)
			{
				ArraySegment<byte> buff = _sendQueue.Dequeue();
				_pendingList.Add(buff);
			}
			
			// 대기 중인 데이터를 버퍼에 추가
			_sendArgs.BufferList = _pendingList;

			try
			{
				// 비동기 송신 요청 -> 비동기 작업 시작...!
				bool pending = _socket.SendAsync(_sendArgs);
				// 비동기 작업이 즉시 완료된 경우       -> 바로 실행 해줌.
				// 비동기 작업이 즉시 완료되지 않은 경우 -> 완료되면, 알아서 콜백 실행. ☆ 이때, _lock 해제 ☆
				if (pending == false)
					OnSendCompleted(null, _sendArgs);
			}
			catch (Exception e)
			{
				Console.WriteLine($"RegisterSend Failed {e}");
			}
		}

		void OnSendCompleted(object sender, SocketAsyncEventArgs args)
		{
			lock (_lock)
			{
				if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
				{
					try
					{
						_sendArgs.BufferList = null;
						_pendingList.Clear();

						OnSend(_sendArgs.BytesTransferred);
						
						// 송신 큐에 아직 남아 있는 데이터가 있으면 다시 송신 작업을 등록.
						if (_sendQueue.Count > 0)
							RegisterSend();
					}
					catch (Exception e)
					{
						Console.WriteLine($"OnSendCompleted Failed {e}");
					}
				}
				else
				{
					Disconnect();
				}
			}
		}
		
		// 소켓을 Close하고 SendQueue와 PendingList를 정리
		void Clear()
		{
			lock (_lock)
			{
				_sendQueue.Clear();
				_pendingList.Clear();
			}
		}		
		#endregion
		
		/// <summary>
		/// /////////
		/// </summary>

		#region Recv구역(무한 루프)
		void RegisterRecv()
		{
			if (_disconnected == 1)
				return;

			_recvBuffer.Clean();											   // 버퍼 정리
			ArraySegment<byte> segment = _recvBuffer.WriteSegment;			   // 쓰기 가능한 영역 가져오기
			_recvArgs.SetBuffer(segment.Array, segment.Offset, segment.Count); // 버퍼 설정
			
			try
			{
				bool pending = _socket.ReceiveAsync(_recvArgs); // 비동기 수신 시작
				if (pending == false)
					OnRecvCompleted(null, _recvArgs);
			}
			catch (Exception e)
			{
				Console.WriteLine($"RegisterRecv Failed {e}");
			}
		}
		
		void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
		{
			// args에 데이터가 성공적으로 들어옴.
			if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
			{
				try
				{
					// Write 커서를 이동시켜 새 데이터를 쓸 공간 확보.
					// 실패하면 버퍼 오버플로우 위험 → 연결 종료
					if (_recvBuffer.OnWrite(args.BytesTransferred) == false)
					{
						Disconnect();
						return;
					}
					
					// 컨텐츠 쪽으로 데이터를 넘겨주고 얼마나 처리했는지 받는다.
					// ReadSegment로 읽을 수 있는 데이터 영역을 가져옴.
					// OnRecv에서 패킷을 파싱하고 처리.
					// 처리한 데이터 크기(processLen)를 반환 -> 잘못된 크기면 연결 종료
					int processLen = OnRecv(_recvBuffer.ReadSegment);
					if (processLen < 0 || _recvBuffer.DataSize < processLen)
					{
						Disconnect();
						return;
					}
					
					//  Read 커서 이동해, 다음 패킷 처리를 위한 준비.
					// 실패하면 -> 연결 종료
					if (_recvBuffer.OnRead(processLen) == false)
					{
						Disconnect();
						return;
					}

					RegisterRecv();
				}
				catch (Exception e)
				{
					Console.WriteLine($"OnRecvCompleted Failed {e}");
				}
			}
			else
			{
				Disconnect();
			}
		}
		#endregion
	}
}
