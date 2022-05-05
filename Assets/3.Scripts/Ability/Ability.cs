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
        button.onClick.AddListener(OnClickAbility);

        textName.text = abilityName;
        textSubName.text = abilitySubName;
        textDescription.text = abilityDescription;
    }

    public virtual void OnClickAbility()
    {
        onClickAbilityEvent.Invoke(this);
    }
}
