namespace Main.Repository;

public class GlobelMappings : Profile
{
    public GlobelMappings()
    {
        CreateMap<DateOnly, DateTime>()
            .ConvertUsing(dateOnly => dateOnly.ToDateTime(TimeOnly.MinValue));

        CreateMap<DateTime, DateOnly>()
            .ConvertUsing(dateTime => DateOnly.FromDateTime(dateTime));
    }
}
