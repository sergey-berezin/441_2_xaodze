using Microsoft.AspNetCore.Mvc;
using GeneticAlgLib;
using WebApp.Models;
using System.Collections.Concurrent;
using System.Linq;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        private static ConcurrentDictionary<string, GeneticAlgorithm> Experiments = new ConcurrentDictionary<string, GeneticAlgorithm>();
        private static ConcurrentDictionary<string, ExperimentState> ExperimentStates = new ConcurrentDictionary<string, ExperimentState>();

        // PUT /experiments：创建新实验
        [HttpPut("experiments")]
        public IActionResult CreateExperiment([FromBody] ExperimentParams parameters)
        {
            if (Experiments.ContainsKey(parameters.ExperimentName))
            {
                return Conflict("Experiment with the same name already exists.");
            }

            var ga = new GeneticAlgorithm(parameters.PopulationSize, parameters.SquareSizes);
            var best = ga.Population.GetBestIndividual();

            var state = new ExperimentState
            {
                ExperimentName = parameters.ExperimentName,
                Generation = 0,
                Population = ga.Population.Individuals,
                BestIndividual = best
            };

            Experiments[parameters.ExperimentName] = ga;
            ExperimentStates[parameters.ExperimentName] = state;

            return Ok(new { experimentId = parameters.ExperimentName });
        }

        [HttpPost("experiments/{id}")]
        public IActionResult RunStep(string id)
        {
            if (!Experiments.TryGetValue(id, out var ga))
            {
                return NotFound("Experiment not found");
            }

            ga.Run(100);
            var best = ga.Population.GetBestIndividual();

            if (ExperimentStates.TryGetValue(id, out var state))
            {
                state.Generation++;
                state.Population = ga.Population.Individuals;
                state.BestIndividual = best;
            }

            return Ok(new
            {
                Generation = ExperimentStates[id].Generation,
                BestIndividual = new
                {
                    Squares = ExperimentStates[id].BestIndividual.Squares.Select(s => new { s.SideLength, s.X, s.Y }),
                    Loss = best.Loss
                }
            });
        }

        [HttpDelete("experiments/{id}")]
        public IActionResult DeleteExperiment(string id)
        {
            var removed1 = Experiments.Remove(id, out _);
            var removed2 = ExperimentStates.Remove(id, out _);

            if (removed1 && removed2)
            {
                return Ok("Experiment deleted");
            }
            return NotFound("Experiment not found");
        }


        [HttpGet("experiments")]
        public IActionResult GetExperiments()
        {
            var experimentIds = Experiments.Keys.ToList();
            return Ok(experimentIds);
        }

    }
}