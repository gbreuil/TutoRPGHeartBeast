using Godot;
using static Godot.Mathf;

public class PlayerStats : Stats
{
    //Variables
    [Signal] public delegate void PlayerHealthChangedUI();

    //Constructeur
    public PlayerStats()
    {
        MaxHealth = 4;
        Health = MaxHealth;
    }

    //Fonctions

    public int PlayerMaxHealth
    {
        get => MaxHealth;
        set
        {
            MaxHealth = Max(value, 1);
            Health = MaxHealth;
        }
    }

    public int PlayerHealth
    {
        get => Health;
        set
        {
            Health = Clamp(value, 0, MaxHealth);
            EmitSignal("PlayerHealthChangedUI", Health);
            if (Health <= 0)
            {
                EmitSignal("NoHealth");
            }
        }
    }
}