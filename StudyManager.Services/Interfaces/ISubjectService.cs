using StudyManager.Services.Dtos.Subjects;

namespace StudyManager.Services.Interfaces;

public interface ISubjectService
{
    IReadOnlyList<SubjectListItemDto> GetSubjects();
    SubjectDetailsDto GetSubjectDetails(Guid subjectId);
}