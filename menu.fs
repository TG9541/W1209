\ W1209 menu functions

#include STARTTEMP

  \ key constants (A, B, D)
  65 CONSTANT KEY.SET
  66 CONSTANT KEY.PLUS
  68 CONSTANT KEY.MINUS
  70 CONSTANT KEY.PRESET  \ key+ & key-

  $8000 CONSTANT DEF?

TARGET

  : L' ( name -- )
    \ compile xt of next word as literal
    [COMPILE] ' [COMPILE] LITERAL
  ; IMMEDIATE

  : .0 ( n -- )
    \ formatted output for 3 digit 10x fixed point numbers
    DUP -99 < OVER 999 SWAP < OR IF
      \ number (and sign) too big for 3 7S-LED digits
      5 + 10 / . \ +0.5 before "floor division"
    ELSE
      \ -9.9 <= val <= 99.9
      SPACE DUP >R ABS <# # 46 HOLD #S R> SIGN #> TYPE
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
    CREATE
      , , , ,
    DOES>                  ( n a )
      OVER ( n a n ) DEF? = IF
        ( n a ) NIP @+ @ ( def aEE )
      ELSE
        ( n a ) SWAP OVER 2+ 2+ @+ @ ( a n min max )
        ROT MIN MAX DUP .0 ( a n )
      THEN
  \ <DOES
  ;

  \ A bit of trickery:
  \ * the following part of the dictionary is used as a linked list
  \ * menu items are Forth words
  \ * the name of the word is the menu text

  LAST @ ( -- na M.end )
  \ max  min  address def
     60    0  EE.DEL    0  >MENU DEL.
     20  -20  EE.COR    0  >MENU COR.
     15    0  EE.HYS    5  >MENU HYS.
    425  325  EE.SET  375  >MENU SET.
  LAST @ ( -- na M.start )
  CONSTANT M.start

  \ ( compile time stack: M.end )
  : M.next ( -- na )
    \ rotate to next menu item
    m.ptr DUP @ 2- @
    DUP ( M.end ) LITERAL = IF
      DROP M.start
    THEN
    DUP ROT !
  ;

  : M.prev ( -- na )
    \ rotate back one round to the previous menu item
    \ this is very inefficient but probably OK for < 20 items
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
      DEF? SWAP NAME> EXECUTE !  \ get defaults from menu item
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

  : m2set ( n -- 0 )
    ( n ) DUP
    m.ptr @ NAME> EXECUTE ( n a n )
    SWAP 2+ @ ULOCK ! LOCK ( n )
    0 m.time !
  ;

  : m2 ( -- )
    m.val @
    L' m2set L' 1+ L' 1- keyExe
    m.ptr @ NAME> EXECUTE ( a n )
    NIP m.val !
  ;

  : m1select
    DEF? m.ptr @ NAME> EXECUTE ( def aEE )
    NIP @ m.val !
    L' m2 m.level !
  ;

  : m1 ( -- )
    L' m1select L' m.next L' m.prev keyExe
    m.ptr @ count type
  ;

  : m0 ( -- )
    ?KEY IF
      KEY.SET key.test IF
        DROP
        M.timeset
        L' m1 m.level !
      ELSE
        KEY.PRESET key.test IF
          DROP
          m.hold @ IF
            -1 m.hold +!
          THEN
        THEN
      THEN
    ELSE
      m.hold @ 0= IF
        BKEY IF
          ." RES"
        ELSE
          preset
          10 m.hold !
        THEN
      ELSE
        DUP .0
      THEN
    THEN
  ;

  : menu ( n -- n )
    \ menu code, display temperature value
    M.timer 0= IF
      L' m0 m.level !
    THEN
    m.level @ EXECUTE
  ;

  : init ( -- ) init  \ chained init
    M.start m.ptr !   \ point to the first menu item
    0 m.time !        \ time-out -> menu init
    10 m.hold !
  ;

ENDTEMP
