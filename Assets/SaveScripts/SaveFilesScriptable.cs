using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
//using PixelCrushers.DialogueSystem;

[CreateAssetMenu]
public class SaveFilesScriptable : ScriptableObject
{
    public List<SavedHero> saveHeroList = new List<SavedHero>();
    public Vector3 playerPosition;
    public string saveLuaString;
    public bool lastUsedSaveFile = false;
    public string savedSceneName = "TitleScreen";

    public void SaveDialogueState()
    {
        //saveLuaString = PersistentDataManager.GetSaveData();
    }


    public void SaveCharactersData(List<CharacterBuild> characters)
    {
        if (saveHeroList.Count < 4) { 
            for (int i = saveHeroList.Count; i < 4; i++) 
            { 
                saveHeroList.Add(new SavedHero()); 
            } 
        }
        for(int i=0;i< characters.Count; i++)
        {
            saveHeroList[i].avatar = characters[i].avatar;
            saveHeroList[i].Charisma = characters[i].Charisma;
            saveHeroList[i].Agility = characters[i].Agility;
            saveHeroList[i].Intellect = characters[i].Intellect;
            saveHeroList[i].selfEsteemCurrent = characters[i].GetCurrentSelfEsteem();
            saveHeroList[i].selfEsteemMAX = characters[i].GetMaxSelfEsteemAmount();
            saveHeroList[i].staminaCurrent = characters[i].GetCurrentStamina();
            saveHeroList[i].staminaMAX = characters[i].GetMaxStaminaAmount();
        }

        savedSceneName = SceneManager.GetActiveScene().name;
        SaveDialogueState();
    }


    public void CleanSaveInfo()
    {
        playerPosition = Vector3.zero;
        saveHeroList.Clear();
        saveLuaString = "";
        savedSceneName =  "TitleScreen";
    }

    public void LoadCharactersData(List<CharacterBuild> characters)
    {
        for (int i = 0; i < characters.Count; i++)
        {
            characters[i].avatar = saveHeroList[i].avatar;
            characters[i].Charisma = saveHeroList[i].Charisma;
            characters[i].Agility = saveHeroList[i].Agility;
            characters[i].Intellect = saveHeroList[i].Intellect;
            characters[i].SetCurrentSelfEsteem(saveHeroList[i].selfEsteemCurrent); 
        }
    }
}

[System.Serializable]
public class SavedHero
{
    public AnimatedAvatar avatar;
    public string heroName;

    //Main Stats
    public int Charisma;
    public int Intellect;
    public int Agility;
    public int Luck;

    public float selfEsteemMAX;
    public float staminaMAX;
    public float selfEsteemCurrent;
    public float staminaCurrent;

    public List<string> characterSpellBook = new List<string>();
    public Dictionary<string, int> notesTrained = new Dictionary<string, int>();

}





/*string s = PersistentDataManager.GetSaveData(); // Save state.
PersistentDataManager.ApplySaveData(s); // Restore state.
DialogueManager.ResetDatabase(DatabaseResetOptions.KeepAllLoaded); // Reset state to initial values.*/