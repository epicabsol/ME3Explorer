﻿using ME3Script.Analysis.Visitors;
using ME3Script.Utilities;
using System.Collections.Generic;

namespace ME3Script.Language.Tree
{
    public class AssignStatement : Statement
    {
        public Expression Target;
        public Expression Value;
        public AssignStatement(Expression target, Expression value,
            SourcePosition start = null, SourcePosition end = null) 
            : base(ASTNodeType.AssignStatement, start, end) 
        {
            Target = target;
            Value = value;
        }

        public override bool AcceptVisitor(IASTVisitor visitor)
        {
            return visitor.VisitNode(this);
        }
        public override IEnumerable<ASTNode> ChildNodes
        {
            get
            {
                yield return Target;
                yield return Value;
            }
        }
    }
}