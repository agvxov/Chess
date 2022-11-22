using System;
using System.Collections.Generic;

class Program{
	static Random r = new Random();

	static void Main(string[] args){
		Asztal tabla = new Asztal(Jatekmod.klasszikus);

		while(true){
			tabla.print();
			tabla.mozgat();
		}
	}
}
