using ConsoleApp1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlHoaDon xml = new XmlHoaDon("C:/Users/phong/OneDrive/Desktop/New folder/0100686209-999_1C23TYA_116.xml");

            Console.WriteLine(xml.GetJsonTTChung()); Console.WriteLine();
            Console.WriteLine(xml.GetJsonNBan()); Console.WriteLine();
            Console.WriteLine(xml.GetJsonNMua()); Console.WriteLine();

            foreach (var item in (xml.GetJsonHDDVs()))
            {
                Console.WriteLine(item);
            }
            Console.WriteLine();
            foreach (var item in (xml.GetJsonTTKhacs()))
            {
                Console.WriteLine(item);
            }
            Console.WriteLine();
            Console.WriteLine(xml.GetJsonSignatureNBan()); Console.WriteLine();
            Console.WriteLine(xml.GetJsonNBanSignedTime()); Console.WriteLine();
            Console.ReadLine();
        }
    }
}
