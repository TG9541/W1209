\ W1209 data logger functions

  VARIABLE LOGTIM

  \ timer

  : init ( -- ) init  \ chained init
    0 LOGTIM !
  ;

  : logger ( theta -- theta )
    \ data logger
    ( implement me )
  ;
