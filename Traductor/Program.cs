using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Collections;
using System.Globalization;
using System.Threading;
using System.ComponentModel;
using PrettyPrint;


namespace Traductor
{
    // Tokens that represent the input
    internal enum Token
    {
        Numero,
        Palabra,
        SignoIgual,
        BarraVertical,
        ParteArbol,
        Puntuacion,
        Punto,
        Other       // Represents unrecognized charachters
    }

    // Scanner used to find the tokens from a calc lampda string expression
    internal static class Scanner
    {
        // The pattern used with the regular expression class to scan the input
        const string Pattern = @"
                        (?'Numero' [0-9]+ ) |
                        (?'Palabra' [a-zA-ZáéíóúÁÉÍÓÚÑñüÜÇç]{1}[a-zA-ZáéíóúÁÉÍÓÚÑñüÜÇç0-9_]* ) |
                        (?'SignoIgual' = ) |
                        (?'BarraVertical' \| ) |
                        (?'ParteArbol' -\ -\ [\(]{1}[a-zA-Z0-9\ \(\):-]+-\ -\ -\ - ) |
                        (?'Puntuacion' [,;:¿?¡!-_]{1}  ) |
                        (?'Punto' \.) |
                        (?'Other' [^ \r\n\t])";

        // Regular expression used to scan the input
        private static Regex MathRegex = new Regex(Pattern, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.Compiled);

        // Enumerable to get tokens from the given expression (scanner)
        public static IEnumerable<TokenEntity> GetTokens(this string exp)
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
            // return the end string token, to indicate we are done
            yield return new TokenEntity(Token.Other, exp.Length, "\0");
        }
    }

    // Holds token info
    internal class TokenEntity
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
            return string.Format("{0} {1}", Token, Value);
        }
    }

    /// <summary>
    /// Lambda expression calculation class
    /// </summary>
    public class Compilador
    {
        TokenEntity lastToken;
        // Holds the tokens enumurator
        IEnumerator<TokenEntity> tokens;
        public string result;
        public string result1;
        public string result2;
        public string ArbolNuevo;
        public List<string> POSTAGS = new List<string>();

        /// <summary>
        /// Initialize the class from the given string expression
        /// </summary>
        /// <param name="exp">Input expression, ie: (a, b) => a + b * 2</param>
        public Compilador(string exp)
        {
            Console.WriteLine("Compilamos la salida de FreeLing para convertirla en una estructura manejable");
            // Console.WriteLine(exp);
            result = exp;

            this.tokens = exp.GetTokens().GetEnumerator();
            AdvanceToken();

            if (ParseFreeLing())
            {
                //Console.Write("Antes de prettyprint >>>>" + ArbolNuevo + "<<<<<<<\n");
                PrettyPrintCompiler lc = new PrettyPrintCompiler(ArbolNuevo, POSTAGS);
                if (lc.Success)
                {
                    result1 = lc.Result;
                }
                else
                {
                    result1 = "PrettyPrint compilation error";
                }
                
                Console.WriteLine("C o m p i l a d o");
            }
            else
            {
                Console.WriteLine("Parse Freeling Error de Compilación");
                result1 = "Parse Freeling Error de Compilacion";
            }
        }

        private TokenEntity CurrentToken { get { return this.tokens.Current; } }
        private TokenEntity LastToken { get { return this.lastToken; } }

        private bool AdvanceToken()
        {
            this.lastToken = CurrentToken;
            return this.tokens.MoveNext();
        }

        private bool CheckToken(Token token)
        {
            // SOLO AVANZA EL PARSER SI ENCONTRÓ EL TOKEN
            if (CurrentToken != null && CurrentToken.Token == token)
            {
                //Console.Write(CurrentToken.Value);
                AdvanceToken();
                return true;
            }
            return false;
        }

        private bool ParseFreeLing()
        {
            const string ArbolPattern = @"
                        (?'LPar' \( ) |
                        (?'RPar' \) ) |
                        (?'POS' [a-z]{1}[a-z\-]*: ) |
                        (?'Numero' [0-9]+ ) |
                        (?'Other' [^ \r\n\t])";

            Regex RegexParteArbol = new Regex(ArbolPattern, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.Compiled);

            Console.WriteLine("Parse FreeLing");
            string Numero = string.Empty;
            string Palabra = string.Empty;
            string Lemma = string.Empty;
            string Tag1 = string.Empty;
            string Tag2 = string.Empty;
            string POS = string.Empty;
            string Type = string.Empty;

            string Mood = string.Empty;
            string Tense = string.Empty;
            string Person = string.Empty;
            string Num = string.Empty;
            string Gen = string.Empty;
            string Possessornum = string.Empty;
            string ParteArbol = string.Empty;
            string NuevoArbol = string.Empty;

            while (CheckToken(Token.Numero))
            {
                Numero = LastToken.Value;
                if (CheckToken(Token.Palabra))
                {
                    Palabra = LastToken.Value;
                }
                else
                {
                    if (CheckToken(Token.Puntuacion))
                    {
                        Palabra = LastToken.Value;
                    }
                    else
                    {
                        if (CheckToken(Token.Punto))
                        {
                            Palabra = LastToken.Value;
                        }
                        else
                        {
                            if (CheckToken(Token.Numero))
                            {
                                Palabra = LastToken.Value;
                            }
                            else
                            {
                                Console.WriteLine("\n1 Esperamos una palabra, puntuación o punto: {" + LastToken.Value + " " + LastToken.Token + "}");
                                return false;
                            }
                        }
                    }
                }

                if (CheckToken(Token.Palabra))
                {
                    Lemma = LastToken.Value;
                }
                else
                {
                    if (CheckToken(Token.Puntuacion))
                    {
                        Lemma = LastToken.Value;
                    }
                    else
                    {
                        if (CheckToken(Token.Punto))
                        {
                            Lemma = LastToken.Value;
                        }
                        else
                        {
                            if (CheckToken(Token.Numero))
                            {
                                Lemma = LastToken.Value;
                            }
                            else
                            {
                                Console.WriteLine("2 Esperamos una palabra, puntuación o punto: " + LastToken.Value + " " + LastToken.Token);
                                return false;
                            }
                        }
                    }
                }
                //Console.WriteLine("??? Current Token {" + CurrentToken.Token + " " + CurrentToken.Value + "}");
                if (CheckToken(Token.Palabra))
                {
                    Tag1 = LastToken.Value;
                }
                else
                {
                    Console.WriteLine("3 Esperamos una palabra: {" + LastToken.Token + " " + LastToken.Value + "}");
                    return false;
                }

                if (CheckToken(Token.Palabra))
                {
                    Tag2 = LastToken.Value;
                }
                else
                {
                    Console.WriteLine("4 Esperamos una palabra: " + LastToken.Value + " " + LastToken.Token);
                    return false;
                }

                while (CheckToken(Token.Palabra))
                {
                    string clase = LastToken.Value;
                    //Console.WriteLine("debe ser una palabra " + clase);
                    string valor = string.Empty;
                    if (CheckToken(Token.SignoIgual))
                    {
                        if (CheckToken(Token.Palabra) | CheckToken(Token.Numero))
                        {
                            valor = LastToken.Value;
                            //Console.WriteLine("Debe ser un valor " + valor);
                        }
                        else
                        {
                            //Console.WriteLine("Word expected");
                            return false;
                        }
                    }
                    else
                    {
                        //Console.WriteLine("'=' expected");
                        return false;
                    }

                    switch (clase)
                    {
                        case "pos":
                            POS = valor;
                            break;
                        case "type":
                            Type = valor;
                            break;
                        case "mood":
                            Mood = valor;
                            break;
                        case "tense":
                            Tense = valor;
                            break;
                        case "person":
                            Person = valor;
                            break;
                        case "num":
                            Num = valor;
                            break;
                        case "gen":
                            Gen = valor;
                            break;
                        case "possessornum":
                            Possessornum = valor;
                            break;
                        default:
                            Console.WriteLine("Token value unkown " + valor);
                            return false;
                            
                    }
                    //Console.WriteLine("---> " + clase + " " + valor);
                    if (!CheckToken(Token.BarraVertical))
                    {
                        continue;
                    }
                }

                if (CheckToken(Token.ParteArbol))
                {
                    ParteArbol = Regex.Replace(Regex.Replace(LastToken.Value, @"-\ -\ ", @""), @"[\ ]*-\ -", "");
                    MatchCollection AllMatches = RegexParteArbol.Matches(ParteArbol);
                    if (AllMatches.Count > 0)
                    {
                        int i = 1;
                        //Console.WriteLine("-> " + ParteArbol);
                        foreach(Match someMatch in AllMatches)
                        {
                            //Console.Write(someMatch.Value);
                            NuevoArbol += someMatch.Value;
                            if (i == 3)
                            {
                                //Console.Write(" {" + Numero + "} ");
                                NuevoArbol += " {" + Numero + "} ";
                            }
                            i++;
                        }
                        //Console.WriteLine();
                    }
                    else
                    {
                        return false;
                    }
                    //Console.WriteLine("ParteArbol -> " + ParteArbol + "\n");
                }
                else
                {
                    Console.WriteLine("Se esperaba un segmento de árbol");
                    return false;
                }

                ArbolNuevo += NuevoArbol + " ";
                result1 += ParteArbol + " ";
                result2 += Numero + " " + Palabra + " " + Lemma + " " + Tag1 + " " + Tag2 + " " + POS + " " + Type + " " +
                                  Mood + " " + Tense + " " + Person + " " + Num + " " + Gen + "\n";
                POSTAGS.Add(Palabra + " " + Lemma + " " + Tag1 + " " + Tag2 + " " + POS + " " + Type + " " +
                                  Mood + " " + Tense + " " + Person + " " + Num + " " + Gen);

                Numero = string.Empty;
                Palabra = string.Empty;
                Lemma = string.Empty;
                Tag1 = string.Empty;
                Tag2 = string.Empty;
                POS = string.Empty;
                Type = string.Empty;
                Mood = string.Empty;
                Tense = string.Empty;
                Person = string.Empty;
                Num = string.Empty;
                Gen = string.Empty;
                Possessornum = string.Empty;
                ParteArbol = string.Empty;
                NuevoArbol = string.Empty;
            }
            return true;
        }

        // Returns back an error exception
        private Exception GetErrorException(string p, TokenEntity tokenEntity)
        {
            return new Exception(string.Format("Error at '{0}': {1}", tokenEntity != null ? tokenEntity.StartPos : 0, p));
        }

        private static void DisplaySet(HashSet<string> set)
        {
            Console.Write("{");
            foreach (string str in set)
            {
                Console.Write(" {0}", str);
            }
            Console.WriteLine(" }");
        }

        private static string SetToString(HashSet<string> set)
        {
            string result = "{";
            foreach (string str in set)
            {
                result += " " + str;
            }
            result += " } ";
            return result;
        }
    }
}
