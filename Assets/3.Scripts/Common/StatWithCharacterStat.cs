public class StatWithCharacterStat : Stat, NeedCharacterStat
{
    /* CharacterStat */
    protected CharacterStat characterStat = CharacterStat.Default;

    public void SetCharacterStat(CharacterStat characterStat)
    {
        this.characterStat = characterStat;
    }
}