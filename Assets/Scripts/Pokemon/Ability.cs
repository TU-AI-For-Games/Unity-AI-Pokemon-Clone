public abstract class Ability
{
    protected string m_name;
    protected string m_description;

    protected abstract void Outcome(PocketMonster mon);
}