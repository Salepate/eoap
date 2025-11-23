# Etrian Odyssey: HD - Archipelago Client

## Informations

This is a work in progress.<br/>
Require to downgrade EO: HD to the first version<br/>
[AP World (Require AP 0.6.5)](https://github.com/wolicodes/Archipelago/tree/eo1-hd)

## Licensing

There is no Licence at the the moment. <br/>
No one may reproduce, distribute or create derivative work

## Building
* Run Bepinex 6-x (ClrCore Runtime) Once, close EOHD
* Update Path for all references in .csproj if steam is installed elsewhere (probably)
  * This path C:\Games\Steam\steamapps\common\EOHD\BepInEx\interop\
* Edit APUI.cs (change hostname and slotname)

## Testing

* Download a release OR
* Publish, copy content in bepinex plugins folder

## Hierarchy

* EOAP.Plugin - .NET Plugin for EO HD