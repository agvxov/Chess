using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

class Client : TcpClient {
	const int port = 14792;
	const string localhost = "127.0.0.1";

	StreamReader reader = null;
	StreamWriter writer = null;

	public bool doConnect(){
		try{
			Console.WriteLine("Connecting...");
			Connect(localhost, port);
			Console.WriteLine("Connected.");
		}catch(Exception e){
			Console.WriteLine("Exception occured: {0}", e);
			Console.WriteLine("Connection failed.");
			return false;
		}

		return Active;
	}

	public void Main(){
		reader = new StreamReader(GetStream());
		writer = new StreamWriter(GetStream());
		
		string s;
		Thread asyncer = null;
		while(true){
			s = reader.ReadLine();
			if(asyncer != null){
				asyncer.Abort();
			}
			if(s[0] == "\x11"[0]){
				s = s.Substring(1);
				//Console.WriteLine("Esc: {0}", s);
				switch(s){
					case "readline":
						Console.Write(": ");
						writer.WriteLine(Console.ReadLine());
						writer.Flush();
						break;
					case "spinner":
						Console.Write("Waiting for oponent");
						asyncer = new Thread(() => {
							const string dot = ". ";
							const int rep = 3;
							while(true){
								for(int i = 0; i < rep; i++){
									Console.Write(dot);
									Thread.Sleep(500);
								}
								Console.CursorLeft -= dot.Length*rep;
								for(int i = 0; i < (dot.Length*rep); i++){
									Console.Write(" ");
								}
								Console.CursorLeft -= dot.Length*rep;
							}
						});
						asyncer.Start();
						break;
				}
			}else{
				Console.WriteLine(s);
			}
			if(!Connected){
				Console.WriteLine("Connection lost.");
				break;
			}
		}
	}
}
