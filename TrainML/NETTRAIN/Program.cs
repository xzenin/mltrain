// See https://aka.ms/new-console-template for more information


 

using NetTrain;
using NetTrain.IO;

FileLogger.WriteLine("Hello, World!");
Loader loader = new Loader();
loader.Dryrun();


//PrepareWordDataSet prepareWordDataSet = new PrepareWordDataSet();
//prepareWordDataSet.RunThread();

BattigolTrainer trainer = new BattigolTrainer();
//trainer.WordDataSet = prepareWordDataSet;
//trainer.Train();

//BattigolTrainer.RunTrain();
trainer.RunWithoutTrain();

FileLogger.WriteLine("Done!");
