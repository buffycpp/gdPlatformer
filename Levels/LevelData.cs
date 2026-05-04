using Godot;
using System;

[GlobalClass]
public partial class LevelData : Resource
{
	[Export] public PackedScene LevelScene { get; set; }
	[Export] public string Name { get; set; }
	[Export] public string Id { get; set; }
}
