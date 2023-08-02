using UnityEngine;
[CreateAssetMenu(menuName = "宝可梦道具/创建新精灵球")]
public class PokeBall : ItemBase
{
    [Header("球类")]
    [SerializeField] float ballRate;//捕捉概率
    [SerializeField] PokeBallType pokeBallType;
    [SerializeField] Sprite[] throwSprite;
    public Sprite[] ThrowSprite => throwSprite;

    public float CatchRate(int times, Pokemon playerPokemon, Pokemon enemyPokemon)
    {
        switch(pokeBallType)
		{
            case PokeBallType.Times:

                if(base.ID == 21)//计时球
			    {
			    	if(times > 10)
			    	{
			    		return 4f;
			    	}
			    	else
			    	{
			    		return ballRate + 0.3f * times;
			    	}
			    }
			    else//26先机球
			    {
			    	if(times == 1)
			    	{
                        return 5f;
			    	}
			    }

            break;

			//等级相关
			case PokeBallType.Level:

			    if(base.ID == 19)//巢穴球
				{
					if(enemyPokemon.Level <= 30)
					{
						//若目标的等级小于等于30，则捕获率×[ 8-0.2×（目标等级-1）]，否则捕获率×1。
                        return 8 - 0.2f * (enemyPokemon.Level - 1);
					}
				}
				else//等级球
				{
					float Q = playerPokemon.Level / enemyPokemon.Level;
					if(1 < Q && Q <= 2)
					{
						return 2f;
					}
					else if(2 < Q && Q <= 4)
					{
						return 4f;
					}
					else if(Q > 4)
					{
						return 8f;
					}
				}

			break;

			//速度相关
			case PokeBallType.Speed:

			    if(playerPokemon.Speed >= 100)//速度球
				{
					return 4f;
				}

			break;

			//属性相关
			case PokeBallType.Type:

			    if(enemyPokemon.Base.Type1 == PokemonType.水 || enemyPokemon.Base.Type1 == PokemonType.虫
				|| enemyPokemon.Base.Type2 == PokemonType.水 || enemyPokemon.Base.Type2 == PokemonType.虫)
				{
					return 3.5f;
				}
				/*if(pb.ID == 17)//捕网球
				{
					//
				}
				else
				{
					//
				}*/

			break;

			//重量相关
			case PokeBallType.Weight:

			    //float weight = enemyPokemon.Base.;
				/*if(0 < weight && weight <= 99.9)
				{
					catchRate = -20f;
				}
				else if(200 <= weight && weight <= 299.9)
				{
					catchRate = 20f;
				}
				else if(300 <= weight && weight <= 399.9)
				{
					catchRate = 30f;
				}
				else if(weight >= 400)
				{
					catchRate = 40f;
				}
				*/

			break;

			//性别相关
			case PokeBallType.Sex:

			    //if()//己方和敌方不同性别

			break;

			//场景相关
			case PokeBallType.Scene:

			    int id = base.ID;
                if(id == 24)//黑暗球
				{
					//if()//场景为山洞
					int hh = System.DateTime.Now.Hour;
					if(hh > 19 && hh <= 23)
                    {
                        return 3f;
                    }
				}
				else if(id == 28)//梦境球
				{
					//连入之森100%
				}
				else if(id == 18)//潜水球
				{
					//水下、冲浪、垂钓，捕获率×3.5。
					return 3.5f;
				}

			break;

		}
        return ballRate == 0? 1f : ballRate;
    }
}
public enum PokeBallType
{
    Normal, Type, Times, Speed, Weight, Level, Sex, Scene, Stone, Cure
}