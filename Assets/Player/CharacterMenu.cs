using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterMenu : MonoBehaviour
{
    [SerializeField] AnimatedAvatar portrait;
    [SerializeField] Image partyProgressBar;
    [SerializeField] TextMeshProUGUI heroName;
    [SerializeField] TextMeshProUGUI charismaAmount, agilityAmount, intellectAmount, luckAmount;
    [SerializeField] TextMeshProUGUI selfEsteemCurrent, staminaCurrent;
    [SerializeField] TextMeshProUGUI partyLevel, skillPoints;
    [SerializeField] RectTransform mainSpellGrid, quickSpellGrid;
    [SerializeField] TextMeshProUGUI spellName, spellDetails;
    [SerializeField] PartyManager party;
    [SerializeField] GameObject spellButton;
    [SerializeField] CharacterBuild characterDefault;
    bool isFirstLaunch = true;


    //Fully developed chords
    // Spells 
    private void OnEnable()
    {
        if (party.GetPartyState() == PartyState.Explore)
        party.SetPartyState(PartyState.HeroMenu);

        if (isFirstLaunch)
        {
            party.SetHighlithedCharacter(characterDefault);
            isFirstLaunch = false;
        }
    }

    private void OnDisable()
    {
        party.SetPartyState(PartyState.Explore);
    }

    public void SetStats(AnimatedAvatar characterPortrait, int charisma, int agility, int intellect, 
        int luck, float selfEsteem, float stamina, string heroName, List<string> characterSpellBook)
    {
        portrait = characterPortrait;
        charismaAmount.text = charisma.ToString();
        agilityAmount.text = agility.ToString();
        intellectAmount.text = intellect.ToString();
        luckAmount.text = luck.ToString();
        selfEsteemCurrent.text = selfEsteem.ToString();
        staminaCurrent.text = stamina.ToString();
        this.heroName.text = heroName;
        BuildSpellButtons(characterSpellBook); 
    }

    void BuildSpellButtons(List<string> characterSpellBook)
    {
        foreach (Transform t in mainSpellGrid)
        {
            Destroy(t.gameObject);
        }
        spellName.text = "";
        spellDetails.text = "";
/*        foreach (string spellName in characterSpellBook)
        {
            //print("spellname "+spellName);
            GameObject characterSpellButton = Instantiate(spellButton, mainSpellGrid.transform);
            SpellButton buttonSpell = characterSpellButton.GetComponent<SpellButton>();
            buttonSpell.spellStats = spellBookIndexes.spellBookIndexes[spellName];
            buttonSpell.sendSpell.AddListener(GetSpellDesription);
            buttonSpell.GetComponent<Image>().sprite = spellBookIndexes.spellBookIndexes[spellName].sprite;
        }*/
        spellName.text = "";
        spellDetails.text = "";
    }

/*    void GetSpellDesription(string spell)
    {
        spellName.text = spellBookIndexes.spellBookIndexes[spell].spellName;
        spellDetails.text = spellBookIndexes.spellBookIndexes[spell].description;
    }*/

}
