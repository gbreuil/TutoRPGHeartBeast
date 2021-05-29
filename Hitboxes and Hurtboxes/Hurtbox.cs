using Godot;

public class Hurtbox : Area2D
{
    //Variables
    private PackedScene HitEffect;

    private Node2D hitEffect;
    private Node2D main;
    private Timer timer;
    private CollisionShape2D collisionShape2D;

    [Signal] public delegate void InvincibilityEnded();

    [Signal] public delegate void InvincibilityStarted();

    public bool Invincible { get; set; }

    public Hurtbox()
    {
        SetInvincible(false);
    }

    public void SetInvincible(bool value)
    {
        Invincible = value;
        if (Invincible == true)
        {
            EmitSignal("InvincibilityStarted");
        }
        else
        {
            EmitSignal("InvincibilityEnded");
        }
    }

    //Fonctions
    public override void _Ready()
    {
        HitEffect = GD.Load("Effects/HitEffect.tscn") as PackedScene;
        timer = GetNode("Timer") as Timer;
        collisionShape2D = GetNode("CollisionShape2D") as CollisionShape2D;
    }

    public void CreateHitEffect(Area2D area)
    {
        hitEffect = HitEffect.Instance() as Node2D;
        main = GetTree().CurrentScene as Node2D;
        main.AddChild(hitEffect);
        hitEffect.GlobalPosition = GlobalPosition;
    }

    public void TimerTimeOut()
    {
        SetInvincible(false);
    }

    public void StartInvincibility(float duration)
    {
        SetInvincible(true);
        timer.Start(duration);
    }

    public void HurtboxInvincibilityStarted()
    {
        collisionShape2D.SetDeferred("disabled", true);
    }

    public void HurtboxInvincibilityEnded()
    {
        collisionShape2D.Disabled = false;
    }
}