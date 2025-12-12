using Microsoft.ML.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrarinLib.TrainerHere
{
    public class InputData
    {
        public string Text { get; set; }
        public string Label { get; set; } // true = positive, false = negative
    }

    public class Prediction
    {
        [ColumnName("PredictedLabel")]
        public string PredictedLabel { get; set; }
    }
    public class PredictionScore
    {
        [ColumnName("Score")]
        public float[] Scores { get; set; }
    }
}
