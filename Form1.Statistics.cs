using System.Data;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace TechnicalDocumentationAnalyzer;

public partial class Form1
{
    private readonly List<ChartItem> _chartItems = [];

    private ChartKind _currentChartKind = ChartKind.None;
    private string _chartTitle = "Диаграмма";
    private string _chartMessage = "Нет данных для отображения";

    private static readonly Color[] PieColors =
    [
        Color.SteelBlue,
        Color.IndianRed,
        Color.MediumSeaGreen,
        Color.Goldenrod,
        Color.MediumPurple,
        Color.CadetBlue,
        Color.Coral,
        Color.SlateGray
    ];

    private void InitializeStatistics()
    {
        FillChartModeList();

        statisticsColumnComboBox.SelectedIndexChanged +=
            (_, _) =>
            {
                if (!_isUpdatingInterface)
                    UpdateStatistics();
            };

        statisticsModeComboBox.SelectedIndexChanged +=
            (_, _) =>
            {
                if (!_isUpdatingInterface)
                    UpdateStatistics();
            };

        statisticsChartPanel.Paint += StatisticsChartPanel_Paint;
        statisticsChartPanel.Resize +=
            (_, _) => statisticsChartPanel.Invalidate();
    }

    private void FillChartModeList()
    {
        statisticsModeComboBox.Items.Clear();
        statisticsModeComboBox.Items.Add(new ChartModeItem("Автоматически", null));
        statisticsModeComboBox.Items.Add(new ChartModeItem("Столбчатая", ChartKind.Bar));
        statisticsModeComboBox.Items.Add(new ChartModeItem("Круговая", ChartKind.Pie));
        statisticsModeComboBox.Items.Add(new ChartModeItem("Линейная", ChartKind.Line));
        statisticsModeComboBox.Items.Add(new ChartModeItem("Гистограмма", ChartKind.Histogram));
        statisticsModeComboBox.Items.Add(new ChartModeItem("Без графика", ChartKind.None));
        statisticsModeComboBox.SelectedIndex = 0;
    }

    private void FillStatisticsColumnList()
    {
        statisticsColumnComboBox.Items.Clear();

        // Новый CSV может иметь совершенно другую структуру.
        // Поэтому не переносим на него вручную выбранный вид графика.
        if (statisticsModeComboBox.Items.Count > 0)
            statisticsModeComboBox.SelectedIndex = 0;

        if (_sourceTable is null)
        {
            ResetStatistics();
            return;
        }

        foreach (DataColumn column in _sourceTable.Columns)
            statisticsColumnComboBox.Items.Add(column.ColumnName);

        if (statisticsColumnComboBox.Items.Count > 0)
            statisticsColumnComboBox.SelectedIndex = 0;
        else
            ResetStatistics();
    }

    private void UpdateStatistics()
    {
        UpdateStatisticsFilterLabel();

        if (_sourceTable is null ||
            statisticsColumnComboBox.SelectedItem is null)
        {
            ResetStatistics();
            return;
        }

        string columnName = statisticsColumnComboBox.SelectedItem.ToString()!;

        List<string> values = _sourceTable.DefaultView
            .Cast<DataRowView>()
            .Select(row => Convert.ToString(row[columnName])?.Trim() ?? string.Empty)
            .ToList();

        List<string> filledValues = values
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .ToList();

        int totalCount = values.Count;
        int emptyCount = totalCount - filledValues.Count;
        int uniqueCount = filledValues
            .Distinct(StringComparer.CurrentCultureIgnoreCase)
            .Count();

        statisticsTotalLabel.Text = $"Всего записей: {totalCount}";
        statisticsUniqueLabel.Text = $"Уникальных значений: {uniqueCount}";
        statisticsEmptyLabel.Text = $"Пустых значений: {emptyCount}";

        DataKind dataKind = DetectDataKind(filledValues);

        switch (dataKind)
        {
            case DataKind.Category:
                ShowCategoryStatistics(filledValues);
                break;
            case DataKind.Date:
                ShowDateStatistics(filledValues);
                break;
            case DataKind.Number:
                ShowNumberStatistics(filledValues);
                break;
            case DataKind.Identifier:
                ShowIdentifierStatistics(filledValues, uniqueCount);
                break;
            case DataKind.Text:
                ShowTextStatistics(filledValues);
                break;
        }

        ChartKind chartKind = ResolveChartKind(dataKind);
        BuildChart(chartKind, dataKind, columnName, filledValues);

        bool automaticChart =
            statisticsModeComboBox.SelectedItem is ChartModeItem mode &&
            !mode.ChartKind.HasValue;

        statisticsDetailsLabel.Text =
            $"Тип данных: {GetDataKindName(dataKind)}; " +
            $"график: {GetChartKindName(_currentChartKind)}" +
            (automaticChart ? " — выбран автоматически" : " — выбран вручную");

        statisticsChartPanel.Invalidate();
    }

    private DataKind DetectDataKind(List<string> values)
    {
        if (values.Count == 0)
            return DataKind.Text;

        if (values.All(value => TryParseDate(value, out _)))
            return DataKind.Date;

        if (values.All(value => TryParseNumber(value, out _)))
            return DataKind.Number;

        int uniqueCount = values
            .Distinct(StringComparer.CurrentCultureIgnoreCase)
            .Count();

        double uniqueShare = (double)uniqueCount / values.Count;
        double averageLength = values.Average(value => value.Length);

        if (uniqueShare >= 0.8)
            return averageLength >= 25 ? DataKind.Text : DataKind.Identifier;

        return DataKind.Category;
    }

    private ChartKind ResolveChartKind(DataKind dataKind)
    {
        if (statisticsModeComboBox.SelectedItem is ChartModeItem selectedMode &&
            selectedMode.ChartKind.HasValue)
        {
            return selectedMode.ChartKind.Value;
        }

        return dataKind switch
        {
            DataKind.Category => ChartKind.Bar,
            DataKind.Date => ChartKind.Line,
            DataKind.Number => ChartKind.Histogram,
            DataKind.Text => ChartKind.Histogram,
            DataKind.Identifier => ChartKind.None,
            _ => ChartKind.None
        };
    }

    private void ShowCategoryStatistics(List<string> values)
    {
        List<ChartItem> items = BuildCategoryItems(values);
        SetDistributionTable(items);
    }

    private void ShowDateStatistics(List<string> values)
    {
        List<DateTime> dates = values
            .Select(value => TryParseDate(value, out DateTime date)
                ? date.Date
                : (DateTime?)null)
            .Where(date => date.HasValue)
            .Select(date => date!.Value)
            .OrderBy(date => date)
            .ToList();

        if (dates.Count == 0)
        {
            SetSummaryTable(("Корректных дат", "0"));
            return;
        }

        DateTime minimumDate = dates.Min();
        DateTime maximumDate = dates.Max();

        SetSummaryTable(
            ("Корректных дат", dates.Count.ToString()),
            ("Уникальных дат", dates.Distinct().Count().ToString()),
            ("Самая ранняя", minimumDate.ToString("dd.MM.yyyy")),
            ("Самая поздняя", maximumDate.ToString("dd.MM.yyyy")),
            ("Период", $"{(maximumDate - minimumDate).Days} дн."));
    }

    private void ShowNumberStatistics(List<string> values)
    {
        List<double> numbers = values
            .Select(value => TryParseNumber(value, out double number)
                ? number
                : (double?)null)
            .Where(number => number.HasValue)
            .Select(number => number!.Value)
            .OrderBy(number => number)
            .ToList();

        if (numbers.Count == 0)
        {
            SetSummaryTable(("Корректных чисел", "0"));
            return;
        }

        double median = numbers.Count % 2 == 1
            ? numbers[numbers.Count / 2]
            : (numbers[numbers.Count / 2 - 1] + numbers[numbers.Count / 2]) / 2;

        SetSummaryTable(
            ("Корректных чисел", numbers.Count.ToString()),
            ("Минимум", FormatNumber(numbers.Min())),
            ("Максимум", FormatNumber(numbers.Max())),
            ("Среднее", FormatNumber(numbers.Average())),
            ("Медиана", FormatNumber(median)),
            ("Сумма", FormatNumber(numbers.Sum())));
    }

    private void ShowIdentifierStatistics(List<string> values, int uniqueCount)
    {
        int duplicateCount = values.Count - uniqueCount;

        SetSummaryTable(
            ("Заполненных значений", values.Count.ToString()),
            ("Уникальных значений", uniqueCount.ToString()),
            ("Повторяющихся записей", duplicateCount.ToString()),
            ("Доля уникальных", values.Count == 0
                ? "0,0 %"
                : ((double)uniqueCount / values.Count).ToString("P1")));
    }

    private void ShowTextStatistics(List<string> values)
    {
        if (values.Count == 0)
        {
            SetSummaryTable(("Заполненных значений", "0"));
            return;
        }

        List<int> lengths = values
            .Select(value => value.Length)
            .OrderBy(length => length)
            .ToList();

        double median = lengths.Count % 2 == 1
            ? lengths[lengths.Count / 2]
            : (lengths[lengths.Count / 2 - 1] + lengths[lengths.Count / 2]) / 2.0;

        SetSummaryTable(
            ("Заполненных значений", values.Count.ToString()),
            ("Минимальная длина", $"{lengths.Min()} симв."),
            ("Максимальная длина", $"{lengths.Max()} симв."),
            ("Средняя длина", $"{lengths.Average():0.0} симв."),
            ("Медианная длина", $"{median:0.0} симв."));
    }

    private void BuildChart(
        ChartKind chartKind,
        DataKind dataKind,
        string columnName,
        List<string> values)
    {
        _chartItems.Clear();
        _currentChartKind = chartKind;
        _chartTitle = $"Колонка «{columnName}»";
        _chartMessage = "Нет данных для отображения";

        if (chartKind == ChartKind.None)
        {
            SetNoChart(dataKind == DataKind.Identifier
                ? "Диаграмма не построена: значения являются уникальными"
                : "Отображение графика отключено");
            return;
        }

        switch (chartKind)
        {
            case ChartKind.Bar:
            case ChartKind.Pie:
                if (dataKind != DataKind.Category)
                {
                    SetNoChart(
                        chartKind == ChartKind.Bar
                            ? "Столбчатая диаграмма доступна для категорий"
                            : "Круговая диаграмма доступна для категорий");
                    return;
                }

                _chartItems.AddRange(BuildCategoryItems(values));
                _chartTitle = chartKind == ChartKind.Bar
                    ? $"Распределение по колонке «{columnName}»"
                    : $"Доли значений колонки «{columnName}»";
                break;

            case ChartKind.Line:
                if (dataKind != DataKind.Date)
                {
                    SetNoChart("Линейная диаграмма доступна только для дат");
                    return;
                }

                _chartItems.AddRange(BuildDateItems(values));
                _chartTitle = $"Изменение количества записей по времени — «{columnName}»";
                break;

            case ChartKind.Histogram:
                if (dataKind == DataKind.Number)
                {
                    List<double> numbers = values
                        .Select(value => TryParseNumber(value, out double number)
                            ? number
                            : (double?)null)
                        .Where(number => number.HasValue)
                        .Select(number => number!.Value)
                        .ToList();

                    _chartItems.AddRange(BuildHistogramItems(numbers));
                    _chartTitle = $"Распределение чисел — «{columnName}»";
                }
                else if (dataKind is DataKind.Text or DataKind.Identifier)
                {
                    _chartItems.AddRange(
                        BuildHistogramItems(values.Select(value => (double)value.Length).ToList()));
                    _chartTitle = $"Распределение длины текста — «{columnName}»";
                }
                else
                {
                    SetNoChart("Гистограмма доступна для чисел и длины текста");
                    return;
                }
                break;
        }

        if (_chartItems.Count == 0)
            SetNoChart("Нет заполненных значений для отображения");
    }

    private void SetNoChart(string message)
    {
        _currentChartKind = ChartKind.None;
        _chartItems.Clear();
        _chartMessage = message;
    }

    private static List<ChartItem> BuildCategoryItems(List<string> values)
    {
        return values
            .GroupBy(value => value, StringComparer.CurrentCultureIgnoreCase)
            .Select(group => new ChartItem
            {
                Label = group.Key,
                Count = group.Count(),
                Share = values.Count == 0 ? 0 : (double)group.Count() / values.Count
            })
            .OrderByDescending(item => item.Count)
            .ThenBy(item => item.Label)
            .ToList();
    }

    private static List<ChartItem> BuildDateItems(List<string> values)
    {
        List<DateTime> dates = values
            .Select(value => TryParseDate(value, out DateTime date)
                ? date.Date
                : (DateTime?)null)
            .Where(date => date.HasValue)
            .Select(date => date!.Value)
            .OrderBy(date => date)
            .ToList();

        if (dates.Count == 0)
            return [];

        int periodDays = (dates.Max() - dates.Min()).Days;

        if (periodDays <= 45)
        {
            return dates
                .GroupBy(date => date.Date)
                .OrderBy(group => group.Key)
                .Select(group => new ChartItem
                {
                    Label = group.Key.ToString("dd.MM"),
                    Count = group.Count(),
                    Share = (double)group.Count() / dates.Count
                })
                .ToList();
        }

        if (periodDays <= 730)
        {
            return dates
                .GroupBy(date => new DateTime(date.Year, date.Month, 1))
                .OrderBy(group => group.Key)
                .Select(group => new ChartItem
                {
                    Label = group.Key.ToString("MM.yyyy"),
                    Count = group.Count(),
                    Share = (double)group.Count() / dates.Count
                })
                .ToList();
        }

        return dates
            .GroupBy(date => date.Year)
            .OrderBy(group => group.Key)
            .Select(group => new ChartItem
            {
                Label = group.Key.ToString(),
                Count = group.Count(),
                Share = (double)group.Count() / dates.Count
            })
            .ToList();
    }

    private static List<ChartItem> BuildHistogramItems(List<double> values)
    {
        if (values.Count == 0)
            return [];

        double minimum = values.Min();
        double maximum = values.Max();

        if (Math.Abs(maximum - minimum) < double.Epsilon)
        {
            return
            [
                new ChartItem
                {
                    Label = FormatNumber(minimum),
                    Count = values.Count,
                    Share = 1
                }
            ];
        }

        int binCount = Math.Min(10, Math.Max(3, (int)Math.Ceiling(Math.Sqrt(values.Count))));
        double binWidth = (maximum - minimum) / binCount;
        int[] counts = new int[binCount];

        foreach (double value in values)
        {
            int index = Math.Min(
                binCount - 1,
                (int)((value - minimum) / binWidth));
            counts[index]++;
        }

        var items = new List<ChartItem>();

        for (int i = 0; i < binCount; i++)
        {
            double start = minimum + i * binWidth;
            double end = i == binCount - 1 ? maximum : start + binWidth;
            items.Add(new ChartItem
            {
                Label = $"{FormatNumber(start)}–{FormatNumber(end)}",
                Count = counts[i],
                Share = (double)counts[i] / values.Count
            });
        }

        return items;
    }

    private void SetDistributionTable(List<ChartItem> items)
    {
        var table = new DataTable();
        table.Columns.Add("Значение", typeof(string));
        table.Columns.Add("Количество", typeof(int));
        table.Columns.Add("Доля", typeof(string));

        foreach (ChartItem item in items)
            table.Rows.Add(item.Label, item.Count, item.Share.ToString("P1"));

        statisticsGrid.DataSource = table;
        statisticsGrid.ClearSelection();
    }

    private void SetSummaryTable(params (string Name, string Value)[] rows)
    {
        var table = new DataTable();
        table.Columns.Add("Показатель", typeof(string));
        table.Columns.Add("Значение", typeof(string));

        foreach ((string name, string value) in rows)
            table.Rows.Add(name, value);

        statisticsGrid.DataSource = table;

        if (statisticsGrid.Columns["Показатель"] is DataGridViewColumn indicatorColumn)
        {
            indicatorColumn.DisplayIndex = 0;
            indicatorColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        if (statisticsGrid.Columns["Значение"] is DataGridViewColumn valueColumn)
        {
            valueColumn.DisplayIndex = 1;
            valueColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        statisticsGrid.ClearSelection();
    }

    private void UpdateStatisticsFilterLabel()
    {
        var activeFilters = new List<string>();
        string searchText = searchTextBox.Text.Trim();

        if (searchText.Length > 0)
            activeFilters.Add($"поиск «{searchText}»");

        if (filterColumnComboBox.SelectedIndex > 0 &&
            filterValueComboBox.SelectedIndex > 0)
        {
            string columnName = filterColumnComboBox.SelectedItem?.ToString() ?? string.Empty;
            string selectedValue = filterValueComboBox.SelectedItem?.ToString() ?? string.Empty;
            activeFilters.Add($"{columnName} = «{selectedValue}»");
        }

        statisticsFilterLabel.Text = activeFilters.Count == 0
            ? "Активный фильтр: отсутствует"
            : $"Активный фильтр: {string.Join("; ", activeFilters)}";
    }

    private void ResetStatistics()
    {
        _chartItems.Clear();
        _currentChartKind = ChartKind.None;
        _chartTitle = "Диаграмма";
        _chartMessage = "Нет данных для отображения";
        statisticsGrid.DataSource = null;
        statisticsTotalLabel.Text = "Всего записей: 0";
        statisticsUniqueLabel.Text = "Уникальных значений: 0";
        statisticsEmptyLabel.Text = "Пустых значений: 0";
        statisticsFilterLabel.Text = "Активный фильтр: отсутствует";
        statisticsDetailsLabel.Text = "Тип данных: —";
        statisticsChartPanel.Invalidate();
    }

    private void StatisticsChartPanel_Paint(object? sender, PaintEventArgs e)
    {
        Graphics graphics = e.Graphics;
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.Clear(Color.White);

        if (_currentChartKind == ChartKind.None || _chartItems.Count == 0)
        {
            DrawCenteredMessage(graphics, _chartMessage);
            return;
        }

        switch (_currentChartKind)
        {
            case ChartKind.Bar:
                DrawBarChart(graphics);
                break;
            case ChartKind.Pie:
                DrawPieChart(graphics);
                break;
            case ChartKind.Line:
                DrawLineChart(graphics);
                break;
            case ChartKind.Histogram:
                DrawHistogramChart(graphics);
                break;
        }
    }

    private void DrawBarChart(Graphics graphics)
    {
        List<ChartItem> items = _chartItems.Take(15).ToList();
        int width = statisticsChartPanel.ClientSize.Width;
        int height = statisticsChartPanel.ClientSize.Height;
        int labelWidth = Math.Min(220, Math.Max(120, width / 3));
        const int top = 50;
        const int right = 100;
        float plotWidth = Math.Max(1, width - labelWidth - right);
        float rowHeight = Math.Min(55, Math.Max(1, height - top - 15) / (float)items.Count);
        float barHeight = Math.Min(30, Math.Max(10, rowHeight * 0.6f));
        int maximum = Math.Max(1, items.Max(item => item.Count));

        DrawChartTitle(graphics);

        using var barBrush = new SolidBrush(Color.SteelBlue);
        using var textBrush = new SolidBrush(Color.FromArgb(45, 45, 45));
        using var axisPen = new Pen(Color.LightGray);
        using var format = new StringFormat
        {
            LineAlignment = StringAlignment.Center,
            Trimming = StringTrimming.EllipsisCharacter
        };

        graphics.DrawLine(axisPen, labelWidth, top - 5, labelWidth, top + rowHeight * items.Count);

        for (int i = 0; i < items.Count; i++)
        {
            ChartItem item = items[i];
            float rowY = top + i * rowHeight;
            float barY = rowY + (rowHeight - barHeight) / 2;
            float barWidth = plotWidth * item.Count / maximum;

            graphics.DrawString(item.Label, Font, textBrush,
                new RectangleF(8, rowY, labelWidth - 16, rowHeight), format);
            graphics.FillRectangle(barBrush, labelWidth, barY, barWidth, barHeight);
            graphics.DrawString($"{item.Count} ({item.Share:P1})", Font, textBrush,
                labelWidth + barWidth + 6, barY + 6);
        }
    }

    private void DrawPieChart(Graphics graphics)
    {
        List<ChartItem> sorted = _chartItems
            .OrderByDescending(item => item.Count)
            .ToList();

        List<ChartItem> items = sorted.Take(7).ToList();
        int otherCount = sorted.Skip(7).Sum(item => item.Count);
        int total = sorted.Sum(item => item.Count);

        if (otherCount > 0)
        {
            items.Add(new ChartItem
            {
                Label = "Прочие",
                Count = otherCount,
                Share = (double)otherCount / total
            });
        }

        DrawChartTitle(graphics);

        int width = statisticsChartPanel.ClientSize.Width;
        int height = statisticsChartPanel.ClientSize.Height;
        float diameter = Math.Max(80, Math.Min(height - 90, width * 0.48f));
        var pieRectangle = new RectangleF(20, 55, diameter, diameter);
        float startAngle = -90;

        using var borderPen = new Pen(Color.White, 2);
        using var textBrush = new SolidBrush(Color.FromArgb(45, 45, 45));

        for (int i = 0; i < items.Count; i++)
        {
            ChartItem item = items[i];
            float sweepAngle = total == 0 ? 0 : 360f * item.Count / total;
            using var sliceBrush = new SolidBrush(PieColors[i % PieColors.Length]);
            graphics.FillPie(sliceBrush, pieRectangle, startAngle, sweepAngle);
            graphics.DrawPie(borderPen, pieRectangle, startAngle, sweepAngle);
            startAngle += sweepAngle;

            float legendY = 65 + i * 28;
            graphics.FillRectangle(sliceBrush, diameter + 45, legendY, 16, 16);
            graphics.DrawString(
                $"{item.Label}: {item.Count} ({item.Share:P1})",
                Font,
                textBrush,
                diameter + 68,
                legendY);
        }
    }

    private void DrawLineChart(Graphics graphics)
    {
        DrawChartTitle(graphics);

        int width = statisticsChartPanel.ClientSize.Width;
        int height = statisticsChartPanel.ClientSize.Height;
        const int left = 55;
        const int top = 55;
        const int right = 20;
        const int bottom = 65;
        float plotWidth = Math.Max(1, width - left - right);
        float plotHeight = Math.Max(1, height - top - bottom);
        int maximum = Math.Max(1, _chartItems.Max(item => item.Count));

        using var axisPen = new Pen(Color.Gray);
        using var linePen = new Pen(Color.SteelBlue, 3);
        using var pointBrush = new SolidBrush(Color.SteelBlue);
        using var textBrush = new SolidBrush(Color.FromArgb(45, 45, 45));
        using var centerFormat = new StringFormat { Alignment = StringAlignment.Center };

        graphics.DrawLine(axisPen, left, top, left, top + plotHeight);
        graphics.DrawLine(axisPen, left, top + plotHeight, left + plotWidth, top + plotHeight);
        graphics.DrawString(maximum.ToString(), Font, textBrush, 5, top - 7);
        graphics.DrawString("0", Font, textBrush, 25, top + plotHeight - 8);

        var points = new List<PointF>();
        int labelStep = Math.Max(1, (int)Math.Ceiling(_chartItems.Count / 7.0));

        for (int i = 0; i < _chartItems.Count; i++)
        {
            float x = _chartItems.Count == 1
                ? left + plotWidth / 2
                : left + plotWidth * i / (_chartItems.Count - 1);
            float y = top + plotHeight - plotHeight * _chartItems[i].Count / maximum;
            points.Add(new PointF(x, y));

            if (i % labelStep == 0 || i == _chartItems.Count - 1)
            {
                graphics.DrawString(_chartItems[i].Label, Font, textBrush,
                    new RectangleF(x - 35, top + plotHeight + 8, 70, 35), centerFormat);
            }
        }

        if (points.Count > 1)
            graphics.DrawLines(linePen, points.ToArray());

        foreach (PointF point in points)
            graphics.FillEllipse(pointBrush, point.X - 4, point.Y - 4, 8, 8);
    }

    private void DrawHistogramChart(Graphics graphics)
    {
        DrawChartTitle(graphics);

        int width = statisticsChartPanel.ClientSize.Width;
        int height = statisticsChartPanel.ClientSize.Height;
        const int left = 50;
        const int top = 55;
        const int right = 20;
        const int bottom = 75;
        float plotWidth = Math.Max(1, width - left - right);
        float plotHeight = Math.Max(1, height - top - bottom);
        int maximum = Math.Max(1, _chartItems.Max(item => item.Count));
        float cellWidth = plotWidth / _chartItems.Count;

        using var axisPen = new Pen(Color.Gray);
        using var barBrush = new SolidBrush(Color.SteelBlue);
        using var textBrush = new SolidBrush(Color.FromArgb(45, 45, 45));
        using var centerFormat = new StringFormat
        {
            Alignment = StringAlignment.Center,
            Trimming = StringTrimming.EllipsisCharacter
        };

        graphics.DrawLine(axisPen, left, top, left, top + plotHeight);
        graphics.DrawLine(axisPen, left, top + plotHeight, left + plotWidth, top + plotHeight);

        for (int i = 0; i < _chartItems.Count; i++)
        {
            ChartItem item = _chartItems[i];
            float barHeight = plotHeight * item.Count / maximum;
            float x = left + i * cellWidth + 2;
            float y = top + plotHeight - barHeight;
            float barWidth = Math.Max(1, cellWidth - 4);

            graphics.FillRectangle(barBrush, x, y, barWidth, barHeight);
            graphics.DrawString(item.Count.ToString(), Font, textBrush,
                new RectangleF(x, y - 20, barWidth, 18), centerFormat);
            graphics.DrawString(item.Label, Font, textBrush,
                new RectangleF(x - 5, top + plotHeight + 7, barWidth + 10, 42), centerFormat);
        }
    }

    private void DrawChartTitle(Graphics graphics)
    {
        using var titleFont = new Font(Font, FontStyle.Bold);
        using var brush = new SolidBrush(Color.FromArgb(45, 45, 45));
        using var format = new StringFormat
        {
            Trimming = StringTrimming.EllipsisCharacter,
            FormatFlags = StringFormatFlags.NoWrap
        };

        graphics.DrawString(_chartTitle, titleFont, brush,
            new RectangleF(10, 15,
                Math.Max(1, statisticsChartPanel.ClientSize.Width - 20), 25),
            format);
    }

    private void DrawCenteredMessage(Graphics graphics, string message)
    {
        SizeF size = graphics.MeasureString(message, Font);
        graphics.DrawString(message, Font, Brushes.Gray,
            (statisticsChartPanel.ClientSize.Width - size.Width) / 2,
            (statisticsChartPanel.ClientSize.Height - size.Height) / 2);
    }

    private static bool TryParseDate(string value, out DateTime result)
    {
        result = default;

        if (!value.Contains('-') && !value.Contains('.') && !value.Contains('/'))
            return false;

        return DateTime.TryParse(
                   value,
                   CultureInfo.CurrentCulture,
                   DateTimeStyles.AllowWhiteSpaces,
                   out result)
               ||
               DateTime.TryParse(
                   value,
                   CultureInfo.InvariantCulture,
                   DateTimeStyles.AllowWhiteSpaces,
                   out result);
    }

    private static bool TryParseNumber(string value, out double result)
    {
        return double.TryParse(
                   value,
                   NumberStyles.Any,
                   CultureInfo.CurrentCulture,
                   out result)
               ||
               double.TryParse(
                   value,
                   NumberStyles.Any,
                   CultureInfo.InvariantCulture,
                   out result);
    }

    private static string FormatNumber(double number) => number.ToString("0.##");

    private static string GetDataKindName(DataKind kind) => kind switch
    {
        DataKind.Category => "категория",
        DataKind.Date => "дата",
        DataKind.Number => "число",
        DataKind.Identifier => "идентификатор",
        DataKind.Text => "текст",
        _ => "неизвестно"
    };

    private static string GetChartKindName(ChartKind kind) => kind switch
    {
        ChartKind.Bar => "столбчатая диаграмма",
        ChartKind.Pie => "круговая диаграмма",
        ChartKind.Line => "линейная диаграмма",
        ChartKind.Histogram => "гистограмма",
        ChartKind.None => "без графика",
        _ => "без графика"
    };

    private enum DataKind
    {
        Category,
        Date,
        Number,
        Identifier,
        Text
    }

    private enum ChartKind
    {
        None,
        Bar,
        Pie,
        Line,
        Histogram
    }

    private sealed class ChartModeItem
    {
        public string DisplayName { get; }
        public ChartKind? ChartKind { get; }

        public ChartModeItem(string displayName, ChartKind? chartKind)
        {
            DisplayName = displayName;
            ChartKind = chartKind;
        }

        public override string ToString() => DisplayName;
    }

    private sealed class ChartItem
    {
        public string Label { get; init; } = string.Empty;
        public int Count { get; init; }
        public double Share { get; init; }
    }
}