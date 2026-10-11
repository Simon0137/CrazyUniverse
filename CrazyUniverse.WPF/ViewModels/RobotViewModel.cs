

using System.ComponentModel;
using System.Runtime.CompilerServices;
using CrazyUniverse.Core.Models;

namespace CrazyUniverse.WPF.ViewModels;

public class RobotViewModel(Robot robot) : INotifyPropertyChanged
{
    public Robot Model { get; } = robot;

    public string Type { get; } = robot switch
    {
        CleanerBot => "Cleaner",
        ExplorerBot => "Explorer",
        GuardBot => "Guard",
        RepairBot => "Repair",
        _ => "Undefined"
    };
    public string Id { get; } = robot.RobotId;

    public double Charge
    {
        get;
        set
        {
            if (value.Equals(field)) return;
            field = value;
            OnPropertyChanged();
        }
    } = robot.Charge;

    public double Durability
    {
        get;
        set
        {
            if (value.Equals(field)) return;
            field = value;
            OnPropertyChanged();
        }
    } = robot.Durability;

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}