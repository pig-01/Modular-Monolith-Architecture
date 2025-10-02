using System.Text.Json.Serialization;
using Base.Infrastructure.Toolkits.Extensions;

namespace Base.Infrastructure.Toolkits.Test.Extensions;

public class StringExtensionTests
{


  public StringExtensionTests()
  {

  }

  public class TestClass
  {

    [JsonPropertyName("name")]
    public string? Name { get; set; }
  }
  public class Form
  {
    public Form() { }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("id")]
    public long? Id { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("displayName")]
    public required string DisplayName { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("description")]
    public required string Description { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("formTemplate")]
    public required string FormTemplate { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("drawing")]
    public required string Drawing { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("status")]
    public required string Status { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("enabledCategoryLock")]
    public bool? EnabledCategoryLock { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("enabled")]
    public bool? Enabled { get; set; }

  }

  [Fact]
  public void FromJsonStateUnderTestExpectedBehavior()
  {
    // Arrange
    string json = /*lang=json,strict*/ """
            {
                "name": "Test"
            }
            """;

    // Act
    TestClass? result = json.FromJson<TestClass>();

    // Assert
    Assert.Equal("Test", result.Name);
  }

  [Fact]
  public void FromJsonStateUnderTestListTest()
  {
    // Arrange
    string json = /*lang=json,strict*/ """
            [
              {
                "id": 11,
                "displayName": "E-水資源管理",
                "description": "",
                "attributes": [
                  {
                    "name": "20ddcf66a4e14da5a8f5e49b51d5d48f",
                    "defaultValue": [],
                    "displayName": "組織單位",
                    "fieldInfo": {
                      "type": "text",
                      "value": [
                        ""
                      ]
                    },
                    "required": true,
                    "readOnly": false,
                    "type": "title",
                    "hide": false,
                    "id": "field_6"
                  },
                  {
                    "name": "8d7b3a95ce94470cbd04a306bad60229",
                    "defaultValue": [],
                    "displayName": "用水計費期間(起)",
                    "fieldInfo": {
                      "type": "date",
                      "value": [
                        ""
                      ]
                    },
                    "required": true,
                    "readOnly": false,
                    "type": "others",
                    "hide": false,
                    "id": "field_1"
                  },
                  {
                    "name": "557988a46f844fd68bf0379d11a65e8d",
                    "defaultValue": [],
                    "displayName": "用水計費期間(迄)",
                    "fieldInfo": {
                      "type": "date",
                      "value": [
                        ""
                      ]
                    },
                    "required": true,
                    "readOnly": false,
                    "type": "others",
                    "hide": false,
                    "id": "field_2"
                  },
                  {
                    "name": "14cd46fcc4b64b2c87f0db33cdfcc964",
                    "defaultValue": [],
                    "displayName": "本期用水度數",
                    "fieldInfo": {
                      "type": "text",
                      "value": [
                        ""
                      ]
                    },
                    "required": true,
                    "readOnly": false,
                    "type": "summary",
                    "hide": false,
                    "id": "field_3"
                  }
                ],
                "signoffList": [],
                "notificationSetting": {},
                "formTemplate": "{\"header_description\":{\"logo\":[],\"title\":[],\"date\":[],\"storage\":[],\"is_header_show\":false,\"height\":120},\"form_description\":[{\"row_id\":\"row_1\",\"table_type\":\"title\",\"class_name\":\"table-type_title\",\"table_height\":40,\"table_textalign\":\"center\",\"table_columns\":[{\"title\":\"水資源管理\",\"has_child\":false}],\"is_focus\":false,\"focus_state_class\":\"\"},{\"row_id\":\"row_7\",\"table_type\":\"space\",\"class_name\":\"table-space\",\"table_height\":40,\"table_textalign\":\"center\",\"table_columns\":[],\"is_focus\":false,\"focus_state_class\":\"\"},{\"row_id\":\"row_6\",\"table_type\":\"fields-h\",\"class_name\":\"table-type_cols\",\"table_height\":40,\"table_textalign\":\"center\",\"table_columns\":[{\"has_child\":true,\"id\":\"field_6\",\"col_width\":720,\"col_inner_xasix\":360,\"col_outter_xasix\":720,\"table_columns\":{\"header_textalign\":\"left\",\"content_textalign\":\"left\",\"header_placeholder\":\"編輯名稱\",\"header_width\":360,\"header_height\":40,\"content_width\":360},\"fields_bind_index\":0,\"headerFontSize\":13,\"contentFontSize\":13,\"placeholder\":\"叡揚資訊德惠辦公室\"}],\"is_focus\":false,\"focus_state_class\":\"\"},{\"row_id\":\"row_2\",\"table_type\":\"fields-h\",\"class_name\":\"table-type_cols\",\"table_height\":40,\"table_textalign\":\"center\",\"table_columns\":[{\"has_child\":true,\"id\":\"field_1\",\"col_width\":360,\"col_inner_xasix\":125,\"col_outter_xasix\":360,\"table_columns\":{\"header_textalign\":\"left\",\"content_textalign\":\"left\",\"header_placeholder\":\"編輯名稱\",\"header_width\":125,\"header_height\":40,\"content_width\":235},\"fields_bind_index\":1,\"headerFontSize\":13,\"contentFontSize\":13,\"canSetRule\":false,\"rule\":\"\"},{\"has_child\":true,\"id\":\"field_2\",\"col_width\":360,\"col_inner_xasix\":488,\"col_outter_xasix\":720,\"table_columns\":{\"header_textalign\":\"left\",\"content_textalign\":\"left\",\"header_placeholder\":\"編輯名稱\",\"header_width\":128,\"header_height\":40,\"content_width\":232},\"fields_bind_index\":2,\"headerFontSize\":13,\"contentFontSize\":13,\"canSetRule\":false,\"rule\":\"\"}],\"is_focus\":false,\"focus_state_class\":\"\"},{\"row_id\":\"row_3\",\"table_type\":\"fields-h\",\"class_name\":\"table-type_cols\",\"table_height\":40,\"table_textalign\":\"center\",\"table_columns\":[{\"has_child\":true,\"id\":\"field_3\",\"col_width\":720,\"col_inner_xasix\":360,\"col_outter_xasix\":720,\"table_columns\":{\"header_textalign\":\"left\",\"content_textalign\":\"left\",\"header_placeholder\":\"編輯名稱\",\"header_width\":360,\"header_height\":40,\"content_width\":360},\"fields_bind_index\":3,\"headerFontSize\":13,\"contentFontSize\":13,\"affix\":{\"suffix\":\"度\"}}],\"is_focus\":false,\"focus_state_class\":\"\"}],\"id_index\":9,\"formtype\":\"normal\"}",
                "drawing": "https://bizform.vikosmos.com/staging/api/forms/11/images/thumbnail.png",
                "status": "publish",
                "categories": [],
                "enabledCategoryLock": false,
                "survey": {
                  "id": 1,
                  "expiredTime": "9999-12-31T15:59:59Z",
                  "enabledExpiredTime": false,
                  "startTime": "0001-01-01T00:00:00Z",
                  "enabledStartTime": false,
                  "quantity": 0,
                  "usedQuantity": 0,
                  "enabledQuantity": false,
                  "enabled": false,
                  "enabledUpload": false,
                  "enabledTitleAndDescription": false,
                  "enabledRedirectUrl": false,
                  "disabledDefaultHeader": false
                },
                "privileges": [],
                "settings": {
                  "proposerUpdateWorkFlow": {
                    "enabled": false
                  },
                  "currentAuditorUpdateWorkFlow": {
                    "enabled": false
                  },
                  "duplicateDocument": {
                    "enabled": true
                  },
                  "deputyAuditor": {
                    "enabled": true
                  }
                },
                "enabled": true,
                "automationRules": []
              },
              {
                "id": 12,
                "displayName": "E-溫室氣體排放",
                "description": "",
                "attributes": [
                  {
                    "name": "20ddcf66a4e14da5a8f5e49b51d5d48f",
                    "defaultValue": [],
                    "displayName": "組織單位",
                    "fieldInfo": {
                      "type": "custom",
                      "value": [
                        "related",
                        "crm",
                        "main",
                        "6cdeb690f6174006922658934c325a16",
                        "companies"
                      ]
                    },
                    "required": true,
                    "readOnly": false,
                    "type": "others",
                    "hide": false,
                    "id": "field_6"
                  },
                  {
                    "name": "8d7b3a95ce94470cbd04a306bad60229",
                    "defaultValue": [],
                    "displayName": "資料蒐集期間(起)",
                    "fieldInfo": {
                      "type": "date",
                      "value": [
                        ""
                      ]
                    },
                    "required": true,
                    "readOnly": false,
                    "type": "title",
                    "hide": false,
                    "id": "field_1"
                  },
                  {
                    "name": "557988a46f844fd68bf0379d11a65e8d",
                    "defaultValue": [],
                    "displayName": "資料蒐集期間(迄)",
                    "fieldInfo": {
                      "type": "date",
                      "value": [
                        ""
                      ]
                    },
                    "required": true,
                    "readOnly": false,
                    "type": "summary",
                    "hide": false,
                    "id": "field_2"
                  },
                  {
                    "name": "14cd46fcc4b64b2c87f0db33cdfcc964",
                    "defaultValue": [],
                    "displayName": "範疇一排放量",
                    "fieldInfo": {
                      "type": "text",
                      "value": [
                        ""
                      ]
                    },
                    "required": true,
                    "readOnly": false,
                    "type": "others",
                    "hide": false,
                    "id": "field_3"
                  },
                  {
                    "name": "0dc56b9dd25f4e3d91f57b0fd81ba78e",
                    "defaultValue": [],
                    "displayName": "範疇二排放量",
                    "fieldInfo": {
                      "type": "text",
                      "value": [
                        ""
                      ]
                    },
                    "required": true,
                    "readOnly": false,
                    "type": "others",
                    "hide": false,
                    "id": "field_4"
                  },
                  {
                    "name": "e35273875cc34b62b51dfc67b5ddbeef",
                    "defaultValue": [],
                    "displayName": "範疇三排放量",
                    "fieldInfo": {
                      "type": "text",
                      "value": [
                        ""
                      ]
                    },
                    "required": false,
                    "readOnly": false,
                    "type": "others",
                    "hide": false,
                    "id": "field_5"
                  }
                ],
                "signoffList": [],
                "notificationSetting": {},
                "formTemplate": "{\"header_description\":{\"logo\":[],\"title\":[],\"date\":[],\"storage\":[],\"is_header_show\":false,\"height\":120},\"form_description\":[{\"row_id\":\"row_1\",\"table_type\":\"title\",\"class_name\":\"table-type_title\",\"table_height\":40,\"table_textalign\":\"center\",\"table_columns\":[{\"title\":\"溫室氣體排放\",\"has_child\":false}],\"is_focus\":false,\"focus_state_class\":\"\"},{\"row_id\":\"row_7\",\"table_type\":\"space\",\"class_name\":\"table-space\",\"table_height\":40,\"table_textalign\":\"center\",\"table_columns\":[],\"is_focus\":false,\"focus_state_class\":\"\"},{\"row_id\":\"row_6\",\"table_type\":\"fields-h\",\"class_name\":\"table-type_cols\",\"table_height\":40,\"table_textalign\":\"center\",\"table_columns\":[{\"has_child\":true,\"id\":\"field_6\",\"col_width\":720,\"col_inner_xasix\":360,\"col_outter_xasix\":720,\"table_columns\":{\"header_textalign\":\"left\",\"content_textalign\":\"left\",\"header_placeholder\":\"編輯名稱\",\"header_width\":360,\"header_height\":40,\"content_width\":360},\"fields_bind_index\":0,\"headerFontSize\":13,\"contentFontSize\":13,\"placeholder\":\"叡揚資訊德惠辦公室\",\"canSetRule\":false,\"rule\":\"\"}],\"is_focus\":false,\"focus_state_class\":\"\"},{\"row_id\":\"row_2\",\"table_type\":\"fields-h\",\"class_name\":\"table-type_cols\",\"table_height\":40,\"table_textalign\":\"center\",\"table_columns\":[{\"has_child\":true,\"id\":\"field_1\",\"col_width\":360,\"col_inner_xasix\":125,\"col_outter_xasix\":360,\"table_columns\":{\"header_textalign\":\"left\",\"content_textalign\":\"left\",\"header_placeholder\":\"編輯名稱\",\"header_width\":125,\"header_height\":40,\"content_width\":235},\"fields_bind_index\":1,\"headerFontSize\":13,\"contentFontSize\":13,\"canSetRule\":false,\"rule\":\"\"},{\"has_child\":true,\"id\":\"field_2\",\"col_width\":360,\"col_inner_xasix\":488,\"col_outter_xasix\":720,\"table_columns\":{\"header_textalign\":\"left\",\"content_textalign\":\"left\",\"header_placeholder\":\"編輯名稱\",\"header_width\":128,\"header_height\":40,\"content_width\":232},\"fields_bind_index\":2,\"headerFontSize\":13,\"contentFontSize\":13,\"canSetRule\":false,\"rule\":\"\"}],\"is_focus\":false,\"focus_state_class\":\"\"},{\"row_id\":\"row_3\",\"table_type\":\"fields-h\",\"class_name\":\"table-type_cols\",\"table_height\":40,\"table_textalign\":\"center\",\"table_columns\":[{\"has_child\":true,\"id\":\"field_3\",\"col_width\":720,\"col_inner_xasix\":360,\"col_outter_xasix\":720,\"table_columns\":{\"header_textalign\":\"left\",\"content_textalign\":\"left\",\"header_placeholder\":\"編輯名稱\",\"header_width\":360,\"header_height\":40,\"content_width\":360},\"fields_bind_index\":3,\"headerFontSize\":13,\"contentFontSize\":13,\"affix\":{\"suffix\":\"tCO2e\"}}],\"is_focus\":false,\"focus_state_class\":\"\"},{\"row_id\":\"row_4\",\"table_type\":\"fields-h\",\"class_name\":\"table-type_cols\",\"table_height\":40,\"table_textalign\":\"center\",\"table_columns\":[{\"has_child\":true,\"id\":\"field_4\",\"col_width\":720,\"col_inner_xasix\":360,\"col_outter_xasix\":720,\"table_columns\":{\"header_textalign\":\"left\",\"content_textalign\":\"left\",\"header_placeholder\":\"編輯名稱\",\"header_width\":360,\"header_height\":40,\"content_width\":360},\"fields_bind_index\":4,\"headerFontSize\":13,\"contentFontSize\":13,\"affix\":{\"suffix\":\"tCO2e\"}}],\"is_focus\":false,\"focus_state_class\":\"\"},{\"row_id\":\"row_5\",\"table_type\":\"fields-h\",\"class_name\":\"table-type_cols\",\"table_height\":40,\"table_textalign\":\"center\",\"table_columns\":[{\"has_child\":true,\"id\":\"field_5\",\"col_width\":720,\"col_inner_xasix\":360,\"col_outter_xasix\":720,\"table_columns\":{\"header_textalign\":\"left\",\"content_textalign\":\"left\",\"header_placeholder\":\"編輯名稱\",\"header_width\":360,\"header_height\":40,\"content_width\":360},\"fields_bind_index\":5,\"headerFontSize\":13,\"contentFontSize\":13,\"affix\":{\"suffix\":\"tCO2e\"}}],\"is_focus\":false,\"focus_state_class\":\"\"}],\"id_index\":6,\"formtype\":\"normal\"}",
                "drawing": "https://bizform.vikosmos.com/staging/api/forms/12/images/thumbnail.png",
                "status": "publish",
                "categories": [],
                "enabledCategoryLock": false,
                "survey": {
                  "id": 2,
                  "expiredTime": "9999-12-31T15:59:59Z",
                  "enabledExpiredTime": false,
                  "startTime": "0001-01-01T00:00:00Z",
                  "enabledStartTime": false,
                  "quantity": 0,
                  "usedQuantity": 0,
                  "enabledQuantity": false,
                  "enabled": false,
                  "enabledUpload": false,
                  "enabledTitleAndDescription": false,
                  "enabledRedirectUrl": false,
                  "disabledDefaultHeader": false
                },
                "privileges": [],
                "settings": {
                  "proposerUpdateWorkFlow": {
                    "enabled": false
                  },
                  "currentAuditorUpdateWorkFlow": {
                    "enabled": false
                  },
                  "duplicateDocument": {
                    "enabled": true
                  },
                  "deputyAuditor": {
                    "enabled": true
                  }
                },
                "enabled": true,
                "automationRules": []
              }
            ]
            """;

    // Act
    List<Form>? result = json.FromJson<List<Form>>();

    // Assert
    Assert.Equal(2, result.Count);
  }
}
