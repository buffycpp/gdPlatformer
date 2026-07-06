using Godot;
using GodotPlugins.Game;
using System;

[Tool]
public partial class Laser : Node2D, ITriggerable
{
	private float _width = 32f;
	[Export] public float Width { get { return _width; } set { _width = value; UpdateComponents(); }}

	private Color _primaryColor;
	[Export] public Color PrimaryColor { get {return _primaryColor ;} set { _primaryColor = value; UpdateComponents();  } }

	[Export] public Line2D MainLine;
	[Export] public Line2D GlowLine;
	[Export] public Sprite2D SpriteLeft;
	[Export] public Sprite2D SpriteRight;
	[Export] public CollisionShape2D CollisionShape2D;


	[Export] public float PulseSpeed = 2.0f;
	[Export] public float PulseStrength = 0.08f;

	private bool _isOn = true;
	[Export] public bool IsOn { get { return _isOn; } set { _isOn = value; Toggle(value); } }	

	private float time;

	private float innerBaseWidth;
	private float outerBaseWidth;

	private float innerBaseAlpha;
	private float outerBaseAlpha;

	private bool initialState = false;

	public override void _Ready()
	{
		initialState = _isOn;

		if (GlowLine != null)
		{
			innerBaseWidth = Width;
			innerBaseAlpha = GlowLine.DefaultColor.A;
		}

		if (MainLine != null)
		{
			outerBaseWidth = MainLine.Width;
			outerBaseAlpha = MainLine.DefaultColor.A;
		}

		if (GameController.Instance != null)
		{
			GameController.Instance.ResetLevelEvent += OnResetLevel;
		}
	}

	public override void _ExitTree()
	{
		if (GameController.Instance != null)
		{
			GameController.Instance.ResetLevelEvent -= OnResetLevel;
		}
	}

    public override void _Process(double delta)
	{
		if (Engine.IsEditorHint())
		{
			return;
		}

		time += (float)delta;
		AnimateLaser();
	}

	private void AnimateLaser()
	{
		float pulse = (Mathf.Sin(time * PulseSpeed) + 1f) * 0.5f;

		// centered around 1.0 → subtle scaling
		float energy = 1f + (pulse - 0.5f) * PulseStrength;

		if (GlowLine != null)
		{
			GlowLine.Width = innerBaseWidth * energy;

			Color c = GlowLine.DefaultColor;
			c.A = innerBaseAlpha; // keep core stable
			GlowLine.DefaultColor = c;
		}

		if (MainLine != null)
		{
			MainLine.Width = outerBaseWidth * energy;

			Color c = MainLine.DefaultColor;
			c.A = outerBaseAlpha * energy; // subtle breathing
			MainLine.DefaultColor = c;
		}
	}

	private void UpdateComponents()
	{
		if (MainLine == null || GlowLine == null || SpriteLeft == null || SpriteRight == null || CollisionShape2D == null)
		{
			return;
		}

		MainLine.SetPointPosition(0, new Vector2(-Width, 0));
		MainLine.SetPointPosition(1, new Vector2(Width, 0));

		GlowLine.SetPointPosition(0, new Vector2(-Width, 0));
		GlowLine.SetPointPosition(1, new Vector2(Width, 0));

		MainLine.DefaultColor = PrimaryColor;
		GlowLine.DefaultColor = new Color(PrimaryColor, 0.35f);

		SpriteLeft.Position = new Vector2(-Width, 0);
		SpriteRight.Position = new Vector2(Width, 0);

		((RectangleShape2D)CollisionShape2D.Shape).Size = new Vector2(Width * 2, 5f);
	}

    public bool CanTrigger()
    {
        return true;
    }

    public void Trigger(string actionName = "default")
    {
        if (actionName == "default")
		{
			Toggle(!_isOn);
			return;
		}

		if (actionName == "on")
		{
			Toggle(true);
			return;
		}

		if (actionName == "off")
		{
			Toggle(false);
			return;	
		}
    }

	public void Toggle(bool on)
	{
		if (MainLine == null || GlowLine == null || CollisionShape2D == null)
		{
			return;
		}

		_isOn = on;

		MainLine.Visible = on;
		GlowLine.Visible = on;
		CollisionShape2D.Disabled = !on;
	}

    private void OnResetLevel()
    {
        Toggle(initialState);
    }	
}