using UnityEngine;
using System.Collections;

public enum HeroClassType
{
	Warrior = 0,
	Manatarms,
	Archer,
	Rogue,
	Sage,
	Cleric,
	Wizard,
	Sorcerer,
	LENGTH,
}

public enum MainStatType 
{
	Strength = 0,
	Vitality,
	Agility,
	Guile,
	Wisdom,
	Piety,
	Intelligence,
	Willpower,
	LENGTH,
}

public enum SecondaryStatType 
{
	Damage = 0,
	HandToHand,
	Block,
	Compassion,
    moveSpeed,
    range,
    size,
	LENGTH,
}

public class Hero : Goon {
	
	#region Fields
	
	public GameObject ItemsContainer;
	
	// Base stats
	private int _heroClass;
	public int HeroClass
    {
        get
        {
            return _heroClass;
        }
        set
        {
            _heroClass = value;
        }
    }	
	
	private int _level;
	public int Level
    {
        get
        {
            return _level;
        }
        set
        {
            _level = value;
        }
    }		
	private int _xp;
	public int XP
    {
        get
        {
            return _xp;
        }
        set
        {
            _xp = value;
        }
    }	
	
	private int[] _mainStatsBase = new int[(int) MainStatType.LENGTH];
	
	public int GetStatBase(MainStatType a)
	{
		return _mainStatsBase[(int) a];	
	}
	
	public void ChangeStatBase(MainStatType a, int change)
	{
		_mainStatsBase[(int) a] += change;
		
		RecalcStats();
	}
	
	private int[] _mainStatsActive = new int[(int) MainStatType.LENGTH];
	
	public int GetStatActive(MainStatType a)
	{
		return _mainStatsActive[(int) a];	
	}
	
	// INVENTORY
	private ItemManager[] _inventory = new ItemManager[CONSTANTS.InvCharSize];
	public ItemManager[] Inventory
	{
		get
		{
			return _inventory;	
		}
	}
	
	private ItemManager[] _equipment = new ItemManager[(int) EquipmentSlotType.LENGTH];
	public ItemManager[] Equipment
	{
		get
		{
			return _equipment;	
		}
	}
	
	public void RecalcStats()
	{
		for(int i=0; i<(int) MainStatType.LENGTH; ++i)
		{
			_mainStatsActive[i] = _mainStatsBase[i];	
		}
		
		_secondaryStats[(int) SecondaryStatType.Damage] = (float)_mainStatsActive[(int) MainStatType.Strength];
		_secondaryStats[(int) SecondaryStatType.HandToHand] = (float)_mainStatsActive[(int) MainStatType.Agility];
		_secondaryStats[(int) SecondaryStatType.Block] = (float)_mainStatsActive[(int) MainStatType.Guile];
		_secondaryStats[(int) SecondaryStatType.Compassion] = (float)_mainStatsActive[(int) MainStatType.Piety];
        _secondaryStats[(int) SecondaryStatType.moveSpeed] = ((float)_mainStatsActive[(int)MainStatType.Agility])*10;
        _secondaryStats[(int) SecondaryStatType.range] = 600f;
        _secondaryStats[(int) SecondaryStatType.size] = ((float) _mainStatsActive[(int)MainStatType.Strength])/2;

        _maxHP = 5*_mainStatsActive[(int) MainStatType.Vitality];
		if(_currentHP <= 0)
		{
			if(!_dead)
			{
				_currentHP = _maxHP;
			}
		}
		else
		{
			_currentHP = Mathf.Min(_currentHP, _maxHP);
		}
	}
	
	#endregion
	
	/*
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	*/
	
	// Returns TRUE and registers item in inventory IF empty spot in inventory is found
	public bool TryToPlaceItemInInventory(ItemManager item)
	{
		// Run over intentory until you reach a free slot.
		for(int i=0; i<CONSTANTS.InvCharSize; ++i)
		{
			if(_inventory[i] == null)
			{
				_inventory[i] = item;
				return true;
			}
		}
		
		return false;
	}
	
	#region Save/Load
		
	public void SaveHero(int i)
	{	
		System.IO.StreamWriter file = new System.IO.StreamWriter("HeroData"+i+".txt");
		
		file.WriteLine(_heroClass.ToString());
		file.WriteLine(_myName);
		for(int j=0; j < (int) MainStatType.LENGTH; ++j)
		{
			file.WriteLine(_mainStatsBase[j].ToString());
		}
		//file.WriteLine(_strength.ToString());
		//file.WriteLine(_agility.ToString());
		//file.WriteLine(_magic.ToString());
		
		file.WriteLine(_level.ToString());
		file.WriteLine(_xp.ToString());
		
		//SaveSkills(file);
		
		//SaveInventory(file);
		
		file.Close();
	}
	
	public void LoadHero(int i)
	{		
		System.IO.StreamReader file = new System.IO.StreamReader("HeroData"+i+".txt");
		
		_heroClass = int.Parse(file.ReadLine());
		_myName = file.ReadLine();		
		
		if(file.Peek() != -1)
		{
			for(int j=0; j < (int) MainStatType.LENGTH; ++j)
			{
				_mainStatsBase[j] = int.Parse(file.ReadLine());
			}
			//_strength = int.Parse(file.ReadLine());
			//_agility = int.Parse(file.ReadLine());
			//_magic = int.Parse(file.ReadLine());
			_level = int.Parse(file.ReadLine());
			_xp = int.Parse(file.ReadLine());
		}
		/*
		else
		{
			_level = 1;
			_xp = 0;
		
			switch(_heroClass)
			{
			case 0:
				_strength = 10;
				_agility = 5;
				_magic = 1;
				break;
			case 1:
				_strength = 5;
				_agility = 10;
				_magic = 3;
				break;
			case 2:
				_strength = 2;
				_agility = 2;
				_magic = 100;
				break;
			}	
		}
		*/
		
		//LoadSkills(file);
		
		//LoadInventory(file);
		
		file.Close();	
		
		RecalcStats();
	}
	
	
	/*
	private void SaveSkills(System.IO.StreamWriter file)
	{
		string skills = "";
		for (int i=0; i<_skillValues.Length; ++i)
		{
			if(_skillValues[i])
			{
				skills += "1";	
			}
			else
			{
				skills+= "0";	
			}
		}
		
		file.WriteLine(skills);
		
		file.WriteLine(_quickSkills[0]);
		file.WriteLine(_quickSkills[1]);
		file.WriteLine(_quickSkills[2]);
		file.WriteLine(_quickSkills[3]);
		file.WriteLine(_quickSkills[4]);
	}
	
	private void LoadSkills(System.IO.StreamReader file)
	{
		string skills = file.ReadLine();
				
		for (int i=0; i<_skillValues.Length; ++i)
		{
			if(skills[i] == '1')
			{
				_skillValues[i] = true;	
			}
			else
			{
				_skillValues[i] = false;
			}
		}
		
		_quickSkills[0] = int.Parse(file.ReadLine());
		_quickSkills[1] = int.Parse(file.ReadLine());
		_quickSkills[2] = int.Parse(file.ReadLine());
		_quickSkills[3] = int.Parse(file.ReadLine());
		_quickSkills[4] = int.Parse(file.ReadLine());
	}
	
	private void SaveInventory(System.IO.StreamWriter file)
	{
		// store inventory
		for (int i=0; i<_items.Length; ++i)
		{
			if(_items[i] != null)
			{
				file.WriteLine(i);
				_items[i].GetComponent<ItemData>().SaveToFile(file);
			}
		}
		
		// store equipment
		for (int i=0; i<_equippedItems.Length; ++i)
		{
			if(_equippedItems[i] != null)
			{
				file.WriteLine(_items.Length + i);
				_equippedItems[i].GetComponent<ItemData>().SaveToFile(file);
			}
		}
	}
	
	private void LoadInventory(System.IO.StreamReader file)
	{
		while(file.Peek() != -1)
		{
			int i = int.Parse(file.ReadLine());
			// item in inventory
			if(i < _items.Length)
			{
				GameObject placeholder = new GameObject();
				placeholder.AddComponent<ItemData>();
				placeholder.GetComponent<ItemData>().LoadFromFile(file);
				AddItemInventory(i, placeholder.GetComponent<ItemData>());
				Destroy(placeholder);
			}
			// item equipped
			else
			{
				i = i % _items.Length;
				GameObject placeholder = new GameObject();
				placeholder.AddComponent<ItemData>();
				placeholder.GetComponent<ItemData>().LoadFromFile(file);
				AddItemEquipment(i, placeholder.GetComponent<ItemData>());
				Destroy(placeholder);
			}
		}
	}
	
	*/
	
	#endregion
}