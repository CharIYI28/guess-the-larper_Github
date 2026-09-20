using Godot;
using System;

public partial class Voting : Node2D
{
	[Export] public Godot.Collections.Array<Button> buttons = new Godot.Collections.Array<Button>();
	
	private Button selectedbtn;
	private Button newbtn;
	private StyleBoxFlat selectedStyle;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		selectedStyle = new StyleBoxFlat();
        selectedStyle.BgColor = new Color(0.18f, 0.55f, 0.34f); // Dark Green
        selectedStyle.CornerRadiusTopLeft = 5;
        selectedStyle.CornerRadiusTopRight = 5;
        selectedStyle.CornerRadiusBottomLeft = 5;
        selectedStyle.CornerRadiusBottomRight = 5;
		for (int i = 0; i < buttons.Count; i++)
		{
			Button btn = buttons[i];
			int index = i;
			btn.Pressed += () => Onpressed(index);
		}
	}

	private void Onpressed(int num)
	{
		newbtn = buttons[num];

		if (selectedbtn != null)
		{
			selectedbtn.RemoveThemeStyleboxOverride("normal");
		}

		newbtn.AddThemeStyleboxOverride("normal", selectedStyle);
		selectedbtn = newbtn;
		GD.Print("btn");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
