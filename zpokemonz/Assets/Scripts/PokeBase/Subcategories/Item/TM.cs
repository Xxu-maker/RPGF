using UnityEngine;
[CreateAssetMenu(menuName = "宝可梦道具/创建新技能机")]
public class TM : ItemBase
{
    [SerializeField] SkillBase skill;
    public SkillBase SKill => skill;
    public override bool Use(Pokemon pokemon)//CheckLearnableSkill
    {
        foreach(LearnableSkill learn in pokemon.Base.LearnableSkills)
        {
            if(learn.Base == skill)
            {
                foreach(Skill currentSkill in pokemon.Skill)
                {
                    if(currentSkill.Base == skill)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        return false;
    }
    public string LearnSkill(Pokemon pokemon, int n)
    {
        if(pokemon.Skill.Count < 4)
		{
			pokemon.LearnSkill(skill);
            return pokemon.NickName + "学会了" + skill.SkillName;
		}
		else
		{
            string oldName = pokemon.Skill[n].Base.SkillName;
            pokemon.Skill[n] = new Skill(skill);
            return $"{pokemon.NickName}忘记了{oldName},学会了{skill.SkillName}";
		}
    }
}