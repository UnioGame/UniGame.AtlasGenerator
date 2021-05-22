namespace UniModules.UniGame.AtlasGenerator.Editor.Abstract
{
#if !ODIN_INSPECTOR_3

    public interface ISearchFilterable
    {
        bool IsMatch(string searchString);
    }
    
#endif
}