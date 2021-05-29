using Godot;

public class HealthUI : Control
{
    //Variables
    private PlayerStats PlayerHearts = new PlayerStats();

    private HBoxContainer heartsBar;
    private Texture heart_full;
    private Texture heart_empty;

    //Fonctions
    public override void _Ready()
    {
        PlayerHearts = GetNode("/root/PlayerStats") as PlayerStats; // Chargement singleton PlayerStats
        PlayerHearts.Connect("PlayerHealthChangedUI", this, "PlayerHealth");
        heartsBar = GetNode("HeartsBar") as HBoxContainer;
        heart_full = GD.Load("UI/HeartUIEmpty.png") as Texture;
        heart_empty = GD.Load("UI/HeartUIFull.png") as Texture;
    }

    public void HeartsBarUIUpdate()
    {
        for (int i = 0; i < heartsBar.GetChildCount(); i++)
            if (PlayerHearts.PlayerHealth <= i)
            {
                heartsBar.GetChild(i).Set("texture", heart_full);
            }
            else
            {
                heartsBar.GetChild(i).Set("texture", heart_empty);
            }
    }

    public override void _Process(float delta)
    {
        HeartsBarUIUpdate();
    }
}