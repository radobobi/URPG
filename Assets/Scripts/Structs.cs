using UnityEngine;
using System.Collections;

public struct Enemy
{
	private string _myName;
	public string MyName{get{return _myName;}}
	private int[] _stats;
	public int[] Stats{get{return _stats;}}
	private int _HP;
	public int HP{get{return _HP;}}
	
	public Enemy(string myName, int[] stats, int HP)
	{
		_myName = myName;
		_stats = stats;
		_HP = HP;
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
