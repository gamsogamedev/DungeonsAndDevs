using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public static class WorldStateReader
{
    private static XmlDocument _worldState;
    
    public static void Initialize()
    {
        _worldState = new XmlDocument();

        var worldStateFile = Resources.Load("worldState") as TextAsset;
        if (worldStateFile is null)
        {
            Debug.LogError("World State file not found");    
            return;
        }
        
        _worldState.Load(new StringReader(worldStateFile.text));
    }

    public static void EditNode(string nodeName, string nodeValue)
    {
        var root = _worldState.SelectSingleNode("root") ?? _worldState.CreateElement("root");

        var nodeSearch = root.SelectSingleNode(nodeName);
        if (nodeSearch is not null) nodeSearch.InnerText = nodeValue;
        else
        {
            var node = _worldState.CreateElement(nodeName);
            node.InnerText = nodeValue;
            root.AppendChild(node);
        }
        
        _worldState.Save(Application.dataPath + "/Resources/worldState.xml");
    }

    public static string RetrieveNodeValue(string nodeName)
    {
        var root = _worldState.SelectSingleNode("root");
        if (root is null)
        {
            Debug.Log("Could not find root in file");
            return null;
        }

        var nodeSearch = root.SelectSingleNode(nodeName);
        if (nodeSearch is null)
        {
            Debug.Log($"Could not find <{nodeName}> in file");
            return null;
        }

        return nodeSearch.InnerText;
    }

    public static void RemoveNode(string nodeName)
    {
        var root = _worldState.SelectSingleNode("root");
        if (root is null)
        {
            Debug.Log("Could not find root in file");
            return;
        }

        var nodeSearch = root.SelectSingleNode(nodeName);
        if (nodeSearch is null)
        {
            Debug.Log($"Could not find <{nodeName}> in file");
            return;
        }

        root.RemoveChild(nodeSearch);
    }
}
