using MathLock.Models;

namespace MathLock.Services;

public class QuestionGenerator
{
    private readonly Random _random;

    public QuestionGenerator(Random? random = null)
    {
        _random = random ?? new Random();
    }

    public MathQuestion NextQuestion(DifficultyLevel difficulty)
    {
        // Randomly pick a question style
        var types = new[]
        {
            QuestionType.Standard,
            QuestionType.MissingOperand,
            QuestionType.Comparison,
            QuestionType.NumberBond,
            QuestionType.VisualGroup
        };

        var selectedType = types[_random.Next(types.Length)];

        return selectedType switch
        {
            QuestionType.MissingOperand => GenerateMissingOperand(difficulty),
            QuestionType.Comparison => GenerateComparison(difficulty),
            QuestionType.NumberBond => GenerateNumberBond(difficulty),
            QuestionType.VisualGroup => GenerateVisualGroup(difficulty),
            _ => GenerateStandard(difficulty)
        };
    }

    public MathQuestion GenerateStandard(DifficultyLevel difficulty)
    {
        int a, b, answer;
        string op;

        if (difficulty == DifficultyLevel.Grade1_2)
        {
            // + or - within 20
            bool isAdd = _random.Next(2) == 0;
            if (isAdd)
            {
                a = _random.Next(1, 11);
                b = _random.Next(1, 10);
                answer = a + b;
                op = "+";
            }
            else
            {
                a = _random.Next(4, 21);
                b = _random.Next(1, a);
                answer = a - b;
                op = "-";
            }
        }
        else if (difficulty == DifficultyLevel.Grade2_3)
        {
            // +, -, x, ÷ within 100
            int roll = _random.Next(4);
            if (roll == 0) // Addition
            {
                a = _random.Next(10, 60);
                b = _random.Next(5, 40);
                answer = a + b;
                op = "+";
            }
            else if (roll == 1) // Subtraction
            {
                a = _random.Next(20, 100);
                b = _random.Next(5, a);
                answer = a - b;
                op = "-";
            }
            else if (roll == 2) // Multiplication
            {
                a = _random.Next(2, 10);
                b = _random.Next(2, 10);
                answer = a * b;
                op = "×";
            }
            else // Division
            {
                b = _random.Next(2, 10);
                answer = _random.Next(2, 10);
                a = b * answer;
                op = "÷";
            }
        }
        else
        {
            // Grade 4-5
            int roll = _random.Next(4);
            if (roll == 0)
            {
                a = _random.Next(50, 450);
                b = _random.Next(30, 350);
                answer = a + b;
                op = "+";
            }
            else if (roll == 1)
            {
                a = _random.Next(80, 500);
                b = _random.Next(20, a);
                answer = a - b;
                op = "-";
            }
            else if (roll == 2)
            {
                a = _random.Next(4, 13);
                b = _random.Next(6, 13);
                answer = a * b;
                op = "×";
            }
            else
            {
                b = _random.Next(3, 12);
                answer = _random.Next(4, 15);
                a = b * answer;
                op = "÷";
            }
        }

        string ansStr = answer.ToString();
        var choices = GenerateChoices(answer);

        return new MathQuestion
        {
            Type = QuestionType.Standard,
            PromptTitle = "Quick Mental Math",
            ExpressionText = $"{a} {op} {b} = ?",
            TargetAnswer = ansStr,
            Choices = choices,
            UsesMultipleChoice = true
        };
    }

    public MathQuestion GenerateMissingOperand(DifficultyLevel difficulty)
    {
        bool isMultiplication = difficulty != DifficultyLevel.Grade1_2 && _random.Next(2) == 0;
        int a, b, result;
        string expr;
        string target;

        if (isMultiplication)
        {
            a = _random.Next(2, 10);
            b = _random.Next(2, 10);
            result = a * b;
            bool hideFirst = _random.Next(2) == 0;
            if (hideFirst)
            {
                expr = $"? × {b} = {result}";
                target = a.ToString();
            }
            else
            {
                expr = $"{a} × ? = {result}";
                target = b.ToString();
            }
        }
        else
        {
            int max = difficulty == DifficultyLevel.Grade1_2 ? 20 : 60;
            a = _random.Next(2, max / 2);
            b = _random.Next(2, max / 2);
            result = a + b;
            bool hideFirst = _random.Next(2) == 0;
            if (hideFirst)
            {
                expr = $"? + {b} = {result}";
                target = a.ToString();
            }
            else
            {
                expr = $"{a} + ? = {result}";
                target = b.ToString();
            }
        }

        int targetVal = int.Parse(target);
        return new MathQuestion
        {
            Type = QuestionType.MissingOperand,
            PromptTitle = "Find the Missing Number",
            ExpressionText = expr,
            TargetAnswer = target,
            Choices = GenerateChoices(targetVal),
            UsesMultipleChoice = true
        };
    }

    public MathQuestion GenerateComparison(DifficultyLevel difficulty)
    {
        // Compares two mini expressions or an expression and a number
        int leftVal, rightVal;
        string leftText, rightText;

        if (difficulty == DifficultyLevel.Grade1_2)
        {
            int l1 = _random.Next(2, 10), l2 = _random.Next(1, 9);
            leftVal = l1 + l2;
            leftText = $"{l1} + {l2}";

            // Decide right
            int mode = _random.Next(3);
            if (mode == 0) // equal
            {
                rightVal = leftVal;
                rightText = rightVal.ToString();
            }
            else if (mode == 1) // greater
            {
                rightVal = leftVal + _random.Next(1, 4);
                rightText = rightVal.ToString();
            }
            else
            {
                rightVal = Math.Max(1, leftVal - _random.Next(1, 4));
                rightText = rightVal.ToString();
            }
        }
        else
        {
            // Multiplication vs number or addition
            int a = _random.Next(3, 9), b = _random.Next(3, 9);
            leftVal = a * b;
            leftText = $"{a} × {b}";

            int offset = _random.Next(-3, 4); // could be 0 (equal)
            rightVal = Math.Max(2, leftVal + offset);
            rightText = rightVal.ToString();
        }

        string target = leftVal < rightVal ? "<" : (leftVal > rightVal ? ">" : "=");

        return new MathQuestion
        {
            Type = QuestionType.Comparison,
            PromptTitle = "Compare: Which is bigger?",
            ExpressionText = $"{leftText}   [ ? ]   {rightText}",
            TargetAnswer = target,
            Choices = new List<string> { "<", "=", ">" },
            UsesMultipleChoice = true
        };
    }

    public MathQuestion GenerateNumberBond(DifficultyLevel difficulty)
    {
        int target = difficulty == DifficultyLevel.Grade1_2 ? 10 : (difficulty == DifficultyLevel.Grade2_3 ? 100 : 1000);
        int given;

        if (target == 10)
        {
            given = _random.Next(1, 10);
        }
        else if (target == 100)
        {
            // Usually nice endings like 5 or multiples of 10
            int[] ones = { 0, 5, _random.Next(1, 9) };
            int pickedOne = ones[_random.Next(ones.Length)];
            given = _random.Next(1, 9) * 10 + pickedOne;
        }
        else
        {
            given = _random.Next(10, 90) * 10;
        }

        int answer = target - given;
        var choices = GenerateChoices(answer);

        return new MathQuestion
        {
            Type = QuestionType.NumberBond,
            PromptTitle = $"Make {target}!",
            ExpressionText = $"{given} + [ ? ] = {target}",
            TargetAnswer = answer.ToString(),
            Choices = choices,
            VisualHint = $"How much more do you need to reach {target}?",
            UsesMultipleChoice = true
        };
    }

    public MathQuestion GenerateVisualGroup(DifficultyLevel difficulty)
    {
        int groups = _random.Next(2, 6);
        int perGroup = _random.Next(2, 6);
        int total = groups * perGroup;

        string[] icons = { "⭐", "🍎", "🎈", "🐶", "🚀", "🍭" };
        string icon = icons[_random.Next(icons.Length)];

        // Build visual representation
        List<string> groupParts = new();
        for (int i = 0; i < groups; i++)
        {
            groupParts.Add(string.Concat(Enumerable.Repeat(icon, perGroup)));
        }
        string visual = string.Join("   ", groupParts);

        return new MathQuestion
        {
            Type = QuestionType.VisualGroup,
            PromptTitle = $"Count the {icon}",
            ExpressionText = $"{groups} groups of {perGroup} {icon} = ?",
            TargetAnswer = total.ToString(),
            Choices = GenerateChoices(total),
            VisualHint = visual,
            UsesMultipleChoice = true
        };
    }

    public List<string> GenerateChoices(int correctAnswer)
    {
        HashSet<int> pool = new() { correctAnswer };
        int tries = 0;

        while (pool.Count < 4 && tries < 50)
        {
            tries++;
            // Generate realistic distractors (off by 1, 2, 5, 10, or inverted digits)
            int delta = _random.Next(1, 5) * (_random.Next(2) == 0 ? 1 : -1);
            if (_random.Next(3) == 0 && correctAnswer > 15)
            {
                delta = (_random.Next(1, 3) * 10) * (_random.Next(2) == 0 ? 1 : -1);
            }
            int candidate = correctAnswer + delta;
            if (candidate >= 0 && candidate != correctAnswer)
            {
                pool.Add(candidate);
            }
        }

        // Fallback if pool is too small
        int fallback = 1;
        while (pool.Count < 4)
        {
            if (!pool.Contains(correctAnswer + fallback))
                pool.Add(correctAnswer + fallback);
            else if (correctAnswer - fallback >= 0 && !pool.Contains(correctAnswer - fallback))
                pool.Add(correctAnswer - fallback);
            fallback++;
        }

        var result = pool.Select(x => x.ToString()).ToList();
        // Shuffle choices
        return result.OrderBy(_ => _random.Next()).ToList();
    }
}
