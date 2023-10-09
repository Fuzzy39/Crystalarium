using Microsoft.Xna.Framework;

namespace CrystalCore.Util.Graphics
{

    /// <summary>
    /// A liniear gradient between several Color Stops
    /// </summary>
    public class Gradient
    {

        private List<ColorStop> colors;


        /// <summary>
        /// the value of the first color stop
        /// </summary>
        public float MinValue
        {
            get
            {
                return colors[0].Value;
            }
        }

        public float MaxValue
        {
            get
            {
                return colors.Last().Value;
            }
        }

        public Gradient(params ColorStop[] colors)
        {

            this.colors = new List<ColorStop>();

            foreach (ColorStop color in colors)
            {
                AddColorStop(color);
            }
        }


        public void AddColorStop(ColorStop toAdd)
        {
            foreach (ColorStop color in colors)
            {
                if (color.Value == toAdd.Value)
                {
                    throw new InvalidOperationException("May not add ColorStop: " + toAdd.ToString() + ". ColorStops may not have the same value in a gradient.");
                }
            }

            colors.Add(toAdd);
            colors.Sort((x, y) => Math.Sign(x.Value - y.Value));
        }

        public Color getColor(float value)
        {
            if (value <= MinValue)
            {
                return colors.First().Color;
            }

            if (value >= MaxValue)
            {
                return colors.Last().Color;
            }

            // now we have to do actual gradient things

            int i = 0;

            for (; i < colors.Count - 1; i++)
            {
                if (value == colors[i].Value)
                {
                    return colors[i].Color;
                }

                if (value < colors[i + 1].Value)
                {

                    break;
                }
            }

            // i and i+1 represents the index of the colors we need to blend

            float lerpFactor = (value - colors[i].Value) / (colors[i + 1].Value - colors[i].Value);

            return LerpColor(colors[i].Color, colors[i + 1].Color, lerpFactor);


        }




        public static Color LerpColor(Color one, Color two, float amount)
        {

            int red = (int)MathF.Round(MathHelper.Lerp(one.R, two.R, amount));
            int blue = (int)MathF.Round(MathHelper.Lerp(one.B, two.B, amount));
            int green = (int)MathF.Round(MathHelper.Lerp(one.G, two.G, amount));
            int alpha = (int)MathF.Round(MathHelper.Lerp(one.A, two.A, amount));

            return new(red, green, blue, alpha);
        }




        public class ColorStop
        {
            private Color color;
            private float value;

            public Color Color
            {
                get { return color; }
                set { color = value; }
            }

            /// <summary>
            /// The value at which this color is brightest
            /// </summary>
            public float Value
            {
                get { return value; }
            }

            public ColorStop(Color color, float value)
            {
                this.color = color;
                this.value = value;
            }



        }


    }
}
