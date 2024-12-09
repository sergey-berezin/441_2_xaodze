using GeneticAlg;
using Newtonsoft.Json;
using System.Security;
using System.Text;
using System.Xml;

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

        public void SaveExperimentState(string experimentName) 
        {
            var state = new ExperimentState
            {
                   ExperimentName = experimentName,
                   Generation = this.Generation,
                   Population = this.Population.Individuals,
                   BestIndividual = this.Population.GetBestIndividual()
            };
            string directoryPath = Path.Combine("C:\\Users\\26847\\EvolutionSquares", "SavedExperiments");
            Directory.CreateDirectory(directoryPath);

            string fileName = $"{experimentName}.json";
            string filePath = Path.Combine(directoryPath, fileName);

            string jsonData = JsonConvert.SerializeObject(state, Newtonsoft.Json.Formatting.Indented);

            File.WriteAllText(filePath, jsonData);
        }

        public void LoadExperimentState(string experimentName)
        {
            string directoryPath = Path.Combine("C:\\Users\\26847\\EvolutionSquares", "SavedExperiments");
            string fileName = $"{experimentName}.json";
            string filePath = Path.Combine(directoryPath, fileName);

            try
            {
                if (!File.Exists(filePath))
                    throw new FileNotFoundException($"Experiment file '{fileName}' not found.");

                string jsonData = File.ReadAllText(filePath);
                var state = JsonConvert.DeserializeObject<ExperimentState>(jsonData);

                // 恢复算法状态
                this.Generation = state.Generation;
                this.Population.Individuals = state.Population;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading experiment: {ex.Message}");
            }
        }

        public void UpdateRunsMetadata(string experimentName, string filePath)
        {
            string directoryPath = Path.Combine("C:\\Users\\26847\\EvolutionSquares", "SavedExperiments");
            string metadataname = $"runs.json";
            string metadataPath = Path.Combine(directoryPath, metadataname);

            List<RunMetadata> metadata = new List<RunMetadata>();
            if (File.Exists(metadataPath))
            {
                string jsonData = File.ReadAllText(metadataPath);
                metadata = JsonConvert.DeserializeObject<List<RunMetadata>>(jsonData) ?? new List<RunMetadata>();
            }

            // 如果实验已存在，则更新
            var existing = metadata.FirstOrDefault(m => m.ExperimentName == experimentName);
            if (existing != null)
            {
                existing.FilePath = filePath;
            }
            else
            {
                metadata.Add(new RunMetadata { ExperimentName = experimentName, FilePath = filePath });
            }

            // 保存到文件
            File.WriteAllText(metadataPath, JsonConvert.SerializeObject(metadata, Newtonsoft.Json.Formatting.Indented));
        }

        
    }
}
