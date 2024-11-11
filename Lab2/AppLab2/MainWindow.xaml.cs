using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Threading.Tasks;
using System.Linq;
using GeneticAlgLib;
using System.Security.Cryptography;
using System.Text;

namespace AppLab2
{
    public partial class MainWindow : Window
    {
        private GeneticAlgorithm ga;
        private bool isRunning = false;
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            var squareSizes = new List<int>();
            for (int i = 0; i < int.Parse(txtSquare1.Text); i++)
                squareSizes.Add(1);
            for (int i = 0; i < int.Parse(txtSquare2.Text); i++)
                squareSizes.Add(2);
            for (int i = 0; i < int.Parse(txtSquare3.Text); i++)
                squareSizes.Add(3);
            ga = new GeneticAlgorithm(20, squareSizes);
            isRunning = true;

            Task runTask = Task.Run(() => RunAlgorithm());
            await runTask;
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            isRunning = false;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            txtSquare1.Text = "";
            txtSquare2.Text = "";
            txtSquare3.Text = "";
            canvasRectangles.Children.Clear();
            isRunning = false;
        }

        private void RunAlgorithm()
        {
            int generationLimit = 10000; // 设定遗传算法的代数

            for (int i = 0; i < generationLimit && isRunning; i++)
            {
                ga.Run(1);

                this.Dispatcher.Invoke(() => {
                    DrawSquares(ga.Population.GetBestIndividual());
                    Individual bestIndividual = ga.Population.GetBestIndividual();
                    outputText.Text = string.Empty;
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine($"Generation: {ga.Generation}, Best Loss" +
                        $"" +
                        $": {bestIndividual.Loss}");

                    foreach (var square in bestIndividual.Squares)
                    {
                        stringBuilder.AppendLine($"Square at ({square.X}, {square.Y})");
                    }

                    outputText.Text += Environment.NewLine + stringBuilder.ToString();

                });
                System.Threading.Thread.Sleep(5); // 休眠一小段时间，以便可视化看到变化
            }
        }

        private void DrawSquares(Individual bestIndividual)
        {
            canvasRectangles.Children.Clear();
            double width = canvasRectangles.ActualWidth;
            double height = canvasRectangles.ActualHeight;
            double step = 20;  // 定义网格点间距

            for (double x = 0; x < width; x += step)
            {
                for (double y = 0; y < height; y += step)
                {
                    Ellipse point = new Ellipse
                    {
                        Stroke = Brushes.Black,
                        Fill = Brushes.Black,
                        Width = 2,
                        Height = 2
                    };
                    Canvas.SetLeft(point, x);
                    Canvas.SetTop(point, y);
                    canvasRectangles.Children.Add(point);
                }
            }
            foreach (var square in bestIndividual.Squares)
            {
                Rectangle rectangle = new Rectangle
                {
                    Width = square.SideLength * 20, // 放大以便于观察
                    Height = square.SideLength * 20,
                    Stroke = Brushes.Black
                };

                switch (square.SideLength)
                {
                    case 1:
                        rectangle.Fill = Brushes.AliceBlue;
                        break;
                    case 2:
                        rectangle.Fill = Brushes.LightSalmon; 
                        break;
                    case 3:
                        rectangle.Fill = Brushes.LightPink; 
                        break;
                }
                Canvas.SetLeft(rectangle, square.X * 20);
                Canvas.SetBottom(rectangle, 20+square.Y * 20);
                canvasRectangles.Children.Add(rectangle);
            }
        }
    }
}