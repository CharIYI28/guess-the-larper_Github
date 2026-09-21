using Godot;
using System;

public partial class LarperPage : Node2D
{
	[Export] private Timer timer;
	[Export] private Label labeltime;
	private int currentmin = 10;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GameMechanics.instance.larpertimestart += Ontimestart;
		timer.Timeout += Ontimeout;
		labeltime.Visible = false;
	}

    public override void _ExitTree()
    {
        GameMechanics.instance.larpertimestart -= Ontimestart;
		timer.Timeout -= Ontimeout;
    }

	private void Ontimestart()
	{
		timer.Start();
		labeltime.Visible = true;
	}

	private void Ontimeout()
	{
		if (currentmin == 0)
		{
			GD.Print("time out");
			timer.Stop();
			GetTree().ChangeSceneToFile("res://voting.tscn");
		}
		else
		{
			currentmin -= 1;
		}
		labeltime.Text = $"{currentmin}";
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
