using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static event Action<string, AudioSource> OnAngrySound;
    public static event Action<string, AudioSource> OnHappySound;
    public static event Action<string, AudioSource> OnLaugh;
    [SerializeField] private AudioSource cardDropSound;
    [SerializeField] private AudioSource myTurnSound;
    [SerializeField] private AudioSource winSound;
    [SerializeField] private AudioSource cardMoveSound;
    [SerializeField] private AudioSource victorySound;
    [SerializeField] private AudioSource[] happySounds;
    [SerializeField] private AudioSource[] angrySounds;
    [SerializeField] private AudioSource[] winSongs;
    [SerializeField] private AudioSource[] neutralSongs;
    [SerializeField] private AudioSource[] lossSongs;
    [SerializeField] private AudioSource[] laughs;

    private Dictionary<string, bool> playerMelds = new Dictionary<string, bool>();
    private class PlayerClass
    {
        public string name;
        public int points;

        public PlayerClass(string name, int points)
        {
            this.name = name;
            this.points = points;
        }
    }

    private List<PlayerClass> endResult = new List<PlayerClass>();
    private bool gameOverContinue = false;

    private void OnEnable()
    {
        Meld.OnCardAddedToMeld += (c, i) => PlayCardDropSound();
        Meld.OnCardAddedToPlayerMeld += (c, i, s) => PlayCardDropSound();
        Meld.OnCardRemovedFromMeld += (c, i) => PlayCardMoveSound();
        PropsManager.OnPlayerTurnUpdated += (p) =>
        {
            if (p.Equals(PhotonNetwork.LocalPlayer.NickName))
            {
                PlayMyTurnSound();
            }
        };
        TakeDeckCardButton.OnTakeDeckCardButtonClicked += (s) => PlayCardMoveSound();
        StackCardsPanel.OnTakeAllFromStack += (s) => PlayCardMoveSound();
        StackCardsPanel.OnTakeFirstFromStack += (s) => PlayCardMoveSound();
        StackButton.OnCardAddedToStack += (c, i) => PlayCardDropSound();
        PropsManager.OnPlayerWon += (p) =>
        {
            if (!p.Equals(""))
            {
                PlayWinSound();
            }
        };
        PlayerPropsManager.OnPlayerMeldCardsUpdated += PlayerMeldsUpdated;
        PlayerPropsManager.OnPlayerPointsUpdated += PlayerPointsUpdated;
        ContinueButton.OnContinueButtonClicked += CheckGameOver;
    }

    private void CheckGameOver()
    {
        gameOverContinue = true;
        CheckPlayEndSong();
    }

    private void PlayerPointsUpdated(string playerName, int[] points)
    {
        //TODO change to 14
        if (points != null && points.Length == 14)
        {
            endResult.Add(new PlayerClass(playerName, points.Aggregate(0, (acc, curr) => acc + curr)));
            CheckPlayEndSong();
        }
    }

    private void CheckPlayEndSong()
    {
        if (gameOverContinue && endResult.Count == PhotonNetwork.PlayerList.Length)
        {
            endResult.Sort((a, b) => a.points - b.points);
            int myPosition = endResult.FindIndex((p) => p.name.Equals(PhotonNetwork.LocalPlayer.NickName));
            if (myPosition == 0)
            {
                GameUtils.GetRandom(winSongs).Play();
            }
            else if (myPosition == endResult.Count - 1)
            {
                GameUtils.GetRandom(lossSongs).Play();
            }
            else
            {
                GameUtils.GetRandom(neutralSongs).Play();
            }
        }
    }

    private void PlayerMeldsUpdated(string playerName, string[][] meld)
    {
        if (!playerMelds.ContainsKey(playerName) || (meld != null && meld.Length <= 1))
        {
            playerMelds[playerName] = false;
        }
        else
        {
            if (!playerMelds[playerName])
            {
                PlayMeldSounds(playerName);
                playerMelds[playerName] = true;
            }
            else if (meld != null && meld.Length > 1)
            {
                OnLaugh?.Invoke((string)PropsManager.instance.GetProp(Props.PLAYER_TURN),
                    GameUtils.GetRandom(laughs));
            }
        }
    }

    private void PlayMeldSounds(string playerName)
    {
        System.Random r = new System.Random();
        AudioSource[] angry = angrySounds.OrderBy((x) => r.Next())
        .Take(PhotonNetwork.PlayerList.Length - 1).ToArray();
        int i = 0;
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (p.NickName.Equals(playerName))
            {
                OnHappySound?.Invoke(playerName, GetRandom(happySounds));
            }
            else
            {
                LeanTween.delayedCall(0.5f + (float)r.NextDouble() * 1.5f, () =>
                {
                    OnAngrySound?.Invoke(p.NickName, angry[i++]);
                });
            }
        }
    }

    private AudioSource GetRandom(AudioSource[] sources)
    {
        System.Random r = new System.Random();
        return sources[r.Next(sources.Length)];
    }

    private void PlayCardDropSound()
    {
        cardDropSound.Play();
    }

    private void PlayMyTurnSound()
    {
        myTurnSound.Play();
    }

    private void PlayWinSound()
    {
        winSound.Play();
    }

    private void PlayCardMoveSound()
    {
        cardMoveSound.Play();
    }

    private void PlayVictorySound()
    {
        victorySound.Play();
    }
}
