using System;
using System.Collections.Generic;
using System.Text;

namespace DavidHazi
{
	enum jatekmod
	{
		classical,
		horde
	}
	class Tabla
	{
		private static string sor = "+--+--+--+--+--+--+--+--+";
		public const int szelesseg = 8;
		private Babuk[,] mezo = new Babuk[8,8];
		private List<Babuk> dead = new List<Babuk>();
		public Babuk getMezo(int x,int y)
		{
			if (x > szelesseg || y > szelesseg || x < 1 || y < 1) throw new Exception("nem megfelelő koordináták");
			return mezo[x-1, y-1];
		}
		public void setMezo( int x, int y, Babuk babu = null)
		{
			if (x > szelesseg || y > szelesseg || x < 1 || y < 1) throw new Exception("nem megfelelő koordináták");
			mezo[x-1, y-1] = babu;
		}
		public Tabla()
		{

		}
		public Tabla(jatekmod jatekmod)
		{
			if(jatekmod == jatekmod.classical)
			{
				for (int i = 1; i <= szelesseg; i++)
				{
					setMezo(i, 2, new Pawn(szinek.EMBER,i,2));
					setMezo(i, 7, new Pawn(szinek.BLACK,i,7));
				}
			}
		}

		public void print()
		{
			for (int i = 1; i < 8; i++) {
				 Console.WriteLine(sor)
				;for (int h = 1; h < 8; h++) {
					 Console.Write("| ")
					; if (getMezo(i, h) != null)
					{
						Console.Write(getMezo(i, h).c)
					   ;
					}
					else 
					{
						 Console.Write(' ')
						;
					}
				}
				Console.WriteLine("|");
			}
			 Console.WriteLine(sor)
			;
		}
	}
}
