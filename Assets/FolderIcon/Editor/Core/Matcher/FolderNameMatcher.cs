namespace FolderIcon.Editor.Core.Matcher
{
    public class FolderNameMatcher : IFolderMatcher
    {
        private readonly MatchMethod _matchMethod;


        public bool IsMatch(string folderPath)
        {
            return false;
        }

        public int GetPriority()
        {
            return _matchMethod switch
            {
                MatchMethod.Exact => 90,
                MatchMethod.Pattern => 60,
                MatchMethod.Regex => 80,
            };
        }
    }
}