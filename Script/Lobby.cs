using Godot;
using System;

public partial class Lobby : Node2D
{
	[Export] private Label idlabel;
	[Export] private Button copy;
	[Export] private ItemList playerlist;
	[Export] private Button leave;
	[Export] private Button start;
	
	string currentid = "";
	public override void _Ready()
	{
		start.Visible = false;
		copy.Visible = false;

		if (NetworkManager.instance.hoster == true)
		{
			copy.Visible = true;
			start.Visible = true;
			string roomid = NetworkManager.instance.HostRoom(NetworkManager.instance.localplayername);
			idlabel.Text = $"Status: Hosting Room ID: {roomid}";
			currentid = roomid;
		}
		else
		{
			idlabel.Text = "Waiting for the host";
			long myid = Multiplayer.GetUniqueId();
			NetworkManager.instance.Registerid(myid, NetworkManager.instance.localplayername);
		}
		copy.Pressed += PressCopyButton;
		leave.Pressed += PressLeaveButton;
		start.Pressed += PressStartButton;

		NetworkManager.instance.listupdate += listupdater;
		listupdater();
	}

	private void PressStartButton()
	{
		GameManagerID.Instance.Onhostclickstart();
		GameMechanics.instance.starting_point();
	}
	private void listupdater()
	{
		if (NetworkManager.instance == null || playerlist == null)
		{
			return;
		}
		playerlist.Clear();

		foreach(var entry in NetworkManager.instance.players)
		{
			long peerID = entry.Key;
			string name = (string)entry.Value["name"];

			string labeltext = $"{name}";
			if (peerID == Multiplayer.GetUniqueId())
			{
				labeltext += "[YOU]";
			}
			if (peerID == 1)
			{
				labeltext += "[HOST]";
			}
			playerlist.AddItem(labeltext);
		}
	}

	private void PressCopyButton()
	{
		DisplayServer.ClipboardSet(currentid);
	}

	private void PressLeaveButton()
	{
		NetworkManager.instance.leaveroom();
		GetTree().ChangeSceneToFile("res://base.tscn");
	}

	public override void _ExitTree()
	{
		if (NetworkManager.instance != null)
		{
			NetworkManager.instance.listupdate -= listupdater;
			copy.Pressed -= PressCopyButton;
			leave.Pressed -= PressLeaveButton;
		}

	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
