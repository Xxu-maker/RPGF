using UnityEngine;
using UnityEngine.UI;
public class BookUI : MonoBehaviour
{
    [SerializeField] Text numberText;
    public void SetData(int index)
    {
        numberText.text = index.ToString();
    }
}