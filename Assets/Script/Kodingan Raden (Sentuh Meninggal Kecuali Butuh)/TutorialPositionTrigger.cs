using UnityEngine;
using System.Collections.Generic;

public class TutorialPositionTrigger : MonoBehaviour
{
    [SerializeField] private List<GameObject> tutorialCanvases;
    [SerializeField] private int canvasIndex;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            tutorialCanvases[canvasIndex].SetActive(true);
        }
    }
}
