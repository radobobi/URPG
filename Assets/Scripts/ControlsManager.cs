using UnityEngine;
using System.Collections;

public enum GameMode
{
	Town = 0,
	Adventure,
	LENGTH
}

public enum BattleStatus
{
	Scouting = 0,
	Engaged,
	Victorious,
    MidFight,
    LENGTH
}

public enum MiddlePanelType
{
	BattleLog = 0,
	LootList,
	LENGTH
}

public enum RightPanelType
{
	CharSheet = 0,
	Inventory,
	Skills,
	Tactics,
	Statistics,
	LENGTH
}

public enum EquipmentSlotType
{
	MainHand = 0,
	OffHand,
	Head,
	Cloak,
	Chest,
	Gloves,
	Boots,
	LENGTH
}

public class ControlsManager : MonoBehaviour {
	
	#region Field
	
	public GameObject CharactersContainer;
	public GameObject ItemsContainer;
	
	private GameMode _gameMode = GameMode.Town;
	// LEFT PANEL
	private Hero[] _characters = new Hero[6];
	
	private int _selectedCharacter = -1;
	
	// MIDDLE PANEL
	private MiddlePanelType _middlePanel = MiddlePanelType.BattleLog;
	private string _lastLog = "";
	private int _logLength=0;
	
	private Vector2 _scrollViewVector = Vector2.zero;
	
	// RIGHT PANEL
		// Inventory Window
	private RightPanelType _rightPanel = RightPanelType.CharSheet;
	
	private Texture[] _itemTextures = new Texture[(int) ItemType.LENGTH];
	private Texture2D _invRagdoll;// = Resources.Load("InvBody") as Texture;
	
	private ItemManager _itemUnderCursor;
	

		// Skills Window
	private GUIContent[] _skills_1_List;
	private ComboBox _skills_1_Control;// = new ComboBox();
	//private GUIStyle _listStyle = new GUIStyle();
	Rect _skills_1_Box = new Rect(0, 0, 150, 30);
	
		// Tactics Windows
	private GUIContent[] _tacticsTargetSelectionList;
	private ComboBox _tacticsTargetSelectionControl;// = new ComboBox();
	private GUIStyle _listStyle = new GUIStyle();
	Rect _tacticsTargetSelectionBox = new Rect(0, 0, 150, 30);
	
	private GUIContent[] _tacticsBasePositionList;
	private ComboBox _tacticsBasePositionControl;// = new ComboBox();
	//private GUIStyle _listStyle = new GUIStyle();
	Rect _tacticsBasePositionBox = new Rect(0, 50, 150, 30);
	
	
	#endregion
	
	#region Start/Update/OnGUI
	
	// Use this for initialization
	void Start () {
		//Initialize characters
		for (int i=0; i<=5; ++i)
		{
			_characters[i] = CharactersContainer.AddComponent<Hero>();
			_characters[i].LoadHero(i);
		}
		
		//Initialize item icons
		for (int i=0; i< (int) ItemType.LENGTH; ++i)
		{
			_itemTextures[i] = Resources.Load(((ItemType)i).ToString()+" Icon") as Texture;	
		}
		_invRagdoll = Resources.Load("InvBody") as Texture2D;
		_background = Resources.Load("MenuBox") as Texture;
		
		_listStyle.normal.textColor = Color.white; 
		_listStyle.onHover.background =
		_listStyle.hover.background = new Texture2D(2, 2);
		_listStyle.padding.left =
		_listStyle.padding.right =
		_listStyle.padding.top =
		_listStyle.padding.bottom = 4;
		
		// tactics menus init
		_tacticsTargetSelectionList = new GUIContent[(int) TacticsTargetSelection.LENGTH];
		for( int i=0; i< (int) TacticsTargetSelection.LENGTH; ++i)
		{
			_tacticsTargetSelectionList[i] = new GUIContent(((TacticsTargetSelection) i).ToString());
		}
		_tacticsTargetSelectionControl = new ComboBox(
			_tacticsTargetSelectionBox, new GUIContent("Pick Tactic"), 
			_tacticsTargetSelectionList, "button", "box", _listStyle);
		
		_tacticsBasePositionList = new GUIContent[(int) TacticsBasePosition.LENGTH];
		for( int i=0; i< (int) TacticsBasePosition.LENGTH; ++i)
		{
			_tacticsBasePositionList[i] = new GUIContent(((TacticsBasePosition) i).ToString());
		}
		_tacticsBasePositionControl = new ComboBox(
			_tacticsBasePositionBox, new GUIContent("Pick Tactic"), 
			_tacticsBasePositionList, "button", "box", _listStyle);
		
		_skills_1_List = new GUIContent[(int) Skill.LENGTH];
		for( int i=0; i< (int) Skill.LENGTH; ++i)
		{
			_skills_1_List[i] = new GUIContent(((Skill) i).ToString());
		}
		_skills_1_Control = new ComboBox(
			_skills_1_Box, new GUIContent("Pick Skill"), 
			_skills_1_List, "button", "box", _listStyle);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	private ItemManager[] _lootList = new ItemManager[CONSTANTS.LootSize];
	
	private void LootListClear()
	{
		for( int i=0; i < _lootList.Length; ++i)
		{
			Destroy(_lootList[i]);
			_lootList[i] = null;	
		}
	}
	
	private void LootListGenerate(int size)
	{
		size = Mathf.Min(size, CONSTANTS.LootSize);
		
		for ( int i=0; i < size; ++i)
		{
			_lootList[i] = ItemsContainer.AddComponent<ItemManager>();
		}
	}
	
	void OnGUI () {
		_itemUnderCursorUnclicked = null;
		
		//Left Panel (Charactes Panel)
		int leftPanelWidth = Screen.width / 4;
		int leftPanelHeight = Screen.height;
		GUI.BeginGroup (new Rect (0, 0, leftPanelWidth, leftPanelHeight));
		for (int i=0; i<=5; ++i)
		{
			LeftPanelButtons(i,leftPanelWidth, leftPanelHeight);
			//_characters[i].LoadHero(i);
		}
		GUI.EndGroup ();
		
		//Middle Panel
		int midPanelWidth = Screen.width / 2;
		int midPanelHeight = Screen.height;
		GUI.BeginGroup (new Rect (leftPanelWidth, 0, midPanelWidth, midPanelHeight));
		GUI.Box (new Rect (0, 0, midPanelWidth, 3*midPanelHeight/4), "MAIN PANEL");
		
		switch(_gameMode)
		{
		case GameMode.Town:
			
			break;
			
		case GameMode.Adventure:
			
			switch(_middlePanel)
			{
			case MiddlePanelType.BattleLog:
				switch(_battleStatus)
				{
				case BattleStatus.Scouting:
					//GUI.EndScrollView();
					GUI.Label(new Rect (0, 30, midPanelWidth, 3*midPanelHeight/4), 
						"Exploring " + _dungeon.MyName + ".");
					break;
					
				case BattleStatus.Engaged:
					//GUI.EndScrollView();
					GUI.Label(new Rect (0, 30, midPanelWidth, 3*midPanelHeight/4), 
						"You have encountered "+_currentBattle.CurrentMob.ToString()+".");
					break;
					
				case BattleStatus.Victorious:
					_scrollViewVector = GUI.BeginScrollView (new Rect (0, 0, midPanelWidth, 3*midPanelHeight/4), 
					_scrollViewVector, new Rect (0, 0, midPanelWidth, 16*_logLength));
					_lastLog = GUI.TextArea (new Rect (0, 0, midPanelWidth, 16*_logLength), _lastLog);
					GUI.EndScrollView();
					break;
                case BattleStatus.MidFight:
                    _scrollViewVector = GUI.BeginScrollView(new Rect(0, 0, midPanelWidth, 3 * midPanelHeight / 4),
                    _scrollViewVector, new Rect(0, 0, midPanelWidth, 16 * _logLength));
                    _lastLog = GUI.TextArea(new Rect(0, 0, midPanelWidth, 16 * _logLength), _lastLog);
                    GUI.EndScrollView();
                    break;
                }
				break;
			case MiddlePanelType.LootList:
				MiddlePanelLoot(midPanelWidth, 3*midPanelHeight/4);
				break;
			}
			
			break;
		}
		
		
		GUI.EndGroup();
		GUI.BeginGroup(new Rect(leftPanelWidth, 3*midPanelHeight/4, midPanelWidth, midPanelHeight/4));
		GUI.Box (new Rect (0, 0, midPanelWidth, midPanelHeight/4), "BUTTONS AREA");
		switch(_gameMode)
		{
		case GameMode.Town:
			MiddlePanelButtonsTown(midPanelWidth, midPanelHeight/4);
			break;
			
		case GameMode.Adventure:
			MiddlePanelButtonsAdventure(midPanelWidth, midPanelHeight/4);
			break;
		}
		GUI.EndGroup ();
		
		//Right Panel
		int rightPanelWidth = Screen.width / 4;
		int rightPanelHeight = Screen.height;
		GUI.BeginGroup (new Rect (leftPanelWidth+midPanelWidth, 0, rightPanelWidth, rightPanelHeight));
		GUI.Box (new Rect (0, 0, rightPanelWidth, rightPanelHeight), 
			_selectedCharacter == -1 ? "NO SELECTION" : _characters[_selectedCharacter].MyName);
		
		RightPanelButtons(rightPanelWidth,rightPanelHeight);
		
		switch (_rightPanel)
		{
    		case RightPanelType.CharSheet:
				GUI.Label (new Rect (0, rightPanelHeight/10, rightPanelWidth, 50), "CHARSHEET");
				GUI.BeginGroup (new Rect (0, rightPanelHeight/10, rightPanelWidth, 9*rightPanelHeight/10));
				RightPanelCharSheet(rightPanelWidth, 9*rightPanelHeight/10);
				GUI.EndGroup ();
        		break;
    		case RightPanelType.Inventory:
        		GUI.Label (new Rect (0, rightPanelHeight/10, rightPanelWidth, 50), "INVENTORY");
				GUI.BeginGroup (new Rect (0, rightPanelHeight/10, rightPanelWidth, 9*rightPanelHeight/10));
				RightPanelInventory(rightPanelWidth, 9*rightPanelHeight/10);
				GUI.EndGroup ();
        		break;
			case RightPanelType.Skills:
        		GUI.Label (new Rect (0, rightPanelHeight/10, rightPanelWidth, 50), "SKILLS");
				GUI.BeginGroup (new Rect (0, rightPanelHeight/10, rightPanelWidth, 9*rightPanelHeight/10));
				RightPanelSkills(rightPanelWidth, 9*rightPanelHeight/10);
				GUI.EndGroup ();
        		break;
    		case RightPanelType.Tactics:
        		GUI.Label (new Rect (0, rightPanelHeight/10, rightPanelWidth, 50), "TACTICS");
				GUI.BeginGroup (new Rect (0, rightPanelHeight/10, rightPanelWidth, 9*rightPanelHeight/10));
				RightPanelTactics(rightPanelWidth, 9*rightPanelHeight/10);
				GUI.EndGroup ();
        		break;    		
			case RightPanelType.Statistics:
        		GUI.Label (new Rect (0, rightPanelHeight/10, rightPanelWidth, 50), "STATS");
        		break;
		}
		
		GUI.EndGroup ();
		
		if(_itemUnderCursor != null)
		{
			Event e = Event.current;
			
			GUI.Label(new Rect(e.mousePosition.x, e.mousePosition.y, 100,100), 
				_itemTextures[(int) _itemUnderCursor.MyItemType]);	
		}
		
		if(_itemUnderCursorUnclicked != null)
		{
			DisplayItemStats(_itemUnderCursorUnclicked);
		}
	}
	
	#endregion
	
	#region LeftPanel
	
	private void LeftPanelButtons(int i, int panelWidth, int panelHeight)
	{
		GUI.BeginGroup (new Rect ((i%2)*(panelWidth/2), (i%3)*(panelHeight/3), panelWidth/2, panelHeight/3));
		if(GUI.Button (new Rect (0, 0, panelWidth/2, panelHeight/3), "CHARACTER"+i))
		{
			if(_itemUnderCursor != null)
			{
				if(_characters[i].TryToPlaceItemInInventory(_itemUnderCursor))
				{
					_itemUnderCursor = null;	
				}
			}
			else
			{
				if (_selectedCharacter == i)
				{
					_selectedCharacter = -1;	
				}
				else
				{
					_selectedCharacter = i;	
					//_tacticsTargetSelectionControl.Show();
					
					// skills clear boxes
					_skills_1_Control.SelectedItemIndex = (int) _characters[i].Skill_1;
					
					// tactics clear boxes
					_tacticsTargetSelectionControl.SelectedItemIndex = (int) _characters[i].TargetSelection;
					_tacticsBasePositionControl.SelectedItemIndex = (int) _characters[i].BasePosition;
				}
			}
		}
		GUI.EndGroup ();
	}
	
	#endregion
	
	#region MiddlePanel
	
	private void MiddlePanelButtonsTown(int panelWidth, int panelHeight)
	{
		if(GUI.Button(new Rect(0, 0, panelWidth/3, panelHeight/2), "Embark on an adventure!"))
		{
			if(_itemUnderCursor != null)
			{
				return;	
			}
			
			_gameMode = GameMode.Adventure;
			_dungeon = new DungeonManager(CONSTANTS.DungeonBase);
		}
	}
	
	private DungeonManager _dungeon;
	
	private BattleStatus _battleStatus = BattleStatus.Scouting;
	
	private BattleManager _currentBattle;
	
	private void MiddlePanelButtonsAdventure(int panelWidth, int panelHeight)
	{
		if(_battleStatus == BattleStatus.Victorious)
		{
			if(GUI.Button(new Rect(0, 0, panelWidth/3, panelHeight/2), "Last Battle Log"))
			{
				if(_itemUnderCursor != null)
				{
					return;	
				}
				_middlePanel = MiddlePanelType.BattleLog;
			}
		}
		
		switch(_battleStatus)
		{
		case BattleStatus.Scouting:	
			if(GUI.Button(new Rect(0, panelHeight/2, panelWidth/3, panelHeight/2), "Explore"))
			{
				if(_itemUnderCursor != null)
				{
					return;	
				}
				
				_currentBattle = CharactersContainer.AddComponent<BattleManager>();
				//newBattle.InitializeBattle(_characters);
				_currentBattle.RegisterHeroes(_characters);
				_currentBattle.SpawnGoons(_dungeon.GenerateMob());
				_battleStatus = BattleStatus.Engaged;
			}
			break;
			
		case BattleStatus.Engaged:
        case BattleStatus.MidFight:
			if(GUI.Button(new Rect(0, panelHeight/2, panelWidth/3, panelHeight/2), "Fight"))
			{
				if(_itemUnderCursor != null)
				{
					return;	
				}
                print("battle is starting ");
				bool battleOver = _currentBattle.ConductBattle();
				_lastLog = _currentBattle.Log;
				_logLength = StringLinesCount(_lastLog);
                //_battleStatus = BattleStatus.MidFight;
                print("is battle over: " + battleOver);
                if (battleOver)
                    {
                        print("battle is over ");
                        _battleStatus = BattleStatus.Victorious;
                        LootListClear();
                        LootListGenerate(Random.Range(0, CONSTANTS.LootSize));
                        Destroy(_currentBattle);
                    }
			}
			break;

		case BattleStatus.Victorious:
			if(GUI.Button(new Rect(0, panelHeight/2, panelWidth/3, panelHeight/2), "Proceed"))
			{
				if(_itemUnderCursor != null)
				{
					return;	
				}
				
				_battleStatus = BattleStatus.Scouting;
			}
			break;
		}
			
		if(_battleStatus == BattleStatus.Victorious)
		{
			if(GUI.Button(new Rect(panelWidth/3, 0, panelWidth/3, panelHeight/2), "Loot"))
			{
				_middlePanel = MiddlePanelType.LootList;
			}
		}
		
		if(GUI.Button(new Rect(2*panelWidth/3, panelHeight/2, panelWidth/3, panelHeight/2), "Back to town."))
		{
			if(_itemUnderCursor != null)
			{
				return;	
			}
			
			_gameMode = GameMode.Town;
			LootListClear();
			_lastLog = "";
		}
	}
	
	private void MiddlePanelLoot(int panelWidth, int panelHeight)
	{
		int slotWidth = panelWidth/CONSTANTS.LootCols;
		int slotHeight = panelHeight/(5*CONSTANTS.LootRows);
		
		ItemsSubpanel(slotWidth, slotHeight, _lootList, CONSTANTS.LootCols, CONSTANTS.LootRows);
	}
	
	private ItemManager _itemUnderCursorUnclicked;
	
	private void ItemsSubpanel(int slotWidth, int slotHeight, ItemManager[] selectedInventory, int cols, int rows)
	{
		// Base display & react loop
		for (int i=0; i < cols; ++i)
		{
			for (int j=0; j < rows; ++j)
			{
				int slotnumber = i*rows+j;
				Rect slotRect = new Rect (i*slotWidth, j*slotHeight, slotWidth, slotHeight);
				//Event e = Event.current;
				
				ItemsManageSlot(slotRect, ref selectedInventory[slotnumber]);
			}
		}
		
		// Mouse Hover detection loop
		for (int i=0; i < cols; ++i)
		{
			for (int j=0; j < rows; ++j)
			{
				int slotnumber = i*rows+j;
				
				if(selectedInventory[slotnumber] != null)
				{
					Rect slotRect = new Rect (i*slotWidth, j*slotHeight, slotWidth, slotHeight);
					Event e = Event.current;
					
					//MOUSEHOVER TEST
					if(slotRect.Contains(e.mousePosition))
					{
						_itemUnderCursorUnclicked = selectedInventory[slotnumber];
						//DisplayItemStats(selectedInventory[slotnumber], e.mousePosition);
						//GUI.Label(new Rect(slotRect.xMax, slotRect.yMin, 100, 100), 
						//	selectedInventory[slotnumber].ToString());	
					}
				}
			}
		}
	}
	
	private Texture _background;
	private void DisplayItemStats(ItemManager item)
	{
		if(item == null || item.ItemBonuses == null)
		{
			return;	
		}
		
		Vector2 mousePositionReal = Input.mousePosition;
		
		int drop = CONSTANTS.ItemDisplayVerticalLinedrop;
		
		int bonusCount = item.ItemBonuses.Length;
		
		int baseWidth = CONSTANTS.ItemDisplayBoxWidth;
		int baseHeight = 10+drop*(4+bonusCount);
		
		int mouseX = (int) mousePositionReal.x;
		int mouseY = (int) (Screen.height - mousePositionReal.y);
		
		int boxX = (mouseX+baseWidth) < Screen.width ? 
			mouseX : mouseX - baseWidth;
		int boxY = (mouseY+baseHeight) < Screen.height ? 
			mouseY : mouseY - baseHeight;
		
		Rect statsBox = new Rect(boxX, boxY, 
			baseWidth, 10+drop*(4+bonusCount));
		
		GUI.Label(statsBox, _background);
		GUI.contentColor = Color.black;
		GUI.Label(new Rect(5+boxX, boxY + 5, 
			baseWidth, drop), item.ItemName);
		GUI.contentColor = Color.red;
		GUI.Label(new Rect(5+boxX, boxY + 5 + drop, 
			baseWidth, drop), item.MyItemType.ToString());
		GUI.contentColor = Color.green;
		GUI.Label(new Rect(5+boxX, boxY + 5 + 2*drop, 
			baseWidth, drop), item.MyItemClass.ToString());
		
		GUI.contentColor = Color.blue;
		for(int i=0; i < bonusCount; ++i)
		{
			GUI.Label(new Rect(5+boxX, boxY + 10 + 3*drop + i*drop, 
				baseWidth, drop), 
				item.ItemBonusValues[i] + " " +item.ItemBonuses[i].ToString());	
		}
		
		
	}
	
	#endregion
	
	#region RightPanel
	
	private void RightPanelTactics(int panelWidth, int panelHeight)
	{
		if(_selectedCharacter == -1)
		{
			return;
		}
		
		Hero myChar = _characters[_selectedCharacter];
		
		myChar.TargetSelection = (TacticsTargetSelection) _tacticsTargetSelectionControl.Show();
		GUI.Label (new Rect (150, 0, panelWidth-150, 30), 
			"Current Target Selection: "+myChar.TargetSelection.ToString());
		
		myChar.BasePosition = (TacticsBasePosition) _tacticsBasePositionControl.Show();
		GUI.Label (new Rect (150, 50, panelWidth-150, 30), 
			"Current Base Position: "+myChar.BasePosition.ToString());
	}
			
	private void RightPanelCharSheet(int panelWidth, int panelHeight)
	{
		if(_selectedCharacter != -1)
		{
			GUI.Label (new Rect (0, 15, panelWidth/2, 50), "Name: "+_characters[_selectedCharacter].MyName);
			GUI.Label (new Rect (0, 30, panelWidth/2, 50), 
				"Class: "+ ((HeroClassType) _characters[_selectedCharacter].HeroClass).ToString());
			GUI.Label (new Rect (0, 45, panelWidth/2, 50), "Level: "+_characters[_selectedCharacter].Level);
			GUI.Label (new Rect (0, 60, panelWidth/2, 50), "HP: "
				+_characters[_selectedCharacter].CurrentHP+"/"+_characters[_selectedCharacter].MaxHP);
			int skip = 15;
			int start = 90;
			for(int i=0; i< (int) MainStatType.LENGTH; ++i)
			{
				int baseStat = _characters[_selectedCharacter].GetStatBase((MainStatType)i);
				int activeStat = _characters[_selectedCharacter].GetStatActive((MainStatType)i);
				
				GUI.Label (new Rect (0, start+i*skip, panelWidth/2, 50), 
					((MainStatType)i).ToString() + ": " + activeStat + "("+baseStat +")");
			}
			start += (skip*((int) MainStatType.LENGTH)+10);
			for(int i=0; i< (int) SecondaryStatType.LENGTH; ++i)
			{
				int secondaryStat = _characters[_selectedCharacter].GetStatSecondary((SecondaryStatType)i);
				GUI.Label (new Rect (0, start+i*skip, panelWidth/2, 50), 
					((SecondaryStatType)i).ToString() + ": " + secondaryStat);
			}
		}
	}
	
	private void RightPanelInventory(int panelWidth, int panelHeight)
	{
		if(_selectedCharacter == -1)
		{
			return;
		}
	
		Hero character = _characters[_selectedCharacter];
		
		// EQUIPMENT
		
		ItemManager[] selectedEquipment = character.Equipment;
		
		int slotWidth = panelWidth/CONSTANTS.InvCharCols;
		int slotHeight = panelHeight/(2*CONSTANTS.InvCharRows);
		
		GUI.Label(new Rect(panelWidth/6,0, panelWidth, panelHeight/2), _invRagdoll);
		
		Rect equipmentHead = new Rect(panelWidth/2, 1*panelHeight/16, slotWidth, slotHeight);
		ItemsManageSlot(equipmentHead, 
			ref selectedEquipment[(int) EquipmentSlotType.Head], EquipmentSlotType.Head);
		Rect equipmentChest = new Rect(panelWidth/2, 3*panelHeight/16, slotWidth, slotHeight);
		ItemsManageSlot(equipmentChest, 
			ref selectedEquipment[(int) EquipmentSlotType.Chest], EquipmentSlotType.Chest);
		Rect equipmentCloak = new Rect(3*panelWidth/4, 2*panelHeight/16, slotWidth, slotHeight);
		ItemsManageSlot(equipmentCloak, 
			ref selectedEquipment[(int) EquipmentSlotType.Cloak], EquipmentSlotType.Cloak);
		Rect equipmentMainHand = new Rect(panelWidth/4, 3*panelHeight/16, slotWidth, slotHeight);
		ItemsManageSlot(equipmentMainHand, 
			ref selectedEquipment[(int) EquipmentSlotType.MainHand], EquipmentSlotType.MainHand);
		Rect equipmentOffHand = new Rect(3*panelWidth/4, 4*panelHeight/16, slotWidth, slotHeight);
		ItemsManageSlot(equipmentOffHand, 
			ref selectedEquipment[(int) EquipmentSlotType.OffHand], EquipmentSlotType.OffHand);
		Rect equipmentGloves = new Rect(panelWidth/4, 5*panelHeight/16, slotWidth, slotHeight);
		ItemsManageSlot(equipmentGloves, 
			ref selectedEquipment[(int) EquipmentSlotType.Gloves], EquipmentSlotType.Gloves);
		Rect equipmentBoots = new Rect(panelWidth/2, 6*panelHeight/16, slotWidth, slotHeight);
		ItemsManageSlot(equipmentBoots, 
			ref selectedEquipment[(int) EquipmentSlotType.Boots], EquipmentSlotType.Boots);
		
		// INVENTORY
		
		//ItemManager[] selectedInventory = character.Inventory;
		
		GUI.BeginGroup(new Rect(0, panelHeight/2, panelWidth, panelHeight/2));
		ItemsSubpanel(slotWidth, slotHeight, character.Inventory, CONSTANTS.InvCharCols, CONSTANTS.InvCharRows);
		GUI.EndGroup();
	}
	
	private void RightPanelSkills(int panelWidth, int panelHeight)
	{
		if(_selectedCharacter == -1)
		{
			return;
		}
		
		Hero myChar = _characters[_selectedCharacter];
		
		myChar.Skill_1 = (Skill) _skills_1_Control.Show();
		GUI.Label (new Rect (150, 0, panelWidth-150, 30), 
			"Current Skill: "+myChar.Skill_1.ToString());
	}
	
	private void ItemsManageSlot(Rect slotRect, ref ItemManager slotItem, EquipmentSlotType? slotType = null)
	{
		if(slotItem == null)
		{
			if(GUI.Button (slotRect, "SLOT") && Event.current.button == 0)
			{
				if(_itemUnderCursor != null && 
					(slotType == null || EquipmentSlotCheckItemCompatability(slotType.Value, _itemUnderCursor.MyItemClass)))
				{
					slotItem = _itemUnderCursor;
					_itemUnderCursor = null;
				}
				else
				{
					if(slotType == null)
					{
						//slotItem = ItemsContainer.AddComponent<ItemManager>();
					}
				}
			}
		}
		else
		{
			if(GUI.Button (slotRect, _itemTextures[(int) slotItem.MyItemType]) && Event.current.button == 0)
			{
				if(_itemUnderCursor != null && 
					(slotType == null || EquipmentSlotCheckItemCompatability(slotType.Value, _itemUnderCursor.MyItemClass)))
				{
					ItemManager tmp = _itemUnderCursor;
					_itemUnderCursor = slotItem;
					slotItem = tmp;
				}
				else
				{
					_itemUnderCursor = slotItem;
					slotItem = null;
				}
			}
		}
	}
	
	private void RightPanelButtons(int panelWidth, int panelHeight)
	{
		// Buttons
		if(GUI.Button (new Rect (0, panelHeight/20, panelWidth/5, panelHeight/20), "CharSheet"))
		{
			if (_rightPanel == RightPanelType.CharSheet)
			{
				_rightPanel = RightPanelType.CharSheet;		
			}
			else
			{
				_rightPanel = RightPanelType.CharSheet;	
			}
		}
		if(GUI.Button (new Rect (panelWidth/5, panelHeight/20, panelWidth/5, panelHeight/20), "Inventory"))
		{
			if (_rightPanel == RightPanelType.Inventory)
			{
				_rightPanel = RightPanelType.CharSheet;	
			}
			else
			{
				_rightPanel = RightPanelType.Inventory;	
			}
		}
		if(GUI.Button (new Rect (2*panelWidth/5, panelHeight/20, panelWidth/5, panelHeight/20), "Skills"))
		{
			if (_rightPanel == RightPanelType.Skills)
			{
				_rightPanel = RightPanelType.CharSheet;	
			}
			else
			{
				_rightPanel = RightPanelType.Skills;	
			}
		}
		if(GUI.Button (new Rect (3*panelWidth/5, panelHeight/20, panelWidth/5, panelHeight/20), "Tactics"))
		{
			if (_rightPanel == RightPanelType.Tactics)
			{
				_rightPanel = RightPanelType.CharSheet;	
			}
			else
			{
				_rightPanel = RightPanelType.Tactics;	
			}
		}
		if(GUI.Button (new Rect (4*panelWidth/5, panelHeight/20, panelWidth/5, panelHeight/20), "Statistics"))
		{
			if (_rightPanel == RightPanelType.Statistics)
			{
				_rightPanel = RightPanelType.CharSheet;	
			}
			else
			{
				_rightPanel = RightPanelType.Statistics;	
			}
		}
	}

	#endregion
	
	#region Utlity
	
	// returns true if item fits slot; false otherwise
	private bool EquipmentSlotCheckItemCompatability(EquipmentSlotType slotType, ItemClass itemClass)
	{
		//returnVal = false;
		switch(slotType)
		{
			// Helmet
		case EquipmentSlotType.Head:
			if(itemClass == ItemClass.Helmet)
			{
				return true;	
			}
			break;
			// Cloak
		case EquipmentSlotType.Cloak:		
			if(itemClass == ItemClass.Cloak)
			{
				return true;	
			}
			break;
			// Gloves			
		case EquipmentSlotType.Gloves:		
			if(itemClass == ItemClass.Gloves)
			{
				return true;	
			}
			break;
			// Armor 			
		case EquipmentSlotType.Chest:		
			if(itemClass == ItemClass.Armor)
			{
				return true;	
			}
			break;
			// Main-hand (1h or 2h weapon)		
		case EquipmentSlotType.MainHand:		
			if(itemClass == ItemClass.Melee1H)
			{
				return true;	
			}
			else if((itemClass == ItemClass.Melee2H || itemClass == ItemClass.Ranged2H) )
				//&& _equippedItems[5] == null)
			{
				return true;	
			}
			break;
			// off-hand (shielf or second weapon)		
		case EquipmentSlotType.OffHand:		
			if(itemClass == ItemClass.OffHand || itemClass == ItemClass.Melee1H)
			{
				//GUITexture mainhandItem = _equippedItems[4];
				//if(mainhandItem == null)
				//{
					return true;	
				//}
				//else if(!TwoHandedItemEquipped())
				//{
				//	return true;
				//}
			}
			break;
			// boots		
		case EquipmentSlotType.Boots:		
			if(itemClass == ItemClass.Boots)
			{
				return true;	
			}
			break;
		}
		
		return false;
	}
	
	private int StringLinesCount(string s)
	{
		int count = 0;
		
		for(int i=0; i<s.Length; ++i)
		{
			//print(s.Substring(i,2));
			if( s[i] == '\n')
			{
				
				++count;	
			}
		}
		
		return count;
	}
	
	#endregion
}
