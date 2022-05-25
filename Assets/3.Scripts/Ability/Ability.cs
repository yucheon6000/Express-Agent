using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public abstract class Ability : MonoBehaviour
{
    public class OnClickAbilityEvent : UnityEvent<Ability> { }

    [Header("[Ability]")]
    [SerializeField]
    protected string abilityName;
    [SerializeField]
    protected string abilitySubName;
    [Multiline]
    [SerializeField]
    protected string abilityDescription;
    public string AbilityDescription => abilityDescription;

    public OnClickAbilityEvent onClickAbilityEvent = new OnClickAbilityEvent();

    [Header("[UI]")]
    [SerializeField]
    protected TextMeshProUGUI textName;
    [SerializeField]
    protected TextMeshProUGUI textSubName;
    [SerializeField]
    protected TextMeshProUGUI textDescription;

    protected Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        if (button)
            button.onClick.AddListener(OnClickAbility);

        if (textName)
            textName.text = abilityName;
        if (textSubName)
            textSubName.text = abilitySubName;
        if (textDescription)
            textDescription.text = abilityDescription;
    }

    public virtual void OnClickAbility()
    {
        onClickAbilityEvent.Invoke(this);
    }
}
