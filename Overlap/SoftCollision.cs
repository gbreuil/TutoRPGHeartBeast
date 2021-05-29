using Godot;

public class SoftCollision : Area2D
{
    //Variables
    public bool Collision { get; set; }

    public Area2D Area { get; set; }

    //Functions

    public SoftCollision()
    {
        SetCollision(false);
    }

    public void SoftCollisionAreaEntered(Area2D area)
    {
        SetCollision(true);
        Area = area;
    }

    public void SoftCollisionAreaExited(Area2D area)
    {
        SetCollision(false);
        Area = area;
    }

    public void SetCollision(bool value)
    {
        Collision = value;
    }
}