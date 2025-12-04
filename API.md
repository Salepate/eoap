# Essential Classes

## Action Menu

### Aim
Handle battle inputs

### Notes
Did not have a chance to use it, but looks like it work

## Camp.CampMenuController
### Aim
Pause menu controller

### Methods
    * SetDecideKeepList -- invoked once each time the pause menu is open. But not sure what it does, probably a list of items to discard

## Master.Event
### Aim
    Seems to control events/quest/missions operations

## Master.EventFlagTbl
### Aim
    Contains all save flags and associated ops
### Methods
    GetEventFlag() - check a specific game flag state
    SetEventFlags - toggle a specific game flag

## Master.MasterManager
### Aim
    Provide essential game data like monster data

## Methods
    GetMasterEnemyData() - retrieve monster data

## Master.MasterTbbData.ENEMY_DATA
### Aim
    Store monster data, seems reallocated every time
### Fields
    FBBBOFLFELG - atk
    FDFEGNNAEJL - xp 
    GCIBMJNJMFD	- def
    JLJFLNCKHIB	- max hp

## DungeonTreasureState

### Aim
Dungeon treasure box operations

### Notes
Monobehaviour

### Methods
    * GetItemFunc() - retrieve the associated reward
    * TreasureOpenCheckFunc() - check if the tbox can be opened or is already opened

## Master.Event
### Aim
update quest and mission status

### Methods 
    * SetEventMissionComplete -- invoked after the "COMPLETED" animation is done
## GoldItem

### Aim
Party inventory management 

### Notes
Lots of static and working methods

### Methods
    * AddPartyItem() - Add item to inventory
    * PayGold() - consume gold


### Non Working
    * BuyItem() - seems ignored

## ItemMain
### Aim
Retrieve static/persistent data regarding a specific Item

### Methods
    * IsSoldOutEnable() - invoked a lot

## ShopBuyMenu

### Aim
Perform shop operations (Apothecary+Main Shop)

### Notes
Stable, but lots of wrapped data (like the selected item)

### Methods
    * Buy() - add item + show popup i think
    * GetTabDataList() - list of items to be displayed when browsing a category
    * GetEquippableItemsCount() - how many slots can be used to equip the currently selected item

## TextLabelDataManager

### Aim
Singleton used to get localization keys

### Fields
    * BMIFAMFAEKK - Instance
### Methods
    * GetLabelItemName() - Return proper localization key to use with next op
    * TryGetText() - get localized string from key

## TitleMenu

### Aim
Monobehaviour that handles all title menu logic

### Methods
    * Open() -- invoked at start (during blackscreen)

## Town.ShopItemListController.Data

### Aim
contains item data used for selling/buying

### Fields
    GOGABMGPOND - Item ID
    ELLNBOJNCPG - Price (as string)