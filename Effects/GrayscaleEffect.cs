using Godot;
using System;

public partial class GrayscaleEffect : ColorRect
{
	public void Enable()
	{
		Visible = true;
	}

	public void Disable()
	{
		Visible = false;
	}

}
