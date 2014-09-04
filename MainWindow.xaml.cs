using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO;

namespace _2048_WPF
{
	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MainWindow : Window
	{
		const int INF = (1 << 30);
		private int[] dx = { -1, 0, 1, 0 };
		private int[] dy = { 0, 1, 0, -1 };
		private int[] startx = { 0, 0, 3, 3 };
		private int[] starty = { 0, 3, 3, 0 };
		private Brush norNumCol = new SolidColorBrush(Color.FromRgb(0, 0, 0));
		private Brush newNumCol = new SolidColorBrush(Color.FromRgb(34, 139, 34));

		private int[][] num;
		private TextBlock[][] numGrid;
		private Random rnd;
		private bool isEnd, isAuto;
		private int nowScore, bestScore;
		private int prePos;

		public MainWindow()
		{
			InitializeComponent();
			Initialize2048();
		}

		private void Initialize2048()
		{
			num = new int[4][];
			for (int i = 0; i < 4; ++i)
				num[i] = new int[4];

			numGrid = new TextBlock[4][];
			for (int i = 0; i < 4; ++i)
				numGrid[i] = new TextBlock[4];

			numGrid[0][0] = this.number_grid_0_0;
			numGrid[0][1] = this.number_grid_0_1;
			numGrid[0][2] = this.number_grid_0_2;
			numGrid[0][3] = this.number_grid_0_3;
			numGrid[1][0] = this.number_grid_1_0;
			numGrid[1][1] = this.number_grid_1_1;
			numGrid[1][2] = this.number_grid_1_2;
			numGrid[1][3] = this.number_grid_1_3;
			numGrid[2][0] = this.number_grid_2_0;
			numGrid[2][1] = this.number_grid_2_1;
			numGrid[2][2] = this.number_grid_2_2;
			numGrid[2][3] = this.number_grid_2_3;
			numGrid[3][0] = this.number_grid_3_0;
			numGrid[3][1] = this.number_grid_3_1;
			numGrid[3][2] = this.number_grid_3_2;
			numGrid[3][3] = this.number_grid_3_3;

			for (int i = 0; i < 4; ++i)
				for (int j = 0; j < 4; ++j)
				{
					num[i][j] = 0;
					numGrid[i][j].Text = "";
				}

			rnd = new Random((int)(DateTime.Now.Ticks % 19911002));

			if (Storage.Read(ref nowScore, ref bestScore, ref num))
			{
				now_score.Text = nowScore.ToString();
				best_score.Text = bestScore.ToString();
				for (int i = 0; i < 4; ++i)
					for (int j = 0; j < 4; ++j)
					{
						if (num[i][j] > 0)
							numGrid[i][j].Text = num[i][j].ToString();
						else
							numGrid[i][j].Text = "";
					}
			}
			else
			{
				pushNum(false);
				pushNum(false);
			}

			isAuto = false;
			isEnd = false;
			CheckEnd();
			prePos = -1;
		}

		private void setNumber(int x, int y, int number)
		{
			num[x][y] = number;
			if (number == 0)
				numGrid[x][y].Text = "";
			else
				numGrid[x][y].Text = number.ToString();
		}

		private int getNewNumber()
		{
			if (rnd.Next(10) > 0)
				return 2;
			return 4;
		}

		private KeyValuePair<int, int> getRandomEmptyGrid()
		{
			int cnt = 0, k;
			for (int i = 0; i < 4; ++i)
				for (int j = 0; j < 4; ++j) if (numGrid[i][j].Text == "")
						cnt++;
			k = rnd.Next(cnt);
			for (int i = 0; i < 4; ++i)
				for (int j = 0; j < 4; ++j) if (numGrid[i][j].Text == "")
					{
						if (k == 0)
							return new KeyValuePair<int, int>(i, j);
						k--;
					}
			return new KeyValuePair<int, int>(-1, -1);
		}

		private void CheckEnd()
		{
			bool end = true;
			for (int i = 0; end && i < 4; ++i)
				for (int j = 0; end && j < 4; ++j)
				{
					if (num[i][j] == 0)
						end = false;
					else 
					{
						if (i + 1 < 4 && num[i + 1][j] == num[i][j])
							end = false;
						if (j + 1 < 4 && num[i][j + 1] == num[i][j])
							end = false;
					}
				}

			if (end)
				isEnd = true;
		}

		private void pushNum( bool mark = true )
		{
			KeyValuePair<int, int> pos;
			pos = getRandomEmptyGrid();
			if( mark )
				numGrid[pos.Key][pos.Value].Foreground = newNumCol;
			prePos = pos.Key * 100 + pos.Value;
			setNumber(pos.Key, pos.Value, getNewNumber());
		}

		private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.W || e.Key == Key.Up)
				Move(0);
			else if (e.Key == Key.D || e.Key == Key.Right)
				Move(1);
			else if (e.Key == Key.S || e.Key == Key.Down)
				Move(2);
			else if (e.Key == Key.A || e.Key == Key.Left)
				Move(3);
		}

		private void AddScore(int x)
		{
			nowScore += x;
			now_score.Text = nowScore.ToString();
			if (nowScore > bestScore)
			{
				bestScore = nowScore;
				best_score.Text = bestScore.ToString();
			}
		}

		private bool Move(int dir)
		{
			if( prePos != -1 )
				numGrid[prePos / 100][prePos % 100].Foreground = norNumCol;

			int rd = (dir + 1) % 4;
			int nx, ny, sc = 0;
			bool islegal = false;

			nx = startx[dir]; ny = starty[dir];
			for (int i = 0; i < 4; ++i)
			{
				List<int> numList = new List<int>();
				List<int> newNumList = new List<int>();
				for (int j = 0; j < 4; ++j)
				{
					int n = num[nx - j * dx[dir]][ny - j * dy[dir]];
					if (n > 0 )
					{
						numList.Add(n);
						setNumber(nx - j * dx[dir], ny - j * dy[dir], 0);
					}
					else if (j + 1 < 4 && num[nx - (j + 1) * dx[dir]][ny - (j + 1) * dy[dir]] > 0)
						islegal = true;
				}
				for (int j = 1; j <= numList.Count; ++j)
				{
					if (j < numList.Count && numList[j - 1] == numList[j])
					{
						sc += numList[j] * 2;
						newNumList.Add(numList[j] * 2);
						j++;
					}
					else
						newNumList.Add(numList[j - 1]);
				}
				for (int j = 0; j < newNumList.Count; ++j)
					setNumber(nx - j * dx[dir], ny - j * dy[dir], newNumList[j]);
				nx += dx[rd]; ny += dy[rd];
			}

			if (sc > 0)
				islegal = true;
			AddScore(sc);

			if (islegal)
			{
				pushNum();
				Storage.Save(nowScore, bestScore, ref num);
			}
			else
				prePos = -1;

			Evaluator_0 eva = new Evaluator_0();
			best_score.Text = eva.evaluate(ref num).ToString();

			CheckEnd();
			return islegal;
		}

		private int LargeNum()
		{
			int res = 0;
			for (int i = 0; i < 4; ++i)
				for (int j = 0; j < 4; ++j)
					res = Math.Max(res, num[i][j]);
			return res;
		}

		private void new_button_Click(object sender, RoutedEventArgs e)
		{
			nowScore = 0;
			now_score.Text = nowScore.ToString();
			
			for( int i = 0; i < 4; ++i )
				for( int j = 0; j < 4; ++j )
				{
					num[i][j] = 0;
					numGrid[i][j].Text = "";
				}

			pushNum(false);
			pushNum(false);

			isAuto = false;
			isEnd = false;
			prePos = -1;
		}

		private void auto_button_Click(object sender, RoutedEventArgs e)
		{
			 if( isAuto )
			 {
				 isAuto = false;
				 auto_button.Content = "ATUO";
			 }
			 else
			 {
				 isAuto = true;
				 auto_button.Content = "STOP";

				 Evaluator_0 eva = new Evaluator_0();
				 Search sea = new Search(100000, eva);

				 while( isAuto && !isEnd )
				 {
					 int dir = sea.AlphaBeta(ref num);
					 //int dir = rnd.Next(4);
					 Move(dir);
					 System.Windows.Forms.Application.DoEvents();
				 }

				 isAuto = false;
				 auto_button.Content = "ATUO";
			 }
		}

		private void test_button_Click(object sender, RoutedEventArgs e)
		{
			string fileName = DateTime.Now.ToString("yyyyMMdd_HH_mm_ss") + ".txt";
			string path = @"D:\2048test\" + fileName;

			using (StreamWriter writer = new StreamWriter(path))
			{
				int sum = 0, times = 20;

				for (int tim = 0; tim < times; ++tim)
				{
					nowScore = 0;
					now_score.Text = nowScore.ToString();

					for (int i = 0; i < 4; ++i)
						for (int j = 0; j < 4; ++j)
						{
							num[i][j] = 0;
							numGrid[i][j].Text = "";
						}

					pushNum(false);
					pushNum(false);

					isEnd = false;
					prePos = -1;

					Evaluator_0 eva = new Evaluator_0();
					Search sea = new Search(300000, eva);

					while ( !isEnd)
					{
						int dir = sea.AlphaBeta(ref num);
						//int dir = rnd.Next(4);
						Move(dir);
						System.Windows.Forms.Application.DoEvents();
					}

					writer.Write(LargeNum() + "\t" + nowScore + "\n");
					writer.Flush();

					sum += nowScore;
				}

				writer.WriteLine(sum / times);
			}
		}
	}
}