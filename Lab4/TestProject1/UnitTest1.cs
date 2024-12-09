using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using WebApp.Models;
using FluentAssertions;
using System.Text.Json;

namespace WebApp.Tests
{
    public class ExperimentControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ExperimentControllerTests(WebApplicationFactory<Program> factory)
        {
            // ???????????
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateExperiment_ShouldReturnExperimentId()
        {
            var experimentParams = new ExperimentParams
            {
                ExperimentName = "TestExperiment",
                PopulationSize = 1000,
                SquareSizes = new List<int> { 1, 1, 2, 3 }
            };

            var response = await _client.PutAsJsonAsync("/experiments", experimentParams);

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var json = JsonSerializer.Deserialize<JsonElement>(responseString);
            var experimentId = json.GetProperty("experimentId").GetString();

            Assert.Equal("TestExperiment", experimentId);
        }

        [Fact]
        public async Task RunStep_ShouldUpdateGenerationAndReturnBestIndividual()
        {
            // Arrange: ????
            var experimentParams = new ExperimentParams
            {
                ExperimentName = "RunStepTest",
                PopulationSize = 300,
                SquareSizes = new List<int> { 1, 2, 3 }
            };

            var createResponse = await _client.PutAsJsonAsync("/experiments", experimentParams);
            createResponse.EnsureSuccessStatusCode();

            var runResponse = await _client.PostAsync("/experiments/RunStepTest", null);
            runResponse.EnsureSuccessStatusCode();

            var runData = await runResponse.Content.ReadFromJsonAsync<JsonElement>();

            var generation = runData.GetProperty("generation").GetInt32();
            Assert.True(generation > 0, "Generation should be greater than 0");

            var bestIndividual = runData.GetProperty("bestIndividual");
            Assert.NotNull(bestIndividual);

            var loss = bestIndividual.GetProperty("loss").GetDouble();
            Assert.True(loss >= 0, "Loss should be a non-negative value");

            var squares = bestIndividual.GetProperty("squares").EnumerateArray();
            Assert.True(squares.Any(), "Squares array should not be empty");

            foreach (var square in squares)
            {
                var sideLength = square.GetProperty("sideLength").GetInt32();
                var x = square.GetProperty("x").GetInt32();
                var y = square.GetProperty("y").GetInt32();

                Assert.True(sideLength > 0, "Square side length should be positive");
                Assert.True(x >= 0 && y >= 0, "Square coordinates should be non-negative");
            }
        }


        [Fact]
        public async Task DeleteExperiment_ShouldRemoveExperimentFromList()
        {
            // ????
            var experimentParams = new ExperimentParams
            {
                ExperimentName = "DeleteTest",
                PopulationSize = 300,
                SquareSizes = new List<int> { 1, 2, 2, 3 }
            };
            var createResponse = await _client.PutAsJsonAsync("/experiments", experimentParams);
            createResponse.EnsureSuccessStatusCode();

            // ????
            var deleteResponse = await _client.DeleteAsync("/experiments/DeleteTest");
            deleteResponse.EnsureSuccessStatusCode();
            var deleteResult = await deleteResponse.Content.ReadAsStringAsync();
            deleteResult.Should().Contain("Experiment deleted");

            // ????????
            var getResponse = await _client.GetAsync("/experiments");
            getResponse.EnsureSuccessStatusCode();

            var experimentList = await getResponse.Content.ReadFromJsonAsync<List<string>>();
            experimentList.Should().NotContain("DeleteTest");
        }
    }
}
