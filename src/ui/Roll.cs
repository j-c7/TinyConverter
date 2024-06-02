using Godot;
using System;

namespace TinyConversor;

public partial class Roll : TextureRect
{
	[Export]
	private float _rotationSpeed = 10.0f;
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Rotation += _rotationSpeed * (float)delta; 
	}
}
