using Godot;

public class Grass : Node2D
{
    //Variables
    private PackedScene GrassEffect;

    private Node2D grassEffect;

    //Fonctions
    public void HurtboxAreaEntered(Godot.Area2D area)
    {
        CreateGrassEffect();
        QueueFree();
    }

    public void CreateGrassEffect()
    {
        grassEffect = GrassEffect.Instance() as Node2D;
        GetParent().AddChild(grassEffect);
        grassEffect.GlobalPosition = GlobalPosition;
        QueueFree();
    }

    public override void _Ready()
    {
        GrassEffect = GD.Load("Effects/GrassEffect.tscn") as PackedScene;
    }
}