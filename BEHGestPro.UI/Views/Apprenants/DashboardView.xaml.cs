using BEHGestPro.UI.ViewModels;
using System.Windows.Controls;

namespace BEHGestPro.UI.Views.Apprenants;

public partial class DashboardView : UserControl
{
    public DashboardView()
    {
        InitializeComponent();
        Loaded += async (_, _) =>
        {
            if (DataContext is DashboardViewModel vm)
                await vm.LoadAsync();
        };
    }
}
