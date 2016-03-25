namespace MMogri.Utils
{
    [System.Serializable]
    public class ScalingValue
    {
        float minValue;
        float maxValue;
        Curve curve;

        public ScalingValue() { }

        public ScalingValue(float min, float max, float x0, float x1, float x2, float x3)
        {
            minValue = min;
            maxValue = max;
            curve = new Curve(x0, x1, x2, x3);
        }

        public ScalingValue(float min, float max, Curve c)
        {
            minValue = min;
            maxValue = max;
            curve = c;
        }

        public float GetValue(int lvl)
        {
            return curve.EvalValue(lvl / 100f) * (maxValue - minValue) + minValue;
        }

        public Curve GetCurve
        {
            get
            {
                return curve;
            }
        }
    }
}