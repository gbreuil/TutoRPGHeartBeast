using Godot;

public class Player : KinematicBody2D
{
    //Variables

    private enum PlayerState
    { //Etat du joueur
        MOVE,
        ROLL,
        ATTACK
    };

    private AnimationPlayer animationPlayer;
    private AnimationPlayer blinkAnimationPlayer;
    private AnimationTree animationTree;
    private AnimationNodeStateMachinePlayback animationState;
    private Hurtbox hurtbox;
    private PackedScene PlayerHurtSound;
    private AudioStreamPlayer playerHurtSound;
    private Vector2 velocity = Vector2.Zero;
    private Vector2 roll_vector = Vector2.Down;
    private PlayerState state = PlayerState.MOVE;
    private PlayerStats PlayerHealth = new PlayerStats();
    private BatHitbox batHitbox = new BatHitbox();

    [Export] private int MaxSpeed = 80;
    [Export] private int RollSpeed = 100;
    [Export] private int Acceleration = 500;
    [Export] private int Friction = 500;

    //Fonctions
    public override void _Ready()
    {
        animationPlayer = GetNode("AnimationPlayer") as AnimationPlayer;
        blinkAnimationPlayer = GetNode("BlinkAnimationPlayer") as AnimationPlayer;
        animationTree = GetNode("AnimationTree") as AnimationTree;
        animationState = animationTree.Get("parameters/playback") as AnimationNodeStateMachinePlayback;
        animationTree.Active = true;
        PlayerHealth = GetNode("/root/PlayerStats") as PlayerStats; // Loading singleton PlayerStats
        PlayerHealth.Connect("NoHealth", this, "PlayerNoHealth");
        hurtbox = GetNode("Hurtbox") as Hurtbox;
        PlayerHurtSound = GD.Load("Player/PlayerHurtSound.tscn") as PackedScene;
    }

    public void PlayerNoHealth()
    {
        playerHurtSound = PlayerHurtSound.Instance() as AudioStreamPlayer;
        GetTree().CurrentScene.AddChild(playerHurtSound);
        playerHurtSound.Playing = true;
        QueueFree();
    }

    public void MoveState(float delta) //Player in movement
    {
        var input_vector = Vector2.Zero;
        input_vector.x = Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left");
        input_vector.y = Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up");
        input_vector = input_vector.Normalized();

        if (input_vector != Vector2.Zero)
        {
            roll_vector = input_vector;
            animationTree.Set("parameters/Idle/blend_position", input_vector);
            animationTree.Set("parameters/Run/blend_position", input_vector);
            animationTree.Set("parameters/Attack/blend_position", input_vector);
            animationTree.Set("parameters/Roll/blend_position", input_vector);
            animationState.Travel("Run");
            velocity = velocity.MoveToward(input_vector * MaxSpeed, Acceleration * delta);
        }
        else
        {
            animationState.Travel("Idle");
            velocity = velocity.MoveToward(Vector2.Zero, Friction * delta);
        }

        Move();

        if (Input.IsActionJustPressed("player_roll"))
        {
            state = PlayerState.ROLL;
        }

        if (Input.IsActionJustPressed("player_attack"))
        {
            state = PlayerState.ATTACK;
        }
    }

    public void RollState(float delta) // Player Rolling
    {
        velocity = roll_vector * RollSpeed;
        animationState.Travel("Roll");
        Move();
        hurtbox.StartInvincibility(0.5F);
    }

    public void AttackState(float delta) //Player Attack (SPACE)
    {
        animationState.Travel("Attack");
        velocity = Vector2.Zero;
    }

    public void Move()
    {
        velocity = MoveAndSlide(velocity);
    }

    public void AttackAnimationFinished()
    {
        state = PlayerState.MOVE;
    }

    public void RollAnimationFinished()
    {
        velocity = Vector2.Zero;
        state = PlayerState.MOVE;
    }

    public void HurtboxAreaEntered(Area2D area)
    {
        PlayerHealth.Health -= batHitbox.Damage;
        hurtbox.StartInvincibility(1F);
        hurtbox.CreateHitEffect(area);
    }

    public void HurtboxInvincibilityEnded()
    {
        blinkAnimationPlayer.Play("Stop");
    }

    public void HurtboxInvincibilityStarted()
    {
        blinkAnimationPlayer.Play("Start");
    }

    public override void _Process(float delta)
    {
        switch (state)
        {
            case PlayerState.MOVE:
                MoveState(delta);
                break;

            case PlayerState.ROLL:
                RollState(delta);
                break;

            case PlayerState.ATTACK:
                AttackState(delta);
                break;

            default:
                GD.Print("NO STATE USED");
                break;
        }
    }
}