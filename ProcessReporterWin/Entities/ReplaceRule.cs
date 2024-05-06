namespace ProcessReporterWin.Entities;

public class ReplaceRule
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public bool Editing { get; set; }
    public bool NotEditing => !Editing;
}