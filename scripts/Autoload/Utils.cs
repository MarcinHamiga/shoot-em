using Godot;
using System;
using System.Xml;
using System.Collections.Generic;

namespace Shootemmono.scripts.Autoload;
[GlobalClass]
public partial class Utils : Node
{
    public override void _Ready()
    {
        GD.Print("Utils autoload initialized.");
    }

    public Dictionary<string, object> ReadXml(string filePath)
    {
        var result = new Dictionary<string, object>();
        if (!FileAccess.FileExists(filePath))
        {
            GD.Print("File not found: " + filePath);
            return result;
        }
        
        var xmlDoc = new XmlDocument();
        xmlDoc.Load(filePath);
        result = ParseXmlNode(xmlDoc.DocumentElement);

        return result;
    }

    private static Dictionary<string, object> ParseXmlNode(XmlNode node)
    {
        var dict = new Dictionary<string, object>();

        if (node.Attributes != null)
        {
            foreach (XmlAttribute attribute in node.Attributes)
            {
                dict[attribute.Name] = attribute.Value;
            }
        }

        foreach (XmlNode child in node.ChildNodes)
        {
            if (child.NodeType == XmlNodeType.Element)
            {
                if (!dict.ContainsKey(child.Name))
                {
                    dict[child.Name] = new List<object>();
                }
                
                ((List<object>)dict[child.Name]).Add(ParseXmlNode(child));
            }
            else if (child.NodeType == XmlNodeType.Text || child.NodeType == XmlNodeType.CDATA)
            {
                dict["#text"] = child.Value.Trim();
            }
        }

        return dict;
    }
}