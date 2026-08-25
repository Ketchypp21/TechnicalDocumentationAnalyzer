using System.Data;
using System.Text;
using Microsoft.VisualBasic.FileIO;

namespace TechnicalDocumentationAnalyzer.Services;

public static class CsvLoader
{
    private static readonly char[] PossibleDelimiters = { ';', ',', '\t' };

    public static DataTable Load(string filePath)
    {
        char delimiter = DetectDelimiter(filePath);

        using var parser = new TextFieldParser(
            filePath,
            Encoding.UTF8,
            detectEncoding: true);

        parser.TextFieldType = FieldType.Delimited;
        parser.SetDelimiters(delimiter.ToString());
        parser.HasFieldsEnclosedInQuotes = true;
        parser.TrimWhiteSpace = true;

        if (parser.EndOfData)
            throw new InvalidDataException("Выбранный CSV-файл пуст.");

        string[] headers = parser.ReadFields()
            ?? throw new InvalidDataException("Не удалось прочитать заголовки CSV.");

        var table = new DataTable();
        var usedColumnNames = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase);

        for (int i = 0; i < headers.Length; i++)
        {
            string columnName = CreateUniqueColumnName(
                headers[i],
                i,
                usedColumnNames);

            table.Columns.Add(columnName, typeof(string));
        }

        while (!parser.EndOfData)
        {
            string[]? fields = parser.ReadFields();

            if (fields is null || fields.All(string.IsNullOrWhiteSpace))
                continue;

            if (fields.Length > table.Columns.Count)
            {
                throw new InvalidDataException(
                    $"В строке обнаружено {fields.Length} значений, " +
                    $"но заголовков только {table.Columns.Count}.");
            }

            DataRow row = table.NewRow();

            for (int i = 0; i < table.Columns.Count; i++)
            {
                row[i] = i < fields.Length
                    ? fields[i]
                    : string.Empty;
            }

            table.Rows.Add(row);
        }

        return table;
    }

    private static char DetectDelimiter(string filePath)
    {
        using var reader = new StreamReader(
            filePath,
            Encoding.UTF8,
            detectEncodingFromByteOrderMarks: true);

        string? firstLine;

        do
        {
            firstLine = reader.ReadLine();
        }
        while (firstLine is not null &&
               string.IsNullOrWhiteSpace(firstLine));

        if (firstLine is null)
            throw new InvalidDataException("Выбранный CSV-файл пуст.");

        char selectedDelimiter = PossibleDelimiters
            .OrderByDescending(delimiter =>
                CountDelimiterOutsideQuotes(firstLine, delimiter))
            .First();

        int delimiterCount =
            CountDelimiterOutsideQuotes(firstLine, selectedDelimiter);

        if (delimiterCount == 0)
        {
            throw new InvalidDataException(
                "Не удалось определить разделитель CSV.");
        }

        return selectedDelimiter;
    }

    private static int CountDelimiterOutsideQuotes(
        string line,
        char delimiter)
    {
        bool insideQuotes = false;
        int count = 0;

        foreach (char character in line)
        {
            if (character == '"')
            {
                insideQuotes = !insideQuotes;
            }
            else if (character == delimiter && !insideQuotes)
            {
                count++;
            }
        }

        return count;
    }

    private static string CreateUniqueColumnName(
        string originalName,
        int columnIndex,
        HashSet<string> usedNames)
    {
        string baseName = string.IsNullOrWhiteSpace(originalName)
            ? $"Колонка {columnIndex + 1}"
            : originalName.Trim();

        string result = baseName;
        int duplicateNumber = 2;

        while (!usedNames.Add(result))
        {
            result = $"{baseName} ({duplicateNumber})";
            duplicateNumber++;
        }

        return result;
    }
}