using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace tga2tex
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Enter file name.");
            }
            else if (File.Exists(args[0]))
            {
                FileStream fs = new FileStream(args[0], FileMode.Open);
                BinaryReader r = new BinaryReader(fs);
                fs.Seek(12, SeekOrigin.Begin);
                byte[] width = r.ReadBytes(2);
                byte[] hight = r.ReadBytes(2);
                FileInfo fi = new FileInfo(args[0]);
                //データサイズを取得
                long datasize = fi.Length-18;

                fs.Seek(18, SeekOrigin.Begin);
                //データを読み込むバイト型配列を作成する
                byte[] data = new byte[datasize];
                //データをすべて読み込む
                fs.Read(data, 0, data.Length);
                byte[] tex = new byte[56 + data.Length];
                //ヘッダにデータサイズ追加
                Array.Copy(BitConverter.GetBytes(datasize), 0,tex, 52, 4);
                int[] unk = new int[10] { 4, 65536, 1, 2, 2, 1, 0, 0, 0, 1};
                byte[] unkh = new byte[unk.Length * 4];
                byte[] unkt = new byte[4];
                for (int i = 0; i < 10; i++)
                {
                    unkt = BitConverter.GetBytes(unk[i]);
                    unkh[i * 4] = unkt[0];
                    unkh[i * 4 + 1] = unkt[1];
                    unkh[i * 4 + 2] = unkt[2];
                    unkh[i * 4 + 3] = unkt[3]; 
                }
                //残りのヘッダ追加
                Array.Copy(unkh, 0, tex, 12, 40);
                Array.Copy(BitConverter.GetBytes(datasize), 0, tex, 8, 4);
                int widthi = BitConverter.ToInt16(width, 0);
                int highti = BitConverter.ToInt16(hight, 0);
                Array.Copy(BitConverter.GetBytes(highti), 0, tex, 4, 4);
                Array.Copy(BitConverter.GetBytes(widthi), 0, tex, 0, 4);
                //データ追加
                Array.Copy(data, 0, tex, 56, data.Length);

                //閉じる
                fs.Close();

                string dir = Environment.CurrentDirectory;
                string baseName = Path.GetFileNameWithoutExtension(args[0]);
                FileStream fp = new FileStream(dir + "\\" + baseName + ".tex", FileMode.Create);
                BinaryWriter sw = new BinaryWriter(fp);
                for (int i = 0; i < tex.Length; i++)
                {
                    sw.Write(tex[i]);
                }
                //閉じる
                sw.Close();
                fs.Close();
            }
            else
            {
                Console.WriteLine("File not found.");
            }

        }
    }
}
