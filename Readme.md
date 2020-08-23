# Sailaway Grib Converter


This program converts Xygrib files downloaded from [qtVLM](https://www.meltemus.com/index.php/en/) for usage with the sailing simulation game [Sailaway](https://sailaway.world/). 

It shifts the grib 3 hours to the future using `wgrib2.exe` to better match the Sailaway in-game weather conditions.

*Sailaway Grib Converter* runs in the background without any user interaction.

## Installation

- Download `SailawayGribConverter.zip` from [Releases](https://github.com/elpatron68/SailawayGribConverter/releases/latest) and extract it to a directory of your choice.
- Create a *Desktop* shortcut (or *Autostart* shortcut) to `SailawayGribConverter.exe`.
- Open the shortcut properties and append `[1 blank]"<Path to qtVLM grib directory>"[1 blank]"<Full path to wgrib2.exe>"` to the *Target* field in the shortcut´s properties.
  - Example for Target: `"C:\Tools\SailawayGribConverter\SailawayGribConverter.exe" "C:\Program Files\qtVlm\grib" "C:\Tools\SailawayGribConverter\wgrib2.exe"`

## Usage

### General

- Start Sailaway Grib Converter from the shortcut **before** you start qtVLM.
- Select area and download **Xygrib** to a grib slot of your choice.
- As soon as the file is downloaded, it will be converted using `wgrib2.exe`.
- Reload the grib file in qtVLM (*Grib* - *Grib Slot X* - *Reload*).
- Exit the program by right-clicking on its tray icon if you don´t use it anymore.

### Select Time Shift

Right click on the tray icon and select `Time Shift` - `+x hour(s)` to adjust the time span the grib is moved. 
The selected value will be permanenty saved. Default is `+3 hours`.

### Pause

Right click on the tray icon and select `Pause` if you don´t want *SailawayGribConverter* to automatically convert downloaded grib files.

## How it works

*Sailaway Grib Converter* uses a *File System Watcher* to get noticed about any newly created files with the extension `*.grb2` in the directory where qtVLM saves downloaded gribs.

Once a grib file is downloaded, it will immediately be time-shifted to +3 hours using `wgrib2.exe`. The new file overwrites the former (unshifted) file, so the user has to reload the Grib in qtVLM (or restart qtVLM) to let the changes take effect.

Use qtVLMs *Grid Info* (`Crtl-I`) to compare the weather conditions from the game with the ones from the grid (works best if you use [Sailaway to NMEA](https://github.com/expilu/sailaway-api-to-nmea))