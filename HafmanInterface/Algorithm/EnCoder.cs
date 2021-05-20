using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace HafmanInterface.Algorithm
{
    class EnCoder
    {
        public byte[] Source { get; set; }
        private List<(byte Symbol, int Periodicity)> symbolWeight;
        public static List<(byte Symbol, List<bool> Code)> tableCodes;

        public EnCoder(byte[] Source)
        {
            this.Source = Source;
            symbolWeight = new List<(byte Symbol, int Periodicity)>();
            tableCodes = new List<(byte Symbol, List<bool> Code)>();
        }

        public EnCoder()
        {
            symbolWeight = new List<(byte Symbol, int Periodicity)>();
            tableCodes = new List<(byte Symbol, List<bool> Code)>();
        }

        public void LoadSource(string namefile)
        {
            byte[] array;
            using (FileStream fstream = new FileStream(namefile, FileMode.Open))
            {
                array = new byte[fstream.Length];
                fstream.Read(array, 0, array.Length);
            }

            Source = array;
        }

        /// <summary> установка таблицы веса символов </summary>
        /// <returns>суммарный вес</returns>
        private int SetsymbolWeight()
        {
            int sum = 0;
            bool b;
            foreach (byte sb in Source)
            {
                sum++;
                b = false;
                for (int i = 0; i < symbolWeight.Count; i++) //вообще есть any, но он возвращает bool
                {
                    if (symbolWeight[i].Symbol == sb) //нашли символ, надо просто увеличить его встречаемость
                    {
                        b = true;
                        symbolWeight[i] = (symbolWeight[i].Symbol, symbolWeight[i].Periodicity + 1);
                        break;
                    }
                }

                if (!b) symbolWeight.Add((sb, 1)); // не нашли символ
            }

            return sum;
        }

        /// <summary> строим таблицу кодов </summary>
        public void DoCoder()
        {
            int sum = SetsymbolWeight();
            TREES(symbolWeight, new List<bool>(), sum);
            Console.WriteLine("Это веса\n");
            foreach (var s in symbolWeight)
            {
                Console.WriteLine();
                Console.Write((char) s.Symbol + " " + s.Periodicity + " " + s.Symbol);
            }

            Console.WriteLine("Это коды\n");
            foreach (var s in tableCodes)
            {
                Console.WriteLine();
                Console.Write((char) s.Symbol + " ");
                foreach (bool b in s.Code)
                {
                    Console.Write(b ? "1" : "0");
                }
            }


            //TableToBites();
            
            SaveFileDialog win = new SaveFileDialog();
            if (win.ShowDialog() == true)
            {
                WriteFile(CodeToBites(), TableToBites(),win.FileName);
            }
           
        }

        private BitArray CodeToBites()
        {
            List<bool> boolbit = new List<bool>();
            foreach (byte c in Source)
            {
                foreach (var tc in tableCodes)
                {
                    if (tc.Symbol == c)
                    {
                        boolbit.AddRange(tc.Code);
                        break;
                    }
                }
            }

            //byte numLastBites = (byte) (boolbit.Count % 8);
            // BitArray lcode = new BitArray(new byte[] {numLastBites});
            //
            // BitArray ret = lcode.Append(new BitArray(boolbit.ToArray()));

            return new BitArray(boolbit.ToArray());
        }

        private BitArray TableToBites()
        {
            BitArray ba = new BitArray(new byte[] {(byte) (tableCodes.Count-1)});

            foreach (var tc in tableCodes)
            {
                var b = new BitArray(new byte[] {tc.Symbol});
                var c = new BitArray(new byte[] {(byte) tc.Code.Count});
                var d = new BitArray(tc.Code.ToArray());
                var summary = b.Cast<bool>().Concat(c.Cast<bool>()).Concat(tc.Code.ToArray())
                    .ToArray();
                BitArray CodeOneSym = new BitArray(summary);
                ba = ba.Append(CodeOneSym);
            }

            var bites = ba.Cast<bool>().ToArray();
            return ba;
        }

        private void WriteFile(BitArray cb, BitArray tb, string namefile)
        {
            BitArray summArray = tb.Append(cb);
            byte numLastBites = (byte) (summArray.Count % 8);
            BitArray lcode = new BitArray(new byte[] {numLastBites});
            summArray = lcode.Append(summArray);
            // byte[] codeBytes = new byte[cb.Count % 8 == 0 ? cb.Count / 8 : cb.Count / 8 + 1];
            // cb.CopyTo(codeBytes, 0);
            // byte[] tableBytes = new byte[tb.Count % 8 == 0 ? tb.Count / 8 : tb.Count / 8 + 1];
            // tb.CopyTo(tableBytes, 0);
            byte[] summBytes = new byte[summArray.Count % 8 == 0 ? summArray.Count / 8 : summArray.Count / 8 + 1];
            summArray.CopyTo(summBytes, 0);
            using (FileStream fstream = new FileStream(namefile, FileMode.Create))
            {
                // fstream.Write(tableBytes);
                // fstream.Write(codeBytes);
                fstream.Write(summBytes,0,summBytes.Length);
            }
        }

        private void TREE(List<(byte Symbol, int Periodicity)> sb, List<bool> code, int sum)
        {
            if (sb.Count == 1)
            {
                tableCodes.Add((sb[0].Symbol, code));
                return;
            }

            int halfsum = 0;
            var leftsb = new List<(byte Symbol, int Periodicity)>();
            var rightsb = new List<(byte Symbol, int Periodicity)>();
            for (int i = 0; i < sb.Count; i++)
            {
                if (halfsum + sb[i].Periodicity <= sum / 2)
                {
                    halfsum += sb[i].Periodicity;
                    leftsb.Add(sb[i]);
                }
                else
                {
                    var leftcode = code.ToList();
                    leftcode.Add(true);
                    if (leftsb.Count == 0)
                    {
                        leftsb.Add(sb[0]);
                        sb.RemoveAt(0);
                    }

                    TREE(leftsb, leftcode, halfsum);
                    for (int j = i; j < sb.Count; j++)
                    {
                        rightsb.Add(sb[j]);
                    }

                    var rightcode = code.ToList();
                    rightcode.Add(false);
                    TREE(rightsb, rightcode, sum - halfsum);
                    break;
                }
            }
        }

        private void TREES(List<(byte Symbol, int Periodicity)> sb, List<bool> code, int sum)
        {
            if (sb.Count == 1)
            {
                tableCodes.Add((sb[0].Symbol, code));
                return;
            }

            int halfsum = sum / 2;
            var leftsb = new List<(byte Symbol, int Periodicity)>();
            var rightsb = new List<(byte Symbol, int Periodicity)>();
            int leftsum = 0;
            int rightsum = 0;
            for (int i = 0; i < sb.Count; i++)
            {
                if (leftsum == 0 || (Math.Abs(halfsum - leftsum)  > Math.Abs(halfsum - (leftsum + sb[i].Periodicity))  && i!=sb.Count-1))
                {
                    leftsum += sb[i].Periodicity;
                    leftsb.Add(sb[i]);
                }
                else
                {
                    rightsum += sb[i].Periodicity;
                    rightsb.Add(sb[i]);
                }
            }
            var lcode = code.ToList();
            lcode.Add(true);
            TREES(leftsb, lcode, leftsum);
            var rcode = code.ToList();
            rcode.Add(false);
            TREES(rightsb,rcode,rightsum);

        }
    }
}