using Godot;
using System;

public partial class Base : Node2D
{
	[Export] private Button hostbutton;
	[Export] private Button joinbutton;
	[Export] private LineEdit lineid;
	[Export] private Label labeljoin;
	[Export] private Button copy;
	[Export] private LineEdit nameinput;
	
	private string currentid = "";
	public override void _Ready()
	{
		hostbutton.Pressed += PressHostButton;
		joinbutton.Pressed += PressJoinButton;
		copy.Pressed += PressCopyButton;

		copy.Visible = false;

		NetworkManager.instance.sucessconnection += HandleSuccessConnect;
		NetworkManager.instance.failedconnection += HandleFailedConnect;
	}

    public override void _ExitTree()
    {
        if (NetworkManager.instance != null)
        {
            NetworkManager.instance.sucessconnection -= HandleSuccessConnect;
			NetworkManager.instance.failedconnection -= HandleFailedConnect;
        }

		hostbutton.Pressed -= PressHostButton;
		joinbutton.Pressed -= PressJoinButton;
		copy.Pressed -= PressCopyButton;
    }

	private void PressHostButton()
	{
		// string roomid = NetworkManager.instance.HostRoom();
		// labeljoin.Text = $"Status: Hosting Room ID: {roomid}";
		// currentid = roomid;
		// copy.Visible = true;
		// GD.Print($"currentid is {currentid}");
		if (nameinput.Text == "")
		{
			labeljoin.Text = "Please enter your name";
		} else
		{
			NetworkManager.instance.hoster = true;
			GetTree().ChangeSceneToFile("res://lobby.tscn");
			NetworkManager.instance.localplayername = nameinput.Text.Trim();
			GameMechanics.instance.myname = nameinput.Text.Trim();
		}
		
	}

	private void HandleSuccessConnect()
	{
		GetTree().ChangeSceneToFile("res://lobby.tscn");
	}

	private void HandleFailedConnect()
	{
		labeljoin.Text = "error";
	}

	private void PressCopyButton()
	{
		DisplayServer.ClipboardSet(currentid);
	}
	private async void PressJoinButton()
	{
		if (nameinput.Text == "")
		{
			labeljoin.Text = "Please insert your name";
		} else
		{
			string roomid = lineid.Text.Trim();
			string name = (nameinput != null && !string.IsNullOrWhiteSpace(nameinput.Text))
				? nameinput.Text.Trim()
				: "player";
			if (!string.IsNullOrEmpty(roomid))
			{
				NetworkManager.instance.localplayername = name;
				GameMechanics.instance.myname = name;
				await NetworkManager.instance.JoinRoom(roomid,name);
				labeljoin.Text = $"Status: Connecting to {roomid}...";
				
			}
		}
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
