\ W1209 menu functions

  \ formatted output for fixed point numbers
  : .0 ( n -- )
    DUP 0< OVER 999 SWAP < OR IF
      10 / .  \ negative or > 99.9
    ELSE
      space <# # 46 hold #S #> type
    THEN
  ;

  \ key constants (A, B , D)
  : KEYSET   ( -- n ) 65 ;
  : KEYPLUS  ( -- n ) 66 ;
  : KEYMINUS ( -- n ) 68 ;

  CREATE MENUDAT 2 ,
    -100 , 1000 , 375 ,
    -20 , 20 , 0 ,

  VARIABLE MTIME   \ down counter for auto menu exit
  VARIABLE MLEVEL  \ menu level 0 .. 2
  VARIABLE MPARA   \ parameter number for level 1 & 2
  VARIABLE MVAL    \ parameter value in level 2

  : initmenu ( -- )
    0 MTIME !
    0 MLEVEL !
    0 MPARA !
    0 MVAL !
  ;

  : mtimeset ( -- )
    \ set MTIME to "seconds by ticks"
    [ 10 200 * ] LITERAL MTIME !
  ;

  : mtimer  ( -- )
    \ decrement and test MTIME
    MTIME @ DUP IF
      1- DUP MTIME !
    THEN
  ;

  : init ( -- ) init  \ chained init
    initmenu
  ;

  : mlevel0 ( n -- n )
    \ menu level 0
    DUP .0 CR
    ?KEY IF
      KEYSET = IF
        1 MLEVEL !
        mtimeset
      THEN
    THEN
  ;

  \ factor out SET/PLUS/MINUS by defining actions

  : mlevel12 ( -- )
    MLEVEL @  1 = IF
      80 emit MPARA @ . CR
      ?KEY IF
        mtimeset DUP KEYSET = IF
          DROP
          MLEVEL 2 !
        ELSE
          DUP KEYMINUS = IF
          ELSE
            KEYPLUS = IF
            THEN
          THEN
        THEN
      THEN
    ELSE
      ." inp" CR
    THEN
  ;

  : menu ( n -- n )
    \ menu code, display temperature value
    MLEVEL @ IF
      mtimer IF
        mlevel12
      ELSE
        initmenu
      THEN
    ELSE
      mlevel0
    THEN
  ;
