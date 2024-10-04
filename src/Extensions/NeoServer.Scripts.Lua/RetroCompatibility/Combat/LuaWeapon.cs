using NeoServer.Game.Common.Item;
using NeoServer.Scripts.Lua.EventRegister;
using NLua;

namespace NeoServer.Scripts.Lua.RetroCompatibility.Combat;

public class LuaWeapon
{
    public ushort Id { get; set; }
    public int Attack { get; set; }
    public string Action { get; set; }
    public ShootType ShootType { get; set; }
    public int MaxHitChance { get; set; }
    public string AmmoTypeName { get; set; }
    public WeaponType WeaponType { get; set; }
    public LuaWeapon(WeaponType weaponType)
    {
        WeaponType = weaponType;
    }

    public void id(ushort id) => Id = id;
    public void attack(int attack) => Attack = attack;
    public void action(string value) => Action = value;
    public void ammoType(string ammoType) => AmmoTypeName = ammoType;

    public void shootType(ShootType shootType) => ShootType = shootType;
    public void maxHitChance(int value) => MaxHitChance = value;
    public LuaFunction onUseWeapon { get; set; }
    public void register()
    {
        LuaEventManager.Register(new ItemKey
        {
            ServerId = Id
        }, "attack", onUseWeapon);
    }
}