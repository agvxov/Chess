.PHONY: main clean

SRCD:=src/
SRC:=Program.cs Figura.cs Asztal.cs enum.cs
SRC:=$(addprefix ${SRCD},${SRC})
OUTPUT:=chess.mono

main:
	mcs ${SRC} -o ${OUTPUT}

run:
	mono ${OUTPUT}

clean:
	rm ${OUTPUT}
