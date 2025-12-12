using NetTrain;
using NetTrain.IO;

namespace WordPredictor
{
    public partial class FormPredictor : Form
    {
        public FormPredictor()
        {
            InitializeComponent();
            this.Disposed += FormPredictor_Disposed;
            FileLogger.WriteThere = (x) =>
            {
                textBoxOutput.Text += Environment.NewLine + x;
            };
        }

        private void FormPredictor_Disposed(object? sender, EventArgs e)
        {
            FileLogger.WriteThere = null;
        }

        private void buttonOptions_Click(object sender, EventArgs e)
        {
            string text = textBoxInput.Text;

          
            BattigolTrainer.PredictWords(text,5);

            FileLogger.WriteLine("Done!");

        }
    }
}
