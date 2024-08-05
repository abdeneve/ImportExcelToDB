using Domain.Extensions;

namespace UnitTest;

public class StringExtensionTest
{
    public static TheoryData<string, string> ExtractDateTestData => new()
    {
        { "file_12_03_2024.txt", "12-03-2024" },
        { "file-25-11-2023.doc", "25-11-2023" },
        { "file_without_date.pdf", string.Empty },
        { "file_with_invalid_date_format.txt", string.Empty },
        { "file_with_date_at_end_2024-04-15.csv", string.Empty },
    };

    [Theory]
    [MemberData(nameof(ExtractDateTestData))]
    public void ExtractDate_ShouldReturnCorrectDate(string fileName, string expectedDate)
    {
        // Arrange
        // No additional setup needed

        // Act
        var actualDate = fileName.ExtractDate();

        // Assert
        Assert.Equal(expectedDate, actualDate);
    }
}