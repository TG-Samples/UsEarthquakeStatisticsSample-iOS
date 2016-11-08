using System;

namespace MapSuiteEarthquakeStatistics
{
    public class RangeChangedEventArgs : EventArgs
    {
        private nfloat lowerValue;
        private nfloat upperValue;

        public RangeChangedEventArgs(nfloat lowerValue, nfloat upperValue)
        {
            this.lowerValue = lowerValue;
            this.upperValue = upperValue;
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
    }
}