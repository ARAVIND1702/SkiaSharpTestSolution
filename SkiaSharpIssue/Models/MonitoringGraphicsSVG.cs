using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Xamarin.Forms;

namespace SkiaSharpIssue.Models
{
    internal class MonitoringGraphicsSVG
    {
        private XmlNode _frames;
        private float _width;
        private float _height;
        private float _x;
        private float _y;

        public MonitoringGraphicsSVG(XmlNode frames, float width, float height, float x, float y)
        {
            _frames = frames;
            _width = width;
            _height = height;
            _x = x;
            _y = y;
        }

        public XmlNode GetFrame(int index)
        {
            return _frames.ChildNodes[index];
        }

        public float GetFrameXValue(int svgFrameIndex)
        {
            XmlNode frame = GetFrame(svgFrameIndex);
            float x = 0;
            if (frame.Attributes["x"] != null)
            {
                var isXValueParsable = float.TryParse(frame.Attributes["x"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out x);
                if (isXValueParsable)
                    return x;
            }
            return x;
        }

        public float GetFrameYValue(int svgFrameIndex)
        {
            XmlNode frame = GetFrame(svgFrameIndex);
            float y = 0;
            if (frame.Attributes["y"] != null)
            {
                var isYValueParsable = float.TryParse(frame.Attributes["y"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out y);
                if (isYValueParsable)
                    return y;
            }
            return y;
        }

        //public Stream GetStream(int svgFrameIndex)
        //{
        //    try
        //    {
        //        XmlNode frame = GetFrame(svgFrameIndex);
        //        if (frame == null) {
        //            Console.WriteLine("frame is empty aravind");

        //        }
        //        string frameString = frame.InnerXml.ToString();
        //        try
        //        {
        //            frameString = Regex.Replace(frameString, "(<a:svg.*?>)|(</a:svg>)", "");
        //        } 
        //        catch (Exception ex)
        //        {
        //            throw new Exception("Failed to get stream for svg element:" + ex.Message);
        //        }
        //        string frameStringWithStartRoot = frameString.Insert(0, "<root>");
        //        string frameStringWithEndRoot = frameStringWithStartRoot.Insert(frameStringWithStartRoot.Length, "</root>");
        //        var stream = new MemoryStream();
        //        var writer = new StreamWriter(stream);
        //        writer.Write(frameStringWithEndRoot);
        //        writer.Flush();
        //        stream.Position = 0;
        //        return stream;

        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Failed to get stream for svg element:" + ex.Message);
        //    }
        //}

        public Stream GetStream(int svgFrameIndex)
        {
            try
            {
                XmlNode frame = GetFrame(svgFrameIndex);

                if (frame == null)
                {
                    Console.WriteLine("Frame is empty.");
                    return null; // Return null or handle the case where the frame is empty.
                }

                // Create an XmlDocument to manipulate the XML content
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(frame.OuterXml);

                // Remove namespaces from all elements
                RemoveNamespaces(xmlDoc.DocumentElement);

                // Convert the modified XML content back to a stream
                var stream = new MemoryStream();
                xmlDoc.Save(stream);
                stream.Position = 0;

                return stream;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to get stream for svg element: " + ex.Message);
            }
        }

        // Recursive method to remove namespaces from all elements
        private void RemoveNamespaces(XmlNode node)
        {
            if (node.NodeType == XmlNodeType.Element)
            {
                // Remove the namespace prefix
                node.Prefix = string.Empty;

                // Remove namespaces from attributes
                foreach (XmlAttribute attribute in node.Attributes)
                {
                    if (attribute.Name.StartsWith("xmlns"))
                    {
                        // Preserve the xmlns attribute for the root element
                        if (node == node.OwnerDocument.DocumentElement && attribute.Name == "xmlns")
                        {
                            continue;
                        }

                        // Remove other xmlns attributes
                        attribute.OwnerElement.Attributes.Remove(attribute);
                    }
                    else
                    {
                        attribute.Prefix = string.Empty;
                    }
                }

                // Recursively process child nodes
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    RemoveNamespaces(childNode);
                }
            }
        }

        public float GetHeight()
        {
            return _height;
        }

        public float GetWidth()
        {
            return _width;
        }

        public float GetXValue()
        {
            return _x;
        }

        public float GetYValue()
        {
            return _y;
        }
    }
}
