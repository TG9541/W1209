\ W1209 basic data logging thermostat
\ © 2017 TG9541, refer to https://github.com/TG9541/W1209/blob/master/LICENSE
\ Note: not yet tested with real eggs :-)

NVM

#require MARKER

RAM

  : TARGET NVM ;

  $4000 CONSTANT EE.SET    \ EEPROM cell "control set-point"
  $4002 CONSTANT EE.HYS    \ EEPROM cell "control hysteresis"
  $4004 CONSTANT EE.DEL    \ EEPROM cell "control delay"
  $4006 CONSTANT EE.COR    \ EEPROM cell "offset correction"
  $4008 CONSTANT EE.LOG    \ EEPROM cell "logger interval"

  $8000 CONSTANT DEFAULT   \ Default value indicator (-32768)

TARGET

#require .0

  : init ( -- ) \ chained init - starting point
    6 ADC!      \ W1209: use ADC channel 6
  ;

#include measure.fs
#include menu.fs
#include logger.fs
#include control.fs

  : task ( -- )
    \ the application runs as a background task
    measure  (       -- theta )    \ measure temperature
    logger   ( theta -- theta )    \ data logging
    control  ( theta -- theta )    \ temperature control
    menu     ( theta -- theta )    \ menu and display code
  ;

  : start   ( -- )
    \ start-up word
    init                   \ perform chained init
    [ ' task ] LITERAL BG !
  ;

  \ set boot vector to start-up word
  ' start 'BOOT !

RAM
\ Done. Type COLD to re-start!
