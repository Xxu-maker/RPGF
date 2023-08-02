public class Quest
{
    public QuestBase Base { get; private set; }
    public QuestStatus Status{ get; private set; }

    public Quest(QuestBase _base)
    {
        Base = _base;
    }

    public void StartQuest()
    {
        Status = QuestStatus.Started;
        //DialogManager.Instance
    }
}
public enum QuestStatus{ None, Started, Completed }