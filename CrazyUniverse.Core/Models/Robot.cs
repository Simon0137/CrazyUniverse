namespace CrazyUniverse.Core.Models;

public abstract class Robot(string robotId, double currentCharge = 75, double currentDurability = 100)
{
    public string RobotId { get; } = robotId;
    public double Charge { get; protected set; } = currentCharge;
    public double Durability { get; internal set; } = currentDurability;

    protected Queue<string> RobotLog = new();

    protected bool TrySpendCharge(double count)
    {
        if (Charge - count < 0)
        {
            return false;
        }

        Charge -= count;
        return true;
    }

    protected virtual void AddLog(string log) => RobotLog.Enqueue(log);
    public string GetLastLog() => RobotLog.Dequeue();

    public abstract void CrazyAction();
}