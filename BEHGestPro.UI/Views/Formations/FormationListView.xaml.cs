using BEHGestPro.UI.ViewModels.Formations;
using System.Windows.Controls;

namespace BEHGestPro.UI.Views.Formations;

public partial class FormationListView : UserControl
{
    public FormationListView()
    {
        InitializeComponent();
        Loaded += async (_, _) =>
        {
            if (DataContext is FormationListViewModel vm)
                await vm.LoadAsync();
        };
    }
}
