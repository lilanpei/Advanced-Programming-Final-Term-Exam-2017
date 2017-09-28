using Annotation_system.Annotations;
using Annotation_system.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Annotation_system.Parser
{
    class AnnotationParse
    {
        private readonly IEnumerator<Token> _tokens;
        private List<Table> tbs;

        public AnnotationParse(IEnumerable<Token> tokens)
        {
            _tokens = tokens.GetEnumerator();
            tbs = new List<Table>();
        }

        public List<Table> Parse()
        {
            while (_tokens.MoveNext())
            {
                var t = ParseAnnotation();
                _tokens.MoveNext();
                ParseKeyword(KeywordEnum._public);
                _tokens.MoveNext();
                ParseKeyword(KeywordEnum._interface);
                _tokens.MoveNext();
                var idf = ParseChars<StringToken>();
                if (!(t is Table))
                    throw new Exception("Syntax error with unexpect Object Type");
                ((Table)t).type = idf;
                _tokens.MoveNext();
                ParseBody((Table)t);
            }
            return tbs;
        }

        private void ParseBody(Table t)
        {
            ParseSymbol(SymbolEnum.leftbrace);
            _tokens.MoveNext();
            do
            {
                var anno = ParseAnnotation();
                if (anno is Id)
                    t.id = (Id)anno;
                else if (anno is Column)
                    t.columns = new List<Column> { (Column)anno };
                else if (anno is Many2One)
                    t.relation = (Many2One)anno;
                else if (anno is One2Many)
                    t.relation = (One2Many)anno;
                else
                    throw new Exception(" syntax error with unexpect object type");
                _tokens.MoveNext();
                ParseDefinition(anno);
                _tokens.MoveNext();
            } while ((_tokens.Current is SymbolToken) && (((SymbolToken)_tokens.Current).symbol == SymbolEnum.at));

            if ((_tokens.Current is SymbolToken) && ((SymbolToken)_tokens.Current).symbol == SymbolEnum.rightbrace)
            {
                ParseSymbol(SymbolEnum.rightbrace);
                return;
            }
            throw new Exception("Syntax error with unexpect token :" + _tokens.Current.str);
        }

        private void ParseDefinition(Annotation annobody)
        {
            if (_tokens.Current is KeywordToken)
            {
                if (((KeywordToken)_tokens.Current).keyword == KeywordEnum.Integer)
                {
                    ParseKeyword(KeywordEnum.Integer);
                    _tokens.MoveNext();
                    string variable = ParseChars<StringToken>();
                    _tokens.MoveNext();
                    ParseSymbol(SymbolEnum.semicolon);
                }
                else if (((KeywordToken)_tokens.Current).keyword == KeywordEnum.String)
                {
                    ParseKeyword(KeywordEnum.String);
                    _tokens.MoveNext();
                    string variable = ParseChars<StringToken>();
                    _tokens.MoveNext();
                    ParseSymbol(SymbolEnum.semicolon);
                }
                else if (((KeywordToken)_tokens.Current).keyword == KeywordEnum.List)
                {
                    ParseComplexType();
                    _tokens.MoveNext();
                    string variable = ParseChars<StringToken>();
                    _tokens.MoveNext();
                    ParseSymbol(SymbolEnum.semicolon);
                }
                else
                    throw new Exception("Syntax error with unexpect Keyword :" + _tokens.Current.str);
            }
            else if (_tokens.Current is StringToken)
            {
                var idf = ParseChars<StringToken>();
                _tokens.MoveNext();
                string variable = ParseChars<StringToken>();
                _tokens.MoveNext();
                ParseSymbol(SymbolEnum.semicolon);
            }
        }

        private void ParseComplexType()
        {
            ParseKeyword(KeywordEnum.List);
            _tokens.MoveNext();
            if (_tokens.Current is SymbolToken)
            {
                if (((SymbolToken)_tokens.Current).symbol == SymbolEnum.leftanglebracket)
                {
                    ParseSymbol(SymbolEnum.leftanglebracket);
                    _tokens.MoveNext();
                }
                var idf = ParseChars<StringToken>();
                _tokens.MoveNext();
                ParseSymbol(SymbolEnum.rightanglebracket);
            }
            else
                throw new Exception("Syntax error with unexpect token :" + _tokens.Current.str);
        }

        public string ParseChars<T>()
        {
            if (_tokens.Current is T)
                return _tokens.Current.str;
            throw new Exception("Syntax error with unexpect string :" + _tokens.Current.str);
        }

        public void ParseKeyword(KeywordEnum keyword)
        {
            if (((KeywordToken)_tokens.Current).keyword == keyword)
                return;
            throw new Exception("Syntax error with unexpect keyword :" + _tokens.Current.str);
        }

        public Annotation ParseAnnotation()
        {
            ParseSymbol(SymbolEnum.at);
            _tokens.MoveNext();
            var anno = ParseAnnotator();
            _tokens.MoveNext();
            ParseSymbol(SymbolEnum.leftparenthesis);
            _tokens.MoveNext();
            ParsePair(anno);
            _tokens.MoveNext();
            if (_tokens.Current is SymbolToken)
            {
                while (((SymbolToken)_tokens.Current).symbol == SymbolEnum.comma)
                {
                    ParseSymbol(SymbolEnum.comma);
                    _tokens.MoveNext();
                    ParsePair(anno);
                    _tokens.MoveNext();
                }
                if (((SymbolToken)_tokens.Current).symbol == SymbolEnum.rightparenthesis)
                {
                    ParseSymbol(SymbolEnum.rightparenthesis);
                    return anno;
                }
                throw new Exception("Syntax error with unexpect symbol :" + _tokens.Current.str);
            }
            else
                throw new Exception("Syntax error with unexpect keyword :" + _tokens.Current.str);
        }

        private void ParsePair(Annotation anno)
        {
            if (_tokens.Current is KeywordToken)
            {
                if (((KeywordToken)_tokens.Current).keyword == KeywordEnum.name)
                {
                    ParseKeyword(KeywordEnum.name);
                    _tokens.MoveNext();
                    ParseSymbol(SymbolEnum.equal);
                    _tokens.MoveNext();
                    ParseSymbol(SymbolEnum.doublequotation);
                    _tokens.MoveNext();
                    anno.Name = ParseChars<StringToken>();
                    _tokens.MoveNext();
                    ParseSymbol(SymbolEnum.doublequotation);
                }
                else if (((KeywordToken)_tokens.Current).keyword == KeywordEnum.length)
                {
                    ParseKeyword(KeywordEnum.length);
                    _tokens.MoveNext();
                    ParseSymbol(SymbolEnum.equal);
                    _tokens.MoveNext();
                    ParseSymbol(SymbolEnum.doublequotation);
                    _tokens.MoveNext();
                    if (anno is Column)
                        ((Column)anno).Length = ParseNumber();
                    else
                        throw new Exception(" syntax error with unexpect object type");
                    _tokens.MoveNext();
                    ParseSymbol(SymbolEnum.doublequotation);
                }
                else if (((KeywordToken)_tokens.Current).keyword == KeywordEnum.target)
                {
                    ParseKeyword(KeywordEnum.target);
                    _tokens.MoveNext();
                    ParseSymbol(SymbolEnum.equal);
                    _tokens.MoveNext();
                    ParseSymbol(SymbolEnum.doublequotation);
                    _tokens.MoveNext();
                    var varible = ParseChars<StringToken>();
                    if (anno is Many2One)
                    {
                        ((Many2One)anno).Target = varible;
                    }
                    else if (anno is One2Many)
                    {
                        ((One2Many)anno).Target = varible;
                    }
                    else
                        throw new Exception(" syntax error with unexpect object type");
                    _tokens.MoveNext();
                    ParseSymbol(SymbolEnum.doublequotation);
                }
                else if (((KeywordToken)_tokens.Current).keyword == KeywordEnum.mappedBy)
                {
                    ParseKeyword(KeywordEnum.mappedBy);
                    _tokens.MoveNext();
                    ParseSymbol(SymbolEnum.equal);
                    _tokens.MoveNext();
                    ParseSymbol(SymbolEnum.doublequotation);
                    _tokens.MoveNext();
                    var varible = ParseChars<StringToken>();
                    if (anno is One2Many)
                        ((One2Many)anno).MappedBy = varible;
                    else
                        throw new Exception(" syntax error with unexpect object type");
                    _tokens.MoveNext();
                    ParseSymbol(SymbolEnum.doublequotation);
                }
                else
                    throw new Exception("Syntax error with unexpect keyword :" + _tokens.Current.str);
            }
            else
                throw new Exception("Syntax error with unexpect string :" + _tokens.Current.str);
        }

        private int ParseNumber()
        {
            return(_tokens.Current as IntToken).Value;
        }

        private Annotation ParseAnnotator()
        {
            if (_tokens.Current is KeywordToken)
            {
                if (((KeywordToken)_tokens.Current).keyword == KeywordEnum.Table)
                {
                    ParseKeyword(KeywordEnum.Table);
                    var tb = new Table { };
                    tbs.Add(tb);
                    return tb;
                }
                else if (((KeywordToken)_tokens.Current).keyword == KeywordEnum.Id)
                {
                    ParseKeyword(KeywordEnum.Id);
                    return new Id { };
                }
                else if (((KeywordToken)_tokens.Current).keyword == KeywordEnum.Column)
                {
                    ParseKeyword(KeywordEnum.Column);
                    return new Column { };
                }
                else if (((KeywordToken)_tokens.Current).keyword == KeywordEnum.Many2One)
                {
                    ParseKeyword(KeywordEnum.Many2One);
                    return new Many2One { };
                }
                else if (((KeywordToken)_tokens.Current).keyword == KeywordEnum.One2Many)
                {
                    ParseKeyword(KeywordEnum.One2Many);
                    return new One2Many { };
                }
                else
                    throw new Exception("Syntax error with unexpect keyword :" + _tokens.Current.str);
            }
            else
                throw new Exception("Syntax error with unexpect string :" + _tokens.Current.str);
        }

        public void ParseSymbol(SymbolEnum symbol)
        {
            if (_tokens.Current is SymbolToken)
            {
                if (((SymbolToken)_tokens.Current).symbol == symbol)
                    return;
                throw new Exception("Syntax error with unexpect symbol :" + _tokens.Current.str);
            }
            throw new Exception("Syntax error with unexpect token :" + _tokens.Current.str);
        }
    }
}
