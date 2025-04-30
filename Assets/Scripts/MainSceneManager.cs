
using UnityEngine;

public class MainSceneManager : MonoBehaviour
{
    public Canvas interactionCanvas;
    //public GameObject playerObject;
    //public InputManager playerMovement;

    public void ExitInteract()
    {
        Debug.Log("Stopped Interaction with: " + gameObject.name);

        if (interactionCanvas != null)
        {
            interactionCanvas.gameObject.SetActive(false);
            //playerMovement = playerObject.GetComponent<InputManager>();
            //playerMovement.canMove = true;
            Debug.Log("Canvas visibility toggled to: false");
        }
        else
        {
            if (interactionCanvas == null)
            {
                Debug.LogError("Interaction Canvas is not assigned in the Inspector for: " + gameObject.name);
            }
        }
    }
}
