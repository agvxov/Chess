using System;
using System.Collections.Generic;

static class ServerProgram{

	static Server server = new Server();

	static void Main(string[] args){
		Byte[] buffer = new Byte[256];
		String data = null;

		server.Start();
		server.MakeMatch();
	}
}
