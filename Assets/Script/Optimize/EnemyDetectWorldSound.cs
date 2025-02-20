
using UnityEngine;

public class EnemyDetectWorldSound : MonoBehaviour, IHearSound
{
    private EnemyAIBehaviourStateMachine enemyAI;
    private GameManager gm;
    private void Awake() 
    {
        enemyAI = GetComponentInParent<EnemyAIBehaviourStateMachine>();
    }
    private void Start()
    {
        gm = GameManager.instance;
    }
    public void RespondToSound(Vector3 soundOriginPos)
    {
        if(!gm.IsGamePlaying() || enemyAI.CharaIdentity.IsDead) return;

        enemyAI.RespondToSound(soundOriginPos);
    }

}
