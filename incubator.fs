\ W1209 basic "thermostat for incubator" example
\ Note: for illustration only - untested with real eggs :-)

NVM

\ chained init - starting point
: init ( -- )
;

#include measure.fs
#include control.fs
#include logger.fs
#include menu.fs

\ background task with temperature control
: task ( -- )
  measure            \ measure temperature
  control            \ temperature control
  logger             \ data logging
  menu               \ menu and display code
;

\ start-up word
: start   ( -- )
  init
  [ ' task ] LITERAL BG !
;

\ set boot vector to start-up word
' start 'BOOT !
RAM

\ Done. Type COLD to start!
