using UnityEngine;

public class UI_TestInven : UI_Scene
{
    enum GameObjects
    {
        GridPanel
    }

    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(GameObjects));
        
        GameObject gridPanel = Get<GameObject>((int)GameObjects.GridPanel);
        foreach (Transform child in gridPanel.transform)
            ClientManager.Resource.R_Destroy(child.gameObject);

        // 실제 인벤토리 정보를 참고해서, 띄워주기
        for (int i = 0; i < 8; i++)
        {
            GameObject item = ClientManager.UI.MakeSubItem<UI_Inven_Item>(gridPanel.transform).gameObject;            
            UI_Inven_Item invenItem = item.GetOrAddComponent<UI_Inven_Item>();
            invenItem.SetInfo($"집행검{i}번");
        }
    }
}