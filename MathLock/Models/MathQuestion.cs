namespace MathLock.Models;

public enum QuestionType
{
    Standard,       // e.g. 7 + 8 = ? or 6 * 4 = ?
    MissingOperand, // e.g. 7 + ? = 15 or ? * 4 = 24
    Comparison,     // e.g. 3 * 6 [?] 20 - 3 (Choices: <, =, >)
    NumberBond,     // e.g. Target is 100 or 20, choose the partner for 65
    VisualGroup     // e.g. 4 groups of 3 stars = ?
}

public class MathQuestion
{
    public QuestionType Type { get; set; } = QuestionType.Standard;
    public string PromptTitle { get; set; } = string.Empty;
    public string ExpressionText { get; set; } = string.Empty;
    public string TargetAnswer { get; set; } = string.Empty;
    public List<string> Choices { get; set; } = new();
    public string VisualHint { get; set; } = string.Empty;
    public bool UsesMultipleChoice { get; set; } = true;
}
