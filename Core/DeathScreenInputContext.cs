using Godot;

public class DeathScreenInputContext : IInputContext
{
    public void ProcessInput(double delta)
    {
        if (Input.IsActionJustPressed("jump"))
        {
            GameController.Instance.ResetCurrentLevelAfterDeath();
        }
    }
}