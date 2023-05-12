using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MessageSO", menuName = "ScriptableObjects/MessageSO", order = 4)]
public class MessageSO : ScriptableObject
{
    public List<Narrative> narratives;
}

[Serializable]
public class Narrative
{
    [TextArea(4, 10)]
    public string message;
    public List<Choice> choices;    
}

[Serializable]
public class Choice
{
    [TextArea(2, 10)]
    public string choicesText;
    public MessageSO messageAfterChoice;
}