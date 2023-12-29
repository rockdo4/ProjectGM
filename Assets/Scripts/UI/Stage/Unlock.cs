public class Unlock
{
    public int id;
    public bool unlocked = false;

    public Unlock(int id, bool unlocked = false)
    {
        this.id = id;
        this.unlocked = unlocked;
    }
}
