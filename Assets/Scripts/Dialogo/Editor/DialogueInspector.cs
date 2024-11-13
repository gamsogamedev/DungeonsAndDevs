using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(ScriptableDialogue))]
public class DialogueInspector : Editor
{
    public VisualTreeAsset xml_scheme;
    
    public override VisualElement CreateInspectorGUI()
    {
        VisualElement inspector = new VisualElement();
        inspector.Add(new Label("Custom Inspector!"));

        xml_scheme = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/CustomInspectorScheme.uxml");
        inspector = xml_scheme.Instantiate();
        
        return inspector;
    }
}
