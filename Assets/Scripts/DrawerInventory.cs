using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class DrawerInventory : MonoBehaviour
{
    private Texture[] _itemTextures = new Texture[(int)ItemType.LENGTH];
    private Texture2D _invRagdoll;// = Resources.Load("InvBody") as Texture
    
    //private ItemManager _itemUnderCursorUnclicked;

    private ControlsManager _controlsManager;
    public void SetControlsManager(ControlsManager controlsManager)
    {
        _controlsManager = controlsManager;
    }

    void Start()
    {
        //Initialize item icons
        for (int i = 0; i < (int)ItemType.LENGTH; ++i)
        {
            _itemTextures[i] = Resources.Load(((ItemType)i).ToString() + " Icon") as Texture;
        }
        _invRagdoll = Resources.Load("InvBody") as Texture2D;
    }

    public void ItemsSubpanel(int slotWidth, int slotHeight, ItemManager[] selectedInventory, int cols, int rows)
    {
        // Base display & react loop
        for (int i = 0; i < cols; ++i)
        {
            for (int j = 0; j < rows; ++j)
            {
                int slotnumber = i * rows + j;
                Rect slotRect = new Rect(i * slotWidth, j * slotHeight, slotWidth, slotHeight);
                //Event e = Event.current;

                ItemsManageSlot(slotRect, ref selectedInventory[slotnumber]);
            }
        }

        // Mouse Hover detection loop
        for (int i = 0; i < cols; ++i)
        {
            for (int j = 0; j < rows; ++j)
            {
                int slotnumber = i * rows + j;

                if (selectedInventory[slotnumber] != null)
                {
                    Rect slotRect = new Rect(i * slotWidth, j * slotHeight, slotWidth, slotHeight);
                    Event e = Event.current;

                    //MOUSEHOVER TEST
                    if (slotRect.Contains(e.mousePosition))
                    {
                        _controlsManager.ItemUnderCursorUnclicked = selectedInventory[slotnumber];
                        //DisplayItemStats(selectedInventory[slotnumber], e.mousePosition);
                        //GUI.Label(new Rect(slotRect.xMax, slotRect.yMin, 100, 100), 
                        //	selectedInventory[slotnumber].ToString());	
                    }
                }
            }
        }
    }

    public void UtilPanelInventory(int panelWidth, int panelHeight, Hero character)
    {
        if (character == null)
        {
            return;
        }

        //Hero character = _characters[_selectedCharacter];

        // EQUIPMENT

        ItemManager[] selectedEquipment = character.Equipment;

        int slotWidth = panelWidth / CONSTANTS.InvCharCols;
        int slotHeight = panelHeight / (2 * CONSTANTS.InvCharRows);

        GUI.Label(new Rect(panelWidth / 6, 0, panelWidth, panelHeight / 2), _invRagdoll);

        Rect equipmentHead = new Rect(panelWidth / 2, 1 * panelHeight / 16, slotWidth, slotHeight);
        ItemsManageSlot(equipmentHead,
            ref selectedEquipment[(int)EquipmentSlotType.Head], EquipmentSlotType.Head);
        Rect equipmentChest = new Rect(panelWidth / 2, 3 * panelHeight / 16, slotWidth, slotHeight);
        ItemsManageSlot(equipmentChest,
            ref selectedEquipment[(int)EquipmentSlotType.Chest], EquipmentSlotType.Chest);
        Rect equipmentCloak = new Rect(3 * panelWidth / 4, 2 * panelHeight / 16, slotWidth, slotHeight);
        ItemsManageSlot(equipmentCloak,
            ref selectedEquipment[(int)EquipmentSlotType.Cloak], EquipmentSlotType.Cloak);
        Rect equipmentMainHand = new Rect(panelWidth / 4, 3 * panelHeight / 16, slotWidth, slotHeight);
        ItemsManageSlot(equipmentMainHand,
            ref selectedEquipment[(int)EquipmentSlotType.MainHand], EquipmentSlotType.MainHand);
        Rect equipmentOffHand = new Rect(3 * panelWidth / 4, 4 * panelHeight / 16, slotWidth, slotHeight);
        ItemsManageSlot(equipmentOffHand,
            ref selectedEquipment[(int)EquipmentSlotType.OffHand], EquipmentSlotType.OffHand);
        Rect equipmentGloves = new Rect(panelWidth / 4, 5 * panelHeight / 16, slotWidth, slotHeight);
        ItemsManageSlot(equipmentGloves,
            ref selectedEquipment[(int)EquipmentSlotType.Gloves], EquipmentSlotType.Gloves);
        Rect equipmentBoots = new Rect(panelWidth / 2, 6 * panelHeight / 16, slotWidth, slotHeight);
        ItemsManageSlot(equipmentBoots,
            ref selectedEquipment[(int)EquipmentSlotType.Boots], EquipmentSlotType.Boots);

        // INVENTORY

        //ItemManager[] selectedInventory = character.Inventory;

        GUI.BeginGroup(new Rect(0, panelHeight / 2, panelWidth, panelHeight / 2));
        ItemsSubpanel(slotWidth, slotHeight, character.Inventory, CONSTANTS.InvCharCols, CONSTANTS.InvCharRows);
        GUI.EndGroup();
    }

    private void ItemsManageSlot(Rect slotRect, ref ItemManager slotItem, EquipmentSlotType? slotType = null)
    {
        if (slotItem == null)
        {
            if (GUI.Button(slotRect, "SLOT") && Event.current.button == 0)
            {
                if (_controlsManager.ItemUnderCursor != null &&
                    (slotType == null || EquipmentSlotCheckItemCompatability(slotType.Value, _controlsManager.ItemUnderCursor.MyItemClass)))
                {
                    slotItem = _controlsManager.ItemUnderCursor;
                    _controlsManager.ItemUnderCursor = null;
                }
                else
                {
                    if (slotType == null)
                    {
                        //slotItem = ItemsContainer.AddComponent<ItemManager>();
                    }
                }
            }
        }
        else
        {
            if (GUI.Button(slotRect, _itemTextures[(int)slotItem.MyItemType]) && Event.current.button == 0)
            {
                if (_controlsManager.ItemUnderCursor != null &&
                    (slotType == null || EquipmentSlotCheckItemCompatability(slotType.Value, _controlsManager.ItemUnderCursor.MyItemClass)))
                {
                    ItemManager tmp = _controlsManager.ItemUnderCursor;
                    _controlsManager.ItemUnderCursor = slotItem;
                    slotItem = tmp;
                }
                else
                {
                    _controlsManager.ItemUnderCursor = slotItem;
                    slotItem = null;
                }
            }
        }
    }

    // returns true if item fits slot; false otherwise
    private bool EquipmentSlotCheckItemCompatability(EquipmentSlotType slotType, ItemClass itemClass)
    {
        //returnVal = false;
        switch (slotType)
        {
            // Helmet
            case EquipmentSlotType.Head:
                if (itemClass == ItemClass.Helmet)
                {
                    return true;
                }
                break;
            // Cloak
            case EquipmentSlotType.Cloak:
                if (itemClass == ItemClass.Cloak)
                {
                    return true;
                }
                break;
            // Gloves			
            case EquipmentSlotType.Gloves:
                if (itemClass == ItemClass.Gloves)
                {
                    return true;
                }
                break;
            // Armor 			
            case EquipmentSlotType.Chest:
                if (itemClass == ItemClass.Armor)
                {
                    return true;
                }
                break;
            // Main-hand (1h or 2h weapon)		
            case EquipmentSlotType.MainHand:
                if (itemClass == ItemClass.Melee1H)
                {
                    return true;
                }
                else if ((itemClass == ItemClass.Melee2H || itemClass == ItemClass.Ranged2H))
                //&& _equippedItems[5] == null)
                {
                    return true;
                }
                break;
            // off-hand (shielf or second weapon)		
            case EquipmentSlotType.OffHand:
                if (itemClass == ItemClass.OffHand || itemClass == ItemClass.Melee1H)
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
                if (itemClass == ItemClass.Boots)
                {
                    return true;
                }
                break;
        }

        return false;
    }

    public void DrawItemAtMouseCursor(float x, float y)
    {
        GUI.Label(new Rect(x, y, 100, 100),
                _itemTextures[(int)_controlsManager.ItemUnderCursor.MyItemType]);
    }
}

