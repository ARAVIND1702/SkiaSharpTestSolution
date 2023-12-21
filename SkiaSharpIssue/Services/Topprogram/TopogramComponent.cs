using System.Collections.Generic;

namespace SkiaSharpIssue.Services.Topprogram
{
    public enum TopogramComponentType
    {
        NONE,
        SENSOR,
        CONSUMER,
        PASSIVE,
    }

    public class TopogramComponent
    {
        private TopogramComponentType _type;
        private string _name;
        private string _bitmap;
        private string _values;
        private int _numberOfFrames;
        private string _icon;
        private string _animationType;
        private int _xPosition;
        private int _yPosition;
        private string _numericDisplay;
        private string _displayName;
        private string _dependencies;
        private float _valueDisplayXPosition;
        private float _valueDisplayYPosition;


        public TopogramComponent(TopogramComponent component)
        {
            _type = component._type;
            _name = component._name;
            _bitmap = component._bitmap;
            _name = component._name;
            _values = component._values;
            _numberOfFrames = component._numberOfFrames;
            _xPosition = component._xPosition;
            _yPosition = component._yPosition;
            _numericDisplay = component._numericDisplay;
            _displayName = component._displayName;
            _dependencies = component._dependencies;
            _animationType = component._animationType;
        }

        public TopogramComponent()
        {
            _type = TopogramComponentType.NONE;
            _name = "";
            _bitmap = "bsp_pump_3_2.svg";
            _name = "";
            _values = "";
            _numberOfFrames = -1;
            _animationType = "";
            _xPosition = -1;
            _yPosition = -1;
            _numericDisplay = "";
            _displayName = "";
            _dependencies = "";

        }

        public TopogramComponent(TopogramComponentType type, string name, string bitmap, string values, int numberOfFrames, string icon, string animationType,
            int xPosition, int yPosition, string numericDisplay, string displayName, string dependencies)
        {
            _type = type;
            _name = name;
            _bitmap = bitmap;
            _name = name;
            _values = values;
            _numberOfFrames = numberOfFrames;
            _animationType = animationType;
            _xPosition = xPosition;
            _yPosition = yPosition;
            _numericDisplay = numericDisplay;
            _displayName = displayName;
            _dependencies = dependencies;
        }

        public string GetBitMap()
        {
            return _bitmap;
        }

        public int GetXPosition()
        {
            return _xPosition;
        }

        public int GetYPosition()
        {
            return _yPosition;
        }

        public TopogramComponentType GetComponentType()
        {
            return _type;
        }

        public string GetDependencies()
        {
            return _dependencies;
        }

        public string GetName()
        {
            return _name;
        }
        public void SetValueDisplayXPosition(float valueDisplayXPosition)
        {
            _valueDisplayXPosition = valueDisplayXPosition;
        }
        public float GetValueDisplayXPosition()
        {
            return _valueDisplayXPosition;
        }
        public void SetValueDisplayYPosition(float valueDisplayYPosition)
        {
            _valueDisplayYPosition = valueDisplayYPosition;
        }
        public float GetValueDisplayYPosition()
        {
            return _valueDisplayYPosition;
        }

        public string GetFileName()
        {
            if (string.IsNullOrEmpty(_bitmap))
                return string.Empty;
            return _bitmap + ".svg";
        }

        public int GetNumberOfStates()
        {
            var splittedSvgFileName = _bitmap.Split(new char[] { '_', '.' });
            var statesParsed = int.TryParse(splittedSvgFileName[splittedSvgFileName.Length - 1], out var states);
            if (!statesParsed)
                return 0;
            return states;
        }

        public int GetNumberOfFrames()
        {
            if (_numberOfFrames > 0)
                return _numberOfFrames;
            var splittedSvgFileName = _bitmap.Split(new char[] { '_', '.' });
            bool framesParsed = false;
            int frames = 0;
            if (splittedSvgFileName.Length > 2)
                framesParsed = int.TryParse(splittedSvgFileName[splittedSvgFileName.Length - 2], out frames);
            if (!framesParsed)
                return 0;
            return frames;
        }

        public string GetDisplayName()
        {
            return _displayName;
        }

        public string GetNumericDisplay()
        {
            return _numericDisplay;
        }

        public List<int> GetValues()
        {
            if (!string.IsNullOrEmpty(_values))
            {
                var values = _values.Split(',');
                List<int> valuesList = new List<int>();
                for (int i = 0; i < values.Length; i++)
                {
                    if (int.TryParse(values[i], out var value))
                    {
                        valuesList.Add(value);
                    }
                }
                return valuesList;
            }
            return new List<int>();
        }

        public string GetAnimationType()
        {
            return _animationType;
        }

    }
}