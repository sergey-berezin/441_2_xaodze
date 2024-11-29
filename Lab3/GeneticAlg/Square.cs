namespace GeneticAlgLib
{
    public class Square
    {
        public int SideLength { get; }
        public int X { get; set; }  // Левый нижний угол
        public int Y { get; set; }

        public Square(int sideLength, int x = 0, int y = 0)
        {
            SideLength = sideLength;
            X = x;
            Y = y;
        }

        public Boolean IsOverlaps(Square other)
        {
            return X < other.X + other.SideLength && X + SideLength > other.X && //По оси X они перекрываются
                   Y < other.Y + other.SideLength && Y + SideLength > other.Y; // По оси Y они перекрываются
        }
    }
}