using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractorScript : MonoBehaviour
{
    [SerializeField] private Transform interactionPoint;
    [SerializeField] private float interactionRadius;
    [SerializeField] private LayerMask interactableMask;
    public KeyCode interactionKey = KeyCode.E;

    private readonly Collider[] collisions = new Collider[3];
    [SerializeField] private int numFound;

    private void Update()
    {
        numFound = Physics.OverlapSphereNonAlloc(interactionPoint.position, interactionRadius, collisions, interactableMask);

        if (numFound > 0)
        {
            var interactable = collisions[0].GetComponent<IInteractable>();

            if (interactable != null && Input.GetKeyDown(interactionKey)) 
            {
                interactable.Interact(this);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(interactionPoint.position, interactionRadius);
    }
}
