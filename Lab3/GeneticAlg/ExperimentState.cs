using GeneticAlgLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlg
{
    public class ExperimentState
    {
        public string ExperimentName { get; set; }      
        public int Generation { get; set; }          
        public List<Individual> Population { get; set; }  
        public Individual BestIndividual { get; set; }    
    }
}
