public class CombatResult
{
    public bool PlayerWin;
    public CombatRuntime Runtime;

    public CombatResult(bool win, CombatRuntime data)
    {
        PlayerWin = win;
        Runtime = data;
    }
}