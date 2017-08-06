\ STM8S helper: execute next word, and store result
\ refer to github.com/TG9541/stm8ef/blob/master/LICENSE.md

\ execute the following word, compile the result as literal
: 'ELIT ( <string> -> n -- ) 
  ' EXECUTE [COMPILE] LITERAL 
; IMMEDIATE

\ execute the following word, write result to dictionary
: 'E,  ( <string> -> n -- ) 
  ' EXECUTE [COMPILE] , 
; IMMEDIATE

\ execute the following word, write result to dictionary as char
: 'EC,  ( <string> -> c -- ) 
  ' EXECUTE [COMPILE] C, 
; IMMEDIATE
