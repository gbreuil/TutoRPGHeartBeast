using Godot;

public class Camera2D : Godot.Camera2D
{
    //Variables
    private Position2D topLeft;

    private Position2D bottomRight;

    public override void _Ready()
    {
        topLeft = GetNode("Limits/TopLeft") as Position2D;
        bottomRight = GetNode("Limits/BottomRight") as Position2D;
        LimitTop = (int)topLeft.Position.y;
        LimitLeft = (int)topLeft.Position.x;
        LimitBottom = (int)bottomRight.Position.y;
        LimitRight = (int)bottomRight.Position.x;
    }
}