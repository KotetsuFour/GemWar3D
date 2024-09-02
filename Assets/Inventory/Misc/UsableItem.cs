using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsableItem : Item
{
	public string desc;
	private StatToEdit stat;
	private int amount;
	public UsableItem(string name, string desc, StatToEdit stat, int amount, int uses, int id) : base(name, uses, id)
    {
		this.desc = desc;
		this.stat = stat;
		this.amount = amount;
    }

	public override Item clone()
	{
		return new UsableItem(itemName, desc, stat, amount, uses, id);
	}
    public override string description()
    {
		return desc + (uses == -1 ? " (--/--)" : " (" + usesLeft + "/" + uses + ")");
    }
	public enum StatToEdit
    {
		CURRENTHP, MAXHP, STRENGTH, MAGIC, SKILL, SPEED, LUCK, DEFENSE, RESISTANCE, MOVEMENT
    }
	public void use(Unit holder)
    {
		if (stat == StatToEdit.CURRENTHP)
        {
			holder.heal(amount);
        } else if (stat == StatToEdit.MAXHP)
        {
			holder.maxHP += amount;
		}
		else if (stat == StatToEdit.STRENGTH)
		{
			holder.strength += amount;
		}
		else if (stat == StatToEdit.MAGIC)
		{
			holder.magic += amount;
		}
		else if (stat == StatToEdit.SKILL)
		{
			holder.skill += amount;
		}
		else if (stat == StatToEdit.SPEED)
		{
			holder.speed += amount;
		}
		else if (stat == StatToEdit.LUCK)
		{
			holder.luck += amount;
		}
		else if (stat == StatToEdit.DEFENSE)
		{
			holder.defense += amount;
		}
		else if (stat == StatToEdit.RESISTANCE)
		{
			holder.resistance += amount;
		}
		else if (stat == StatToEdit.MOVEMENT)
        {
			holder.movement += amount;
        }

		usesLeft--;
	}
}
