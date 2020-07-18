STM8EF_BOARD=W1209-FD
STM8EF_VER=2.2.25
STM8EF_BIN=stm8ef-bin.zip
STM8EF_URL=https://github.com/TG9541/stm8ef/releases/download/${STM8EF_VER}/${STM8EF_BIN}

E4THCOM=e4thcom
TERM_PORT=ttyUSB0
TERM_BAUD=9600
TERM_FLAGS="-p mcu:target:lib"

ifeq ($(BOARD),)

# e4thcom style Forth code can't have a file suffix (but filenames are uppercase)
mmforth:=$(shell echo `ls|gawk '/^[A-Z0-9]*$$/ {print}'`)
forth=$(wildcard *fs) $(mmforth)

.PHONY: test clean

# Usage:make term BOARD=<board dir> [TERM_PORT=ttyXXXX] [TERM_BAUD=nnnn] [TERM_FLAGS="--half-duplex --idm"]
all: load

release: zip tgz

zip: simload
	find out/ -name "*.ihx" -print | zip -r out/stm8ef-bin docs/words.md inc/* mcu/* lib/* -@
	find out/ -name "simbreak.txt" -print | zip -r out/stm8ef-bin tools/* -@
	find out/ -name "target" -print | zip -r out/stm8ef-bin -@

tgz: simload
	( find out/ -path "*target/*" -print0 ; find out/ -name "*.ihx" -type f -print0 ; find out/ -name "simbreak.txt" -type f -print0 ) | tar -czvf out/stm8ef-bin.tgz LICENSE.md docs/words.md mcu lib tools --null -T -
	( find out/ -name "forth.rst" -type f -print0 ) | tar -czvf out/stm8ef-rst.tgz --null -T -

build: words
	make BOARD=CORE

load: flash
	tools/codeload.py -b out/$(STM8EF_BOARD) -p /dev/$(TERM_PORT) serial $(STM8EF_BOARD)/board.fs

flash: target
	stm8flash -c stlinkv2 -p stm8s103f3 -w out/$(STM8EF_BOARD)/$(STM8EF_BOARD).ihx

defaults:
	stm8flash -c stlinkv2 -p stm8s103f3 -s opt -w tools/stm8s103FactoryDefaults.bin

test: simload
	test/mbtest.sh $(STM8EF_BOARD)

simload: $(forth) target
	tools/simload.sh $(STM8EF_BOARD)
	touch simload

target: binary
	rm -f target
	ln -s out/${STM8EF_BOARD}/target target

binary: depend
	make BOARD=W1209-FD

depend:
	if [ ! -d "out" ]; then \
		curl -# -L -O ${STM8EF_URL}; \
		unzip -q -o ${STM8EF_BIN} -x out/*; \
		unzip -q -o ${STM8EF_BIN} out/${STM8EF_BOARD}/*; \
		rm ${STM8EF_BIN}; \
	fi
	touch depend

term:
	$(E4THCOM) -t stm8ef -p .:lib $(TERM_FLAGS) -d $(TERM_PORT) -b B$(TERM_BAUD)

clean:
	rm -rf target docs lib mcu out inc tools
	rm -f forth.asm forth.h forth.mk main.c STM8S103.efr STM8S105.efr
	rm -f simload depend

else
# the STM8 eForth make is a dependency
include forth.mk
endif
