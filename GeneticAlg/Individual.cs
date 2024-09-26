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
        public double Fitness { get; private set; }

        public Individual(List<int> sizes)
        {
            Squares = sizes.Select(size => new Square(size)).ToList();
            RandomizePlacement();
            CalculateFitness();
        }

        public Individual(List<Square> squares)
        {
            Squares = squares;
            AdjustForNoOverlap();  // Ensure no overlap at initialization
            CalculateFitness();
        }

        private void RandomizePlacement()
        {
            Random rand = new Random();
            foreach (var square in Squares)
            {
                bool overlaps;
                do
                {
                    square.X = rand.Next(0, 100 - square.SideLength);
                    square.Y = rand.Next(0, 100 - square.SideLength);
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
                        square.X = rand.Next(0, 100 - square.SideLength);
                        square.Y = rand.Next(0, 100 - square.SideLength);
                    }
                } while (overlaps);
            }
        }

        public void CalculateFitness()
        {
            int maxX = Squares.Max(s => s.X + s.SideLength);
            int minX = Squares.Min(s => s.X);
            int maxY = Squares.Max(s => s.Y + s.SideLength);
            int minY = Squares.Min(s => s.Y);
            Fitness = (maxX - minX) * (maxY - minY);
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
                        square.X = rand.Next(0, 100 - square.SideLength);
                        square.Y = rand.Next(0, 100 - square.SideLength);
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
            if (square.X > 100 - square.SideLength) square.X = 100 - square.SideLength;
            if (square.Y > 100 - square.SideLength) square.Y = 100 - square.SideLength;

            if (Squares.Any(other => other != square && square.IsOverlaps(other)))
            {
                square.X = oldX;
                square.Y = oldY;
            }

            CalculateFitness();
        }
    }
}
