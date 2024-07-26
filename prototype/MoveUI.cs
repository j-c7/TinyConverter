using Godot;
using System;

public partial class MoveUI : Control
{
	bool _isFollowing = false;
	Vector2 _startPos = default;

    public override void _EnterTree()
    {
		GuiInput += OnTilebarGuiInput;
    }

    public override void _ExitTree()
    {
        GuiInput -= OnTilebarGuiInput;
    }

    void OnTilebarGuiInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton)
		{
			_isFollowing = !_isFollowing;
			_startPos = GetLocalMousePosition();
		}

		if (_isFollowing)
			GetWindow().Position = (Vector2I)(GetWindow().Position + GetLocalMousePosition() - _startPos);
	}

}
