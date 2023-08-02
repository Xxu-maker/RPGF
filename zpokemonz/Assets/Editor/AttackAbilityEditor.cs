using UnityEditor;
/// <summary>
/// 攻击特性编辑器扩展
/// </summary>
[CustomEditor(typeof(AttackAbility))]
public class AttackAbilityEditor : Editor
{
    private SerializedObject attackAbility;

    //需要显示的变量 与AttackAbility里变量名相同
    private SerializedProperty
        id, abilityName, description, triggerType,//继承父类的参数
        attackType,
        percent, isVolatileCondition, conditionID,
        hpPercentUsedToCheck, typeUsedToCheckBoost,
        criticalPercent, damage;

    private void OnEnable()
    {
        //新建编辑器UI面板
        attackAbility = new SerializedObject(target);

        //获取变量
        id = attackAbility.FindProperty("id");
        abilityName = attackAbility.FindProperty("abilityName");
        description = attackAbility.FindProperty("description");
        triggerType = attackAbility.FindProperty("triggerType");

        attackType = attackAbility.FindProperty("attackType");

        percent = attackAbility.FindProperty("percent");
        isVolatileCondition = attackAbility.FindProperty("isVolatileCondition");
        typeUsedToCheckBoost = attackAbility.FindProperty("typeUsedToCheckBoost");

        hpPercentUsedToCheck = attackAbility.FindProperty("hpPercentUsedToCheck");
        typeUsedToCheckBoost = attackAbility.FindProperty("typeUsedToCheckBoost");

        criticalPercent = attackAbility.FindProperty("criticalPercent");
        damage = attackAbility.FindProperty("damage");
    }

    public override void OnInspectorGUI()
    {
        //更新
        attackAbility.Update();

        //基础参数
        EditorGUILayout.PropertyField(id);
        EditorGUILayout.PropertyField(abilityName);
        EditorGUILayout.PropertyField(description);
        EditorGUILayout.PropertyField(triggerType);

        //用来判断的Enum
        EditorGUILayout.PropertyField(attackType);

        //根据attackType枚举int值判断哪些内容该显示
        switch(attackType.enumValueIndex)
        {
            //概率对对手触发异常状态
            case 0:
                EditorGUILayout.PropertyField(percent);
                EditorGUILayout.PropertyField(isVolatileCondition);
                EditorGUILayout.PropertyField(typeUsedToCheckBoost);
            break;

            //HP低于百分之33相应技能威力提升百分之50
            case 2:
                EditorGUILayout.PropertyField(hpPercentUsedToCheck);
                EditorGUILayout.PropertyField(typeUsedToCheckBoost);
            break;

            //暴击效果
            case 7:
                EditorGUILayout.PropertyField(criticalPercent);
                EditorGUILayout.PropertyField(damage);
            break;
        }

        //应用
        attackAbility.ApplyModifiedProperties();
    }
}