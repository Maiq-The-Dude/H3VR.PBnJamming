# PB 'n' Jamming 

A WIP firearm malfunction mod inspired by [Meatyceiver 2](https://github.com/potatoes1286/H3VR.Meatyceiver2). This mod builds onto Meatyceiver's core idea and adds modular firearm tags used to determine failure rates at a granular level.

## Features
- A supported firearm's malfunction rate is determined by the combination of the following firearm tags:
  - Firearm ItemID
  - Action
  - Era
  - Magazine ItemID
  - Round Type
  - Round Class
- Firearm tags are modular and easy to configure. Uses [Deli](https://github.com/Deli-Counter/Deli) to easily load corresponding JSON files. See the [Configuration](https://github.com/Maiq-The-Dude/PBnJamming#configuration) section below for more information.
- Firearms currently supported:
  - BoltActionRifle
  - BreakActionWeapon
  - ClosedBolt
  - Handgun
  - LeverActionFirearm
  - OpenBolt
  - Revolver
  - RevolvingShotgun
  - RollingBlock
  - TubeFedShotgun  
  
## Firearm Malfunction Configuration
- Default firearm tags are configured under the [`failures`](https://github.com/Maiq-The-Dude/PBnJamming/tree/main/mod/failures) folder. 
- Each firearm tag currently supports five malfunction properties, each containing a float used as its weighted chance value.
- Malfunction properties:
  - fire
  - feed
  - extract
  - lockOpen
  - discharge
  
## User Configs
Additional user configs can be found under `mods\configs\pbnjamming.cfg`

## Adding Custom Content Support
Adding and configuring custom content like guns or magazines is easy. [Here is an example repo](https://github.com/Maiq-The-Dude/PBnJamming.ExampleAddon) containing a mod to add two custom guns and one custom magazine.

## Installation
No releases available yet. To install you must build the project yourself and place into `mods` folder as this is a Deli mod.

## Uninstallation
To uninstall, you only need to delete the `mods\PBnJamming.deli` file.
