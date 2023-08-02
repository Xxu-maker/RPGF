using System;
public class Condition
{
    [NonSerialized] public ConditionID ConditionID;
    [NonSerialized] public string Name;
    [NonSerialized] public string StartMessage;
    [NonSerialized] public Action<Pokemon> OnStart;
    [NonSerialized] public Func<Pokemon, bool> OnBeforeMove;
    [NonSerialized] public Action<Pokemon> OnAfterTurn;
}