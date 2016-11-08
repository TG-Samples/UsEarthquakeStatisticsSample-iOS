using System;
using CoreAnimation;
using CoreGraphics;
using UIKit;

namespace MapSuiteEarthquakeStatistics
{
    public class CERangeSliderKnobLayer : CALayer
    {
        private bool highlight;
        private UIRangeSlider slider;

        public bool Highlight
        {
            get { return highlight; }
            set { highlight = value; }
        }

        public UIRangeSlider Slider
        {
            get { return slider; }
            set { slider = value; }
        }

        public override void DrawInContext(CGContext ctx)
        {
            nfloat cornerRadius = Bounds.Height * Slider.Curvaceousness / 2;
            UIBezierPath switchOutline = UIBezierPath.FromRoundedRect(Bounds, cornerRadius);

            ctx.AddPath(switchOutline.CGPath);
            ctx.Clip();

            ctx.SetFillColor(Slider.TrackColor.CGColor);
            ctx.AddPath(switchOutline.CGPath);
            ctx.FillPath();

            ctx.AddPath(switchOutline.CGPath);
            ctx.SetStrokeColor(UIColor.Gray.CGColor);
            ctx.SetLineWidth(0.5f);
            ctx.StrokePath();
        }
    }
}