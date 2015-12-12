using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugFeature
{
    public static class Program
    {
        static string[] lines;
        static string path = @"Input.txt";
        static List<MyData> KeepData = new List<MyData>();
        static List<int> TimeStampData = new List<int>();

        static void Main(string[] args)
        {
            bool inputCond = true;
            int i = 0;
            int bugnoline=0, numpatch=0;
            char[] delimiter = { ' ' }; 
            readData();
            do
            {
                KeepData = new List<MyData>();
                inputCond = true;
                string item = lines[i];
                string[] splitdata = item.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                //it is a number of bug in line and patch
                if (splitdata.Length == 2)
                {
                    Console.WriteLine(lines[i]);
                    bugnoline = 0;
                    numpatch = 0;
                    if (!int.TryParse(splitdata[0], out bugnoline) || !int.TryParse(splitdata[1], out numpatch))
                    {
                        Console.WriteLine("Input data is not a number");
                        break;
                    }

                }
                else if(splitdata.Length  == 3)
                {
 
                    //it is a time, patch and after resolve bug by patch
                    MyData inputd;
                    for (int j = 0; j < bugnoline; j++)
                    {
                        Console.WriteLine(lines[i]);
                        item = lines[i];
                        splitdata = item.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                        
                        inputd = new MyData();
                        int data;
                        //checking is that a time
                        inputCond = int.TryParse(splitdata[0], out data);
                        
                        inputd.time = data;
                        inputd.patchCondition = splitdata[1];
                        inputd.patchSolve = splitdata[2];

                        if (!inputCond)
                        {
                            break;
                        }
                        else
                        {
                            KeepData.Add(inputd);
                        }
                        i++;
                    }
                    i--;
                    if (KeepData.Count > 0)
                    {
                        TimeStampData = new List<int>();
                        string x = CalNewData();
                        //try to find solution for graph
                        CalData(0,x,string.Empty);


                        //checking for from timestamp
                        if (TimeStampData.Count > 0)
                        {

                            Console.WriteLine("Fixing bug using mininum time is " + TimeStampData.Min());

                        }
                        else
                        {
                            Console.WriteLine("Can not fix this bug");
                        }
                    }
                }
                i++;

            } while (bugnoline != 0 || numpatch != 0 && lines.Length >= i && inputCond);

            Console.Read();

        }

        static string CalNewData()
        {
            string DefaultValue = string.Empty;
            for (int k = 0; k < KeepData[0].patchCondition.Length; k++)
            {
                DefaultValue = DefaultValue + "+";

            }
            return DefaultValue;
        }

        static bool CheckingPreCondition(string x, string y)
        {
            char[] stx = x.ToCharArray(); //x = preCond
            char[] sty = y.ToCharArray(); //y = patchCond
            int k = 0;                
            for (int i = 0; i < stx.Length; i++)
            {
                if (y[i] == '0')
                {
                    //ignore case
                    k++;
                }
                else if (x[i] == y[i])
                {
                    k++;
                }
            }
            if (k == stx.Length)
                return true;
            else return false;
        }

        static void CalData(int sumvalue, string testsubject, string patchbefore)
        {
            for (int i = 0; i < KeepData.Count; i++)
            {   
                MyData p = KeepData[i];

                //checking that is not the same last one of the patch
                if(string.Compare(patchbefore,p.patchCondition) != 0)
                {
                    bool isCond = CheckingPreCondition(testsubject,p.patchCondition);
                    if (isCond)
                    {
                        //change value
                        char[] CalDebugData = DebugData(testsubject.ToCharArray(), p.patchSolve.ToCharArray());
                        //checking if can fix all bug
                        bool isFullPatched = checkingFixValue(CalDebugData);
                        if (isFullPatched)
                        {
                            TimeStampData.Add(sumvalue + p.time);
                        }
                        else if (sumvalue < 1000)//maximum cal time before stackoverflow
                        {
                                //find next value if there is not full value
                                CalData(sumvalue + p.time, new string(CalDebugData ), p.patchCondition);
                        }
                    
                    }
                }
            }
        }

        static bool checkingFixValue(char[] input)
        {
            int b = 0;
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '-')
                {
                    b++;
                }

            }
            if (b == input.Length)
                return true;
            else return false;
        }

        static char[] DebugData(char[] input,char[] key)
        {
            for (int i = 0; i < key.Length; i++)
            {
                if (key[i] != '0')
                {
                    input[i] = key[i];
                }
            }
            return input;
        }

        static void readData()
        {
            lines = null;
            if (File.Exists(path))
            {
                lines = System.IO.File.ReadAllLines(path);
            }
            else Console.WriteLine("Error, Product file not found");
        }
    }
    public class MyData
    {
        public int time { get; set; }
        public string patchCondition { get; set; }
        public string patchSolve { get; set; }
    }
}
