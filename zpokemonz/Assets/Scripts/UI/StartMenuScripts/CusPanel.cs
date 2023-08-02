using UnityEngine;
using UnityEngine.SceneManagement;
public class CusPanel : MonoBehaviour
{
    public RectTransform crossTransform;
    public RectTransform aTransform;
    public void BackMenu()
    {
        MyData.ctrl = crossTransform.position;
        MyData.aButton = aTransform.position;
        MyData.change = true;
        SceneManager.LoadScene(0);
    }
}