\ W1209 temperature control functions

\ temperature threshold in [0.1ºC]
: TEMPLIMIT ( -- n )
  375  \ 37.5ºC
;

\ chained init
: init ( -- ) init
;

\ simple temperature control
: control ( n -- n )
  DUP TEMPLIMIT < OUT!   \ keep temperature stable
;
