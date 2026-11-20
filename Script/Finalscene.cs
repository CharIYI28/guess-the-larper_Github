using Godot;
using System;
using System.Runtime.InteropServices.Marshalling;

public partial class Finalscene : Node2D
{
	[Export] private Label labelname;
	[Export] private Label title;
	[Export] private Timer announce;
	[Export] private Label answer;
	[Export] private Button endbtn;
	[Export] private Timer leavetimer;
	public override void _Ready()
	{
		// labelname.Text = GameMechanics.instance.mode;
		GameMechanics.instance.showchecker += submitname;
		GameMechanics.instance.sendresult += sentnamesubmit;
		GameMechanics.instance.leave += leaving;
		announce.Timeout += ontimeout;
		// GameMechanics.instance.initiatemode();
		endbtn.Pressed += endonpress;
		leavetimer.Timeout += leavetimeout;

		answer.Visible = false;
		endbtn.Visible = false;

		if (GameMechanics.instance.resultId != -99)
		{
			sentnamesubmit(GameMechanics.instance.resultName, GameMechanics.instance.resultId);
		} 

		if (Multiplayer.IsServer())
		{
			GetTree().CreateTimer(0.2f).Timeout += () =>
			{
				GameMechanics.instance.initiatemode();
			};
		}
		announce.Start();
	}

	public void submitname()
	{
		if (GameMechanics.instance.mode == -1)
		{
			if (Multiplayer.IsServer())
			{
				GameMechanics.instance.recievname("", -1);
			}
		}
		else if (GameMechanics.instance.mode == GameManagerID.Instance.mycustomid)
		{
			GameMechanics.instance.recievname(GameMechanics.instance.myname, GameManagerID.Instance.mycustomid);
		}
	}

    public override void _ExitTree()
    {
        GameMechanics.instance.showchecker -= submitname;
		GameMechanics.instance.sendresult -= sentnamesubmit;
		GameMechanics.instance.leave -= leaving;
		announce.Timeout -= ontimeout;
		endbtn.Pressed -= endonpress;
		leavetimer.Timeout -= leavetimeout;
    }


	public void sentnamesubmit(string name, int id)
	{
		title.Visible = true;
		title.Text = $"{name} is a...";
		if (GameMechanics.instance.thechosen == id)
		{
			answer.Text = "LARPER";
		} else if (id > -1)
		{
			answer.Text = "NOT LARPER";
		} else if (id == -1)
		{
			answer.Text = "TIE";
			title.Visible = false;
		}
	}

	private void ontimeout()
	{
		announce.Stop();
		answer.Visible = true;
		if (GameManagerID.Instance.mycustomid == 0)
		{
			endbtn.Visible = true;
		}
	}

	private void endonpress()
	{
		GameMechanics.instance.returncommand();
	}

	private void leaving()
	{
		leavetimer.Start();
	}

	private void leavetimeout()
	{
		leavetimer.Stop();
		GetTree().ChangeSceneToFile("res://lobby.tscn");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
