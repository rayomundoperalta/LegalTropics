﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Tokenizer
{
    // Tokens that represent the input
    public enum Token
    {
        Numero,
        Palabra,
        Puntuacion,
        PuntoFinal,
        SignoIgual,
        VerticalBar,
        MenosDoble,
        MenosCuadruple,
        ParteARbol,
        Otro       // Represents unrecognized charachters
    }

    // Scanner used to find the tokens from a calc lampda string expression
    public static class LambdaCalcScanner
    {
        // The pattern used with the regular expression class to scan the input
        const string Pattern = @"
                (?'Numero' [01234567890]+)
                (?'Palabra' 
                (?'Password' [\w\W\d\D\S\s^\t\v\b\e\r\f\n\a]{10,} )
                (?'IP' \d{1,3}.\d{1,3}.\d{1,3}.\d{1,3} ) |
                (?'ServerName' [\.]+ ) |
                (?'Palabra' [\x2E\w]+ ) |
                (?'Otro' [^ \r\n\t])";

        // Regular expression used to scan the input
        private static Regex MathRegex = new Regex(Pattern, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.Compiled);

        // Enumurable to get tokens from the given expression (scanner)
        public static IEnumerable<TokenEntity> GetLambdaCalcTokens(this string exp)
        {
            Token[] tokens = Enum.GetValues(typeof(Token)).OfType<Token>().ToArray();
            foreach (Match m in MathRegex.Matches(exp))
            {
                // Check which token is matched by this match object
                foreach (Token token in tokens)
                {
                    if (m.Groups[token.ToString()].Success)
                    {
                        yield return new TokenEntity(
                            token,
                            m.Index,
                            m.Value);
                    }
                }
            }
            // return the end string token, to indecate we are done
            yield return new TokenEntity(Token.Otro, exp.Length, "\0");
        }
    }

    // Holds token info
    public class TokenEntity
    {
        public TokenEntity(Token token, int startPos, string value)
        {
            this.Token = token;
            this.StartPos = startPos;
            this.Value = value;
        }

        // Token type
        public Token Token { get; private set; }

        // Start position in the original string
        public int StartPos { get; private set; }

        // Value
        public string Value { get; private set; }

        public override string ToString()
        {
            return string.Format("{0} at {1}: {2}", Token, StartPos, Value);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            foreach (TokenEntity t in "(a, b) => ; a + b * 0.1".GetLambdaCalcTokens())
            {
                Console.WriteLine(t);
            }

            Console.WriteLine("F I N");
            Console.ReadKey();
        }
    }
}