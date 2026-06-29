using BEHGestPro.UI.ViewModels.Salaires;
using System.Windows.Controls;

namespace BEHGestPro.UI.Views.Salaires;

public partial class SalaireListView : UserControl
{
    public SalaireListView()
    {
        InitializeComponent();
        Loaded += async (_, _) =>
        {
            if (DataContext is SalaireListViewModel vm)
                await vm.LoadAsync();
        };
    }
}
