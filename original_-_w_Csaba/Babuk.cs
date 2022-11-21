using System;
using System.Collections.Generic;
using System.Text;

namespace DavidHazi
{
	enum szinek
	{
		BLACK,
		EMBER
	}
	enum helyszinek
	{
		FEKETE,
		WHITE,
		VUOTA
	}
	class Babuk
	{
		private szinek szin;
		private int x;
		private int y;
		public bool isPinned;
		private int value;
		public bool onBoard;
		public char c;
		public Babuk(szinek szin, int x, int y , bool isPinned = false, bool onBoard = false)
		{
			this.szin = szin;
			X = x;
			Y = y;
			this.isPinned = isPinned;
			this.onBoard = onBoard;

		}
		public int X
		{
			get
			{
				return x;
			}
			set
			{
				if (x < 1 || x > 8) throw new Exception("ez szar lesz öcsi");
				x = value;
			}
		}
		public int Y
		{
			get
			{
				return y;
			}
			set
			{
				if (y < 1 || y > 8) throw new Exception("ez szar lesz öcsi");
				y = value;
			}
		}
	}
	class Pawn : Babuk
	{
		private bool firstMove;
		public Pawn(szinek szin, int x, int y, bool isPinned = false, bool onBoard = false, bool firstMove = false) : base(szin, x, y, isPinned, onBoard = false)
		{
			this.firstMove = firstMove;
			c = 'P';
		}
	}
	class Rook : Babuk
	{
		private bool firstMove;
		public Rook(szinek szin, int x, int y, bool isPinned = false, bool onBoard = false, bool firstMove = false) : base(szin, x, y, isPinned, onBoard = false)
		{
			this.firstMove = firstMove;
			c = 'R';
		}
	}
	class Queen : Babuk
	{
		public Queen(szinek szin, int x, int y, bool isPinned = false, bool onBoard = false) : base(szin, x, y, isPinned, onBoard = false)
		{
			c = 'Q';
		}
	}
	class Bishop : Babuk
	{
		private helyszinek helyszin;
		public Bishop(szinek szin, int x, int y, bool isPinned = false, bool onBoard = false, helyszinek helyszinek = helyszinek.VUOTA) : base(szin, x, y, isPinned, onBoard = false)
		{
			this.helyszin = helyszinek;
			c = 'B';
		}
	}
	class Knight : Babuk
	{
		public Knight(szinek szin, int x, int y, bool isPinned = false, bool onBoard = false) : base(szin, x, y, isPinned, onBoard = false)
		{
			c = 'k';
		}
	}
	class King : Babuk
	{
		private bool firstMove;
		private bool sakk;
		public King(szinek szin, int x, int y, bool isPinned = false, bool onBoard = false, bool firstMove = false,bool sakk = false) : base(szin, x, y, isPinned, onBoard = false)
		{
			this.firstMove = firstMove;
			this.sakk = sakk;
			c = 'K';
		}
	}
}
