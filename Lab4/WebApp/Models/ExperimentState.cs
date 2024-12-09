using GeneticAlgLib;
using System.Collections.Generic;

namespace WebApp.Models
{
    public class ExperimentState
    {
        public string ExperimentName { get; set; }
        public int Generation { get; set; }
        public List<Individual> Population { get; set; }
        public Individual BestIndividual { get; set; }
    }
}
