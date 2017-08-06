# W1209 Data-Logging Thermostat

This project uses [STM8EF](https://github.com/TG9541/stm8ef) to turn an off-the-shelf [W1209][] into a data logging thermostat. 

This is work in progress. There is a [HACKADAY.IO project][HAD1] where progress is tracked. 

## STM8EF Base System

The code is based on the [STM8EF v2.2.13 binary release](https://github.com/TG9541/stm8ef/releases/tag/v2.2.13). 
A copy of the STM8EF library in the release is included (`mcu`, `lib`), and the W1209-FD.ihx binary in `out/W1209-FD` is used.

## Using this repository

* clone the repository
* install [stm8flash](https://github.com/vdudouyt/stm8flash)
* [connect a ST-LINK-V2 dongle to a W1209][W1209]
* run `make defaults` to wipe the stock firmware 
  * warning: there is no known public source for the stock firmware, and after erasing it there is no way back!
* run `make` to flash the STM8EF binary 
* start hacking

## Contributing

File an issue, post a comment in the [HACKADAY.IO project][HAD1], or contribute docs, code, or new use-cases and requirements.

[HAD1]: https://hackaday.io/project/26258-w1209-data-logging-thermostat
[W1209]: https://github.com/TG9541/stm8ef/wiki/Board-W1209

