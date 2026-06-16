using Godot;
using System;

public partial class TriggerArea : Area2D
{
	[Export] public Node TriggerableNode;

	public ITriggerable Triggerable => TriggerableNode as ITriggerable;
	private bool _hasBeenTriggered = false;

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}
	
	public void OnBodyEntered(Node body)
	{
		if (!_hasBeenTriggered && body.IsInGroup("Player"))
		{
			if ((Triggerable?.CanTrigger()).GetValueOrDefault())
			{
				Triggerable.Trigger();
				_hasBeenTriggered = true;
			}
		}
	}
}
