using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectItemR : RecallItem
{
    public RectTransform Select;
    
    public List<TextMeshProUGUI> Tmps;
    
    public void SetProvider(RecallConfig data)
    {
        foreach (var item in Tmps)
        {
            item.gameObject.SetActive(false);
        }
        
        for (int i = 0; i < data.Datas.Count; i++)
        {
            Tmps[i].text = data.Datas[i];
            Tmps[i].gameObject.SetActive(true);
            if (data.CurrentSelect == i)
            {
                Tmps[i].color = Color.red;
            }
            else
            {
                Tmps[i].color = Color.black;
            }
        }
        
    }
}

