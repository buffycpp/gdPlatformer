using Godot;

public partial class TurretHitParticles : GpuParticles2D
{
	public override void _Ready()
	{
		Finished += OnFinished;
		Emitting = true;
	}

	private void OnFinished()
	{
		QueueFree();
	}
}
