using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgLib
{
    public class Population
    {
        public List<Individual> Individuals { get; set; }

        public Population(int size, List<int> squareSizes)
        {
            Individuals = new List<Individual>();
            for (int i = 0; i < size; i++)
            {
                Individuals.Add(new Individual(squareSizes));
            }
        }

        public void Evolve()
        {
            var newGeneration = new List<Individual>();

            // Selection
            Individuals = Individuals.OrderBy(ind => ind.Loss).ToList();
            newGeneration.AddRange(Individuals.Take(Individuals.Count / 2));

            //Crossover
            Random rand = new Random();
            while (newGeneration.Count < Individuals.Count)
            {
                var parent1 = newGeneration[rand.Next(newGeneration.Count)];
                var parent2 = newGeneration[rand.Next(newGeneration.Count)];
                newGeneration.Add(parent1.Crossover(parent2));
            }

            // Mutation
            foreach (var individual in newGeneration)
            {
                if (rand.NextDouble() < 0.1) // 10% 的突变概率
                {
                    individual.Mutate();
                }
            }
            Individuals.Clear();
            Individuals.AddRange(newGeneration);
        }

        public Individual GetBestIndividual()
        {
            return Individuals.OrderBy(ind => ind.Loss).First();
        }
    }
}
