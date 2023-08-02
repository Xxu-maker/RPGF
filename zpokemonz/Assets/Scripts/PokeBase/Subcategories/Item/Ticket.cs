using UnityEngine;
[CreateAssetMenu(menuName = "宝可梦道具/创建新票劵")]
public class Ticket : ItemBase
{
    [SerializeField] string sceneName;
    public string SceneName => sceneName;
}