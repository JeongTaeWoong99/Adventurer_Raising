using UnityEngine;

public class Util
{
    // 컴포넌트를 가져오거나 추가하는 메서드
    public static T GetOrAddComponent<T>(GameObject go) where T : Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();
        return component;
    }
    
    // GameObject의 자식 오브젝트를 찾는 메서드
    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform == null)
            return null;
        
        return transform.gameObject;
    }
    
    // 특정 타입의 컴포넌트를 가진 자식 오브젝트를 찾는 메서드
    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : Object
    {
        if (go == null)
            return null;
        
        // 자신의 자식들만 찾기(GetChild)
        if (recursive == false)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    T component = transform.GetComponent<T>();
                    if (component != null)
                        return component;
                }
            }
        }
        // 자식의 자식들까지 찾기(GetComponentsInChildren)
        else
        {
            foreach (T component in go.GetComponentsInChildren<T>(true))
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }

        return null;
    }
    
    public static Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;
            var result = FindDeepChild(child, name);
            if (result != null)
                return result;
        }
        return null;
    }
}
