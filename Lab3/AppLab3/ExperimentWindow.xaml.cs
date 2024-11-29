using System;
using System.Text;
using System.Windows;
using GeneticAlgLib;

namespace AppLab3
{
    public partial class ExperimentWindow : Window
    {
        private GeneticAlgorithm _ga;

        // 定义回调函数为了加载
        public Action<string, string, string, StringBuilder, Individual> UpdateMainWindowState { get; set; }

        public ExperimentWindow(GeneticAlgorithm geneticAlgorithm)
        {
            InitializeComponent();
            _ga = geneticAlgorithm;
        }

        // 保存实验
        private void SaveExperiment_Click(object sender, RoutedEventArgs e)
        {
            string experimentName = txtExperimentName.Text.Trim();
            if (string.IsNullOrWhiteSpace(experimentName))
            {
                MessageBox.Show("Please enter a valid experiment name.");
                return;
            }

            try
            {
                _ga.SaveExperimentState(experimentName); // 调用保存方法
                _ga.UpdateRunsMetadata(experimentName, $"{experimentName}.json");
                MessageBox.Show($"Experiment '{experimentName}' saved successfully!");
                this.Close(); // 关闭窗口
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving experiment: {ex.Message}");
            }
        }

        // 加载实验
        private void LoadExperiment_Click(object sender, RoutedEventArgs e)
        {
            string experimentName = txtExperimentName.Text.Trim();
            if (string.IsNullOrWhiteSpace(experimentName))
            {
                MessageBox.Show("Please enter a valid experiment name.");
                return;
            }

            try
            {
                _ga.LoadExperimentState(experimentName); // 加载实验状态

                var bestIndividual = _ga.Population.GetBestIndividual();
                // 统计 SideLength 为 1、2 和 3 的数量
                int countSideLength1 = bestIndividual.Squares.Count(s => s.SideLength == 1);
                int countSideLength2 = bestIndividual.Squares.Count(s => s.SideLength == 2);
                int countSideLength3 = bestIndividual.Squares.Count(s => s.SideLength == 3);

                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine($"Loaded Experiment: {experimentName}");
                stringBuilder.AppendLine($"Generation: {_ga.Generation}");
                stringBuilder.AppendLine($"Best Loss: {bestIndividual.Loss}");
                foreach (var square in bestIndividual.Squares)
                {
                    stringBuilder.AppendLine($"Square at ({square.X}, {square.Y})");
                }

                // 更新主窗口
                UpdateMainWindowState?.Invoke(
                    countSideLength1.ToString(),
                    countSideLength2.ToString(),
                    countSideLength3.ToString(),
                    stringBuilder,
                    bestIndividual);

                MessageBox.Show($"Experiment '{experimentName}' loaded successfully!");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading experiment: {ex.Message}");
            }
        }
    }
}
