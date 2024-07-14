namespace MagicFormulaApp.Web.Services;

public class ChangeTextSizeEventArgs(string size) : EventArgs
{
    public string Size { get; set; } = size;
}
