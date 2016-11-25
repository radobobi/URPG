using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class DrawerUtils : MonoBehaviour
{
    private UtilPanelType _utilPanel = UtilPanelType.CharSheet;
    public UtilPanelType UtilPanel
    {
        get { return _utilPanel; }
    }

    private ControlsManager _controlsManager;
    public void SetControlsManager(ControlsManager controlsManager)
    {
        _controlsManager = controlsManager;
    }

    // Tactics Windows
    private GUIContent[] _tacticsTargetSelectionList;
    private ComboBox _tacticsTargetSelectionControl;// = new ComboBox();
    
    Rect _tacticsTargetSelectionBox = new Rect(0, 0, 150, 30);

    private GUIContent[] _tacticsBasePositionList;
    private ComboBox _tacticsBasePositionControl;// = new ComboBox();
    public ComboBox TacticsBasePositionControl
    {
        get
        {
            return _tacticsBasePositionControl;
        }
    }
    Rect _tacticsBasePositionBox = new Rect(0, 50, 150, 30);

    private GUIContent[] _skills_1_List;
    // Skills Window
    private ComboBox _skills_1_Control;// = new ComboBox();
    //private GUIStyle _listStyle = new GUIStyle();
    Rect _skills_1_Box = new Rect(0, 0, 150, 30);
    private GUIStyle _listStyle = new GUIStyle();

    void Start()
    {
        // tactics menus init
        _tacticsTargetSelectionList = new GUIContent[(int)TacticsTargetSelection.LENGTH];
        for (int i = 0; i < (int)TacticsTargetSelection.LENGTH; ++i)
        {
            _tacticsTargetSelectionList[i] = new GUIContent(((TacticsTargetSelection)i).ToString());
        }
        _tacticsTargetSelectionControl = new ComboBox(
            _tacticsTargetSelectionBox, new GUIContent("Pick Tactic"),
            _tacticsTargetSelectionList, "button", "box", _listStyle);

        _tacticsBasePositionList = new GUIContent[(int)TacticsBasePosition.LENGTH];
        for (int i = 0; i < (int)TacticsBasePosition.LENGTH; ++i)
        {
            _tacticsBasePositionList[i] = new GUIContent(((TacticsBasePosition)i).ToString());
        }
        _tacticsBasePositionControl = new ComboBox(
            _tacticsBasePositionBox, new GUIContent("Pick Tactic"),
            _tacticsBasePositionList, "button", "box", _listStyle);

        _skills_1_List = new GUIContent[(int)Skill.LENGTH];
        for (int i = 0; i < (int)Skill.LENGTH; ++i)
        {
            _skills_1_List[i] = new GUIContent(((Skill)i).ToString());
        }
        _skills_1_Control = new ComboBox(
            _skills_1_Box, new GUIContent("Pick Skill"),
            _skills_1_List, "button", "box", _listStyle);

        _listStyle.normal.textColor = Color.white;
        _listStyle.onHover.background =
        _listStyle.hover.background = new Texture2D(2, 2);
        _listStyle.padding.left =
        _listStyle.padding.right =
        _listStyle.padding.top =
        _listStyle.padding.bottom = 4;
    }

    public void UtilPanelTactics(int panelWidth, int panelHeight)
    {
        if (_controlsManager.SelectedCharacter == -1)
        {
            return;
        }

        Hero myChar = _controlsManager.Characters[_controlsManager.SelectedCharacter];

        myChar.TargetSelection = (TacticsTargetSelection) _tacticsBasePositionControl.Show();
        GUI.Label(new Rect(150, 0, panelWidth - 150, 30),
            "Current Target Selection: " + myChar.TargetSelection.ToString());

        myChar.BasePosition = (TacticsBasePosition) _tacticsBasePositionControl.Show();
        GUI.Label(new Rect(150, 50, panelWidth - 150, 30),
            "Current Base Position: " + myChar.BasePosition.ToString());   
    }

    public void UtilPanelCharSheet(int panelWidth, int panelHeight)
    {
        if (_controlsManager.SelectedCharacter != -1)
        {
            GUI.Label(new Rect(0, 15, panelWidth / 2, 50), "Name: " + _controlsManager.Characters[_controlsManager.SelectedCharacter].MyName);
            GUI.Label(new Rect(0, 30, panelWidth / 2, 50),
                "Class: " + ((HeroClassType)_controlsManager.Characters[_controlsManager.SelectedCharacter].HeroClass).ToString());
            GUI.Label(new Rect(0, 45, panelWidth / 2, 50), "Level: " + _controlsManager.Characters[_controlsManager.SelectedCharacter].Level);
            GUI.Label(new Rect(0, 60, panelWidth / 2, 50), "HP: "
                + _controlsManager.Characters[_controlsManager.SelectedCharacter].CurrentHP + "/" + _controlsManager.Characters[_controlsManager.SelectedCharacter].MaxHP);
            int skip = 15;
            int start = 90;
            for (int i = 0; i < (int)MainStatType.LENGTH; ++i)
            {
                int baseStat = _controlsManager.Characters[_controlsManager.SelectedCharacter].GetStatBase((MainStatType)i);
                int activeStat = _controlsManager.Characters[_controlsManager.SelectedCharacter].GetStatActive((MainStatType)i);

                GUI.Label(new Rect(0, start + i * skip, panelWidth / 2, 50),
                    ((MainStatType)i).ToString() + ": " + activeStat + "(" + baseStat + ")");
            }
            start += (skip * ((int)MainStatType.LENGTH) + 10);
            for (int i = 0; i < (int)SecondaryStatType.LENGTH; ++i)
            {
                double secondaryStat = _controlsManager.Characters[_controlsManager.SelectedCharacter].GetStatSecondary((SecondaryStatType)i);
                GUI.Label(new Rect(0, start + i * skip, panelWidth / 2, 50),
                    ((SecondaryStatType)i).ToString() + ": " + secondaryStat);
            }
        }
    }

    public void UtilPanelSkills(int panelWidth, int panelHeight)
    {
        if (_controlsManager.SelectedCharacter == -1)
        {
            return;
        }

        Hero myChar = _controlsManager.Characters[_controlsManager.SelectedCharacter];

        myChar.Skill_1 = (Skill)_skills_1_Control.Show();
        GUI.Label(new Rect(150, 0, panelWidth - 150, 30),
            "Current Skill: " + myChar.Skill_1.ToString());
    }

    public void UtilPanelButtons(int panelWidth, int panelHeight)
    {
        // Buttons
        if (GUI.Button(new Rect(0, panelHeight / 20, panelWidth / 5, panelHeight / 20), "CharSheet"))
        {
            if (_utilPanel == UtilPanelType.CharSheet)
            {
                _utilPanel = UtilPanelType.CharSheet;
            }
            else
            {
                _utilPanel = UtilPanelType.CharSheet;
            }
        }
        if (GUI.Button(new Rect(panelWidth / 5, panelHeight / 20, panelWidth / 5, panelHeight / 20), "Inventory"))
        {
            if (_utilPanel == UtilPanelType.Inventory)
            {
                _utilPanel = UtilPanelType.CharSheet;
            }
            else
            {
                _utilPanel = UtilPanelType.Inventory;
            }
        }
        if (GUI.Button(new Rect(2 * panelWidth / 5, panelHeight / 20, panelWidth / 5, panelHeight / 20), "Skills"))
        {
            if (_utilPanel == UtilPanelType.Skills)
            {
                _utilPanel = UtilPanelType.CharSheet;
            }
            else
            {
                _utilPanel = UtilPanelType.Skills;
            }
        }
        if (GUI.Button(new Rect(3 * panelWidth / 5, panelHeight / 20, panelWidth / 5, panelHeight / 20), "Tactics"))
        {
            if (_utilPanel == UtilPanelType.Tactics)
            {
                _utilPanel = UtilPanelType.CharSheet;
            }
            else
            {
                _utilPanel = UtilPanelType.Tactics;
            }
        }
        if (GUI.Button(new Rect(4 * panelWidth / 5, panelHeight / 20, panelWidth / 5, panelHeight / 20), "Statistics"))
        {
            if (_utilPanel == UtilPanelType.Statistics)
            {
                _utilPanel = UtilPanelType.CharSheet;
            }
            else
            {
                _utilPanel = UtilPanelType.Statistics;
            }
        }
    }

    public void CharacterPanelButtons(int i, int panelWidth, int panelHeight)
    {
        int horizontalCount = 1;
        int verticalCount = 6;
        GUI.BeginGroup(new Rect((i % horizontalCount) * (panelWidth / horizontalCount),
            (i % verticalCount) * (panelHeight / verticalCount), panelWidth / horizontalCount, panelHeight / verticalCount));
        if (GUI.Button(new Rect(0, 0, panelWidth / horizontalCount, panelHeight / verticalCount), "CHAR" + i))
        {
            if (_controlsManager.ItemUnderCursor != null)
            {
                if (_controlsManager.Characters[i].TryToPlaceItemInInventory(_controlsManager.ItemUnderCursor))
                {
                    _controlsManager.ItemUnderCursor = null;
                }
            }
            else
            {
                if (_controlsManager.SelectedCharacter == i)
                {
                    _controlsManager.SelectedCharacter = -1;
                }
                else
                {
                    _controlsManager.SelectedCharacter = i;

                    // skills clear boxes
                    _skills_1_Control.SelectedItemIndex = (int)_controlsManager.Characters[i].Skill_1;

                    // tactics clear boxes
                    _tacticsTargetSelectionControl.SelectedItemIndex = (int)_controlsManager.Characters[i].TargetSelection;
                    _tacticsBasePositionControl.SelectedItemIndex = (int)_controlsManager.Characters[i].BasePosition;
                }
            }
        }
        GUI.EndGroup();
    }
}