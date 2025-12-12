// See https://aka.ms/new-console-template for more information




using NetTrain.IO;

using TrarinLib.TrainerHere;

FileLogger.WriteLine("Hello, World!");
Loader loader = new Loader();
loader.Dryrun();


//PrepareWordDataSet prepareWordDataSet = new PrepareWordDataSet();
//prepareWordDataSet.RunThread();

NetTrainer trainer = new NetTrainer();
//trainer.WordDataSet = prepareWordDataSet;
//trainer.Train();

//BattigolTrainer.RunTrain();
trainer.RunWithoutTrain();

FileLogger.WriteLine("Done!");
