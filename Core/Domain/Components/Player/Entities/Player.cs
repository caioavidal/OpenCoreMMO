using System.Collections.Generic;

public class Player
{
    public int Id { get; }
    public int Name { get; set; }
    public ChaseMode ChaseMode { get; }
    public int Capacity { get; }
    public int Level { get; }
    public int HealthPoints { get; }
    public int MaxHealthPoints { get; }
    public Vocation Vocation { get; set; }
    public int Mana { get; }
    public int MaxMana { get; }
    public int FightMode { get; }
    public int SoulPoints { get; set; }
    public int MaxSoulPoints { get; set; }
    public IReadOnlyCollection<Skill> Skills { get; set; }

    public int StaminaMinutes { get; set; }

    public bool IsMounted()
    {
        return false;
    }

    // public string GetDescription(bool isYourself)
    // {
    //     if(isYourself){
    //         return $"You are {Vocation}"
    //     }
    // }

}