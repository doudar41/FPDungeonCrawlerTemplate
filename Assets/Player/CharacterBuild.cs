using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public class CharacterBuild : MonoBehaviour
{
    public AnimatedAvatar avatar;
    public string heroName;
    
    //Main Stats
    public int Charisma;
    public int Intellect;
    public int Agility;
    public int Luck;
    public bool heroComplete;

    public UnityEvent<CharacterBuild> sendCharacterTo;

    //Depending stats
    float selfEsteemMAX;
    float staminaMAX;
    float selfEsteemCurrent;
    float staminaCurrent;


    CharacterButton characterButton;
    int partyLevel = 1;

    private void Start()
    {
        ////If game doesn't start form save 
        selfEsteemMAX = GetMaxSelfEsteemAmount();
        selfEsteemCurrent = selfEsteemMAX;
        characterButton = transform.GetComponent<CharacterButton>();
    }

    public void SendCharacterOnClick()
    {
        sendCharacterTo.Invoke(this);
    }


    public float GetMaxSelfEsteemAmount()
    {
        return Charisma * (10 + partyLevel-1) + GlobalStats.AllCharisma;
    }

    public float GetCurrentSelfEsteem()
    {
        return selfEsteemCurrent;
    }

    public float GetMaxStaminaAmount()
    {
        return Agility * (10 + partyLevel - 1) + GlobalStats.AllAgility;
    }

    public float GetCurrentStamina()
    {
        return staminaCurrent;
    }

    public void SetCurrentSelfEsteem(float amount)
    {
        selfEsteemCurrent = amount;
    }

    public void AddCurrentSelfEsteem(float amount)
    {
        selfEsteemCurrent += amount;
    }


}

