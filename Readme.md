# Sailaway Grib Converter


This program converts Xygrib files downloaded from qtVLM for usage with Sailaway. It adds 3 hours to the grib using `wgrib2.exe`.


## Usage

- Download the *ZIP* file from Releases.
- Download [wgrib2](https://ftp.cpc.ncep.noaa.gov/wd51we/wgrib2/Windows_64/) (download all files and copy them to a directory of your choice).
- Create a *Desktop* shortcut (or *Autostart* shortcut) to `SailawayGribConverter.exe`.
- Open the shorcut properties and append `"<Path to qtVLM grib directory>" "<Full path to wgrib2.exe>"` to the *Target* field in the shortcut´s properties.
  - Example for Target: `"C:\Tools\SailawayGribConverter\SailawayGribConverter.exe" "C:\Program Files\qtVlm\grib" "C:\tools\wgrib\wgrib2.exe"`
- Start Sailaway Grib Converter from the shortcut **before** you start qtVLM.
- Select area and download **Xygrib** to a grib slot of your choice.
- As soon as the file is downloaded, it will be converted using `wgrib2.exe`.
- Reload the grib file in qtVLM (*Grib* - *Grib Slot X* - *Reload*).
- Exit the program by right-clicking on its tray icon if you don´t use it anymore.
