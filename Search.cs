using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048_WPF
{
	public class Search
	{
		private int[] dx = { -1, 0, 1, 0 };
		private int[] dy = { 0, 1, 0, -1 };
		private int[] startx = { 0, 0, 3, 3 };
		private int[] starty = { 0, 3, 3, 0 };
		int NodeCnt, MaxNodeCnt, MaxSearchDepth;
		Evaluator Eva;

		const int INF = (1 << 30);

		public Search( int maxNodeCnt, Evaluator eva )
		{
			MaxNodeCnt = maxNodeCnt;
			Eva = eva;
		}

		private int GridCnt(ref int[][] num)
		{
			int cnt = 0;
			for (int i = 0; i < 4; ++i)
				for (int j = 0; j < 4; ++j) if (num[i][j] != 0)
						cnt++;
			return cnt;
		}

		private bool Move(ref int[][] num1, ref int[][] num2, int dir)
		{
			int rd = (dir + 1) % 4;
			int nx, ny;
			bool islegal = false;

			nx = startx[dir]; ny = starty[dir];
			for (int i = 0; i < 4; ++i)
			{
				List<int> numList = new List<int>();
				List<int> newNumList = new List<int>();
				for (int j = 0; j < 4; ++j)
				{
					int n = num1[nx - j * dx[dir]][ny - j * dy[dir]];
					if (n > 0)
						numList.Add(n);
					else if (j + 1 < 4 && num1[nx - (j + 1) * dx[dir]][ny - (j + 1) * dy[dir]] > 0)
						islegal = true;
				}
				for (int j = 1; j <= numList.Count; ++j)
				{
					if (j < numList.Count && numList[j - 1] == numList[j])
					{
						if (numList[j] == 512)
							MaxSearchDepth = 4;

						islegal = true;
						newNumList.Add(numList[j] * 2);
						j++;
					}
					else
						newNumList.Add(numList[j - 1]);
				}
				for (int j = 0; j < 4; ++j)
					num2[nx - j * dx[dir]][ny - j * dy[dir]] = 0;
				for (int j = 0; j < newNumList.Count; ++j)
					num2[nx - j * dx[dir]][ny - j * dy[dir]] = newNumList[j];
				nx += dx[rd]; ny += dy[rd];
			}

			return islegal;
		}

		private int AlphaBetaMax(ref int[][] num, int preMinVal, int depth)
		{
			if (depth > MaxSearchDepth )
				return INF;
			NodeCnt++;

			//int maxVal = Eva.evaluate(ref num);
			int maxVal = depth == MaxSearchDepth ? Eva.evaluate(ref num) : -INF;
			int dir = -1;
			if (maxVal >= preMinVal)
				return INF;

			int[][] movedNum = new int[4][];
			for (int i = 0; i < 4; ++i)
				movedNum[i] = new int[4];

			for (int i = 0; i < 4; ++i)	if(NodeCnt <= MaxNodeCnt)
			{
				if (Move(ref num, ref movedNum, i))
				{
					if (dir == -1)
						dir = i;
					int tmp = AlphaBetaMin(ref movedNum, maxVal, depth);
					if (tmp > maxVal)
					{
						maxVal = tmp;
						dir = i;
					}

					if (maxVal >= preMinVal)
						return INF;
				}
			}

			return depth == 0 ? dir : maxVal;
		}

		private int AlphaBetaMin(ref int[][] num, int preMaxVal, int depth)
		{
			if (depth >= MaxSearchDepth)
				return -INF;

			int minVal = INF;

			for (int i = 0; i < 4; ++i)
			{
				for (int j = 0; j < 4; ++j) if (num[i][j] == 0)
					{
						num[i][j] = 2;
						minVal = Math.Min(minVal, AlphaBetaMax(ref num, minVal, depth + 1));
						if (minVal <= preMaxVal)
							return -INF;
						num[i][j] = 0;
					}
			}

			for (int i = 0; i < 4; ++i)
			{
				for (int j = 0; j < 4; ++j) if (num[i][j] == 0)
					{
						num[i][j] = 4;
						minVal = Math.Min(minVal, AlphaBetaMax(ref num, minVal, depth + 1));
						if (minVal <= preMaxVal)
							return -INF;
						num[i][j] = 0;
					}
			}

			return minVal;
		}

		public int AlphaBeta(ref int[][] num)
		{
			MaxSearchDepth = GridCnt(ref num) > 12 ? 4 : 3;
			NodeCnt = 0;
			return AlphaBetaMax(ref num, INF, 0);
		}
	}
}
