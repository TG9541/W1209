\ W1209 data logger functions
\ TODO 24bit record min (10) , delta (8) , duty cycle (6)

#include STARTTEMP

  $403E CONSTANT EE.PTR
  $4040 CONSTANT L.START
  $4280 CONSTANT L.END

TARGET

  VARIABLE l.tim  \ minute timer
  VARIABLE l.min  \ minute counter

  : L.inc ( aL -- aL )
    \ increment log write pointer w/ wrap around
    2+ DUP [ L.START 2- ] LITERAL L.END ROT
    ( min max ptr ) WITHIN NOT IF
      ( ptr ) DROP L.START
    THEN
  ;

  : L.log ( n -- )
    \ log data w/ wrap around
    ULOCK
    EE.PTR @ L.inc DUP EE.PTR ! ( n ptr ) !
    LOCK
  ;

  : L.dump ( -- )
    \ log dump, start with the oldest entry
    1 EE.PTR @ DUP >R
    BEGIN
      SWAP DUP . 1+ SWAP
      L.inc DUP @ .0 CR
      DUP R@ =
    UNTIL
    R> DROP 2DROP
  ;

  : L.res ( -- )
    \ reset the log minute counter
    1 l.min !
  ;

  : L.wipe ( -- )
    \ wipe data log memory
    L.res
    ULOCK
    L.END L.START DO
      DEFAULT I ! 2
    +LOOP
    LOCK
  ;

  : logger ( theta -- theta )
    \ data logger
    l.tim @ TIM < IF
      TIM [ 200 ( ticks/s ) 60 * ] LITERAL + l.tim !

      l.min @ EE.LOG @ ( [6min] ) 6 * < IF
        1 l.min +!
      ELSE
        L.res ( theta ) DUP L.log
      THEN
    THEN
  ;

  : init ( -- ) init  \ chained init
    L.res
  ;

ENDTEMP
