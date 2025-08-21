using System;
using System.Collections.Generic;
using System.IO;
using Firebase;
using Firebase.Firestore;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Serialization;

#region 데이터 클래스 정의
[Serializable]
public class CharacterInfoData
{
	public string serialNumber;
	public string level;	
	public string nickName;
	public string needExp;
	public string dropExp;
	public string invincibility;
	public string maxHp;
	public string body_Size;
	public string moveSpeed;
	public string findRadius;
	public string normalAttackDamage;
	public string normalAttackRange;
	public string hitLength;
}
[Serializable]
public class AttackInfoData
{
	public string attackSerial;
	public string name;
	public string coolTime;
	public string image;
}

[Serializable]
public class CharacterInfoList 
{ public List<CharacterInfoData> characterInfos; }
[Serializable]
public class AttackInfoList 
{ public List<AttackInfoData> attackInfos; }
#endregion

public class FirestoreManager
{
	[Header("파이어스토어 데이터베이스")] 
	private FirebaseFirestore firestore;

	public async Task Init(FirebaseApp customAppData)
	{
		// 커스텀 정보로 변경
		firestore = FirebaseFirestore.GetInstance(customAppData);
            
		// 병렬 실행
		var tasks = new List<Task>
		{
			LoadAndSaveCollectionToJson<CharacterInfoData, CharacterInfoList>("characterInfos", "CharacterInfoData.json"),
			LoadAndSaveCollectionToJson<AttackInfoData,    AttackInfoList>   ("attackInfos",    "AttackInfoData.json"),
		};
		await Task.WhenAll(tasks);
	}
	
	// Firestore 컬렉션 데이터를 Json으로 저장
	private async Task LoadAndSaveCollectionToJson<TItem, TList>(string collectionName, string outputFileName) where TItem : new() where TList : new()
	{
	    CollectionReference colRef   = firestore.Collection(collectionName);
	    QuerySnapshot       snapshot = await colRef.GetSnapshotAsync();
	
	    List<TItem> items = new List<TItem>();
	    foreach (DocumentSnapshot doc in snapshot.Documents)
	    {
	        if (!doc.Exists)
	            continue;
	
	        Dictionary<string, object> data = doc.ToDictionary();
	        TItem item = new TItem();
	
	        // 🔧 각 필드를 직접 매핑
	        foreach (var kv in data)
	        {
	            var field = typeof(TItem).GetField(kv.Key);
	            if (field != null && kv.Value != null)
	            {
	                string val = kv.Value.ToString();
	                if (!string.IsNullOrEmpty(val))
	                    field.SetValue(item, val);
	            }
	        }
	
	        items.Add(item);
	    }
	
	    // List<T>를 래핑하는 클래스에 넣기
	    object wrapper   = new TList();
	    var    listField = typeof(TList).GetFields()[0]; // 첫 번째 public field
	    listField.SetValue(wrapper, items);
	
	    // JSON 변환
	    string jsonData = JsonUtility.ToJson(wrapper, true);
	    string path     = Path.Combine(Application.dataPath, $"Data/{outputFileName}");
	
	    // 폴더 경로 추출
	    // 폴더가 없으면 생성
	    string directory = Path.GetDirectoryName(path);
	    if (!Directory.Exists(directory))
	        Directory.CreateDirectory(directory);
	
	    // 파일 저장
	    try
	    {
	        File.WriteAllText(path, jsonData);
	        //Debug.Log($"Firestore '{collectionName}' → JSON 저장 완료: {path}");
	    }
	    catch (Exception e)
	    {
	        Debug.LogError($"JSON 저장 실패: {e.Message}");
	    }
	}
}
