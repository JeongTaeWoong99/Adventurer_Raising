using System;
using System.Collections.Generic;
using ServerCore;
using System.Text;

// ※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※
// ☆ 자동 완성 패킷(PacketFormat에서 주석 추가)
public enum PacketID
{
	C_RequestMakeId = 1001,
	S_MakeIdResult = 1002,
	C_RequestLogin = 1003,
	S_LoginResult = 1004,
	S_BroadcastEntityList = 1005,
	S_BroadcastEntityEnter = 1006,
	C_SceneChange = 1007,
	C_EntityLeave = 1008,
	S_BroadcastEntityLeave = 1009,
	S_BroadcastEntityInfoChange = 1010,
	C_EntityMove = 1011,
	S_BroadcastEntityMove = 1012,
	C_EntityRotation = 1013,
	S_BroadcastEntityRotation = 1014,
	C_EntityAnimation = 1015,
	S_BroadcastEntityAnimation = 1016,
	C_EntityDash = 1017,
	S_BroadcastEntityDash = 1018,
	C_EntityAttackAnimation = 1019,
	S_BroadcastEntityAttackAnimation = 1020,
	C_EntityAttackCheck = 1021,
	S_BroadcastEntityAttackResult = 1022,
}

public interface IPacket
{
	ushort Protocol { get; }
	void Read(ArraySegment<byte> segment);
	ArraySegment<byte> Write();
}

public class C_RequestMakeId : IPacket
{
	public string email;
	public string password;
	public string nickName;
	public string serialNumber;

	public ushort Protocol { get { return (ushort)PacketID.C_RequestMakeId; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		ushort emailLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.email = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, emailLen);
		count += emailLen;
		ushort passwordLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.password = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, passwordLen);
		count += passwordLen;
		ushort nickNameLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.nickName = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, nickNameLen);
		count += nickNameLen;
		ushort serialNumberLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.serialNumber = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, serialNumberLen);
		count += serialNumberLen;
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_RequestMakeId), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		ushort emailLen = (ushort)Encoding.Unicode.GetBytes(this.email, 0, this.email.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(emailLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += emailLen;
		ushort passwordLen = (ushort)Encoding.Unicode.GetBytes(this.password, 0, this.password.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(passwordLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += passwordLen;
		ushort nickNameLen = (ushort)Encoding.Unicode.GetBytes(this.nickName, 0, this.nickName.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(nickNameLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += nickNameLen;
		ushort serialNumberLen = (ushort)Encoding.Unicode.GetBytes(this.serialNumber, 0, this.serialNumber.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(serialNumberLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += serialNumberLen;

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}
public class S_MakeIdResult : IPacket
{
	public bool isSuccess;
	public string resultText;

	public ushort Protocol { get { return (ushort)PacketID.S_MakeIdResult; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.isSuccess = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		ushort resultTextLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.resultText = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, resultTextLen);
		count += resultTextLen;
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_MakeIdResult), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.isSuccess), 0, segment.Array, segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		ushort resultTextLen = (ushort)Encoding.Unicode.GetBytes(this.resultText, 0, this.resultText.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(resultTextLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += resultTextLen;

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}
public class C_RequestLogin : IPacket
{
	public string email;
	public string password;

	public ushort Protocol { get { return (ushort)PacketID.C_RequestLogin; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		ushort emailLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.email = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, emailLen);
		count += emailLen;
		ushort passwordLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.password = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, passwordLen);
		count += passwordLen;
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_RequestLogin), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		ushort emailLen = (ushort)Encoding.Unicode.GetBytes(this.email, 0, this.email.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(emailLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += emailLen;
		ushort passwordLen = (ushort)Encoding.Unicode.GetBytes(this.password, 0, this.password.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(passwordLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += passwordLen;

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}
public class S_LoginResult : IPacket
{
	public bool isSuccess;
	public string resultText;
	public string email;
	public string nickname;
	public string serialNumber;
	public string creationDate;
	public int currentLevel;
	public int currentHp;
	public int currentExp;
	public int currentGold;
	public string savedScene;
	public string savedPosition;

	public ushort Protocol { get { return (ushort)PacketID.S_LoginResult; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.isSuccess = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		ushort resultTextLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.resultText = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, resultTextLen);
		count += resultTextLen;
		ushort emailLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.email = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, emailLen);
		count += emailLen;
		ushort nicknameLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.nickname = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, nicknameLen);
		count += nicknameLen;
		ushort serialNumberLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.serialNumber = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, serialNumberLen);
		count += serialNumberLen;
		ushort creationDateLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.creationDate = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, creationDateLen);
		count += creationDateLen;
		this.currentLevel = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.currentHp = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.currentExp = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.currentGold = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		ushort savedSceneLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.savedScene = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, savedSceneLen);
		count += savedSceneLen;
		ushort savedPositionLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.savedPosition = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, savedPositionLen);
		count += savedPositionLen;
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_LoginResult), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.isSuccess), 0, segment.Array, segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		ushort resultTextLen = (ushort)Encoding.Unicode.GetBytes(this.resultText, 0, this.resultText.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(resultTextLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += resultTextLen;
		ushort emailLen = (ushort)Encoding.Unicode.GetBytes(this.email, 0, this.email.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(emailLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += emailLen;
		ushort nicknameLen = (ushort)Encoding.Unicode.GetBytes(this.nickname, 0, this.nickname.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(nicknameLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += nicknameLen;
		ushort serialNumberLen = (ushort)Encoding.Unicode.GetBytes(this.serialNumber, 0, this.serialNumber.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(serialNumberLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += serialNumberLen;
		ushort creationDateLen = (ushort)Encoding.Unicode.GetBytes(this.creationDate, 0, this.creationDate.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(creationDateLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += creationDateLen;
		Array.Copy(BitConverter.GetBytes(this.currentLevel), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.currentHp), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.currentExp), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.currentGold), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		ushort savedSceneLen = (ushort)Encoding.Unicode.GetBytes(this.savedScene, 0, this.savedScene.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(savedSceneLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += savedSceneLen;
		ushort savedPositionLen = (ushort)Encoding.Unicode.GetBytes(this.savedPosition, 0, this.savedPosition.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(savedPositionLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += savedPositionLen;

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}
public class S_BroadcastEntityList : IPacket
{
	public class Entity
	{
		public bool isSelf;
		public int ID;
		public int entityType;
		public string serialNumber;
		public string nickname;
		public int currentLevel;
		public int currentHp;
		public int currentExp;
		public int currentGold;
		public bool live;
		public bool invincibility;
		public float posX;
		public float posY;
		public float posZ;
		public float rotationY;
		public int animationID;
	
		public void Read(ArraySegment<byte> segment, ref ushort count)
		{
			this.isSelf = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
			count += sizeof(bool);
			this.ID = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			this.entityType = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			ushort serialNumberLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.serialNumber = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, serialNumberLen);
			count += serialNumberLen;
			ushort nicknameLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.nickname = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, nicknameLen);
			count += nicknameLen;
			this.currentLevel = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			this.currentHp = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			this.currentExp = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			this.currentGold = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			this.live = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
			count += sizeof(bool);
			this.invincibility = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
			count += sizeof(bool);
			this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
			this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
			this.posZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
			this.rotationY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
			this.animationID = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
		}
	
		public bool Write(ArraySegment<byte> segment, ref ushort count)
		{
			bool success = true;
			Array.Copy(BitConverter.GetBytes(this.isSelf), 0, segment.Array, segment.Offset + count, sizeof(bool));
			count += sizeof(bool);
			Array.Copy(BitConverter.GetBytes(this.ID), 0, segment.Array, segment.Offset + count, sizeof(int));
			count += sizeof(int);
			Array.Copy(BitConverter.GetBytes(this.entityType), 0, segment.Array, segment.Offset + count, sizeof(int));
			count += sizeof(int);
			ushort serialNumberLen = (ushort)Encoding.Unicode.GetBytes(this.serialNumber, 0, this.serialNumber.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(serialNumberLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += serialNumberLen;
			ushort nicknameLen = (ushort)Encoding.Unicode.GetBytes(this.nickname, 0, this.nickname.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(nicknameLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += nicknameLen;
			Array.Copy(BitConverter.GetBytes(this.currentLevel), 0, segment.Array, segment.Offset + count, sizeof(int));
			count += sizeof(int);
			Array.Copy(BitConverter.GetBytes(this.currentHp), 0, segment.Array, segment.Offset + count, sizeof(int));
			count += sizeof(int);
			Array.Copy(BitConverter.GetBytes(this.currentExp), 0, segment.Array, segment.Offset + count, sizeof(int));
			count += sizeof(int);
			Array.Copy(BitConverter.GetBytes(this.currentGold), 0, segment.Array, segment.Offset + count, sizeof(int));
			count += sizeof(int);
			Array.Copy(BitConverter.GetBytes(this.live), 0, segment.Array, segment.Offset + count, sizeof(bool));
			count += sizeof(bool);
			Array.Copy(BitConverter.GetBytes(this.invincibility), 0, segment.Array, segment.Offset + count, sizeof(bool));
			count += sizeof(bool);
			Array.Copy(BitConverter.GetBytes(this.posX), 0, segment.Array, segment.Offset + count, sizeof(float));
			count += sizeof(float);
			Array.Copy(BitConverter.GetBytes(this.posY), 0, segment.Array, segment.Offset + count, sizeof(float));
			count += sizeof(float);
			Array.Copy(BitConverter.GetBytes(this.posZ), 0, segment.Array, segment.Offset + count, sizeof(float));
			count += sizeof(float);
			Array.Copy(BitConverter.GetBytes(this.rotationY), 0, segment.Array, segment.Offset + count, sizeof(float));
			count += sizeof(float);
			Array.Copy(BitConverter.GetBytes(this.animationID), 0, segment.Array, segment.Offset + count, sizeof(int));
			count += sizeof(int);
			return success;
		}	
	}
	public List<Entity> entitys = new List<Entity>();

	public ushort Protocol { get { return (ushort)PacketID.S_BroadcastEntityList; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.entitys.Clear();
		ushort entityLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		for (int i = 0; i < entityLen; i++)
		{
			Entity entity = new Entity();
			entity.Read(segment, ref count);
			entitys.Add(entity);
		}
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastEntityList), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)this.entitys.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach (Entity entity in this.entitys)
			entity.Write(segment, ref count);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}
public class S_BroadcastEntityEnter : IPacket
{
	public int ID;
	public int entityType;
	public string nickname;
	public string serialNumber;
	public int currentLevel;
	public int currentHp;
	public bool live;
	public bool invincibility;
	public float posX;
	public float posY;
	public float posZ;
	public float rotationY;
	public int animationID;

	public ushort Protocol { get { return (ushort)PacketID.S_BroadcastEntityEnter; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.ID = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.entityType = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		ushort nicknameLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.nickname = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, nicknameLen);
		count += nicknameLen;
		ushort serialNumberLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.serialNumber = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, serialNumberLen);
		count += serialNumberLen;
		this.currentLevel = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.currentHp = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.live = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		this.invincibility = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.posZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.rotationY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.animationID = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastEntityEnter), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.ID), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.entityType), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		ushort nicknameLen = (ushort)Encoding.Unicode.GetBytes(this.nickname, 0, this.nickname.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(nicknameLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += nicknameLen;
		ushort serialNumberLen = (ushort)Encoding.Unicode.GetBytes(this.serialNumber, 0, this.serialNumber.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(serialNumberLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += serialNumberLen;
		Array.Copy(BitConverter.GetBytes(this.currentLevel), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.currentHp), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.live), 0, segment.Array, segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		Array.Copy(BitConverter.GetBytes(this.invincibility), 0, segment.Array, segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		Array.Copy(BitConverter.GetBytes(this.posX), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.posY), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.posZ), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.rotationY), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.animationID), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}
public class C_SceneChange : IPacket
{
	public string mmNumber;

	public ushort Protocol { get { return (ushort)PacketID.C_SceneChange; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		ushort mmNumberLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.mmNumber = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, mmNumberLen);
		count += mmNumberLen;
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_SceneChange), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		ushort mmNumberLen = (ushort)Encoding.Unicode.GetBytes(this.mmNumber, 0, this.mmNumber.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(mmNumberLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += mmNumberLen;

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}
public class C_EntityLeave : IPacket
{
	

	public ushort Protocol { get { return (ushort)PacketID.C_EntityLeave; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_EntityLeave), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}
public class S_BroadcastEntityLeave : IPacket
{
	public int ID;
	public int entityType;

	public ushort Protocol { get { return (ushort)PacketID.S_BroadcastEntityLeave; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.ID = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.entityType = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastEntityLeave), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.ID), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.entityType), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}
public class S_BroadcastEntityInfoChange : IPacket
{
	public int ID;
	public int entityType;
	public int currentExp;
	public int currentLevel;
	public int currentHp;
	public bool live;
	public bool invincibility;

	public ushort Protocol { get { return (ushort)PacketID.S_BroadcastEntityInfoChange; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.ID = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.entityType = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.currentExp = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.currentLevel = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.currentHp = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.live = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		this.invincibility = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastEntityInfoChange), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.ID), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.entityType), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.currentExp), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.currentLevel), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.currentHp), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.live), 0, segment.Array, segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		Array.Copy(BitConverter.GetBytes(this.invincibility), 0, segment.Array, segment.Offset + count, sizeof(bool));
		count += sizeof(bool);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}
public class C_EntityMove : IPacket
{
	public bool isInstantAction;
	public float posX;
	public float posY;
	public float posZ;

	public ushort Protocol { get { return (ushort)PacketID.C_EntityMove; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.isInstantAction = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.posZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_EntityMove), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.isInstantAction), 0, segment.Array, segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		Array.Copy(BitConverter.GetBytes(this.posX), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.posY), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.posZ), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}
public class S_BroadcastEntityMove : IPacket
{
	public int ID;
	public int entityType;
	public bool isInstantAction;
	public float posX;
	public float posY;
	public float posZ;

	public ushort Protocol { get { return (ushort)PacketID.S_BroadcastEntityMove; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.ID = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.entityType = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.isInstantAction = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.posZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastEntityMove), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.ID), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.entityType), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.isInstantAction), 0, segment.Array, segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		Array.Copy(BitConverter.GetBytes(this.posX), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.posY), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.posZ), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}
public class C_EntityRotation : IPacket
{
	public float rotationY;

	public ushort Protocol { get { return (ushort)PacketID.C_EntityRotation; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.rotationY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_EntityRotation), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.rotationY), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}
public class S_BroadcastEntityRotation : IPacket
{
	public int ID;
	public int entityType;
	public float rotationY;

	public ushort Protocol { get { return (ushort)PacketID.S_BroadcastEntityRotation; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.ID = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.entityType = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.rotationY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastEntityRotation), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.ID), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.entityType), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.rotationY), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}
public class C_EntityAnimation : IPacket
{
	public int animationID;

	public ushort Protocol { get { return (ushort)PacketID.C_EntityAnimation; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.animationID = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_EntityAnimation), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.animationID), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}
public class S_BroadcastEntityAnimation : IPacket
{
	public int ID;
	public int entityType;
	public int animationID;

	public ushort Protocol { get { return (ushort)PacketID.S_BroadcastEntityAnimation; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.ID = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.entityType = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.animationID = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastEntityAnimation), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.ID), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.entityType), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.animationID), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}
public class C_EntityDash : IPacket
{
	public int animationID;
	public float dirX;
	public float dirY;
	public float dirZ;

	public ushort Protocol { get { return (ushort)PacketID.C_EntityDash; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.animationID = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.dirX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.dirY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.dirZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_EntityDash), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.animationID), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.dirX), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.dirY), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.dirZ), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}
public class S_BroadcastEntityDash : IPacket
{
	public int ID;
	public int entityType;
	public int animationID;
	public float dirX;
	public float dirY;
	public float dirZ;

	public ushort Protocol { get { return (ushort)PacketID.S_BroadcastEntityDash; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.ID = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.entityType = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.animationID = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.dirX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.dirY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.dirZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastEntityDash), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.ID), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.entityType), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.animationID), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.dirX), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.dirY), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.dirZ), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}
public class C_EntityAttackAnimation : IPacket
{
	public int animationID;
	public int attackAnimeNumID;
	public float dirX;
	public float dirY;
	public float dirZ;

	public ushort Protocol { get { return (ushort)PacketID.C_EntityAttackAnimation; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.animationID = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.attackAnimeNumID = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.dirX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.dirY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.dirZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_EntityAttackAnimation), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.animationID), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.attackAnimeNumID), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.dirX), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.dirY), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.dirZ), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}
public class S_BroadcastEntityAttackAnimation : IPacket
{
	public int ID;
	public int entityType;
	public int animationID;
	public int attackAnimeNumID;
	public float dirX;
	public float dirY;
	public float dirZ;

	public ushort Protocol { get { return (ushort)PacketID.S_BroadcastEntityAttackAnimation; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.ID = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.entityType = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.animationID = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.attackAnimeNumID = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.dirX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.dirY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.dirZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastEntityAttackAnimation), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.ID), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.entityType), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.animationID), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.attackAnimeNumID), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.dirX), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.dirY), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.dirZ), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}
public class C_EntityAttackCheck : IPacket
{
	public float attackCenterX;
	public float attackCenterY;
	public float attackCenterZ;
	public float rotationY;
	public string attackSerial;

	public ushort Protocol { get { return (ushort)PacketID.C_EntityAttackCheck; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.attackCenterX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.attackCenterY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.attackCenterZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.rotationY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		ushort attackSerialLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.attackSerial = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, attackSerialLen);
		count += attackSerialLen;
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_EntityAttackCheck), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.attackCenterX), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.attackCenterY), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.attackCenterZ), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.rotationY), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		ushort attackSerialLen = (ushort)Encoding.Unicode.GetBytes(this.attackSerial, 0, this.attackSerial.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(attackSerialLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += attackSerialLen;

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}
public class S_BroadcastEntityAttackResult : IPacket
{
	public int attackerID;
	public int attackerEntityType;
	public int damage;
	public string effectSerial;
	public class Entity
	{
		public int targetID;
		public int targetEntityType;
		public float hitMoveDirX;
		public float hitMoveDirY;
		public float hitMoveDirZ;
	
		public void Read(ArraySegment<byte> segment, ref ushort count)
		{
			this.targetID = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			this.targetEntityType = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			this.hitMoveDirX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
			this.hitMoveDirY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
			this.hitMoveDirZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
		}
	
		public bool Write(ArraySegment<byte> segment, ref ushort count)
		{
			bool success = true;
			Array.Copy(BitConverter.GetBytes(this.targetID), 0, segment.Array, segment.Offset + count, sizeof(int));
			count += sizeof(int);
			Array.Copy(BitConverter.GetBytes(this.targetEntityType), 0, segment.Array, segment.Offset + count, sizeof(int));
			count += sizeof(int);
			Array.Copy(BitConverter.GetBytes(this.hitMoveDirX), 0, segment.Array, segment.Offset + count, sizeof(float));
			count += sizeof(float);
			Array.Copy(BitConverter.GetBytes(this.hitMoveDirY), 0, segment.Array, segment.Offset + count, sizeof(float));
			count += sizeof(float);
			Array.Copy(BitConverter.GetBytes(this.hitMoveDirZ), 0, segment.Array, segment.Offset + count, sizeof(float));
			count += sizeof(float);
			return success;
		}	
	}
	public List<Entity> entitys = new List<Entity>();

	public ushort Protocol { get { return (ushort)PacketID.S_BroadcastEntityAttackResult; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.attackerID = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.attackerEntityType = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.damage = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		ushort effectSerialLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.effectSerial = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, effectSerialLen);
		count += effectSerialLen;
		this.entitys.Clear();
		ushort entityLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		for (int i = 0; i < entityLen; i++)
		{
			Entity entity = new Entity();
			entity.Read(segment, ref count);
			entitys.Add(entity);
		}
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastEntityAttackResult), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.attackerID), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.attackerEntityType), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.damage), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		ushort effectSerialLen = (ushort)Encoding.Unicode.GetBytes(this.effectSerial, 0, this.effectSerial.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(effectSerialLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += effectSerialLen;
		Array.Copy(BitConverter.GetBytes((ushort)this.entitys.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach (Entity entity in this.entitys)
			entity.Write(segment, ref count);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

