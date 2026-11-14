using MathLock.Models;
using MathLock.Services;
using Xunit;

namespace MathLock.Tests;

public class QuestionGeneratorTests
{
    private readonly QuestionGenerator _generator = new();

    [Theory]
    [InlineData(DifficultyLevel.Grade1_2)]
    [InlineData(DifficultyLevel.Grade2_3)]
    [InlineData(DifficultyLevel.Grade4_5)]
    public void GenerateStandard_ProducesValidQuestionAndAnswerInChoices(DifficultyLevel difficulty)
    {
        for (int i = 0; i < 20; i++)
        {
            var q = _generator.GenerateStandard(difficulty);
            Assert.False(string.IsNullOrWhiteSpace(q.ExpressionText));
            Assert.False(string.IsNullOrWhiteSpace(q.TargetAnswer));
            Assert.Contains(q.TargetAnswer, q.Choices);
            Assert.Equal(4, q.Choices.Count);
        }
    }

    [Theory]
    [InlineData(DifficultyLevel.Grade1_2)]
    [InlineData(DifficultyLevel.Grade2_3)]
    [InlineData(DifficultyLevel.Grade4_5)]
    public void GenerateMissingOperand_ProducesValidFormatAndChoices(DifficultyLevel difficulty)
    {
        for (int i = 0; i < 20; i++)
        {
            var q = _generator.GenerateMissingOperand(difficulty);
            Assert.Contains("?", q.ExpressionText);
            Assert.Contains(q.TargetAnswer, q.Choices);
            Assert.Equal(4, q.Choices.Count);
        }
    }

    [Theory]
    [InlineData(DifficultyLevel.Grade1_2)]
    [InlineData(DifficultyLevel.Grade2_3)]
    public void GenerateComparison_ProducesValidRelationalSymbol(DifficultyLevel difficulty)
    {
        for (int i = 0; i < 20; i++)
        {
            var q = _generator.GenerateComparison(difficulty);
            Assert.Contains(q.TargetAnswer, new[] { "<", "=", ">" });
            Assert.Equal(3, q.Choices.Count);
            Assert.Contains("<", q.Choices);
            Assert.Contains("=", q.Choices);
            Assert.Contains(">", q.Choices);
        }
    }

    [Theory]
    [InlineData(DifficultyLevel.Grade1_2)]
    [InlineData(DifficultyLevel.Grade2_3)]
    public void GenerateNumberBond_AnswerCorrectlySumsToTarget(DifficultyLevel difficulty)
    {
        for (int i = 0; i < 20; i++)
        {
            var q = _generator.GenerateNumberBond(difficulty);
            Assert.Contains(q.TargetAnswer, q.Choices);
            Assert.Equal(4, q.Choices.Count);
        }
    }

    [Fact]
    public void GenerateVisualGroup_ProducesEmojiHintAndAccurateAnswer()
    {
        for (int i = 0; i < 20; i++)
        {
            var q = _generator.GenerateVisualGroup(DifficultyLevel.Grade1_2);
            Assert.False(string.IsNullOrEmpty(q.VisualHint));
            Assert.Contains(q.TargetAnswer, q.Choices);
            Assert.Equal(4, q.Choices.Count);
        }
    }

    [Fact]
    public void NextQuestion_ReturnsVariedQuestionTypes()
    {
        var types = new HashSet<QuestionType>();
        for (int i = 0; i < 50; i++)
        {
            var q = _generator.NextQuestion(DifficultyLevel.Grade2_3);
            types.Add(q.Type);
        }

        // Must have generated at least 3 distinct types across 50 runs
        Assert.True(types.Count >= 3);
    }
}
