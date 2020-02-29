namespace VTility.Logic
{
    public class Dimensions
    {
        public Dimensions(double top, double right, double bottom, double left)
        {
            this.top = top;
            this.right = right;
            this.bottom = bottom;
            this.left = left;
        }

        public Dimensions()
        {
            this.top = 0;
            this.right = 0;
            this.bottom = 0;
            this.left = 0;
        }

        public Dimensions(double topBot, double leftRight)
        {
            this.left = leftRight;
            this.top = topBot;
            this.right = 0;
            this.bottom = 0;
        }

        private double top;
        private double right;
        private double bottom;
        private double left;

        public double Top
        {
            get
            {
                return top;
            }

            set
            {
                top = value;
            }
        }

        public double Right
        {
            get
            {
                return right;
            }

            set
            {
                right = value;
            }
        }

        public double Bottom
        {
            get
            {
                return bottom;
            }

            set
            {
                bottom = value;
            }
        }

        public double Left
        {
            get
            {
                return left;
            }

            set
            {
                left = value;
            }
        }

        internal double Height()
        {
            return (top + bottom);
        }

        internal double Width()
        {
            return (left + right);
        }
    }
}