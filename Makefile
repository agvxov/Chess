.PHONY: main clean

SRCD:=src/
SRC:=Program.cs
SRC:=$(addprefix ${SRCD},${SRC})
OUTPUT:=chess.out

main:
	mcs ${SRC} -o ${OUTPUT}

run:
	mono ${OUTPUT}

clean:
