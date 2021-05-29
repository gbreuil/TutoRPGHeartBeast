using Godot;

public class Stats : Node
{
    //Variables
    private int max_health;

    private int health;

    [Signal] public delegate void NoHealth();

    //Constructeur
    public Stats()
    {
        max_health = 1;
        health = max_health;
    }

    //Fonctions
    public int MaxHealth
    {
        get => max_health;
        set
        {
            max_health = value;
            Health = max_health;
        }
    }

    public int Health
    {
        get => health;
        set
        {
            health = value;
            if (health <= 0)
            {
                EmitSignal("NoHealth");
            }
        }
    }
}