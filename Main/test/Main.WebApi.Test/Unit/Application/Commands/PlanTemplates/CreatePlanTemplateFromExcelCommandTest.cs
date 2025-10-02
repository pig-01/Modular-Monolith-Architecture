using System.Text.Json;
using Main.Dto.ViewModel.PlanTemplate;
using Main.WebApi.Application.Commands.PlanTemplates;
using MediatR;
using Xunit;

namespace Main.WebApi.Test.Unit.Application.Commands.PlanTemplates;

/// <summary>
/// CreatePlanTemplateFromExcelCommand 單元測試
/// 測試從 Excel 資料建立計劃模板命令的各種場景
/// </summary>
public class CreatePlanTemplateFromExcelCommandTest
{
    #region Constructor Tests

    [Fact(DisplayName = "建構子_有效參數_應成功建立命令")]
    public void Constructor_ValidParameters_ShouldCreateCommandSuccessfully()
    {
        // Arrange
        const string expectedVersion = "1.0.0";
        List<ViewPlanTemplateExcelData> expectedDataList =
        [
            new()
            {
                GroupName = "環境",
                PlanTemplateName = "溫室氣體排放",
                FormName = "溫室氣體排放表單",
                IsDeploy = true,
                PlanTemplateDetailTitle = "範圍一排放",
                RequestUnitIds = "Demo001,Demo002",
                GriRuleCodes = "305-1,305-2",
                ExposeIndustryIds = "IND001,IND002"
            }
        ];

        // Act
        CreatePlanTemplateFromExcelCommand command = new()
        {
            Version = expectedVersion,
            DataList = expectedDataList
        };

        // Assert
        Assert.NotNull(command);
        Assert.Equal(expectedVersion, command.Version);
        Assert.Equal(expectedDataList, command.DataList);
        Assert.Single(command.DataList);
        Assert.IsAssignableFrom<IRequest<bool>>(command);
    }

    #endregion

    #region Property Tests

    [Fact(DisplayName = "Version屬性_設定有效值_應正確儲存")]
    public void Version_SetValidValue_ShouldStoreCorrectly()
    {
        // Arrange
        CreatePlanTemplateFromExcelCommand command = new()
        {
            Version = "test",
            DataList = []
        };
        const string expectedVersion = "2.1.0";

        // Act
        command.Version = expectedVersion;

        // Assert
        Assert.Equal(expectedVersion, command.Version);
    }

    [Fact(DisplayName = "DataList屬性_設定有效清單_應正確儲存")]
    public void DataList_SetValidList_ShouldStoreCorrectly()
    {
        // Arrange
        CreatePlanTemplateFromExcelCommand command = new()
        {
            Version = "test",
            DataList = []
        };
        List<ViewPlanTemplateExcelData> expectedDataList =
        [
            new() { GroupName = "環境", PlanTemplateName = "測試模板1" },
            new() { GroupName = "社會", PlanTemplateName = "測試模板2" }
        ];

        // Act
        command.DataList = expectedDataList;

        // Assert
        Assert.Equal(expectedDataList, command.DataList);
        Assert.Equal(2, command.DataList.Count);
    }

    [Theory(DisplayName = "Version屬性_不同版本格式_應正確處理")]
    [InlineData("1.0")]
    [InlineData("1.0.0")]
    [InlineData("2.1.3-alpha")]
    [InlineData("v1.0.0")]
    [InlineData("2023.12.01")]
    public void Version_DifferentFormats_ShouldHandleCorrectly(string version)
    {
        // Arrange & Act
        CreatePlanTemplateFromExcelCommand command = new()
        {
            Version = version,
            DataList = []
        };

        // Assert
        Assert.Equal(version, command.Version);
    }

    #endregion

    #region JSON Serialization Tests

    [Fact(DisplayName = "JSON序列化_包含所有資料_應正確序列化")]
    public void JsonSerialization_WithAllData_ShouldSerializeCorrectly()
    {
        // Arrange
        CreatePlanTemplateFromExcelCommand command = new()
        {
            Version = "1.0.0",
            DataList =
            [
                new()
                {
                    GroupName = "環境",
                    PlanTemplateName = "溫室氣體排放",
                    FormName = "溫室氣體排放表單",
                    IsDeploy = true,
                    PlanTemplateDetailTitle = "範圍一排放",
                    RequestUnitIds = "Demo001,Demo002",
                    GriRuleCodes = "305-1,305-2",
                    ExposeIndustryIds = "IND001,IND002"
                }
            ]
        };

        // Act
        string json = JsonSerializer.Serialize(command);
        CreatePlanTemplateFromExcelCommand? deserializedCommand = JsonSerializer.Deserialize<CreatePlanTemplateFromExcelCommand>(json);

        // Assert
        Assert.NotNull(deserializedCommand);
        Assert.Equal(command.Version, deserializedCommand.Version);
        Assert.Equal(command.DataList.Count, deserializedCommand.DataList.Count);

        ViewPlanTemplateExcelData originalData = command.DataList.First();
        ViewPlanTemplateExcelData deserializedData = deserializedCommand.DataList.First();
        Assert.Equal(originalData.GroupName, deserializedData.GroupName);
        Assert.Equal(originalData.PlanTemplateName, deserializedData.PlanTemplateName);
        Assert.Equal(originalData.FormName, deserializedData.FormName);
        Assert.Equal(originalData.IsDeploy, deserializedData.IsDeploy);
    }

    [Fact(DisplayName = "JSON序列化_DataList屬性名稱_應使用dataList")]
    public void JsonSerialization_DataListPropertyName_ShouldUseDataList()
    {
        // Arrange
        CreatePlanTemplateFromExcelCommand command = new()
        {
            Version = "1.0.0",
            DataList =
            [
                new() { GroupName = "測試", PlanTemplateName = "測試模板" }
            ]
        };

        // Act
        string json = JsonSerializer.Serialize(command);

        // Assert
        Assert.Contains("\"dataList\":", json);
        Assert.DoesNotContain("\"DataList\":", json);
    }

    [Fact(DisplayName = "JSON反序列化_使用dataList屬性名稱_應正確反序列化")]
    public void JsonDeserialization_WithDataListPropertyName_ShouldDeserializeCorrectly()
    {
        // Arrange
        const string json = """
        {
            "Version": "1.0.0",
            "dataList": [
                {
                    "groupName": "環境",
                    "planTemplateName": "測試模板"
                }
            ]
        }
        """;

        // Act
        CreatePlanTemplateFromExcelCommand? command = JsonSerializer.Deserialize<CreatePlanTemplateFromExcelCommand>(json);

        // Assert
        Assert.NotNull(command);
        Assert.Equal("1.0.0", command.Version);
        Assert.Single(command.DataList);
        Assert.Equal("環境", command.DataList.First().GroupName);
        Assert.Equal("測試模板", command.DataList.First().PlanTemplateName);
    }

    #endregion

    #region Validation Tests

    [Fact(DisplayName = "驗證_空白資料清單_應允許建立")]
    public void Validation_EmptyDataList_ShouldAllowCreation()
    {
        // Arrange & Act
        CreatePlanTemplateFromExcelCommand command = new()
        {
            Version = "1.0.0",
            DataList = []
        };

        // Assert
        Assert.NotNull(command);
        Assert.Empty(command.DataList);
    }

    [Theory(DisplayName = "驗證_大量資料_應正確處理")]
    [InlineData(100)]
    [InlineData(1000)]
    [InlineData(5000)]
    public void Validation_LargeDataSet_ShouldHandleCorrectly(int dataCount)
    {
        // Arrange
        List<ViewPlanTemplateExcelData> dataList = [];
        for (int i = 0; i < dataCount; i++)
        {
            dataList.Add(new ViewPlanTemplateExcelData
            {
                GroupName = $"Group{i}",
                PlanTemplateName = $"Template{i}",
                FormName = $"Form{i}",
                IsDeploy = i % 2 == 0
            });
        }

        // Act
        CreatePlanTemplateFromExcelCommand command = new()
        {
            Version = "1.0.0",
            DataList = dataList
        };

        // Assert
        Assert.NotNull(command);
        Assert.Equal(dataCount, command.DataList.Count);
    }

    #endregion

    #region Edge Cases Tests

    [Fact(DisplayName = "邊界測試_特殊字元版本_應正確處理")]
    public void EdgeCaseSpecialCharactersInVersionShouldHandleCorrectly()
    {
        // Arrange
        const string specialVersion = "1.0.0-β+測試.2023-12-01";

        // Act
        CreatePlanTemplateFromExcelCommand command = new()
        {
            Version = specialVersion,
            DataList = []
        };

        // Assert
        Assert.Equal(specialVersion, command.Version);
    }

    [Fact(DisplayName = "邊界測試_包含Null值的資料_應正確處理")]
    public void EdgeCaseDataWithNullValuesShouldHandleCorrectly()
    {
        // Arrange
        List<ViewPlanTemplateExcelData> dataList =
        [
            new()
            {
                GroupName = null,
                PlanTemplateName = null,
                FormName = null,
                IsDeploy = null,
                PlanTemplateDetailTitle = null,
                RequestUnitIds = null,
                GriRuleCodes = null,
                ExposeIndustryIds = null
            }
        ];

        // Act
        CreatePlanTemplateFromExcelCommand command = new()
        {
            Version = "1.0.0",
            DataList = dataList
        };

        // Assert
        Assert.NotNull(command);
        Assert.Single(command.DataList);
        ViewPlanTemplateExcelData data = command.DataList.First();
        Assert.Null(data.GroupName);
        Assert.Null(data.PlanTemplateName);
        Assert.Null(data.FormName);
        Assert.Null(data.IsDeploy);
    }

    [Fact(DisplayName = "邊界測試_重複資料項目_應正確處理")]
    public void EdgeCaseDuplicateDataItemsShouldHandleCorrectly()
    {
        // Arrange
        ViewPlanTemplateExcelData duplicateData = new()
        {
            GroupName = "環境",
            PlanTemplateName = "重複模板",
            FormName = "溫室氣體排放表單",
            IsDeploy = true
        };

        List<ViewPlanTemplateExcelData> dataList = [duplicateData, duplicateData];

        // Act
        CreatePlanTemplateFromExcelCommand command = new()
        {
            Version = "1.0.0",
            DataList = dataList
        };

        // Assert
        Assert.NotNull(command);
        Assert.Equal(2, command.DataList.Count);
        Assert.All(command.DataList, item =>
        {
            Assert.Equal("環境", item.GroupName);
            Assert.Equal("重複模板", item.PlanTemplateName);
        });
    }

    #endregion

    #region Interface Implementation Tests

    [Fact(DisplayName = "介面實作_IRequest泛型_應回傳布林值")]
    public void InterfaceImplementationIRequestGenericShouldReturnBoolean()
    {
        // Arrange
        CreatePlanTemplateFromExcelCommand command = new()
        {
            Version = "1.0.0",
            DataList = []
        };

        // Act & Assert
        Assert.IsAssignableFrom<IRequest<bool>>(command);

        // 驗證泛型參數
        Type requestType = typeof(IRequest<bool>);
        Assert.True(requestType.IsAssignableFrom(command.GetType()));
    }

    #endregion
}