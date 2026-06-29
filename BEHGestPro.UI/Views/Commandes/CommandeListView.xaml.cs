using BEHGestPro.UI.ViewModels.Commandes;
using System.Windows.Controls;

namespace BEHGestPro.UI.Views.Commandes;

public partial class CommandeListView : UserControl
{
    public CommandeListView()
    {
        InitializeComponent();
        Loaded += async (_, _) =>
        {
            if (DataContext is CommandeListViewModel vm)
                await vm.LoadAsync();
        };
    }
}
