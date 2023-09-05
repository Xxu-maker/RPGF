using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogItemR : RecallItem
{
    public TextMeshProUGUI Content;
        
    public void SetProvider(RecallConfig data)
    {
        Content.text = data.Datas[0];
    }
}
