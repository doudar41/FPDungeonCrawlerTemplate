using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class CreateCharacterMenu : MonoBehaviour
{
    
    [SerializeField] AnimatedAvatar portrait;
    [SerializeField] AnimatedAvatar defaultPortrait;
    [SerializeField] Image instrumentImage, spellImage;
    string chosenInstrument, chosenSpell;
    [SerializeField] Sprite cellBackground;

    [SerializeField] UISpriteAnimation animatedAvatar;

    [SerializeField] TextMeshProUGUI heronameText;
    [SerializeField] TMP_InputField heronameInputText;
    [SerializeField] TextMeshProUGUI charismaAmount, agilityAmount, intellectAmount, luckAmount;
    [SerializeField] TextMeshProUGUI selfEsteemCurrent, staminaCurrent;
    [SerializeField] TextMeshProUGUI partyLevelText, partyStatsPointsText;
    [SerializeField] TextMeshProUGUI spellName, spellDetails;

    [SerializeField] RectTransform mainSpellGrid;
    //[SerializeField] SpellBook spellBookIndexes;
    [SerializeField] PartyManager party;
    [SerializeField] GameObject spellButton;
    [SerializeField] Button confirmButton;
    [SerializeField] Button startGameButton;
    [SerializeField] int partyStatsPoints;
    int currentCharacterCharisma, currentCharacterAgility, currentCharacterIntellect, currentCharacterLuck;
    int partyStatsPointsCurrent;
    [SerializeField]  int partyLevel = 1;
    [SerializeField] int initialAmountOfStats = 5;
    string heroName;
    bool nameChanged = false;
    CharacterBuild currentCharacter;
    public UnityEvent onClickSpeechEvent;

    // Start is called before the first frame update
    private void Awake()
    {
        party.SetPartyState(PartyState.Creator);
    }
    private void OnDisable()
    {
        party.SetPartyState(PartyState.Explore);
    }
    void Start()
    {
        partyStatsPointsCurrent = partyStatsPoints;
        partyStatsPointsText.text = partyStatsPointsCurrent.ToString();
        GetCharacterFromButton(party.partyCharacters[0]);
        currentCharacter.gameObject.GetComponent<CharacterButton>().SetButtonActive(currentCharacter);
    }


    public void GiveHeroAName(string name)
    {
        heroName = name;
        nameChanged = true;
        CheckForReadyness();
    }

    public void AddStatsPoints(int statIndex) 
    {
        if (partyStatsPointsCurrent < 1) { partyStatsPointsCurrent = 0; 
            partyStatsPointsText.text = partyStatsPointsCurrent.ToString(); return; }

        switch (statIndex)
        {
            case 0:
                currentCharacterCharisma += 1;
                partyStatsPointsCurrent -= 1;
                charismaAmount.text = currentCharacterCharisma.ToString();
                selfEsteemCurrent.text = ((int) GetMaxSelfEsteemAmount()).ToString();
                break;
            case 1:
                currentCharacterAgility += 1;
                partyStatsPointsCurrent -= 1;
                agilityAmount.text = currentCharacterAgility.ToString();
                staminaCurrent.text = ((int)GetMaxStaminaAmount()).ToString();
                break;
            case 2:
                currentCharacterIntellect += 1;
                partyStatsPointsCurrent -= 1;
                intellectAmount.text = currentCharacterIntellect.ToString();
                break;
            case 3:
                currentCharacterLuck += 1;
                partyStatsPointsCurrent -= 1;
                luckAmount.text = currentCharacterLuck.ToString();
                break;
        }
        partyStatsPointsText.text = partyStatsPointsCurrent.ToString();
        CheckForReadyness();
    }

    public float GetMaxSelfEsteemAmount()
    {
        return currentCharacterCharisma * (10 + partyLevel - 1) + GlobalStats.AllCharisma;
    }
    public float GetMaxStaminaAmount()
    {
        return currentCharacterAgility * (10 + partyLevel - 1) + GlobalStats.AllAgility;
    }



    public void SubstructStatsPoints(int statIndex)
    {
        if (partyStatsPointsCurrent >= partyStatsPoints) { partyStatsPointsCurrent = partyStatsPoints; 
            partyStatsPointsText.text = partyStatsPointsCurrent.ToString(); return; }

        switch (statIndex)
        {
            case 0:
                if (currentCharacterCharisma <= initialAmountOfStats) return;
                currentCharacterCharisma -= 1;
                partyStatsPointsCurrent += 1;
                charismaAmount.text = currentCharacterCharisma.ToString();
                selfEsteemCurrent.text = ((int)GetMaxSelfEsteemAmount()).ToString();
                break;
            case 1:
                if (currentCharacterAgility <= initialAmountOfStats) return;
                currentCharacterAgility -= 1;
                partyStatsPointsCurrent += 1;
                agilityAmount.text = currentCharacterAgility.ToString();
                staminaCurrent.text = ((int)GetMaxStaminaAmount()).ToString();
                break;
            case 2:
                if (currentCharacterIntellect <= initialAmountOfStats) return;
                currentCharacterIntellect -= 1;
                partyStatsPointsCurrent += 1;
                intellectAmount.text = currentCharacterIntellect.ToString();
                break;
            case 3:
                if (currentCharacterLuck <= initialAmountOfStats) return;
                currentCharacterLuck -= 1;
                partyStatsPointsCurrent += 1;
                luckAmount.text = currentCharacterLuck.ToString();
                break;
        }
        partyStatsPointsText.text = partyStatsPointsCurrent.ToString();
       
    }

    void CheckForReadyness()
    {
        if (currentCharacter.heroComplete) { return; }
        if (partyStatsPointsCurrent > 0) {/* print("You need to spend all party point " + partyStatsPointsCurrent);*/ return; }
        if (portrait == defaultPortrait || ! nameChanged) { /*print("Choose a name and avatar for your hero");*/ return; }
        if (instrumentImage.sprite == cellBackground || spellImage.sprite == cellBackground) { return; }
        confirmButton.interactable = true;
    }


    public void ConfirmCharacterChanges()
    {
        currentCharacter.avatar = portrait;
        currentCharacter.Charisma = currentCharacterCharisma;
        currentCharacter.Agility = currentCharacterAgility;
        currentCharacter.Intellect = currentCharacterIntellect;
        currentCharacter.Luck = currentCharacterLuck;
        currentCharacter.heroComplete = true;
        currentCharacter.gameObject.GetComponent<CharacterButton>().SetButtonAvatar(currentCharacter.avatar);
        confirmButton.interactable = false;
/*        currentCharacter.characterSpellBook.Add(chosenInstrument);
        currentCharacter.characterSpellBook.Add(chosenSpell);*/
        currentCharacter.heroName = heroName;
/*        currentCharacter.StartSpeechEvent(3);*/
        EventOfSpeechAnimation();
   /*     foreach(CharacterBuild c in party.partyCharacters)
        {
            currentCharacter.notesTrained.Clear();
            if (!c.heroComplete) return;
            foreach (string s in currentCharacter.characterSpellBook)
            {
                currentCharacter.ReadNotesFromSpell(s);
            }
        }*/

        startGameButton.interactable = true;
        party.SetPartyState(PartyState.Explore);
    }

    public void ResetCharacterChanges()
    {
        currentCharacter.avatar = defaultPortrait;
        currentCharacter.Charisma = initialAmountOfStats;
        currentCharacter.Agility = initialAmountOfStats;
        currentCharacter.Intellect = initialAmountOfStats;
        currentCharacter.Luck = initialAmountOfStats;
        currentCharacter.heroComplete = false;
        currentCharacter.gameObject.GetComponent<CharacterButton>().SetButtonAvatar(currentCharacter.avatar);
       // currentCharacter.characterSpellBook.Clear();
        currentCharacter.heroName = "";
        GetCharacterFromButton(currentCharacter);
    }

    void SetCharacterCurrentStats()
    {
        currentCharacterCharisma = currentCharacter.Charisma;
        charismaAmount.text = currentCharacterCharisma.ToString();
        currentCharacterAgility = currentCharacter.Agility;
        agilityAmount.text = currentCharacter.Agility.ToString();
        currentCharacterIntellect = currentCharacter.Intellect;
        intellectAmount.text = currentCharacterIntellect.ToString();
        currentCharacterLuck = currentCharacter.Luck;
        luckAmount.text = currentCharacter.Luck.ToString();
        selfEsteemCurrent.text = ((int)GetMaxSelfEsteemAmount()).ToString();
        staminaCurrent.text = ((int)GetMaxStaminaAmount()).ToString();

    }


    public void GetCharacterFromButton(CharacterBuild character)
    {
        if (party.GetPartyState() != PartyState.Creator) return;
        confirmButton.interactable = false;
        if (character.heroComplete) { 
            currentCharacter = character;
            SetCharacterCurrentStats();
            partyStatsPointsCurrent = 0;
            heronameText.text = character.heroName;
            heronameInputText.text = character.heroName;
            SetCharacterPortrait(character.avatar);
           /* SetInstrument(spellBookIndexes.GetSpellScriptable(currentCharacter.characterSpellBook[0]));
            SetSpell(spellBookIndexes.GetSpellScriptable(currentCharacter.characterSpellBook[1]));*/
            partyStatsPointsText.text = partyStatsPointsCurrent.ToString() ;
            return;
        }
       /* SetStats(character, character.avatar, character.Charisma,
            character.Agility,
            character.Intellect,
            character.Luck,*/
            //character.GetMaxSelfEsteemAmount(), 0, character.heroName, character.characterSpellBook);


        SetCharacterCurrentStats();

        partyStatsPointsCurrent = partyStatsPoints;
        partyStatsPointsText.text = partyStatsPointsCurrent.ToString();
        portrait = defaultPortrait;
        heroName = "";
        heronameText.text = "";
        heronameInputText.text = "";
        nameChanged = false;
        instrumentImage.sprite = cellBackground;
        spellImage.sprite = cellBackground;
        animatedAvatar.m_SpriteArray = defaultPortrait;
    }

    public void SetStats(CharacterBuild character, AnimatedAvatar characterPortrait, int charisma, int agility, int intellect,
    int luck, float selfEsteem, float stamina, string heroName, List<string> characterSpellBook)
    {
        currentCharacter = character;
        charismaAmount.text = charisma.ToString();
        agilityAmount.text = agility.ToString();
        intellectAmount.text = intellect.ToString();
        luckAmount.text = luck.ToString();
        selfEsteemCurrent.text = selfEsteem.ToString();
        staminaCurrent.text = stamina.ToString();
    }

    public void SetCharacterPortrait(AnimatedAvatar avatar)
    {
        animatedAvatar.m_SpriteArray = avatar;
        portrait = avatar;
/*        currentCharacter.voiceHarmonics = avatar.highLowHarmonics;
        currentCharacter.StartSpeechEvent(0);*/
        EventOfSpeechAnimation();
        CheckForReadyness();
    }

    public void EventOfSpeechAnimation()
    {
        StartCoroutine(TalkABitAnimation());
    }

    IEnumerator TalkABitAnimation()
    {
        animatedAvatar.animationIndex = 1;
        if(currentCharacter.avatar !=defaultPortrait)
        currentCharacter.gameObject.GetComponent<UISpriteAnimation>().animationIndex = 1;
        yield return new WaitForSeconds(0.5f);
        animatedAvatar.animationIndex = 0;
        if (currentCharacter.avatar != defaultPortrait)
        currentCharacter.gameObject.GetComponent<UISpriteAnimation>().animationIndex = 0;
    }

/*    public void SetInstrument(SpellScriptable spell)
    {
        instrumentImage.sprite = spell.spell.sprite;
        chosenInstrument = spell.spell.spellNameIndex;
        CheckForReadyness();
    }

    public void SetSpell(SpellScriptable spell)
    {
        //Check for rectTransform active Instrument or spell
        spellImage.sprite = spell.spell.sprite;
        chosenSpell = spell.spell.spellNameIndex;
        CheckForReadyness();
    }*/



}
