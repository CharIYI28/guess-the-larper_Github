using Godot;
using System;

public partial class RealPage : Node2D
{
	[Export] private Timer drumrolling;
	[Export] private Timer tothemain;
	[Export] private Label topic_label;
	[Export] private Label label;
	[Export] private Label topic__;
	private Random _random = new Random();
	private int random_holder = 0;
	[Export] private Godot.Collections.Array<Topic_template> topics = new Godot.Collections.Array<Topic_template>();
	[Export] private Label maincontent;
	[Export] private Timer maintimer;
	[Export] private Label timer;
	private int alltopics = 0;

	private int currentmin = 10;

	public override void _Ready()
	{
		label.Visible = true;
		topic_label.Visible = true;
		// topic__.Visible = true;
		alltopics = topics.Count;
		GameMechanics.instance.topicrandom = topics.Count;
		GameMechanics.instance.randomshuffle();
		// random_holder = GD.RandRange(0, alltopics-1);
		// GD.Print(random_holder);
		drumrolling.Timeout += Ondrumout;
		tothemain.Timeout += Ontothemain;
		maintimer.Timeout += Onmaintimer;
		drumrolling.Start();
		maincontent.Visible = false;
		timer.Visible = false;
	}

    public override void _ExitTree()
    {
        drumrolling.Timeout -= Ondrumout;
    }

	private void Ondrumout()
	{
		random_holder = GameMechanics.instance.currentholder;
		topic_label.Text = $"{topics[random_holder].name}";
		drumrolling.Stop();
		tothemain.Start();
	}

	private void Ontothemain()
	{
		tothemain.Stop();
		maincontent.Text = $"{topics[random_holder].text}";
		maincontent.Visible = true;
		timer.Visible = true;
		label.Visible = false;
		// topic__.Visible = false;
		topic_label.Visible = false;
		maintimer.Start();
		// if (Multiplayer.IsServer())
		// {
		// 	Rpc(MethodName.sender);
		// }
		GameMechanics.instance.larpertimer();
	}
	
	// [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	// private void sender()
	// {
	// 	if (GameMechanics.instance.larper_ == true)
	// 	{
	// 		GameMechanics.instance.larpertimer();
	// 	}
	// }

	private void Onmaintimer()
	{		
		if (currentmin == 0)
		{
			GD.Print("time out");
			maintimer.Stop();
		}
		else
		{
			currentmin -= 1;
		}
		timer.Text = $"{currentmin}";
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
