using Godot;

public class WanderController : Node2D
{
    //Variables
    [Export] private float wander_range = 32;

    private Vector2 startPosition;
    private Vector2 targetPosition;
    private RandomNumberGenerator rng = new RandomNumberGenerator();
    private Vector2 targetVector;
    private float randomFloat;
    private Timer timer;

    public override void _Ready()
    {
        rng.Randomize();
        startPosition = GlobalPosition;
        targetPosition = GlobalPosition;
        timer = GetNode("Timer") as Timer;
        UpdateTargetPosition();
    }

    public Vector2 GetTargetPosition()
    {
        return targetPosition;
    }

    public void SetTargetPosition(Vector2 value)
    {
        targetPosition = value;
    }

    public void UpdateTargetPosition()
    {
        randomFloat = rng.RandfRange(-wander_range, wander_range);
        targetVector = new Vector2(randomFloat, randomFloat);
        SetTargetPosition(startPosition + targetVector);
    }

    public float GetTimeLeft()
    {
        return timer.TimeLeft;
    }

    public void StartWanderTimer(float duration)
    {
        timer.Start(duration);
    }

    public void _on_Timer_timeout()
    {
        UpdateTargetPosition();
    }
}