using ServerCore;	
using System;
using System.Collections.Generic;
using System.Text;

// ※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※
// ☆ 자동 완성 패킷(PacketFormat에서 주석 추가)
// 구분을 위해서, ClientPacketManager.cs와 ServerPacketManager.cs로 스크립트 이름 다르게 함.
public class PacketManager
{
	// 싱글톤
	static PacketManager _instance = new PacketManager();
	public static PacketManager Instance { get { return _instance; } }

	PacketManager()
	{
		Register();
	}

	// ushort : Protocol ID
    // Action<PacketSessio, ArraySegment<byte>> : PacketSession, ArraySegment를 인자로 받는 특정 행동
    // 패킷을 생성하는 기능을 보관
	Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>> _makeFunc = new Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>>();
	
	// ushort : Protocol ID
    // Action<PacketSessio, Action<PacketSession, IPacket>> : PacketSession, IPacket을 인자로 받는 특정 행동
	Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new Dictionary<ushort, Action<PacketSession, IPacket>>();
	
	// ClientPacketManager.cs와 ServerPacketManager.cs의 등록해야 하는 펑션이 다름.(각각, 사용하는게 정해져 있음.)
	// ClientPacketManager는 PDL.xml에서 'S_'가 붇은 패킷 펑션+핸들 등록하고 사용.(Server에서 Client로 보내는 패킷)
	// ServerPacketManager은 PDL.xml에서 'C_'가 붇은 패킷 펑션+핸들 등록하고 사용.(Client에서 Server로 보내는 패킷)
	// 'S_'는 서버에서 클라 보내는 패킷 형식이니, ClientPacketManager에 서버 패킷을 처리하는 펑션+핸들을 등록을 해줘야 하는 것.
	// 'C_'는 클라에서 서보 보내는 패킷 형식이니, ServerPacketManager에 클라 패킷을 처리하는 펑션+핸들을 등록을 해줘야 하는 것.
	public void Register()
	{
		
		_makeFunc.Add((ushort)PacketID.S_MakeIdResult, MakePacket<S_MakeIdResult>);
		 _handler.Add((ushort)PacketID.S_MakeIdResult, PacketHandler.S_MakeIdResultHandler);

		_makeFunc.Add((ushort)PacketID.S_LoginResult, MakePacket<S_LoginResult>);
		 _handler.Add((ushort)PacketID.S_LoginResult, PacketHandler.S_LoginResultHandler);

		_makeFunc.Add((ushort)PacketID.S_BroadcastEntityList, MakePacket<S_BroadcastEntityList>);
		 _handler.Add((ushort)PacketID.S_BroadcastEntityList, PacketHandler.S_BroadcastEntityListHandler);

		_makeFunc.Add((ushort)PacketID.S_BroadcastEntityEnter, MakePacket<S_BroadcastEntityEnter>);
		 _handler.Add((ushort)PacketID.S_BroadcastEntityEnter, PacketHandler.S_BroadcastEntityEnterHandler);

		_makeFunc.Add((ushort)PacketID.S_BroadcastEntityLeave, MakePacket<S_BroadcastEntityLeave>);
		 _handler.Add((ushort)PacketID.S_BroadcastEntityLeave, PacketHandler.S_BroadcastEntityLeaveHandler);

		_makeFunc.Add((ushort)PacketID.S_BroadcastEntityInfoChange, MakePacket<S_BroadcastEntityInfoChange>);
		 _handler.Add((ushort)PacketID.S_BroadcastEntityInfoChange, PacketHandler.S_BroadcastEntityInfoChangeHandler);

		_makeFunc.Add((ushort)PacketID.S_BroadcastEntityMove, MakePacket<S_BroadcastEntityMove>);
		 _handler.Add((ushort)PacketID.S_BroadcastEntityMove, PacketHandler.S_BroadcastEntityMoveHandler);

		_makeFunc.Add((ushort)PacketID.S_BroadcastEntityRotation, MakePacket<S_BroadcastEntityRotation>);
		 _handler.Add((ushort)PacketID.S_BroadcastEntityRotation, PacketHandler.S_BroadcastEntityRotationHandler);

		_makeFunc.Add((ushort)PacketID.S_BroadcastEntityAnimation, MakePacket<S_BroadcastEntityAnimation>);
		 _handler.Add((ushort)PacketID.S_BroadcastEntityAnimation, PacketHandler.S_BroadcastEntityAnimationHandler);

		_makeFunc.Add((ushort)PacketID.S_BroadcastEntityDash, MakePacket<S_BroadcastEntityDash>);
		 _handler.Add((ushort)PacketID.S_BroadcastEntityDash, PacketHandler.S_BroadcastEntityDashHandler);

		_makeFunc.Add((ushort)PacketID.S_BroadcastEntityAttackAnimation, MakePacket<S_BroadcastEntityAttackAnimation>);
		 _handler.Add((ushort)PacketID.S_BroadcastEntityAttackAnimation, PacketHandler.S_BroadcastEntityAttackAnimationHandler);

		_makeFunc.Add((ushort)PacketID.S_BroadcastEntityAttackResult, MakePacket<S_BroadcastEntityAttackResult>);
		 _handler.Add((ushort)PacketID.S_BroadcastEntityAttackResult, PacketHandler.S_BroadcastEntityAttackResultHandler);

		_makeFunc.Add((ushort)PacketID.S_BroadcastChatting, MakePacket<S_BroadcastChatting>);
		 _handler.Add((ushort)PacketID.S_BroadcastChatting, PacketHandler.S_BroadcastChattingHandler);

	}
	
	// 받은 패킷의 확인
	// 클라에서 서버로 보낸 패킷이면, 서버의 ClientSession에서 처리
	// 서버에서 클라로 보낸 패킷이면, 클라의 ServerSession에서 처리
	public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer, Action<PacketSession, IPacket> onRecvCallback = null)
	{
		ushort count = 0;
		
		// 패킷의 전체 크기 읽기(현재 코드에서는 사용되지 않지만, 패킷의 유효성을 검사하는 데 사용될 수 있음.)
		ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
		count += 2;
		
		// 패킷 ID 읽기
		ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
		count += 2;
		
		// 패킷 ID에 해당하는 처리 함수 찾기
		// Register에서 미리 만들어 두기 때문에, TryGetValue에서 id로 빠르게 찾고
		// 찾은 만들어져 있는(=_makeFunc)를 func에다 넣어줌(=out func).
		Func<PacketSession, ArraySegment<byte>, IPacket> func = null;
		if (_makeFunc.TryGetValue(id, out func))
		{
			// 패킷 생성 및 처리
			IPacket packet = func.Invoke(session, buffer);
			
			// ★ onRecvCallback해아하는 작업이 있음.
			// ★ 즉, 실제 클라이언트(=유니티)에서는 메인쓰레드에서만 유니티 작업이 작동하니,
			// ★ PacketQueue.Instance.Push(p) 콜백을 통해, PacketQueue에 작업을 푸쉬해줌.
			// ★ 그리고, 푸쉬해둔, HandlePacket작업을 NetworkManger에서 AllPop하여 처리.
			if (onRecvCallback != null)
				onRecvCallback.Invoke(session, packet);
			// 바로 처리 가능하면, HandlePacket 바로 실행.(서버 or 더미클라)
			else
				HandlePacket(session, packet);
		}
	}
	
	// where T : IPacket, new() => T에 조건을 달아줌, IPacket을 상속받아야하고, new가 가능해야 한다.
    // IPacket을 상속한 Packet을 생성한 후 해당 패킷의 Protocol에 따라 해당하는 작업을 실행한다.
    // PacketHandler에 등록한 인터페이스를 호출
	T MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
	{
		T pkt = new T();
		pkt.Read(buffer);
		return pkt;
	}
	
	// 클라이언트 => Update 메인 쓰레드에서 확인하고, 처리.
	// 더미클라  => 바로 처리.
	// 서버     => 바로 처리.
	public void HandlePacket(PacketSession session, IPacket packet)
	{
		Action<PacketSession, IPacket> action = null;
		if (_handler.TryGetValue(packet.Protocol, out action))
			action.Invoke(session, packet);
	}
}