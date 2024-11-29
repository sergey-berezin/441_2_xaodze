using System.Collections.Concurrent;

namespace GeneticAlgLib
{
    public class Population
    {
        public List<Individual> Individuals { get; set; }

        public void Initialize(List<Individual> individuals)
        {
            Individuals = individuals;

        }
        public Population(int size, List<int> squareSizes)
        {
            Individuals = new List<Individual>();
            var locker = new object(); // 用于线程安全的添加

            Parallel.For(0, size, i =>
            {
                var individual = new Individual(squareSizes);
                lock (locker)
                {
                    Individuals.Add(individual);
                }
            });
        }

        private static ThreadLocal<Random> threadRand = new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));

        public void Evolve()
        {
            // Selection
            Individuals = Individuals.OrderBy(ind => ind.Loss).ToList();
            var newGeneration = new ConcurrentBag<Individual>(Individuals.Take(Individuals.Count / 2));

            // Crossover
            Parallel.For(newGeneration.Count, Individuals.Count, i =>
            {
                var parent1 = newGeneration.ElementAt(threadRand.Value.Next(newGeneration.Count));
                var parent2 = newGeneration.ElementAt(threadRand.Value.Next(newGeneration.Count));
                var offspring = parent1.Crossover(parent2);
                newGeneration.Add(offspring);
            });

            // Mutation
            Parallel.ForEach(newGeneration, individual =>
            {
                if (threadRand.Value.NextDouble() < 0.1) // 10% 突变概率
                {
                    individual.Mutate();
                }
            });

            Individuals = newGeneration.ToList();
        }

        public Individual GetBestIndividual()
        {
            return Individuals.OrderBy(ind => ind.Loss).First();
        }
    }
}
