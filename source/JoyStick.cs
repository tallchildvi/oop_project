using Godot;
using System;


public partial class JoyStick : Node2D
{
	private Vector2 coordinates = new Vector2(0, 0);
	private Vector2 direction = new Vector2(0, 0);
	private Sprite2D stick;
	private Sprite2D jsbase;
	private float clickRadius;
	private bool dragging = false;


	public override void _Ready()
	{
		stick = GetNode<Sprite2D>("Stick");

		jsbase = GetNode<Sprite2D>("JSBase");

		clickRadius = jsbase.Texture.GetWidth() * jsbase.Scale.X / 2;
	}

	public override void _Process(double delta)
	{
		//GD.Print(GetDirection());
	}
	
	public void SetScale(Vector2 scale){
		this.Scale = scale;
	}
	
	public override void _Input(InputEvent @event)
	{

		if (@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Left)
		{

			if (mouseEvent.Pressed && (mouseEvent.GlobalPosition - jsbase.GlobalPosition).Length() < clickRadius)
			{
				dragging = true;
			}

			else
			{
				dragging = false;
				stick.GlobalPosition = jsbase.GlobalPosition;
				coordinates = new Vector2(0, 0);
			}

		}

		if (@event is InputEventMouseMotion motionEvent && dragging)
		{
			coordinates = motionEvent.GlobalPosition - jsbase.GlobalPosition;

	//		direction = coordinates;
			if (coordinates.Length() > clickRadius)
			{
				direction = coordinates.Normalized() * clickRadius;
			}
			else
			{
				direction = coordinates;
			}
			stick.GlobalPosition = jsbase.GlobalPosition + direction;

		}

	}

	public Vector2 GetDirection()
	{
		return coordinates.Normalized(); 
	}
}
