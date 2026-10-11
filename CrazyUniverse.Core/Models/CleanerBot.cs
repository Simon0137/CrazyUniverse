using CrazyUniverse.Core.Interfaces;

namespace CrazyUniverse.Core.Models;

public class CleanerBot(string robotId, double currentCharge, double currentDurability) :
    Robot(robotId, currentCharge, currentDurability),
    IChargeable
{
    public void Recharge(double count)
    {
        var addCharge = count / 2;
        Charge += addCharge;

        if (Charge > 100)
        {
            Charge = 100;
            AddLog($"Robot {RobotId} recharged completely");
        }
        else
            AddLog($"Because of the cheap battery, robot {RobotId} only charged to {addCharge}");
    }

    public override void CrazyAction()
    {
        var deltaCharge = new Random().Next(25, 51);

        if (Charge > deltaCharge)
        {
            Charge -= deltaCharge;
            AddLog($"Robot {RobotId} overheats, and for {deltaCharge} charge, it cleans the entire room almost instantly");
        }
        else
        {
            AddLog($"Robot {RobotId} wanted to go crazy, but its battery wasn't charged enough for that");
        }
    }
}