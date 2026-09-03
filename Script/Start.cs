using Godot;
using System;
using System.Collections.Generic;

public partial class Start : Node2D
{
	// this is a stest for a github
	[Export] private Label decision;
	[Export] private Timer timer;
	[Export] private Timer timer2;
	[Export] private Timer timer3;
	public bool timeoutvar = true;
	private bool judgeholder = false;
	private int num1 = 0;
	private int num2 = -1;
	private Godot.Collections.Array<string> liststring = new Godot.Collections.Array<string>();
	public override void _Ready()
	{
		GameMechanics.instance.textchanger += Ontextchanger;
		GameMechanics.instance.larperchecker();	
		timer.Timeout += OnTimeout;
		timer2.Timeout += OnTimeout2;
		timer3.Timeout += OnTimeout3;
		timeoutvar = false;
		timer.Start();
		timer2.Start();
		// if (timeoutvar == false)
		// {
		// 	whiletimehas();
		// }
		liststring.Add("Larper");
		liststring.Add("Real Fan");
	}

	public override void _ExitTree()
	{
		GameMechanics.instance.textchanger -= Ontextchanger;
		timer.Timeout -= OnTimeout;
		timer2.Timeout -= OnTimeout2;
		timer3.Timeout -= OnTimeout3;
		num1 = 0;

	}

	private void OnTimeout3()
	{
		timer3.Stop();
		if (judgeholder == true)
		{
			GetTree().ChangeSceneToFile("res://Larper_page.tscn");
		}
		else
		{
			GetTree().ChangeSceneToFile("res://Real_page.tscn");
		}
	}

	private void OnTimeout()
	{
		timer.Stop();
		timer2.Stop();
		timeoutvar = true;
		Ontextchanger(judgeholder);
		timer3.Start();
	}

	private void OnTimeout2()
	{
		num1 += 1;
			if (num1 % 2 == 0)
			{
				decision.Text = "";
				decision.Text = liststring[0];
			}
			else
			{
				decision.Text = "";
				decision.Text = liststring[1];
			}
	}

	// private void whiletimehas()
	// {
	// 	for (int i = 0; timeoutvar == true; i*=-1)
	// 	{
	// 		GD.Print(i+(i*i));
	// 	}
	// }

	public void Ontextchanger(bool judge)
	{
		judgeholder = judge;
		if (timeoutvar == true)
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
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
