using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Audit.Objects;
using MySql.Data.MySqlClient;
using static System.Int32;

namespace Audit.Pages;

public partial class CategoriesPage : Page
{
    public CategoriesPage()
    {
        InitializeComponent();
        var app = (App) Application.Current;
        CategoriesGrid.ItemsSource = app.ArrCategories;
        app.LastDataGrid = CategoriesGrid;
        if (app.ActiveUser.Type != TypeUser.hr)
        {
            IdSearch.Visibility = Visibility.Collapsed;
            (IdSearch.Parent as Grid)!.ColumnDefinitions[0].Width = new GridLength(0);
            CategoriesGrid.Columns[0].Visibility = Visibility.Collapsed;
            
            CategoriesGrid.Columns[1].IsReadOnly = true;
            CategoriesGrid.Columns[2].IsReadOnly = true;
            CategoriesGrid.CanUserAddRows = false;
        }
    }


    private readonly ObservableCollection<Category> _arr = new ();
    private void Search_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        //TODO Optimizations https://stackoverflow.com/questions/13815607/find-a-record-in-wpf-datagrid-by-typing
        _arr.Clear();
        PaymentSearch.Foreground = Brushes.White;
        IdSearch.Foreground = Brushes.White;
        
        var app = (App) Application.Current;
        if (string.IsNullOrWhiteSpace(IdSearch.Text + NameSearch.Text + PaymentSearch.Text))
        {
            CategoriesGrid.ItemsSource = app.ArrCategories;
            return;
        }
        CategoriesGrid.ItemsSource = _arr;
        
        var id = -1;
        if (!string.IsNullOrWhiteSpace(IdSearch.Text) && !TryParse(IdSearch.Text, out id))
        {
            IdSearch.Foreground = Brushes.Red;
            return;
        }
        var sId = id.ToString();

        var name = NameSearch.Text;

        var payment = -1;
        if (!string.IsNullOrWhiteSpace(PaymentSearch.Text) && !TryParse(PaymentSearch.Text, out payment))
        {
            PaymentSearch.Foreground = Brushes.Red;
            return;
        }

        var sPayment = payment.ToString();
        foreach (var c in app.ArrCategories)
        {
            if (id != -1 && !c.Id.ToString().Contains(sId))
                continue;

            if (!string.IsNullOrWhiteSpace(name) && !c.Name.Contains(name))
                continue;
            
            if (payment != -1 && !c.Payment.ToString().Contains(sPayment))
                continue;
            
            _arr.Add(c);
        }
    }

    private void Grid_PreviewCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        if (e.Command != DataGrid.DeleteCommand)
            return;
        
        e.Handled = true;
        
        ((sender as DataGrid)!.SelectedItem as Category)!.Remove();
        _isFull = false;
    }
    
    private bool _inEditNewItemMode;
    private Category _lastItem;
    private bool _isFull;
    private bool SkipNextSelect;
    private void CategoriesGrid_OnAddingNewItem(object? sender, AddingNewItemEventArgs e)
    {
        if (e.NewItem != null)
            return;
        
        try
        {
            _lastItem = new Category(-1, "", 0);
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
    private bool _isFieldPaymentOk;
    private bool _isInEditCell;
    private bool _termianteCellEdit;
    private void CategoriesGrid_OnCellEditEnding(object? sender, DataGridCellEditEndingEventArgs e)
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
                if (e.Column == CategoriesGrid.Columns[1])
                    _isFieldNameOk = false;
                else if (e.Column == CategoriesGrid.Columns[2])
                    _isFieldPaymentOk = false;
            }
            _isInEditCell = false;
            return;
        }

        try {
            if (e.Column == CategoriesGrid.Columns[1])
            {
                ((Category) CategoriesGrid.SelectedItem).CheckName(((TextBox) e.EditingElement).Text);
                _isFieldNameOk = true;
            }
            else if (e.Column == CategoriesGrid.Columns[2])
            {
                var sPayment = ((TextBox) e.EditingElement).Text;
                if (!TryParse(sPayment, out var payment))
                    throw new Exception("Ожидалось целое число");
                
                ((Category) CategoriesGrid.SelectedItem).CheckPayment(payment);
                _isFieldPaymentOk = true;
            }

            _isInEditCell = false;
        }
        catch (Exception exception)
        {
            e.Cancel = true;
            MessageBox.Show(exception.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
        }
    }
    
    private void CategoriesGrid_OnRowEditEnding(object? sender, DataGridRowEditEndingEventArgs e)
    {
        if (!_inEditNewItemMode) 
            return;
        
        if (!_isFieldPaymentOk || !_isFieldNameOk)
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
        _isFieldPaymentOk = _isFieldNameOk = false;
        _inEditNewItemMode = false;
        _termianteCellEdit = false;
        _isInEditCell = false;
    }

    private void CategoriesGrid_OnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
    {
        if (!_inEditNewItemMode || SkipNextSelect)
        {
            SkipNextSelect = false;
            return;
        }

        _termianteCellEdit = true;
        var cc = CategoriesGrid.CurrentCell;
        foreach (var col in CategoriesGrid.Columns)
        {
            CategoriesGrid.CurrentCell = new DataGridCellInfo(_lastItem, col);
            CategoriesGrid.CancelEdit();
        }
        CategoriesGrid.CurrentCell = cc;
        _isInEditCell = false;
    }

    private void CategoriesGrid_OnPreparingCellForEdit(object? sender, DataGridPreparingCellForEditEventArgs e)
    {
        if (_isInEditCell)
            CategoriesGrid.CancelEdit();
        _isInEditCell = true;
    }

    private void CategoriesGrid_OnInitializingNewItem(object sender, InitializingNewItemEventArgs e)
    {
        if (!_isFull)
            return;
        
        var app = (App) Application.Current;
        app.ArrCategories.Remove((Category)e.NewItem);
    }
}