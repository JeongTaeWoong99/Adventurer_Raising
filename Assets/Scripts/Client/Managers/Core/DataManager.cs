using System.Collections.Generic;
using System.IO;
using UnityEngine;

// JSON 데이터를 Dictionary 형태로 변환하기 위한 인터페이스
// Key와 Value 타입을 제네릭으로 받아 다양한 데이터 타입에 대응
public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}

// 게임의 데이터를 관리하는 매니저 클래스
// JSON 파일에서 데이터를 로드하고 Dictionary 형태로 저장하여 관리
public class DataManager
{
    // Key: serialNumber_level (예: "P000_1", "M001_1")
    // Value: CharacterInfoData (플레이어/몬스터/오브젝트 통합 정보)
    public Dictionary<string, CharacterInfoData> CharacterInfoDict { get; private set; } = new Dictionary<string, CharacterInfoData>();
    public Dictionary<string, AttackInfoData>    AttackInfoDict    { get; private set; } = new Dictionary<string, AttackInfoData>();
    
    // DataManager 초기화 함수
    public void Init()
    {
        // JSON 파일에서 characterInfos 정보를 로드하여 Dictionary에 저장
        var loaderCharacterInfoStateData = LoadJson<Data.CharacterInfoStateData, string, CharacterInfoData>("CharacterInfoData.json");
        CharacterInfoDict = loaderCharacterInfoStateData.MakeDict();
        var loaderAttackInfoStateData    = LoadJson<Data.AttackInfoStateData,    string, AttackInfoData>   ("AttackInfoData.json");
        AttackInfoDict = loaderAttackInfoStateData.MakeDict();
    }
    
    // JSON 파일을 로드하여 지정된 타입으로 변환하는 함수
    // Loader : 변환할 데이터 타입
    // Key    : Dictionary의 Key 타입
    // Value  : Dictionary의 Value 타입
    // path   : JSON 파일 경로
    private Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        string filePath = Path.Combine(Application.dataPath, $"Data/{path}");
        string json     = File.ReadAllText(filePath);
        return JsonUtility.FromJson<Loader>(json);
    }
}