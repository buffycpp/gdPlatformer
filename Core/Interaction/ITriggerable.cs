public interface ITriggerable
{
    public bool CanTrigger();
    public void Trigger(string actionName = "default");
}