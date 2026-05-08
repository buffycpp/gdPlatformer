using Godot;

public class GameInputContext : IInputContext
{
	public void ProcessInput(double delta)
	{
		float xInput = Input.GetAxis("move_left", "move_right");
		PlayerController.Instance.SetXInput(xInput);

		if (Input.IsActionJustPressed("jump"))
		{
			PlayerController.Instance.TryJump();
		}

		if (Input.IsActionJustPressed("interact"))
		{
			PlayerController.Instance.TryInteract();
		}
	}
}
