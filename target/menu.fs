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

\ set MTIME to "seconds by ticks"
: mtimeset ( -- )
  [ 10 200 * ] LITERAL MTIME !
;

\ decrement and test MTIME
: mtimer  MTIME @ DUP IF
    1- DUP MTIME !
  THEN
;

\ chained init
: init ( -- ) init
  initmenu
;
\

\ menu level 0
: mlevel0 ( n -- n )
  DUP .0 CR
  ?KEY IF
    KEYSET = IF
      1 MLEVEL !
      mtimeset
    THEN
  THEN
;

: mlevel12 ( -- )
  ." MEN" CR
;

\ menu code, display temperature value
: menu ( n -- n )
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

