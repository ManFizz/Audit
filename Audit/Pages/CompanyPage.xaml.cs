using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Audit.Objects;

namespace Audit.Pages;

public partial class CompanyPage : Page
{
    public CompanyPage()
    {
        InitializeComponent();
        var app = (App) Application.Current;
        CompanyGrid.ItemsSource = app.ArrCompany;
        app.LastDataGrid = CompanyGrid;
        if (app.ActiveUser.Type is not (TypeUser.timekeeper or TypeUser.hr or TypeUser.admin))
        {
            var gridSearch = (IdSearch.Parent as Grid)!;
            
            gridSearch.ColumnDefinitions[0].Width = new GridLength(0);
            IdSearch.Visibility = Visibility.Collapsed;
            CompanyGrid.Columns[0].Visibility = Visibility.Collapsed;
            
            CompanyGrid.Columns[1].IsReadOnly = true;
            CompanyGrid.Columns[2].IsReadOnly = true;
            CompanyGrid.CanUserAddRows = false;
        }
    }

    private void Search_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        var app = (App) Application.Current;
        var view = CollectionViewSource.GetDefaultView(app.ArrCompany);
        view.Filter = o => {
            var company = ((o as Company)!);
            return (string.IsNullOrWhiteSpace(NameSearch.Text) || company.Name.Contains(NameSearch.Text))
                   && (string.IsNullOrWhiteSpace(IdSearch.Text) || company.Id.ToString().Contains(IdSearch.Text))
                   && (string.IsNullOrWhiteSpace(AddressSearch.Text) || company.Address.Contains(AddressSearch.Text));
        };
    }

    private void CompanyGrid_OnPreviewCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        if (e.Command != DataGrid.DeleteCommand)
            return;
        
        e.Handled = true;
        
        ((Company) ((DataGrid)sender).SelectedItem).Remove();
    }

    private bool _inEditNewItemMode;
    private Company _lastItem;
    private bool _isFull;
    private bool SkipNextSelect;
    private void CompanyGrid_OnAddingNewItem(object? sender, AddingNewItemEventArgs e)
    {
        if (e.NewItem != null)
            return;
        
        try
        {
            _lastItem = new Company(-1, "", "");
        } catch (Exception exception) {
            MessageBox.Show(exception.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
            _isFull = true;
            return;
        }
        
        _inEditNewItemMode = true;
        e.NewItem = _lastItem;
        SkipNextSelect = true;
    }
    
    
    private bool _isFieldNameOk;
    private bool _isFieldAddressOk;
    private bool _isInEditCell;
    private bool _termianteCellEdit;
    private void CompanyGrid_OnCellEditEnding(object? sender, DataGridCellEditEndingEventArgs e)
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
                if (e.Column == CompanyGrid.Columns[1])
                    _isFieldNameOk = false;
                else if (e.Column == CompanyGrid.Columns[2])
                    _isFieldAddressOk = false;
            }
            _isInEditCell = false;
            return;
        }

        try {
            if (e.Column == CompanyGrid.Columns[1])
            {
                ((Company) CompanyGrid.SelectedItem).CheckName(((TextBox) e.EditingElement).Text);
                _isFieldNameOk = true;
            }
            else if (e.Column == CompanyGrid.Columns[2])
            {
                ((Company)CompanyGrid.SelectedItem).CheckAddress(((TextBox)e.EditingElement).Text);
                _isFieldAddressOk = true;
            }
            
            _isInEditCell = false;
        }
        catch (Exception exception)
        {
            e.Cancel = true;
            MessageBox.Show(exception.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
        }
    }

    private void CompanyGrid_OnRowEditEnding(object? sender, DataGridRowEditEndingEventArgs e)
    {
        if (!_inEditNewItemMode) 
            return;
        
        _inEditNewItemMode = false;
        if (!_isFieldNameOk || !_isFieldAddressOk)
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
        _isFieldAddressOk = _isFieldNameOk = false;
        _inEditNewItemMode = false;
        _termianteCellEdit = false;
        _isInEditCell = false;
    }
    
    private void CompanyGrid_OnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
    {
        if (!_inEditNewItemMode || SkipNextSelect)
        {
            SkipNextSelect = false;
            return;
        }

        _termianteCellEdit = true;
        var cc = CompanyGrid.CurrentCell;
        foreach (var col in CompanyGrid.Columns)
        {
            CompanyGrid.CurrentCell = new DataGridCellInfo(_lastItem, col);
            CompanyGrid.CancelEdit();
        }
        CompanyGrid.CurrentCell = cc;
        _isInEditCell = false;
    }

    private void CompanyGrid_OnPreparingCellForEdit(object? sender, DataGridPreparingCellForEditEventArgs e)
    {
        if (_isInEditCell)
            CompanyGrid.CancelEdit();
        _isInEditCell = true;
    }

    private void CompanyGrid_OnInitializingNewItem(object sender, InitializingNewItemEventArgs e)
    {
        if (!_isFull)
            return;
        
        var app = (App) Application.Current;
        app.ArrCompany.Remove((Company)e.NewItem);
    }
}