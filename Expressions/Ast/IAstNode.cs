namespace Expressions.Ast
{
    internal interface IAstNode
    {
        T Accept<T>(IAstVisitor<T> visitor);
    }
}
