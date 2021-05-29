using Godot;

public class PlayerDetectionZone : Area2D
{
    //Variables
    public Area2D Player { get; set; }

    //Functions

    public PlayerDetectionZone()
    {
        Player = null;
    }

    public bool CanSeePlayer()
    {
        return Player != null;
    }

    public void PlayerDetectionZoneBodyEntered(Area2D body)
    {
        Player = body;
    }

    public void PlayerDetectionZoneBodyExited(Area2D body)
    {
        Player = null;
    }
}