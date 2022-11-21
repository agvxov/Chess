using System;


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
