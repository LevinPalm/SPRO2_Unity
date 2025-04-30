using UnityEngine;

public class SimpleInteractable : MonoBehaviour
{
    public string description = "ask a question to the Vendor";
    public Canvas interactionCanvas;
    public GameObject playerObject;
    private InputManager playerMovement;
    
    private void Start()
    {
        
        // Ensure the canvas starts hidden
        if (interactionCanvas != null)
        {
            interactionCanvas.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("Interaction Canvas is not assigned in the Inspector for: " + gameObject.name);
        }

        
        // Get the InputManager component once at the start
        if (playerObject != null)
        {
            playerMovement = playerObject.GetComponent<InputManager>();
            if (playerMovement == null)
            {
                Debug.LogError("InputManager script not found on the Player GameObject assigned to: " + gameObject.name);
            }
        }
        
        else
        {
            Debug.LogError("Player GameObject is not assigned in the Inspector for: " + gameObject.name);
        }
        
    }
    
    public void Interact()
    {
        Debug.Log("Interacted with: " + gameObject.name);

        if (interactionCanvas != null)
        {
            interactionCanvas.gameObject.SetActive(true);
            
            if (playerMovement != null)
            {
                playerMovement.canMove = false;
                Debug.Log("Canvas visibility toggled to: true, Player movement disabled.");
            }
            else
            {
                Debug.LogError("Could not disable player movement because InputManager reference is null.");
            }
            
}
        else
        {
            Debug.LogError("Interaction Canvas is not assigned in the Inspector for: " + gameObject.name);
        }
    }

    public void ExitInteract()
    {
        Debug.Log("Stopped Interaction with: " + gameObject.name);

        if (interactionCanvas != null)
        {
            interactionCanvas.gameObject.SetActive(false);
            
            if (playerMovement != null)
            {
                playerMovement.canMove = true;
                Debug.Log("Canvas visibility toggled to: false, Player movement enabled.");
            }
            else
            {
                Debug.LogError("Could not enable player movement because InputManager reference is null.");
            }
            
        }
        else
        {
            Debug.LogError("Interaction Canvas is not assigned in the Inspector for: " + gameObject.name);
        }
    }

    public string GetDescription()
    {
        return description;
    }
}
