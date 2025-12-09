namespace Crolow.Cms.Server.Core.Enums
{
    public enum DataRelationType
    {
        Default = 0,
        FromItem = 1,
        FromField = 2
    }


    public enum DataRelationTarget
    {
        Default = 0,
        Children = 1,
        ChildrenFolder = 2,
        InternalLink = 4,
        ExternalLink = 8,
    }

    public enum DataRelationCount
    {
        Default = 0,
        Single = 1,
        SingleOrEmpty = 2,
        Multiple = 4,
        MultipleOrEmpty = 8
    }



}
