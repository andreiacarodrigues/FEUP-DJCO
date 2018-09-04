using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveLoadController
{
    public static SaveData data = null;
    public static List<string> FountainsUsedList = new List<string>();
    public static List<string> EnemiesDefeatedList = new List<string>();
    public static List<string> ScrollsCaughtList = new List<string>();
    public static bool VinesDestroyedStatic = false;

    [System.Serializable]
    public class SaveData
    {
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
        public float CurrHealth { get; set; }
        public int CurrExp { get; set; }
        public int CurrLevel { get; set; }
        public int ScrollsCaughtCount { get; set; }
        public string[] FountainsUsed { get; set; }
        public string[] EnemiesDefeated { get; set; }
        public string[] ScrollsCaught { get; set; }
        public bool VinesDestroyed { get; set; }
    }

    public static void ResetValues()
    {
        data = null;
        FountainsUsedList = new List<string>();
        EnemiesDefeatedList = new List<string>();
        ScrollsCaughtList = new List<string>();
        VinesDestroyedStatic = false;
    }

    public static void Save(bool ToFile = true)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        SaveData saveData = new SaveData
        {
            PositionX = player.transform.position.x,
            PositionY = player.transform.position.y,
            PositionZ = player.transform.position.z,
            ScrollsCaughtCount = StoryController.GetNumberScrollsFound(),
            CurrHealth = Battle.playerEntity.CurrentHealth,
            CurrExp = Battle.playerEntity.CurrentExperience,
            CurrLevel = Battle.playerEntity.Stats.Level,
            FountainsUsed = FountainsUsedList.ToArray(),
            EnemiesDefeated = EnemiesDefeatedList.ToArray(),
            ScrollsCaught = ScrollsCaughtList.ToArray(),
            VinesDestroyed = VinesDestroyedStatic
        };

        if(!ToFile)
        {
            data = saveData;
            return;
        }

        BinaryFormatter binaryFormatter = new BinaryFormatter();

        if (File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            File.Delete(Application.persistentDataPath + "/playerInfo.dat");
        }

        FileStream fileStream = File.Create(Application.persistentDataPath + "/playerInfo.dat");
        Debug.Log("Game Saved On: " + Application.persistentDataPath + "/playerInfo.dat");
        binaryFormatter.Serialize(fileStream, saveData);
        fileStream.Close();
    }

    public static bool Load()
    {
        if (File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
            data = (SaveData)binaryFormatter.Deserialize(fileStream);

            FountainsUsedList.Clear();
            EnemiesDefeatedList.Clear();
            ScrollsCaughtList.Clear();

            foreach (string fountain in data.FountainsUsed)
            {
                FountainsUsedList.Add(fountain);
            }

            foreach (string enemy in data.EnemiesDefeated)
            {
                EnemiesDefeatedList.Add(enemy);
            }

            foreach (string scroll in data.ScrollsCaught)
            {
                ScrollsCaughtList.Add(scroll);
            }

            fileStream.Close();
            return true;
        }
        return false;
    }
}