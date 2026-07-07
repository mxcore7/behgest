using BEHGestPro.UI.ViewModels.Utilisateurs;
using System.Windows.Controls;

namespace BEHGestPro.UI.Views.Utilisateurs;

public partial class UtilisateurListView : UserControl
{
    public UtilisateurListView()
    {
        InitializeComponent();
        Loaded += async (_, _) =>
        {
            if (DataContext is UtilisateurListViewModel vm)
                await vm.LoadAsync();
        };
    }
}
