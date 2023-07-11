namespace DataExplorer.EfCore.Specifications.Builders;

public interface IChainControlledSpecification
{
    bool IsChainDiscarded { get; set; }
}
