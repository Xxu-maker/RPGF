using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DPadSprite : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] Sprite originButtonSprite;
    [SerializeField] List<Sprite> highLightButtonSprites;
    private int current = 4;

    public void ChangeSprite(int i)
    {
        if(i != current)
        {
            current = i;
            image.sprite = highLightButtonSprites[i];
        }
    }

    public void ResetSprite()
    {
        if(current != 4)
        {
            current = 4;
            image.sprite = originButtonSprite;
        }
    }
}