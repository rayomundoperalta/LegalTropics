using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Traductor;

namespace TestTraductor
{
    class Program
    {
        static void Main(string[] args)
        {
            Compilador lc = new Compilador(@"
1  Nuestra    nuestro    DP1FSP  DP  pos=determiner|type=possessive|person=1|gen=feminine|num=singular|possessornum=plural - - (S:0(sn:2(espec-fs:1(pos-fs:1)) - - - -
2  Agrupación agrupación NP00000 NP  pos=noun|type=proper                                                                  - - (grup-nom-fs:2(w-fs:2)))        - - - -
3  llevará    llevar     VMIF3S0 VMI pos=verb|type=main|mood=indicative|tense=future|person=3|num=singular                 - - (grup-verb:3(verb:3))           - - - -
4  el         el         DA0MS0  DA  pos=determiner|type=article|gen=masculine|num=singular                                - - (sn:5(espec-ms:4(j-ms:4))       - - - -
5  control    control    NCMS000 NC  pos=noun|type=common|gen=masculine|num=singular                                       - - (grup-nom-ms:5(n-ms:5)))        - - - -
6  de         de         SP      SP  pos=adposition|type=preposition                                                       - - (sp-de:6                        - - - -
7  asistencia asistencia NCFS000 NC  pos=noun|type=common|gen=feminine|num=singular                                        - - (sn:7(grup-nom-fs:7(n-fs:7))))  - - - -
8  de         de         SP      SP  pos=adposition|type=preposition                                                       - - (sp-de:8                        - - - -
9  el         el         DA0MS0  DA  pos=determiner|type=article|gen=masculine|num=singular                                - - (sn:10(espec-ms:9(j-ms:9))      - - - -
10 personal   personal   NCMS000 NC  pos=noun|type=common|gen=masculine|num=singular                                       - - (grup-nom-ms:10(n-ms:10)        - - - -
11 contratado contratar  VMP00SM VMP pos=verb|type=main|mood=participle|num=singular|gen=masculine                         - - (s-a-ms:11(parti-ms:11)))))     - - - -
12 .          .          Fp      Fp  pos=punctuation|type=period                                                           - - (F-term:12))                    - - - -

");
            //Console.WriteLine(lc.result + "\n");
            Console.WriteLine(lc.result1 + "\n");
            int line = 1;
            foreach (string postag in lc.POSTAGS)
            {
                Console.WriteLine(line++ + " " + postag);
            }
            Console.ReadKey();
        }
    }
}
