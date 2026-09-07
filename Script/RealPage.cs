using Godot;
using System;

public partial class RealPage : Node2D
{
	[Export] private Timer drumrolling;
	[Export] private Timer tothemain;
	[Export] private Label topic_label;
	private Random _random = new Random();
	private int random_holder = 0;
	[Export] private Godot.Collections.Array<Topic_template> topics = new Godot.Collections.Array<Topic_template>();
	[Export] private Label maincontent;
	[Export] private Timer maintimer;
	private int alltopics = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		alltopics = topics.Count;
		random_holder = GD.RandRange(0, alltopics-1);
		GD.Print(random_holder);
		drumrolling.Timeout += Ondrumout;
		tothemain.Timeout += Ontothemain;
		drumrolling.Start();
		maincontent.Visible = false;
	}

    public override void _ExitTree()
    {
        drumrolling.Timeout -= Ondrumout;
    }

	private void Ondrumout()
	{
		topic_label.Text = $"{topics[random_holder].name}";
		drumrolling.Stop();
		tothemain.Start();
	}

	private void Ontothemain()
	{
		maincontent.Text = $"{topics[random_holder].text}";
		maincontent.Visible = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
