using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class PlayerInteraction : Node
{
    [Export] public Area2D InteractionArea { get; set; }
    private List<IInteractable> _interactables = new();

    public override void _Ready()
    {
        InteractionArea.AreaEntered += OnInteractionAreaEntered;
        InteractionArea.AreaExited += OnInteractionAreaExited;        
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

        CallDeferred(nameof(TriggerInteraction));
    }

    public void TriggerInteraction()
    {
        GD.Print("trying to interact");
        _interactables.FirstOrDefault(e => e.CanInteract())?.Interact();
    }
}