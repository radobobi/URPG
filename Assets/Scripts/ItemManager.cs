using UnityEngine;
using System.Collections;

public enum ItemType 
{
	swordShort = 0,
	swordLong,
	axeHatchet,
	axeBattle,
	
	swordTwoHanded,
	axeTwoHanded,
	staff,
	bow,
	
	Shield,
		
	Helmet,
	
	Cloak,
	
	Armor,
	Tunic,
	Robe,
	
	Gloves,
	
	Boots,

	LENGTH,
}

public enum ItemClass
{
	Melee1H = 0,
	Melee2H,
	Ranged2H,
	OffHand,
	Helmet,
	Cloak,
	Armor,
	Gloves,
	Boots,
	LENGTH
}

public enum ItemBonusType
{
	PlusStrength = 0,
	PlusAgility = 1,
	PlusMagic = 2,
	PlusMaxHP = 3,
	PlusSpeed = 4,
	LENGTH = 5,
}

public class ItemManager : MonoBehaviour {
	
	private string _itemName;
	public string ItemName
	{
		get
        {
            return _itemName;
        }
        set
        {
            _itemName = value;
        }
	}
	
	private ItemType _itemType;
	public ItemType MyItemType
	{
		get
        {
            return _itemType;
        }
        set
        {
            _itemType = value;
        }
	}
	
	private ItemClass _itemClass;
	public ItemClass MyItemClass
	{
		get
        {
            return _itemClass;
        }
        set
        {
            _itemClass = value;
        }
	}
	
	private ItemBonusType[] _itemBonuses;
	public ItemBonusType[] ItemBonuses{get{return _itemBonuses;}}
	private float[] _itemBonusValues;
	public float[] ItemBonusValues{get{return _itemBonusValues;}}
	
	// Use this for initialization
	void Start () {
		// TEMPORARY
		// Assigns a random type at item generation
		//_itemType = (ItemType) Random.Range(0, (int) ItemType.LENGTH);
		SpawnRandom();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	// assumes the item already has a type
	private void SetClass()
	{
		switch(_itemType)
		{
		case ItemType.swordShort:
		case ItemType.swordLong:
		case ItemType.axeHatchet:
		case ItemType.axeBattle:
			_itemClass = ItemClass.Melee1H;
			break;
			
		case ItemType.swordTwoHanded:
		case ItemType.axeTwoHanded:
		case ItemType.staff:
			_itemClass = ItemClass.Melee2H;
			break;
			
		case ItemType.bow:
			_itemClass = ItemClass.Ranged2H;
			break;
			
		case ItemType.Shield:
			_itemClass = ItemClass.OffHand;
			break;
			
		case ItemType.Helmet:
			_itemClass = ItemClass.Helmet;
			break;
				
		case ItemType.Cloak:
			_itemClass = ItemClass.Cloak;
			break;
			
		case ItemType.Armor:
		case ItemType.Tunic:
		case ItemType.Robe:
			_itemClass = ItemClass.Armor;
			break;
			
		case ItemType.Gloves:
			_itemClass = ItemClass.Gloves;
			break;
			
		case ItemType.Boots:
			_itemClass = ItemClass.Boots;
			break;
		}
	}
	
	public void SetBonuses(ItemBonusType[] bonuses)
	{
		_itemBonuses = bonuses;
	}
	
	public void SetBonusValues(float[] bonusValues)
	{
		_itemBonusValues = bonusValues;	
	}
	
	public void SpawnRandom()
	{
		_itemType = (ItemType) Random.Range(0, (int) ItemType.LENGTH);
		SetClass();
		_itemName = _itemType.ToString();
		
		int numberOfBonuses = Random.Range(1,6);
		
		_itemBonuses = new ItemBonusType[numberOfBonuses];
		_itemBonusValues = new float[numberOfBonuses];
		for(int i=0; i<numberOfBonuses; ++i)
		{
			_itemBonuses[i] = (ItemBonusType) Random.Range(0, (int) ItemBonusType.LENGTH);
			_itemBonusValues[i] = Random.Range(1f, 10f);
		}
	}
	
	public void SaveToFile(System.IO.StreamWriter file)
	{
		file.WriteLine(_itemName);
		file.WriteLine(((int)_itemType).ToString());
		file.WriteLine(_itemBonuses.Length.ToString());
		for(int i=0; i<_itemBonuses.Length; ++i)
		{
			file.WriteLine(((int)_itemBonuses[i]).ToString());
			file.WriteLine(_itemBonusValues[i].ToString());
		}
	}
	
	public void LoadFromFile(System.IO.StreamReader file)
	{
		_itemName = file.ReadLine();
		_itemType = (ItemType) int.Parse(file.ReadLine());
		SetClass();
		
		int numberOfBonuses = int.Parse(file.ReadLine());
		
		_itemBonuses = new ItemBonusType[numberOfBonuses];
		_itemBonusValues = new float[numberOfBonuses];
		
		for(int i=0; i<numberOfBonuses; ++i)
		{
			_itemBonuses[i] = (ItemBonusType) int.Parse(file.ReadLine());
			_itemBonusValues[i] = float.Parse(file.ReadLine());
		}
	}
	
	public override string ToString ()
	{
		string returnValue = "Name: " + _itemName + "\n" + "Type: " + _itemType.ToString()
			+ "\n" + "Class: " + _itemClass.ToString();
		
		if(_itemBonuses == null)
		{
			return returnValue;	
		}
		
		for(int i=0; i<_itemBonuses.Length; ++i)
		{
			returnValue = returnValue + "\n" + _itemBonuses[i].ToString() + 
				": +" + _itemBonusValues[i].ToString();	
		}
		
		return returnValue;
	}
}
