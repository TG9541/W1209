\ STM8S103 PORTC register words
\ refer to github.com/TG9541/stm8ef/blob/master/LICENSE.md

: PC_ODR  $500A [COMPILE] LITERAL ; IMMEDIATE \ Port C data output latch register     (0x00)
: PC_IDR  $500B [COMPILE] LITERAL ; IMMEDIATE \ Port C input pin value register       (0xXX)
: PC_DDR  $500C [COMPILE] LITERAL ; IMMEDIATE \ Port C data direction register        (0x00)
: PC_CR1  $500D [COMPILE] LITERAL ; IMMEDIATE \ Port C control register 1             (0x00)
: PC_CR2  $500E [COMPILE] LITERAL ; IMMEDIATE \ Port C control register 2             (0x00)

