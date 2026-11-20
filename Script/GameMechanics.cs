using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

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
	public int mode = 0;
	public event System.Action showchecker;
	public event System.Action<string, int> sendresult;
	public event System.Action leave;
	public string resultName = "";
	public int resultId = -99; // -99 = not calculated yet
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

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode =MultiplayerPeer.TransferModeEnum.Reliable)]
	private void votecollector(int vote)
	{
		votes.Add(vote);
		GD.Print("votes" + votes);

	}

	public void initiatemode()
	{
		CalculateMode(votes);
	}

	private void CalculateMode(Godot.Collections.Array<int> votes)
	{
		Dictionary<int, int> counts = new Dictionary<int, int>();
		foreach (int vote in votes)
		{
			if (counts.ContainsKey(vote))
			{
				counts[vote]++;
			}
			else
			{
				counts[vote] = 1;
			}
		}

		mode = votes[0];
		int maxcount = 0;

		foreach (var pair in counts)
		{
			GD.Print("pair-->" + pair.Value);
			if (pair.Value > maxcount)
			{
				maxcount = pair.Value;
				mode = pair.Key;
			}
			else if (pair.Value == maxcount)
			{
				mode = -1;
			}
		}
		GD.Print("mode: " + mode);
		showresultcaller();
	}

	public void showresultcaller()
	{
		// Rpc(MethodName.showresultname);
		// showchecker?.Invoke();
		Rpc(MethodName.clientsubname, mode);
	}

	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private void clientsubname(int calculatedmoded)
	{
		mode = calculatedmoded;
		showchecker?.Invoke();
	}

	public void recievname(string name, int id)
	{
		Rpc(MethodName.showresultname, name, id);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public void showresultname(string name, int id)
	{
		resultName = name;
		resultId = id;
		sendresult?.Invoke(name, id);
	}

	public void returncommand()
	{
		Rpc(MethodName.returning);

		GetTree().CreateTimer(0.1f).Timeout += () =>
		{
			returning();
		};
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private void returning()
	{
		GameManagerID.Instance.thechosen1 = -1;
		votes.Clear();
		resultName = "";
		resultId = -99;
		leave?.Invoke();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
