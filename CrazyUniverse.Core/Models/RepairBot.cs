using CrazyUniverse.Core.Interfaces;

namespace CrazyUniverse.Core.Models;

public class RepairBot(string robotId, double currentCharge, double currentDurability, double repairEff = 0.5) :
    Robot(robotId, currentCharge, currentDurability),
    IChargeable,
    IRepair
{
    public bool TryRepairRobot(Robot robot, double repairPoints)
    {
        var spentCharge = repairPoints * repairEff;
        if (!TrySpendCharge(spentCharge))
        {
            AddLog($"Robot {RobotId} has not enough charge for repairing");
            return false;
        }

        robot.Durability = Math.Clamp(robot.Durability + repairPoints, 0, 100);
        AddLog($"Robot {RobotId} repaired robot {robot.RobotId} {(robot.Durability >= 100 ? "completely" : $"to {repairPoints}")}");
        return true;
    }

    public void Recharge(double count)
    {
        Charge += count;
        if (Charge > 100)
        {
            Charge = 100;
            AddLog($"Robot {RobotId} recharged completely");
        }
        else
            AddLog($"Robot {RobotId} recharged to {count}");
    }

    public override void CrazyAction()
    {
        var random = new Random().Next(0, 2);

        if (Charge > 20 && random == 0)
        {
            Charge -= 20;

            var addDurability = new Random().Next(15, 26);
            Durability += addDurability;
            if (Durability > 100) Durability = 100;

            AddLog($"Robot {RobotId} went mad and sacrificed 20 charge in exchange for {addDurability} durability");
        }
        else if (Durability > 20 && random > 0)
        {
            Durability -= 20;

            var addCharge = new Random().Next(15, 26);
            Charge += addCharge;
            if (Charge > 100) Charge = 100;

            AddLog($"Robot {RobotId} went mad and sacrificed 20 durability in exchange for {addCharge} charge");
        }
        else
        {
            Charge = 1;
            Durability = 1;

            AddLog($"Robot {RobotId} went mad and, as a result, reduced his charge and durability to 1");
        }
    }
}