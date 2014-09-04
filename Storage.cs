using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace _2048_WPF
{
	static public class Storage
	{
		const string fileName = @"D:\2048.dat";

		static public void Save(int nowScore, int bestScore, ref int[][] num)
		{
			using (BinaryWriter bw = new BinaryWriter(File.Open(fileName, FileMode.Create)))
			{
				bw.Write(nowScore);
				bw.Write(bestScore);
				for (int i = 0; i < 4; ++i)
					for (int j = 0; j < 4; ++j)
						bw.Write(num[i][j]^19911002);
			}
		}

		static public bool Read(ref int nowScore, ref int bestScore, ref int[][] num)
		{
			if (File.Exists(fileName) == false)
				return false;

			using (BinaryReader br = new BinaryReader(File.Open(fileName, FileMode.Open)))
			{
				nowScore = br.ReadInt32();
				bestScore = br.ReadInt32();
				for (int i = 0; i < 4; ++i)
					for (int j = 0; j < 4; ++j)
						num[i][j] = (19911002^br.ReadInt32());
			}
			return true;
		}
	}
}
