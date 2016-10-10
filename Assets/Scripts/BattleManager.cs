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
    private int _roundsPerMin = 600;
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
        float prevHeroSize = 0f;
        float heroSpacing = 10f;
        for (int i = 0; i < heroes.Length; i++)
        {
            _heroActionsEnd[i] = 0;
            Hero aHero = heroes[i];
            aHero.setMyPos(2000f + prevHeroSize + aHero.GetStatSecondary(SecondaryStatType.size) + heroSpacing, 2000f, 0f);
            prevHeroSize = prevHeroSize + heroSpacing + aHero.GetStatSecondary(SecondaryStatType.size);
        }
    }

    public void ConductBattle()
    {
        //print("Battle begins.");
        InvokeRepeating("invokeBattle", 0f, _roundInterval);
	}

    private void invokeBattle()
    {
        if (!GoonsDead(_goons) && !GoonsDead(_heroes))
        {
            //print("Starting round: " + _round);
            ConductRound();
            _logCallback(_log);
        }
        else
        {
            //print("Stopping At round: " + _round);
            _battleCleanupCallback();
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
		_log = "\n" + "*** ROUND " + _round + " ***" + _log;
		
        for (int i=0; i<_heroes.Length; i++)
        {
            //print("Hero " + _heroes[i].MyName + " Position " + _heroes[i].MyPos);
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
            //print("Goon " + _goons[i].MyName + " Position " + _goons[i].MyPos);
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
                        _log = "\n" + acting.MyName + " tried attacking " + target.MyName + " but it was too late, " 
                            + target.MyName + " has already died." + _log;
                    }
                    else if (acting.Dead)
                    {

                    }
                    else
                    {
                        //print("executing attack action of " + acting.MyName);
                        float hitStrength = acting.GetStatSecondary(SecondaryStatType.Damage);
                        target.TakeDamage(hitStrength);
                        _log = "\n" + acting.MyName + " hits " + target.MyName + " for "
                            + hitStrength + " damage. " + target.CurrentHP + " HP left." + _log;
                        if (target.Dead)
                        {
                            _log = "\n" + target.MyName + " has died." + _log;
                        }
                    }
                    break;
                case Skill.Heal:
                    if (target.Dead)
                    {
                        _log = "\n" + acting.MyName + " tried healing " + target.MyName 
                            + " but it was too late, " + target.MyName + " has already died." + _log;
                    }
                    else if (acting.Dead)
                    {

                    }
                    else
                    {
                        //print("executing heal action of " + acting.MyName);
                        float healStrength = acting.GetStatSecondary(SecondaryStatType.Compassion);
                        target.Heal(healStrength);
                        _log = "\n" + acting.MyName + " heals " + target.MyName + " for "
                            + healStrength + " HP. " + target.CurrentHP + " HP left." + _log;
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
        float dist;

        int turnsToAct;
        switch (hero.Skill_1)
		{
		    case Skill.Heal:
			
			    target = ChooseTargetToHeal(hero);
                dist = Vector3.Distance(hero.MyPos, target.MyPos);
                if (dist <= hero.GetStatSecondary(SecondaryStatType.range))
                {
                    turnsToAct = 60 - hero.GetStatBase(MainStatType.Wisdom) - hero.GetStatBase(MainStatType.Willpower);
                    //print("initiating heal action " + hero.MyName + " and his stat is " + hero.GetStatBase(MainStatType.Wisdom));
                    anAction = new Action(hero, target, Skill.Heal, _round, _round + turnsToAct);
                    _heroActions[whichHero] = anAction;
                    _heroActionsEnd[whichHero] = _round + turnsToAct;
                }
                else
                {
                    float ms = hero.GetStatSecondary(SecondaryStatType.moveSpeed);
                    float ratio = Mathf.Min(ms, dist - hero.GetStatSecondary(SecondaryStatType.size) - target.GetStatSecondary(SecondaryStatType.size)) / dist;
                    hero.setMyPos(hero.MyPos.x + (target.MyPos.x - hero.MyPos.x) * ratio, hero.MyPos.y + (target.MyPos.y - hero.MyPos.y) * ratio, 0f);
                }
            
			    break;
			
			
		    case Skill.Attack:
		
			    // choose target...
			    target = ChooseTargetToAttack(hero);
                dist = Vector3.Distance(hero.MyPos, target.MyPos);
                if (dist <= hero.GetStatSecondary(SecondaryStatType.range))
                {
                    //attack or whatever
                    turnsToAct = 40 - hero.GetStatBase(MainStatType.Agility);
                    //print("initiating attack action " + hero.MyName + " and his stat is " + hero.GetStatBase(MainStatType.Agility));
                    anAction = new Action(hero, target, Skill.Attack, _round, _round + turnsToAct);
                    //print("acting unit " + anAction.getActing().MyName + " and target unit " + anAction.getTarget().MyName);
                    _heroActions[whichHero] = anAction;
                    _heroActionsEnd[whichHero] = _round + turnsToAct;
                }
                else
                {
                    float ms = hero.GetStatSecondary(SecondaryStatType.moveSpeed);
                    float ratio = Mathf.Min(ms, dist - hero.GetStatSecondary(SecondaryStatType.size) - target.GetStatSecondary(SecondaryStatType.size)) / dist;
                    //Vector3 prevPos = hero.MyPos;
                    hero.setMyPos(hero.MyPos.x + (target.MyPos.x - hero.MyPos.x) * ratio, hero.MyPos.y + (target.MyPos.y - hero.MyPos.y) * ratio, 0f);
                    //print("Hero " + hero.MyName + " moves from " + prevPos + " to " + hero.MyPos + " with ratio " + ratio + " with target " + target.MyPos);
                }

                
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
        float dist;

        int turnsToAct;
        switch (goon.Skill_1)
        {
            case Skill.Heal:

                target = ChooseTargetToHeal(goon);
                dist = Vector3.Distance(goon.MyPos, target.MyPos);
                if (dist <= goon.GetStatSecondary(SecondaryStatType.range))
                {
                    turnsToAct = 60;
                    anAction = new Action(goon, target, Skill.Heal, _round, _round + turnsToAct);
                    //print("initiating heal action " + goon.MyName);
                    _goonActions[whichGoon] = anAction;
                    _goonActionsEnd[whichGoon] = _round + turnsToAct;
                }
                else
                {
                    float ms = goon.GetStatSecondary(SecondaryStatType.moveSpeed);
                    float ratio = Mathf.Min(ms, dist - goon.GetStatSecondary(SecondaryStatType.size) - target.GetStatSecondary(SecondaryStatType.size)) / dist;
                    goon.setMyPos(goon.MyPos.x + (target.MyPos.x - goon.MyPos.x)*ratio, goon.MyPos.y + (target.MyPos.y - goon.MyPos.y) * ratio, 0f);
                }
                break;




            case Skill.Attack:

                // choose target...
                target = ChooseTargetToAttack(goon);
                dist = Vector3.Distance(goon.MyPos, target.MyPos);
                if (dist <= goon.GetStatSecondary(SecondaryStatType.range))
                {
                    //attack or whatever
                    turnsToAct = 40;
                    //print("initiating attack action " + goon.MyName);
                    anAction = new Action(goon, target, Skill.Attack, _round, _round + turnsToAct);
                    //print("acting unit " + anAction.getActing().MyName + " and target unit " + anAction.getTarget().MyName);
                    _goonActions[whichGoon] = anAction;
                    _goonActionsEnd[whichGoon] = _round + turnsToAct;
                }
                else
                {
                    float ms = goon.GetStatSecondary(SecondaryStatType.moveSpeed);
                    float ratio = Mathf.Min(ms, dist - goon.GetStatSecondary(SecondaryStatType.size) - target.GetStatSecondary(SecondaryStatType.size)) / dist;
                    //Vector3 prevPos = goon.MyPos;
                    goon.setMyPos(goon.MyPos.x + (target.MyPos.x - goon.MyPos.x) * ratio, goon.MyPos.y + (target.MyPos.y - goon.MyPos.y) * ratio, 0f);
                    //print("goon " + goon.MyName + " moves from " + prevPos + " to " + goon.MyPos + " with ratio " + ratio + " with target " + target.MyPos);
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
        
        target = SelectClosestEnemy(hero, targetPool);

        /*switch(hero.TargetSelection)
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
		}*/
        return target;
	}

    private Goon SelectClosestEnemy(Goon unit, Goon[] enemies)
    {
        int target = 0;
        float closestDist = 5000f;

        for (int i=0; i<enemies.Length; i++)
        {
            float dist = Vector3.Distance(unit.MyPos, enemies[i].MyPos) - enemies[i].GetStatSecondary(SecondaryStatType.size);
            if (dist < closestDist && !enemies[i].Dead)
            {
                target = i;
                closestDist = dist;
            }
            //print("Distance between " + unit.MyPos + " and " + enemies[i].MyPos + " is: " + dist);
        }

        return enemies[target];
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
        float currentLowestHP = -1;
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
        float currentLowestHP = -1;
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
				float currentHP = current.HP;
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
            Goon aGoon = _goons[i];
            float randAngle = Random.Range(0f, Mathf.PI);
            aGoon.setMyPos(2054f + Mathf.Cos(randAngle)*1000, 2000f + Mathf.Sin(randAngle) * 1000, 0f);
        }
    }

    private ControlsManager.WriteToLogDelegate _logCallback;
    private ControlsManager.BattleCleanupDelegate _battleCleanupCallback;

    public void RegisterWriteToLogDelegate(ControlsManager.WriteToLogDelegate logCallback)
    {
        _logCallback = logCallback;
    }

    public void RegisterBattleCleanupCallback(ControlsManager.BattleCleanupDelegate battleCleanupCallback)
    {
        _battleCleanupCallback = battleCleanupCallback;
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