\ W1209 basic "thermostat for incubator" example 
\ Note: for illustration only - untested with real eggs :-)

NVM
#include measure.fs

\ temperature threshold in [0.1ºC]
: TEMPLIMIT ( -- n )
  375  \ 37.5ºC
;

\ formatted output for fixed point numbers
: .0 ( n -- )
  DUP 0< OVER 999 SWAP < OR IF
    10 / .  \ negative or > 99.9
  ELSE
    space <# # 46 hold #S #> type
  THEN
;

VARIABLE THERM10     \ filtered thermometer value as [0.1ºC]

\ background task with temperature control
: btask ( -- )
  measure            \ measure temperature
  DUP THERM10 !      \ save it for later use
  DUP .0 CR          \ print it
  TEMPLIMIT < OUT!   \ keep temperature stable
;

\ start-up word
: start   ( -- )
  10000 LPFDIG !
  [ ' btask ] LITERAL BG !
;

\ set boot vector to start-up word
' start 'BOOT !
RAM

\ Done! Type COLD to start, or power cycle

