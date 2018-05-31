using SSOClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSOGameExample
{
    public class GameSpecificAccount : UserAccount
    {
        public GameSpecificAccount() { }
        public GameSpecificAccount(UserAccount account)
        {
            SetBaseAccount(account);
        }

        public int StrMod;
        public int DexMod;
        public int ConMod;
        public int ChaMod;
        public int IntMod;


        public int Strength { get { return base.Strength + StrMod; } }
        public int Dexterity { get { return base.Dexterity + DexMod; } }
        public int Constitution { get { return base.Constitution + ConMod; } }
        public int Charisma { get { return base.Charisma + ChaMod; } }
        public int Wisdom { get { return 10; } }
        public int Inteligence { get { return base.Inteligence + IntMod; } }
        public readonly int PointBuy = 13;

        public override void WriteStats()
        {
            Console.WriteLine(Username + ":");
            Console.WriteLine("  Str: " + Strength);
            Console.WriteLine("  Dex: " + Dexterity);
            Console.WriteLine("  Con: " + Constitution);
            Console.WriteLine("  Int: " + Inteligence);
            Console.WriteLine("  Cha: " + Charisma);
        }

        public void SetBaseAccount(UserAccount account)
        {
            base.Username = account.Username;
            base.Strength = account.Strength;
            base.Dexterity = account.Dexterity;
            base.Constitution = account.Constitution;
            base.Charisma = account.Charisma;
            base.Wisdom = account.Wisdom;
            base.Inteligence = account.Inteligence;
        }
    }
}
