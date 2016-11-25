using UnityEngine;
using System.Collections;

public struct Enemy
{
	private string _myName;
	public string MyName{get{return _myName;}}
	private float[] _stats;
	public float[] Stats{get{return _stats;}}
	private float _HP;
	public float HP{get{return _HP;} }
    private float _XP;
    public float XP { get { return _XP; } }

    public Enemy(string myName, float[] stats, float HP, float XP)
	{
		_myName = myName;
		_stats = stats;
		_HP = HP;
        _XP = XP;
	}
}

public struct Mob
{
	private Enemy[] _mobTypes;
	public Enemy[] MobTypes{get{return _mobTypes;}}
	private int[] _mobCounts;
	public int[] MobCounts{get{return _mobCounts;}}
	
	public Mob(Enemy[] mobTypes, int[] mobCounts)
	{
		_mobTypes = mobTypes;
		_mobCounts = mobCounts;
	}
	
	public override string ToString ()
	{
		string s = "";
		
		for (int i=0; i<_mobCounts.Length; ++i)
		{
			if(i>0)
			{
			s += ", ";	
			}
			s += _mobCounts[i].ToString() + " " + _mobTypes[i].MyName;
		}
		
		return s;
	}
}

public struct Dungeon
{
	private string _myName;
	public string MyName{get{return _myName;}}
	
	private Mob[] _mobs;
	public Mob[] Mobs{get{return _mobs;}}
	private int[] _mobFrequencies;
	public int[] MobFrequencies{get{return _mobFrequencies;}}
	
	public Dungeon(string myName, Mob[] mobs, int[] mobFrequencies)
	{
		_myName = myName;
		_mobs = mobs;
		_mobFrequencies = mobFrequencies;
	}
}

