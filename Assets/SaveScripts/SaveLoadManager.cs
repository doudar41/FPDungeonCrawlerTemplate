using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadManager : MonoBehaviour
{
    public  SaveFilesScriptable[] listOfSaveFiles;
    [SerializeField] PartyManager party;
    [SerializeField] SaveFilesScriptable autoSaveFile;
    int lastUsedSaveFileIndex = 0;

    void Start()
    {
        
    }

    public void LoadSaveFile(int index)
    {
        for(int i = 0; i < listOfSaveFiles.Length; i++)
        {
            if(i != index)
            {
                listOfSaveFiles[i].lastUsedSaveFile = false;
            }
            else
            {
                listOfSaveFiles[i].lastUsedSaveFile = true;
            }
        }
        SceneManager.LoadScene(listOfSaveFiles[index].savedSceneName);
    }

    public void LoadAutoSaveFile()
    {
        for (int i = 0; i < listOfSaveFiles.Length; i++)
        {
            listOfSaveFiles[i].lastUsedSaveFile = false;
        }
        SceneManager.LoadScene(autoSaveFile.savedSceneName);
    }

    public void CleanSaveFile(int index)
    {
        listOfSaveFiles[index].CleanSaveInfo();
    }
}
