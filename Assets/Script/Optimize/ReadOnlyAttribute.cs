using UnityEngine;

public class ReadOnlyAttribute : PropertyAttribute
{
    public bool CanBeEditedInGame {get;}
    public ReadOnlyAttribute(bool canBeEditedInGame)
    {
        CanBeEditedInGame = canBeEditedInGame;
    }
}

