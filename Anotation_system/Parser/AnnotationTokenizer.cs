using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Annotation_system.Enums;
using Annotation_system.Annotations;

namespace Annotation_system.Parser
{
    class AnnotationTokenizer
    {
        private StringReader _reader;
        private string text;

        public AnnotationTokenizer(string text)
        {
            this.text = text;
            _reader = new StringReader(text);
        }

        public IEnumerable<Token> Tokenize()
        {
            var tokens = new List<Token>();
            while (_reader.Peek() != -1)
            {
                while (Char.IsWhiteSpace((char)_reader.Peek()))
                {
                    _reader.Read();
                }

                if (_reader.Peek() == -1)
                    break;
                var c = (char)_reader.Peek();
                if (Char.IsLetter(c))
                {
                    var token = ParseKeyword();
                    tokens.Add(token);
                }
                else if (Char.IsDigit(c))
                {
                    var nr = ParseNumber();
                    tokens.Add(new IntToken(nr));
                }
                else if (c == '@')
                {
                    tokens.Add(new SymbolToken { symbol = SymbolEnum.at, str = c.ToString() });
                    _reader.Read();
                }
                else if (c == ',')
                {
                    tokens.Add(new SymbolToken { symbol = SymbolEnum.comma, str = c.ToString() });
                    _reader.Read();
                }
                else if (c == ';')
                {
                    tokens.Add(new SymbolToken { symbol = SymbolEnum.semicolon, str = c.ToString() });
                    _reader.Read();
                }
                else if (c == '=')
                {
                    tokens.Add(new SymbolToken { symbol = SymbolEnum.equal, str = c.ToString() });
                    _reader.Read();
                }
                else if (c == '(')
                {
                    tokens.Add(new SymbolToken { symbol = SymbolEnum.leftparenthesis, str = c.ToString() });
                    _reader.Read();
                }
                else if (c == ')')
                {
                    tokens.Add(new SymbolToken { symbol = SymbolEnum.rightparenthesis, str = c.ToString() });
                    _reader.Read();
                }
                else if (c == '{')
                {
                    tokens.Add(new SymbolToken { symbol = SymbolEnum.leftbrace, str = c.ToString() });
                    _reader.Read();
                }
                else if (c == '}')
                {
                    tokens.Add(new SymbolToken { symbol = SymbolEnum.rightbrace, str = c.ToString() });
                    _reader.Read();
                }
                else if (c == '<')
                {
                    tokens.Add(new SymbolToken { symbol = SymbolEnum.leftanglebracket, str = c.ToString() });
                    _reader.Read();
                }
                else if (c == '>')
                {
                    tokens.Add(new SymbolToken { symbol = SymbolEnum.rightanglebracket, str = c.ToString() });
                    _reader.Read();
                }
                else if (c == '"')
                {
                    tokens.Add(new SymbolToken { symbol = SymbolEnum.doublequotation, str = c.ToString() });
                    _reader.Read();
                }
                else
                    throw new Exception("Unknown character in text: " + c);
            }
            return tokens;
        }

        private int ParseNumber()
        {
            var digits = new List<int>();
            while (Char.IsDigit((char)_reader.Peek()))
            {
                var digit = (char)_reader.Read();
                int i;
                if (int.TryParse(Char.ToString(digit), out i))
                {
                    digits.Add(i);
                }
                else
                    throw new Exception("Could not parse integer number when parsing digit: " + digit);
            }

            var nr = 0;
            var mul = 1;

            digits.Reverse();
            digits.ForEach(d =>
            {
                nr += d * mul;
                mul *= 10;
            });

            return nr;
        }

        private Token ParseKeyword()
        {
            var text = new StringBuilder();
            bool IsVar = false;
            while (Char.IsLetter((char)_reader.Peek()) || Char.IsDigit((char)_reader.Peek()))
            {
                text.Append((char)_reader.Read());
                if (Char.IsDigit((char)_reader.Peek()))
                {
                    IsVar = true;
                }
                else
                    IsVar = false;
            }

            var potentialKeyword = text.ToString();

            if (!IsVar)
            {
                var kw = new KeywordToken { str = potentialKeyword };
                switch (potentialKeyword)
                {
                    case "public":
                        kw.keyword = KeywordEnum._public;
                        break;
                    case "interface":
                        kw.keyword = KeywordEnum._interface;
                        break;
                    case "Table":
                        kw.keyword = KeywordEnum.Table;
                        break;
                    case "Id":
                        kw.keyword = KeywordEnum.Id;
                        break;
                    case "Column":
                        kw.keyword = KeywordEnum.Column;
                        break;
                    case "Many2One":
                        kw.keyword = KeywordEnum.Many2One;
                        break;
                    case "One2Many":
                        kw.keyword = KeywordEnum.One2Many;
                        break;
                    case "name":
                        kw.keyword = KeywordEnum.name;
                        break;
                    case "length":
                        kw.keyword = KeywordEnum.length;
                        break;
                    case "target":
                        kw.keyword = KeywordEnum.target;
                        break;
                    case "mappedBy":
                        kw.keyword = KeywordEnum.mappedBy;
                        break;
                    case "Integer":
                        kw.keyword = KeywordEnum.Integer;
                        break;
                    case "String":
                        kw.keyword = KeywordEnum.String;
                        break;
                    case "List":
                        kw.keyword = KeywordEnum.List;
                        break;
                    default:
                        return new CharsToken { str = text.ToString() };
                }
                return kw;
            }
            else
            {
                if (char.IsUpper(text[0]))
                    return new IdentifierToken { str = text.ToString() };
                else
                    return new VariableToken { str = text.ToString() };
            }
        }
    }
}
