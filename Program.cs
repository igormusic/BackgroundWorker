using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        BackgroundWorker worker = new BackgroundWorker();
        worker.DoWork += Worker_DoWork;
        worker.ProgressChanged += Worker_ProgressChanged;
        worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        worker.WorkerReportsProgress = true;
        worker.WorkerSupportsCancellation = true;
        worker.RunWorkerAsync();

        while (worker.IsBusy) {
            Thread.Sleep(500);
        }

        Console.WriteLine("Back to Main - exiting");

        Console.ReadKey();
    }

    private static void Worker_DoWork(object sender, DoWorkEventArgs e)
    {
        try
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            Random random = new Random();
            


            for (int i = 1; i <= 20; i++)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                worker.ReportProgress(i);

                int randomNumber = random.Next(1, 11);

                if (randomNumber == 6) {
                    Console.WriteLine("throwing exception");
                    throw new Exception("An error occurred in Worker_DoWork");
                }

                Thread.Sleep(1000);
            }
            // Throw an exception to simulate an error
            
        }
        catch (Exception ex)
        {
            // Set the Error property of the DoWorkEventArgs object
            // to propagate the exception back to the calling code
            e.Result = ex;
        }
    }

    private static void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
        int progress = e.ProgressPercentage;
        Console.WriteLine($"Progress: {progress}");
        if (progress == 10)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            if (worker.IsBusy)
            {
                
                worker.CancelAsync();

                Console.WriteLine("Stopping background worker...");

                //Thread workerThread = worker.GetType().GetField("worker", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(worker) as Thread;
                //workerThread.Abort(); // stop the worker thread
            }
        }
    }

    private static void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        if (e.Result is Exception)
        {
            Exception exception= (Exception)e.Result;

            // Handle the error here
            Console.WriteLine($"An error occurred: {exception.Message}");
        }
    }
}
