# SkiaSharpTestSolution

Last Edited: December 26, 2023 10:24 AM

# Adding new Svgs for test application

To test the new Svg’s with the test application

1. Add svg resource as an embedded 
2. Make Changes on **Resources.resx**  and **Resources.Designer.cs**

```xml
<data name="not_working" type="System.Resources.ResXFileRef, System.Windows.Forms">
    <value>Resources\not_working.svg;System.Byte[], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </data>
```

```csharp
public static byte[] not_working {
            get {
                object obj = ResourceManager.GetObject("not_working", resourceCulture);
                return ((byte[])(obj));
            }
        }
```

1. Add the resource name in the Aboutviewmodel.cs 

```csharp
foreach (var topogramSvg in renderedTopogramSvgs)
            {
                try
                {
                    float componentXPosition = topogramSvg.XPosition;
                    float componentYPosition = topogramSvg.YPosition;
                    
                    float valueXPosition = 0;
                    float valueYPosition = 0;
                    svgName = topogramSvg.SVGFileName;
                    if (string.IsNullOrEmpty(svgName))
                    {
                        valueXPosition = componentXPosition * ratio;
                        valueYPosition = info.Height - (componentYPosition + 35) * ratio;
                    }
                    else
                    {
                        var buffer = Resources.not_working; // Add resource here
                        var xml = Encoding.UTF8.GetString(buffer);
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(xml);            // add the svg name
                        parser = new SVGParser(svgName: "not_working", XmlDoc: xmlDoc, topogramsvg: topogramSvg);
                        int svgFrameIndex = topogramSvg.FrameIndex;
                        MonitoringGraphicsSVG monitoringGraphicsSVG = parser.GetMonitoringGraphicsSVG();
                        if (monitoringGraphicsSVG == null || monitoringGraphicsSVG.GetFrame(svgFrameIndex) == null) // missing svg or frame
                        {
                            topogramSvg.IsRendered = false;
                            continue;
                        }
                        //A graphic with coordinates 0,0 is placed to the left edge of the screen, 45 points above the visible bottom line.
                        float svgXPosition = 0.0f;
                        float svgYPosition = 0.0f;

                        svgXPosition = (componentXPosition + monitoringGraphicsSVG.GetFrameXValue(svgFrameIndex) + monitoringGraphicsSVG.GetXValue()) * ratio;
                        svgYPosition = info.Height - (componentYPosition - monitoringGraphicsSVG.GetFrameYValue(svgFrameIndex) - monitoringGraphicsSVG.GetYValue() + 45) * ratio;

                        var stream = monitoringGraphicsSVG.GetStream(svgFrameIndex: svgFrameIndex);
                        var svg = new Svg.Skia.SKSvg(); //initialise the new library
                        svg.Load(stream);
                        var matrix = SKMatrix.CreateScaleTranslation(ratio, ratio, svgXPosition, svgYPosition);
                        canvas.DrawPicture(svg.Picture, ref matrix);

                        float svgWidth = monitoringGraphicsSVG.GetWidth();
                        float svgHeight = monitoringGraphicsSVG.GetHeight();
                        valueXPosition = svgXPosition + (svgWidth * ratio) / 2;
                        valueYPosition = svgYPosition + (svgHeight * ratio) + 35;
                    }
                    topogramSvg.LabelXPosition = valueXPosition;
                    topogramSvg.LabelYPosition = valueYPosition;
                   // DisplayLabelAndValue(topogramSvg, canvas, ratio);
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                }
            }
```

1. Hard code the frame limits for rendering the animation. Frame limits can be found using the svg naming.

bsp_pump_3_2.svg → 3 Frames for 2 State
Total ⇒ 3 x 2 = 6 Frames 

```csharp
foreach (var item in topogramComponents)
            {
                topogramSvgs.Add(new TopogramSvg
                {
                    CurrentState = 1,
                    FrameIndex = 0,
                    FrameLimit = 5, 
                    NOfFrames = 5,
                    NOfStates = 1,
                    SVGFileName = item.GetFileName(),
                    AnimationType = item.GetAnimationType(),
                    XPosition = (float)item.GetXPosition(),
                    YPosition = (float)item.GetYPosition(),
                    Name = item.GetName(),
                    DisplayName = item.GetDisplayName(),
                    NumericDisplay = item.GetNumericDisplay(),
                    Dependencies = item.GetDependencies() == "" ? new string[0] : item.GetDependencies().Split(','),
                    Type = item.GetComponentType(),
                    Values = item.GetValues(),
                    IsRendered = !(item.GetComponentType() == TopogramComponentType.PASSIVE && item.GetValues().Count == 0),
                    IsStateValueChanging = false,
                });

            } // use same svg 2length
            renderedTopogramSvgs = topogramSvgs;
        }
```

1. ****SVGParser****  is modified and it takes **XmlDocument** additional argument.

```csharp
public SVGParser(string svgName, XmlDocument XmlDoc, TopogramSvg topogramsvg)
        {
            _xmldoc = XmlDoc;
            _svgName = svgName;
            _NOfframe = topogramsvg.NOfFrames;
            _NOfStates = topogramsvg.NOfStates;

            ParseSVG();
        }
```

1. Topogram remains the same, If issues with animation check the frame-limit. It may cause the issue in rendering.
2. Animation is mocked for test purpose.

# svg.skia (New Extension to SkiaSharp Lib)

Refer

[https://github.com/wieslawsoltes/Svg.Skia](https://github.com/wieslawsoltes/Svg.Skia)