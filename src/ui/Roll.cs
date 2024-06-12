namespace TinyConverter;

public partial class Roll : TextureRect
{
	[Export]
	private float _rotationSpeed = 10.0f;
	
	public override void _Process(double delta)
	{
		Rotation += _rotationSpeed * (float)delta; 
	}
}
