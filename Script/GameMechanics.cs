using Godot;
using System;
using System.Collections.Generic;

public partial class GameMechanics : Node
{
	public Dictionary<long, Godot.Collections.Dictionary> playersingame = new Dictionary<long, Godot.Collections.Dictionary>();
	public Godot.Collections.Array<int> ids = new Godot.Collections.Array<int>();
	public static GameMechanics instance {get; private set;}
	private Random _random = new Random();
	public event System.Action<bool> textchanger;
	public int thechosen;
	public override void _Ready()
	{
		instance = this;
	}

	public void starting_point()
	{
		// ids.Clear();
		Rpc(MethodName.launch_start);

	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private void launch_start()
	{
		// thechosen = chosenid;
		GetTree().ChangeSceneToFile("res://start.tscn");
		// ids = GameManagerID.Instance.customids;
		GD.Print(GameManagerID.Instance.customids);
		GD.Print(thechosen);
	}

	public void larperchecker()
	{
		if (thechosen == GameManagerID.Instance.mycustomid)
		{
			textchanger?.Invoke(true);
			GD.Print("larper i am");
		}
		else
		{
			textchanger?.Invoke(false);
			GD.Print("real fan");
		}
	}
	
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private void trustedchecker()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
