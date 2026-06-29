using System.Windows;
using System.Windows.Input;

namespace BEHGestPro.UI.Views.Shared;

public partial class FormDialog : Window
{
    public FormDialog(string title, System.Windows.Controls.UserControl content, double width = 580, double height = 480)
    {
        InitializeComponent();
        TxtTitle.Text = title;
        Width  = width;
        Height = height;
        ContentHost.Content = content;
    }

    private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) =>
        DragMove();

    private void CloseBtn_Click(object sender, RoutedEventArgs e) =>
        Close();

    /// <summary>Close dialog with result = true (save confirmed).</summary>
    public void CloseWithSuccess()
    {
        DialogResult = true;
        Close();
    }
}
