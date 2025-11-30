# Etrian Odyssey: HD - Archipelago Client

## Copyrights and Trademark
**Etrian Odyssey HD** is a game that has been published on Steam in 2023 (and Switch) by **Sega** and developped by **Atlus**.<br/>
The game and all its assets are the property of its rightful owner (Atlus).
This work is an independent research modding project and is not official, endorsed, or for profit.
All rights to the game content are reserved by its owners. 

The repository does not contain any file that are parts of the original Game.
## Informations

This is a work in progress.<br/>
Require to downgrade EO: HD to the first version<br/>
[AP World](https://github.com/wolicodes/Archipelago/tree/eo1-hd)

## Archipelago Features

* Shopsanity
* Treasure Box randomizer

## Other Features
* Custom debug menu and native game debug menu access

## Hierarchy

* EOAP.Plugin - .NET Plugin for EO HD
* Dumps - some logs that I generated to understand parts of the game

## Licensing

There is no Licence at the the moment. <br/>
No one may reproduce, distribute or create derivative work

## Building

* Run Bepinex 6-x (ClrCore Runtime) Once, close EOHD
* Update Path for all references in .csproj if steam is installed elsewhere (probably)
  * This path C:\Games\Steam\steamapps\common\EOHD\BepInEx\interop\

## Testing

* Get Latest Release
* Copy Full zip in to game folder or Plugin zip into Bepinex/plugins
* Edit ap_configuration.json (game root folder) to set your server address and slot name

## Limitations
* Disregard other mods, should be used as a standalone
* Quicksave will also save world data so you might loose items
* Connect before loading save (will be fixed)
* No game inputs override found yet, so connection data is edited in json


## Credits

* Woli (world maintainer)
* Seph (insights/message system)