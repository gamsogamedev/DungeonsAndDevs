using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(ScriptableDialogue))]
public class DialogueInspector : Editor
{
    public VisualTreeAsset xml_scheme;
}
