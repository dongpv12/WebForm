using System;
using WebForm.Common;

public class DataMemoryService : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(LoadMemory, cancellationToken);
        Task.Run(LoadSymbolMemory, cancellationToken);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private async Task LoadMemory()
    {
        while (true)
        {
            try
            {


                DataMemory.LoadNews();
                  
               
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
            }
            await Task.Delay(TimeSpan.FromSeconds(30));
        }
    }

    private async Task LoadSymbolMemory()
    {
        while (true)
        {
            try
            {


                DataMemory.LoadSymbol();


            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
            }
            await Task.Delay(TimeSpan.FromSeconds(60*30));
        }
    }

}
