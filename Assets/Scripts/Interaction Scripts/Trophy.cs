using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trophy : MonoBehaviour, IInteractable
{
    [SerializeField] private string prompt;
    [SerializeField] private LogicScript logic;

    public string InteractionPrompt => prompt;
    public bool Interact(InteractorScript interactor)
    {
        logic.WinGame();
        return true;
    }
}
