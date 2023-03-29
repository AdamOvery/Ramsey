interface ICliqueCollection
{
    IEnumerable<Clique> GetCliques();
    int Length { get; }
}