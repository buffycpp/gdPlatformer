using Godot;

public partial class Turret : TimeSyncedNode2D
{
	[Export] public float FiringInterval = 2f;
	[Export] public float ProjectileSpeed = 100f;
	[Export] public AnimatedSprite2D animatedSprite;
	[Export] public GpuParticles2D muzzleFlashParticles;
	[Export] public PackedScene projectileScene;
	[Export] public AudioStream shootSound;

	private int _lastShotCycle = -1;

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (!_isAwake)
			return;

		float levelTime = GameController.Instance.LevelTime;

		int cycle = Mathf.FloorToInt(levelTime / FiringInterval);
		float t = levelTime % FiringInterval;

		// Fire halfway through each cycle.
		if (t >= FiringInterval * 0.5f && cycle != _lastShotCycle)
		{
			_lastShotCycle = cycle;
			Shoot();
		}
	}

	public void Shoot()
	{
		animatedSprite.Play("shoot");
		muzzleFlashParticles.Emitting = true;
		SoundManager.Instance.PlaySfx(shootSound, GlobalPosition, volumeDb: -8f, pitch: (float)GD.RandRange(2.4, 2.7), maxDistance: 500f);

		var projectile = projectileScene.Instantiate<TurretProjectile>();
		projectile.GlobalPosition = GlobalPosition;
		projectile.Rotation = GlobalRotation;
		projectile.Speed = ProjectileSpeed;

		GetTree().CurrentScene.AddChild(projectile);		
	}
}
