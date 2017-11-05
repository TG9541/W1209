\ W1209 temperature control functions
\ For now: just a simple "chicken egg incubator" 2-point control example

  : TEMPLIMIT ( -- n )
    \ temperature threshold in [0.1ºC]
    375  \ 37.5ºC
  ;

  : init ( -- ) init \ chained init
  ;

  : control ( theta -- theta )
    \ simple temperature control
    DUP TEMPLIMIT < OUT!   \ keep temperature stable
  ;
