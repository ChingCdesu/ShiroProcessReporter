namespace ProcessReporterWin.Entities;

public class FilterRule
{
    public string KeyWord { get; set; } = string.Empty;
    public bool Editing { get; set; }
    public bool NotEditing => !Editing;
}