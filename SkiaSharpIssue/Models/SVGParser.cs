
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml;
using Path = System.IO.Path;

namespace SkiaSharpIssue.Models
{
    public class SVGParser
    {
        private MonitoringGraphicsSVG monitoringGraphicsSVG;
        private string _svgName;
        private XmlDocument _xmldoc;
        internal string monitoringGraphicsDirectory = "MonitoringGraphics";
        internal List<string> SvgsForPreservingTransforms = new List<string>
        {
            "plug_1_2.svg"
        };
        public SVGParser(string svgName, XmlDocument XmlDoc)
        {
            _xmldoc = XmlDoc;
            _svgName = svgName;
            ParseSVG();
        }
        public void ParseSVG()
        {
            try
            {
                // IPlatformSpecificServiceLocator locator = Mvx.IoCProvider.Resolve<IPlatformSpecificServiceLocator>();
                // var svgFileDirectory = Path.Combine(locator.GetPlatformSpecificService().getFolder() + "/" + monitoringGraphicsDirectory);
                // var svgPath = Path.Combine(svgFileDirectory, _svgName);
                // var fileExists = File.Exists(svgPath);
                //if (!fileExists)
                //{
                //    svgPath = UtilityFunctions.GetActualFilePath(svgFileDirectory, _svgName, svgPath);
                //}
                //if (fileExists || !string.IsNullOrEmpty(svgPath))
                //{

                        XmlDocument xmlDoc = _xmldoc;
                if(xmlDoc != null) { 
                        // Creating namespace object
                        
                        XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                        nsmgr.AddNamespace("a", "http://www.w3.org/2000/svg");
                        XmlNodeList search;
                        try
                        {//find all nested tags with transform attribute
                            search = xmlDoc.SelectNodes("//@transform", nsmgr); // find all transform nodes
                            foreach (XmlAttribute transformNode in search)
                            {
                                //if parent has  sieblings -> apply transform
                                if (transformNode.Value.Contains("scale") && transformNode.OwnerElement.ParentNode.ParentNode.ChildNodes.Count > 1)
                                {
                                    if (SvgsForPreservingTransforms.Contains(_svgName))
                                        throw new InvalidDataException($"SVG {_svgName} does not get to change dimensions based on its transform attribute");
                                    // iterate over each transform node  attributes
                                    //get scale x y  factors from scale transform
                                    var mxy = transformNode.Value.Split(new char[] { ',', ' ' });
                                    var mx = Convert.ToDouble(mxy[0].Remove(0, 6), CultureInfo.InvariantCulture);
                                    var pos = mxy.Length - 1;
                                    var my = Convert.ToDouble(mxy[pos].Remove(mxy[pos].Length - 1).Trim(), CultureInfo.InvariantCulture);
                                    //svg parent of path
                                    XmlAttributeCollection _parentofPathAttr = transformNode.OwnerElement.FirstChild.Attributes;
                                    //path
                                    XmlAttributeCollection _pathattr = transformNode.OwnerElement.FirstChild.FirstChild.Attributes;
                                    //parent level , multiply x y values of node attribute according to scale
                                    foreach (XmlAttribute _xmlattr in _parentofPathAttr)
                                    {
                                        if (_xmlattr.Name == "x")
                                        {
                                            transformNode.Value = "scale(1, 1)";
                                            _xmlattr.Value = (mx * Convert.ToDouble(_xmlattr.Value, CultureInfo.InvariantCulture)).ToString();
                                        }
                                        if (_xmlattr.Name == "y")
                                        {
                                            transformNode.Value = "scale(1, 1)";
                                            _xmlattr.Value = (my * Convert.ToDouble(_xmlattr.Value, CultureInfo.InvariantCulture)).ToString();
                                        }
                                    }
                                    SKPath skpathorig = SKPath.ParseSvgPathData(transformNode.OwnerElement.FirstChild.FirstChild.Attributes["d"].Value);
                                    string dnew = skpathorig.ToSvgPathData();
                                    transformNode.OwnerElement.FirstChild.FirstChild.Attributes["d"].Value = dnew;
                                }

                            }
                        }
                        catch (Exception ex)
                        {
                            Console.Write("exception : " + ex.Message);
                        }
                        float width = 0;
                        float height = 0;
                        float x = 0;
                        float y = 0;
                        bool isWidthValueParsable = false;
                        bool isHeightValueParsable = false;
                        var svg = xmlDoc.GetElementsByTagName("a:svg")[0];
                        XmlNodeList svgFrames = xmlDoc.GetElementsByTagName("a:svg");
                        if (svg == null)
                        {
                            svg = xmlDoc.GetElementsByTagName("svg")[0];
                            svgFrames = xmlDoc.GetElementsByTagName("svg");
                        }
                        var enumerator = svgFrames.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            if (((XmlNode)enumerator.Current).Attributes["width"] != null)
                                isWidthValueParsable = float.TryParse(((XmlNode)enumerator.Current).Attributes["width"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out width);
                            if (((XmlNode)enumerator.Current).Attributes["height"] != null)
                                isHeightValueParsable = float.TryParse(((XmlNode)enumerator.Current).Attributes["height"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out height);
                            if (((XmlNode)enumerator.Current).Attributes["x"] != null)
                                float.TryParse(((XmlNode)enumerator.Current).Attributes["x"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out x);
                            if (((XmlNode)enumerator.Current).Attributes["y"] != null)
                                float.TryParse(((XmlNode)enumerator.Current).Attributes["y"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out y);
                            if (isWidthValueParsable || isHeightValueParsable)
                                break;
                        }
                        if (isHeightValueParsable && isWidthValueParsable)
                            monitoringGraphicsSVG = new MonitoringGraphicsSVG(svg, width, height, x, y);
                        else
                            throw new SVGParserException("SVG height and width could not be parsed for " + _svgName);
                    }
                    else
                    {
                        Console.Write("Missing SVG: " + _svgName);
                    }
                
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to get stream for svg element:" + ex);
            }
        }
        public MonitoringGraphicsSVG GetMonitoringGraphicsSVG()
        {
            return monitoringGraphicsSVG;
        }
    }
}