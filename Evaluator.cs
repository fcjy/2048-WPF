using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048_WPF
{
	public interface Evaluator
	{
		int evaluate(ref int[][] num);
	}

	public class Evaluator_0 : Evaluator
	{
		const int INF = (1 << 30);
		const int orderWeight = 2048;
		const int diffWeight = 8;
		const int nullGridWeight = 2048;
		const int largeNumWeight = 32;

		public int evaluate(ref int[][] num)
		{
			int N = num.Length, M = num[0].Length;
			int order = 0, diff = 0, nullGrid = 0, largeNum = 0;

			#region order
			for (int i = 0; i < N; ++i)
			{
				int A = -INF, B = INF, cnt = 0;
				for (int j = 0; j < M; ++j)
				{
					if (num[i][j] != 0)
					{
						if (num[i][j] < A)
							A = INF;
						else
							A = num[i][j];

						if (num[i][j] > B)
							B = -INF;
						else
							B = num[i][j];

						cnt++;
					}
				}
				if (cnt > 1 && (A != INF || B != -INF))
					order++;
			}

			for (int i = 0; i < M; ++i)
			{
				int A = -INF, B = INF, cnt = 0;
				for (int j = 0; j < N; ++j)
				{
					if (num[j][i] != 0)
					{
						if (num[j][i] < A)
							A = INF;
						else
							A = num[j][i];

						if (num[j][i] > B)
							B = -INF;
						else
							B = num[j][i];

						cnt++;
					}
				}
				if (cnt > 1 && (A != INF || B != -INF))
					order++;
			}
			#endregion

			#region diff
			for (int i = 0; i < N; ++i)
				for (int j = 0; j < M; ++j)
				{
					if (i + 1 < N && num[i + 1][j] != 0)
						diff += Math.Abs(num[i][j] - num[i + 1][j]);
					if (j + 1 < M && num[i][j + 1] != 0)
						diff += Math.Abs(num[i][j] - num[i][j + 1]);
				}
			#endregion

			#region nullGrid
			for (int i = 0; i < N; ++i)
				for (int j = 0; j < M; ++j)
					if (num[i][j] == 0)
						nullGrid++;
			#endregion

			#region largeNum
			for (int i = 0; i < 4; ++i)
				for (int j = 0; j < 4; ++j)
					largeNum = Math.Max(largeNum, num[i][j]);
			#endregion

			return order * orderWeight - diff * diffWeight + nullGrid * nullGridWeight + largeNum * largeNumWeight;
		}
	}
}