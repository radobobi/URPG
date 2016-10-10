using UnityEngine;
using System.Collections;

public class CONSTANTS : MonoBehaviour {

	public static int InvCharRows = 5;
	public static int InvCharCols = 5;
	public static int InvCharSize = InvCharRows*InvCharCols;
	
	public static int LootRows = 2;
	public static int LootCols = 10;
	public static int LootSize = LootRows*LootCols;
	
	public static int ItemDisplayBoxWidth = 150;
	public static int ItemDisplayVerticalLinedrop = 20;

    public static int BaseUnitRadius = 32;

	public static Enemy EnemyGoblinoid = new Enemy("Goblinoid", new float[]{5, 5, 5, 5, 40, 50, 24}, 30);
	public static Enemy EnemyOrcoid = new Enemy("Orcoid", new float[]{5, 5, 5, 5, 50, 100, 32}, 50);
	
	public static Mob MobGoblinoids = new Mob(new Enemy[] {EnemyGoblinoid}, new int[]{5});
	public static Mob MobGoborcoids = new Mob(new Enemy[] {EnemyGoblinoid, EnemyOrcoid}, new int[]{2,2});
	public static Dungeon DungeonBase = new Dungeon("Base Dungeon", new Mob[]{MobGoblinoids, MobGoborcoids},
		new int[]{10, 5});
} 
