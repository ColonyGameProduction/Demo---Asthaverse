
using UnityEngine;

public class DebuggingCommand : MonoBehaviour
{
    public CharacterIdentity identity;
    public bool Ded;

    private void Update() {
        if(Ded)
        {
            Ded = false;
            if(identity!=null)identity.Hurt(identity.CurrHealth);
        }
        

    }
}
