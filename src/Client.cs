using System;
using System.IO;
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
		while(true){
			s = reader.ReadLine();
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
						Console.Write(". . .");
						Console.WriteLine("");
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
