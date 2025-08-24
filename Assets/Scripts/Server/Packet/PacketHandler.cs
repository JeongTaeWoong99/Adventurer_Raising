using ServerCore;

class PacketHandler
{
    # region ===== DB 영역 =====
    
    // 아이디 생성 요청
    public static void S_MakeIdResultHandler(PacketSession session, IPacket packet)
    {
        S_MakeIdResult pkt = packet as S_MakeIdResult;
        ClientManager.Dispatcher.Push(() => { NetworkManager.DataBase.MakeIdResult(pkt); });
    }
	
	// 로그인 확인 요청
    public static void S_LoginResultHandler(PacketSession session, IPacket packet)
    {
        S_LoginResult pkt = packet as S_LoginResult;
        ClientManager.Dispatcher.Push(() => { NetworkManager.DataBase.LoginResult(pkt); });
    }
    
    #endregion
 
    # region ===== 관리 영역 =====
    
    // 플레이어 리스트
    public static void S_BroadcastEntityListHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastEntityList pkt = packet as S_BroadcastEntityList;
        ClientManager.Dispatcher.Push(() => { NetworkManager.Management.EntityList(pkt); });
    }
    
    // 플레이어 입장
    public static void S_BroadcastEntityEnterHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastEntityEnter pkt = packet as S_BroadcastEntityEnter;
        ClientManager.Dispatcher.Push(() => { NetworkManager.Management.EntityEnter(pkt); });
    }
    
    // 플레이어 나감
    public static void S_BroadcastEntityLeaveHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastEntityLeave pkt = packet as S_BroadcastEntityLeave;
        ClientManager.Dispatcher.Push(() => { NetworkManager.Management.EntityLeave(pkt); });
    }
    
    // 플레이어 정보 변경 적용
    public static void S_BroadcastEntityInfoChangeHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastEntityInfoChange pkt = packet as S_BroadcastEntityInfoChange;
        ClientManager.Dispatcher.Push(() => { NetworkManager.Management.EntityInfoChange(pkt); });
    }
    #endregion

    # region ===== 조작 영역 =====
    // 공통 이동
    public static void S_BroadcastEntityMoveHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastEntityMove pkt = packet as S_BroadcastEntityMove;
        ClientManager.Dispatcher.Push(() => { NetworkManager.Operation.EntityMove(pkt); });
    }
    
    // 공통 회전을 처리
    public static void S_BroadcastEntityRotationHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastEntityRotation pkt = packet as S_BroadcastEntityRotation;
        ClientManager.Dispatcher.Push(() => { NetworkManager.Operation.EntityRotation(pkt); });
    }
    
    // 공통 애니메이션
    public static void S_BroadcastEntityAnimationHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastEntityAnimation pkt = packet as S_BroadcastEntityAnimation;
        ClientManager.Dispatcher.Push(() => { NetworkManager.Operation.EntityAnimation(pkt); });
    }
    
    // 플레이어 대쉬
    public static void S_BroadcastEntityDashHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastEntityDash pkt = packet as S_BroadcastEntityDash;
        ClientManager.Dispatcher.Push(() => { NetworkManager.Operation.Dash(pkt); });
    }
    
    // 공격 애니메이션
    public static void S_BroadcastEntityAttackAnimationHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastEntityAttackAnimation pkt = packet as S_BroadcastEntityAttackAnimation;
        ClientManager.Dispatcher.Push(() => { NetworkManager.Operation.EntityAttackAnimation(pkt); });
    }
    
    // 공격 결과
    public static void S_BroadcastEntityAttackResultHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastEntityAttackResult pkt = packet as S_BroadcastEntityAttackResult;
        ClientManager.Dispatcher.Push(() => { NetworkManager.Operation.EntityAttackResult(pkt); });
    }
    
    // 채팅
    public static void S_BroadcastChattingHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastChatting pkt = packet as S_BroadcastChatting;
        ClientManager.Dispatcher.Push(() => { NetworkManager.Operation.Chatting(pkt); });
    }
    
    // 스킬 그래픽 생성 
    public static void S_BroadcastEntityAttackEffectCreateHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastEntityAttackEffectCreate pkt = packet as S_BroadcastEntityAttackEffectCreate;
        ClientManager.Dispatcher.Push(() => { NetworkManager.Operation.SkillCreate(pkt); });
    }
    # endregion
}