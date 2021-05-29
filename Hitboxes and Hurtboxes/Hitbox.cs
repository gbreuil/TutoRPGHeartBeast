using Godot;

public class Hitbox : Area2D
{
    //Constructeur
    public Hitbox()
    {
        Damage = 1;
    }

    //Fonctions
    public int Damage { get; set; }
}