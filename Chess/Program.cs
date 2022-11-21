using System;
using System.Collections.Generic;

/*-----------------------------------------Megjegyzések--------------------------------------------\\
|	>az elnevezések direkt ékezet mentes szavak, ezért érződnek sutának
\\-------------------------------------------------------------------------------------------------*/

/*--------------------------------------------Todos------------------------------------------------\\
|	>rájönni a dotnet cli-vel, hogy lehet több fájlban dolgozni
|	>horda
|	-nevek:
|		>"játékmód"
|		>"fehér"
|	>a klasszikus felállítás algoritmizálása
|	-spagetti:
|		>Torony::mozgat() kód duplikáció
|		>RohamOsztagos::mozgat() kód duplikáció
|		>a goto-t lehet jól használni; nekem, most kurvára nem sikerült
|	-lépések
|		>en passant
\\-------------------------------------------------------------------------------------------------*/

namespace Hazi{

#region progi
//###########################
//  ¤¤¤ Execution Entry ¤¤¤ 
//###########################
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
#endregion










#region enums\
//I^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^\
//I  _____                           \
//I |  ___|                          \
//I | |__ _ __  _   _ _ __ ___  ___  \
//I |  __| '_ \| | | | '_ ` _ \/ __| \
//I | |__| | | | |_| | | | | | \__ \ \
//I \____/_| |_|\__,_|_| |_| |_|___/ \
//I..................................I
	public enum MozgatErrno{
		FAIN,
		NULL_FIGURA,
		NULL_ASZTAL,
		SZEGELVE,
		BLOKKOLVA,
		TILOS,
		NEM_RUBLIKA,
		ROSSZ_SZIN
	}

	public enum Jatekmod{
		klasszikus,
		horda
	}

	public enum Szin{
		FEKETE = -1,
		FEHER = 1
	}

	public enum Helyszin{
		FEKETE,
		WHITE,
		SEMMILYEN
	}

#endregion










public class HelyInfo{
	public uint x;
	public uint y;
	public Asztal asztal;

	public HelyInfo(){}
	public HelyInfo(uint _x, uint _y, Asztal _asztal){
		this.x = _x;
		this.y = _y;
		this.asztal = _asztal;
	}
}
public class AtloInfo{
	private int xy;
	private int yx;
	public int XY{
		get{ return this.xy; }
	}
	public int YX{
		get{ return this.yx; }
	}
	public AtloInfo(uint x, uint y){
		this.Calc(x, y);
	}
	public void Calc(uint x, uint y){
		this.xy = (int)x - (int)y;
		Console.WriteLine("{0} - {1} -> {2}", x, y, this.xy);
		this.yx = (int)y + (int)x;
		Console.WriteLine("{0} + {1} -> {2}", y, x, this.yx);
	}
}







#region Figura
//I^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^\
//I ______ _                        \
//I |  ___(_)                       \
//I | |_   _  __ _ _   _ _ __ __ _  \
//I |  _| | |/ _` | | | | '__/ _` | \
//I | |   | | (_| | |_| | | | (_| | \
//I \_|   |_|\__, |\__,_|_|  \__,_| \
//I           __/ |                 \
//I          |___/                  \
//I.................................I
	public class Figura{
		//------------------------
		//    Belső változók
		//------------------------
		public static MozgatErrno mozgat_errno = MozgatErrno.FAIN;
		public bool vajon_szegelve;
		public bool vajon_asztalon;

		protected Szin szin;
		protected HelyInfo helyinfo;
		protected uint meritum;
		protected char jel;
			// ### Set-Get ###
				public uint X{
					get{
						if(!this.vajon_asztalon){ throw new NullReferenceException(""); }
						return this.helyinfo.x;
					}
					set{
						if(Asztal.TERMET < value) throw new ArgumentOutOfRangeException("");
						this.helyinfo.x = value;
					}
				}
				public uint Y{
					get{
						if(!this.vajon_asztalon){ throw new NullReferenceException(""); }
						return this.helyinfo.y;
					}
					set{
						if (Asztal.TERMET < value) throw new ArgumentOutOfRangeException("");
						this.helyinfo.y = value;
					}
				}
				public Asztal Asztal{
					get{ return this.helyinfo.asztal; }
					set{ this.helyinfo.asztal = value; }
				}
			public uint Meritum{
				get{ return this.meritum; }
			}
			public char Jel{
				get{ return this.jel; }
			}
			public Szin Szin{
				get{ return this.szin; }
			}
		//------------------------
		//    Belső eljárások
		//------------------------
			// ### Speciális ###
			 	// Constructor-ok
					public Figura(Szin _szin, HelyInfo _helyinfo = null,
									bool _vajon_szegelve = false){
						this.szin = _szin;
						this.helyinfo = new HelyInfo();
						if(_helyinfo == null){
							this.vajon_asztalon = false;
							this.vajon_szegelve = false;
						}else{
							if(_helyinfo.asztal == null){ throw new NullReferenceException(""); }
							this.X = _helyinfo.x;
							this.Y = _helyinfo.y;
							this.Asztal = _helyinfo.asztal;
							this.vajon_asztalon = true;
							this.vajon_szegelve = _vajon_szegelve;
						}
					}
			 // ### Állapot befolyásolók ###
			 	public bool vajon_mozgat(){
					if(!this.vajon_asztalon){ mozgat_errno = MozgatErrno.NULL_ASZTAL; return false; }
					if(this.vajon_szegelve){ mozgat_errno = MozgatErrno.SZEGELVE; return false; }
					return true;
				}
				public virtual bool mozgat(uint _x, uint _y){ return false; }
				protected void rak(uint _x, uint _y){
					this.Asztal[this.X, this.Y] = null;
					this.X = _x;
					this.Y = _y;
					this.Asztal[_x, _y] = this;
					mozgat_errno = MozgatErrno.FAIN;
				}
			// ### Állapot ellenőrzők ###
	}

	//  ___                                  _   
	// / __|_  _ ___ _ __  _ _ ___ _ __  ___| |__
	//| (_ | || / -_) '  \| '_/ -_) '  \/ -_) / /
	// \___|\_, \___|_|_|_|_| \___|_|_|_\___|_\_\
	//      |__/                           
		class Gyalog : Figura{
			private bool vajon_kezdett;
			public Gyalog(Szin _szin, HelyInfo _helyinfo = null,
							bool _vajon_szegelve = false, bool _vajon_kezdett = false)
								: base(_szin, _helyinfo, _vajon_szegelve){

				this.vajon_kezdett = _vajon_kezdett;
				this.meritum = 1;
				if(this.szin == Szin.FEKETE){
					this.jel = '♟';
				}else{
					this.jel = '♙';
				}
			}
			public override bool mozgat(uint _x, uint _y){
				if(_x == this.X){
					// Simpla
					if(_y == (this.Y + (int)this.szin)){
						if(this.Asztal[this.X, _y] != null){ mozgat_errno = MozgatErrno.BLOKKOLVA; return false; }
					}else
					// Dupla
					if( !this.vajon_kezdett && _y == (this.Y + (int)this.szin * 2) && _x == this.X ){
						if(this.Asztal[this.X, (uint)(this.Y + (int)this.szin)] != null || this.Asztal[this.X, _y] != null){
							mozgat_errno = MozgatErrno.BLOKKOLVA; return false;
						}
					}
					else{
						goto ILLEGAL;
					}
				}else
				// Ütés
				if( (_x == this.X - 1 || _x == this.X + 1) && _y == (this.Y + (int)this.szin)
							&& this.Asztal[_x, _y] != null && this.Asztal[_x, _y].Szin != this.Szin){
					this.Asztal.addHalottak(this.Asztal[_x, _y]);
				}else
				// En passant
				if(false){
				}else // Illegális
				{ goto ILLEGAL; }
				this.rak(_x, _y);
				this.vajon_kezdett = true;

				return true;

				ILLEGAL:
					mozgat_errno = MozgatErrno.TILOS;
					return false;
			}
		}


		class Paci : Figura{
			public Paci(Szin _szin, HelyInfo _helyinfo = null,
							bool _vajon_szegelve = false)
								: base(_szin,  _helyinfo, _vajon_szegelve){

				this.meritum = 3;
				if(this.szin == Szin.FEKETE){
					this.jel = '♞';
				}else{
					this.jel = '♘';
				}
			}
			public override bool mozgat(uint _x, uint _y){
				for(int i = 2; i >= -2; i = i - 4){
					//Console.WriteLine("i: {0}", i);
					if(_x == (this.X + i)){
						for(int h = 1; h >= -1; h = h - 2){
							//Console.WriteLine("\th: {0}", h);
							if(_y == (this.Y + h)){
								goto BREAK;
							}
						}
					}else if(_y == (this.Y + i)){
						for(int h = 1; h >= -1; h = h - 2){
							//Console.WriteLine("\th: {0}", h);
							//Console.WriteLine("{0} vs {1}", _x, this.X + h);
							if(_x == (this.X + h)){
								goto BREAK;
							}
						}
					}
				}
				mozgat_errno = MozgatErrno.TILOS;
				return false;
				BREAK:
					if(this.Asztal[_x, _y] != null){
						if(this.Asztal[_x, _y].Szin == this.szin){
							mozgat_errno = MozgatErrno.BLOKKOLVA;
							return false;
						}else{
							this.Asztal.addHalottak(this.Asztal[_x, _y]);
						}
					}
					this.rak(_x, _y);

					return true;
			}
		}


		class RohamOsztagos : Figura{
			private Helyszin helyszin;
			private AtloInfo atloinfo;
			public AtloInfo Atloinfo{
				get{ return this.atloinfo; }
			}
			public RohamOsztagos(Szin _szin, HelyInfo _helyinfo = null,
							bool _vajon_szegelve = false, Helyszin _helyszin = Helyszin.SEMMILYEN)
								: base(_szin, _helyinfo, _vajon_szegelve){

				this.helyszin = _helyszin;
				this.atloinfo = new AtloInfo(this.X, this.Y);
				this.meritum = 3;
				if(this.szin == Szin.FEKETE){
					this.jel = '♝';
				}else{
					this.jel = '♗';
				}
			}
			public override bool mozgat(uint _x, uint _y){
				AtloInfo arg_atloinfo = new AtloInfo(_x, _y);
				if(arg_atloinfo.XY == this.atloinfo.XY){
					int h = Math.Sign((int)_x - (int)this.X);
					Console.WriteLine("h: {0}", h);
					for(long i = 2*h; i != ((int)(arg_atloinfo.YX) - (int)(this.Atloinfo.YX)); i = i + 2*h){
						Console.WriteLine("i: {0} -> {1} (next is {2})", i, (((int)(arg_atloinfo.YX) - (int)(this.Atloinfo.YX)) + 2*2*h), i + 2*h);
						if(this.Asztal[(uint)(this.X + (i / 2)), (uint)(this.Y + (i / 2))] != null){
							mozgat_errno = MozgatErrno.BLOKKOLVA; return false; }
						}
						// Ütés
					if(this.Asztal[_x, _y] != null){
						if(this.Asztal[_x, _y].Szin != this.szin){
							this.Asztal.addHalottak(this.Asztal[_x, _y]);
						}else{
							mozgat_errno = MozgatErrno.BLOKKOLVA; return false;
						}
					}
				}else if(arg_atloinfo.YX == this.atloinfo.YX){
					int h = Math.Sign((int)_x - (int)this.X);
					Console.WriteLine("h: {0}", h);
					for(long i = 2*h; i != ((int)(arg_atloinfo.XY) - (int)(this.Atloinfo.XY)); i = i + 2*h){
						Console.WriteLine("i: {0} -> {1} (next is {2})", i, (int)(arg_atloinfo.XY) - (int)(this.Atloinfo.XY), i + 2*2*h);
						if(this.Asztal[(uint)(this.X + (i / 2)), (uint)(this.Y - (i / 2))] != null){
							mozgat_errno = MozgatErrno.BLOKKOLVA; return false; }
						}
						// Ütés
					if(this.Asztal[_x, _y] != null){
						if(this.Asztal[_x, _y].Szin != this.szin){
							this.Asztal.addHalottak(this.Asztal[_x, _y]);
						}else{
							mozgat_errno = MozgatErrno.BLOKKOLVA; return false;
						}
					}
				}else{
					mozgat_errno = MozgatErrno.TILOS;
					return false;
				}
				this.atloinfo.Calc(_x, _y);
				this.rak(_x, _y);

				return true;		
			}
		}


		class Torony : Figura{
			private bool vajon_kezdett;
			public Torony(Szin _szin, HelyInfo _helyinfo = null,
							bool _vajon_szegelve = false, bool _vajon_kezdett = false)
								: base(_szin, _helyinfo, _vajon_szegelve){

				this.vajon_kezdett = _vajon_kezdett;
				this.meritum = 1;
				if(this.szin == Szin.FEKETE){
					this.jel = '♜';
				}else{
					this.jel = '♖';
				}
			}
			public override bool mozgat(uint _x, uint _y){
				// Verikáls
				if(this.X == _x){
					int h = Math.Sign((int)_y - (int)this.Y);
					//Console.WriteLine("h: {0}", h);
					for(uint i = (uint)(this.Y + h); i != _y; i = (uint)(i + h)){
						//Console.WriteLine("i: {0} -> {1} (next is {2})", i, _y - h, i + h);
						if(this.Asztal[_x, i] != null){ mozgat_errno = MozgatErrno.BLOKKOLVA; return false; }
					}
					// Ütés
					if(this.Asztal[_x, _y] != null){
						if(this.Asztal[_x, _y].Szin != this.szin){
							this.Asztal.addHalottak(this.Asztal[_x, _y]);
						}else{
							mozgat_errno = MozgatErrno.BLOKKOLVA; return false;
						}
					}
				}
				// Horzontáls
				else if(this.Y == _y){
					int h = Math.Sign((int)_x - (int)this.X);
					//Console.WriteLine("h: {0}", h);
					for(uint i = (uint)(this.X + h); i != _x; i = (uint)(i + h)){
						//Console.WriteLine("i: {0} -> {1} (next is {2})", i, _x - h, i + h);
						if(this.Asztal[i, _y] != null){ mozgat_errno = MozgatErrno.BLOKKOLVA; return false; }
					}
					// Ütés
					if(this.Asztal[_x, _y] != null){
						if(this.Asztal[_x, _y].Szin != this.szin){
							this.Asztal.addHalottak(this.Asztal[_x, _y]);
						}else{
							mozgat_errno = MozgatErrno.BLOKKOLVA; return false;
						}
					}
				}else
				// Illegális
				{
					mozgat_errno = MozgatErrno.TILOS;
					return false;
				}
				this.rak(_x, _y);
				this.vajon_kezdett = true;

				return true;		
			}
		}


		class Parancsnok : Figura{
			private AtloInfo atloinfo;
			public AtloInfo Atloinfo{
				get{ return this.atloinfo; }
			}
			public Parancsnok(Szin _szin, HelyInfo _helyinfo,
							bool _vajon_szegelve = false)
								: base(_szin, _helyinfo, _vajon_szegelve){
				this.atloinfo = new AtloInfo(this.X, this.Y);
				this.meritum = 9;
				if(this.szin == Szin.FEKETE){
					this.jel = '♛';
				}else{
					this.jel = '♕';
				}
			}
			public override bool mozgat(uint _x, uint _y){
				AtloInfo arg_atloinfo = new AtloInfo(_x, _y);
				// Verikáls
				if(this.X == _x){
					int h = Math.Sign((int)_y - (int)this.Y);
					Console.WriteLine("h: {0}", h);
					for(uint i = (uint)(this.Y + h); i != _y; i = (uint)(i + h)){
						Console.WriteLine("i: {0} -> {1} (next is {2})", i, _y - h, i + h);
						if(this.Asztal[_x, i] != null){ mozgat_errno = MozgatErrno.BLOKKOLVA; return false; }
					}
					// Ütés
					if(this.Asztal[_x, _y] != null){
						if(this.Asztal[_x, _y].Szin != this.szin){
							this.Asztal.addHalottak(this.Asztal[_x, _y]);
						}else{
							mozgat_errno = MozgatErrno.BLOKKOLVA; return false;
						}
					}
				}
				// Horzontáls
				else if(this.Y == _y){
					int h = Math.Sign((int)_x - (int)this.X);
					//Console.WriteLine("h: {0}", h);
					for(uint i = (uint)(this.X + h); i != _x; i = (uint)(i + h)){
						//Console.WriteLine("i: {0} -> {1} (next is {2})", i, _x - h, i + h);
						if(this.Asztal[i, _y] != null){ mozgat_errno = MozgatErrno.BLOKKOLVA; return false; }
					}
					// Ütés
					if(this.Asztal[_x, _y] != null){
						if(this.Asztal[_x, _y].Szin != this.szin){
							this.Asztal.addHalottak(this.Asztal[_x, _y]);
						}else{
							mozgat_errno = MozgatErrno.BLOKKOLVA; return false;
						}
					}
				}
				// Átló 1
				else if(arg_atloinfo.XY == this.atloinfo.XY){
					int h = Math.Sign((int)_x - (int)this.X);
					Console.WriteLine("h: {0}", h);
					for(long i = 2*h; i != ((int)(arg_atloinfo.YX) - (int)(this.Atloinfo.YX)); i = i + 2*h){
						Console.WriteLine("i: {0} -> {1} (next is {2})", i, (((int)(arg_atloinfo.YX) - (int)(this.Atloinfo.YX)) + 2*2*h), i + 2*h);
						if(this.Asztal[(uint)(this.X + (i / 2)), (uint)(this.Y + (i / 2))] != null){
							mozgat_errno = MozgatErrno.BLOKKOLVA; return false; }
						}
						// Ütés
					if(this.Asztal[_x, _y] != null){
						if(this.Asztal[_x, _y].Szin != this.szin){
							this.Asztal.addHalottak(this.Asztal[_x, _y]);
						}else{
							mozgat_errno = MozgatErrno.BLOKKOLVA; return false;
						}
					}
				}
				// Átló 2
				else if(arg_atloinfo.YX == this.atloinfo.YX){
					int h = Math.Sign((int)_x - (int)this.X);
					Console.WriteLine("h: {0}", h);
					for(long i = 2*h; i != ((int)(arg_atloinfo.XY) - (int)(this.Atloinfo.XY)); i = i + 2*h){
						Console.WriteLine("i: {0} -> {1} (next is {2})", i, (int)(arg_atloinfo.XY) - (int)(this.Atloinfo.XY), i + 2*2*h);
						if(this.Asztal[(uint)(this.X + (i / 2)), (uint)(this.Y - (i / 2))] != null){
							mozgat_errno = MozgatErrno.BLOKKOLVA; return false; }
						}
						// Ütés
					if(this.Asztal[_x, _y] != null){
						if(this.Asztal[_x, _y].Szin != this.szin){
							this.Asztal.addHalottak(this.Asztal[_x, _y]);
						}else{
							mozgat_errno = MozgatErrno.BLOKKOLVA; return false;
						}
					}
				// Illegális
				}else{
					mozgat_errno = MozgatErrno.TILOS;
					return false;
				}
				Console.WriteLine("Waldo");
				this.atloinfo.Calc(_x, _y);
				this.rak(_x, _y);

				return true;		
			}
		}


		class Fejedelem : Figura{
			private bool vajon_kezdett;
			private bool sakk;
			public Fejedelem(Szin _szin, HelyInfo _helyinfo = null,
							bool _vajon_szegelve = false, bool _vajon_kezdett = false, bool _sakk = false)
								: base(_szin, _helyinfo, _vajon_szegelve){
				this.vajon_kezdett = _vajon_kezdett;
				this.sakk = _sakk;
				this.meritum = 10;
				if(this.szin == Szin.FEKETE){
					this.jel = '♚';
				}else{
					this.jel = '♔';
				}
			}
			public override bool mozgat(uint _x, uint _y){
				if( _x >= (this.X - 1) && _x <= (this.X + 1)
					&& _y >= (this.Y - 1) && _y <= (this.Y + 1)){
					if(this.Asztal[_x, _y] != null){
						if(this.Asztal[_x, _y].Szin == this.szin){
							mozgat_errno = MozgatErrno.BLOKKOLVA;
							return false;
						}else{
							this.Asztal.addHalottak(this.Asztal[_x, _y]);
						}
					}
				}else{
					mozgat_errno = MozgatErrno.TILOS;
					return false;
				}
				this.rak(_x, _y);
				this.vajon_kezdett = true;

				return true;		
			}
		}
#endregion








#region Asztal\
//I^^^^^^^^^^^^^^^^^^^^^^^^^^^^\
//I   ___          _        _  \
//I  / _ \        | |      | | \
//I / /_\ \___ ___| |_ __ _| | \
//I |  _  / __|_  / __/ _` | | \
//I | | | \__ \/ /| || (_| | | \
//I \_| |_/___/___|\__\__,_|_| \
//I............................I
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

#endregion

}

