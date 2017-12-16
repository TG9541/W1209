\ W1209 data logger functions
\ © 2017 TG9541, refer to https://github.com/TG9541/W1209/blob/master/LICENSE

#include STARTTEMP

  $403E CONSTANT EE.PTR
  $4040 CONSTANT L.START
  $4280 CONSTANT L.END

  DEFAULT SET. DROP .S       \ default temperature from menu
        CONSTANT L.OFFS    \ use as offset in log memory

TARGET

  VARIABLE l.tim           \ seconds timer
  VARIABLE l.sec           \ seconds counter

  VARIABLE l.rcnt          \ control relay "half cycle" count
  VARIABLE l.ron           \ seconds relay on
  VARIABLE l.roff          \ seconds relay off

  VARIABLE l.tmin          \ minimal temperature
  VARIABLE l.tmax          \ maximal temparature

  : L.res ( -- )
    \ reset min/max temperature and control output metrics
    10000   l.tmin !
    DEFAULT l.tmax !
    0 l.ron !
    0 l.roff !
    0 l.rcnt !
  ;

  : L.inc ( aL -- aL )
    \ increment log write pointer w/ wrap around
    2+ DUP [ L.START 2- ] LITERAL L.END ROT
    ( min max ptr ) WITHIN NOT IF
      ( ptr ) DROP L.START
    THEN
  ;

  : L.pack ( u6 n10 -- w )
    \ pack a signed n10 (10 valid bits) w6 to w
    64 *   SWAP 63 MIN +
  ;

  : L.unpack ( w  -- n6 n10 )
    \ unpack w to an unsigned u6 and a signed n10
    DUP   63 AND   SWAP 64 /
  ;

  : L.log ( -- )
    \ write log data to ring buffer
    \ get & inc ring buffer pointer
    ULOCK EE.PTR @ L.inc
    \ pack max-temp offset, "control active" count
    l.tmax @ l.tmin @ - ( n6 )  l.rcnt @ 2/ 1023 MIN ( n10 )
    L.pack OVER (  a n a -- a ) !
    \ pack duty cycle, min-temp
    l.ron @ l.roff @ OVER + 63 SWAP */ ( n6 ) l.tmin @ L.OFFS - ( n10 )
    L.pack SWAP L.inc DUP ( w a a ) EE.PTR ! ( w a ) !
    LOCK L.res
  ;

  : L.dump ( -- )
    \ log dump, start with the oldest entry
    EE.PTR @ DUP >R
    BEGIN
      L.inc DUP L.inc DUP ( L L+ L+ )
      ROT ( L+ L+ L ) @ L.unpack ( L+ L+ tdif rcnt )
      ROT @ L.unpack ( tdif rcnt duty temp )
      SWAP ( duty ) 100 63 */ .
      SWAP ( rcnt ) .
      ( tdif temp ) L.OFFS + SWAP ( tmin tdif ) OVER
      ( tmin tdif tmin ) + SWAP ( tmax tmin ) .0 .0 CR
      DUP R@ =
    UNTIL
    R> 2DROP
  ;

  : L.wipe ( -- )
    \ wipe data log memory
    ULOCK L.START L.END OVER - ERASE  LOCK
  ;

  : logger ( theta -- theta )
    \ data logger

    1 l.tim                    \ second timer ...
    DUP ( l.tim ) @  200 ( 1s/5ms ) U< IF
      ( 1 l.tim ) +!           \ ... 5ms increment
    ELSE
      ( 1 l.tim ) !            \ ... second tick

      \ record temperatur and controller output
      DUP l.tmin @ MIN l.tmin !
      DUP l.tmax @ MAX l.tmax !

      \ record controller output (relay trip cycle #, average duty cycle)
      OUT @ ( flag ) IF
        1 l.ron +!               \ inc heating cycles count
        l.rcnt @ 1 OR l.rcnt !   \ set bit0
      ELSE
        1 l.roff +!              \ inc inactive cycles count
        l.rcnt @ 1 AND l.rcnt +! \ inc l.rcnt once
      THEN

      \ only log data if interval > 0
      ( theta ) EE.LOG @ IF
        1 l.sec                \ second counter ...
        DUP ( l.sec ) @ EE.LOG @ 360 ( 0.1h as s ) * U< IF
          ( 1 l.sec ) +!       \ ... increment
        ELSE
          ( 1 l.sec ) !        \ ... reset
          L.log                \ pack and log data
        THEN
      THEN
    THEN
  ;

  : init ( -- ) init  \ chained init
    L.res
  ;

ENDTEMP
