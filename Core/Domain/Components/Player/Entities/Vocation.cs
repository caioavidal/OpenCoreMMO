using System.Collections.Generic;

public class Formula
{
    public string MeleeDamage { get; set; }
    public string DistDamage { get; set; }
    public string Defense { get; set; }
    public string Armor { get; set; }
}

public class VocationSkill
{
    public string Id { get; set; }
    public string Multiplier { get; set; }
}

public class Vocation
{
    public string Id { get; set; }
    public string Clientid { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Gaincap { get; set; }
    public string Gainhp { get; set; }
    public string Gainmana { get; set; }
    public string Gainhpticks { get; set; }
    public string Gainhpamount { get; set; }
    public string Gainmanaticks { get; set; }
    public string Gainmanaamount { get; set; }
    public string Manamultiplier { get; set; }
    public string Attackspeed { get; set; }
    public string Basespeed { get; set; }
    public string Soulmax { get; set; }
    public string Gainsoulticks { get; set; }
    public string Fromvoc { get; set; }
    public Formula Formula { get; set; }
    public IList<VocationSkill> Skill { get; set; }
}

