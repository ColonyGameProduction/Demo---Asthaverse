
using UnityEngine;

public class SceneMoveManager : MonoBehaviour
{
    public static SceneMoveManager Instance {get; private set;}
    private void Awake() 
    {
        Instance = this;
    }

    // public void RestartScene()
    // {

    // }

    // private void MoveScene()

}
