using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace Audit.MainWindow;

public partial class MainWindow : Window
{
    private readonly LogInWindow _logInWindow;
    public MainWindow(LogInWindow logInWindow)
    {
        InitializeComponent();
        _logInWindow = logInWindow;
        
    }

    private bool _shown;
    protected override void OnContentRendered(EventArgs e)
    {
        base.OnContentRendered(e);

        if (_shown)
            return;

        _shown = true;
        _logInWindow.Close();
    }

    private void UpdateNavColors(Button btn)
    {
        BtnCategories.Background = Brushes.LightGray;
        BtnWorkers.Background = Brushes.LightGray;
        BtnHoursRecords.Background = Brushes.LightGray;
        BtnCompany.Background = Brushes.LightGray;
        BtnUsers.Background = Brushes.LightGray;
        btn.Background = Brushes.Lavender;
    }
    
    private void OnClick_BtnNavCategories(object sender, RoutedEventArgs e)
    {
        UpdateNavColors(BtnCategories);
        CenterFrame.Source = new Uri("Pages/CategoriesPage.xaml", UriKind.Relative);
        RightFrame.Source = new Uri("Forms/CategoriesForm.xaml", UriKind.Relative);
    }

    private void OnClick_BtnNavWorkers(object sender, RoutedEventArgs e)
    {
        UpdateNavColors(BtnWorkers);
        CenterFrame.Source = new Uri("Pages/WorkersPage.xaml", UriKind.Relative);
        RightFrame.Source = new Uri("Forms/WorkersForm.xaml", UriKind.Relative);
    }

    private void OnClick_BtnNavHoursRecords(object sender, RoutedEventArgs e)
    {
        UpdateNavColors(BtnHoursRecords);
        CenterFrame.Source = new Uri("Pages/HoursRecordsPage.xaml", UriKind.Relative);
        RightFrame.Source = new Uri("Forms/HoursRecordsForm.xaml", UriKind.Relative);
    }

    private void OnClick_BtnNavCompany(object sender, RoutedEventArgs e)
    {
        UpdateNavColors(BtnCompany);
        CenterFrame.Source = new Uri("Pages/CompanyPage.xaml", UriKind.Relative);
        RightFrame.Source = new Uri("Forms/CompanyForm.xaml", UriKind.Relative);
    }

    private void OnClick_BtnNavUsers(object sender, RoutedEventArgs e)
    {
        UpdateNavColors(BtnUsers);
        CenterFrame.Source = new Uri("Pages/UsersPage.xaml", UriKind.Relative);
        RightFrame.Source = new Uri("Forms/UsersForm.xaml", UriKind.Relative);
    }

    private void OnClick_ExtendBtn(object sender, RoutedEventArgs e)
    {
        if (RightFrame.Visibility == Visibility.Visible)
        {
            BtnExtra.Background = Brushes.LightGray;
            RightFrame.Visibility = Visibility.Collapsed;
            ExtraColumnDefinition.MinWidth = 0;
            ExtraColumnDefinition.Width = new GridLength(0);
        }
        else
        {
            BtnExtra.Background = Brushes.Lavender;
            RightFrame.Visibility = Visibility.Visible;
            ExtraColumnDefinition.MinWidth = 200;
            ExtraColumnDefinition.Width = new GridLength(1.5, GridUnitType.Star);
        }
    }
}