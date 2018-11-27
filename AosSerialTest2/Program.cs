using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AosSerialTest1
{
    class Program
    {
        static void Main(string[] args)
        {
            /*AosSerial ser = new AosSerial();
            ser.Start("COM4", 115200);*/
            //Controller controller = new Controller();

            string str;
            while (true)
            {
                str = Console.ReadLine();
                //ser.Write(str);
                if ("exit" == str)
                    break;
                //controller.SetValue(str);
            }
            

            //ser.Close();
            //controller.Close();
        }
    }
}
