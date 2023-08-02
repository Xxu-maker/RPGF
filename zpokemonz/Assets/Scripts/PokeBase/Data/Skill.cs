using UnityEngine;
[System.Serializable]
public class Skill
{
    private SkillBase skillBase;
    private int pp;
    private int maxPP;

    public SkillBase Base => skillBase;

    public Skill(SkillBase _base)
    {
        skillBase = _base;
        pp = _base.PP;
        maxPP = pp;
    }

    /// <summary>
    /// PP值处理
    /// </summary>
    public void PPValueChange(int value) { pp = Mathf.Clamp(pp + value, 0, maxPP); }

    /// <summary>
    /// PP加满
    /// </summary>
    public void ResetPP()
    {
        if(pp != maxPP)
        {
            pp = maxPP;
        }
    }

    /// <summary>
    /// 检查PP值是否满了
    /// </summary>
    public bool IsThePPMax() { return pp == maxPP; }

    /// <summary>
    /// 检查是否还有PP值
    /// </summary>
    public bool CheckIfPPIsGreaterThanZero() { return pp > 0; }

    /// <summary>
    /// 当前最大PP是否达到规定最大PP值
    /// </summary>
    public bool HasTheCurrentMaxPPReachedTheBaseMaxPP() { return maxPP == skillBase.MaxPP; }

    /// <summary>
    /// 加最大PP值
    /// </summary>
    public void AddMaxPP(int value)
    {
        maxPP += value;
        //不能超过规定最大值
        if(maxPP > skillBase.MaxPP)
        {
            maxPP = skillBase.MaxPP;
        }
    }

    /// <summary>
    /// $"{pp}/{maxPP}"
    /// </summary>
    public string GetPPValueString() { return $"{pp}/{maxPP}"; }

    /// <summary>
    /// 获得当前百分比颜色
    /// </summary>
    public Color GetPercentColor() { return pp == 0? Color.red : pp < maxPP * 0.5f? Color.yellow : Color.black;}

    //存档用
    public Skill(SkillSaveData saveData)
    {
        skillBase = AllSkill.GetPokemonByID(saveData.id);
        pp = saveData.pp;
        maxPP = saveData.maxPP;
    }

    public SkillSaveData GetSaveData()
    {
        SkillSaveData saveData = new SkillSaveData()
        {
            id = Base.Sid,
            pp = this.pp,
            maxPP = this.maxPP
        };
        return saveData;
    }
}

[System.Serializable]
public struct SkillSaveData
{
    public int id;
    public int pp;
    public int maxPP;
}