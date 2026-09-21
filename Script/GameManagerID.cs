using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class GameManagerID : Node
{
	public static GameManagerID Instance {get; private set;}
	// private Godot.Collections.Array<long> _verifiedids = new Godot.Collections.Array<long>();
	public int mycustomid {get; private set;} = -1;
	private readonly HashSet<long> _verifiedIds = new HashSet<long>();
	public Godot.Collections.Array<int> customids = new Godot.Collections.Array<int>();
	public Godot.Collections.Array<int> realcustomids = new Godot.Collections.Array<int>();
	private Random _random = new Random();
	private int thechosen1;
	public override void _Ready()
	{
		Instance = this;
		// Multiplayer.PeerConnected += PeerConnected;
	}

	public void Onhostclickstart()
	{
		customids.Clear();
		if (!Multiplayer.IsServer()) return;
		var rawpeer = Multiplayer.GetPeers();

		int totalplayers = rawpeer.Length + 1;
		for (int i = 0; i < totalplayers; i++)
		{
			customids.Add(i);
			
		}

		thechosen1 = customids[_random.Next(customids.Count)];

		int hostcustomids = 0;

		for (int i = 0; i < rawpeer.Length; i++)
		{
			long clientpeerid = rawpeer[i];
			int clientcustomid = i + 1;

			RpcId(clientpeerid, MethodName.startmatchclient, customids, clientcustomid, thechosen1);
		}
		startmatchclient(customids, hostcustomids, thechosen1);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private void startmatchclient(Godot.Collections.Array<int> allcustomids, int assignedid, int chosen)
	{
		realcustomids = allcustomids;
		mycustomid = assignedid;
		GD.Print($"my id:{mycustomid}");
		GD.Print($"all ids:{allcustomids}");
		GD.Print($"chosen:{chosen}");
		GameMechanics.instance.ids = allcustomids;
		GameMechanics.instance.thechosen = chosen;
		GameMechanics.instance.myid = mycustomid;
	}

    // private void PeerConnected(long id)
	// {
	// 	if (Multiplayer.IsServer())
	// 	{
	// 		RpcId(id, MethodName.Requesthandshake);
	// 	}
	// }

	// [Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	// private void Requesthandshake()
	// {
	// 	RpcId(1, MethodName.Confirmhandshake);
	// }

	// [Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	// private void Confirmhandshake()
	// {
	// 	if (!Multiplayer.IsServer()) return;

	// 	long realid = Multiplayer.GetRemoteSenderId();

	// 	if (_verifiedIds.Add(realid))
	// 	{
	// 		GD.Print($"Successfully verified Peer ID: {realid}");
    //         GD.Print($"Current verified list: [{string.Join(", ", _verifiedIds)}]");
	// 		GD.Print(_verifiedIds);
	// 	}
	// }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
