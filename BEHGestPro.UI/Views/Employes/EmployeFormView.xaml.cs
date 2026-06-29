using BEHGestPro.UI.Views.Shared;
using System.Windows;
using System.Windows.Controls;

namespace BEHGestPro.UI.Views.Employes;

public partial class EmployeFormView : UserControl
{
    public EmployeFormView()
    {
        InitializeComponent();
    }

    private void CancelBtn_Click(object sender, RoutedEventArgs e)
    {
        var window = Window.GetWindow(this) as FormDialog;
        window?.Close();
    }
}
