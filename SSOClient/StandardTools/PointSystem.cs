using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSOClient.StandardTools
{
    public static class PointSystem
    {
        public static int CalculatePointCost(int points)
        {
            if (points > 18 || points < 7)
                return 1000;
            else if (points == 18)
                return 17;
            else if (points == 17)
                return 13;
            else if (points == 16)
                return 10;
            else if (points == 15)
                return 7;
            else if (points == 14)
                return 5;
            else if (points == 7)
                return -2;
            else
                return points - 10;
        }

        public static int CalculateTotalPoitnsForAccount(UserAccount account)
        {
            var points = 0;
            points += CalculatePointCost(account.Strength);
            points += CalculatePointCost(account.Dexterity);
            points += CalculatePointCost(account.Constitution);
            points += CalculatePointCost(account.Charisma);
            points += CalculatePointCost(account.Inteligence);
            points += CalculatePointCost(account.Wisdom);

            return points;
        }

        public static bool IsValidAccount(UserAccount account)
        {
            return CalculateTotalPoitnsForAccount(account) == account.PointBuy && account.Username != null && account.Username.Length > 3;
        }
    }
}
