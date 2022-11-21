using System;

#region figura
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
#endregion

#region gyalog
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
#endregion

#region paci
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
#endregion

#region rohamosztagos
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
#endregion

#region torony
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
#endregion

#region parancsnok
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
#endregion

#region fejedelem
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
