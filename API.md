# Essential Classes

## Action Menu

### Desc
Handle battle inputs

### Notes
Did not have a chance to use it, but looks like it work

## Camp.CampMenuController
### Desc
Pause menu controller

### Methods
    * SetDecideKeepList -- invoked once each time the pause menu is open. But not sure what it does, probably a list of items to discard

## Master.Event
### Desc
    Seems to control events/quest/missions operations

## Master.EventFlagTbl
### Desc
    Contains all save flags and associated ops
### Methods
    GetEventFlag() - check a specific game flag state
    SetEventFlags - toggle a specific game flag

## Master.MasterManager
### Desc
    Provide essential game data like monster data

## Methods
    GetMasterEnemyData() - retrieve monster data

## Master.MasterTbbData.ENEMY_DATA
### Desc
    Store monster data, seems reallocated every time
### Fields
    FBBBOFLFELG - atk
    FDFEGNNAEJL - xp 
    GCIBMJNJMFD	- def
    JLJFLNCKHIB	- max hp

## DungeonTreasureState

### Desc
Dungeon treasure box operations

### Notes
Monobehaviour

### Methods
    * GetItemFunc() - retrieve the associated reward
    * TreasureOpenCheckFunc() - check if the tbox can be opened or is already opened

## DungeonUtil
### Methods
    * SetDiscoveryEnemy() - invoked for each defeated enemy (triggered at result screen)


## Master.Event
### Desc
update quest and mission status

### Methods 
    * SetEventMissionComplete() - invoked after the "COMPLETED" animation is done
## GoldItem

### Desc
Party inventory management 

### Notes
Lots of static and working methods

### Methods
    * AddPartyItem() - Add item to inventory
    * PayGold() - consume gold


### Non Working
    * BuyItem() - seems ignored

## ItemMain
### Desc
Retrieve static/persistent data regarding a specific Item

### Methods
    * IsSoldOutEnable() - invoked a lot

## ShopBuyMenu

### Desc
Perform shop operations (Apothecary+Main Shop)

### Notes
Stable, but lots of wrapped data (like the selected item)

### Methods
    * Buy() - add item + show popup i think
    * GetTabDataList() - list of items to be displayed when browsing a category
    * GetEquippableItemsCount() - how many slots can be used to equip the currently selected item

## TextLabelDataManager

### Desc
Singleton used to get localization keys

### Fields
    * BMIFAMFAEKK - Instance
### Methods
    * GetLabelItemName() - Return proper localization key to use with next op
    * TryGetText() - get localized string from key

## TitleMenu

### Desc
Monobehaviour that handles all title menu logic

### Methods
    * Open() -- invoked at start (during blackscreen)

## Town.ShopItemListController.Data

### Desc
contains item data used for selling/buying

### Fields
    GOGABMGPOND - Item ID
    ELLNBOJNCPG - Price (as string)