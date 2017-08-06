
all: flash

clean:
	rm out/*

defaults:
	stm8flash -c stlinkv2 -p stm8s103f3 -s opt -w tools/stm8s103FactoryDefaults.bin

flash:
	stm8flash -c stlinkv2 -p stm8s103f3 -w out/W1209-FD/W1209-FD.ihx

forth: main.ihx
	tools/simload.sh $(BOARD)


