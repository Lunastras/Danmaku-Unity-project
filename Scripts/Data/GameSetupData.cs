using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetupData
{
    //These values need to be initialized
    //if not initialized, the game manager will give them default values.
    //These values indicate the setup was not properly initialised
    public static GameSetupData instance = null;
    public int initialRandIndex = -1;
    public List<InputForFrame> inputs = null;
    public bool isPlayBack = false;
    public bool isTutorial = false;
}
