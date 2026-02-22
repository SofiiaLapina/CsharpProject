namespace StudyManager.Storage;

public sealed class SubjectData
{
    public Guid Id { get; }
    public string Name { get; private set; }
    public int EctsCredits { get; private set; }
    public KnowledgeArea Area { get; private set; }

    public SubjectData(Guid id, string name, int ectsCredits, KnowledgeArea area)
    {
        Id = id;
        Name = name;
        EctsCredits = ectsCredits;
        Area = area;
    }

    public void Update(string name, int ectsCredits, KnowledgeArea area)
    {
        Name = name;
        EctsCredits = ectsCredits;
        Area = area;
    }
}
