using Petal.Framework.EntityComponent;

namespace Hgm.Components;

/// <summary>
/// dummy class
/// </summary>
public sealed class CharacterSheet : Component
{
    public static readonly ComponentInfo ComponentInfo = new()
    {
        ContentIdentifier = "hedgemen:character_sheet",
        ComponentType = typeof(CharacterSheet)
    };

    public int Strength
    {
        get;
        set;
    } = 10;
    
    public int Dexterity
    {
        get;
        set;
    } = 10;
    
    public int Constitution
    {
        get;
        set;
    } = 10;
    
    public int Intelligence
    {
        get;
        set;
    } = 10;
    
    public int Wisdom
    {
        get;
        set;
    } = 10;
    
    public int Charisma
    {
        get;
        set;
    } = 10;

    public CharacterSheet()
    {
        
    }

    public override void PropagateEvent(EntityEvent e)
    {
        switch (e)
        {
            case StatChangeEvent args:
                ChangeStat(args.StatName, args.Amount);
                break;
        }
    }

    public void ChangeStat(string statName, int amount)
    {
        switch (statName)
        {
            case "strength":
                Strength += amount;
                break;
            case "dexterity":
                Dexterity += amount;
                break;
            case "constitution":
                Constitution += amount;
                break;
            case "intelligence":
                Intelligence += amount;
                break;
            case "wisdom":
                Wisdom += amount;
                break;
            case "charisma":
                Charisma += amount;
                break;
            default:
                return;
        }
    }

    public override ComponentInfo GetComponentInfo()
        => ComponentInfo;
}