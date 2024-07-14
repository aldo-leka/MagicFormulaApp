namespace MagicFormulaApp.Web.Services;

public class OpenFilingService
{
    public AsyncEvent<OpenFilingEventArgs> FilingSelected { get; set; }

    public async Task OnFilingSelectedAsync(object sender, OpenFilingEventArgs e)
    {
        if (FilingSelected != null)
        {
            await FilingSelected.InvokeAsync(sender, e);
        }
    }
}
