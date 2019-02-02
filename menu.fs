\ W1209 menu functions
\ © 2017 TG9541, refer to https://github.com/TG9541/W1209/blob/master/LICENSE

#include STARTTEMP

#require :NVM
#require ]B!

\res MCU: STM8S103
\res export PC_DDR
  5  CONSTANT PNTX

  \ key constants (A, B, D)
  65 CONSTANT KEY.SET
  66 CONSTANT KEY.PLUS
  68 CONSTANT KEY.MINUS
  70 CONSTANT KEY.PRESET  \ key+ & key-

  : L' ( name -- )
    \ compile xt of next word as literal
    [COMPILE] ' [COMPILE] LITERAL
  ; IMMEDIATE

TARGET

  VARIABLE m.time   \ down counter for auto menu exit
  VARIABLE m.level  \ menu level 0 .. 2
  VARIABLE m.val    \ parameter value in level 2
  VARIABLE m.ptr    \ current menu
  VARIABLE m.hold   \ key hold count
  VARIABLE m.disp   \ last display value
  VARIABLE m.tx     \ delay menu action after TX

  : M..0 ( n -- n )
    \ lazy .0: print number only if value has changed
    DUP m.disp @ = NOT IF
      DUP m.disp ! DUP .0
    THEN
  ;

  : @+ ( a1 -- n a2 )
    \ return next n at a1 and a2 = a1+2
    DUP @ SWAP 2+
  ;

  : >MENU ( c -- )
    \ define menu entry w/ processing
    CREATE
      , , , ,
    DOES>                  ( n a )
      OVER ( n a n ) DEFAULT = IF
        \ mode A: return default value & EE address
        ( n a ) NIP @+ @ ( def aEE )
      ELSE
        \ mode B: return bounded value & table address
        ( n a ) SWAP OVER 2+ 2+ @+ @ ( a n min max )
        ROT MIN MAX ( a n )
      THEN
      M..0 ( n -- n )    \  print .n if number has changed
  \ <DOES
  ;

  \ Implementation hints:
  \ * menu items are Forth words
  \ * a section of the dictionary is used as a linked list
  \ * the name of a word is used as the menu item string

  LAST @ ( -- na M.end )
  \ max  min  address def
    100    0  EE.LOG   10  >MENU LOG.  \ [0.1h], 0: off
    600    0  EE.DEL    0  >MENU DEL.  \ [0.1s]
     20  -20  EE.COR    0  >MENU COR.  \ [0.1C]
     20    1  EE.HYS    5  >MENU HYS.  \ [0.1C]
    800  100  EE.SET  375  >MENU SET.  \ [0.1C]
  LAST @ CONSTANT M.START

  \ ( compile time stack: M.end )
  : M.next ( -- na )
    \ rotate to next menu item
    m.ptr DUP @ 2- @
    DUP ( M.end ) LITERAL = IF
      DROP M.START
    THEN
    DUP ROT !
  ;

  : M.prev ( -- na )
    \ rotate back one round to the previous menu item
    \ note: this inefficient but OK for < 20 menu items
    m.ptr @ DUP >R
    BEGIN
      M.next DUP
      R@ = NOT WHILE
      NIP
    REPEAT
    R> 2DROP
    DUP m.ptr !
  ;

  : preset ( -- )
    \ initialize parameters with defaults
    ULOCK
    m.ptr @ DUP >R        \ get current menu item link address
    BEGIN
      DEFAULT SWAP NAME> EXECUTE !  \ get defaults from menu item
      M.next DUP
      R@ = NOT WHILE
    REPEAT
    R> 2DROP
    LOCK
  ;

  : M.timeset ( -- )
    \ set m.time to "seconds by ticks"
    [ 10 200 * ] LITERAL m.time !
  ;

  : M.timer  ( -- n )
    \ decrement and test m.time
    \ m.time @ DUP 0= NOT + DUP m.time !
    m.time @ DUP IF
      1- DUP m.time !
    THEN
  ;

  : M.key= ( n key -- n flag )
    \ compare key code
    OVER =
  ;

  : keyExe ( xtset xt+ xt- -- )
    \ pressed key selects and executes an xt
    ?KEY IF M.timeset ELSE 0 THEN

    KEY.SET M.key= IF
      DROP
      2DROP    EXECUTE  \ xtset
    ELSE KEY.PLUS M.key= IF
      DROP
      DROP NIP EXECUTE  \ xt+
    ELSE KEY.MINUS M.key= IF
      DROP
      NIP  NIP EXECUTE  \ xt-
    ELSE
      2DROP 2DROP       \ do nothing
    THEN THEN THEN
  ;

  : M.exec ( n -- a n | def aEE )
    \ execute m.ptr "method"
    m.ptr @ NAME> EXECUTE
  ;

  : M2.set ( n -- 0 )
    \ M2 action: store parameter value to EE
    ( n ) DUP M.exec ( n a n )
    SWAP 2+ @ ( n n aEE ) ULOCK ! LOCK ( n )
    0 m.time !  \ return to M0 (OK, that's a hack)
  ;

  : M2 ( -- )
    \ edit parameter value
    m.val @
    L' M2.set L' 1+ L' 1- keyExe
    M.exec ( a n ) NIP m.val !
  ;

  : M1.select
    \ M1 action: initialize and start parameter editor
    DEFAULT M.exec ( def aEE )
    NIP @ m.val !
    L' M2 m.level !
  ;

  : M1 ( -- )
    \ select parameter
    L' M1.select L' M.next L' M.prev keyExe
    m.ptr @ count type   \ print name in dictionary
  ;

  : M.rh ( -- )
    \ reset hold
    25 m.hold !
  ;

  : M0 ( theta -- theta )
    \ main menu - normally print theta
    \ enter menu on key set, preset on key +/-
    ?KEY IF
      \ key actions
      KEY.SET M.key= IF
        DROP M.timeset   L' M1 m.level !
      ELSE KEY.PRESET M.key= IF
        DROP m.hold @ IF
          -1 m.hold +!
        THEN
      THEN THEN
    ELSE
      m.hold @ 0= IF
        BKEY IF
          \ show that +/- have been held long enough
          ." RES"
        ELSE
          \ key released - execute preset
          preset M.rh   \ reset hold
        THEN
      ELSE
        M..0 ( n -- n ) \  print n if number has changed
      THEN
    THEN
  ;

  \ menu code with time-out handling
  : menu ( theta -- theta )
    \ work around GPIO dual use effects
    m.tx @ ?DUP IF
      1- m.tx !
    ELSE
      [ 0 PC_DDR PNTX ]B!
      M.timer 0= IF
        \ time-out sub menus
        L' M0 m.level !
      THEN
      m.level @ EXECUTE
    THEN
  ;

  \ wrap TX! with GPIO control
  :NVM ( c -- )
    [ 1 PC_DDR PNTX ]B!   2 m.tx !
    TX!
  ;NVM ( xt )

  : init ( -- ) init    \ chained init
    M.rh                \ reset hold
    M.START m.ptr !     \ point to the first menu item
    0 m.time !          \ time-out -> menu init
    [ ( xt ) ] LITERAL 'EMIT !     \ work-around W1209 key GPIO dual use as TxD
  ;

ENDTEMP
