using System;

namespace ServerCore
{
	public class RecvBuffer
	{
		// [r][][w][][][][][][][]
		//
		// 버퍼 초기 상태:
		// [][][][][][][][][][]
		//
		// 40바이트 수신:
		// [데이터40바이트][][][][][][][][][]
		//
		// 60바이트 추가 수신:
		// [데이터100바이트][][][][][][][][][]
		//
		// 패킷 처리 후:
		// [처리된100바이트][][][][][][][][][]
		ArraySegment<byte> _buffer; // 실제 데이터가 저장되는 버퍼
		int _readPos;               // 읽기 시작 위치
		int _writePos;              // 쓰기 시작 위치

		public RecvBuffer(int bufferSize)
		{
			_buffer = new ArraySegment<byte>(new byte[bufferSize], 0, bufferSize);
		}

		public int DataSize { get { return _writePos - _readPos; } }		// 실제 데이터 크기
		public int FreeSize { get { return _buffer.Count - _writePos; } }   // 남은 공간

		public ArraySegment<byte> ReadSegment
		{
			get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _readPos, DataSize); }
		}

		public ArraySegment<byte> WriteSegment
		{
			get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _writePos, FreeSize); }
		}

		public void Clean()
		{
			int dataSize = DataSize;
			if (dataSize == 0)
			{
				// 남은 데이터가 없으면 복사하지 않고 커서 위치만 리셋
				_readPos = _writePos = 0;
			}
			else
			{
				// 남은 찌끄레기가 있으면 시작 위치로 복사
				Array.Copy(_buffer.Array, _buffer.Offset + _readPos, _buffer.Array, _buffer.Offset, dataSize);
				_readPos = 0;
				_writePos = dataSize;
			}
		}

		public bool OnRead(int numOfBytes)
		{
			if (numOfBytes > DataSize)
				return false;

			_readPos += numOfBytes;
			return true;
		}

		// 첫 번째 수신: 40바이트
		// _recvBuffer.OnWrite(40);을 통해, writePos가 40으로 이동
		// 버퍼 상태: [40바이트 데이터][남은 공간]

		// 두 번째 수신: 60바이트
		// _recvBuffer.OnWrite(60);을 통해, writePos가 100으로 이동
		// 버퍼 상태: [100바이트 데이터][남은 공간]
		public bool OnWrite(int numOfBytes)
		{
			if (numOfBytes > FreeSize)
				return false;

			_writePos += numOfBytes;
			return true;
		}
	}
}