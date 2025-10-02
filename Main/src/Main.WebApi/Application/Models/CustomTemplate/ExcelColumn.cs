using Main.Domain.SeedWork;

namespace Main.Dto.ViewModel.CustomTemplate;

public class TemplateColumn(int id, string name) : Enumeration(id, name)
{
    public static readonly TemplateColumn GroupName = new(1, "指標分類");
    public static readonly TemplateColumn RowNumber = new(2, "議題編號");
    public static readonly TemplateColumn PlanTemplateName = new(3, "指標議題");
    public static readonly TemplateColumn DefaultCycle = new(4, "預設週期");
    public static readonly TemplateColumn DocumentName = new(5, "demo Bizform 表單名稱");
    public static readonly TemplateColumn PlanTemplateDetailTitle = new(6, "指標項目");
}

public class PlanTemplateNameColumn(int id, string name) : Enumeration(id, name)
{
    public static readonly PlanTemplateNameColumn Traditional = new(1, "指標議題(繁體中文)");
    public static readonly PlanTemplateNameColumn Simplified = new(2, "指標議題(简体中文)");
    public static readonly PlanTemplateNameColumn English = new(3, "指標議題(English)");
    public static readonly PlanTemplateNameColumn Japanese = new(4, "指標議題(日本語)");
}

public class PlanTemplateDetailTitleColumn(int id, string name) : Enumeration(id, name)
{
    public static readonly PlanTemplateDetailTitleColumn Traditional = new(1, "指標項目(繁體中文)");
    public static readonly PlanTemplateDetailTitleColumn Simplified = new(2, "指標項目(简体中文)");
    public static readonly PlanTemplateDetailTitleColumn English = new(3, "指標項目(English)");
    public static readonly PlanTemplateDetailTitleColumn Japanese = new(4, "指標項目(日本語)");
}
