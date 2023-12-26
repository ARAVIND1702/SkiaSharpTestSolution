using HarfBuzzSharp;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using SkiaSharpIssue.Models;
using SkiaSharpIssue.Services.Topprogram;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using System.Xml;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SkiaSharpIssue.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        Stopwatch stopwatch = new Stopwatch();
        List<TopogramSvg> renderedTopogramSvgs = new List<TopogramSvg>();
        List<TopogramSvg> topogramSvgs = new List<TopogramSvg>();
        List<TopogramComponent> topogramComponents = new List<TopogramComponent>();

        public AboutViewModel()
        {
            Title = "About";
            OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://aka.ms/xamarin-quickstart"));
            Screen();

        }
        //private AnimationController animationController;
        public ICommand OpenWebCommand { get; }
        public Action RefreshViewAction { get; set; }
        // custom topprogramcomponent
        TopogramComponent SVG_0 = new TopogramComponent();
        TopogramComponent SVG_1 = new TopogramComponent();
        TopogramComponent SVG_2 = new TopogramComponent();
        TopogramComponent SVG_3 = new TopogramComponent();
        TopogramComponent SVG_4 = new TopogramComponent();
        TopogramComponent SVG_5 = new TopogramComponent();

        //
        private readonly List<TopogramSvg> animatedSvgs;
        private readonly Timer animationTimer;
        private readonly int animationInterval = 100; // Set the interval based on your requirements

        

        public void Screen()
        {

            topogramComponents.Add(SVG_0);
            //topogramComponents.Add(SVG_1);
            //topogramComponents.Add(SVG_2);
            //topogramComponents.Add(SVG_3);
            //topogramComponents.Add(SVG_4);
            //topogramComponents.Add(SVG_5);
            topogramSvgs = new List<TopogramSvg>();  //by pass by creating an list contains topprogramcomponent obj and now for-each this list and initiliase the this list contains (topprogramsvg) from topprorancomponent func

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

        public  void Paint(Assembly assembly, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKCanvasView instructionsGraphic;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;
            canvas.Clear(SKColors.White);
            string svgName = string.Empty;
            string transcolor = string.Empty;
            SKColor skcolTransp = SKColor.Empty;
            float svgMaxWidth = 242;
            float svgMaxHeight = 310;
            float scaleX = (info.Width / svgMaxWidth) * 0.90f;
            float scaleY = (info.Height / svgMaxHeight) * 0.90f;
            float ratio = Math.Min(scaleX, scaleY);
            SVGParser parser = null; // Declare the parser variable outside the loop


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
                        var buffer = Resources.bsp_pump_3_2; // add resource here
                        var xml = Encoding.UTF8.GetString(buffer);
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(xml);
                        parser = new SVGParser(svgName: "bsp_pump_3_2", XmlDoc: xmlDoc, topogramsvg: topogramSvg);
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
                        var svg = new Svg.Skia.SKSvg(); //initilaise the new lib here
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
            canvas.Flush();
            SKImage skim = surface.Snapshot();
            SKBitmap bmp1 = SKBitmap.Decode(skim.Encode());
            SKRect rb;
            canvas.GetLocalClipBounds(out rb);//this rectangle (due to translation) is the new 0,0 (this  is the most left,top pixel)of the canvas . It is translated (-16,45) from the abslute 0,0

            skcolTransp = bmp1.GetPixel(0, 0);
            transcolor = "#" + skcolTransp.ToString().Substring(3);
            canvas.Clear(SKColors.White); //remove black background from canvas, by setting all pixels  white 
            canvas.Flush();
        
                    
                
                    // return true to continue the timer, false to stop
                    foreach (var topogramSvg in renderedTopogramSvgs)
                    {
                        try
                        {
                            string bitmap = topogramSvg.Name;
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
                                var buffer = Resources.bsp_pump_3_2;
                                var xml = Encoding.UTF8.GetString(buffer);
                                XmlDocument xmlDoc = new XmlDocument();
                                xmlDoc.LoadXml(xml);
                                parser = new SVGParser(svgName: "bsp_pump_3_2", XmlDoc: xmlDoc, topogramsvg: topogramSvg);
                                int svgFrameIndex = topogramSvg.FrameIndex;
                                MonitoringGraphicsSVG monitoringGraphicsSVG = parser.GetMonitoringGraphicsSVG();
                                if (monitoringGraphicsSVG == null || monitoringGraphicsSVG.GetFrame(svgFrameIndex) == null) // missing svg or frame
                                {
                                    topogramSvg.IsRendered = false;
                                    continue;
                                }
                                monitoringGraphicsSVG.ApplyTransparentColor(svgFrameIndex, skcolTransp);
                                //A graphic with coordinates 0,0 is placed to the left edge of the screen, 45 points above the visible bottom line.
                                float svgXPosition = 0.0f;
                                float svgYPosition = 0.0f;

                                svgXPosition = (componentXPosition + monitoringGraphicsSVG.GetFrameXValue(svgFrameIndex) + monitoringGraphicsSVG.GetXValue()) * ratio;
                                svgYPosition = info.Height - (componentYPosition - monitoringGraphicsSVG.GetFrameYValue(svgFrameIndex) - monitoringGraphicsSVG.GetYValue() + 45) * ratio;

                                var stream = monitoringGraphicsSVG.GetStream(svgFrameIndex: svgFrameIndex);
                                var svg = new Svg.Skia.SKSvg();
                                svg.Load(stream);
                                var matrix = SKMatrix.CreateScaleTranslation(ratio, ratio, svgXPosition, svgYPosition);
                             
                                    canvas.DrawPicture(svg.Picture, ref matrix);
                                    // No need to clear the canvas here
                               

                                // No need to clear the canvas here
                                float svgWidth = monitoringGraphicsSVG.GetWidth();
                                float svgHeight = monitoringGraphicsSVG.GetHeight();
                                valueXPosition = svgXPosition + (svgWidth * ratio) / 2;
                                valueYPosition = svgYPosition + (svgHeight * ratio) + 35;
                            }
                            topogramSvg.LabelXPosition = valueXPosition;
                            topogramSvg.LabelYPosition = valueYPosition;
                            // DisplayLabelAndValue(topogramSvg, canvas, ratio);
                            if (topogramSvg.NOfFrames > 1)
                            {
                                ChangeAnimationFrame(animatedSvg: topogramSvg);
                                
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.Write(ex.Message);
                        }
                    }

            canvas.Flush();
                    if (RefreshViewAction != null)
                        RefreshViewAction();

                }







        //// Drawing not working svg
        /*
                    buffer = Resources.not_working;
                    xml = Encoding.UTF8.GetString(buffer);
                    xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(xml);
        // node = xmlDoc.GetElementsByTagName("a:svg")[0];
        // monitoringGraphicsSVG = new MonitoringGraphicsSVG(node, 151.956F, 197.611F, 0, 0);
                    parser = new SVGParser(svgName: "nil", XmlDoc: xmlDoc);
                    monitoringGraphicsSVG = parser.GetMonitoringGraphicsSVG();
                    svgFrameIndex = 0;
                    svgXPosition = (17F + monitoringGraphicsSVG.GetFrameXValue(svgFrameIndex) + monitoringGraphicsSVG.GetXValue()) * ratio;
                    svgYPosition = info.Height - (225F - monitoringGraphicsSVG.GetFrameYValue(svgFrameIndex) - monitoringGraphicsSVG.GetYValue() + 45) * ratio;
                    stream = monitoringGraphicsSVG.GetStream(svgFrameIndex: svgFrameIndex);
                    svg = new Svg.Skia.SKSvg();
                    svg.Load(stream);
                    matrix = SKMatrix.CreateScaleTranslation(ratio, ratio, svgXPosition, svgYPosition);

                        canvas.DrawPicture(svg.Picture, ref matrix);
        */


        private  async void ChangeAnimationFrame(TopogramSvg animatedSvg)
        {
            stopwatch.Start();

            while (true) {


            if (animatedSvg.FrameIndex + 1 <= animatedSvg.FrameLimit)
            {

                animatedSvg.FrameIndex++;
                await Task.Delay(TimeSpan.FromSeconds(1.0 / 30));

            }
            if (animatedSvg.AnimationType == "cyclic" && animatedSvg.FrameIndex + 1 > animatedSvg.FrameLimit)
            {
                animatedSvg.FrameIndex = animatedSvg.CurrentState * animatedSvg.NOfFrames - animatedSvg.NOfFrames;
            }
                if (animatedSvg.FrameIndex == 5) {
                    animatedSvg.FrameIndex = 0;
                }
            }

            
            stopwatch.Stop();




        }

        public  void SetFrames(TopogramSvg animatedSVG)
        {
            animatedSVG.FrameIndex = (animatedSVG.CurrentState - 1) * animatedSVG.NOfFrames;
            animatedSVG.FrameLimit = animatedSVG.CurrentState * animatedSVG.NOfFrames;
            RefreshViewAction();

        }



    }

    public class TopogramSvg
    {
        public string SVGFileName { get; set; }
        public int NOfFrames { get; set; }
        public int NOfStates { get; set; }
        public int CurrentState { get; set; }
        public int FrameIndex { get; set; }
        public int FrameLimit { get; set; }
        public string AnimationType { get; set; }
        public float XPosition { get; set; }
        public float YPosition { get; set; }
        public string Name { get; set; }
        public string LabelValue { get; set; }
        public float LabelXPosition { get; set; }
        public float LabelYPosition { get; set; }
        public string DisplayName { get; set; }
        public string NumericDisplay { get; set; }
        public string[] Dependencies { get; set; }
        public TopogramComponentType Type { get; set; }
        public List<int> Values { get; set; }
        public bool IsRendered { get; set; }
        public int StateValue { get; set; }
        public bool IsStateValueChanging { get; set; }

        public TopogramSvg Clone()
        {
            return new TopogramSvg
            {
                Name = Name,
                XPosition = XPosition,
                YPosition = YPosition,
                SVGFileName = SVGFileName,
                FrameIndex = FrameIndex,
                IsRendered = IsRendered,
                FrameLimit = FrameLimit,
                AnimationType = AnimationType,
                CurrentState = CurrentState,
                NOfFrames = NOfFrames,
                LabelXPosition = LabelXPosition,
                LabelYPosition = LabelYPosition,
                NumericDisplay = NumericDisplay,
                LabelValue = LabelValue,
                IsStateValueChanging = IsStateValueChanging,
                DisplayName = DisplayName,
            };
        }
    }
    
}

