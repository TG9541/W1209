\ W1209 basic "thermostat for incubator" example
\ Note: for illustration only - untested with real eggs :-)

NVM
#require MARKER
RAM
  : TARGET NVM ;

  $4000 CONSTANT EE.SET
  $4002 CONSTANT EE.HYS
  $4004 CONSTANT EE.COR
  $4006 CONSTANT EE.DEL
  $4008 CONSTANT EE.LOG

  $8000 CONSTANT DEFAULT

TARGET

  : init ( -- )
    \ chained init - starting point
  ;

  : .0 ( n -- )
    \ print fixed point number
    DUP DEFAULT = IF
      DROP ."  DEF."  \ default display, e.g. sensor error
    ELSE
      \ formatted output for 3 digit 10x fixed point numbers
      DUP -99 < OVER 999 SWAP < OR IF
        \ number (and sign) too big for 3 7S-LED digits
        5 + 10 / . \ +0.5 before "floor division"
      ELSE
        \ -9.9 <= val <= 99.9
        SPACE DUP >R ABS <# # 46 HOLD #S R> SIGN #> TYPE
      THEN
    THEN
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
  ' start 'BOOT !
  \ Done. Type COLD to re-start!
RAM
