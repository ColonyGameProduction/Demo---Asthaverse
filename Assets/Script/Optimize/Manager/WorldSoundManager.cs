
using UnityEngine;


public class WorldSoundManager : MonoBehaviour
{
    public static WorldSoundManager Instance {get; private set;}
    [SerializeField] private SOWorldSoundList worldSoundList;
    private Collider[] _heardSounds;
    private void Awake() 
    {
        Instance = this;
    }
    public void MakeSound(WorldSoundName soundName, Vector3 originPos, LayerMask _charaEnemyMask)
    {
        WorldSound chosenSound = null;
        foreach(WorldSound sound in worldSoundList.worldSounds)
        {
            if(soundName == sound.Name)
            {
                chosenSound = sound;
                break;
            }
        }
        if(chosenSound == null)return;
        _heardSounds = Physics.OverlapSphere(originPos, chosenSound.SoundRange, _charaEnemyMask);
        for(int i=0; i<_heardSounds.Length;i++)
        {
            IHearSound hearSound = _heardSounds[i].GetComponent<IHearSound>();
            if(hearSound != null)
            {
                Debug.Log("heard sound is" + soundName);
                hearSound.RespondToSound(originPos);
            }
        }
    }
}
