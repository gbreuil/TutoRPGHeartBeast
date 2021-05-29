using Godot;

public class Effect : AnimatedSprite
{
    //Variables

    //Fonctions

    public override void _Ready()
    {
        Connect("animation_finished", this, "AnimationFinished");
        Play("Animate");
    }

    public void AnimationFinished()
    {
        QueueFree();
    }
}