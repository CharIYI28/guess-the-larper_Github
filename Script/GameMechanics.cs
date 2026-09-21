using Godot;
using System;
using System.Collections.Generic;

public partial class GameMechanics : Node
{
	public Dictionary<long, Godot.Collections.Dictionary> playersingame = new Dictionary<long, Godot.Collections.Dictionary>();
	public Godot.Collections.Array<int> ids = new Godot.Collections.Array<int>();
	public Godot.Collections.Array<int> votes = new Godot.Collections.Array<int>();
	public static GameMechanics instance {get; private set;}
	private Random _random = new Random();
	public event System.Action<bool> textchanger;
	public event System.Action larpertimestart;
	public event System.Action<string, int> namechange;
	public event System.Action votesignal;
	public int thechosen;
	public bool larper_ = false;
	public int topicrandom = 0;
	public  int gamerandomholder = 0;
	public int myid = 0;
	public int currentholder = 0;
	public string myname = "";
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
			larper_ = true;
			GD.Print("larper i am");
		}
		else
		{
			textchanger?.Invoke(false);
			GD.Print("real fan");
		}
	}

	public void larpertimer()
	{
		Rpc(MethodName.caller);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private void caller()
	{
		if (larper_ == true)
		{
			larpertimestart?.Invoke();
		}
	}

	public void randomshuffle()
	{
		if (!Multiplayer.IsServer()) return;
		gamerandomholder = GD.RandRange(0, topicrandom-1);
		Rpc(MethodName.sendtopic, gamerandomholder);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private void sendtopic(int number)
	{
		currentholder = number;
		GD.Print(currentholder);
	}

	public void name_giverremote(int num, string name)
	{
		Rpc(MethodName.receiveidname, num, name);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal =true, TransferMode =MultiplayerPeer.TransferModeEnum.Reliable)]
	private void receiveidname(int id, string name)
	{
		namechange?.Invoke(name, id);
	}

	public void endcaller()
	{
		Rpc(MethodName.sendingsignals);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal =true, TransferMode =MultiplayerPeer.TransferModeEnum.Reliable)]
	private void sendingsignals()
	{
		votesignal?.Invoke();
	}

	public void votereceive(int vote)
	{
		Rpc(MethodName.votecollector, vote);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal =true, TransferMode =MultiplayerPeer.TransferModeEnum.Reliable)]
	private void votecollector(int vote)
	{
		votes.Add(vote);
		GD.Print(votes);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
