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

public enum MainPanelType
{
	BattleLog = 0,
	LootList,
	LENGTH
}

public enum UtilPanelType
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
    public GameObject ScriptsContainer;

    private DrawerInventory _drawerInventory;
    private DrawerUtils _drawerUtils;

    private GameMode _gameMode = GameMode.Town;
	// CHARACTERS PANEL
	private Hero[] _characters = new Hero[6];
	public Hero[] Characters
    {
        get
        {
            return _characters;
        }
    }

    private int _selectedCharacter = -1;
    public int SelectedCharacter
    {
        get
        {
            return _selectedCharacter;
        }
        set
        {
            _selectedCharacter = value;
        }
    }

    // MAIN PANEL
    private MainPanelType _mainPanel = MainPanelType.BattleLog;
	private string _lastLog = "";
	private int _logLength=0;
	
	private Vector2 _scrollViewVector = Vector2.zero;
	
	// UTILITY PANEL
		// Inventory Window


    private ItemManager _itemUnderCursor;
    public ItemManager ItemUnderCursor
    {
        get
        {
            return _itemUnderCursor;
        }
        set
        {
            _itemUnderCursor = value;
        }
    }

    private ItemManager _itemUnderCursorUnclicked;
    public ItemManager ItemUnderCursorUnclicked
    {
        get
        {
            return _itemUnderCursorUnclicked;
        }
        set
        {
            _itemUnderCursorUnclicked = value;
        }
    }

    private Texture[] _charTextures = new Texture[(int) GoonSprites.LENGTH];

    #endregion

    #region Start/Update/OnGUI

    // Use this for initialization
    void Start () {
        _drawerInventory = ScriptsContainer.AddComponent<DrawerInventory>();
        _drawerInventory.SetControlsManager(this);

        _drawerUtils = ScriptsContainer.AddComponent<DrawerUtils>();
        _drawerUtils.SetControlsManager(this);

        //Initialize characters
        for (int i=0; i<=5; ++i)
		{
			_characters[i] = CharactersContainer.AddComponent<Hero>();
			_characters[i].LoadHero(i);
		}
        for (int i = 0; i < (int)GoonSprites.LENGTH; ++i)
        {
            _charTextures[i] = Resources.Load(((GoonSprites)i).ToString()) as Texture2D;
        }
		_background = Resources.Load("MenuBox") as Texture;
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
	
    private void UpdateLogWindow(int logPanelWidth, int logPanelHeight)
    {
        _scrollViewVector = GUI.BeginScrollView(new Rect(0, 0, logPanelWidth, logPanelHeight),
                    _scrollViewVector, new Rect(0, 0, logPanelWidth, 16 * _logLength));
        _lastLog = GUI.TextArea(new Rect(0, 0, logPanelWidth, 16 * _logLength), _lastLog);
        GUI.EndScrollView();
    }

    private void UpdateBattleWindow(int panelWidth, int panelHeight)
    {
        float mapLength = 4000;
        float unit_radius = CONSTANTS.BaseUnitRadius;

        foreach(Goon goon in _currentBattle.GetGoonsList())
        {
            GUI.Label(new Rect(panelWidth * goon.MyPos.x / mapLength - goon.GetStatSecondary(SecondaryStatType.size) / 2, 
                panelHeight * goon.MyPos.y / mapLength - goon.GetStatSecondary(SecondaryStatType.size) / 2, 
                goon.GetStatSecondary(SecondaryStatType.size), 
                goon.GetStatSecondary(SecondaryStatType.size)), 
                _charTextures[!goon.Dead ? (int)GoonSprites.goon_base : (int)GoonSprites.goon_dead]);
        }
        foreach (Hero hero in _currentBattle.GetHeroesList())
        { 
            GUI.Label(new Rect(panelWidth * hero.MyPos.x / mapLength - hero.GetStatSecondary(SecondaryStatType.size) / 2, 
                panelHeight * hero.MyPos.y / mapLength - hero.GetStatSecondary(SecondaryStatType.size) / 2,
                hero.GetStatSecondary(SecondaryStatType.size),
                hero.GetStatSecondary(SecondaryStatType.size)), 
                _charTextures[!hero.Dead ? (int)GoonSprites.hero_base : (int)GoonSprites.hero_dead]);
        }
    }

    // NOTE: Is it necessary to compute panel sizes at every frame?
    void OnGUI () {
		_itemUnderCursorUnclicked = null;
		
		// Characters Panel (Charactes Panel)
		int charsPanelWidth = Screen.width / 16;
		int charsPanelHeight = Screen.height;
		GUI.BeginGroup (new Rect (Screen.width - charsPanelWidth, 0, charsPanelWidth, charsPanelHeight));
		for (int i=0; i<=5; ++i)
		{
            _drawerUtils.CharacterPanelButtons(i, charsPanelWidth, charsPanelHeight);
			//_characters[i].LoadHero(i);
		}
		GUI.EndGroup ();
		
		// Main Panel
		int mainPanelWidth = 10 * Screen.width / 16;
		int mainPanelHeight = 3 * Screen.height / 4;
		GUI.BeginGroup (new Rect (0, 0, mainPanelWidth, mainPanelHeight));
		GUI.Box (new Rect (0, 0, mainPanelWidth, mainPanelHeight), "MAP PANEL");
		
		switch(_gameMode)
		{
		case GameMode.Town:		
			break;
			
		case GameMode.Adventure:		
			switch(_mainPanel)
			{
			case MainPanelType.BattleLog:
				switch(_battleStatus)
				{
				case BattleStatus.Scouting:
					//GUI.EndScrollView();
					GUI.Label(new Rect (0, 30, mainPanelWidth, mainPanelHeight), 
						"Exploring " + _dungeon.MyName + ".");
					break;
	
				case BattleStatus.Engaged:
					//GUI.EndScrollView();
					GUI.Label(new Rect (0, 30, mainPanelWidth, mainPanelHeight), 
						"You have encountered " + _currentBattle.CurrentMob.ToString()+".");
					break;
                    
				case BattleStatus.Victorious:
                    UpdateBattleWindow(mainPanelWidth, mainPanelHeight);
                    break;
                case BattleStatus.MidFight:
                    UpdateBattleWindow(mainPanelWidth, mainPanelHeight);
                    break;
                    
                }
				break;
			case MainPanelType.LootList:
				MainPanelLoot(mainPanelWidth, mainPanelHeight);
				break;
			}
			
			break;
		}	
		GUI.EndGroup();

        // Log Panel
        int logPanelWidth = mainPanelWidth;
        int logPanelHeight = Screen.height / 4;
        GUI.BeginGroup(new Rect(0, mainPanelHeight, logPanelWidth, logPanelHeight));
        GUI.Box(new Rect(0, 0, logPanelWidth, logPanelHeight), "LOG PANEL");
        UpdateLogWindow(logPanelWidth, logPanelHeight);
        GUI.EndGroup ();

        // Buttons Panel
        int buttonPanelWidth = Screen.width / 16;
        int buttonPanelHeight = Screen.height;
        GUI.BeginGroup(new Rect(mainPanelWidth, 0, buttonPanelWidth, buttonPanelHeight));
        GUI.Box(new Rect(0, 0, buttonPanelWidth, buttonPanelHeight), "BUTTONS PANEL");
        switch (_gameMode)
        {
            case GameMode.Town:
                MainPanelButtonsTown(buttonPanelWidth, buttonPanelHeight);
                break;

            case GameMode.Adventure:
                ButtonsAdventure(buttonPanelWidth, buttonPanelHeight);
                break;
        }
        GUI.EndGroup();

        // Utility Panel
        int utilPanelWidth = Screen.width / 4;
		int utilPanelHeight = Screen.height;
		GUI.BeginGroup (new Rect (mainPanelWidth + buttonPanelWidth, 0, utilPanelWidth, utilPanelHeight));
		GUI.Box (new Rect (0, 0, utilPanelWidth, utilPanelHeight), 
			_selectedCharacter == -1 ? "NO SELECTION" : _characters[_selectedCharacter].MyName);

        _drawerUtils.UtilPanelButtons(utilPanelWidth, utilPanelHeight);
		
		switch (_drawerUtils.UtilPanel)
		{
    		case UtilPanelType.CharSheet:
				GUI.Label (new Rect (0, utilPanelHeight / 10, utilPanelWidth, 50), "CHARSHEET");
				GUI.BeginGroup (new Rect (0, utilPanelHeight / 10, utilPanelWidth, 9* utilPanelHeight / 10));
                _drawerUtils.UtilPanelCharSheet(utilPanelWidth, 9* utilPanelHeight / 10);
				GUI.EndGroup ();
        		break;
    		case UtilPanelType.Inventory:
        		GUI.Label (new Rect (0, utilPanelHeight / 10, utilPanelWidth, 50), "INVENTORY");
				GUI.BeginGroup (new Rect (0, utilPanelHeight / 10, utilPanelWidth, 9 * utilPanelHeight / 10));
                _drawerInventory.UtilPanelInventory(utilPanelWidth, 9* utilPanelHeight / 10,
                    _selectedCharacter == -1 ? null : _characters[_selectedCharacter]);
				GUI.EndGroup ();
        		break;
			case UtilPanelType.Skills:
        		GUI.Label (new Rect (0, utilPanelHeight / 10, utilPanelWidth, 50), "SKILLS");
				GUI.BeginGroup (new Rect (0, utilPanelHeight / 10, utilPanelWidth, 9 * utilPanelHeight / 10));
                _drawerUtils.UtilPanelSkills(utilPanelWidth, 9 * utilPanelHeight / 10);
				GUI.EndGroup ();
        		break;
    		case UtilPanelType.Tactics:
        		GUI.Label (new Rect (0, utilPanelHeight / 10, utilPanelWidth, 50), "TACTICS");
				GUI.BeginGroup (new Rect (0, utilPanelHeight / 10, utilPanelWidth, 9 * utilPanelHeight / 10));
                _drawerUtils.UtilPanelTactics(utilPanelWidth, 9 * utilPanelHeight / 10);
				GUI.EndGroup ();
        		break;    		
			case UtilPanelType.Statistics:
        		GUI.Label (new Rect (0, utilPanelHeight / 10, utilPanelWidth, 50), "STATS");
        		break;
		}
		
		GUI.EndGroup ();
		
		if(_itemUnderCursor != null)
		{
			Event e = Event.current;

            _drawerInventory.DrawItemAtMouseCursor(e.mousePosition.x, e.mousePosition.y);
        }
		
		if(_itemUnderCursorUnclicked != null)
		{
			DisplayItemStats(_itemUnderCursorUnclicked);
		}
	}

	#endregion
	
	#region CharacterPanel
	
	
	
	#endregion
	
	#region MainPanel
	
	private void MainPanelButtonsTown(int panelWidth, int panelHeight)
	{
        int horizontalCount = 1;
        int verticalCount = 6;

        // Position 0 button.
        if (GUI.Button(new Rect(
            (0 % horizontalCount) * panelWidth / horizontalCount,
            (0 % verticalCount) * panelHeight / verticalCount, 
            panelWidth / horizontalCount, panelHeight / verticalCount), "Embark!"))
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
	
	private void ButtonsAdventure(int panelWidth, int panelHeight)
	{
        int horizontalCount = 1;
        int verticalCount = 6;

		if(_battleStatus == BattleStatus.Victorious)
		{
            // Position 3 button
			if(GUI.Button(new Rect(
                (3 % horizontalCount) * panelWidth / horizontalCount,
                (3 % verticalCount) * panelHeight / verticalCount,
                panelWidth / horizontalCount, panelHeight / verticalCount), "Last Battle Log"))
			{
				if(_itemUnderCursor != null)
				{
					return;	
				}
				_mainPanel = MainPanelType.BattleLog;
			}
		}
		
		switch(_battleStatus)
		{
		case BattleStatus.Scouting:
                // Position 0 button
                if (GUI.Button(new Rect(
                    (0 % horizontalCount) * panelWidth / horizontalCount, 
                    (0 % verticalCount) * panelHeight / verticalCount, 
                    panelWidth / horizontalCount, panelHeight / verticalCount), "Explore"))
			{
				if(_itemUnderCursor != null)
				{
					return;	
				}
				_mainPanel = MainPanelType.BattleLog;
				_currentBattle = CharactersContainer.AddComponent<BattleManager>();
				//newBattle.InitializeBattle(_characters);
				_currentBattle.RegisterHeroes(_characters);
				_currentBattle.SpawnGoons(_dungeon.GenerateMob());
                _currentBattle.RegisterWriteToLogDelegate(WriteToLog);
                _currentBattle.RegisterBattleCleanupCallback(BattleCleanup);
				_battleStatus = BattleStatus.Engaged;
			}
			break;
			
		case BattleStatus.Engaged:
        case BattleStatus.MidFight:
            // Position 0 button.
			if(GUI.Button(new Rect(
                (0 % horizontalCount) * panelWidth / horizontalCount,
                (0 % verticalCount) * panelHeight / verticalCount,
                panelWidth / horizontalCount, panelHeight / verticalCount), "Fight"))
			{
				if(_itemUnderCursor != null)
				{
					return;	
				}
                //print("battle is starting ");
                _mainPanel = MainPanelType.BattleLog;
                _currentBattle.ConductBattle();
				//_lastLog = _currentBattle.Log;
				//_logLength = StringLinesCount(_lastLog);
                _battleStatus = BattleStatus.MidFight;
                //print("is battle over: " + battleOver);
			}
			break;

		case BattleStatus.Victorious:
            // Position 0 button.
            if (GUI.Button(new Rect(
                (0 % horizontalCount) * panelWidth / horizontalCount,
                (0 % verticalCount) * panelHeight / verticalCount,
                panelWidth / horizontalCount, panelHeight / verticalCount), "Proceed"))
			{
				if(_itemUnderCursor != null)
				{
					return;	
				}

                LootListClear();
                _mainPanel = MainPanelType.BattleLog;
				_battleStatus = BattleStatus.Scouting;
			}
			break;
		}
			
		if(_battleStatus == BattleStatus.Victorious)
		{
            // Position 4 button.
			if(GUI.Button(new Rect(
                (4 % horizontalCount) * panelWidth / horizontalCount,
                (4 % verticalCount) * panelHeight / verticalCount,
                panelWidth / horizontalCount, panelHeight / verticalCount), "Loot"))
			{
				_mainPanel = MainPanelType.LootList;
			}
		}
		
        // Position 5 button.
		if(GUI.Button(new Rect(
            (5 % horizontalCount) * panelWidth / horizontalCount,
            (5 % verticalCount) * panelHeight / verticalCount,
            panelWidth / horizontalCount, panelHeight / verticalCount), "Back to town."))
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
	
	private void MainPanelLoot(int panelWidth, int panelHeight)
	{
		int slotWidth = panelWidth/CONSTANTS.LootCols;
		int slotHeight = panelHeight/(5*CONSTANTS.LootRows);
		
		_drawerInventory.ItemsSubpanel(slotWidth, slotHeight, _lootList, CONSTANTS.LootCols, CONSTANTS.LootRows);
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

    #region UtilPanel

 
	#endregion
	
	#region Utlity
	
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

    public delegate void WriteToLogDelegate(string log);
    public delegate void BattleCleanupDelegate();

    private void WriteToLog(string log)
    {
        _lastLog = log;
        _logLength = StringLinesCount(_lastLog);
        //UpdateLogWindow();
    }

    private void BattleCleanup()
    {
        //print("Battle is over.");
        _battleStatus = BattleStatus.Victorious;
        LootListClear();
        LootListGenerate(Random.Range(0, CONSTANTS.LootSize));
        Destroy(_currentBattle);
    }

	#endregion
}
