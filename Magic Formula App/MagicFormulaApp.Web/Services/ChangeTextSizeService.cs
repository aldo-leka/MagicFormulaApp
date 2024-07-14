namespace MagicFormulaApp.Web.Services;

public class ChangeTextSizeService
{
    public EventHandler TextSizeChanged;

    public void OnTextSizeChanged(object sender, ChangeTextSizeEventArgs e)
    {
        if (TextSizeChanged is not null)
        {
            TextSizeChanged(sender, e);
        }
    }
}
