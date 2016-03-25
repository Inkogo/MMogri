using System;

namespace MMogri.Utils
{
    [System.Serializable]
    public class Curve
    {
        float x0, x1, x2, x3;

        public Curve() { }

        public Curve(float x0, float x1, float x2, float x3)
        {
            this.x0 = x0;
            this.x1 = x1;
            this.x2 = x2;
            this.x3 = x3;
        }

        public float EvalValue(float t)
        {
            return (float)(
                x0 * Math.Pow((1 - t), 3) +
                x1 * 3 * t * Math.Pow((1 - t), 2) +
                x2 * 3 * Math.Pow(t, 2) * (1 - t) +
                x3 * Math.Pow(t, 3)
            );
        }
    }
}