using Main.Domain.SeedWork;

namespace Main.Dto.ViewModel.CustomTemplate;

public class ErrorExcelData(int rowNumber, Enumeration column, ErrorType type, ViewCustomTemplateExcelData data)
{
    public int RowNumber { get; set; } = rowNumber;

    public Enumeration Column { get; set; } = column;

    public ErrorType Type { get; set; } = type;

    public ViewCustomTemplateExcelData Data { get; set; } = data;
}
