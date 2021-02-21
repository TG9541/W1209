\ W1209 temperature measurement with filter and noise suppression
\ © 2017 TG9541, refer to https://github.com/TG9541/W1209/blob/master/LICENSE

\ Note: W1209 thermostats may require adjustment,
\       especially when used outside the range of -5C to +20C
\       Refer to https://github.com/TG9541/W1209/wiki/W1209-Sensor

#include STARTTEMP

  900 CONSTANT USENSMAX

TARGET

#require @inter
#require dig2tem

  VARIABLE lpf.adc      \ LPF memory for ADC value
  VARIABLE lpf.tem      \ LPF memory for temperature
  VARIABLE lpf.tem2     \ unchatter memory
  VARIABLE theta        \ temperature value

  : lpf32 ( n1 a -- n2 )
    \ low pass filter, multiplies n1 by 32, uses a as LPF memory
    ( a ) DUP >R @ DUP 32 / - + DUP R> !
  ;

  : unchatter ( n1 -- n2 )
    \ remove chatter (+/- 0.5 digit window), divides n1 by 32
    16 / DUP lpf.tem2 @ - ABS  2-
    0< IF
      DROP lpf.tem2 @
    ELSE
      DUP lpf.tem2 !
    THEN
    2/
  ;

  : init ( -- ) init  \ chained init
    \ init LPF memories with max. value
    [ dig2tem 2+ DUP @ ] LITERAL lpf.adc !
    [         2+     @ ] LITERAL lpf.tem !
  ;

  : measure   ( -- theta )
    \ temperature measurement
    ADC@                \ read noisy W1209 sensor input

    DUP USENSMAX < IF
      lpf.adc lpf32     \ low pass filter, "turn noise into digits", 32*ADC
      dig2tem @inter    \ interpolation: lpf32 digits to temperature
      lpf.tem lpf32     \ low pass filter, "smoothen measurement", 32*theta
      unchatter         \ digitalization chatter removel, divides by 32
      EE.COR @ +        \ apply corrective offset from menu "Cor."
    ELSE
      DROP DEFAULT      \ sensor error - default
    THEN

    DUP theta !         \ keep a copy of theta, e.g. for console use
  ;

ENDTEMP
