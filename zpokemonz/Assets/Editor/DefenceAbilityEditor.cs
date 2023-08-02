using UnityEditor;
/// <summary>
/// 防守特性编辑器拓展
/// </summary>
[CustomEditor(typeof(DefenceAbility))]
public class DefenceAbilityEditor : Editor
{
    private SerializedObject defenceAbility;

    private SerializedProperty
        id, abilityName, description, triggerType,//继承父类的参数
        defenceType,
        immunityConditionID,
        typesUsedToCheckDefence,
        stat;

    private void OnEnable()
    {
        defenceAbility = new SerializedObject(target);

        id = defenceAbility.FindProperty("id");
        abilityName = defenceAbility.FindProperty("abilityName");
        description = defenceAbility.FindProperty("description");
        triggerType = defenceAbility.FindProperty("triggerType");

        defenceType = defenceAbility.FindProperty("defenceType");

        immunityConditionID = defenceAbility.FindProperty("immunityConditionID");

        typesUsedToCheckDefence = defenceAbility.FindProperty("typesUsedToCheckDefence");

        stat = defenceAbility.FindProperty("stat");
    }

    public override void OnInspectorGUI()
    {
        defenceAbility.Update();

        //基础参数
        EditorGUILayout.PropertyField(id);
        EditorGUILayout.PropertyField(abilityName);
        EditorGUILayout.PropertyField(description);
        EditorGUILayout.PropertyField(triggerType);

        //用来判断的Enum
        EditorGUILayout.PropertyField(defenceType);

        switch(defenceType.enumValueIndex)
        {
            //免疫状态
            case 0:
                EditorGUILayout.PropertyField(immunityConditionID);
            break;

            //攻击无效化的属性
            case 3 | 4 | 7 | 9:
                EditorGUILayout.PropertyField(typesUsedToCheckDefence);
            break;

            //用于防止单项能力被降低
            case 10:
                EditorGUILayout.PropertyField(stat);
            break;
        }

        defenceAbility.ApplyModifiedProperties();
    }
}