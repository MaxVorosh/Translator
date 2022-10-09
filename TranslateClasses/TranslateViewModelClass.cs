namespace TranslateClass;

public class TranslateViewModelClass
{
    private TranslateClass _translate;

    public TranslateViewModelClass()
    {
        _translate = new TranslateClass();
    }

    public string TranslateText(string text)
    {
        return _translate.TranslateText(text);
    }
}