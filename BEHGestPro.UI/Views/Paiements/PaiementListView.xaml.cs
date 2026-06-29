using BEHGestPro.UI.ViewModels.Paiements;
using System.Windows.Controls;

namespace BEHGestPro.UI.Views.Paiements;

public partial class PaiementListView : UserControl
{
    public PaiementListView()
    {
        InitializeComponent();
        Loaded += async (_, _) =>
        {
            if (DataContext is PaiementListViewModel vm)
                await vm.LoadAsync();
        };
    }
}
