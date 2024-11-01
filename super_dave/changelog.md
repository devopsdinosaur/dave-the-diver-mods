﻿# 0.0.12

## Bug Fixes
- Fixed all existing features to work with latest game version (v1.0.3.1530)


# 0.0.11

## Features
- Split "Infinite Crab Traps" into two new options: Infinite and Auto Drop crab traps.  Now users can have infinite manual crab traps and/or auto-dropping traps.
- Added 'auto-pickup-items.txt' file (created after first run in BepInEx/config) to specify which items should be automatically picked up.  Open the file or the mod Nexus page for editing details.
- Auto-pickup will now open chests.

## Minor Changes
- Toxic Aura: Sleep now defaults to true.
- Adjusted default aura sizes up to 6m.

## Bug Fixes
- Toxic aura will now pulse immediately when hotkey is pressed.
- Fixed a glitch where Dave would auto-drop traps on non-existent rocks with superhuman speed (thanks to mishrasumitranjan for finding this issue).

## Known Issues
- Toxic Aura will hit ally fish (ex. baby whale).  Currently need to temporarily disable the aura (default: Ctrl+Backspace) when getting near the fish.

## In Progress
- Allow toxic aura to differentiate between ally and non-ally fish.
- Add auto-pickup hittables (ore/seaweed/etc).
- Ability to set all equipment to anything when starting dive.


# 0.0.10
Minor fix for bug with hotkeys in 0.0.9.

# 0.0.9

## Features
- Added option to disable item info pop-ups when diving.
- Auto-pickup and Toxic auras now have options to set pulse frequency (to reduce lag).

## Bug Fixes
- Swim speed buff no longer affects fish, just Dave.
- Auto crab traps now correctly use lvl 9 bait for better catches.

## Known Issues
- Toxic Aura will hit ally fish (ex. baby whale).  Currently need to temporarily disable the aura (default: Ctrl+Backspace) when getting near the fish.

## In Progress
- Allow toxic aura to differentiate between ally and non-ally fish.
- Add auto-pickup of chests and hittables (ore/seaweed/etc).
- Ability to set all equipment to anything when starting dive.


# 0.0.8

## Features
- Added auto-pickup for fish and most ground items (including pots [chests coming soon]).

## Bug Fixes
- Infinite drones now working as expected.
- Infinite crab traps option now auto-places traps on rocks.

## Known Issues
- Toxic Aura will hit ally fish (ex. baby whale).  Currently need to temporarily disable the aura (default: Ctrl+Backspace) when getting near the fish.

## In Progress
- Allow toxic aura to differentiate between ally and non-ally fish.
- Add auto-pickup of chests and hittables (ore/seaweed/etc).
- Ability to set all equipment to anything when starting dive.


# 0.0.7

## Features
- Added infinite crab traps option.
- Added infinite salvage drones option.
- Added droneless large pickup option.
- Added hotkeys to toggle off/on toxic aura and to switch kill/sleep.

## Bug Fixes
- Sleep Aura no longer hits already sleeping fish.
- Sushi staff cooking speed now correctly speeds up (not slows down) cooking.
- Boat walk speed fixed to work with latest game version.

## In Progress
- Ability to set all equipment to anything when starting dive.