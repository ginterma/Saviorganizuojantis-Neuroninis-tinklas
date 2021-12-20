using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Saviorganizuojantis_Neuroninis_tinklas
{
    class SOM
    {
        static void Main(string[] args)
        {
            var duomenys = DuomenuNuskaitymas("duomenys.txt"); // dviejų dimensijų masyvas užpildomas priliyginant jį funkcijos gražinamam masyvui. Funkcijai nurodomas duomenų failo pavadinimas.
            for(int i=0; i<duomenys.GetLength(0); i++)
            {
                for(int j=0; j < duomenys.GetLength(1); j++)
                {
                    Console.Write("{0} ", duomenys[i, j]); // išspausdinami pradiniai duomenys
                }
                Console.WriteLine();
            }
            double[,,] som = TinkloMokymas(duomenys, 2, 10, 10);// paleidžiamas mokymo algoritmas
            Console.WriteLine("Neuronų svoriai:");
            for(int i = 0; i < som.GetLength(0); i++)
            {
                for(int j=0; j < som.GetLength(1); j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        Console.Write("{0} ", som[i, j,k]); // išspausdinami gautų neuronų svoriai
                    }
                    Console.WriteLine();
                }
                
            }
        }
        public static double[,,] TinkloMokymas(double[,] duomenys, int epochos, int x_dydis, int y_dydis) // pagrindinis mokymo algoritmas. Argumentai paduodami - duomenų dvimatis masyvas, epochų skaičius
                                                                                                          // neuronų eilučių ir stulpeliu skaičius. Funkcija gražina trijų dimensijų neuronų svorių masyvą.
        {   Random random = new Random();
            double[,,] som_tinklas = new double[x_dydis, y_dydis,4]; // sukuriamas trijų dimensijų masyvas.
            double[] atstumai = new double[x_dydis * y_dydis]; // sukuriamas masyvas kuriame bus saugomi atstumai tarp duomenų ir neuronų svorių.
            int x_nugaletojas = 0; // būsimo neurono nugalėtojo x koordinatės inicializacija.
            int y_nugaletojas = 0; // būsimo neurono nugalėtojo y koordinatės inicializija.
            double atstumu_sumos = 0; // atstumų sumų inicializija.
            int[,,] atvaizdavimo_lentele = new int[x_dydis, y_dydis, duomenys.GetLength(0)]; // lentelė duomenims atvaizduoti.
            double kvantavimo_paklaida; // kvantavimo klaidos inicializacija.
            for(int i=0; i<x_dydis; i++)
            {
                for(int j=0; j < y_dydis; j++)
                {
                    Console.Write("Atsitiktiniai svoriai: ");
                    for (int k = 0; k < 4; k++)
                    {
                        som_tinklas[i, j, k] = random.NextDouble(); // sugeneruojami atsitiktiniai pradiniai tinklo svoriai. Svorių reikšmės yra [0;1].
                        Console.Write("{0} ", som_tinklas[i, j, k]); // išspausdinamos reikšmės.
                    }
                    Console.WriteLine("");
                }
                
            }
            for (int k = 1; k < epochos+1; k++) // epochų ciklas
            {
                for (int i = 0; i < duomenys.GetLength(0); i++) // kiekvieno duomenų vektoriaus ciklas.
                {
                    int index = 0; // indeksas skirtas sekti ir išsaugoti duomenų ir neuronų svorių atstumus.
                    int minimum = 0;
                    
                        
                        for(int c = 0; c < som_tinklas.GetLength(0); c++) // ciklas eina per neuronų eilutes.
                    {     
                            for (int p = 0; p<som_tinklas.GetLength(1); p++) // ciklas eina per neuronų stulpelius.
                            {
                            atstumai[index] = 0;
                                for (int j = 0; j < duomenys.GetLength(1); j++) // ciklas eina per neuronų ir duomenų svorius.
                                {
                                atstumai[index] = atstumai[index] + Math.Pow((som_tinklas[c, p, j] - duomenys[i, j]),2); //skaičiuojama neuronų svorių ir duomenų svorių skirtumų kvadratų sumos.
                                }
                            atstumai[index] = Math.Sqrt(atstumai[index]); // apskaičiuojamas atstumas tarp svorių.
                            //Console.WriteLine(atstumai[index]);
                            index++;
                           
                        
                            }
                        
                        }

                        
                    index = 0;
                    for (int j = 0; j < atstumai.GetLength(0); j++) // ciklas skirtas rasti mažiausią atstumą.
                    {
                        if (atstumai[j] == atstumai.Min())
                        {
                            minimum = j;
                        }
                    }
                        int index2 = 0;
                        for(int c = 0; c< som_tinklas.GetLength(0); c++) // ciklai eina per neuronų stulpelius ir eilutes ir ieško neurono laimėtojo koordinačių.
                        {
                            for(int p = 0; p <som_tinklas.GetLength(1); p++)
                            {
                                if (index2 == minimum) 
                                {
                                 x_nugaletojas = c; // neurono laimėtojo x koordinatė.
                                 y_nugaletojas = p; // neurono nugalėtojo y koordinatė.
                                }
                            index2++;
                            }
                        }
                    for(int c = 0; c < som_tinklas.GetLength(0); c++) // dar kartą einama per neurono stulpelius ir eilutes
                    {
                        for(int p = 0; p < som_tinklas.GetLength(1); p++)
                        {
                            double svoriu_suma;
                            double[] neurono_svoriai = new double[4];
                            double[] laimetojo_svoriai = new double[4];
                            for(int b = 0; b<4; b++)
                            {
                                neurono_svoriai[b] = som_tinklas[c, p, b]; // kiekvieno neurono svoriai sudedami į atskirą masyvą, kad būtų galima paskaičiuoti einamojo neurono svorių ir laimetojo neurono svorių atsutumą.
                                laimetojo_svoriai[b] = som_tinklas[x_nugaletojas, y_nugaletojas, b]; // laimėtojo neurono svoriai sudedami į atskirą masyvą irgi.

                            }
                            svoriu_suma = SvoriuAtstumoSkaiciavimas(neurono_svoriai, laimetojo_svoriai); // paskaičiuojama svorių suma.
                            double atstumas = KaimynystesAtstumoSkaiciavimas(c, p, x_nugaletojas, y_nugaletojas); // paskaičiuojama SOM kaimynystės eilė.
                            double mokymo_greitis;
                            if (atstumas == 0) // jei einamasis neuronas yra laimėtojas neuronas, tai svoriai nekeičiami.
                            {
                                mokymo_greitis = 0;
                            }
                            else // Apskaičiuojamas mokymo greitis tam tikram neuronui.
                            {
                                mokymo_greitis = (1 / (i + 1)) * Math.Exp(((-1) * Math.Pow(svoriu_suma, 2)) / (2 * Math.Pow(atstumas, 2)));
                            }
                            
                            for (int j = 0; j < 4; j++)
                            {
                                som_tinklas[c, p,j] = som_tinklas[c, p,j] + mokymo_greitis * (duomenys[i,j] - som_tinklas[c,p,j]); // SOM mokymo taisyklė.
                            }
                            index++;
                        }
                    }

                    for (int m = 0; m < atstumai.GetLength(0); m++)
                    {
                        atstumai[m] = 0; // nunulinamos atstumų sumos.
                    }

                    
                    if (k == epochos) // jei tai yra paskutinės epocha, išspausdinti neuroną laimėtoją ir einamojo duomens numerį.
                    {   for(int j=0; j<4; j++)
                        {
                            atstumu_sumos = atstumu_sumos + Math.Abs(duomenys[i, j] - som_tinklas[x_nugaletojas, y_nugaletojas, j]);
                        }
                        Console.WriteLine("Neorono nugaletėtojo koordinate x: {0} koordinate y: {1}, jam priklausančio duomens numeris: {2}", x_nugaletojas+1, y_nugaletojas+1, i + 1);
                    }
                }
            }
            kvantavimo_paklaida = atstumu_sumos / duomenys.GetLength(0); // skaičiuojama kvantavimo paklaida.
            Console.WriteLine("Kvantavimo paklaida: {0}", kvantavimo_paklaida); // spausdinama kvantavimo paklaida.
                
                return som_tinklas;
        }
       static public double KaimynystesAtstumoSkaiciavimas(int neuronas_x, int neuronas_y, int laimetojas_x, int laimetojas_y) //Funkcija skaičiuoja kaimynystės eilę, argumentus paima einamojo ir neurono laimėtojo koordinates.
            // funckija gražina SOM kaimynystės eilės skaičių.
        {
            return Math.Sqrt(Math.Pow(neuronas_x - laimetojas_x, 2) + Math.Pow(neuronas_y - laimetojas_y, 2));
        }
        static public double SvoriuAtstumoSkaiciavimas(double[] neuronas, double[] neuronas_laimetojas) // funkcija skaičiuoja einamojo neurono ir neurono laimėtojo svorių Euklidinį atstumą.
        {
            double suma = 0;
            for(int i=0; i<4; i++)
            {
                suma = suma + Math.Pow((neuronas[i] - neuronas_laimetojas[i]), 2);
            }

            suma = Math.Sqrt(suma);
            return suma;
        }
        public static double[,] DuomenuNuskaitymas(string failo_pavadinimas) // duomenų nuskaitymo funkcija, argumentą priima duomenų failo pavadinimą ir gražina užpildyta masyvą su duomenimis iš failo.
        {
            var index0 = 0;
            var index1 = 0;
            var duomenu_sarasas = new List<double>();
            var failo_vieta = @"C:\Users\PC\source\repos\Saviorganizuojantis Neuroninis tinklas\Saviorganizuojantis Neuroninis tinklas\" + // nurodoma kur yra duomenų failas.
                              failo_pavadinimas; 
            var lines = File.ReadAllLines(failo_vieta); // nuskaitomas visos duomenų failo eilutės
            var pirmas_duomenu_rinkinys = new double[lines.Length - 1, 4]; // sukuriamas dvimatis masyvas, duomenims laikyti.
            foreach (var line in lines)
            {
                if (index1 == lines.Length - 1) break;
                index0 = 0;
                duomenu_sarasas = line.Split(',').Select(double.Parse).ToList(); // kiekvienas duomenų elementas yra atskiriamas ir konvertuojamas į double tipą.
                for (var i = 0; i < 4; i++) pirmas_duomenu_rinkinys[index1, i] = duomenu_sarasas[i]; // užpildomas masyvas duomenimis
                index1++;
                duomenu_sarasas.Clear();
            }

            return pirmas_duomenu_rinkinys; // gražinamas užpildytas masyvas su duomenimis
        }
    }



}
