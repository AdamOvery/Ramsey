public class Clique
{
    private String id;
    public delegate void CliqueChangedHandler(Clique sender, CliqueValue value, CliqueValue previousValue);
    public event CliqueChangedHandler CliqueChanged = delegate { };

    private CliqueValue _value = CliqueValue.AllOff;
    public CliqueValue value
    {
        get
        {
            return _value;
        }
        protected set
        {
            var previousValue = _value;
            if (value == previousValue) return;
            _value = value;
            this.CliqueChanged.Invoke(this, value, previousValue);
        }
    }
    private int onInnerCliquesCount;
    private int offInnerCliquesCount;

    readonly int size;
    public Clique(int size, string id, Clique[] innerCliques)
    {
        this.size = size;
        offInnerCliquesCount = size;
        foreach (Clique innerClique in innerCliques)
        {
            innerClique.CliqueChanged += onInnerCliqueChanged;
        }
        this.id = id;
    }

    protected void onInnerCliqueChanged(Clique innerClique, CliqueValue value, CliqueValue previousValue)
    {
        if (previousValue == CliqueValue.AllOn) onInnerCliquesCount -= 1;
        else if (previousValue == CliqueValue.AllOff) offInnerCliquesCount -= 1;

        if (value == CliqueValue.AllOn) onInnerCliquesCount += 1;
        else if (value == CliqueValue.AllOff) offInnerCliquesCount += 1;

        this.value = (onInnerCliquesCount == size) ? CliqueValue.AllOn
                : (offInnerCliquesCount == size) ? CliqueValue.AllOff
                : CliqueValue.InBetween;
    }

    public enum CliqueValue
    {
        AllOn,
        AllOff,
        InBetween
    }

    public override string ToString()
    {
        return id;
    }
}
