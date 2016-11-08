using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;
using System;
using System.Drawing;

namespace MapSuiteEarthquakeStatistics
{
    [Register("UIRangeSlider")]
    public class UIRangeSlider : UIControl
    {
        private string name;
        private nfloat minValue;
        private nfloat maxValue;
        private nfloat lowerValue;
        private nfloat upperValue;

        private CALayer trackLayer;
        private CALayer rangeLayer;
        private CERangeSliderKnobLayer upperKnobLayer;
        private CERangeSliderKnobLayer lowerKnobLayer;
        private CGPoint previousTouchPoint;

        private UIColor trackColor;
        private nfloat curvaceousness;

        private nfloat knobWidth;
        private nfloat usableTrackLength;

        public event EventHandler<RangeChangedEventArgs> RangeChanged;

        public UIRangeSlider(RectangleF frame)
            : this(frame, 0, 10, 2, 8)
        {
        }

        public UIRangeSlider(RectangleF frame, float minValue, float maxValue, float lowerValue, float upperValue)
            : base(frame)
        {
            this.upperValue = upperValue;
            this.lowerValue = lowerValue;
            this.minValue = minValue;
            this.maxValue = maxValue;
            Initialize();
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public nfloat LowerValue
        {
            get { return lowerValue; }
            set { lowerValue = value; }
        }

        public nfloat UpperValue
        {
            get { return upperValue; }
            set { upperValue = value; }
        }

        public nfloat Curvaceousness
        {
            get { return curvaceousness; }
            set { curvaceousness = value; }
        }

        public UIColor TrackColor
        {
            get { return trackColor; }
            set { trackColor = value; }
        }

        public override bool BeginTracking(UITouch uitouch, UIEvent uievent)
        {
            previousTouchPoint = uitouch.LocationInView(this);
            if (lowerKnobLayer.Frame.Contains(previousTouchPoint))
            {
                lowerKnobLayer.Highlight = true;
                lowerKnobLayer.SetNeedsDisplay();
            }
            else if (upperKnobLayer.Frame.Contains(previousTouchPoint))
            {
                upperKnobLayer.Highlight = true;
                upperKnobLayer.SetNeedsDisplay();
            }

            return lowerKnobLayer.Highlight || upperKnobLayer.Highlight;
        }

        public override bool ContinueTracking(UITouch uitouch, UIEvent uievent)
        {
            CGPoint touchPoint = uitouch.LocationInView(this);

            nfloat delta = touchPoint.X - previousTouchPoint.X;
            nfloat valueDelta = (maxValue - minValue) * delta / usableTrackLength;

            previousTouchPoint = touchPoint;

            if (lowerKnobLayer.Highlight)
            {
                lowerValue += valueDelta;
                lowerValue = LimitBounds(lowerValue, minValue, upperValue);
            }
            else if (upperKnobLayer.Highlight)
            {
                upperValue += valueDelta;
                upperValue = LimitBounds(upperValue, lowerValue, maxValue);
            }

            CATransaction.Begin();
            CATransaction.DisableActions = true;
            SetLayerFrames();
            CATransaction.Commit();

            OnRangeChanged();

            return true;
        }

        public override void EndTracking(UITouch uitouch, UIEvent uievent)
        {
            lowerKnobLayer.Highlight = false;
            upperKnobLayer.Highlight = false;

            lowerKnobLayer.SetNeedsDisplay();
            upperKnobLayer.SetNeedsDisplay();
        }

        protected virtual void OnRangeChanged()
        {
            EventHandler<RangeChangedEventArgs> handler = RangeChanged;
            if (handler != null)
            {
                handler(this, new RangeChangedEventArgs(lowerValue, upperValue));
            }
        }

        public nfloat PositionForValue(nfloat value)
        {
            return usableTrackLength * (value - minValue) / (maxValue - minValue) + knobWidth / 2f;
        }

        private void Initialize()
        {
            trackColor = UIColor.FromWhiteAlpha(1f, 1);
            curvaceousness = 1;

            trackLayer = new CALayer();
            trackLayer.BackgroundColor = UIColor.FromRGB(183, 183, 183).CGColor;
            Layer.AddSublayer(trackLayer);

            rangeLayer = new CALayer();
            rangeLayer.BackgroundColor = UIColor.FromRGB(0, 122, 255).CGColor;
            Layer.AddSublayer(rangeLayer);

            upperKnobLayer = new CERangeSliderKnobLayer();
            upperKnobLayer.Name = "Upper";
            upperKnobLayer.Slider = this;
            Layer.AddSublayer(upperKnobLayer);

            lowerKnobLayer = new CERangeSliderKnobLayer();
            lowerKnobLayer.Name = "Lower";
            lowerKnobLayer.Slider = this;
            Layer.AddSublayer(lowerKnobLayer);

            SetLayerFrames();
        }

        private static nfloat LimitBounds(nfloat value, nfloat minValue, nfloat maxValue)
        {
            if (value < minValue) return minValue;
            if (value > maxValue) return maxValue;

            return value;
        }

        private void SetLayerFrames()
        {
            float trackHeight = 2;
            trackLayer.Frame = new CGRect(0, Bounds.Height / 2f - trackHeight / 2f, Bounds.Width, trackHeight);
            trackLayer.SetNeedsDisplay();

            knobWidth = Bounds.Height;
            usableTrackLength = Bounds.Size.Width - knobWidth;

            nfloat upperKnobCentre = PositionForValue(upperValue);
            SetKnobLayer(upperKnobCentre, upperKnobLayer);

            nfloat lowerKnobCentre = PositionForValue(lowerValue);
            SetKnobLayer(lowerKnobCentre, lowerKnobLayer);

            rangeLayer.Frame = new CGRect(lowerKnobCentre, Bounds.Height / 2f - trackHeight / 2f, upperKnobCentre - lowerKnobCentre, trackHeight);
        }

        private void SetKnobLayer(nfloat centerPosition, CERangeSliderKnobLayer layer)
        {
            layer.Frame = new CGRect(centerPosition - knobWidth / 2f, 0, knobWidth, knobWidth);
            layer.ShadowOffset = new SizeF(0, 3);
            layer.ShadowOpacity = 0.4f;
            layer.ShadowColor = UIColor.Gray.CGColor;
            layer.SetNeedsDisplay();
        }
    }
}