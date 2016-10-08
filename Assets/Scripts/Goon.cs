using UnityEngine;
using System.Collections;
	
public enum TacticsTargetSelection
{
	Random_Target = 0,
	Lowest_Current_HP,
	Lowest_Max_HP,
	LENGTH,
}

public enum TacticsBasePosition
{
	Front = 0,
	Back,
	LENGTH
}

public enum Skill
{
	Attack = 0,
	Heal,
	LENGTH
}

public class Goon : MonoBehaviour
{
	protected string _myName;
	public string MyName
    {
        get
        {
            return _myName;
        }
        set
        {
            _myName = value;
        }
    }	
	
	protected int[] _secondaryStats = new int[(int) SecondaryStatType.LENGTH];
	public int GetStatSecondary(SecondaryStatType a)
	{
		return _secondaryStats[(int) a];	
	}
	
	protected int _maxHP;
	public int MaxHP
	{
		get{return _maxHP;}	
	}
	
	protected int _currentHP = 0;
	public int CurrentHP
	{
		get{return _currentHP;}	
	}
	
	protected bool _dead = false;
	public bool Dead
	{
		get{return _dead;}	
	}
	
	// Returns actual damage taken
	public int TakeDamage(int dmg)
	{
		int hitStrength = Mathf.Max(dmg - _secondaryStats[(int) SecondaryStatType.Block],1);
		_currentHP -= hitStrength;	
		if(_currentHP <= 0)
		{
			_dead = true;	
		}
		
		return hitStrength;
	}
	
	public void Heal(int pts)
	{
		_currentHP = Mathf.Min(_currentHP+pts, _maxHP);	
	}
	
	protected TacticsTargetSelection _targetSelection;
	public TacticsTargetSelection TargetSelection
	{
		get{return _targetSelection;}
		set{_targetSelection = value;}
	}
	
	protected TacticsBasePosition _basePosition;
	public TacticsBasePosition BasePosition
	{
		get{return _basePosition;}
		set{_basePosition = value;}
	}
	
	protected Skill _skill_1;
	public Skill Skill_1
	{
		get{return _skill_1;}
		set{_skill_1 = value;}
	}
	
	public void SetMainStats (string name, int HP, int[] secondaryStats)
	{
		_myName = name;
		_maxHP = HP;
		_currentHP = _maxHP;
		
		for(int i=0; i< (int) SecondaryStatType.LENGTH; ++i)
		{
			_secondaryStats[i] = secondaryStats[i];	
		}
	}
}