using Godot;
using System;

public partial class Lobby : Node2D
{
	[Export] private Label idlabel;
	[Export] private Button copy;
	[Export] private ItemList playerlist;
	[Export] private Button leave;
	[Export] private Button start;
	[Export] private Button buttonadd;
	[Export] private Button buttonminus;
	[Export] private Label labelnummax;
	[Export] private Label maximumtext;
	
	string currentid = "";
	public override void _Ready()
	{
		start.Visible = false;
		copy.Visible = false;
		buttonadd.Visible = false;
		buttonminus.Visible = false;
		labelnummax.Visible = false;
		maximumtext.Visible = false;

		bool isAlreadyConnected = Multiplayer.MultiplayerPeer != null &&
		!(Multiplayer.MultiplayerPeer is OfflineMultiplayerPeer) &&
		Multiplayer.MultiplayerPeer.GetConnectionStatus() == MultiplayerPeer.ConnectionStatus.Connected;

		if (NetworkManager.instance.hoster == true)
		{
			copy.Visible = true;
			start.Visible = true;
			buttonadd.Visible = true;
			buttonminus.Visible = true;
			labelnummax.Visible = true;
			maximumtext.Visible = true;

			if (!isAlreadyConnected)
			{
				string roomid = NetworkManager.instance.HostRoom(NetworkManager.instance.localplayername);
				NetworkManager.instance.currentroomid = roomid;
				currentid = roomid;
			} else
			{
				currentid = NetworkManager.instance.currentroomid;
			}
			idlabel.Text = $"Status: Hosting Room ID: {currentid}";
			
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
		buttonadd.Pressed += buttonaddpress;
		buttonminus.Pressed += buttonminuspress;

		NetworkManager.instance.listupdate += listupdater;
		listupdater();
		labelnummax.Text = $"{NetworkManager.instance.players.Count}";
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
			buttonadd.Pressed -= buttonaddpress;
			buttonminus.Pressed -= buttonminuspress;
			start.Pressed -= PressStartButton;
		}
	}

	private void buttonaddpress()
	{
		if (NetworkManager.instance.maxplayer < 5)
		{
			NetworkManager.instance.maxplayer += 1;
			labelnummax.Text = $"{NetworkManager.instance.maxplayer}";
		}
	}

	private void buttonminuspress()
	{
		if (NetworkManager.instance.maxplayer > NetworkManager.instance.players.Count)
		{
			NetworkManager.instance.maxplayer -= 1;
			labelnummax.Text = $"{NetworkManager.instance.maxplayer}";
		}
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
