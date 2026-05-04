using Godot;
using System;

[GlobalClass]
public partial class LevelList : Resource
{
    [Export] public LevelData[] Levels { get; set; }
}
