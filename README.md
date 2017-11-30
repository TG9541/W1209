# W1209 Data-Logging Thermostat

[![Travis-CI](https://travis-ci.org/TG9541/W1209.svg?branch=master)](https://travis-ci.org/TG9541/W1209)

This project uses [STM8EF](https://github.com/TG9541/stm8ef) to turn an off-the-shelf [W1209][] into a data logging thermostat. It provides [source code](https://github.com/TG9541/W1209), a ready-to-use [firmware](https://github.com/TG9541/W1209/releases), and [documentation](https://github.com/TG9541/W1209/wiki).

Features are:

* heating thermostat, e.g. for building a chicken egg incubator
* basic sensor failure detection
* parameters for set-point, hysteresis, and trip-delay
* easy to use parameters menu (no need to search for a manual!)
* temperature logger with 0.1h to 10h intervall, and a 288 entry ring-buffer
* logger access throug a serial console
* fully programmable in Forth, even while it's running!

Although it's feature-complete, this is work in progress:

* Beta-grade "bleeding edge"
* not even an "endurance run" yet

Planned features:

* Improved logger with min/max temperature, and heating duty cycle
* Field-bus feature for thermostats
* more fail-safe features (parameter integrity, maybe a buzzer)

Please check out the [HACKADAY.IO project][HAD1]!

## Getting Started

The GitHub releases page contains a binary file that can be directly programmed to a W1209 using a ST-Link compatible programmer. The W1209 works stand-alone, and the parameters can be set using the board keys (`set`, `+`, `-`).

For programming the W1209 using a cheap ST-Link adapter is recommended - please refer to the [STM8 eForth Wiki](https://github.com/TG9541/stm8ef/wiki/STM8S-Programming#flashing-the-stm8) for instructions.

W1209|ST-Link programmer|TTL-Serial-Interface
-|-|-
![W1209](https://user-images.githubusercontent.com/5466977/33417013-d2b29dec-d59f-11e7-8187-e608e856fe16.png)|![Programmer](https://ae01.alicdn.com/kf/HTB1QVvYRXXXXXa5XFXXq6xXFXXXP/ST-Link-V2-stlink-mini-STM8STM32-STLINK-simulator-download-programming-With-Cover.jpg_220x220.jpg)|![TTL-Serial](https://ae01.alicdn.com/kf/HTB1x__9OFXXXXc7XVXXq6xXFXXX6/-Free-Shipping-CH340-module-USB-to-TTL-CH340G-upgrade-download-a-small-wire-brush-plate.jpg_220x220.jpg)

After programming, set the default parameter values, by holding the keys `+` and `-` until the text `rES` appears in the display (about 4s). Pressing the `set` key leads to the parameter menu (the menu exits when no key is pressed in 10s). 

The parameters are as follows:

Display|Range|Default|Unit|Description
-|-|-|-|-
`SEt.`| 10.0 - 80.0 |37.5| [°C]| Heating thermostat set point (switch off above)
`LoG.`| 0.0 - 10.0 | 10.0 |[h]| Logger interval in hours
`dEL.`| 0.0 - 60.0 | 0.0 | [s] | thermostat heating trip delay
`Cor.`| -2.0 - 2.0 | 0.0 | [°C] | thermometer offset (for corrections around desired set-point)
`hYS.`| 0.1 - 2.0 | 0.5 | [°C] | thermostat hysteresis (difference between the lower and the upper trip points)

The data log can be accessed through the Forth console with the command `L.dump`. The log can be wiped with the command `L.wipe`. To use the Forth console, connect a serial interface adapter to the `+` and `-` key pins.

## Working with the Code in this Repository

Clone this repository, an run `make depend` for dependency resolution. This will download an STM8 eForth binary, and add required folders, files, and symlinks.

For [programming the W1209 binary](https://github.com/TG9541/W1209/blob/master/out/W1209-FD/W1209-FD.ihx) please follow the instructions in the [STM8EF Wiki page for board W1209](
https://github.com/TG9541/stm8ef/wiki/Board-W1209#flashing-the-stm8ef-binary) (if `stm8flash` is installed just run `make flash`).

Interactive scripting through the serial console is supported by the STM8 eForth base binary. Please refer to the [instructions for getting a serial console](https://github.com/TG9541/stm8ef/wiki/Board-W1209#serial-communication-through-the-key-pins).

The recommended terminal emulator is [e4thcom](https://wiki.forth-ev.de/doku.php/en:projects:e4thcom): it supports line editing, and upload of source files with `#include`, and using a library with `#require`. Type `#i main.fs` to load the complete source code.

For Continuous Integration, `make simload` uses ucSim to create an STM8 binary file that contains the full thermostat script, including the W1209-FD base image. The Docker image [tg9541/docker-sdcc](https://hub.docker.com/r/tg9541/docker-sdcc/) contains tool dependencies for Continuous Integration (refer to `.travis.yml`).

## Using this repository

* clone the repository
* install [stm8flash](https://github.com/vdudouyt/stm8flash)
* [connect a ST-LINK-V2 dongle to a W1209][W1209]
* run `make defaults` to wipe the stock firmware
  * warning: there is no known public source for the stock firmware, and after erasing it there is no way back!
* run `make` to flash the STM8EF binary
* for interactive scripting install [e4thcom](
https://wiki.forth-ev.de/doku.php/en:projects:e4thcom)
* optionally install the development version of ucSim (or use the Docker image) to take advantage of off-line image creation

## About the STM8EF Base System

The code is based on the [STM8EF binary release](https://github.com/TG9541/stm8ef/releases). The Makefile automatically retrieves, and resolves the version specified by `STM8EF_VER`.

Please refer to the [STM8EF Wiki](https://github.com/TG9541/stm8ef/wiki) for more information.

## Contributing

File an issue, post a comment in the [HACKADAY.IO project][HAD1], or contribute docs, code, or new use-cases and requirements.

And above all: please write about it in forums, blogs, or on Twitter (please use #W1209 and #STM8EF hashtags).

[HAD1]: https://hackaday.io/project/26258-w1209-data-logging-thermostat
[W1209]: https://github.com/TG9541/stm8ef/wiki/Board-W1209
