using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Audit.Objects;
using MySql.Data.MySqlClient;

namespace Audit.Pages;

public partial class HoursRecordsPage : Page
{
    public HoursRecordsPage()
    {
        InitializeComponent();
        var app = (App) Application.Current;
        HoursRecordsGrid.ItemsSource = app.ArrHoursRecords;
        app.LastDataGrid = HoursRecordsGrid;
        
        if (app.ActiveUser.Type != TypeUser.timekeeper)
        {
            var gridSearch = (IdSearch.Parent as Grid)!;
            IdSearch.Visibility = Visibility.Collapsed;
            gridSearch.ColumnDefinitions[0].Width = new GridLength(0);
            HoursRecordsGrid.Columns[0].Visibility = Visibility.Collapsed;
            
            IdCompanySearch.Visibility = Visibility.Collapsed;
            gridSearch.ColumnDefinitions[1].Width = new GridLength(0);
            HoursRecordsGrid.Columns[1].Visibility = Visibility.Collapsed;
            
            HoursRecordsGrid.Columns[2].IsReadOnly = true;
            
            IdWorkerSearch.Visibility = Visibility.Collapsed;
            gridSearch.ColumnDefinitions[2].Width = new GridLength(0);
            HoursRecordsGrid.Columns[3].Visibility = Visibility.Collapsed;
            
            HoursRecordsGrid.Columns[4].IsReadOnly = true;
            HoursRecordsGrid.Columns[5].IsReadOnly = true;
            HoursRecordsGrid.Columns[6].IsReadOnly = true;
            HoursRecordsGrid.CanUserAddRows = false;
        }
        
        if(app.ActiveUser.Type == TypeUser.worker)
            Search_OnTextChanged(null, null); //Force update table
    }
    

    private readonly ObservableCollection<HoursRecord> _arr = new ();
    private void Search_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        _arr.Clear();
        IdSearch.Foreground = Brushes.White;
        IdCompanySearch.Foreground = Brushes.White;
        IdWorkerSearch.Foreground = Brushes.White;
        DataSearch.Foreground = Brushes.White;
        HoursSearch.Foreground = Brushes.White;
        
        var app = (App) Application.Current;
        if (app.ActiveUser.Type != TypeUser.worker 
            || string.IsNullOrWhiteSpace(IdSearch.Text + IdCompanySearch.Text + IdWorkerSearch.Text + DataSearch.Text + HoursSearch.Text))
        {
            HoursRecordsGrid.ItemsSource = app.ArrHoursRecords;
            return;
        }
        HoursRecordsGrid.ItemsSource = _arr;
        
        var id = -1;
        if (!string.IsNullOrWhiteSpace(IdSearch.Text) && !int.TryParse(IdSearch.Text, out id))
        {
            IdSearch.Foreground = Brushes.Red;
            return;
        }
        var sId = id.ToString();
        
        var idComp = -1;
        if (!string.IsNullOrWhiteSpace(IdCompanySearch.Text) && !int.TryParse(IdCompanySearch.Text, out idComp))
        {
            IdCompanySearch.Foreground = Brushes.Red;
            return;
        }
        var sIdComp = idComp.ToString();
        
        var idWork = -1;
        if (app.ActiveUser.Type == TypeUser.worker)
            idWork = app.ActiveUser.Id;
        else if (!string.IsNullOrWhiteSpace(IdWorkerSearch.Text) && !int.TryParse(IdWorkerSearch.Text, out idWork))
        {
            IdWorkerSearch.Foreground = Brushes.Red;
            return;
        }
        var sIdWork = idWork.ToString();
        
        var regex = new Regex(@"[^0-9\.]+");
        var matches = regex.Matches(DataSearch.Text);
        if (matches.Count > 0)
        {
            DataSearch.Foreground = Brushes.Red;
            return;
        }
        var sDt = DataSearch.Text.Trim();
        
        
        var hours = -1;
        if (!string.IsNullOrWhiteSpace(HoursSearch.Text) && !int.TryParse(HoursSearch.Text, out hours))
        {
            HoursSearch.Foreground = Brushes.Red;
            return;
        }
        var sHours = hours.ToString();
        
        foreach (var hoursRecord in app.ArrHoursRecords)
        {
            if (id != -1 && !hoursRecord.Id.ToString().Contains(sId))
                continue;
            
            if (idComp != -1 && !hoursRecord.CompanyId.ToString().Contains(sIdComp))
                continue;
            
            if (idWork != -1 && !hoursRecord.WorkerId.ToString().Contains(sIdWork))
                continue;
            
            if (!string.IsNullOrWhiteSpace(sDt) && !hoursRecord.Date.Contains(sDt))
                continue;
            
            if (hours != -1 && !hoursRecord.Hours.ToString().Contains(sHours))
                continue;


            _arr.Add(hoursRecord);
        }
    }

    private bool _isFull;
    private void HoursRecordsGrid_OnPreviewCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        if (e.Command != DataGrid.DeleteCommand)
            return;
        
        ((sender as DataGrid)!.SelectedItem as HoursRecord)!.Remove();
        e.Handled = true;
        _isFull = false;
    }

    private bool _inEditNewItemMode;
    private HoursRecord _lastItem;
    private bool _skipNextSelect;
    private void HoursRecordsGrid_OnAddingNewItem(object? sender, AddingNewItemEventArgs e)
    {
        if (e.NewItem != null)
            return;
        try
        {
            _lastItem = new HoursRecord(-1, -1, -1, DateTime.UtcNow.Date.ToString("dd.MM.yyyy"), 1);
            e.NewItem = _lastItem;
        } catch (Exception exception) {
            MessageBox.Show(exception.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
            _isFull = true;
            return;
        }

        _inEditNewItemMode = true;
        e.NewItem = _lastItem;
        _skipNextSelect = true;
    }

    private bool[] _isFieldOk = {true, false, true, false, true, false, false};
    private bool _isInEditCell;
    private void HoursRecordsGrid_OnCellEditEnding(object? sender, DataGridCellEditEndingEventArgs e)
    {
        if (_termianteCellEdit)
        {
            _isInEditCell = false;
            return;
        }

        if (e.EditAction != DataGridEditAction.Commit)
        {
            if (_inEditNewItemMode)
            {
                for (var i = 0; i < HoursRecordsGrid.Columns.Count; i++)
                {
                    if (e.Column == HoursRecordsGrid.Columns[i])
                    {
                        _isFieldOk[i] = false;
                        break;
                    }
                }
            }
            _isInEditCell = false;
            return;
        }

        try {
            if (e.Column == HoursRecordsGrid.Columns[1])
            {
                var sItem = ((TextBox) e.EditingElement).Text;
                if (!int.TryParse(sItem, out var item))
                    throw new Exception("Ожидалось целое число");
                try
                {
                    HoursRecord.CheckCompanyId(item);
                }
                catch (Exception)
                {
                    e.Cancel = true;
                    return;
                }

                _isFieldOk[1] = true;
            }
            else if (e.Column == HoursRecordsGrid.Columns[3])
            {
                var sItem = ((TextBox) e.EditingElement).Text;
                if (!int.TryParse(sItem, out var item))
                    throw new Exception("Ожидалось целое число");
                try
                {
                    HoursRecord.CheckWorkerId(item);
                }
                catch (Exception)
                {
                    e.Cancel = true;
                    return;
                }

                _isFieldOk[3] = true;
            }
            else if (e.Column == HoursRecordsGrid.Columns[5])
            {
                HoursRecord.CheckDate(((TextBox) e.EditingElement).Text);
                _isFieldOk[5] = true;
            }
            else if (e.Column == HoursRecordsGrid.Columns[6])
            {
                var sItem = ((TextBox) e.EditingElement).Text;
                if (!int.TryParse(sItem, out var item))
                    throw new Exception("Ожидалось целое число");
                try
                {
                    HoursRecord.CheckHours(item);
                }
                catch (Exception)
                {
                    e.Cancel = true;
                    return;
                }

                _isFieldOk[6] = true;
            }

            _isInEditCell = false;
        }
        catch (Exception exception)
        {
            e.Cancel = true;
            MessageBox.Show(exception.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
        }
    }
    
    
    private void HoursRecordsGrid_OnRowEditEnding(object? sender, DataGridRowEditEndingEventArgs e)
    {
        if (!_inEditNewItemMode) 
            return;
        
        if (_isFieldOk.Any(t => !t))
        {
            if (e.EditAction == DataGridEditAction.Commit && !_termianteCellEdit)
            {
                e.Cancel = true;
                return;
            }
            
            //_lastItem.Remove(); //never return false
        }
        else
        {
            _lastItem.Insert();
        }

        _isFieldOk = new [] {true, false, true, false, true, false, false};
        
        _inEditNewItemMode = false;
        _termianteCellEdit = false;
        _isInEditCell = false;
    }

    private bool _termianteCellEdit;
    private void HoursRecordsGrid_OnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
    {
        if (!_inEditNewItemMode || _skipNextSelect)
        {
            _skipNextSelect = false;
            return;
        }
    
        _termianteCellEdit = true;
        var cc = HoursRecordsGrid.CurrentCell;
        foreach (var col in HoursRecordsGrid.Columns)
        {
            HoursRecordsGrid.CurrentCell = new DataGridCellInfo(_lastItem, col);
            HoursRecordsGrid.CancelEdit();
        }
        HoursRecordsGrid.CurrentCell = cc;
        _isInEditCell = false;
    }
    
    private void HoursRecordsGrid_OnPreparingCellForEdit(object? sender, DataGridPreparingCellForEditEventArgs e)
    {
        if (_isInEditCell)
            HoursRecordsGrid.CancelEdit();
        _isInEditCell = true;
        
        if (e.Column == HoursRecordsGrid.Columns[1])
        {
            RightFrame.Source = new Uri("CompanyPage.xaml", UriKind.Relative);
            BtnCompany.Visibility = Visibility.Visible;
            BtnWorker.Visibility = Visibility.Collapsed;
        }
        else if (e.Column == HoursRecordsGrid.Columns[3])
        {
            RightFrame.Source = new Uri("WorkersPage.xaml", UriKind.Relative);
            BtnWorker.Visibility = Visibility.Visible;
            BtnCompany.Visibility = Visibility.Collapsed;
        }
        else
        {
            RightFrame.Source = null;
            BtnCompany.Visibility = Visibility.Collapsed;
            BtnWorker.Visibility = Visibility.Collapsed;
        }
    }

    private void HoursRecordsGrid_OnInitializingNewItem(object sender, InitializingNewItemEventArgs e)
    {
        if (!_isFull)
            return;
        
        var app = (App) Application.Current;
        app.ArrHoursRecords.Remove((HoursRecord)e.NewItem);
    }
    
    private void RightFrame_OnContentRendered(object? sender, EventArgs e)
    {
        var app = (App) Application.Current;
        var dg = app.LastDataGrid;
        foreach (var col in dg.Columns)
            col.IsReadOnly = true;
    }

    private void HoursRecordsGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        RightFrame.Source = null;
        BtnCompany.Visibility = Visibility.Collapsed;
        BtnWorker.Visibility = Visibility.Collapsed;
    }

    private void BtnCompany_OnClick(object sender, RoutedEventArgs e)
    {
        var app = (App) Application.Current;
        var dg = app.LastDataGrid;
        var selectedItem = dg.SelectedItem;
        if (selectedItem == null)
        {
            MessageBox.Show("Для изменения, требуется выбрать запись", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
            return;
        }
        
        ((HoursRecord) HoursRecordsGrid.SelectedItem).CompanyId = ((Company) selectedItem).Id;
    }

    private void BtnWorker_OnClick(object sender, RoutedEventArgs e)
    {
        var app = (App) Application.Current;
        var dg = app.LastDataGrid;
        var selectedItem = dg.SelectedItem;
        if (selectedItem == null)
        {
            MessageBox.Show("Для изменения, требуется выбрать запись", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
            return;
        }
        
        ((HoursRecord) HoursRecordsGrid.SelectedItem).WorkerId = ((Worker) selectedItem).Id;
    }
}