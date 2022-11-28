using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

class Server{
	
	const int port = 14792;
	IPAddress localhost = IPAddress.Parse("127.0.0.1");
	TcpListener server = null;

	List<TcpClient> clients = new List<TcpClient>();
	List<Thread> matches = new List<Thread>();
	const Object matchMakingLock = null;

	bool running = false;

	public bool Start(){
		Console.WriteLine("Server starting...");
		server = new TcpListener(localhost, port);

		running = true;
		Thread ListnerThread = new Thread(Listen);
		ListnerThread.Start();

		Console.WriteLine("Listening...");

		return true;
	}

	public bool Stop(){
		running = false;

		return true;
	}

	void Listen(){
		server.Start();
		while(running){
			var c = server.AcceptTcpClient();
			lock(clients){
				clients.Add(c);
			}
			Console.WriteLine("Client connected. New number of Clients: {0}", clients.Count + (matches.Count*2));
		}
	}

	public void MakeMatch(){
		while(true){
			if(clients.Count != 0 && clients.Count % 2 == 0){
				lock(clients) lock(matches){
					if(clients.Count != 0 && clients.Count % 2 == 0){
						Console.WriteLine("Making a match...");
						var c1 = clients[0];
						var c2 = clients[1];
						clients.RemoveAt(1);
						clients.RemoveAt(0);
						matches.Add(new Thread(() => {
							Console.WriteLine("Match started.");
							Play(c1, c2);
							Console.WriteLine("Match finished.");
						}));
						matches[matches.Count-1].Start();
					}
				}
			}
		}

	}

	internal struct Player{
		public TcpClient c;
		public StreamReader r;
		public StreamWriter w;
		public Szin s;
	};

	bool? Play(TcpClient c1, TcpClient c2){
		Player[] players = new Player[2];
		players[0].c = c1;
		players[0].r = new StreamReader(c1.GetStream());
		players[0].w = new StreamWriter(c1.GetStream());
		players[0].s = Szin.FEHER;
		players[1].c = c2;
		players[1].r = new StreamReader(c2.GetStream());
		players[1].w = new StreamWriter(c2.GetStream());
		players[1].s = Szin.FEKETE;

		Asztal asztal = new Asztal(Jatekmod.klasszikus);

		string s = "";
		Thread tInput = null;
		while(true){
			try{
				foreach(var i in players){
					i.w.Write(asztal.ToString());
					i.w.Flush();
					if(i.s == asztal.mozgathat){
						i.w.WriteLine("\x11spinner");
						i.w.Flush();
					}else{
						tInput = new Thread(() => {
								i.w.WriteLine("\x11readline");
								i.w.Flush();
								s = i.r.ReadLine();
								asztal.mozgat(s);
							});
						tInput.Start();
					}
				}
				tInput.Join();
				Console.WriteLine("Recieved input: {0}", s);
			}catch(Exception e){
				Console.WriteLine(e);
				break;
			}
		}

		foreach(var i in players){
			i.w.WriteLine("poll");
			i.w.Flush();
			if(i.c.Connected){
				lock(clients){
					clients.Add(i.c);
				}
			}
		}

		return null;
	}
}

