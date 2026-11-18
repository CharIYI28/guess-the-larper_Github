using Godot;
using System;
using System.Runtime.InteropServices.Marshalling;

public partial class Finalscene : Node2D
{
	[Export] private Label labelname;
	[Export] private Label title;
	[Export] private Timer announce;
	[Export] private Label answer;
	public override void _Ready()
	{
		// labelname.Text = GameMechanics.instance.mode;
		GameMechanics.instance.showchecker += submitname;
		GameMechanics.instance.sendresult += sentnamesubmit;
		announce.Timeout += ontimeout;
		GameMechanics.instance.initiatemode();
		announce.Start();
		answer.Visible = false;
	}

	public void submitname()
	{
		if (GameMechanics.instance.mode == GameManagerID.Instance.mycustomid)
		{
			GameMechanics.instance.recievname(GameMechanics.instance.myname, GameManagerID.Instance.mycustomid);
		}
	}

	public void sentnamesubmit(string name, int id)
	{
		title.Text = $"{name} is a...";
		if (GameMechanics.instance.thechosen == id)
		{
			answer.Text = "LARPER";
		} else
		{
			answer.Text = "NOT LARPER";
		}
	}

	private void ontimeout()
	{
		announce.Stop();
		answer.Visible = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
