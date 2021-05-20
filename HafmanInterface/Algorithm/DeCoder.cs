using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace HafmanInterface.Algorithm
{
    public class DeCoder
    {
        public List<byte> Rezult { get; protected set; }
        public static List<(byte Symbol, List<bool> Code)> tableCodes;

        public DeCoder()
        {
            tableCodes = new List<(byte Symbol, List<bool> Code)>();
            Rezult = new List<byte>();
        }
        
        public void ReadFile(string filename)
        {
            byte[] array;
            using (FileStream fstream = new FileStream(filename,FileMode.Open))
            {
                 array = new byte[fstream.Length];
                fstream.Read(array, 0, array.Length);
                
            }
            ReadTable(array);
        }

        public void PrintRezult()
        {
            SaveFileDialog win = new SaveFileDialog();
            
            if (win.ShowDialog() == true)
            {
                using (FileStream fstream = new FileStream(win.FileName,FileMode.Create))
                {
                    fstream.Write(Rezult.ToArray(),0,Rezult.Count);
                }
            }

        }

        public void ReadTable(byte[] array)
        {
            byte numLastBites = array[0];
            byte delbites = numLastBites==0?(byte)0:(byte)(8 - numLastBites);
            int CountSymbols = array[1];
            BitArray ba = new BitArray(array);
            int handler = 16;
            
            for (int i = 0; i < CountSymbols; i++)          //декодер таблицы
            {
                BitArray symbit = new BitArray(8);
                for (int j = 0; j < 8; j++)
                {
                    symbit[j] = ba[handler++];
                }
                byte symbol = symbit.GetFirstByte();
                BitArray lengthbit = new BitArray(8);
                for (int j = 0; j < 8; j++)
                {
                    lengthbit[j] = ba[handler++];
                }

                byte length = lengthbit.GetFirstByte();
                
                BitArray codebit = new BitArray(length);
                for (int j = 0; j < length; j++)
                {
                    codebit[j] = ba[handler++];
                }

                var code = codebit.Cast<bool>().ToList();
                tableCodes.Add((symbol,code));
            }

            List<bool> CurrentSym = new List<bool>();
            while (handler<ba.Count-delbites)
            {
                CurrentSym.Add(ba[handler]);
                foreach (var sc in tableCodes)
                {
                    if (CurrentSym.SequenceEqual(sc.Code))  //нашли символ
                    {
                        Rezult.Add(sc.Symbol);
                        CurrentSym = new List<bool>();
                        break;
                    }
                }
                handler++;
            }
            
            
            
        }
        
    }
}