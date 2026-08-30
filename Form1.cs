using System.Data;
using System.Text;
using TechnicalDocumentationAnalyzer.Services;

namespace TechnicalDocumentationAnalyzer;

public partial class Form1 : Form
{
    private DataTable? _sourceTable;
    private string? _currentFilePath;

    // Запрещает обработку событий, пока интерфейс
    // перестраивается под новый CSV.
    private bool _isUpdatingInterface;

    public Form1()
    {
        InitializeComponent();

        documentsGrid.VirtualMode = false;
        statisticsGrid.VirtualMode = false;

        InitializeStatistics();
        FillEncodingList();

        loadButton.Click += LoadButton_Click;

        encodingComboBox.SelectedIndexChanged +=
            EncodingComboBox_SelectedIndexChanged;

        filterColumnComboBox.SelectedIndexChanged +=
            FilterColumnComboBox_SelectedIndexChanged;

        filterValueComboBox.SelectedIndexChanged +=
            (_, _) =>
            {
                if (!_isUpdatingInterface)
                    ApplyFilters();
            };

        searchTextBox.TextChanged +=
            (_, _) =>
            {
                if (!_isUpdatingInterface)
                    ApplyFilters();
            };

        resetFiltersButton.Click +=
            ResetFiltersButton_Click;
    }

    private void FillEncodingList()
    {
        encodingComboBox.Items.Clear();

        encodingComboBox.Items.Add(
            new EncodingItem(
                "Автоматически",
                null));

        var encodings = Encoding
            .GetEncodings()
            .OrderBy(info => info.DisplayName);

        foreach (EncodingInfo encodingInfo in encodings)
        {
            Encoding encoding;

            try
            {
                encoding = encodingInfo.GetEncoding();
            }
            catch
            {
                continue;
            }

            string displayName =
                $"{encoding.EncodingName} — " +
                $"{encoding.WebName} " +
                $"({encoding.CodePage})";

            encodingComboBox.Items.Add(
                new EncodingItem(
                    displayName,
                    encoding));
        }

        encodingComboBox.SelectedIndex = 0;
    }

    private void LoadButton_Click(
        object? sender,
        EventArgs e)
    {
        using var openFileDialog = new OpenFileDialog
        {
            Title = "Выберите CSV-файл",
            Filter = "CSV-файлы (*.csv)|*.csv|" +
                     "Все файлы (*.*)|*.*",
            CheckFileExists = true,
            Multiselect = false
        };

        if (openFileDialog.ShowDialog() != DialogResult.OK)
            return;

        _currentFilePath = openFileDialog.FileName;

        LoadCurrentFile(showSuccessMessage: true);
    }

    private void EncodingComboBox_SelectedIndexChanged(
        object? sender,
        EventArgs e)
    {
        if (_currentFilePath is null ||
            _isUpdatingInterface)
        {
            return;
        }

        LoadCurrentFile(showSuccessMessage: false);
    }

    private void LoadCurrentFile(bool showSuccessMessage)
    {
        if (_currentFilePath is null)
            return;

        try
        {
            var encodingItem =
                encodingComboBox.SelectedItem
                as EncodingItem;

            Encoding? selectedEncoding =
                encodingItem?.Encoding;

            // Сначала файл полностью загружается
            // во временную таблицу. Текущие данные
            // не меняются, если загрузка завершится ошибкой.
            DataTable loadedTable = CsvLoader.Load(
                _currentFilePath,
                selectedEncoding);

            _isUpdatingInterface = true;

            try
            {
                _sourceTable = loadedTable;

                // Фильтры предыдущего CSV не должны
                // применяться к новому набору колонок.
                searchTextBox.Clear();

                documentsGrid.DataSource =
                    _sourceTable.DefaultView;

                FillColumnFilter();
                FillStatisticsColumnList();

                fileNameLabel.Text =
                    $"Файл: {Path.GetFileName(_currentFilePath)}";

                Text =
                    $"Анализ технической документации — " +
                    $"{Path.GetFileName(_currentFilePath)}";
            }
            finally
            {
                _isUpdatingInterface = false;
            }

            // Единственный пересчёт после того,
            // как все списки уже обновлены.
            ApplyFilters();

            if (showSuccessMessage)
            {
                MessageBox.Show(
                    $"Файл успешно загружен.\n" +
                    $"Колонок: {_sourceTable.Columns.Count}\n" +
                    $"Записей: {_sourceTable.Rows.Count}\n" +
                    $"Кодировка: {encodingItem}",
                    "Загрузка завершена",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
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
                filterColumnComboBox.Items.Add(
                    column.ColumnName);
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
        if (_isUpdatingInterface)
            return;

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
            filterColumnComboBox.SelectedItem?
                .ToString()
            ?? string.Empty;

        // Дополнительная защита от старого имени колонки.
        if (!_sourceTable.Columns.Contains(columnName))
        {
            filterValueComboBox.Enabled = false;
            filterValueComboBox.SelectedIndex = 0;

            ApplyFilters();
            return;
        }

        var values = _sourceTable.Rows
            .Cast<DataRow>()
            .Select(row =>
                Convert.ToString(row[columnName])
                    ?.Trim()
                ?? string.Empty)
            .Where(value => value.Length > 0)
            .Distinct(
                StringComparer.CurrentCultureIgnoreCase)
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
        if (_isUpdatingInterface ||
            _sourceTable is null)
        {
            return;
        }

        var expressions = new List<string>();

        string searchText =
            searchTextBox.Text.Trim();

        if (searchText.Length > 0)
        {
            string escapedSearch =
                EscapeFilterValue(searchText);

            var columnExpressions =
                _sourceTable.Columns
                    .Cast<DataColumn>()
                    .Select(column =>
                        $"[{EscapeColumnName(column.ColumnName)}] " +
                        $"LIKE '%{escapedSearch}%'");

            expressions.Add(
                $"({string.Join(
                    " OR ",
                    columnExpressions)})");
        }

        string? selectedColumn =
            filterColumnComboBox.SelectedItem?
                .ToString();

        string? selectedValue =
            filterValueComboBox.SelectedItem?
                .ToString();

        if (filterColumnComboBox.SelectedIndex > 0 &&
            filterValueComboBox.SelectedIndex > 0 &&
            !string.IsNullOrWhiteSpace(selectedColumn) &&
            selectedValue is not null &&
            _sourceTable.Columns.Contains(selectedColumn))
        {
            expressions.Add(
                $"[{EscapeColumnName(selectedColumn)}] = " +
                $"'{EscapeFilterValue(selectedValue)}'");
        }

        _sourceTable.DefaultView.RowFilter =
            string.Join(" AND ", expressions);

        UpdateRecordCount();
        UpdateStatistics();
    }

    private void ResetFiltersButton_Click(
        object? sender,
        EventArgs e)
    {
        if (_sourceTable is null)
            return;

        _isUpdatingInterface = true;

        try
        {
            searchTextBox.Clear();

            if (filterColumnComboBox.Items.Count > 0)
            {
                filterColumnComboBox.SelectedIndex = 0;
            }

            filterValueComboBox.Items.Clear();
            filterValueComboBox.Items.Add("Все значения");
            filterValueComboBox.SelectedIndex = 0;
            filterValueComboBox.Enabled = false;
        }
        finally
        {
            _isUpdatingInterface = false;
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

    private static string EscapeFilterValue(
        string value)
    {
        return value.Replace("'", "''");
    }

    private static string EscapeColumnName(
        string columnName)
    {
        return columnName
            .Replace("\\", "\\\\")
            .Replace("]", "\\]");
    }

    private sealed class EncodingItem
    {
        public string DisplayName { get; }

        public Encoding? Encoding { get; }

        public EncodingItem(
            string displayName,
            Encoding? encoding)
        {
            DisplayName = displayName;
            Encoding = encoding;
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}