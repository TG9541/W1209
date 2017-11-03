\ W1209 temperature measurement with filter and noise suppression

#include math/inter.fs

\ read W1209 sensor input
: getadc ( -- n )
  6 ADC! ADC@ 0 ADC!
;

VARIABLE LPFDIG      \ lop pass filter state
\ low pass filter, applies a factor of 32
: lpf2* ( n -- n )
  LPFDIG @ DUP 32 / - + DUP LPFDIG !
;

\ interpolation table lpf2* to twice the temperature
CREATE dig2temp2 15 , \ # value pairs
        1216 , 2200 ,
        1534 , 2000 ,
        1982 , 1800 ,
        2560 , 1600 ,
        3296 , 1400 ,
        4320 , 1200 ,
        5634 , 1000 ,
        7370 ,  800 ,
        9632 ,  600 ,
       10942 ,  500 ,
       12382 ,  400 ,
       15522 ,  200 ,
       19012 ,    0 ,
       20768 , -100 ,
       22466 , -200 ,

VARIABLE THETA   \ temperature, noise suppression

\ +/-0.5 digit noise suppression, 2/
: hyst2/ ( n -- n/2 )
  DUP THETA @ - ABS  2-
  0< IF
    DROP THETA @
  ELSE
    DUP THETA !
  THEN
  2/
;

\ chained init
: init ( -- ) init
  \ init LPF state with max. value
  dig2temp2 DUP @ 1- 2* 1+ + @ LPFDIG !
;

\ temperature measurement
: measure   ( -- )
  getadc lpf2*
  dig2temp2 @inter
  hyst2/
;

