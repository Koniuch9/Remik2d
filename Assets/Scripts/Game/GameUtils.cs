
using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Realtime;
using UnityEngine;

public class GameUtils
{
    public static string[] Shuffle(string[] cards)
    {
        System.Random r = new System.Random();
        return cards.OrderBy(x => r.Next()).ToArray();
    }

    public static string[] GetDeckCards(int decksNumber, int jokersNumber)
    {
        string[] decks = CardUtils.getDecks(decksNumber);
        return decks.Concat(CardUtils.getJokers(jokersNumber)).ToArray();
    }

    public static Dictionary<string, string> DealPlayers(string[] cards, Player[] players, int howMany)
    {
        Dictionary<string, string> result = new Dictionary<string, string>();
        for (int i = 0; i < players.Length; i++)
        {
            Player p = players[i];
            result[p.NickName] = String.Join(',', cards[(i * howMany)..((i + 1) * howMany)]);
        }
        return result;
    }

    public static int cardsNumberFromType(GameType type)
    {
        return (int)type + 8;
    }

    public static GameType typeFromTurn(int gameTurn)
    {
        if (gameTurn < 7)
        {
            return (GameType)gameTurn;
        }
        else
        {
            return (GameType)(13 - gameTurn);
        }
    }

    public static char[] meldsFromType(GameType gameType)
    {
        switch (gameType)
        {
            case GameType.TWO_TRIPLES:
                return new char[] { 'T', 'T' };
            case GameType.SEQUENCE_TRIPLE:
                return new char[] { 'S', 'T' };
            case GameType.TWO_SEQUENCE:
                return new char[] { 'S', 'S' };
            case GameType.THREE_TRIPLES:
                return new char[] { 'T', 'T', 'T' };
            case GameType.SEQUENCE_TWO_TRIPLES:
                return new char[] { 'S', 'T', 'T' };
            case GameType.TWO_SEQUENCE_TRIPLE:
                return new char[] { 'S', 'S', 'T' };
            case GameType.THREE_SEQUENCE:
                return new char[] { 'S', 'S', 'S' };
        }
        return null;
    }

    public static int countPoints(string[] cardsDescriptions, bool doublePoints)
    {
        int points = cardsDescriptions.Aggregate(0, (acc, curr) => acc + CardUtils.getPoints(curr));
        if (points == 0) return -100;
        if (doublePoints) points *= 2;
        return (int)Math.Round((double)points / 10.0f, MidpointRounding.AwayFromZero) * 10;
    }

    public static void PrintList<T>(string first, List<T> list)
    {
        string s = "";
        foreach (T obj in list)
        {
            s += obj.ToString() + ", ";
        }
        Debug.Log(first + s);
    }

    public static T GetRandom<T>(T[] values)
    {
        System.Random r = new System.Random();
        return values[r.Next(values.Length)];
    }
}
