using Godot;

[GlobalClass]
public partial class TriggerSetup : Resource
{
	[Export] public NodePath TriggerableNode {get; set;}
	[Export] public string ActionName {get; set;} = "default";
}