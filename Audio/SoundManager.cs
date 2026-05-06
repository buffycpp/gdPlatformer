using Godot;
using System.Collections.Generic;

public partial class SoundManager : SingletonNode<SoundManager>
{
	[Export] public int PoolSize = 16;

	private List<AudioStreamPlayer2D> _pool = new();
	private Queue<AudioStreamPlayer2D> _available = new();

	public override void _Ready()
	{
		// Create pool
		for (int i = 0; i < PoolSize; i++)
		{
			var player = new AudioStreamPlayer2D();
			AddChild(player);

			player.Finished += () => ReturnToPool(player);

			_pool.Add(player);
			_available.Enqueue(player);
		}
	}

	public void PlaySfx(AudioStream stream, Vector2 position, float volumeDb = 0f, float pitch = 1f)
	{
		if (_available.Count == 0)
		{
			GD.Print("No available audio players in pool!");
			return;
		}

		var player = _available.Dequeue();

		player.Stream = stream;
		player.GlobalPosition = position;
		player.VolumeDb = volumeDb;
		player.PitchScale = pitch;
		player.Bus = "SFX";

		player.Play();
	}

	private void ReturnToPool(AudioStreamPlayer2D player)
	{
		player.Stop();
		player.Stream = null;
		_available.Enqueue(player);
	}
}
