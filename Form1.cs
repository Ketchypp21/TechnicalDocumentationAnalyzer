using System.Data;
using TechnicalDocumentationAnalyzer.Services;

namespace TechnicalDocumentationAnalyzer;

public partial class Form1 : Form
{
    private DataTable? _sourceTable;

    public Form1()
    {
        InitializeComponent();

        loadButton.Click += LoadButton_Click;

        filterColumnComboBox.SelectedIndexChanged +=
            FilterColumnComboBox_SelectedIndexChanged;

        filterValueComboBox.SelectedIndexChanged +=
            (_, _) => ApplyFilters();

        searchTextBox.TextChanged +=
            (_, _) => ApplyFilters();

        resetFiltersButton.Click +=
            ResetFiltersButton_Click;
    }

    private void LoadButton_Click(object? sender, EventArgs e)
    {
        using var openFileDialog = new OpenFileDialog
        {
            Title = "Выберите CSV-файл",
            Filter = "CSV-файлы (*.csv)|*.csv|Все файлы (*.*)|*.*",
            CheckFileExists = true,
            Multiselect = false
        };

        if (openFileDialog.ShowDialog() != DialogResult.OK)
            return;

        try
        {
            _sourceTable = CsvLoader.Load(openFileDialog.FileName);

            documentsGrid.DataSource = _sourceTable.DefaultView;

            FillColumnFilter();

            fileNameLabel.Text =
                $"Файл: {Path.GetFileName(openFileDialog.FileName)}";

            Text =
                $"Анализ технической документации — " +
                $"{Path.GetFileName(openFileDialog.FileName)}";

            UpdateRecordCount();

            MessageBox.Show(
                $"Файл успешно загружен.\n" +
                $"Колонок: {_sourceTable.Columns.Count}\n" +
                $"Записей: {_sourceTable.Rows.Count}",
                "Загрузка завершена",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                $"Не удалось загрузить CSV-файл:\n" +
                $"{exception.Message}",
                "Ошибка",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void FillColumnFilter()
    {
        filterColumnComboBox.Items.Clear();
        filterColumnComboBox.Items.Add("Все колонки");

        if (_sourceTable is not null)
        {
            foreach (DataColumn column in _sourceTable.Columns)
            {
                filterColumnComboBox.Items.Add(column.ColumnName);
            }
        }

        filterColumnComboBox.SelectedIndex = 0;

        filterValueComboBox.Items.Clear();
        filterValueComboBox.Items.Add("Все значения");
        filterValueComboBox.SelectedIndex = 0;
        filterValueComboBox.Enabled = false;
    }

    private void FilterColumnComboBox_SelectedIndexChanged(
        object? sender,
        EventArgs e)
    {
        filterValueComboBox.Items.Clear();
        filterValueComboBox.Items.Add("Все значения");

        if (_sourceTable is null ||
            filterColumnComboBox.SelectedIndex <= 0)
        {
            filterValueComboBox.Enabled = false;
            filterValueComboBox.SelectedIndex = 0;

            ApplyFilters();
            return;
        }

        string columnName =
            filterColumnComboBox.SelectedItem!.ToString()!;

        var values = _sourceTable.Rows
            .Cast<DataRow>()
            .Select(row =>
                Convert.ToString(row[columnName])?.Trim()
                ?? string.Empty)
            .Where(value => value.Length > 0)
            .Distinct(StringComparer.CurrentCultureIgnoreCase)
            .OrderBy(value => value);

        foreach (string value in values)
        {
            filterValueComboBox.Items.Add(value);
        }

        filterValueComboBox.Enabled = true;
        filterValueComboBox.SelectedIndex = 0;

        ApplyFilters();
    }

    private void ApplyFilters()
    {
        if (_sourceTable is null)
            return;

        var expressions = new List<string>();

        string searchText = searchTextBox.Text.Trim();

        if (searchText.Length > 0)
        {
            string escapedSearch =
                EscapeFilterValue(searchText);

            var columnExpressions = _sourceTable.Columns
                .Cast<DataColumn>()
                .Select(column =>
                    $"[{EscapeColumnName(column.ColumnName)}] " +
                    $"LIKE '%{escapedSearch}%'");

            expressions.Add(
                $"({string.Join(" OR ", columnExpressions)})");
        }

        if (filterColumnComboBox.SelectedIndex > 0 &&
            filterValueComboBox.SelectedIndex > 0)
        {
            string columnName =
                filterColumnComboBox.SelectedItem!.ToString()!;

            string selectedValue =
                filterValueComboBox.SelectedItem!.ToString()!;

            expressions.Add(
                $"[{EscapeColumnName(columnName)}] = " +
                $"'{EscapeFilterValue(selectedValue)}'");
        }

        _sourceTable.DefaultView.RowFilter =
            string.Join(" AND ", expressions);

        UpdateRecordCount();
    }

    private void ResetFiltersButton_Click(
        object? sender,
        EventArgs e)
    {
        searchTextBox.Clear();

        if (filterColumnComboBox.Items.Count > 0)
        {
            filterColumnComboBox.SelectedIndex = 0;
        }

        ApplyFilters();
    }

    private void UpdateRecordCount()
    {
        if (_sourceTable is null)
        {
            recordsCountLabel.Text = "Записей: 0";
            return;
        }

        recordsCountLabel.Text =
            $"Записей: {_sourceTable.DefaultView.Count} " +
            $"из {_sourceTable.Rows.Count}";
    }

    private static string EscapeFilterValue(string value)
    {
        return value.Replace("'", "''");
    }

    private static string EscapeColumnName(string columnName)
    {
        return columnName
            .Replace("\\", "\\\\")
            .Replace("]", "\\]");
    }
}