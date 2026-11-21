using Godot;
using System;
using System.Collections.Generic;

public partial class Topic_template : Resource
{
    [Export] public string name;
    [Export] public string text;
    [Export] public Godot.Collections.Array<string> funfacts = new Godot.Collections.Array<string>();
}
