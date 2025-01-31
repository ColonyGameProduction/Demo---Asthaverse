
using UnityEngine;

public class EnemyDetectWorldSound : MonoBehaviour, IHearSound
{
    private EnemyAIBehaviourStateMachine enemyAI;
    private void Awake() 
    {
        enemyAI = GetComponentInParent<EnemyAIBehaviourStateMachine>();
    }
    public void RespondToSound(Vector3 soundOriginPos)
    {
        enemyAI.RespondToSound(soundOriginPos);
    }

}
