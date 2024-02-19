using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PartyManager : MonoBehaviour
{
    public List<CharacterBuild> partyCharacters = new List<CharacterBuild>();
    public UnityEvent<CharacterBuild> setActiveCharacterButton;
    public SaveFilesScriptable autoSaveAsset;
    public SaveLoadManager saveLoadManager;
    public Transform playerStart;
    public Transform player;
    public bool freshStart = false;

    SaveFilesScriptable currentSaveAsset;
    [SerializeField] CharacterMenu heroMenu;
    int partyXPLimitBreak = 10;
    int partyXP = 0;
    int partySkillPoints = 0;
    CharacterBuild highligthedCharacter;


    PartyState partyState;

    private void Awake()
    {
        if (!freshStart)
        {
            foreach(SaveFilesScriptable s in saveLoadManager.listOfSaveFiles)
            {
                if (s.lastUsedSaveFile)
                {
                    currentSaveAsset = s;
                    if (currentSaveAsset.playerPosition != Vector3.zero) player.position = currentSaveAsset.playerPosition;
                    else {
                        autoSaveAsset.playerPosition = playerStart.position;
                        player.position = playerStart.position; 
                    }
                    //PersistentDataManager.ApplySaveData(currentSaveAsset.saveLuaString);
                    currentSaveAsset.LoadCharactersData(partyCharacters);
                    return;
                }
            }
            if (autoSaveAsset.playerPosition == Vector3.zero) autoSaveAsset.playerPosition = playerStart.position;
            player.position = playerStart.position;
            //PersistentDataManager.ApplySaveData(autoSaveAsset.saveLuaString);
            autoSaveAsset.LoadCharactersData(partyCharacters);
            autoSaveAsset.SaveCharactersData(partyCharacters);
        }

    }

    public void AutoSaveProgress()
    {
        autoSaveAsset.SaveCharactersData(partyCharacters);
    }

    public void SaveProgressToFile(int index)
    {
        saveLoadManager.listOfSaveFiles[index].playerPosition = player.position;
        saveLoadManager.listOfSaveFiles[index].SaveCharactersData(partyCharacters);
    }

    private void Start()
    {


        foreach (CharacterBuild c in partyCharacters)
        {
            CharacterButton button = c.gameObject.GetComponent<CharacterButton>();
            setActiveCharacterButton.AddListener(button.SetButtonActive);
        }
    }
    public void SetPartyState(PartyState enterPartyState)
    {
        partyState = enterPartyState;
    }

    public PartyState GetPartyState()
    {
        return partyState;
    }

    public void SetHighlithedCharacter(CharacterBuild character)
    {
        highligthedCharacter = character;
        setActiveCharacterButton.Invoke(character);
    }

    public CharacterBuild GetHighligthedCharacter()
    {
        return highligthedCharacter;
    }

    public void AddPartyXP(int amount)
    {
        partyXP += amount;
        if (partyXP >= partyXPLimitBreak)
        {
            if ((partyXP - partyXPLimitBreak) > 0)
            {
                partyXP -= partyXPLimitBreak;
                AddSkillPoint(5);
            }
            else
            {
                partyXP = 0;
                AddSkillPoint(5);
            }
        }
    }

    public void UseSkillPoint()
    {
        if(partySkillPoints>0) partySkillPoints--;
    }


    public void AddSkillPoint(int amount)
    {
        partySkillPoints += amount;
    }
}

public static class GlobalStats
{
    public static int AllCharisma, AllAgility, AllIntellect, AllLuck;
    public static PartyState partyStateGlobal;
}

[System.Serializable]
public enum PartyState
{
    GameStart,
    Explore,
    Battle,
    Dialogue,
    PuzzleLocation,
    HeroMenu,
    Creator
}