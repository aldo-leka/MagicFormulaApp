using MagicFormulaApp.Web.Models;

namespace MagicFormulaApp.Web.Services;

public class SearchSymbolEventArgs(Ticker ticker) : EventArgs
{
    public Ticker Ticker { get; set; } = ticker;
}
