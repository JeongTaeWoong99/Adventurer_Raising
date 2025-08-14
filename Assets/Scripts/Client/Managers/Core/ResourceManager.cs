using DamageNumbersPro;
using UnityEngine;

// ★ 직접 만들어 사용하는 Load, Instantiate, Destroy메서드는 ResourceManager에서 새로 만들어 사용하는 것을 나타내기 위해서,
// ★ 메서드 앞에 R_을 붙인다.
public class ResourceManager
{
    // ★ Resources 폴더에서 리소스를 로드하는 메서드
    // GameObject의 경우 오브젝트 풀링을 사용하여 최적화
    // where T : Object -> 로드하는 타입이 Object여야 한다.
    public T R_Load<T>(string path) where T : Object
    {
        // GameObject가 아닌 경우 일반 로드
        if (typeof(T) != typeof(GameObject)) 
            return Resources.Load<T>(path);
        
        // GameObject인 경우 오브젝트 풀링 사용
        string name = path;                     // Resources/Prefabs/UnityChan
        int index = name.LastIndexOf('/');      // 마지막 '/'의 인덱스 번호
        if (index >= 0)                         // 마지막 인덱스 번호 + 1부터 Substring하여, 이름 받아오기.
            name = name.Substring(index + 1);

        // 풀에서 name으로 원본 오브젝트 확인
        var go = ClientManager.Pool.GetOriginal(name);
        
        // 있음 -> 풀에서 리턴
        if (go != null)
            return go as T;
        
        // 없음 -> 풀에 없으면 새로 로드해서 리턴.
        return Resources.Load<T>(path);
    }

	// ★ 프리팹을 인스턴스화하는 메서드 (기본)
	public GameObject R_Instantiate(string path, Transform parent = null,Vector3 createPos = default)
    {
        // 프리팹 로드
        GameObject original = R_Load<GameObject>($"Prefabs/{path}");
        
        // 로드 실패
        if (original == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }

        GameObject go;
        // Poolable 컴포넌트가 있음 -> 오브젝트 풀링 사용
        if (original.GetComponent<Poolable>() != null)
        {
            go = ClientManager.Pool.Pop(original, parent).gameObject;
            go.transform.position += createPos;    // 생성 위치를 바꿔줘야 하면, 바꿔주기
            return go;
        }
        // Poolable 컴포넌트가 없음 -> 일반 인스턴스화
        else
        {
            go = Object.Instantiate(original, parent);
            go.name = original.name;
            go.transform.position += createPos;    // 생성 위치를 바꿔줘야 하면, 바꿔주기
            return go;
        }

    }

	// ★ 프리팹 인스턴스화 오버로드 (데미지 숫자 전달용)
	public GameObject R_Instantiate(string path, Transform parent, Vector3 createPos, int settingNumber)
	{
		// 프리팹 로드
		GameObject original = R_Load<GameObject>($"Prefabs/Number/{path}");

		// 로드 실패
		if (original == null)
		{
			Debug.Log($"Failed to load prefab : {path}");
			return null;
		}

		GameObject go;
		// Poolable 컴포넌트가 있음 -> 오브젝트 풀링 사용
		if (original.GetComponent<Poolable>() != null)
		{
			go = ClientManager.Pool.Pop(original, parent).gameObject;
			go.transform.position += createPos;
		}
		// Poolable 컴포넌트가 없음 -> 일반 인스턴스화
		else
		{
			go = Object.Instantiate(original, parent);
			go.name = original.name;
			go.transform.position += createPos;
		}

		// 리소스 후처리 (데미지 넘버는 인스턴스에서 설정)
		var damageNumberMesh = go.GetComponent<DamageNumberMesh>();
		if (damageNumberMesh)
			damageNumberMesh.number = settingNumber;

		return go;
	}

	// ★ 프리팹 인스턴스화 오버로드 (회전 전달용)
	public GameObject R_Instantiate(string path, Transform parent, Vector3 createPos, Quaternion rotation)
	{
		// 프리팹 로드
		GameObject original = R_Load<GameObject>($"Prefabs/{path}");
		
		// 로드 실패
		if (original == null)
		{
			Debug.Log($"Failed to load prefab : {path}");
			return null;
		}

		GameObject go;
		// Poolable 컴포넌트가 있음 -> 오브젝트 풀링 사용
		if (original.GetComponent<Poolable>() != null)
		{
			go = ClientManager.Pool.Pop(original, parent).gameObject;
			go.transform.rotation = rotation;      // 회전 적용
			go.transform.position += createPos;    // 위치 적용
			return go;
		}
		// Poolable 컴포넌트가 없음 -> 일반 인스턴스화
		else
		{
			go = Object.Instantiate(original, parent);
			go.name = original.name;
			go.transform.rotation = rotation;      // 회전 적용
			go.transform.position += createPos;    // 위치 적용
			return go;
		}
	}
    
    // 오브젝트 풀링을 위한, R_Destroy
    public void R_Destroy(GameObject go)
    {
        // 삭제할 오브젝트가 없음.
        if (go == null)
            return;
        
        // 오브젝트에 Poolable이 있는지 확인
        Poolable poolable = go.GetComponent<Poolable>();
        
        // Poolable 스크립트가 있음 -> 재사용 풀에 Push
        if (poolable != null)
        {
            ClientManager.Pool.Push(poolable);
            return;
        }
        
        // Poolable 스크립트가 없음 -> 삭제
        Object.Destroy(go);
    }
}
