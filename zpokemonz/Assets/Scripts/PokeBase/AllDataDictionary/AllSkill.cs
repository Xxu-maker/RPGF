using System.Collections.Generic;
using UnityEngine;
public class AllSkill
{
    static Dictionary<int, SkillBase> skills;
    private static bool AlreadyLoad;
    public static void Init()
    {
        if(AlreadyLoad)
        {
            return;
        }
        skills = new Dictionary<int, SkillBase>();
        SkillBase[] skillArray = ResM.Instance.LoadAll<SkillBase>("SkillSO/");
        foreach(SkillBase skill in skillArray)
        {
            int id = skill.Sid;
            if(skills.ContainsKey(id))
            {
                Debug.LogError($"有两个技能的ID相同{id}");
                continue;
            }
            skills[id] = skill;
        }
        AlreadyLoad = true;
    }
    public static SkillBase GetPokemonByID(int id)
    {
        if(!skills.ContainsKey(id))
        {
            Debug.LogError($"没有查询到Base{id}");
            return null;
        }
        return skills[id];
    }
}