namespace MagicFormulaApp.Web.Services;

public class SearchSymbolService
{
    public AsyncEvent<SearchSymbolEventArgs> SymbolChanged { get; set; }

    public async Task OnSymbolChangedAsync(object sender, SearchSymbolEventArgs e)
    {
        if (SymbolChanged != null)
        {
            await SymbolChanged.InvokeAsync(sender, e);
        }
    }
}
