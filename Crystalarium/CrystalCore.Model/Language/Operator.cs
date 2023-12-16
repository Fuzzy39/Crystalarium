namespace CrystalCore.Model.Language
{
    public enum Operator
    {
        EqualTo,
        GreaterThan,
        LessThan,
        NotEqualTo,
        Or,
        And,
        Xor,
        // woo, arithmatic
        Add,
        Subtract,
        Multiply,
        Divide,
        Modulo
    }
    public static class OperatorExtensions
    {

        internal static Token Operate(this Operator op, Token a, Token b)
        {

            if (!op.IsValid(a.Type, b.Type))
            {
                throw new InvalidOperationException("Invalid operation. How did this happen?");
            }

            return op switch
            {
                Operator.EqualTo => new Token(TokenType.boolean, a.Value.Equals(b.Value)),
                Operator.NotEqualTo => new Token(TokenType.boolean, !a.Value.Equals(b.Value)),

                Operator.GreaterThan => new Token(TokenType.boolean, (int)a.Value > (int)b.Value),
                Operator.LessThan => new Token(TokenType.boolean, (int)a.Value < (int)b.Value),

                Operator.Or => new Token(TokenType.boolean, (bool)a.Value || (bool)b.Value),
                Operator.And => new Token(TokenType.boolean, (bool)a.Value & (bool)b.Value),
                Operator.Xor => new Token(TokenType.boolean, (bool)a.Value ^ (bool)b.Value),

                Operator.Add => new Token(TokenType.integer, (int)a.Value + (int)b.Value),
                Operator.Subtract => new Token(TokenType.integer, (int)a.Value - (int)b.Value),
                Operator.Multiply => new Token(TokenType.integer, (int)a.Value * (int)b.Value),
                Operator.Divide => new Token(TokenType.integer, (int)a.Value / (int)b.Value),
                Operator.Modulo => new Token(TokenType.integer, (int)a.Value % (int)b.Value),
                _ => throw new InvalidOperationException("Missing a case here!"),
            };
        }

        internal static bool IsValid(this Operator op, TokenType a, TokenType b)
        {

            if (op == Operator.Add || op == Operator.Subtract || op == Operator.Multiply || op == Operator.Divide || op == Operator.Modulo)
            {
                return a == TokenType.integer && b == TokenType.integer;
            }

            if (op == Operator.NotEqualTo || op == Operator.EqualTo)
            {
                return true;
            }


            if (a != b)
            {

                return false;
            }

            if (a == TokenType.boolean)
            {
                if (op == Operator.Or || op == Operator.And || op == Operator.Xor)
                {
                    return true;
                }

                return false;
            }

            if (op == Operator.GreaterThan || op == Operator.LessThan)
            {
                return true;
            }


            return false;
        }

        internal static TokenType ReturnType(this Operator op)
        {
            // this is ugly.
            if (op == Operator.Add || op == Operator.Subtract || op == Operator.Multiply || op == Operator.Divide || op == Operator.Modulo)
            {
                return TokenType.integer;
            }

            return TokenType.boolean;
        }
    }
}
