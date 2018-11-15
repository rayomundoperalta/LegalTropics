using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PrettyPrint
{
    // Tokens that represent the input
    internal enum Token
    {
        LPar,
        RPar,
        LLlave,
        RLlave,
        POS,
        Numero,
        Other       // Represents unrecognized charachters
    }

    // Scanner used to find the tokens from a calc lampda string expression
    internal static class Scanner
    {
        // The pattern used with the regular expression class to scan the input
        const string Pattern = @"
                        (?'LPar' \( ) |
                        (?'RPar' \) ) |
                        (?'LLlave' \{ ) |
                        (?'RLlave' \} ) |
                        (?'POS' [a-z]{1}[a-z\-]*: ) |
                        (?'Numero' [0-9]+ ) |
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
    public class PrettyPrintCompiler
    {
        TokenEntity lastToken;
        // Holds the tokens enumurator
        IEnumerator<TokenEntity> tokens;
        public string Result = string.Empty;
        public List<string> POSTAGS;
        public bool Success = false;

        /// <summary>
        /// Initialize the class from the given string expression
        /// </summary>
        /// <param name="exp">Input expression, ie: (a, b) => a + b * 2</param>
        public PrettyPrintCompiler(string exp, List<string> postags)
        {
            POSTAGS = postags;
            Console.WriteLine("PrettyPrint Compiler");
            //Console.WriteLine("PrettyPrint Dump------" + exp + "----\n");

            this.tokens = exp.GetTokens().GetEnumerator();
            AdvanceToken();

            Result = exp + "\n";
            ParentesisLastToken = false;
            Success = PrettyPrint();
            //Console.WriteLine("C o m p i l a d o");
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
            //Console.WriteLine("CT-> " + CurrentToken.Token + " =? " + token + "(" + CurrentToken.Value + ")");
            if (CurrentToken != null && CurrentToken.Token == token)
            {
                AdvanceToken();
                return true;
            }
            return false;
        }

        string BlankStack = string.Empty;
        bool ParentesisLastToken;
        int LeafPosTag;

        private bool PrettyPrint()
        {
            if (CheckToken(Token.LPar))
            {
                //Console.Write(LastToken.Value);
                if (ParentesisLastToken)
                {
                    Result += BlankStack + LastToken.Value + '\n'.ToString();
                    ParentesisLastToken = false;
                }
                else
                {
                    Result += '\n'.ToString() + BlankStack + LastToken.Value + '\n'.ToString();
                }
                BlankStack += "--+";
                Result += BlankStack;
                if (CheckToken(Token.POS))
                {
                    //Console.Write(LastToken.Value);
                    bool imprimePOS = false;
                    bool verboAuxiliar = false;
                    ParentesisLastToken = false;
                    Result += LastToken.Value;
                    switch (LastToken.Value)
                    {
                        case "sp-de:":
                            imprimePOS = true;
                            break;
                        case "verb:":
                            verboAuxiliar = true;
                            break;
                        default:
                            break;
                    }
                    
                    if (CheckToken(Token.Numero))
                    {
                        //Console.Write(LastToken.Value);
                        bool HayHijos = false;
                        Result += LastToken.Value;
                        LeafPosTag = Int32.Parse(LastToken.Value);
                        if (imprimePOS)
                        {
                            imprimePOS = false;
                            Result += " " + POSTAGS[LeafPosTag - 1];
                        }
                        
                        if (CheckToken(Token.LLlave))
                        {
                            if (CheckToken(Token.Numero))
                            {
                                //Console.WriteLine("Detectamos renglón " + LastToken.Value);
                                if (verboAuxiliar)
                                {
                                    verboAuxiliar = false;
                                    if (LeafPosTag != Int32.Parse(LastToken.Value))
                                    {
                                        Result += " " + POSTAGS[Int32.Parse(LastToken.Value) - 1];
                                    }
                                }
                                if (!CheckToken(Token.RLlave))
                                {
                                    Console.WriteLine("6 Esperabamos una llave");
                                    return false;
                                }
                            }
                            else
                            {
                                Console.WriteLine("5 Esperabamos un numero");
                                return false;
                            }
                        }
                        while (!CheckToken(Token.RPar))
                        {
                            HayHijos = true;
                            if (!PrettyPrint()) return false;
                        }
                        //Console.Write(LastToken.Value);
                        if (!HayHijos)
                        {
                            Result += " " + POSTAGS[LeafPosTag - 1];
                        }
                        BlankStack = BlankStack.Remove(0, 3);
                        Result += '\n'.ToString() + BlankStack + LastToken.Value + '\n'.ToString();
                        ParentesisLastToken = true;
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Esperabamos un Numero");
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("Esperabamos un POS Tag");
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Esperabamos un '('");
                return false;
            }
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

    /*
    class Program
    {
        static void Main(string[] args)
        {
            Compilador lc = new Compilador(@"1  Será            ser             VSIF3S0 VSI pos=verb|type=semiauxiliary|mood=indicative|tense=future|person=3|num=singular - - (grup-verb:1(verb:1)                              - - - -
2  ,               ,               Fc      Fc  pos=punctuation|type=comma                                                     - - (Fc:2)                                            - - - -
3  responsabilidad responsabilidad NCFS000 NC  pos=noun|type=common|gen=feminine|num=singular                                 - - (sn:3(grup-nom-fs:3(n-fs:3))                      - - - -
4  de              de              SP      SP  pos=adposition|type=preposition                                                - - (sp-de:4                                          - - - -
5  el              el              DA0MS0  DA  pos=determiner|type=article|gen=masculine|num=singular                         - - (sn:6(espec-ms:5(j-ms:5))                         - - - -
6  proveedor       proveedor       NCMS000 NC  pos=noun|type=common|gen=masculine|num=singular                                - - (grup-nom-ms:6(n-ms:6)                            - - - -
7  asignado        asignar         VMP00SM VMP pos=verb|type=main|mood=participle|num=singular|gen=masculine                  - - (s-a-ms:7(parti-ms:7))))))                        - - - -
8  entregar        entregar        VMN0000 VMN pos=verb|type=main|mood=infinitive                                             - - (grup-verb-inf:8(infinitiu:8(inf:8(forma-inf:8))) - - - -
9  la              el              DA0FS0  DA  pos=determiner|type=article|gen=feminine|num=singular                          - - (sn:10(espec-fs:9(j-fs:9))                        - - - -
10 lista           lista           NCFS000 NC  pos=noun|type=common|gen=feminine|num=singular                                 - - (grup-nom-fs:10(n-fs:10))                         - - - -
11 de              de              SP      SP  pos=adposition|type=preposition                                                - - (sp-de:11                                         - - - -
12 asistencia      asistencia      NCFS000 NC  pos=noun|type=common|gen=feminine|num=singular                                 - - (sn:12(grup-nom-fs:12(n-fs:12))))))               - - - -
13 semanalmente    semanalmente    RG      RG  pos=adverb|type=general                                                        - - (sadv:13)                                         - - - -
14 .               .               Fp      Fp  pos=punctuation|type=period                                                    - - (F-term:14))                                      - - - -
");
            Console.WriteLine(lc.result1 + "\n");
            int line = 1;
            foreach (string postag in lc.POSTAGS)
            {
                Console.WriteLine(line++ + " " + postag);
            }
            Console.ReadKey();
        }
    }
    */
}

/*
(grup-verb:2(sn:1(grup-nom-ms:1(w-ms:1))) (verb:2) (sn:3(grup-nom-ms:3(n-ms:3)) (sp-de:4 (sn:6(espec-fs:5(j-fs:5)) (grup-nom-fs:6(n-fs:6)))) (sp-de:7 (sn:8(grup-nom-fp:8(n-fp:8) (s-a-fp:9(a-fp:9))))) (relativa:11(relatiu-sn:10) (verb:11) (sn:13(espec-fs:12(j-fs:12)) (grup-nom-fs:13(n-fs:13)) (sp-de:14 (sn:16(espec-mp:15(j-mp:15)) (grup-nom-mp:16(n-mp:16) (s-a-mp:17(parti-mp:17)))))) (grup-sp:18(prep:18) (sn:19(grup-nom-ms:19(w-ms:19)))) (grup-sp:20(prep:20) (sn:22(espec-ms:21(j-ms:21)) (grup-nom-ms:22(n-ms:22) (n-fs:23)))) 
 */
