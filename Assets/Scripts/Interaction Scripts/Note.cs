using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour, IInteractable
{
    [SerializeField] private string prompt;
    //public string prompt;
    public string InteractionPrompt => prompt;
    public bool Interact(InteractorScript interactor)
    {
        Debug.Log(InteractionPrompt);
        Debug.Log("reading the note");
        return true;
    }
}
