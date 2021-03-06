\ W1209 temperature control functions
\ © 2017 TG9541, refer to https://github.com/TG9541/W1209/blob/master/LICENSE

\ For now: just a simple 2-point controller

#include STARTTEMP

   0 CONSTANT OFF
  -1 CONSTANT ON

TARGET

  VARIABLE c.heat
  VARIABLE c.delay

  : C.off ( -- n )
    \ upper threshold [0.1ºC]
    EE.SET @
  ;

  : C.on ( -- n )
    \ lower threshold [0.1ºC]
    C.off EE.HYS @ -
  ;

  : controller ( theta -- flag )
    \ simple temperature control with hystesis & delay
    c.heat @ IF
      ( theta ) C.off SWAP < IF
        OFF c.heat !
        EE.DEL @ ( [10s] )
        20 ( ticks [5ms] ) * c.delay !
      THEN
    ELSE
      ( theta ) C.on < IF
        c.delay @ IF
          -1 c.delay +!
        ELSE
          ON c.heat !
        THEN
      THEN
    THEN
    c.heat @           \ return flag
  ;

  : control ( theta -- theta )
    DUP DEFAULT = NOT IF
      DUP controller ( flag )
    ELSE
      0  ( flag )      \ sensor value undefined: control variable inactive
    THEN
    ( flag ) OUT!      \ switch relay
  ;

  : init ( -- ) init   \ chained init
    OFF c.heat !
    0 c.delay !
  ;

ENDTEMP
