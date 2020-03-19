using System.Collections.Generic;
namespace NeoServer.Server.Model.Players
{
    public class Player
    {
        public int Id { get; }
        public string CharacterName { get; set; }

        public Account Account { get; set; }
        public ChaseMode ChaseMode { get; set; }
        public int Capacity { get; }
        public uint Level { get; }
        public int HealthPoints { get; }
        public int MaxHealthPoints { get; }
        public VocationType Vocation { get; set; }
        public Gender Gender { get; set; }
        public bool Online { get; set; }
        public int Mana { get; set; }
        public int MaxMana { get; set; }
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
}