public abstract class Item
{
    public string itemName;
    public int uses;
    public int usesLeft;
    public int id;

    public static Weapon rose_shield = new Armor("Rose Quartz Shield", -1, 1, 80, 5, 2, 1, 2, -1, 5, false, null, 0);
    public static Weapon pearl_spear = new Lance("Pearl Spear", -1, 7, 70, 0, 9, 1, 2, -1, false, null, 1);
    public static Weapon biggs_whip = new Whip("Jasper Whip", -1, 7, 70, 0, 9, 1, 2, -1, false, null, 2);
    public static Weapon ocean_club = new Club("Jasper Mace", -1, 7, 70, 0, 9, 1, 1, -1, false, null, 3);
    public static Weapon bismuth_hammer = new Axe("Bismuth Hammer", -1, 7, 70, 0, 9, 1, 1, -1, false, null, 4);
    public static Weapon iron_sword = new Sword("Iron Sword", 0, 6, 70, 0, 6, 1, 1, 40, false, null, 5);
    public static Weapon quartz_axe = new Axe("Quartz Axe", -1, 9, 60, 0, 12, 1, 1, -1, false, null, 6);
    public static Weapon palm_laser = new SpecialWeapon("Palm Laser", -1, 5, 70, 0, 4, 1, 2, -1, true, null, 7);
    public static Weapon ruby_pike = new Lance("Ruby Pike", -1, 6, 100, 5, 6, 1, 1, -1, false, null, 8);
    public static Weapon moon_bow = new Bow("Moonstone Bow", -1, 10, 65, 0, 11, 2, 3, -1, false, null, 9);
    public static Weapon priest_bow = new Bow("Priestly Bow", -1, 10, 65, 0, 14, 2, 2, -1, false, null, 10);
    public static Weapon iron_lance = new Lance("Iron Lance", 0, 7, 70, 0, 9, 1, 1, 40, false, null, 11);
    public static Weapon iron_axe = new Axe("Iron Axe", 0, 9, 65, 0, 10, 1, 1, 30, false, null, 12);
    public static Weapon iron_gauntlet = new Fist("Iron Gauntlet", 0, 3, 85, 5, 3, 1, 1, 50, false, null, 13);
    public static Weapon iron_shield = new Armor("Iron Shield", 0, 1, 80, 0, 3, 1, 2, 40, 3, false, null, 14);
    public static Weapon iron_whip = new Whip("Iron Whip", 0, 6, 70, 0, 7, 1, 2, 40, false, null, 15);
    public static Weapon iron_bow = new Bow("Iron Bow", 0, 7, 65, 0, 6, 2, 2, 40, false, null, 16);
    public static Weapon iron_club = new Club("Iron Club", 0, 6, 75, 5, 9, 1, 1, 30, false, null, 17);
    public static UsableItem currentHP = new UsableItem("Rose's Tear", "Heals 10 HP", UsableItem.StatToEdit.CURRENTHP, 10, 3, 18);
    public static UsableItem maxHP = new UsableItem("Snerson Robe", "Increases Max HP by 5", UsableItem.StatToEdit.MAXHP, 5, 1, 19);
    public static UsableItem str = new UsableItem("Strength Ring", "Increases Strength by 2", UsableItem.StatToEdit.STRENGTH, 2, 1, 20);
    public static UsableItem mag = new UsableItem("", "Incrases Magic by 2", UsableItem.StatToEdit.MAGIC, 2, 1, 21);
    public static UsableItem skl = new UsableItem("", "Increases Skill by 2", UsableItem.StatToEdit.SKILL, 2, 1, 22);
    public static UsableItem spd = new UsableItem("", "Increases Speed by 2", UsableItem.StatToEdit.SPEED, 2, 1, 23);
    public static UsableItem lck = new UsableItem("Moon Goddess Icon", "Increases Luck by 2", UsableItem.StatToEdit.LUCK, 2, 1, 24);
    public static UsableItem def = new UsableItem("", "Increases Defense by 2", UsableItem.StatToEdit.DEFENSE, 2, 1, 25);
    public static UsableItem res = new UsableItem("", "Increases Resistance by 2", UsableItem.StatToEdit.RESISTANCE, 2, 1, 26);
    public static UsableItem mov = new UsableItem("", "Increases Movement by 2", UsableItem.StatToEdit.MOVEMENT, 2, 1, 27);
    public static Weapon ship_laser = new SpecialWeapon("Ship Laser", -1, 14, 70, 10, 12, 1, 2, -1, true, null, 28);
    public static Weapon elite_sword = new Sword("Elite Quartz Sword", -1, 15, 55, 0, 16, 1, 1, -1, false, null, 29);
    public static Weapon citrine_sword = new Sword("Citrine Sword", -1, 14, 60, 10, 15, 1, 1, -1, false, null, 30);
    public static Weapon aventurine_axe = new Sword("Aventurine Axe", -1, 13, 60, 10, 14, 1, 1, -1, false, null, 31);
    public static Weapon pacifist_gauntlet = new Fist("Pacifist Gauntlet", -1, 0, 100, 0, 0, 1, 1, -1, false, null, 32);
    public static Weapon topaz_lance = new Lance("Topaz Fusion Lance", -1, 18, 60, 0, 18, 1, 1, -1, false, null, 33);
    public static Weapon guard_shield = new Armor("Guard Shield", -1, 7, 80, 0, 7, 1, 1, -1, 3, false, null, 34);
    public static Weapon iron_blade = new Sword("Iron Blade", 10, 12, 55, 0, 15, 1, 1, 30, false, new UnitClass.UnitType[] { UnitClass.UnitType.QUARTZ }, 35);

    public static Item[] itemIndex = new Item[] { rose_shield, pearl_spear, biggs_whip, ocean_club, bismuth_hammer,
                iron_sword, quartz_axe, palm_laser, ruby_pike, moon_bow, priest_bow, iron_lance,
                iron_axe, iron_gauntlet, iron_shield, iron_whip, iron_bow, iron_club, currentHP, maxHP,
                str, mag, skl, spd, lck, def, res, mov, ship_laser, elite_sword, citrine_sword,
                aventurine_axe, pacifist_gauntlet, topaz_lance, guard_shield, iron_blade };
    public Item(string itemName, int uses, int id)
    {
        this.itemName = itemName;
        this.uses = uses;
        this.usesLeft = uses;
        this.id = id;
    }
    public abstract Item clone();
    public abstract string description();
}
