# W1209 Data-Logging Thermostat

This project uses [STM8EF](https://github.com/TG9541/stm8ef) to turn an off-the-shelf [W1209][] into a data logging thermostat. 

This is work in progress. There is a [HACKADAY.IO project][HAD1] where progress is tracked.

## Getting Started

For [programming the W1209 binary](https://github.com/TG9541/W1209/blob/master/out/W1209-FD/W1209-FD.ihx) please follow the instructions in the [STM8EF Wiki page for board W1209](
https://github.com/TG9541/stm8ef/wiki/Board-W1209#flashing-the-stm8ef-binary).

Interactive scripting through the serial console is supported once the base STM8EF system has been transferred. Please refer to the [instructions for getting a serial console](https://github.com/TG9541/stm8ef/wiki/Board-W1209#serial-communication-through-the-key-pins).

The recommended terminal emulator is [e4thcom](https://wiki.forth-ev.de/doku.php/en:projects:e4thcom): it supports line editing, and upload of source files with library #includes.

The `Makefile` uses ucSim to create an STM8 binary file that contains the full thermostat script, including the W1209-FD base image.

## Using this repository

* clone the repository
* install [stm8flash](https://github.com/vdudouyt/stm8flash)
* [connect a ST-LINK-V2 dongle to a W1209][W1209]
* run `make defaults` to wipe the stock firmware 
  * warning: there is no known public source for the stock firmware, and after erasing it there is no way back!
* run `make` to flash the STM8EF binary
* for interactive scripting install [e4thcom](
https://wiki.forth-ev.de/doku.php/en:projects:e4thcom)
* optionally install the development version of ucSim to take advantage of off-line image creation

## About the STM8EF Base System

The code is based on the [STM8EF v2.2.13 binary release](https://github.com/TG9541/stm8ef/releases/tag/v2.2.13). 
A copy of the STM8EF library in the release is included (`mcu`, `lib`), and the W1209-FD.ihx binary in `out/W1209-FD` is used.

Please refer to the [STM8EF Wiki](https://github.com/TG9541/stm8ef/wiki) for more information.

## Contributing

File an issue, post a comment in the [HACKADAY.IO project][HAD1], or contribute docs, code, or new use-cases and requirements.

And above all: please write about it in forums, blogs, or on Twitter (please use #W1209 and #STM8EF hashtags).

[HAD1]: https://hackaday.io/project/26258-w1209-data-logging-thermostat
[W1209]: https://github.com/TG9541/stm8ef/wiki/Board-W1209

