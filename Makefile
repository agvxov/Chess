.PHONY: main clean run

CC:=mcs
SRCD:=src/
SRC:=Figura.cs Asztal.cs PlaccInfo.cs enum.cs
SRC:=$(addprefix ${SRCD},${SRC})
COMP:=${CC} ${SRC}
OUTPUT:=chess.exe Chess_server.exe Chess_client.exe

main: server client singleplayer
	@echo done

server:
	${COMP} ${SRCD}/Server.cs -out:$(word 2,${OUTPUT})

client:
	${COMP} ${SRCD}/Client.cs -out:$(word 3,${OUTPUT})

singleplayer:
	${COMP} ${SRCD}/SinglePlayer.cs -out:$(word 1,${OUTPUT})

run:
	mono $(word 2,3,${OUTPUT})

run_single:
	mono $(word 1,${OUTPUT})

clean:
	rm ${OUTPUT}
