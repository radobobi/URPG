using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour {

    //public GameObject TrashContainer;
    private GameObject _trashContainer;// = new GameObject();

    private Hero[] _heroes;
    public Hero[] GetHeroesList()
    {
        return _heroes;
    }
    private Goon[] _goons;
    public Goon[] GetGoonsList()
    {
        return _goons;
    }
    private Action[] _heroActions;
    private Action[] _goonActions;
    private int[] _heroActionsEnd;
    private int[] _goonActionsEnd;

    private int _round = 1;
    private float _roundsPerSec = 100;
    private float _slowMo = 1;
    private float _roundInterval;

    private List<ActiveEffect> _effects;

    private bool _battleOver;

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
        _roundInterval = 1f / _roundsPerSec;
        _effects = new List<ActiveEffect>();
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
            aHero.MyPos = new Vector3(2000f + prevHeroSize + aHero.GetStatSecondary(SecondaryStatType.size) + heroSpacing, 2000f, 0f);
            prevHeroSize = prevHeroSize + heroSpacing + aHero.GetStatSecondary(SecondaryStatType.size);
        }
    }

    public void ConductBattle()
    {
        //print("Battle begins.");
        _battleOver = false;
        InvokeRepeating("invokeBattle", 0f, _roundInterval);
	}

    private void invokeBattle()
    {
        if (!_battleOver)
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

        if (!_battleOver)
        {
            ExecuteEffects();
        }

        if (!_battleOver)
        {
            ActHeroes();
        }

        if (!_battleOver)
        {
            ActEnemies();
        }
		
		++_round;
	}

    private void ActEnemies()
    {
        for (int i = 0; i < _goons.Length; i++)
        {
            //print("Goon " + _goons[i].MyName + " Position " + _goons[i].MyPos);
            if (_round == _goonActionsEnd[i])
            {
                executeAction(_goonActions[i]);
            }
            if (_round >= _goonActionsEnd[i])
            {
                if (GoonsDead(_heroes) || GoonsDead(_goons))
                {
                    _battleOver = true;
                    break;
                }
                ConductActionsGoon(i);
            }
        }
    }

    private void ActHeroes()
    {
        for (int i = 0; i < _heroes.Length; i++)
        {
            //print("Hero " + _heroes[i].MyName + " Position " + _heroes[i].MyPos);
            if (_round == _heroActionsEnd[i])
            {
                //print("hero counter: " + i);
                executeAction(_heroActions[i]);
            }
            if (_round >= _heroActionsEnd[i])
            {
                if (GoonsDead(_heroes) || GoonsDead(_goons))
                {
                    _battleOver = true;
                    break;
                }
                ConductActionsHero(i);
            }
        }
    }

    private void ExecuteEffects()
    {
        for (int i = 0; i < _effects.Count; i++)
        {
            ActiveEffect anEffect = _effects[i];
            switch (anEffect.SkillType)
            {
                case Skill.Fireball:
                    AdvanceFireball(anEffect);
                    break;
            }
        }


        if (GoonsDead(_heroes) || GoonsDead(_goons))
        {
            _battleOver = true;
        }
    }

    private void AdvanceFireball(ActiveEffect aFB)
    {
        if (aFB.Collides)
        {

        }
        else
        {
            float dist = Vector3.Distance(aFB.Pos, aFB.TargetUnit.MyPos) - aFB.Size - aFB.TargetUnit.GetStatSecondary(SecondaryStatType.size);
            if (dist <= aFB.MoveSpeed/_slowMo)
            {
                float ratio = dist / Vector3.Distance(aFB.Pos, aFB.TargetUnit.MyPos);
                aFB.Pos = new Vector3(aFB.Pos.x + (aFB.TargetUnit.MyPos.x - aFB.Pos.x) * ratio, aFB.Pos.y + (aFB.TargetUnit.MyPos.y - aFB.Pos.y) * ratio, 0f);
                Goon[] targetPool = aFB.Source is Hero ? _goons : _heroes;
                //("Blowing up the fireball at " + aFB.Pos + " on turn " + _round);
                ApplyAOEDamage(aFB.Pos, aFB.EndDamage, aFB.DamageAOE, aFB.HitUnits, targetPool, aFB.Source);
                _effects.Remove(aFB);
            }
            else
            {
                float ms = aFB.MoveSpeed / _slowMo;
                float ratio = ms / Vector3.Distance(aFB.Pos, aFB.TargetUnit.MyPos);
                //Vector3 fbPos1 = aFB.Pos;
                aFB.Pos = new Vector3(aFB.Pos.x + (aFB.TargetUnit.MyPos.x - aFB.Pos.x) * ratio, aFB.Pos.y + (aFB.TargetUnit.MyPos.y - aFB.Pos.y) * ratio, 0f);
                //print("Fireball moving from " + fbPos1 + " to " + aFB.Pos + " with a ratio of " + ratio);
            }
        }
    }

    private void ApplyAOEDamage(Vector3 blastPos, float[] damage, float[] blastRadius, List<Goon> hitUnits, Goon[] targetPool, Goon source)
    {
        for (int i=0; i<targetPool.Length; i++)
        {
            bool hitAlready = false;
            if (hitUnits != null)
            {
                if (hitUnits.Contains(targetPool[i]))
                {
                    print("Target has already been hit by this skill");
                    hitAlready = true;
                }
            }
            if (!hitAlready && !targetPool[i].Dead)
            {
                float dist = Vector3.Distance(blastPos, targetPool[i].MyPos) - targetPool[i].GetStatSecondary(SecondaryStatType.size);
                for (int j = 0; j < blastRadius.Length; j++)
                {
                    if (dist <= blastRadius[j])
                    {
                        targetPool[i].TakeDamage(damage[j]);
                        _log = "\n" + source.MyName + "'s AOE skill hits " + targetPool[i].MyName + " for "
                            + damage[j] + " damage. " + targetPool[i].CurrentHP + " HP left." + _log;
                        if (targetPool[i].Dead)
                        {
                            _log = "\n" + targetPool[i].MyName + " has died." + _log;
                        }
                        break;
                    }
                }
            }
        }
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
                case Skill.Fireball:
                    if (acting.Dead)
                    {

                    }
                    else
                    {
                        _log = "\n" + acting.MyName + " casts a fireball towards " + target.MyName + _log;
                        float hitStrength = acting.GetStatSecondary(SecondaryStatType.Damage)*2;
                        ActiveEffect aFireball = new ActiveEffect();
                        aFireball.Source = acting;
                        aFireball.TargetType = (int)TargetType.Unit;
                        aFireball.TargetUnit = target;
                        aFireball.EndDamage = new float[2] { hitStrength, hitStrength/2 };
                        aFireball.PierceCoeff = 0f;
                        aFireball.Collides = false;
                        aFireball.DamageAOE = new float[2] { 50f, 100f };
                        aFireball.SkillType = Skill.Fireball;
                        aFireball.Size = 2f;
                        aFireball.MoveSpeed = 100;
                        _effects.Add(aFireball);
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
                    float ms = hero.GetStatSecondary(SecondaryStatType.moveSpeed) / _slowMo;
                    float ratio = Mathf.Min(ms, dist - hero.GetStatSecondary(SecondaryStatType.size) - target.GetStatSecondary(SecondaryStatType.size)) / dist;
                    hero.MyPos = new Vector3(hero.MyPos.x + (target.MyPos.x - hero.MyPos.x) * ratio, hero.MyPos.y + (target.MyPos.y - hero.MyPos.y) * ratio, 0f);
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
                    float ms = hero.GetStatSecondary(SecondaryStatType.moveSpeed) / _slowMo;
                    float ratio = Mathf.Min(ms, dist - hero.GetStatSecondary(SecondaryStatType.size) - target.GetStatSecondary(SecondaryStatType.size)) / dist;
                    //Vector3 prevPos = hero.MyPos;
                    hero.MyPos = new Vector3(hero.MyPos.x + (target.MyPos.x - hero.MyPos.x) * ratio, hero.MyPos.y + (target.MyPos.y - hero.MyPos.y) * ratio, 0f);
                    //print("Hero " + hero.MyName + " moves from " + prevPos + " to " + hero.MyPos + " with ratio " + ratio + " with target " + target.MyPos);
                }

                
			    break;


            case Skill.Fireball:

                // choose target...
                target = ChooseTargetToAttack(hero);
                dist = Vector3.Distance(hero.MyPos, target.MyPos);
                if (dist <= hero.GetStatSecondary(SecondaryStatType.range)*1.5)
                {
                    //attack or whatever
                    turnsToAct = 40 - hero.GetStatBase(MainStatType.Agility);
                    //print("initiating attack action " + hero.MyName + " and his stat is " + hero.GetStatBase(MainStatType.Agility));
                    anAction = new Action(hero, target, Skill.Fireball, _round, _round + turnsToAct);
                    //print("acting unit " + anAction.getActing().MyName + " and target unit " + anAction.getTarget().MyName);
                    _heroActions[whichHero] = anAction;
                    _heroActionsEnd[whichHero] = _round + turnsToAct;
                }
                else
                {
                    float ms = hero.GetStatSecondary(SecondaryStatType.moveSpeed) / _slowMo;
                    float ratio = Mathf.Min(ms, dist - hero.GetStatSecondary(SecondaryStatType.size) - target.GetStatSecondary(SecondaryStatType.size)) / dist;
                    //Vector3 prevPos = hero.MyPos;
                    hero.MyPos = new Vector3(hero.MyPos.x + (target.MyPos.x - hero.MyPos.x) * ratio, hero.MyPos.y + (target.MyPos.y - hero.MyPos.y) * ratio, 0f);
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
                    float ms = goon.GetStatSecondary(SecondaryStatType.moveSpeed) / _slowMo;
                    float ratio = Mathf.Min(ms, dist - goon.GetStatSecondary(SecondaryStatType.size) - target.GetStatSecondary(SecondaryStatType.size)) / dist;
                    goon.MyPos = new Vector3(goon.MyPos.x + (target.MyPos.x - goon.MyPos.x) * ratio, goon.MyPos.y + (target.MyPos.y - goon.MyPos.y) * ratio, 0f);
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
                    float ms = goon.GetStatSecondary(SecondaryStatType.moveSpeed) / _slowMo;
                    float ratio = Mathf.Min(ms, dist - goon.GetStatSecondary(SecondaryStatType.size) - target.GetStatSecondary(SecondaryStatType.size)) / dist;
                    //Vector3 prevPos = goon.MyPos;
                    goon.MyPos = new Vector3(goon.MyPos.x + (target.MyPos.x - goon.MyPos.x) * ratio, goon.MyPos.y + (target.MyPos.y - goon.MyPos.y) * ratio, 0f);
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
            aGoon.MyPos = new Vector3(2054f + Mathf.Cos(randAngle) * 1000, 2000f + Mathf.Sin(randAngle) * 1000, 0f);
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