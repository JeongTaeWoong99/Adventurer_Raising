using System.Collections.Generic;

// C_ 패킷들 PacketQueueManager에 담아놨다가, NetworkManager에서 Update에서 순차적으로 차리
public class PacketQueueManager
{
    public static PacketQueueManager Instance { get; } = new PacketQueueManager();

    Queue<IPacket> _packetQueue = new Queue<IPacket>();
    object _lock = new object();

    public void Push(IPacket packet)
    {
        lock (_lock)
        {
            _packetQueue.Enqueue(packet);
        }
    }

    // 유니티에서는 외부 스레드에서 GameObject등의 접근을 막아두기 때문에
    // 실제로 handling하는 부분은 NetworkManager와 같이 MonoBehavior를 상속하는 곳에서 처리해야함
    public IPacket Pop()
    {
        lock (_lock)
        {
            if (_packetQueue.Count == 0)
                return null;

            return _packetQueue.Dequeue();
        }
    }

    public List<IPacket> PopAll()
    {
        List<IPacket> list = new List<IPacket>();
        lock (_lock)
        {
            while (_packetQueue.Count > 0)
                list.Add(_packetQueue.Dequeue());
        }

        return list;
    }
}
