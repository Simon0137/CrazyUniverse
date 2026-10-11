using System.Collections.ObjectModel;
using System.Windows;
using CrazyUniverse.Core;
using CrazyUniverse.Core.Interfaces;
using CrazyUniverse.Core.Models;
using CrazyUniverse.WPF.ViewModels;

namespace CrazyUniverse.WPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public ObservableCollection<RobotViewModel> RobotVMs { get; set; } = [];
    public ObservableCollection<string> Logs { get; set; } = [];

    public RobotViewModel? SelectedVm { get; set; }
    public RobotViewModel? SelectedVm2 { get; set; }

    private bool _isScan = false;
    private bool _isRepair = false;

    public MainWindow()
    {
        InitializeComponent();

        RobotVMs.Add(new RobotViewModel(RobotFactory.AddGuardBot("G-001")));
        RobotVMs.Add(new RobotViewModel(RobotFactory.AddExplorerBot("E-001")));
        RobotVMs.Add(new RobotViewModel(RobotFactory.AddCleanerBot("C-001")));
        RobotVMs.Add(new RobotViewModel(RobotFactory.AddRepairBot("R-001")));

        RobotList.ItemsSource = RobotVMs;
        LogsList.ItemsSource = Logs;
        RobotTypeCombo.ItemsSource = new[]
        {
            new RobotTypeViewModel
            {
                Name = "Cleaner",
                RobotType = typeof(CleanerBot)
            },
            new RobotTypeViewModel
            {
                Name = "Explorer",
                RobotType = typeof(ExplorerBot)
            },
            new RobotTypeViewModel
            {
                Name = "Guard",
                RobotType = typeof(GuardBot)
            },
            new RobotTypeViewModel
            {
                Name = "Repair",
                RobotType = typeof(RepairBot)
            }
        };

        WpfUtils.SetDefault(RobotIdBox, "from 3 to 12 symbols");

        WpfUtils.InitErrorLabel(RobotIdErrorLabel, "Incorrect length");
        WpfUtils.InitErrorLabel(RobotTypeErrorLabel, "Robot type is not selected");
    }

    private void RobotList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (_isScan || _isRepair)
        {
            SelectedVm2 = RobotList.SelectedItem as RobotViewModel;
            if (SelectedVm2 == null) return;

            switch (SelectedVm?.Model)
            {
                case IScan scanRobot:
                    try
                    {
                        var (robotId, charge, durability) = scanRobot.ScanRobot(SelectedVm2.Model);
                        SelectedVm.Charge = SelectedVm.Model.Charge;
                        Logs.Add(SelectedVm.Model.GetLastLog());
                        Logs.Add($"Scanning results - ID: {robotId} Charge: {charge:F2}% Durability: {durability:F2}%");
                    }
                    catch
                    {
                        Logs.Add(SelectedVm.Model.GetLastLog());
                    }
                    break;
                case IRepair repairRobot:
                    if (!repairRobot.TryRepairRobot(SelectedVm2.Model, 15))
                        Logs.Add(SelectedVm.Model.GetLastLog());
                    else
                    {
                        SelectedVm2.Durability = SelectedVm2.Model.Durability;
                        SelectedVm.Charge = SelectedVm.Model.Charge;
                        Logs.Add(SelectedVm.Model.GetLastLog());
                    }
                    break;
            }
        }
        else
        {
            SelectedVm = RobotList.SelectedItem as RobotViewModel;

            if (SelectedVm == null)
            {
                ChargeButton.IsEnabled = false;
                ScanButton.IsEnabled = false;
                RepairButton.IsEnabled = false;
                DeleteRobotButton.IsEnabled = false;
                CrazyActionButton.IsEnabled = false;
            }
            else
                CheckButtons(SelectedVm.Model);
        }
    }

    private void CheckButtons(Robot model)
    {
        var isCharge = false;
        var isScan = false;
        var isRepair = false;

        if (model is IChargeable)
            isCharge = true;
        if (model is IRepair)
            isRepair = true;
        if (model is IScan)
            isScan = true;

        ChargeButton.IsEnabled = isCharge;
        ScanButton.IsEnabled = isScan;
        RepairButton.IsEnabled = isRepair;
        DeleteRobotButton.IsEnabled = true;
        CrazyActionButton.IsEnabled = true;
    }

    private void ChargeButton_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedVm?.Model is not IChargeable chargeable) return;

        chargeable.Recharge(10);
        SelectedVm.Charge = SelectedVm.Model.Charge;

        Logs.Add(SelectedVm.Model.GetLastLog());
    }

    private void DeleteRobotButton_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedVm == null) return;
        RobotVMs.Remove(SelectedVm);
    }

    private void RobotIdBox_GotFocus(object sender, RoutedEventArgs e) => WpfUtils.SetActiveText(RobotIdBox, RobotIdErrorLabel);
    private void RobotIdBox_LostFocus(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(RobotIdBox.Text)) WpfUtils.SetDefault(RobotIdBox, "from 3 to 12 symbols");
    }

    private void AddRobotButton_Click(object sender, RoutedEventArgs e)
    {
        var hasError = false;

        if (string.IsNullOrWhiteSpace(RobotIdBox.Text) 
            || RobotIdBox.Text == "from 3 to 12 symbols"
            || RobotIdBox.Text.Length < 3
            || RobotIdBox.Text.Length > 12)
        {
            WpfUtils.SetError(RobotIdBox, RobotIdErrorLabel);
            hasError = true;
        }

        var robotType = (RobotTypeCombo.SelectionBoxItem as RobotTypeViewModel)?.RobotType;
        if (robotType == null)
        {
            RobotTypeErrorLabel.Visibility = Visibility.Visible;
            hasError = true;
        }

        if (hasError) return;

        if (robotType == typeof(CleanerBot))
            RobotVMs.Add(new RobotViewModel(RobotFactory.AddCleanerBot(RobotIdBox.Text)));
        else if (robotType == typeof(ExplorerBot))
            RobotVMs.Add(new RobotViewModel(RobotFactory.AddExplorerBot(RobotIdBox.Text)));
        else if (robotType == typeof(GuardBot))
            RobotVMs.Add(new RobotViewModel(RobotFactory.AddGuardBot(RobotIdBox.Text)));
        else if (robotType == typeof(RepairBot))
            RobotVMs.Add(new RobotViewModel(RobotFactory.AddRepairBot(RobotIdBox.Text)));
    }

    private void RobotTypeCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) => RobotTypeErrorLabel.Visibility = Visibility.Hidden;

    private void CrazyActionButton_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedVm == null) return;
        SelectedVm.Model.CrazyAction();

        SelectedVm.Charge = SelectedVm.Model.Charge;
        SelectedVm.Durability = SelectedVm.Model.Durability;

        Logs.Add(SelectedVm.Model.GetLastLog());
    }

    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedVm != null) CheckButtons(SelectedVm.Model);
        ClearButton.IsEnabled = false;

        TodoLabel.Content = string.Empty;
        _isRepair = false;
        _isScan = false;
    }

    private void ScanButton_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedVm == null) return;

        ChargeButton.IsEnabled = false;
        ScanButton.IsEnabled = false;
        RepairButton.IsEnabled = false;
        DeleteRobotButton.IsEnabled = false;
        CrazyActionButton.IsEnabled = false;
        ClearButton.IsEnabled = true;

        _isScan = true;
        TodoLabel.Content = $"{SelectedVm.Id} is scanning\nSelect a robot for scanning";
        ClearButton.Content = "Stop scanning";
    }

    private void RepairButton_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedVm == null) return;

        ChargeButton.IsEnabled = false;
        ScanButton.IsEnabled = false;
        RepairButton.IsEnabled = false;
        DeleteRobotButton.IsEnabled = false;
        CrazyActionButton.IsEnabled = false;
        ClearButton.IsEnabled = true;

        _isRepair = true;
        TodoLabel.Content = $"{SelectedVm.Id} is repairing\nSelect a robot for repairing";
        ClearButton.Content = "Stop repairing";
    }
}