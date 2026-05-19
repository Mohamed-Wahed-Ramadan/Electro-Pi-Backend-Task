using AutoMapper;
using ProjectManagement.Application.DTOs.Projects;
using ProjectManagement.Application.DTOs.Tasks;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Project, ProjectDto>()
            .ForMember(d => d.OwnerName, opt => opt.MapFrom(s => $"{s.Owner.FirstName} {s.Owner.LastName}"))
            .ForMember(d => d.TaskCount, opt => opt.MapFrom(s => s.Tasks.Count));

        CreateMap<ProjectTask, TaskDto>()
            .ForMember(d => d.AssignedUserName, opt => opt.MapFrom(s =>
                s.AssignedUser != null ? $"{s.AssignedUser.FirstName} {s.AssignedUser.LastName}" : null));
    }
}
