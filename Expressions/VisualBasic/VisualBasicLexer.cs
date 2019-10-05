﻿using Antlr.Runtime;

namespace Expressions.VisualBasic
{
    partial class VisualBasicLexer
    {
        public override void ReportError(RecognitionException e)
        {
            throw new ExpressionsException("Invalid syntax", ExpressionsExceptionType.SyntaxError, e);
        }

        protected override object RecoverFromMismatchedToken(IIntStream input, int ttype, BitSet follow)
        {
            throw new MismatchedTokenException(ttype, input);
        }

        public override object RecoverFromMismatchedSet(IIntStream input, RecognitionException e, BitSet follow)
        {
            throw e;
        }
    }
}
