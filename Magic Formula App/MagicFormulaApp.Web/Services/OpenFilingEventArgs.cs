using MagicFormulaApp.Web.Models;

namespace MagicFormulaApp.Web.Services
{
    public class OpenFilingEventArgs(Report report) : EventArgs
    {
        public Report Report { get; set; } = report;
    }
}
