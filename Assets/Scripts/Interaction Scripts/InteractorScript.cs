using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractorScript : MonoBehaviour
{
    [SerializeField] private Transform interactionPoint;
    [SerializeField] private float interactionRadius;
    [SerializeField] private LayerMask interactableMask;
    [SerializeField] private InteractionPromptUI promptUI;
    public KeyCode interactionKey = KeyCode.E;

    private readonly Collider[] collisions = new Collider[3];
    [SerializeField] private int numFound;

    private IInteractable interactable;
    private Outline objectOutline;

    private void Update()
    {
        numFound = Physics.OverlapSphereNonAlloc(interactionPoint.position, interactionRadius, collisions, interactableMask);

        if (numFound > 0)
        {
            interactable = collisions[0].GetComponent<IInteractable>();
            
            if (interactable != null)
            {
                objectOutline = collisions[0].gameObject.GetComponent<Outline>();
                objectOutline.enabled = true;
                //var outline = interactableObject.AddComponent<Outline>();
                //outline.OutlineMode = Outline.Mode.OutlineAll;
                //outline.OutlineColor = Color.yellow;
                objectOutline.OutlineWidth = 20f;

                if (!promptUI.isDisplayed)
                {
                    promptUI.SetUp(interactable.InteractionPrompt);
                    Debug.Log(interactable.InteractionPrompt);
                }
                if (Input.GetKeyDown(interactionKey))
                {
                    interactable.Interact(this);
                }
            }
        }

        else
        {
            if (interactable != null)
            {
                interactable = null;
            }
            if (promptUI.isDisplayed)
            {
                promptUI.Close();
                objectOutline.enabled = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(interactionPoint.position, interactionRadius);
    }
}
