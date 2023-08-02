using UnityEngine;
/// <summary>
/// 草地 用Composite Collider 2D作Trigger
/// </summary>
public class Grass : MonoBehaviour, PlayerTrigger
{
	[Header("可遇到宝可梦和等级")]
	[SerializeField] PokemonBase[] wild;
	[SerializeField] int minLevel;
	[SerializeField] int maxLevel;
	[SerializeField] int battlePercent = 10;

    public void OnPlayerTrigger()
    {
		//踩草声音
        AudioManager.Instance.WalkGrass();

		//几率战斗
		if(UnityEngine.Random.Range(1, 101) <= battlePercent)
		{
			GameManager.Instance.StartBattle
			(
				wild[Random.Range(0, wild.Length)],
			    Random.Range(minLevel, maxLevel)
			);
	    }
    }
}