using UnityEngine;
using System.Collections;

public class BattleManager : MonoBehaviour {

    //public GameObject TrashContainer;
    private GameObject _trashContainer;// = new GameObject();

    private Hero[] _heroes;
    private Goon[] _goons;
    private Action[] _heroActions;
    private Action[] _goonActions;
    private int[] _heroActionsEnd;
    private int[] _goonActionsEnd;

    private int _round = 1;
    private double _gameTimer = 0;
    private int _roundsPerMin = 6000;
    private float _roundInterval;

    private string _log = "";
    public string Log
    {
        get { return _log; }
    }

    private Mob _currentMob;
    public Mob CurrentMob { get { return _currentMob; } }

    // Use this for initialization
    void Start() {
        //trashContainer = new GameObject("TrashContainer");
    }

    public void RegisterHeroes(Hero[] heroes)
    {
        _roundInterval = (float) 60 / (float) _roundsPerMin;
        //print("interval: " + _roundInterval);
        _heroes = heroes;
        _heroActions = new Action[heroes.Length];
        _heroActionsEnd = new int[heroes.Length];
        for (int i = 0; i < heroes.Length; i++)
        {
            _heroActionsEnd[i] = 0;
        }
    }

    public void InitializeBattle(Hero[] heroes)
    {
        _heroes = heroes;
        //SpawnGoons(Random.Range(5,10));
        SpawnGoons(CONSTANTS.MobGoborcoids);
    }

    public bool ConductBattle()
    {
        print("conducting battle begins ");
        InvokeRepeating("invokeBattle", 0f, _roundInterval);
        //while (!GoonsDead(_goons) && !GoonsDead(_heroes))
        //{
         //   ConductRound();
        //}
        if (GoonsDead(_goons) || GoonsDead(_heroes))
        {
            return true;
        }
        return false;
	}

    private void invokeBattle()
    {
        if (!GoonsDead(_goons) && !GoonsDead(_heroes))
        {
            print("Starting round: " + _round);
            ConductRound();
        }
        else
        {
            print("Stopping At round: " + _round);
            CancelInvoke("invokeBattle");
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
		
        for (int i=0; i<_heroes.Length; i++)
        {
            if (_round == _heroActionsEnd[i])
            {
                //print("hero counter: " + i);
                executeAction(_heroActions[i]);
            }
            if (_round >= _heroActionsEnd[i])
            {
                if (GoonsDead(_goons))
                {
                    break;
                }
                ConductActionsHero(i);
            }
        }

        for (int i=0; i<_goons.Length; i++)
        {
            if (_round == _goonActionsEnd[i])
            {
                executeAction(_goonActions[i]);
            }
            if (_round >= _goonActionsEnd[i])
            {
                if (GoonsDead(_heroes))
                {
                    // GAME OVER!
                    break;
                }
                ConductActionsGoon(i);
            }
        }
		
		++_round;
	}

    private void executeAction(Action act)
    {
        if (act.getEndTurn() == _round)
        {
            Goon acting = act.getActing();
            Goon target = act.getTarget();
            switch (act.getSkill())
            {
                case Skill.Attack:
                    if (target.Dead)
                    {
                        _log += "\n" + acting.MyName + " tried attacking " + target.MyName + " but it was too late, " + target.MyName + " has already died.";
                    }
                    else if (acting.Dead)
                    {

                    }
                    else
                    {
                        //print("executing attack action of " + acting.MyName);
                        int hitStrength = acting.GetStatSecondary(SecondaryStatType.Damage);
                        target.TakeDamage(hitStrength);
                        _log += "\n" + acting.MyName + " hits " + target.MyName + " for "
                            + hitStrength + " damage. " + target.CurrentHP + " HP left.";
                        if (target.Dead)
                        {
                            _log += "\n" + target.MyName + " has died.";
                        }
                    }
                    break;
                case Skill.Heal:
                    if (target.Dead)
                    {
                        _log += "\n" + acting.MyName + " tried healing " + target.MyName + " but it was too late, " + target.MyName + " has already died.";
                    }
                    else if (acting.Dead)
                    {

                    }
                    else
                    {
                        //print("executing heal action of " + acting.MyName);
                        int healStrength = acting.GetStatSecondary(SecondaryStatType.Compassion);
                        target.Heal(healStrength);
                        _log += "\n" + acting.MyName + " heals " + target.MyName + " for "
                            + healStrength + " HP. " + target.CurrentHP + " HP left.";
                    }
                    break;
            }
        }
        else
        {
            // Create an Error case
        }
    }
	
	private void ConductActionsHero(int whichHero)
    {
        Hero hero = _heroes[whichHero];
        // Reality check...
        if (hero.Dead)
		{
			return;	
		}
		
		// choose action
		Goon target;
        Action anAction;

        int turnsToAct;
        switch (hero.Skill_1)
		{
		case Skill.Heal:
			
			target = ChooseTargetToHeal(hero);
			
            turnsToAct = 60 - hero.GetStatBase(MainStatType.Wisdom) - hero.GetStatBase(MainStatType.Willpower);
            //print("initiating heal action " + hero.MyName + " and his stat is " + hero.GetStatBase(MainStatType.Wisdom));
            anAction = new Action(hero, target, Skill.Heal, _round, _round+turnsToAct);
            _heroActions[whichHero] = anAction;
            _heroActionsEnd[whichHero] = _round + turnsToAct;
			break;
			
			
		case Skill.Attack:
		
			// choose target...
			target = ChooseTargetToAttack(hero);
			
			//attack or whatever
            turnsToAct = 40 - hero.GetStatBase(MainStatType.Agility);
            //print("initiating attack action " + hero.MyName + " and his stat is " + hero.GetStatBase(MainStatType.Agility));
            anAction = new Action(hero, target, Skill.Attack, _round, _round + turnsToAct);
            //print("acting unit " + anAction.getActing().MyName + " and target unit " + anAction.getTarget().MyName);
            _heroActions[whichHero] = anAction;
            _heroActionsEnd[whichHero] = _round + turnsToAct;
			break;	
		}
    }

    private void ConductActionsGoon(int whichGoon)
    {
        Goon goon = _goons[whichGoon];
        // Reality check...
        if (goon.Dead)
        {
            return;
        }

        // choose action
        Goon target;
        Action anAction;

        int turnsToAct;
        switch (goon.Skill_1)
        {
            case Skill.Heal:

                target = ChooseTargetToHeal(goon);
                
                turnsToAct = 60;
                anAction = new Action(goon, target, Skill.Heal, _round, _round + turnsToAct);
                //print("initiating heal action " + goon.MyName);
                _goonActions[whichGoon] = anAction;
                _goonActionsEnd[whichGoon] = _round + turnsToAct;
                break;


            case Skill.Attack:

                // choose target...
                target = ChooseTargetToAttack(goon);

                //attack or whatever
                turnsToAct = 40;
                //print("initiating attack action " + goon.MyName);
                anAction = new Action(goon, target, Skill.Attack, _round, _round + turnsToAct);
                //print("acting unit " + anAction.getActing().MyName + " and target unit " + anAction.getTarget().MyName);
                _goonActions[whichGoon] = anAction;
                _goonActionsEnd[whichGoon] = _round + turnsToAct;
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
        _goonActions = new Action[_goons.Length];
        _goonActionsEnd = new int[_goons.Length];
        for (int i = 0; i < _goons.Length; i++)
        {
            _goonActionsEnd[i] = 0;
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