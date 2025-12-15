using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    private string saveFileName = "mysave_v2.json";

    public SaveData data;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Load();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(data, true);
        string path = Path.Combine(Application.persistentDataPath, saveFileName);
        File.WriteAllText(path, json);
    }

    public void Load()
    {
        string path = Path.Combine(Application.persistentDataPath, saveFileName);

        if (File.Exists(path))
        {
            try
            {
                string json = File.ReadAllText(path);
                data = JsonUtility.FromJson<SaveData>(json);
            }
            catch
            {
                Debug.LogWarning("Fichier corrompu, création d'un nouveau.");
                data = new SaveData();
            }
        }
        else
        {
            data = new SaveData();
        }

        if (data.upgrades == null) data.upgrades = new List<UpgradeSave>();
        if (data.unlockedItems == null) data.unlockedItems = new List<string>();
        if (data.assignedWorkerSkins == null) data.assignedWorkerSkins = new List<int>();
    }

    public int GetOrAssignSkinForWorker(int workerID, int totalSkinsAvailable)
    {
        if (workerID < data.assignedWorkerSkins.Count)
        {
            int savedSkinIndex = data.assignedWorkerSkins[workerID];

            if (savedSkinIndex >= totalSkinsAvailable) savedSkinIndex = 0;

            return savedSkinIndex;
        }
        else
        {
            while (data.assignedWorkerSkins.Count <= workerID)
            {
                int randomSkin = Random.Range(0, totalSkinsAvailable);
                data.assignedWorkerSkins.Add(randomSkin);
            }

            Save();

            return data.assignedWorkerSkins[workerID];
        }
    }

    private void OnApplicationQuit()
    {
        Save();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus) Save();
    }

    public void DeleteSave()
    {
        SaveData emptyData = new SaveData();

        string json = JsonUtility.ToJson(emptyData, true);
        string path = Path.Combine(Application.persistentDataPath, saveFileName);
        File.WriteAllText(path, json);

        Debug.Log("SAUVEGARDE ET SKINS EFFACÉS !");

        if (instance == this)
        {
            instance = null;
        }

        Destroy(gameObject);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}