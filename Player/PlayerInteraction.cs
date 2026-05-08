using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class PlayerInteraction
{
    private List<IInteractable> _interactables = new();

    public PlayerInteraction(Area2D interactionArea)
    {
        interactionArea.AreaEntered += OnInteractionAreaEntered;
        interactionArea.AreaExited += OnInteractionAreaExited;
    }

    private void OnInteractionAreaExited(Area2D area)
    {
        GD.Print("this works");
        if (area is IInteractable interactable)
        {
            _interactables.Remove(interactable);
            GD.Print("this doesnt work");
        }
    }

    private void OnInteractionAreaEntered(Area2D area)
    {
        GD.Print("Entered: " + area.Name);
        if (area is IInteractable interactable)
        {
            GD.Print("Added: " + area.Name);
            _interactables.Add(interactable);
        }        
    }

    public void TriggerInteraction()
    {
        GD.Print("trying to interact");
        _interactables.FirstOrDefault(e => e.CanInteract())?.Interact();
    }
}