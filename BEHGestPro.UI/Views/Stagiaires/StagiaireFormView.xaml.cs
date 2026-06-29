using BEHGestPro.UI.Views.Shared;
using System.Windows;
using System.Windows.Controls;

namespace BEHGestPro.UI.Views.Stagiaires;

public partial class StagiaireFormView : UserControl
{
    public StagiaireFormView()
    {
        InitializeComponent();
    }

    private void CancelBtn_Click(object sender, RoutedEventArgs e)
    {
        var window = Window.GetWindow(this) as FormDialog;
        window?.Close();
    }
}
