using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReplayBehaviour : MonoBehaviour
{
    public Text nameText;
    public Text scoreText;

    private int replayIndex;

    public void SetReplayProfile(int replayIndex)
    {
        this.replayIndex = replayIndex;
        ReplayProfile profile = GameData.gameData.saveData.profiles[replayIndex];
        nameText.text = profile.name;
        scoreText.text = profile.score.ToString() + " POINTS";
    }

    public void Selected()
    {
        FindObjectOfType<MainMenuBehaviour>().StartReplay(replayIndex);
    }

    public void Delete()
    {
        GameData.gameData.saveData.profiles.Remove(GameData.gameData.saveData.profiles[replayIndex]);
        GameData.gameData.Save();
        FindObjectOfType<MainMenuBehaviour>().ListReplays();
    }

}
