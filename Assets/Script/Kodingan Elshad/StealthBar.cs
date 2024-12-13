using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StealthBar : MonoBehaviour
{
    GameManager gm;

    public GameObject imageContainer;
    public GameObject fillBarImageParent;
    public GameObject imageContainerCameraCanvas;
    public GameObject imageContainerCheck;
    public GameObject tempPlayer;

    private void Start()
    {
        gm = GameManager.instance;
        imageContainer.gameObject.SetActive(false);
    }

    public void FillingTheImage(float currStealth, float MaxStealth)
    {
        Image fillBarImage;

        if(IsBarVisible(Camera.main, imageContainer))
        {
            imageContainerCheck = null;
            Destroy(imageContainerCameraCanvas);

            fillBarImage = imageContainer.transform.GetChild(1).GetComponent<Image>();
            fillBarImage.fillAmount = currStealth / MaxStealth;
            FillingTheBar(imageContainer, fillBarImage);
        }
        else
        {
            if(imageContainerCheck != imageContainer)
            {
                if(imageContainer.activeSelf)
                {
                    imageContainerCameraCanvas = Instantiate(imageContainer, fillBarImageParent.transform);
                    imageContainer.gameObject.SetActive(false);
                }
                imageContainerCheck = imageContainer;
            }

            if(imageContainerCameraCanvas != null)
            {
                imageContainerCameraCanvas.GetComponent<RectTransform>().localScale = new Vector3(70, 70, 70);
                fillBarImage = imageContainerCameraCanvas.transform.GetChild(1).GetComponent<Image>();
                fillBarImage.fillAmount = currStealth / MaxStealth;
                fillBar(imageContainerCameraCanvas);
                FillingTheBar(imageContainerCameraCanvas, fillBarImage);
            }
        }
    }

    public void FillingTheBar(GameObject imageBarFill, Image fillBarImage)
    {
        if (fillBarImage.fillAmount <= 0)
        {
            imageBarFill.gameObject.SetActive(false);
        }
        else if (fillBarImage.fillAmount > 0 && fillBarImage.fillAmount < 1)
        {
            imageBarFill.gameObject.SetActive(true);
        }
        else if (fillBarImage.fillAmount >= 1)
        {
            imageBarFill.gameObject.SetActive(false);
        }
    }

    public bool IsBarVisible(Camera cam, GameObject go)
    {
        var planes = GeometryUtility.CalculateFrustumPlanes(cam);
        var point = go.transform.position;

        foreach(var plane in planes)
        {
            if(plane.GetDistanceToPoint(point) < 0)
            {
                return false;
            }
        }
        return true;
    }

    public void fillBar(GameObject fillBar)
    {
        Transform player = Camera.main.transform;
        Vector3 fillBarLocation = imageContainer.GetComponentInParent<EnemyAI>().transform.position;

        fillBarLocation.y = player.position.y;

        Vector3 flatForward = player.forward;
        flatForward.y = 0;
        flatForward.Normalize();

        Vector3 direction = (fillBarLocation - player.position).normalized;

        float angle = (Vector3.SignedAngle(direction, flatForward, Vector3.up));
        Debug.Log(angle);
        fillBar.transform.localEulerAngles = new Vector3(0, 0, angle);
    
    }

}
