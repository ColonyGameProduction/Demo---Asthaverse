
public class ReviveFriendQuestHandler : SoloQuestHandler, IUnsubscribeEvent
{
    private PlayableCharacterManager _playableCharacterManager;

    protected override void Start()
    {
        base.Start();
        _playableCharacterManager = PlayableCharacterManager.Instance;
        PlayableCharacterManager.OnFinishReviveAnimation += QuestComplete;
    }

    public override void ActivateQuest()
    {
        base.ActivateQuest();
        QuestComplete();
    }
    protected override void HandleQuestComplete()
    {
        
        if(!_playableCharacterManager.IsEveryoneAlive()) return;

        base.HandleQuestComplete();
    }

    public void UnsubscribeEvent()
    {
        PlayableCharacterManager.OnFinishReviveAnimation -= QuestComplete;
    }
}
