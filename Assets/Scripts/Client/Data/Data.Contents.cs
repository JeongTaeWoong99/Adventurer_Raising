using System;
using System.Collections.Generic;
using System.Linq;

namespace Data
{
	// 캐릭터/몬스터/오브젝트 정보 데이터를 관리하는 클래스
	// JSON 파일에서 로드된 데이터를 Dictionary 형태로 변환
	[Serializable]
	public class CharacterInfoStateData : ILoader<string, CharacterInfoData>
	{
		// JSON 파일에서 로드된 캐릭터 정보 리스트
		public List<CharacterInfoData> characterInfos = new List<CharacterInfoData>();
		// serialNumber + "_" + level 조합을 Key로 사용
		public Dictionary<string, CharacterInfoData> MakeDict()
		{
			return characterInfos.ToDictionary(info => $"{info.serialNumber}_{info.level}");
		}
	}
}