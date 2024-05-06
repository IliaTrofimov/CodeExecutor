using AutoMapper;
using CodeExecutor.Dispatcher.Contracts;

namespace CodeExecutor.Dispatcher.Services.Utils;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        //        Source ------> Destination  

        CreateMap<DbModel.CodeExecution, CodeExecution>()
            .ForMember(
                d => d.Guid,
                s => s.MapFrom(x => x.Id));

        CreateMap<DbModel.CodeExecution, CodeExecutionExpanded>()
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

        CreateMap<CodeExecutionRequest, DbModel.CodeExecution>()
            .ForMember(
                d => d.SourceCode, 
                s => s.MapFrom(x => new DbModel.SourceCode { CodeText = x.CodeText }));

        CreateMap<Language, DbModel.Language>();
        CreateMap<DbModel.Language, Language>();

        CreateMap<DbModel.CodeExecution, SourceCode>()
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