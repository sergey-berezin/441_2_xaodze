using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgLib
{
    public class Individual
    {
        public List<Square> Squares { get; }
        public double Loss { get; private set; }
        private int maxRange;

        public Individual(List<int> sizes)
        {
            Squares = sizes.Select(size => new Square(size)).ToList();

            int totalArea = sizes.Sum(size => size * size);
            maxRange = (int)Math.Sqrt(totalArea * 100);

            RandomizePlacement();
            CalculateLoss();
        }

        public Individual(List<Square> squares)
        {
            Squares = squares;

            int totalArea = squares.Sum(square => square.SideLength * square.SideLength);
            maxRange = (int)Math.Sqrt(totalArea * 100);

            AdjustForNoOverlap();  // Ensure no overlap at initialization
            CalculateLoss();
        }

        private void RandomizePlacement()
        {
            Random rand = new Random();
            foreach (var square in Squares)
            {
                bool overlaps;
                do
                {
                    square.X = rand.Next(0, maxRange - square.SideLength);
                    square.Y = rand.Next(0, maxRange - square.SideLength);
                    overlaps = Squares.Any(other => other != square && square.IsOverlaps(other));
                } while (overlaps);
            }
        }

        private void AdjustForNoOverlap()
        {
            Random rand = new Random();
            foreach (var square in Squares)
            {
                bool overlaps;
                do
                {
                    overlaps = Squares.Any(other => other != square && square.IsOverlaps(other));
                    if (overlaps)
                    {
                        square.X = rand.Next(0, maxRange - square.SideLength);
                        square.Y = rand.Next(0, maxRange - square.SideLength);
                    }
                } while (overlaps);
            }
        }

        public void CalculateLoss()
        {
            int maxX = Squares.Max(s => s.X + s.SideLength);
            int minX = Squares.Min(s => s.X);
            int maxY = Squares.Max(s => s.Y + s.SideLength);
            int minY = Squares.Min(s => s.Y);
            Loss = (maxX - minX) * (maxY - minY);
        }

        public Individual Crossover(Individual other)
        {
            Random rand = new Random();
            var newSquares = this.Squares.Select(s => new Square(s.SideLength)).ToList();
            for (int i = 0; i < newSquares.Count; i++)
            {
                if (rand.NextDouble() < 0.5)
                {
                    newSquares[i].X = other.Squares[i].X;
                    newSquares[i].Y = other.Squares[i].Y;
                }
                else 
                {
                    newSquares[i].X = this.Squares[i].X;
                    newSquares[i].Y = this.Squares[i].Y;
                } 
            }
            AdjustForNoOverlap(newSquares);
            var child = new Individual(newSquares);
            return child;
        }

        private void AdjustForNoOverlap(List<Square> squares)
        {
            Random rand = new Random();
            foreach (var square in squares)
            {
                bool overlaps;
                do
                {
                    overlaps = squares.Any(other => other != square && square.IsOverlaps(other));
                    if (overlaps)
                    {
                        square.X = rand.Next(0, maxRange - square.SideLength);
                        square.Y = rand.Next(0, maxRange - square.SideLength);
                    }
                } while (overlaps);
            }
        }

        public void Mutate()
        {
            Random rand = new Random();
            var square = Squares[rand.Next(Squares.Count)];
            int oldX = square.X;
            int oldY = square.Y;

            square.X += rand.Next(-1, 2);
            square.Y += rand.Next(-1, 2);

            if (square.X < 0) square.X = 0;
            if (square.Y < 0) square.Y = 0;
            if (square.X > maxRange - square.SideLength) square.X = maxRange - square.SideLength;
            if (square.Y > maxRange - square.SideLength) square.Y = maxRange - square.SideLength;

            if (Squares.Any(other => other != square && square.IsOverlaps(other)))
            {
                square.X = oldX;
                square.Y = oldY;
            }

            CalculateLoss();
        }
    }
}
