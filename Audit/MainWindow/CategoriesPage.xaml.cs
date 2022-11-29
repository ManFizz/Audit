using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Audit.Objects;

namespace Audit.MainWindow;

public partial class CategoriesPage : Page
{
    public CategoriesPage()
    {
        InitializeComponent();
        UpdateTable();
    }

    public void UpdateTable()
    {
        var app = (App)Application.Current;
        var result = app.FastQuery("SELECT * FROM categories");
        //app.ArrCategories.Clear();
        while (result.Read())
        {
            //app.ArrCategories.Add(new Category(result.GetInt32(0),result.GetString(1),result.GetInt32(2)));
            var row = new TableRow();
            for (var i = 0; i < 3; i++)
            {
                var cell = new TableCell();
                cell.Blocks.Add(new Paragraph(new Run(result.GetString(i))));
                row.Cells.Add(cell);
            }
            TblCategoriesData.Rows.Add(row);

        }
    }
}