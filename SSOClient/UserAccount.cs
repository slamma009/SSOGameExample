using System;
using System.Collections.Generic;
using System.Text;

namespace SSOClient
{
    public class UserAccount
    {
        public string Username;

        public int Strength = 10;
        public int Dexterity = 10;
        public int Charisma = 10;
        public int Wisdom = 10;
        public int Inteligence = 10;
        public int Constitution = 10;

        public readonly int PointBuy = 15;


        public virtual void WriteStats()
        {
            Console.WriteLine(Username + ":");
            Console.WriteLine("  Str: " + Strength);
            Console.WriteLine("  Dex: " + Dexterity);
            Console.WriteLine("  Con: " + Constitution);
            Console.WriteLine("  Wis: " + Wisdom);
            Console.WriteLine("  Int: " + Inteligence);
            Console.WriteLine("  Cha: " + Charisma);
        }
    }
}
