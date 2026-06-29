using BEHGestPro.UI.ViewModels.Employes;
using System.Windows.Controls;

namespace BEHGestPro.UI.Views.Employes;

public partial class EmployeListView : UserControl
{
    public EmployeListView()
    {
        InitializeComponent();
        Loaded += async (_, _) =>
        {
            if (DataContext is EmployeListViewModel vm)
                await vm.LoadAsync();
        };
    }
}
