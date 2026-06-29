using BEHGestPro.UI.ViewModels.Stagiaires;
using System.Windows.Controls;

namespace BEHGestPro.UI.Views.Stagiaires;

public partial class StagiaireListView : UserControl
{
    public StagiaireListView()
    {
        InitializeComponent();
        Loaded += async (_, _) =>
        {
            if (DataContext is StagiaireListViewModel vm)
                await vm.LoadAsync();
        };
    }
}
