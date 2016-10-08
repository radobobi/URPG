using UnityEngine;
using System.Collections;

public class BattleManager : MonoBehaviour {
	
	//public GameObject TrashContainer;
	private GameObject _trashContainer;// = new GameObject();
	
	private Hero[] _heroes;
	private Goon[] _goons;
	
	private int _round = 0;
	
	private string _log = "";
	public string Log
	{
		get{return _log;}	
	}
	
	private Mob _currentMob;
	public Mob CurrentMob{get{return _currentMob;}}
	
	// Use this for initialization
	void Start () {
		//trashContainer = new GameObject("TrashContainer");
	}
	
	public void RegisterHeroes(Hero[] heroes)
	{
		_heroes = heroes;
	}
	
	public void InitializeBattle(Hero[] heroes)
	{
		_heroes = heroes;
		//SpawnGoons(Random.Range(5,10));
		SpawnGoons(CONSTANTS.MobGoborcoids);
	}
	
	public void ConductBattle()
	{
		while (!GoonsDead(_goons) && !GoonsDead(_heroes))
		{
			ConductRound();	
		}
	}
	
	// Returns TRUE if all goons are dead
	private bool GoonsDead(Goon[] pool)
	{
		foreach (Goon goon in pool)
		{
			if (! goon.Dead)
			{
				return false;	
			}
		}
		
		return true;
	}
	
	private void ConductRound()
	{
		_log += "\n" + "*** ROUND " + _round + " ***";
		
		foreach(Hero hero in _heroes)
		{
			ConductActions(hero);
			if(GoonsDead(_goons))
			{
				break;
			}
		}
		
		foreach(Goon goon in _goons)
		{
			ConductActions(goon);
			if(GoonsDead(_heroes))
			{
				// GAME OVER!
				break;
			}
		}
		
		++_round;
	}
	
	private void ConductActions(Goon goon)
	{
		// Reality check...
		if(goon.Dead)
		{
			return;	
		}
		
		// choose action
		Goon target;
		
		switch(goon.Skill_1)
		{
		case Skill.Heal:
			
			target = ChooseTargetToHeal(goon);
			
			int healStrength = goon.GetStatSecondary(SecondaryStatType.Compassion);
			target.Heal(healStrength);
			_log += "\n" + goon.MyName + " heals " + target.MyName + " for " 
				+ healStrength + " HP. " + target.CurrentHP + " HP left.";
			break;
			
			
		case Skill.Attack:
		
			// choose target...
			target = ChooseTargetToAttack(goon);
			
			//attack or whatever
			int hitStrength = goon.GetStatSecondary(SecondaryStatType.Damage);
			target.TakeDamage(hitStrength);
			_log += "\n" + goon.MyName + " hits " + target.MyName + " for " 
				+ hitStrength + " damage. " + target.CurrentHP + " HP left.";
			if(target.Dead)
			{
				_log+= "\n" + target.MyName + " has died.";
			}
			break;	
		}
	}
	
	private Goon ChooseTargetToHeal(Goon hero)
	{
		Goon target = null;
		Goon[] targetPool = hero is Hero ? _heroes : _goons;
		target = SelectRandomGoon(targetPool);
		return target;
	}
	
	private Goon ChooseTargetToAttack(Goon hero)
	{
		Goon target = null;
		Goon[] targetPool = hero is Hero ? _goons : _heroes;
		
		switch(hero.TargetSelection)
		{
		case TacticsTargetSelection.Random_Target:
			target = SelectRandomGoon(targetPool);
			break;
		case TacticsTargetSelection.Lowest_Current_HP:
			target = FindLowestCurrentHPGoon(targetPool);
			break;
			
		case TacticsTargetSelection.Lowest_Max_HP:
			target = FindLowestMaxHPGoon(targetPool);
			break;
		}
			
		return target;
	}
	
	private Goon SelectRandomGoon(Goon[] targetPool)
	{
		int targetindex = Random.Range(0, targetPool.Length);
		int i = 0;
		Goon target = null;
		while (targetindex > -1)
		{
			if(! targetPool[i].Dead)
			{
				target = targetPool[i];
				--targetindex;
			}
			i = (i+1) % (targetPool.Length);
		}
		
		return target;
	}
	
	private Goon FindLowestCurrentHPGoon(Goon[] targetPool)
	{
		Goon current = null;
		int currentLowestHP = -1;
		foreach (Goon goon in targetPool)
		{
			if(!goon.Dead)
			{
				if(currentLowestHP < 0 || goon.CurrentHP < currentLowestHP)
				{
					currentLowestHP = goon.CurrentHP;
					current = goon;
				}
			}
		}
		
		return current;
	}
	
	private Goon FindLowestMaxHPGoon(Goon[] targetPool)
	{
		Goon current = null;
		int currentLowestHP = -1;
		foreach (Goon goon in targetPool)
		{
			if(!goon.Dead)
			{
				if(currentLowestHP < 0 || goon.MaxHP < currentLowestHP)
				{
					currentLowestHP = goon.MaxHP;
					current = goon;
				}
			}
		}
		
		return current;
	}
	
	/*
	private void SpawnGoons(int number)
	{
		_trashContainer = GameObject.Find("TrashContainer");	
		_goons = new Goon[number];
		
		for (int i=0; i< number; ++i)
		{
			_goons[i] = _trashContainer.AddComponent<Goon>();	
			_goons[i].SetMainStats("Goblinoid "+i, 40+Random.Range(-10,10), new int[]{5, 0, 0});
		}
	}
	*/
	
	public void SpawnGoons(Mob goonMob)
	{
		_currentMob = goonMob;
		
		Enemy[] enemies = goonMob.MobTypes;
		int[] counts = goonMob.MobCounts;
		
		_trashContainer = GameObject.Find("TrashContainer");	
		_goons = new Goon[ArraySum(counts)];	
			
		int index = 0;
		for(int i=0; i<counts.Length; ++i)
		{
			Enemy current = enemies[i];
			for(int j=0; j<counts[i]; ++j)
			{
				_goons[index] = _trashContainer.AddComponent<Goon>();	
				int currentHP = current.HP;
				_goons[index].SetMainStats(current.MyName+" "+j, 
					currentHP+Random.Range(-currentHP/4,currentHP/4), current.Stats);
				
				++index;
			}
		}
	}
	
	private int ArraySum(int[] arr)
	{
		int sum = 0;
		
		foreach(int i in arr)
		{
			sum += i;	
		}
		
		return sum;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

public class DungeonManager 
{
	private string _myName;
	public string MyName{get{return _myName;}}
	
	private Mob[] _mobs;
	public Mob[] Mobs{get{return _mobs;}}
	private int[] _mobFrequencies;
	public int[] MobFrequencies{get{return _mobFrequencies;}}
	
	private int _frequenciesSum;
	
	public Mob GenerateMob()
	{
		int rng = Random.Range(0, _frequenciesSum);
		
		int index = 0;
		for(int i=0; i < _mobFrequencies.Length; ++i)
		{
			rng -= _mobFrequencies[i];
			if(rng<0)
			{
				index = i;
				break;
			}
		}
		
		return _mobs[index];
	}
	
	private void Initialize()
	{
		_frequenciesSum = ArraySum(_mobFrequencies);
	}
	
	public DungeonManager(string myName, Mob[] mobs, int[] mobFrequencies)
	{
		_myName = myName;
		_mobs = mobs;
		_mobFrequencies = mobFrequencies;
		Initialize();
	}
	
	public DungeonManager(Dungeon myDungeon)
	{
		_myName = myDungeon.MyName;
		_mobs = myDungeon.Mobs;
		_mobFrequencies = myDungeon.MobFrequencies;
		Initialize();
	}
	
	private int ArraySum(int[] arr)
	{
		int sum = 0;
		
		foreach(int i in arr)
		{
			sum += i;	
		}
		
		return sum;
	}
}