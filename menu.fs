\ W1209 menu functions

#include STARTTEMP

  \ key constants (A, B, D)
  65 CONSTANT KEY.SET
  66 CONSTANT KEY.PLUS
  68 CONSTANT KEY.MINUS
  70 CONSTANT KEY.PRESET  \ key+ & key-

TARGET

  : L' ( name -- )
    \ compile xt of next word as literal
    [COMPILE] ' [COMPILE] LITERAL
  ; IMMEDIATE

  : .0 ( n -- )
    \ print fixed point print number
    DUP DEFAULT = IF
      DROP ." DEF."  \ default display, e.g. sensor error
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

  : @+ ( a -- n a+2 )
    \ return next a and n at a
    DUP @ SWAP 2+
  ;

  VARIABLE m.time   \ down counter for auto menu exit
  VARIABLE m.level  \ menu level 0 .. 2
  VARIABLE m.val    \ parameter value in level 2
  VARIABLE m.ptr    \ current menu
  VARIABLE m.hold   \ key hold count

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
      DUP .0
  \ <DOES
  ;

  \ Some Forth trickery:
  \ * the following part of the dictionary is used as a linked list
  \ * menu items are Forth words
  \ * the name of the word is the menu text

  LAST @ ( -- na M.end )
  \ max  min  address def
    600    0  EE.DEL    0  >MENU DEL.
     20  -20  EE.COR    0  >MENU COR.
     20    1  EE.HYS    5  >MENU HYS.
    425  325  EE.SET  375  >MENU SET.
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

  : M.timer  ( -- )
    \ decrement and test m.time
    m.time @ DUP IF
      1- DUP m.time !
    THEN
  ;

  : key.test ( n key -- n flag )
    OVER =
  ;

  : keyExe ( xtset xt+ xt- -- )
    \ pressed key selects and executes an xt
    ?KEY IF M.timeset ELSE 0 THEN

    KEY.SET key.test IF
      DROP
      2DROP    EXECUTE  \ xtset
    ELSE KEY.PLUS key.test IF
      DROP
      DROP NIP EXECUTE  \ xt+
    ELSE KEY.MINUS key.test IF
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
    m.ptr @ count type \ print name in dictionary
  ;

  : M.rh ( -- )
    \ reset hold
    25 m.hold !
  ;

  : M0 ( -- )
    \ main menu ( first rough version )
    ?KEY IF
      \ key actions
      KEY.SET key.test IF
        DROP M.timeset   L' M1 m.level !
      ELSE KEY.PRESET key.test IF
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
          preset M.rh \ reset hold
        THEN
      ELSE
        DUP .0
      THEN
    THEN
  ;

  : menu ( n -- n )
    \ menu code, display temperature value
    M.timer 0= IF
      \ time-out sub menus
      L' M0 m.level !
    THEN
    m.level @ EXECUTE
  ;

  : init ( -- ) init  \ chained init
    M.rh              \ reset hold
    M.START m.ptr !   \ point to the first menu item
    0 m.time !        \ time-out -> menu init
  ;

ENDTEMP
