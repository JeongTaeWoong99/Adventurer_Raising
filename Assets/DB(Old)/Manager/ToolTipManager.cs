using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToolTipManager : MonoBehaviour
{
    public static ToolTipManager instance;

    // 툴팁 화면 밖으로 나가는걸 방지
    private RectTransform tooltipRect;
    private float         halfwidth;
    
    // 툴팁
    public GameObject      tooltipObject;
    public TextMeshProUGUI nameText;

    private void Awake()
    {
        instance = this;
        
        tooltipRect = tooltipObject.GetComponent<RectTransform>();
    }

    private void Start()
    {
        tooltipObject.SetActive(false);
        
        halfwidth = GetComponent<CanvasScaler>().referenceResolution.x * 0.5f;    
    }

    private void Update()
    {
        // 툴팁이 활성화되어 있을 때만 위치 업데이트
        if (tooltipObject.activeInHierarchy)
        {
            tooltipObject.transform.position = Input.mousePosition;

            if (tooltipRect.anchoredPosition.x + tooltipRect.sizeDelta.x > halfwidth)
            {
                tooltipRect.pivot = new Vector2(1, 0.5f);
                tooltipObject.transform.position -= new Vector3(10, 0, 0);
            }
            else
            {
                tooltipRect.pivot = new Vector2(0, 0.5f);
                tooltipObject.transform.position += new Vector3(10, 0, 0);
            }
        }
    }

    public void SetupToolTip(string name)
    {
        nameText.text = name;
    }
}
