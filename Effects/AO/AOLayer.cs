using Godot;

public partial class AOLayer : Sprite2D
{
	[Export] public PackedScene AOViewportScene;
	private SubViewport _aoViewport;

	public override void _Ready()
	{
		CallDeferred(nameof(CreateAOLayer));
	}

	public void CreateAOLayer()
	{
		_aoViewport = AOViewportScene.Instantiate<SubViewport>();

		//attach viewport to level
		GetParent().AddChild(_aoViewport);

		Texture = _aoViewport.GetTexture();

		AddAOEntities();
	}

	public void AddAOEntities()
	{
		foreach (var entity in GetTree().GetNodesInGroup("AOEntity"))
		{
			var dupe = entity.Duplicate();
			_aoViewport.AddChild(dupe);
		}
	}
}
