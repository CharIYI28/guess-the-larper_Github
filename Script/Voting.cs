using Godot;
using System;

public partial class Voting : Node2D
{
	[Export] public Godot.Collections.Array<Button> buttons = new Godot.Collections.Array<Button>();
	[Export] private Timer timer;
	[Export] private Button votebutn;
	[Export] private Button endbtn;
	private Godot.Collections.Array<int> btnid = new Godot.Collections.Array<int>();
	private Button selectedbtn;
	private Button newbtn;
	private StyleBoxFlat selectedStyle;
	private int finalvote;
	private int finalfinalvote;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print(GameManagerID.Instance.realcustomids);
		GameMechanics.instance.namechange += NameChanger;
		timer.Timeout += Ontimeout;
		votebutn.Pressed += Onvotepressed;
		endbtn.Pressed += Onendpressed;
		GameMechanics.instance.votesignal += Onvotesignal;
		timer.Start();
		selectedStyle = new StyleBoxFlat();
        selectedStyle.BgColor = new Color(0.18f, 0.55f, 0.34f); // Dark Green
        selectedStyle.CornerRadiusTopLeft = 5;
        selectedStyle.CornerRadiusTopRight = 5;
        selectedStyle.CornerRadiusBottomLeft = 5;
        selectedStyle.CornerRadiusBottomRight = 5;
		for (int i = 0; i < buttons.Count; i++)
		{
			Button btn = buttons[i];
			int index = i;
			btn.Pressed += () => Onpressed(index);
			btn.Visible = false;
		}

		if (Multiplayer.IsServer())
		{
			endbtn.Visible = true;
		}
		else
		{
			endbtn.Visible = false;
		}

		// for (int i=0; i < GameManagerID.Instance.realcustomids.Count; i++)
		// {
		// 	Button btnn = buttons[i];
		// 	btnn.Visible = true;
		// 	btnid.Add(i);
		// 	if (GameManagerID.Instance.mycustomid == i)
		// 	{
		// 		GameMechanics.instance.name_giverremote(i, $"{GameMechanics.instance.myname}");
		// 	}
		// }
	}

	private void Ontimeout()
	{
		systemstaart();
		timer.Stop();
	}
	private void systemstaart()
	{
		for (int i=0; i < GameManagerID.Instance.realcustomids.Count; i++)
		{
			Button btnn = buttons[i];
			btnn.Visible = true;
			btnid.Add(i);
			if (GameManagerID.Instance.mycustomid == i)
			{
				GameMechanics.instance.name_giverremote(i, $"{GameMechanics.instance.myname}");
			}
		}
	}

	private void Onpressed(int num)
	{
		newbtn = buttons[num];

		if (selectedbtn != null)
		{
			selectedbtn.RemoveThemeStyleboxOverride("normal");
		}
		finalvote = num;
		newbtn.AddThemeStyleboxOverride("normal", selectedStyle);
		selectedbtn = newbtn;
		GD.Print("btn");
		GD.Print(btnid[num]);
	}

	private void Onvotepressed()
	{
		finalfinalvote = finalvote;
	}

	private void Onendpressed()
	{
		GameMechanics.instance.endcaller();
	}

	private void Onvotesignal()
	{
		GameMechanics.instance.votereceive(finalfinalvote);
	}
	private void NameChanger(string name, int id)
	{
		buttons[id].Text = name;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
