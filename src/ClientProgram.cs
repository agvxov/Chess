using System;
using System.Threading;
using System.Collections.Generic;

static class Program{

	static Client client = new Client();

	static void Main(string[] args){
		do{
			if(client.doConnect()){
				client.Main();
			}
			Thread.Sleep(1000);
		}while(true);
	}
}
