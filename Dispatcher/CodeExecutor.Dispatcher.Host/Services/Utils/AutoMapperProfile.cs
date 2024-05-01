using AutoMapper;

namespace CodeExecutor.Dispatcher.Host.Services.Utils;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        //        Source ------> Destination  

        CreateMap<DbModel.CodeExecution, Contracts.CodeExecution>()
            .ForMember(
                d => d.Guid,
                s => s.MapFrom(x => x.Id));

        CreateMap<DbModel.CodeExecution, Contracts.CodeExecutionExpanded>()
            .ForMember(
                d => d.Data,
                s => s.MapFrom(x => x.Result != null ? x.Result.Data : null))
            .ForMember(
                d => d.SourceCode,
                s => s.MapFrom(x => x.SourceCode != null ? x.SourceCode.CodeText : null))
            .ForMember(
                d => d.RequestedAt,
                s => s.MapFrom(x => x.RequestedAt))
            .ForMember(
                d => d.Guid,
                s => s.MapFrom(x => x.Id));

        CreateMap<Contracts.CodeExecutionRequest, DbModel.CodeExecution>()
            .ForMember(
                d => d.SourceCode, 
                s => s.MapFrom(x => new DbModel.SourceCode { CodeText = x.CodeText }));
        
        CreateMap<Contracts.Language, DbModel.Language>().ReverseMap();
        
        CreateMap<DbModel.CodeExecution, Contracts.SourceCode>()
            .ForMember(
                d => d.LanguageId,
                s => s.MapFrom(x => x.LanguageId))
            .ForMember(
                d => d.Guid,
                s => s.MapFrom(x => x.Id))
            .ForMember(
                d => d.CodeText,
                s => s.MapFrom(x => x.SourceCode != null ? x.SourceCode.CodeText : null));
    }
}