using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChoiceButtonComponent : MonoBehaviour
{
    public Action choiceAction;
    public Button thisButton;
    public TextMeshProUGUI reply;
    public MessageSO messageSO;

}
