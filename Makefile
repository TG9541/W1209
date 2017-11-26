STM8EF_BOARD=W1209-FD
STM8EF_VER=2.2.20.pre.2
STM8EF_BIN=stm8ef-bin.zip
STM8EF_URL=https://github.com/TG9541/stm8ef/releases/download/${STM8EF_VER}/${STM8EF_BIN}

all: flash

zip:
	zip -r ${STM8EF_BOARD}-forth ${STM8EF_BOARD}-forth.ihx

defaults:
	stm8flash -c stlinkv2 -p stm8s103f3 -s opt -w tools/stm8s103FactoryDefaults.bin

flash: depend
	stm8flash -c stlinkv2 -p stm8s103f3 -w out/W1209-FD/W1209-FD.ihx

load: depend
	tools/codeload.py serial main.fs

simload: depend
	tools/simload.sh $(STM8EF_BOARD)

target: depend
	rm -f target
	rm -f FORTH.efr
	ln -s out/${STM8EF_BOARD}/target target
	ln -s out/${STM8EF_BOARD}/FORTH.efr .

depend:
	if [ ! -d "out" ]; then \
		curl -# -L -O ${STM8EF_URL}; \
		unzip -q -o ${STM8EF_BIN} -x out/*; \
		unzip -q -o ${STM8EF_BIN} out/${STM8EF_BOARD}/*; \
		rm ${STM8EF_BIN}; \
	fi

clean:
	rm -rf target FORTH.efr STM8S103.efr STM8S105.efr docs lib mcu out tools
