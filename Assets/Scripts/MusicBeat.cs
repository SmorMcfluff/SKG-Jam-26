using Unity.IO.LowLevel.Unsafe;

public class MusicBeat
{
    public BeatType Instruments;

    public void Play()
    {
        MusicMaker.instance.PlayBeat(Instruments);
    }

    public void Toggle(BeatType instrument)
    {
        Instruments ^= instrument;
    }

    public bool Has(BeatType instrument)
    {
        return (Instruments & instrument) != 0;
    }
}

[System.Flags]
public enum BeatType
{
    None = 0,
    Snare = 1 << 0,
    Piano = 1 << 1,
    Hey = 1 << 2,

    All = Snare | Piano | Hey
}
