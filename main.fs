\ W1209 basic "thermostat for incubator" example
\ Note: for illustration only - untested with real eggs :-)

NVM
#require MARKER
RAM
  : TARGET NVM ;
TARGET

  : init ( -- )
    \ chained init - starting point
  ;

#include measure.fs
#include control.fs
#include logger.fs
#include menu.fs

  : task ( -- )
    \ the application runs as a background task
    measure            \ measure temperature
    control            \ temperature control
    logger             \ data logging
    menu               \ menu and display code
  ;

  : start   ( -- )
    \ start-up word
    init               \ perform chained init
    [ ' task ] LITERAL BG !
  ;

  \ set boot vector to start-up word
  \ ' start 'BOOT !
  \ Done. Type COLD to start!

RAM
