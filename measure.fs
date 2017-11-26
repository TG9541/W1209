\ W1209 temperature measurement with filter and noise suppression

\ Note: W1209 thermostats may require individual adjusted
\       especially when used outside the range of -5C to +20C
\       Refer to https://github.com/TG9541/W1209/wiki/W1209-Sensor

#include STARTTEMP

  900 CONSTANT USENSMAX

TARGET

#require @inter
  DECIMAL ( uCsim )

  \ note: adjust to specific sensor here if accuracy is required!
  \ note: @inter accepts 2..n value pairs
  \ interpolation table (lpf2*) -> 2*(temperature value)
  CREATE dig2temp2 15 ,  \ number of value pairs
          1216 , 2200 ,  \  1  1216 lpf2* digits -> 20/C * 110 C
          1534 , 2000 ,  \  2
          1982 , 1800 ,  \  3
          2560 , 1600 ,  \  4
          3296 , 1400 ,  \  5
          4320 , 1200 ,  \  6
          5634 , 1000 ,  \  7
          7370 ,  800 ,  \  8
          9632 ,  600 ,  \  9
         10942 ,  500 ,  \ 10
         12382 ,  400 ,  \ 11
         15522 ,  200 ,  \ 12
         19012 ,    0 ,  \ 13
         20768 , -100 ,  \ 14
         22466 , -200 ,  \ 15  22466 lpf2* digits -> 20/C * (-10 C)

  VARIABLE THETA       \ temperature, noise suppression
  VARIABLE LPFDIG      \ low pass filter state

  : getadc ( -- n )
    \ read W1209 sensor input
    6 ADC! ADC@ 0 ADC!
  ;

   \ Note: digitalization jitter is suppressed by a sliding window
   \       (0.05 C hysteresis) after LPF and scaling

  : lpf2* ( n -- n )
    \ low pass filter, applies a factor of 32
    LPFDIG @ DUP 32 / - + DUP LPFDIG !
  ;

  : hyst2/ ( n -- n/2 )
    \ +/-0.5 digit noise suppression, 2/
    DUP THETA @ - ABS  2-
    0< IF
      DROP THETA @
    ELSE
      DUP THETA !
    THEN
    2/
  ;

  : init ( -- ) init  \ chained init
    \ init LPF state with max. value
    dig2temp2 DUP @ 1- 2* 1+ + @ LPFDIG !
  ;

  : measure   ( -- temperature )
    \ temperature measurement
    getadc            \ noisy ADC readout with bad digit resolution
    DUP USENSMAX < IF
      lpf2*             \ low pass filter, turn noise into digits
      dig2temp2 @inter  \ digits to 2*temperature
      hyst2/            \ sliding window makes measurement "steady"
      EE.COR @ +        \ offset from menu
    ELSE
      DROP DEFAULT      \ sensor error - default
    THEN
  ;

ENDTEMP
