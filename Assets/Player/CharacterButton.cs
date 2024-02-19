using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CharacterButton : MonoBehaviour
{
    [SerializeField] UISpriteAnimation characterAvatarAnimation;
    [SerializeField] Image activeRing;
    [SerializeField] Sprite active, notActive;
    [SerializeField] PartyManager party;
    public RectTransform SelfEsteemIndicator, StaminaIndicator;

    float fullHeightSelEsteem, fullHeightStamina;


    private void Start()
    {
        if (SelfEsteemIndicator !=null)
        fullHeightSelEsteem =SelfEsteemIndicator.rect.height;
        if (StaminaIndicator !=null)
        fullHeightStamina = StaminaIndicator.rect.height;
        if (characterAvatarAnimation != null)
            characterAvatarAnimation.m_SpriteArray = GetComponent<CharacterBuild>().avatar;
        party.setActiveCharacterButton.AddListener(SetButtonActive);
    }
    public void SetSelfEsteemIndicator(float amountMax,float amount)
    {
        if (amountMax !=0)
        SelfEsteemIndicator.sizeDelta = new Vector2(SelfEsteemIndicator.sizeDelta.x, (amount*fullHeightSelEsteem ) /  amountMax);
    }

    public void InitCharacterButton()
    {
        fullHeightSelEsteem = SelfEsteemIndicator.rect.height;
        fullHeightStamina = StaminaIndicator.rect.height;
    }

    public void SetButtonActive(CharacterBuild character)
    {

        if (gameObject.GetComponent<CharacterBuild>() == character)
        {
            activeRing.sprite = active;
        }
        else
        {
            activeRing.sprite = notActive;
        }
    }

    public void SetButtonAvatar(AnimatedAvatar avatar)
    {
        characterAvatarAnimation.m_SpriteArray = avatar;
    }
}
