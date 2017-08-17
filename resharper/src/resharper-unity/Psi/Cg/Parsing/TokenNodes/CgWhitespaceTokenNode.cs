﻿using JetBrains.ReSharper.Plugins.Unity.Psi.Cg.Parsing.TokenNodeTypes;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;

namespace JetBrains.ReSharper.Plugins.Unity.Psi.Cg.Parsing.TokenNodes
{
    internal class CgWhitespaceTokenNode : CgWhitespaceTokenNodeBase
    {
        public CgWhitespaceTokenNode(string text)
            : base(text)
        {
        }

        public override NodeType NodeType => CgTokenNodeTypes.WHITESPACE;
        public override bool IsNewLine => false;
    }
}