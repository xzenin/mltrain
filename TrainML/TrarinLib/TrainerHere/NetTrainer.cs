using Microsoft.ML;
using Microsoft.ML.Data;

using NetTrain;
using NetTrain.IO;

using System;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace TrarinLib.TrainerHere
{
    public class NetTrainer
    {
        string prefix = "BanglaPredictor";
        Loader loader = new Loader();
        public PrepareWordDataSet WordDataSet = new PrepareWordDataSet();

        public NetTrainer()
        {
            MainSystem();
        }
        public void Train()
        { 
            var mlContext = new MLContext();
            FileLogger.WriteLine($"Preparing Data started at: {DateTime.Now}");

            var trainingInputData = WordDataSet.GetMegredWords().Select(x => new InputData() { Text = x.Key, Label = x.Value }).ToArray();
            var dataview = mlContext.Data.LoadFromEnumerable(trainingInputData);
            FileLogger.WriteLine($"Dataview created at: {DateTime.Now}");
            var pipeline = mlContext.Transforms.Text.FeaturizeText("Features", nameof(InputData.Text))
                .Append(mlContext.Transforms.Conversion.MapValueToKey("Label", nameof(InputData.Label)))
                .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy())
                .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            FileLogger.WriteLine($"Training started at: {DateTime.Now}");

            var model = pipeline.Fit(dataview);
            FileLogger.WriteLine($"Training completed at: {DateTime.Now}");

            // === SAVE MODEL TO DISK ===
            string modelPath = Path.Combine(loader.BaseDirectory, $"{prefix}.zip");
            mlContext.Model.Save(model, dataview.Schema, modelPath);
            FileLogger.WriteLine($"Model saved to: {modelPath}");

            using (var fileStream = File.Create(loader.GetFile($"{prefix}.idv")))
            {
                mlContext.Data.SaveAsBinary(dataview, fileStream);
            }

           

            // === LOAD MODEL FROM DISK ===
            DataViewSchema schema;
            ITransformer loadedModel = mlContext.Model.Load(modelPath, out schema); 

            var testSample = new InputData { Text = "অকীর্তিকল" };        

            var predictor = mlContext.Model.CreatePredictionEngine<InputData, PredictionScore>(model);
            var result = predictor.Predict(testSample);
            // Get top 10 predictions
            var labels = dataview.GetColumn<string>("Label").Distinct().ToList();
            var top10 = result.Scores
                .Select((score, index) => new { Word = labels[index], Score = score })
                .OrderByDescending(x => x.Score)
                .Take(10);

            FileLogger.WriteLine($"Input: {testSample.Text}");
            FileLogger.WriteLine("Top predictions:");
            foreach (var prediction in top10)
            {
                FileLogger.WriteLine($"{prediction.Word} (score: {prediction.Score:F4})");
            }

        }
        public  void RunWithoutTrain()
        {
            var mlContext = new MLContext();
            IDataView reloadedData = mlContext.Data.LoadFromBinary(loader.GetFile($"{prefix}.idv"));
            
            string modelPath = Path.Combine(loader.BaseDirectory, $"{prefix}.zip");
            

            // === LOAD MODEL FROM DISK ===
            DataViewSchema schema;
            ITransformer loadedModel = mlContext.Model.Load(modelPath, out schema);
            // 5. Test prediction
            var testSample = new InputData { Text = "অকীর্তিকল" };
         


            var predictor2 = mlContext.Model.CreatePredictionEngine<InputData, PredictionScore>(loadedModel);
            var result2 = predictor2.Predict(testSample);
            // Get top 10 predictions
            var labels = reloadedData.GetColumn<string>("Label").Distinct().ToList();
            var top10 = result2.Scores
                .Select((score, index) => new { Word = labels[index], Score = score })
                .OrderByDescending(x => x.Score)
                .Take(10);

            FileLogger.WriteLine($"Input: {testSample.Text}");
            FileLogger.WriteLine("Top predictions:");
            foreach (var prediction in top10)
            {
                FileLogger.WriteLine($"{prediction.Word} (score: {prediction.Score:F4})");
            }

        }
        public  void PredictWords(string word, int maxcount)
        {
            var mlContext = new MLContext();
            IDataView reloadedData = mlContext.Data.LoadFromBinary(loader.GetFile($"{prefix}.idv"));

            string modelPath = Path.Combine(loader.BaseDirectory, $"{prefix}.zip");

            // === LOAD MODEL FROM DISK ===
            DataViewSchema schema;
            ITransformer loadedModel = mlContext.Model.Load(modelPath, out schema);
            // 5. Test prediction
            var testSample = new InputData { Text = word };



            var predictor2 = mlContext.Model.CreatePredictionEngine<InputData, PredictionScore>(loadedModel);
            var result2 = predictor2.Predict(testSample);
            // Get top 10 predictions
            var labels = reloadedData.GetColumn<string>("Label").Distinct().ToList();
            var top10 = result2.Scores
                .Select((score, index) => new { Word = labels[index], Score = score })
                .OrderByDescending(x => x.Score)
                .Take(maxcount);

            FileLogger.WriteLine($"Input: {testSample.Text}");
            FileLogger.WriteLine("Top predictions:");
            foreach (var prediction in top10)
            {
                FileLogger.WriteLine($"{prediction.Word} (score: {prediction.Score:F4})");
            }

        }

         void MainSystem()
        {
            var ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            FileLogger.WriteLine($"Available RAM: {ramCounter.NextValue()} MB");

            ulong totalRam = GetTotalMemoryInGb();
            FileLogger.WriteLine($"Total RAM: {totalRam} GB");
        }

        ulong GetTotalMemoryInGb()
        {
            var searcher = new ManagementObjectSearcher("SELECT Capacity FROM Win32_PhysicalMemory");
            ulong capacity = 0;
            foreach (var item in searcher.Get())
            {
                capacity += Convert.ToUInt64(item.Properties["Capacity"].Value);
            }
            return capacity / (1024 * 1024 * 1024);
        }
    }
}
