namespace GeneticAlgLib
{
    public class GeneticAlgorithm
    {
        public Population Population { get; }
        public int Generation { get; private set; }

        public GeneticAlgorithm(int populationSize, List<int> squareSizes)
        {
            Population = new Population(populationSize, squareSizes);
            Generation = 0;
        }

        public void Run(int generations)
        {
            for (int i = 0; i < generations; i++)
            {
                Population.Evolve();
                Generation++;
            }
        }
    }
}
