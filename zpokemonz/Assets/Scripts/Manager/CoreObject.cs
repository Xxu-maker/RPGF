using UnityEngine;
public class CoreObject : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    public void MainMenu()
    {
        Destroy(gameObject);
    }
}