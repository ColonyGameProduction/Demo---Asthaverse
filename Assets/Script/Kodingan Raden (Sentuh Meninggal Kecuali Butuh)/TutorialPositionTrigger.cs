using UnityEngine;
using System.Collections.Generic;

public class TutorialPositionTrigger : MonoBehaviour
{
    [SerializeField] private List<GameObject> tutorialImages;
    [SerializeField] private int tutorialIndex;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            tutorialImages[tutorialIndex].SetActive(true);
        }
    }
}
