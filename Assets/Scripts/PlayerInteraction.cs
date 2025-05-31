using UnityEngine;
using TMPro;



public class PlayerInteraction : MonoBehaviour
{
    public float interactionRadius = 1.5f;  //determines how close you need to go to a vendor
    public LayerMask interactableLayer; //Layer set in Unity

    private Collider closestCollider = null;

    public TextMeshProUGUI interactionPromptText;

    private void Update()
    {
        FindClosestInteractable();
        if (Input.GetKeyDown(KeyCode.E))
        {            
            TryInteract();
        }
    }

    void FindClosestInteractable()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactionRadius, interactableLayer);  //finding all colliders in range

        closestCollider = null;
        float minDistance = float.MaxValue;
        foreach (Collider hit in hits)  //finding the closest collider
        {
            float distance = Vector3.Distance(transform.position, hit.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestCollider = hit;
            }
        }

        if (closestCollider != null)
        {
            SimpleInteractable interactableScript = closestCollider.GetComponent<SimpleInteractable>();
            if (interactableScript != null && interactionPromptText != null)
            {
                interactionPromptText.text = $"Press E to {interactableScript.GetDescription()}";   //showing the description of each object, here only interact
                interactionPromptText.enabled = true;   //make sure it is visible
            }
            else
            {
                if (interactionPromptText != null) interactionPromptText.enabled = false;
            }
        }
        else
        {
            if (interactionPromptText != null) interactionPromptText.enabled = false;
        }
    }

    void TryInteract()
    {
        if (closestCollider != null)
        {
            SimpleInteractable interactableScript = closestCollider.GetComponent<SimpleInteractable>();
            if (interactableScript != null)
            {
                interactableScript.Interact();
                Debug.Log("Interacted with closest: " + closestCollider.name);
            }
            else
            {
                Debug.LogWarning("Missing script.");
            }
        }
        else
        {
            Debug.Log("Nothing interactable in range to interact with.");
        }
    }
}
