using Godot;
using System;

public partial class Start : Node2D
{
	// this is a stest for a github
	[Export] private Label decision;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GameMechanics.instance.textchanger += Ontextchanger;
		GameMechanics.instance.larperchecker();	
	}

	public override void _ExitTree()
	{
		GameMechanics.instance.textchanger -= Ontextchanger;
	}

	public void Ontextchanger(bool judge)
	{
		if (judge == true)
		{
			GD.Print("larper here");
			decision.Text = "Larper";
		}
		else
		{
			GD.Print("real fan here");
			decision.Text = "Real Fan";
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
