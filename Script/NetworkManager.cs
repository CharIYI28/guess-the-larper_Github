using Godot;
using Godot.NativeInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class NetworkManager : Node
{
	private GodotObject _matchaRoom;
	public static NetworkManager instance {get; private set;}
	public bool hoster = false;
	
	public string localplayername = "player";
	public event System.Action sucessconnection;
	public event System.Action failedconnection;
	public event System.Action listupdate;
	private Callable _onjoinedmatcharoomcallable;
	private Callable _onfailcallabe;
	public Dictionary<long, Godot.Collections.Dictionary> players = new Dictionary<long, Godot.Collections.Dictionary>();
    public override void _Ready()
    {
		instance = this;
		GDScript matchaRoomClass = GD.Load<GDScript>("res://addons/matcha/MatchaRoom.gd");
		
		if (matchaRoomClass != null)
		{
			GD.Print("SUCCESS: godot-matcha is detected and ready!");
		}
		else
		{
			GD.PrintErr("ERROR: Could not find MatchaRoom.gd. Check your addons/matcha folder path.");
		}

		if (ClassDB.ClassExists("WebRTCPeerConnectionExtension"))
		{
			{
				GD.Print("SUCCESS: Godot 4.5 WebRTC GDExtension is active!");
			}
		}
		else
		{
			GD.PrintErr("ERROR: WebRTC GDExtension not found in res://webrtc/");
		}

		Multiplayer.PeerConnected += OnPeerConnected;
		Multiplayer.PeerDisconnected += OnPeerDisconnected;
		Multiplayer.ConnectedToServer += OnConnectedToServer;
		Multiplayer.ConnectionFailed += OnConnectedFailed;

		_onjoinedmatcharoomcallable = Callable.From(Onmatcharoomjoined);
		_onfailcallabe = Callable.From(Onmatchajoinfailed);
	}

	public string HostRoom(string playername)
	{
		cleanupoldpeer();
		players.Clear();
		localplayername = string.IsNullOrEmpty(playername)? "host" : playername;
		if (_matchaRoom != null && IsInstanceValid(_matchaRoom) && _matchaRoom is Node oldNode)
		{
			oldNode.QueueFree();
		}

		GDScript matchaClass = GD.Load<GDScript>("res://addons/matcha/MatchaRoom.gd");
		_matchaRoom = (GodotObject)matchaClass.Call("create_server_room");

		if (_matchaRoom is Node matchanode)
		{
			AddChild(matchanode);
		}

		Multiplayer.MultiplayerPeer = (MultiplayerPeer)_matchaRoom;
		string roomid = (string)_matchaRoom.Get("room_id");
		
		GD.Print($"[Host] Room created: {roomid}");

		Registerid(Multiplayer.GetUniqueId(), localplayername);
		GameMechanics.instance.ids.Add(Multiplayer.GetUniqueId());

		return roomid;
	}

	// async
	public async Task JoinRoom(string roomid, string playername)
	{
		cleanupoldpeer();
		// localplayername = string.IsNullOrWhiteSpace(playername)? "player" : playername;
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		GDScript matchaClass = GD.Load<GDScript>("res://addons/matcha/MatchaRoom.gd");
		_matchaRoom = (GodotObject)matchaClass.Call("create_client_room", roomid);

		// _matchaRoom.Connect("joined_room", _onjoinedmatcharoomcallable);
		// _matchaRoom.Connect("failed_to_join", _onfailcallabe);

		if (_matchaRoom is Node matchanode)
		{
			AddChild(matchanode);
		}
		Multiplayer.MultiplayerPeer = (MultiplayerPeer)_matchaRoom;

		GD.Print($"[Client] Joining room: {roomid}");
	}
	
	private void Onmatcharoomjoined()
	{
		// Multiplayer.MultiplayerPeer = (MultiplayerPeer)_matchaRoom;
		Registerid(Multiplayer.GetUniqueId(), localplayername);
		sucessconnection?.Invoke();
	}

	private void Onmatchajoinfailed()
	{
		failedconnection?.Invoke();
	}
	public void OnPeerConnected(long id)
	{
		long myid = Multiplayer.GetUniqueId();
		// GameMechanics.instance.ids.Add(myid);
		GD.Print($"[Event] Peer connected: {id}, My id: {myid}");
		RpcId(id, MethodName.Sendplayerinfo, localplayername);
	} 

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private void Sendplayerinfo(string name)
	{
		long senderid = Multiplayer.GetRemoteSenderId();
		// long senderid = Multiplayer.GetUniqueId();
		Registerid(senderid, name);
		RpcId(senderid, MethodName.Receiveplayerinfo, localplayername);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private void Receiveplayerinfo(string name)
	{
		long senderid = Multiplayer.GetRemoteSenderId();
		// long senderid = Multiplayer.GetUniqueId();
		Registerid(senderid, name);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private void Removeplayerinfo(long idremove)
	{
		GD.Print("remove");
		if (players.ContainsKey(idremove))
		{
			players.Remove(idremove);
			listupdate?.Invoke();
		}
	}

	public void Registerid(long id, string name)
	{
		// GD.Print(id);
		var info = new Godot.Collections.Dictionary{{"name", name}};
		players[id] = info;
		listupdate?.Invoke();
		// GD.Print(players);
	}

	private void OnPeerDisconnected(long id)
	{
		GD.Print($"Event Peer disconnected: {id}");
		// long myid = Multiplayer.GetUniqueId();
		// Registerid(myid, localplayername);

		if (players.ContainsKey(id))
		{
			players.Remove(id);
			listupdate?.Invoke();
		}
	} 
	private void OnConnectedToServer()
	{
		GD.Print($"event succesfully connected to host");
		GD.Print("this is a test");
		// Registerid(Multiplayer.GetUniqueId(), localplayername);
		sucessconnection?.Invoke();
	} 
	private void OnConnectedFailed()
	{
		GD.PrintErr("connection failed");
		failedconnection?.Invoke();
	} 

	public void leaveroom()
	{
    	if (Multiplayer.MultiplayerPeer != null && 
            Multiplayer.MultiplayerPeer.GetConnectionStatus() == MultiplayerPeer.ConnectionStatus.Connected)
        {
			long myId = Multiplayer.GetUniqueId();
            Rpc(MethodName.Removeplayerinfo, myId);
        }
		cleanupoldpeer();
		
		// if (Multiplayer.MultiplayerPeer != null)
		// {
		// 	Multiplayer.MultiplayerPeer.Close();
		// 	Multiplayer.MultiplayerPeer = null;
		// }
		hoster = false;
		players.Clear();
		// _matchaRoom = null;
	}

	private void cleanupoldpeer()
	{
		Multiplayer.MultiplayerPeer = new OfflineMultiplayerPeer();
		if (_matchaRoom != null && IsInstanceValid(_matchaRoom))
		{
			if (_matchaRoom is Node oldnode)
			{
				oldnode.SetProcess(false);
				oldnode.SetPhysicsProcess(false);
			}

			if (_matchaRoom.HasMethod("close"))
			{
				_matchaRoom.Call("close");
			}

			// Unhook signals to avoid stale triggers
			// if (_matchaRoom.IsConnected("joined_room", Callable.From(Onmatcharoomjoined)))
			// {
			// 	_matchaRoom.Disconnect("joined_room", Callable.From(Onmatcharoomjoined));
			// }
			// if (_matchaRoom.IsConnected("failed_to_join", Callable.From(Onmatchajoinfailed)))
			// {
			// 	_matchaRoom.Disconnect("failed_to_join", Callable.From(Onmatchajoinfailed));
			// }

			if (_matchaRoom.HasMethod("close"))
			{
				_matchaRoom.Call("close");
			}
			else if (_matchaRoom.HasMethod("disconnect_from_room"))
			{
				_matchaRoom.Call("disconnect_from_room");
			}

			if (_matchaRoom is Node nodeToFree)
			{
				if (nodeToFree.GetParent() != null)
				{
					nodeToFree.GetParent().RemoveChild(nodeToFree);
				}
				nodeToFree.QueueFree();
			}
		}
		Multiplayer.MultiplayerPeer = new OfflineMultiplayerPeer();
		_matchaRoom = null;
	}
}