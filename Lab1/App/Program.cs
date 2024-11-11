using GeneticAlgLib;

class Program
{
    static void Main(string[] args)
    {
        var squareSizes = new List<int> { 1, 1, 2, 2, 3, 1,1,1,1,1,1,1,1,1,1,3,3,3,2,2,2,3,2,3,2,3,3,3,2,1};
        var ga = new GeneticAlgorithm(20, squareSizes);

        while (!Console.KeyAvailable)
        {
            ga.Run(1); // 运行1代
            var best = ga.Population.GetBestIndividual();
            Console.WriteLine($"Generation {ga.Generation}, Best Loss: {best.Loss}");
            Thread.Sleep(20);
        }

        var final = ga.Population.GetBestIndividual();
        Console.WriteLine("Final solution:");
        foreach (var square in final.Squares)
        {
            Console.WriteLine($"Square at ({square.X}, {square.Y}) with side length {square.SideLength}");
        }
    }
}
