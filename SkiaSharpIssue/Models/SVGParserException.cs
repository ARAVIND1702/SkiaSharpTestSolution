using System;

namespace  SkiaSharpIssue.Models

{
    public class SVGParserException : Exception
    {
        public SVGParserException()
        {
        }

        public SVGParserException(string message)
       : base(message)
        {
        }

        public SVGParserException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}