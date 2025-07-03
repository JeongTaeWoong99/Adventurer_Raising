using System.Collections.Generic;
using UnityEngine;

public class PoolManager
{
    Transform _root;
	Dictionary<string, Pool> _pool = new Dictionary<string, Pool>();    // string을 이용해, 각각의 풀을 찾는다...
    
    // 초기화
    public void Init()
    {
        if (_root != null) 
            return;
        
        _root = new GameObject { name = "@Pool_Root" }.transform;
        Object.DontDestroyOnLoad(_root);
    }
    
    // 풀에서 관리하는 오브젝트는 만들 때, 5개를 기본으로 만들어서 사용...
    public void CreatePool(GameObject original, int count = 5)
    {
        Pool pool = new Pool();             // 새로운 풀 생성
        pool.Init(original, count);         // 각각의 이름에 맞춰서, 그룹 생성 + 미리 5개 정도 만들어 둠.
        pool.Root.parent = _root;           // _root에 위치
        
        _pool.Add(original.name, pool);     // 딕셔너리에 등록
    }
    
    // 풀에 넣기
    public void Push(Poolable poolable)
    {
        string name = poolable.gameObject.name;
        if (_pool.ContainsKey(name) == false)
        {
            Object.Destroy(poolable.gameObject);
            return;
        }

        _pool[name].Push(poolable);
    }
    
    // 꺼내오기
    public Poolable Pop(GameObject original, Transform parent = null)
    {
        // 딕셔너리 풀에 없음 -> 새로 만들기
        if (_pool.ContainsKey(original.name) == false)
            CreatePool(original);
        
        // 딕셔너리에서 꺼내오기
        return _pool[original.name].Pop(parent);
    }
    
    public GameObject GetOriginal(string name)
    {
        return _pool.ContainsKey(name) == false ? null : _pool[name].Original;
    }

    public void Clear()
    {
        foreach (Transform child in _root)
            Object.Destroy(child.gameObject);

        _pool.Clear();
    }
}

// PoolManager는 여러개의 Pool을 가짐...
// 플레이어 풀, 몬스터 풀, 총알 풀 등등등
internal class Pool
{
    public GameObject Original { get; private set; }
    public Transform Root { get; set; }

    Stack<Poolable> _poolStack = new Stack<Poolable>();
        
    public void Init(GameObject original, int count = 5)
    {
        Original = original;
        Root = new GameObject().transform;
        Root.name = $"{original.name}_Root";
            
        // 만들고 -> 넣어주기
        for (int i = 0; i < count; i++)
            Push(Create());
    }

    Poolable Create()
    {
        GameObject go = Object.Instantiate(Original);
        go.name = Original.name;
        return go.GetOrAddComponent<Poolable>();
    }

    public void Push(Poolable poolable)
    {
        if (poolable == null)
            return;

        poolable.transform.parent = Root;
        poolable.gameObject.SetActive(false);
        poolable.IsUsing = false;

        _poolStack.Push(poolable);
    }

    public Poolable Pop(Transform parent)
    {
        Poolable poolable;

        if (_poolStack.Count > 0)
            poolable = _poolStack.Pop();
        else
            poolable = Create();

        poolable.gameObject.SetActive(true);

        // parent가 null이거나 destroyed된 경우 안전하게 처리
        // if (parent == null || parent == false) // destroyed된 오브젝트는 false로 평가됨
        // {
        //     // DontDestroyOnLoad 해제 용도 - CurrentScene이 null인 경우도 체크
        //     var currentScene = ClientManager.Scene?.CurrentScene;
        //     if (currentScene != null)
        //         poolable.transform.parent = currentScene.transform;
        //     else
        //         poolable.transform.parent = null; // 안전하게 null로 설정
        // }
        if (parent == null || parent == false)
        {
            poolable.transform.parent = null;
        }
        else
        {
            poolable.transform.parent = parent;
        }

        poolable.IsUsing = true;
        return poolable;
    }
}