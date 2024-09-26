using ExcelDataReader;
using System.Data;
using System.IO;
using System.Text;

namespace WPF.Extensions;
public static class ExcelExtension
{
    public static DataTable ReadExcelFile(string filePath)
    {
        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var encoding = Encoding.GetEncoding("UTF-8");
            using (var reader = ExcelReaderFactory.CreateReader(stream,
              new ExcelReaderConfiguration() { FallbackEncoding = encoding }))
            {
                var result = reader.AsDataSet(new ExcelDataSetConfiguration
                {
                    ConfigureDataTable = _ => new ExcelDataTableConfiguration { UseHeaderRow = true }
                });

                if (result.Tables.Count > 0)
                {
                    return result.Tables[0];
                }
            }
        }
        return null;
    }
}
