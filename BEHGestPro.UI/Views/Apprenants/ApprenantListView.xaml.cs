using BEHGestPro.UI.ViewModels.Apprenants;
using System.Windows.Controls;

namespace BEHGestPro.UI.Views.Apprenants;

public partial class ApprenantListView : UserControl
{
    public ApprenantListView()
    {
        InitializeComponent();
        Loaded += async (_, _) =>
        {
            if (DataContext is ApprenantListViewModel vm)
                await vm.LoadAsync();
        };
    }
}
