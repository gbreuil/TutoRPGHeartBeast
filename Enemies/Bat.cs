using Godot;
using System.Collections.Generic; //To use List<T>()
using System.Linq; // To use Cast<T>()

public class Bat : KinematicBody2D
{
    //Variables
    private Node2D root;

    private string BatName;
    private PlayerDetectionZone player_detection_zone;
    private PackedScene EnemyDeathEffect;
    private Node2D enemyDeathEffect;
    private AnimatedSprite animatedSprite;
    private Hurtbox hurtbox;
    private Vector2 velocity = Vector2.Zero;
    private Vector2 knockback = Vector2.Zero;
    private Vector2 direction = Vector2.Zero;
    private BatState state = BatState.CHASE;
    private Stats BatHealth = new Stats();
    private SwordHitbox Sword = new SwordHitbox();
    private SoftCollision softCollision;
    private WanderController wanderController;
    private RandomNumberGenerator rng = new RandomNumberGenerator();
    private AnimationPlayer blinkAnimationPlayer;

    public enum BatState
    {
        IDLE, //Vol stationnaire
        WANDER, //Vol libre
        CHASE //Poursuit le joueur
    };

    private List<BatState> stateList = System.Enum.GetValues(typeof(BatState)).Cast<BatState>().ToList(); //Cree une liste a partir de l'enumeration en utilisant les valeurs

    [Export] private int MaxSpeed = 40;
    [Export] private int Knockback = 120;
    [Export] private int Acceleration = 500;
    [Export] private int Friction = 200;
    [Export] private int Pv = 5;

    //Fonctions
    public override void _Ready()
    {
        rng.Randomize();
        root = GetTree().CurrentScene as Node2D;
        player_detection_zone = GetNode("PlayerDetectionZone") as PlayerDetectionZone;
        animatedSprite = GetNode("AnimatedSprite") as AnimatedSprite;
        hurtbox = GetNode("Hurtbox") as Hurtbox;
        EnemyDeathEffect = GD.Load("Effects/EnemyDeathEffect.tscn") as PackedScene;
        BatHealth.Connect("NoHealth", this, "BatNoHealth");
        softCollision = GetNode("SoftCollision") as SoftCollision;
        wanderController = GetNode("WanderController") as WanderController;
        blinkAnimationPlayer = GetNode("BlinkAnimationPlayer") as AnimationPlayer;
        BatHealth.Health = Pv;
        BatName = this.Name;
        PickRandomState(stateList);
    }

    public void BatNoHealth()
    {
        QueueFree();
        enemyDeathEffect = EnemyDeathEffect.Instance() as Node2D;
        GetParent().AddChild(enemyDeathEffect);
        enemyDeathEffect.GlobalPosition = GlobalPosition;
    }

    public void HurtboxAreaEntered(Area2D area)
    {
        if (BatHealth.Health - Sword.Damage >= 0)
        {
            BatHealth.Health -= Sword.Damage;
        }
        else
        {
            BatHealth.Health = 0;
        }
        Node2D player = root.GetNode("YSort/Player") as Node2D;
        Vector2 direction = GlobalPosition - player.GlobalPosition;
        knockback = direction.Normalized() * Knockback;
        hurtbox.CreateHitEffect(area);
        hurtbox.StartInvincibility(0.4f);
    }

    public void HurtboxInvincibilityStarted()
    {
        blinkAnimationPlayer.Play("Start");
    }

    public void HurtboxInvincibilityEnded()
    {
        blinkAnimationPlayer.Play("Stop");
    }

    public void SeekPlayer()
    {
        if (player_detection_zone.CanSeePlayer())
        {
            state = BatState.CHASE;
        }
    }

    public BatState PickRandomState(List<BatState> stateListRandom)
    {
        List<BatState> shuffled = stateListRandom;
        shuffled.Remove(BatState.CHASE); //Eviter de chercher le joueur pour la liste random
        shuffled = shuffled.OrderBy(x => System.Guid.NewGuid()).ToList(); //Melange une liste a partir d'un GUID
        var t = shuffled.First();
        return t;
    }

    public override void _PhysicsProcess(float delta)
    {
        knockback = knockback.MoveToward(Vector2.Zero, Friction * delta);
        knockback = MoveAndSlide(knockback);
        switch (state)
        {
            case BatState.IDLE:
                velocity = velocity.MoveToward(Vector2.Zero * MaxSpeed, Friction * delta).Normalized();
                SeekPlayer();
                if (wanderController.GetTimeLeft() == 0)
                {
                    state = PickRandomState(stateList);
                    wanderController.StartWanderTimer(rng.RandiRange(1, 3));
                }
                break;

            case BatState.WANDER:
                SeekPlayer();
                if (wanderController.GetTimeLeft() == 0)
                {
                    state = PickRandomState(stateList);
                }
                wanderController.StartWanderTimer(rng.RandiRange(1, 3));
                direction = Position.DirectionTo(wanderController.GetTargetPosition());
                velocity = velocity.MoveToward(direction * MaxSpeed, Acceleration * delta);
                animatedSprite.FlipH = velocity.x < 0;

                if (GlobalPosition.DistanceTo(wanderController.GetTargetPosition()) <= (MaxSpeed * delta))
                {
                    state = PickRandomState(stateList);
                    wanderController.StartWanderTimer(rng.RandiRange(1, 3));
                }
                break;

            case BatState.CHASE:
                var player = player_detection_zone.Player;
                if (player != null)
                {
                    direction = Position.DirectionTo(player.GlobalPosition);
                    velocity = velocity.MoveToward(direction * MaxSpeed, Acceleration * delta);
                }
                else
                {
                    state = BatState.IDLE;
                }
                animatedSprite.FlipH = velocity.x < 0;
                break;

            default:
                GD.Print("NO STATE USED");
                break;
        }
        if (softCollision.Collision == true)
        {
            velocity += softCollision.Area.GlobalPosition.DirectionTo(GlobalPosition).Normalized() * delta * 600;
        }
        velocity = MoveAndSlide(velocity);
    }
}