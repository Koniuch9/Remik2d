using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class CardUtils
{
    private static string[] types = { "X", "S", "H", "C", "D" };
    private static string[] values = { "X", "2", "3", "4", "5", "6", "7", "8", "9", "T", "J", "Q", "K", "A" };

    public static Dictionary<string, string> sequenceMinusOneMap = new Dictionary<string, string>{
        {"2", "A"},
        {"3", "2"},
        {"4", "3"},
        {"5", "4"},
        {"6", "5"},
        {"7", "6"},
        {"8", "7"},
        {"9", "8"},
        {"T", "9"},
        {"J", "T"},
        {"Q", "J"},
        {"K", "Q"},
        {"A", "K"}
    };

    public static Dictionary<string, string> sequencePlusOneMap = new Dictionary<string, string>{
        {"2", "3"},
        {"3", "4"},
        {"4", "5"},
        {"5", "6"},
        {"6", "7"},
        {"7", "8"},
        {"8", "9"},
        {"9", "T"},
        {"T", "J"},
        {"J", "Q"},
        {"Q", "K"},
        {"K", "A"},
        {"A", "2"}
    };

    public static Dictionary<string, int> sequenceMap = new Dictionary<string, int>{
        {"2", 0},
        {"3", 1},
        {"4", 2},
        {"5", 3},
        {"6", 4},
        {"7", 5},
        {"8", 6},
        {"9", 7},
        {"T", 8},
        {"J", 9},
        {"Q", 10},
        {"K", 11},
        {"A", 12}
    };

    private static Dictionary<string, int> valueMap = new Dictionary<string, int>{
        {"X", 1},
        {"2", 2},
        {"3", 3},
        {"4", 4},
        {"5", 5},
        {"6", 6},
        {"7", 7},
        {"8", 8},
        {"9", 9},
        {"T", 10},
        {"J", 11},
        {"Q", 12},
        {"K", 13},
        {"A", 14}
    };

    private static Dictionary<string, int> typeMap = new Dictionary<string, int>{
        {"X", 0},
        {"S", 15},
        {"H", 30},
        {"C", 45},
        {"D", 60}
    };

    private static Dictionary<string, int> pointsMap = new Dictionary<string, int>{
        {"X", 30},
        {"2", 2},
        {"3", 3},
        {"4", 4},
        {"5", 5},
        {"6", 6},
        {"7", 7},
        {"8", 8},
        {"9", 9},
        {"T", 10},
        {"J", 10},
        {"Q", 10},
        {"K", 10},
        {"A", 20}
    };

    public static int getPoints(string cardDescription)
    {
        return pointsMap["" + cardDescription[0]];
    }

    public static string getDescriptionFromData(CardData data)
    {
        return data.value + data.type;
    }

    public static int computeColor(CardData card)
    {
        return typeMap[card.type] + valueMap[card.value];
    }

    public static int computeValue(CardData card)
    {
        return 60 * valueMap[card.value] + typeMap[card.type];
    }

    public static int compareColor(CardData a, CardData b)
    {
        if (a != null && b != null)
            return computeColor(a) - computeColor(b);
        else
            return -1;
    }

    public static int compareValue(CardData a, CardData b)
    {
        if (a != null && b != null)
        {
            return computeValue(a) - computeValue(b);
        }
        return -1;
    }

    public static int compareByColor(GameObject a, GameObject b)
    {
        return compareColor(a.GetComponent<CardData>(), b.GetComponent<CardData>());
    }

    public static int compareByValue(GameObject a, GameObject b)
    {
        return compareValue(a.GetComponent<CardData>(), b.GetComponent<CardData>());
    }

    public static string getRandomCardDecription()
    {
        System.Random random = new System.Random();
        return values[random.Next(1, values.Length)] + types[random.Next(1, types.Length)];
    }

    public static string[] getDecks(int howMany)
    {
        string[] result = new string[52 * howMany];
        string[] deck = getDeck();
        for (int i = 0; i < howMany; i++)
        {
            deck.CopyTo(result, i * 52);
        }
        return result;
    }

    public static string[] getJokers(int howMany)
    {
        return Enumerable.Repeat("XX", howMany).ToArray();
    }

    public static string getJoker()
    {
        return "XX";
    }

    public static string[] getDeck()
    {
        string[] result = new string[52];
        for (int i = 1; i < types.Length; i++)
        {
            for (int j = 1; j < values.Length; j++)
            {
                result[(i - 1) * (values.Length - 1) + j - 1] = values[j] + types[i];
            }
        }
        return result;
    }

    public static string getCardResourceName(string description)
    {
        int length = description.Length;
        if (length > 2 || length < 1) return "";
        if (description == "X") return "cardJoker";
        if (length == 2)
        {
            if (description[0] == 'X' || description[1] == 'X') return "cardJoker";
            string type = "";
            string value = "" + description[0];
            if (description[0] == 'T' || description[0] == 't')
            {
                value = "10";
            }
            switch (description[1])
            {
                case 'S':
                case 's':
                    type = "Spades";
                    break;
                case 'H':
                case 'h':
                    type = "Hearts";
                    break;
                case 'C':
                case 'c':
                    type = "Clubs";
                    break;
                case 'D':
                case 'd':
                    type = "Diamonds";
                    break;
            }
            return "card" + type + value;
        }
        return "";
    }
}
