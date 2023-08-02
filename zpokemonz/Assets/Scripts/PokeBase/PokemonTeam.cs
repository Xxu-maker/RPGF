using UnityEngine;
/// <summary>
/// 玩家宝可梦仓库
/// </summary>
public class PokemonTeam : MonoBehaviour
{
    [SerializeField] Pokemon[]  box0 = new Pokemon[6];
    [SerializeField] Pokemon[]  box1, box2, box3, box4, box5,
                                box6, box7, box8, box9, box10,
                                box11, box12, box13, box14, box15, box16 = new Pokemon[60];
    [SerializeField] Pokemon[]  foster = new Pokemon[4];
    [SerializeField] string[]   boxName = new string[16];
    [SerializeField] Pokemon nullPokemon;
    private Pokemon[][] boxes;
    private int currentBattlePokemonNum;
    public int currentBox = 1;
    public int CurrentBox => currentBox;
    public void SetCurrentValue(int i) => currentBox = i;

    /// <summary>
    /// 背包里的宝可梦
    /// </summary>
    public ref Pokemon[] Pokemons => ref box0;

    /// <summary>
    /// 寄存处的宝可梦
    /// </summary>
    public ref Pokemon[] Foster => ref foster;

    public string[] BoxName => boxName;

    public int CurrentBattlePokemon => currentBattlePokemonNum;//战斗出场位

    private void Awake()
    {
        ReSetBox();
        currentBox = 1;
        int boxesLength = boxes.Length;
        for(int i = 0; i < boxesLength; ++i)//初始化Team中宝可梦数据
        {
            foreach(Pokemon pokemon in boxes[i])
            {
                if(pokemon != null && pokemon.Base != null)
                {
                    pokemon.Init();
                }
            }
        }
    }

    public void ReSetBox()
    {
        boxes = new Pokemon[][]
        {
            box0, box1,  box2,  box3,  box4,  box5,  box6,  box7, box8,
            box9, box10, box11, box12, box13, box14, box15, box16,
            foster
        };
    }

    /// <summary>
    /// 返回健康宝可梦
    /// </summary>
    public Pokemon GetHealthyPokemon()
    {
        for(int i = 0; i < 6; ++i)
        {
            if(box0[i].Base != null && box0[i].isActive)
            {
                currentBattlePokemonNum = i;
                return box0[i];
            }
        }
        return null;
    }

    /// <summary>
    /// 获得宝可梦
    /// </summary>
    public void AddPokemon(Pokemon newPokemon)
    {
        int i = FindVacancy(ref box0);
        if(i != 61)
        {
            box0[i] = newPokemon;
            return;
        }
        else
        {
            int boxesLength = boxes.Length;
            for(int n = 1; n < boxesLength; ++n)
            {
                i = FindVacancy(ref boxes[n]);
                if(i != 61)
                {
                    boxes[n][i] = newPokemon;
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 盒子间交换宝可梦
    /// </summary>
    /// <param name="box1">盒子1</param>
    /// <param name="pos1">在盒子1中的位置</param>
    /// <param name="box2">盒子2</param>
    /// <param name="pos2">在盒子2中的位置</param>
    public void SwapPokemon(int box1, int pos1, int box2, int pos2)
    {
        Pokemon temp = boxes[box1][pos1];
        boxes[box1][pos1] = boxes[box2][pos2];
        boxes[box2][pos2] = temp;
    }

    /// <summary>
    /// 交换并out出交换的宝可梦
    /// </summary>
    /// <param name="box1">盒子1</param>
    /// <param name="pos1">在盒子1中的位置</param>
    /// <param name="box2">盒子2</param>
    /// <param name="pos2">在盒子2中的位置</param>
    /// <param name="a">1号</param>
    /// <param name="b">2号</param>
    public void SwapPokemonAndOut(int box1, int pos1, int box2, int pos2, out Pokemon a, out Pokemon b)
    {
        Pokemon temp = boxes[box1][pos1];
        a = boxes[box1][pos1] = boxes[box2][pos2];
        b = boxes[box2][pos2] = temp;
    }

    /// <summary>
    /// 交换并刷新面板
    /// </summary>
    /// <param name="box1">盒子1</param>
    /// <param name="pos1">在盒子1中的位置</param>
    /// <param name="box2">盒子2</param>
    /// <param name="pos2">在盒子2中的位置</param>
    /// <param name="a">1号刷新方法</param>
    /// <param name="b">2号刷新方法</param>
    public void SwapPokemonAndRefresh
        (
            int box1, int pos1,
            int box2, int pos2,
            System.Action<Pokemon> a, System.Action<Pokemon> b
        )
    {
        Pokemon temp = boxes[box1][pos1];
        a.Invoke(boxes[box1][pos1] = boxes[box2][pos2]);
        b.Invoke(boxes[box2][pos2] = temp);
    }

    /// <summary>
    /// 返回当前打开的箱子
    /// </summary>
    public ref Pokemon[] GetCurrentBox() => ref boxes[currentBox];

#region 查找方法
    Pokemon find;
    /// <summary>
    /// 返回检查位置时的宝可梦
    /// </summary>
    public Pokemon FindPokemon => find;

    /// <summary>
    /// 检查当前位置有无宝可梦 检查背包要+60
    /// </summary>
    public bool HavePokemon(int id)
    {
        if(id < 60)
        {
            //pc
            find = boxes[currentBox][id];
        }
        else
        {
            //背包
            id -= 60;
            find = box0[id];
        }
        return find.Base? true : false;
    }

    /// <summary>
    /// 检查是否能交换
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public bool CanSwap(int n)
        => box0[1].Base == null? boxes[currentBox][n].Base == null? false : true : true;

    /// <summary>
    /// 背包宝可梦排序
    /// </summary>
    public bool SortTeam()
    {
        int n = FindVacancy(ref box0);
        if(n == 61)
        {
            return false;
        }
        else
        {
            int s = FindNextPokemon(n);
            while(s != 0)
            {
                SwapPokemon(0,n,0,s);
                n++;
                s = FindNextPokemon(n);
            }
            return true;
        }
    }

    /// <summary>
    /// 找盒子空位
    /// </summary>
    /// <param name="_box">盒子</param>
    /// <returns>61为当前盒子无空位</returns>
    private int FindVacancy(ref Pokemon[] _box)
    {
        int count = _box.Length;
        for(int i = 0; i < count; ++i)
        {
            if(_box[i].Base == null)
            {
                return i;
            }
        }
        //没有空位
        return 61;
    }

    /// <summary>
    /// 通过编号找空位
    /// </summary>
    /// <param name="id">boxes排序号</param>
    public int FindVacancyByNumber(int id) => FindVacancy(ref boxes[id]);

    /// <summary>
    /// 寻找队伍中n号位之后存在的宝可梦
    /// </summary>
    /// <param name="n">位号</param>
    /// <returns>之后第一只宝可梦位置 (0为无空位)</returns>
    private int FindNextPokemon(int n)
    {
        for(int i = n; i < 6; ++i)
        {
            if(box0[i].Base != null)
            {
                return i;
            }
        }
        //没有空位
        return 0;
    }

    /// <summary>
    /// 队伍中宝可梦数量
    /// </summary>
    public int PlayerTeamPokemonsCount()
    {
        int count = 0;
        foreach(Pokemon pokemon in box0)
        {
            if(pokemon.Base == null)
            {
                break;
            }
            count++;
        }
        return count;
    }
#endregion
#region 对宝可梦进行处理
    /// <summary>
    /// 放生
    /// </summary>
    public void Free(int n)
    {
        if(!boxes[currentBox][n].Lock)
        {
            boxes[currentBox][n] = nullPokemon;
        }
    }

    /// <summary>
    /// 治疗背包宝可梦状态
    /// </summary>
    public void CurePokemon()
    {
        foreach(Pokemon pokemon in box0)
        {
            if(pokemon.Base == null)
            {
                break;
            }
            pokemon.OnCureAll();
        }
    }

    /// <summary>
    /// 结束战斗恢复宝可梦状态
    /// </summary>
    public void EndBattle()
    {
        foreach(Pokemon pokemon in box0)
        {
            if(pokemon.Base == null)
            {
                break;
            }
            pokemon.OnBattleOver();
        }
    }
#endregion
#region 存档处理
    /// <summary>
    /// 获得玩家Team的保存数据
    /// </summary>
    public PokemonSaveData[][] GetPlayerTeamData()
    {
        int length = boxes.Length;
        PokemonSaveData[][] saveDatas = new PokemonSaveData[length][];
        for(int i = 0; i < length; ++i)
        {
            //当前盒子大小
            int count = boxes[i].Length;
            //创建一个一样大的数据组
		    PokemonSaveData[] saveData = new PokemonSaveData[count];
            //当前盒子
            Pokemon[] currentBox = boxes[i];
            //可以//using System.Linq;//currentBox[n].Select(p => p.GetSaveData()).ToArray();
            //存数据
		    for(int n = 0; n < count; ++n)
		    {
		    	saveData[n] = currentBox[n].GetSaveData();
		    }
            saveDatas[i] = saveData;
        }

        return saveDatas;
    }

    /// <summary>
    /// 加载数据
    /// </summary>
    /// <param name="saveDatas">PokemonSaveData[][]</param>
    public void SetTeamData(PokemonSaveData[][] saveDatas)
    {
        int length = boxes.Length;
        for(int i = 0; i < length; ++i)
        {
            int count = boxes[i].Length;
            //当前处理的盒子
            Pokemon[] currentBox = boxes[i];
            PokemonSaveData[] saveData = saveDatas[i];
            //可以Linq//team.Pokemons = saveData.bagPokemons.Select(s => new Pokemon(s)).ToArray();
            for(int n = 0; n < count; ++n)
		    {
		    	currentBox[n] = new Pokemon(saveData[n]);
		    }
        }
    }
#endregion
}