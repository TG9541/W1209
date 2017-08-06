\ W1209 temperature measurement with filter and noise suppression

#include math/inter.fs

\ read W1209 sensor input
: getadc ( -- n )
  6 ADC! ADC@ 0 ADC!
;

VARIABLE LPFDIG      \ lop pass filter state
\ low pass filter, applies a factor of 2
: lpf2*   ( n -- n )
  LPFDIG @ DUP 32 / - + DUP LPFDIG !
;

\ interpolation table lpf2* to double the temperature
CREATE dig2temp2 15 ,
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

VARIABLE HYSTDIG     \ noise cancel storage

\ Cancel +/-0.5 digit noise, 2/
: hyst2/ ( n -- n/2 )
  DUP HYSTDIG @ - ABS  2-
  0< IF
    DROP HYSTDIG @
  ELSE
    DUP HYSTDIG !
  THEN
  2/
;

\ temperature measurement
: measure   ( -- )
  getadc lpf2*
  dig2temp2 @inter
  hyst2/
;

