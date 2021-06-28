using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class SaveData
{
    public SaveData()
    {
        profiles = new List<ReplayProfile>();
    }

    public List<ReplayProfile> profiles;
}

[Serializable]
public struct ReplayProfile
{
    public List<InputForFrame> inputs;
    public string name;
    public int score;
    public int initialRandIndex;
}

[Serializable]
public struct InputForFrame
{
    public float x, y;
    public bool isFiring;
    public bool isCrouching;
}

public class GameData : MonoBehaviour
{
    public static GameData gameData;
    public SaveData saveData;

    void Awake()
    {
        if (gameData == null)
        {
            DontDestroyOnLoad(this.gameObject);
            gameData = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        Load();
    }

    public void Save()
    {
        BinaryFormatter formatter = new BinaryFormatter();

        FileStream file = File.Open(Application.persistentDataPath + "/player.data", FileMode.Create);

        SaveData data = new SaveData();
        data = saveData;

        formatter.Serialize(file, data);

        file.Close();

        Debug.Log("file saved succesfully");
    }

    public void Load()
    {
        bool createNewSave = true;
        try
        {
            if (File.Exists(Application.persistentDataPath + "/player.data"))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/player.data", FileMode.Open);
                saveData = formatter.Deserialize(file) as SaveData;

                file.Close();
                Debug.Log("Loaded");
                createNewSave = false;
            }
        } catch (Exception e)
        {
            Debug.LogWarning("Corrupted save file detected \n full message: " + e);
        }
        
        if(createNewSave)
        {
            saveData = new SaveData();
            Save();
        }
    }
}