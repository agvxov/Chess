using System;
using System.Collections.Generic;

public class Asztal{
	//------------------------
	//    Belső változók
	//------------------------
	public const uint TERMET = 8;
	private const string VKOCKA_ELEM = "+---";
	private const string HKOCKA_ELEM = "| ";
	private const string VKOCKA_DUGASZ = "+";
	private const string HKOCKA_DUGASZ = "|";

	private Jatekmod jatekmod;
	private Figura[,] rublika = new Figura[TERMET, TERMET];
	private List<Figura> halottak = new List<Figura>();
	private Szin mozgatgat = Szin.FEHER;

		// ### Set-Get ###
			public Jatekmod Jatekmod{
				get{ return this.jatekmod; }
				set{ this.jatekmod = value; }
			}
			public Figura this[uint _x, uint _y]{
				get {
					if(_x > TERMET || _y > TERMET){ throw new ArgumentOutOfRangeException(""); }
					return rublika[_x-1, _y-1];
				}
				set {
				if(_x > TERMET || _y > TERMET || _x == 0 || _y == 0) throw new ArgumentOutOfRangeException("");
				rublika[_x-1, _y-1] = value;
				
				}
			}
			public void addHalottak(Figura figura){
				this.halottak.Add(figura);
			}
			public void clearHalottak(){
				this.halottak.Clear();
			}

	//------------------------
	//    Belső eljárások
	//------------------------
		// ### Speciális ###
		 	// Constructor-ok
				public Asztal(){}
				public Asztal(Jatekmod _jatekmod){
					this.jatekmod = _jatekmod;
					if(this.jatekmod == Jatekmod.klasszikus){
						for(uint i = 1; i <= TERMET; i++){
							this[i, 2] =  new Gyalog(Szin.FEHER, new HelyInfo(i, 2, this));
							this[i, 7] =  new Gyalog(Szin.FEKETE, new HelyInfo(i, 7, this));
						}
						this[1, 1] = new Torony(Szin.FEHER, new HelyInfo(1, 1, this));
						this[8, 1] = new Torony(Szin.FEHER, new HelyInfo(8, 1, this));
						this[1, 8] = new Torony(Szin.FEKETE, new HelyInfo(1, 8, this));
						this[8, 8] = new Torony(Szin.FEKETE, new HelyInfo(8, 8, this));
						this[2, 1] = new Paci(Szin.FEHER, new HelyInfo(2, 1, this));
						this[7, 1] = new Paci(Szin.FEHER, new HelyInfo(7, 1, this));
						this[2, 8] = new Paci(Szin.FEKETE, new HelyInfo(2, 8, this));
						this[7, 8] = new Paci(Szin.FEKETE, new HelyInfo(7, 8, this));
						this[3, 1] = new RohamOsztagos(Szin.FEHER, new HelyInfo(3, 1, this));
						this[6, 1] = new RohamOsztagos(Szin.FEHER, new HelyInfo(6, 1, this));
						this[3, 8] = new RohamOsztagos(Szin.FEKETE, new HelyInfo(3, 8, this));
						this[6, 8] = new RohamOsztagos(Szin.FEKETE, new HelyInfo(6, 8, this));
						this[4, 1] = new Parancsnok(Szin.FEHER, new HelyInfo(4, 1, this));
						this[4, 8] = new Parancsnok(Szin.FEKETE, new HelyInfo(5, 8, this));
						this[5, 1] = new Fejedelem(Szin.FEHER, new HelyInfo(5, 1, this));
						this[5, 8] = new Fejedelem(Szin.FEKETE, new HelyInfo(4, 8, this));
					}
				}
		// ### Állapot befolyásolók ###
			private bool _mozgat(uint _x1, uint _y1, uint _x2, uint _y2){
				// -ből határon belül?
				if(_x1 > TERMET || _y1 > TERMET || _x1 == 0 || _y1 == 0){
					Figura.mozgat_errno = MozgatErrno.NEM_RUBLIKA; return false;
				}
				// létező figura?
				if(this[_x1, _y1] == null){ Figura.mozgat_errno = MozgatErrno.NULL_FIGURA; return false; }
				// jó szin?
				if(this[_x1, _y1].Szin != this.mozgatgat){ Figura.mozgat_errno = MozgatErrno.ROSSZ_SZIN; return false; }
				// -be határon belül?
				if(_x2 > TERMET || _y2 > TERMET || _x2 == 0 || _y2 == 0){
					Figura.mozgat_errno = MozgatErrno.NEM_RUBLIKA; return false;
				}
				// mozoghat?
				if(!this[_x1, _y1].vajon_mozgat()){ return false; }

				return this[_x1, _y1].mozgat(_x2, _y2);
			}
			public void mozgat(){

				INPUT:
					Console.Write(": ");
					string l = Console.ReadLine();
					if(l == "pass"){ goto TURN_TURNS; }
					string[] k = l.Split("->");
					foreach(var i in k){
						if(i.Length < 2){ goto INPUT; }
					}
				uint x1, y1, x2, y2;
				x1 = (uint)(k[0][0] - 96);
				x2 = (uint)(k[1][0] - 96);
				y1 = (uint)(k[0][1] - 48);
				y2 = (uint)(k[1][1] - 48);
				//Console.WriteLine("{0}, {1};  {2}, {3}", x1, y1, x2, y2);
				
				if(!this._mozgat(x1, y1, x2, y2)){
					return;
				}

				TURN_TURNS:
					if(this.mozgatgat == Szin.FEHER){
						this.mozgatgat = Szin.FEKETE;
					}else{
						this.mozgatgat = Szin.FEHER;
					}
			}
		// ### Állapot ellenőrzők ###
			public void print(){
				Console.WriteLine("\n\n");
				// --- Tábla és bábúk ---
				for(uint i = Asztal.TERMET; i >= 1; i--){

					Console.Write("   ");
					for(uint h = 1; h <= Asztal.TERMET; h++){
						Console.Write(Asztal.VKOCKA_ELEM);
					}
					Console.WriteLine(Asztal.VKOCKA_DUGASZ);

					Console.Write(" " + i + " ");
					for(uint h = 1; h <= Asztal.TERMET; h++){
						Console.Write(Asztal.HKOCKA_ELEM);
						if(this[h, i] != null){
							Console.Write(this[h, i].Jel);
						}else{
							Console.Write(' ');
						}
						Console.Write(' ');
					}
					Console.WriteLine(Asztal.HKOCKA_DUGASZ);

				}
				Console.Write("   ");
				for(uint h = 1; h <= Asztal.TERMET; h++){
					Console.Write(Asztal.VKOCKA_ELEM);
				}
				Console.WriteLine(Asztal.VKOCKA_DUGASZ);
				Console.Write("   ");
				for(uint i = 0; i < Asztal.TERMET; i++){
					Console.Write("  " + (char)('a'+i) + " " );
				}
				Console.WriteLine();

				// --- Leütött bábúk ---
				Console.Write("\x1b[1mLeütött bábúk: \x1b[0m");
				foreach(var i in this.halottak){
					Console.Write(" " + i.Jel + ',');
				}
				Console.WriteLine();

				// --- Lépés állapot ---
				Console.Write("\x1b[1mSoron következő játékos: \x1b[0m");
				if(this.mozgatgat == Szin.FEHER){
					Console.Write("□ ");
				}else{
					Console.Write("■ ");
				}
				Console.WriteLine("\n\x1b[1mUtolsó lépés legalitása: \x1b[0m" + Figura.mozgat_errno);
			}
}
