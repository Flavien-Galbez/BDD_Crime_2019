using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Xml;
using System.IO;

namespace BDD_Flavien_Projet
{
    class Program
    {
        static void Main(string[] args)
        {
            bool fin = false;
            bool valid = true;
            string lecture = "";
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=ny_crimes;USER=esilvs6;PASSWORD=esilvs6;";
            MySqlConnection connection = new MySqlConnection(connectionString);

            //Menu interactif
            //---------------
            do
            {
                fin = false;
                //
                Console.WriteLine();
                Console.WriteLine("1 : Importer une journée de crimes");
                Console.WriteLine("2 : Exporter le bilan journalier");
                Console.WriteLine("3 : Saisie d'un crime");
                Console.WriteLine("4 : Le nombre de crimes par quartier et par catégorie");
                Console.WriteLine("5 : Récapitulatif pour un mois");
                Console.WriteLine("6 : Evolution mois par mois du grand larcin par quartier");
                Console.WriteLine("7 : Palmarès annuel");
                Console.WriteLine("8 : Evolution mois par mois du pourcentage de cambriolages commis le jour contre ceux commis la nuit (fonction ajoutée)");
                Console.WriteLine("9 : fin");
                //
                do
                {
                    lecture = "";
                    valid = true;

                    Console.Write("\nchoisissez un programme > ");
                    lecture = Console.ReadLine();
                    Console.WriteLine(lecture);
                    if (lecture == "" || !"123456789".Contains(lecture[0]))
                    {
                        Console.WriteLine("votre choix <" + lecture + "> n'est pas valide = > recommencez ");
                        valid = false;
                    }
                } while (!valid);
                //
                //
                switch (lecture[0])
                {
                    case '1':
                        Console.Clear();
                        InsertionJournee(connection);
                        break;
                    case '2':
                        Console.Clear();
                        ExporterBilanJournalier(connection);
                        break;
                    case '3':
                        Console.Clear();
                        SaisirUnCrime(connection);
                        break;
                    case '4':
                        Console.Clear();
                        CrimesParQuartier(connection);
                        break;
                    case '5':
                        Console.Clear();
                        RecapitulatifMensuel(connection);
                        break;
                    case '6':
                        Console.Clear();
                        EvolutionMoisParMois(connection);
                        break;
                    case '7':
                        Console.Clear();
                        PalmaresAnnuel(connection);
                        break;
                    case '8':
                        Console.Clear();
                        EvolutionMoisParMoisCambriolageJourNuit(connection);
                        break;
                    case '9':
                        Console.Clear();
                        Console.WriteLine("\n Fin de programme...\n");
                        Console.ReadKey();
                        fin = true;
                        break;
                    default:
                        Console.WriteLine("\nchoix non valide => faites un autre choix....");
                        break;
                }
            } while (!fin);

        }

        /// <summary>
        /// Fonction permettant de saisir le mois de manière sécurisée
        /// </summary>
        /// <returns></returns>
        static string saisir_mois()
        {
            string mois = "";
            string mois_lecture = "";
            do
            {
                Console.WriteLine("Veuillez saisir un mois en numéro (entre 01 et 12) :");
                mois_lecture = Console.ReadLine();
            } while (mois_lecture == "" || mois_lecture == "0" || mois_lecture == "1" || !"01".Contains(mois_lecture[0]) || (mois_lecture[0] == '0' && !"0123456789".Contains(mois_lecture[1])) || (mois_lecture[0] == '1' && !"012".Contains(mois_lecture[1])));
            Console.Clear();
            mois = mois_lecture[0].ToString() + mois_lecture[1].ToString();
            return mois;
        }

        /// <summary>
        /// Fonction permettant de saisir la date de manière sécurisée
        /// </summary>
        /// <returns></returns>
        static string saisir_date()
        {
            string jour = "";
            string jour_lecture = "";

            //saisie sécurisé du mois
            string mois = saisir_mois();

            //saisie sécurisée du jour, le format est plus lourd mais précis, afin de ne pas sabotter la base de donnée
            switch (mois)
            {
                case "01":
                case "03":
                case "05":
                case "07":
                case "08":
                case "10":
                case "12":
                    do
                    {
                        Console.WriteLine("Veuillez saisir un jour en numéro (entre 01 et 31) :");
                        jour_lecture = Console.ReadLine();
                    } while (jour_lecture == "" || !"0123".Contains(jour_lecture[0]) || ("012".Contains(jour_lecture[0]) && !"0123456789".Contains(jour_lecture[1])) || (jour_lecture[0] == '3' && !"01".Contains(jour_lecture[1])));
                    break;
                case "04":
                case "06":
                case "09":
                case "11":
                    do
                    {
                        Console.WriteLine("Veuillez saisir un jour en numéro (entre 01 et 30) :");
                        jour_lecture = Console.ReadLine();
                    } while (jour_lecture == "" || !"0123".Contains(jour_lecture[0]) || ("012".Contains(jour_lecture[0]) && !"0123456789".Contains(jour_lecture[1])) || (jour_lecture[0] == '3' && !"0".Contains(jour_lecture[1])));
                    break;
                case "02":
                    do
                    {
                        Console.WriteLine("Veuillez saisir un jour en numéro (entre 01 et 29) :");
                        jour_lecture = Console.ReadLine();
                    } while (jour_lecture == "" || !"012".Contains(jour_lecture[0]) || ("012".Contains(jour_lecture[0]) && !"0123456789".Contains(jour_lecture[1])));
                    break;
            }
            Console.Clear();
            jour = jour_lecture[0].ToString() + jour_lecture[1].ToString();
            return (mois + "/" + jour + "/2012");
        }



        /// <summary>
        /// Fonction permettant à l'utilisateur d'insérer des crimes dans la base de données provenant d'un fichier xml renseigné par l'utilisateur
        /// </summary>
        /// <param name="connection"></param>
        static void InsertionJournee(MySqlConnection connection)
        {
			//La méthode utilisée n'est pas une méthode vue en cours ou en TD, je l'ai créer en lisant la doc de xml sur internet
			//car j'ai traité cette question avant les TD où on traitait la lecture d'un fichier XmL
			//donc pas de serialiser ou de XPath ici.
			
            connection.Open();
            Console.WriteLine("Saisir le nom du fichier Xml (avec son extension .xml) :");
            string nom_fichier;
            nom_fichier = Console.ReadLine();
            XmlTextReader readerXmL = new XmlTextReader(nom_fichier);
            string borough = "";
            string jour = "";
            string mois = "";
            string desc_crime = "";
            string desc_specificity = "";
            string jurisdiction = "";
            string id_desc = "";
            string id_jurisd = "";
            string element = "";

            MySqlCommand command = connection.CreateCommand();
            try //En cas d'erreur lors de l'ouverture du fichier, le programme ne plantera pas
            {
                while (readerXmL.Read())
                {
                    switch (readerXmL.NodeType)
                    {
                        case XmlNodeType.Element: // On enregistre le type de l'élément en question
                            /* Permet d'afficher ce qu'il se passe en lecture (test, à décommenter si besoin)
                            Console.Write("<" + readerXmL.Name);
                            Console.WriteLine(">");
                            */
                            element = readerXmL.Name;
                            break;
                        case XmlNodeType.Text: //On enregistre sa valeur
                            /* Permet d'afficher ce qu'il se passe en lecture (test, à décommenter si besoin)
                            Console.WriteLine(readerXmL.Value);
                            */
                            switch (element)
                            {
                                case "borough":
                                    borough = readerXmL.Value;
                                    break;
                                case "jour":
                                    jour = readerXmL.Value;
                                    break;
                                case "mois":
                                    mois = readerXmL.Value;
                                    break;
                                case "desc_crime":
                                    desc_crime = readerXmL.Value;
                                    break;
                                case "desc_specificity":
                                    desc_specificity = readerXmL.Value;
                                    break;
                                case "jurisdiction":
                                    jurisdiction = readerXmL.Value;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case XmlNodeType.EndElement: //Quand l'intégralité de l'élément crime est saisi, on l'insère dans la base de donnée
                            /* Permet d'afficher ce qu'il se passe en lecture (test, à décommenter si besoin)
                            Console.Write("</" + readerXmL.Name);
                            Console.WriteLine(">");
                            */
                            if (readerXmL.Name == "crime")
                            {
                                //récupération de id_desc
                                command.CommandText = "SELECT id FROM crime_description WHERE description = \"" + desc_crime + "\"AND desc_specificity = \"" + desc_specificity + "\" ;";
                                MySqlDataReader readerBddDesc;
                                readerBddDesc = command.ExecuteReader();

                                if (readerBddDesc.Read())
                                {
                                    id_desc = readerBddDesc.GetString(0);
                                    readerBddDesc.Close();
                                }
                                else //Si la description du crime n'existe pas, il faut la créer dans Crime_description
                                {
                                    //cas où le crime n'existe pas dans la base de donnée
                                    readerBddDesc.Close();
                                    command.CommandText = "SELECT (count(*)+1) FROM crime_description ;";
                                    MySqlDataReader readerBddDescNb;
                                    readerBddDescNb = command.ExecuteReader();
                                    readerBddDescNb.Read();
                                    id_desc = readerBddDescNb.GetString(0);
                                    readerBddDescNb.Close();
                                    command.CommandText = "INSERT INTO NY_Crimes.Crime_description VALUES (" + id_desc + " , '" + desc_crime + "' , '" + desc_specificity + "')";
                                    command.ExecuteNonQuery();
                                }

                                //récupération de id_jurid
                                command.CommandText = "SELECT id FROM jurisdiction WHERE name = \"" + jurisdiction + "\" ;";
                                MySqlDataReader readerBddJurid;
                                readerBddJurid = command.ExecuteReader();

                                if (readerBddJurid.Read())
                                {
                                    id_jurisd = readerBddJurid.GetString(0);
                                    readerBddJurid.Close();
                                }
                                else //Si la juridiction du crime n'existe pas, il faut la créer dans jurisdiction
                                {
                                    //cas où la juridiction n'existe pas dans la base de donnée
                                    readerBddJurid.Close();
                                    command.CommandText = "SELECT (count(*)+1) FROM jurisdiction ;";
                                    MySqlDataReader readerBddJuridNb;
                                    readerBddJuridNb = command.ExecuteReader();
                                    readerBddJuridNb.Read();
                                    id_jurisd = readerBddJuridNb.GetString(0);
                                    readerBddJuridNb.Close();
                                    command.CommandText = "INSERT INTO NY_Crimes.jurisdiction VALUES (" + id_jurisd + " , '" + jurisdiction + "')";
                                    command.ExecuteNonQuery();
                                }

                                //On place les coodonnées X et Y à 0 par défault car elles ne sont pas renseigné dans le fichier xml
                                command.CommandText = "INSERT INTO NY_Crimes.Crime (date,borough,coord_X,coord_Y,crime_description_id,jurisdiction_id) "
                                                        + "VALUES ('" + mois + "/" + jour + "/2012', '" + borough + "',0,0, " + id_desc + ", " + id_jurisd + "); ";

                                command.ExecuteNonQuery();
                                borough = "";
                                jour = "";
                                mois = "";
                                desc_crime = "";
                                desc_specificity = "";
                                jurisdiction = "";
                                id_desc = "";
                                id_jurisd = "";
                                element = "";
                            }
                            break;
                    }
                }
                readerXmL.Close();
                Console.WriteLine("\nLe chargement du fichier xml a bien été effectué\n");
            }
            catch
            {
                Console.WriteLine("\nUne erreur a eu lieu lors du chargement du fichier xml, veuillez vérifier l'orthographe du nom du fichier et son espace de stockage\n");
            }

            connection.Close();
            Console.WriteLine("Appuyer sur n'importe quelle touche pour revenir au menu ...");
            Console.ReadKey();
            Console.Clear();
        }



        /// <summary>
        /// Fonction permettant à l'utilisateur de créer un fichier xml des crimes enregistrés sur un mois dans la base de données (mois demandé à l'utilisateur)
        /// </summary>
        /// <param name="connection"></param>
        static void ExporterBilanJournalier(MySqlConnection connection)
        {
			
            XmlDocument docXml = new XmlDocument();

            //saisie sécurisée de la date
            string date = saisir_date();
            string mois = date[0].ToString() + date[1].ToString();
            string jour = date[3].ToString() + date[4].ToString();
            string nom_fichier = "NYP-" + mois + "_" + jour + "_2018.xml"; // On remplace les "/" par des "_" car il s'agit d'un nom de fichier

            XmlElement racine = docXml.CreateElement("resultats");
            docXml.AppendChild(racine);
            XmlDeclaration xmldecl = docXml.CreateXmlDeclaration("1.0", "UTF-8", "no");
            docXml.InsertBefore(xmldecl, racine);
            XmlElement rq = docXml.CreateElement("requete");
            racine.AppendChild(rq);
            XmlElement daterq = docXml.CreateElement("date");
            rq.AppendChild(daterq);
            XmlElement jourrq = docXml.CreateElement("jour");
            daterq.AppendChild(jourrq);
            jourrq.InnerText = jour;
            XmlElement moisrq = docXml.CreateElement("mois");
            daterq.AppendChild(moisrq);
            moisrq.InnerText = mois;
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT c.borough,d.description,d.desc_specificity,j.name "
                                + "FROM crime AS c JOIN crime_description AS d ON c.crime_description_id=d.id JOIN jurisdiction AS j ON c.jurisdiction_id=j.id "
                                + "WHERE c.date =\"" + date + "\"";
            MySqlDataReader reader;
            reader = command.ExecuteReader();

            while (reader.Read())
            {
                XmlElement element = docXml.CreateElement("crime");
                racine.AppendChild(element);

                XmlElement dateX = docXml.CreateElement("date");
                element.AppendChild(dateX);
                XmlElement jourX = docXml.CreateElement("jour");
                dateX.AppendChild(jourX);
                jourX.InnerText = jour;
                XmlElement moisX = docXml.CreateElement("mois");
                dateX.AppendChild(moisX);
                moisX.InnerText = mois;

                XmlElement borough = docXml.CreateElement("borough");
                element.AppendChild(borough);
                borough.InnerText = reader.GetString(0);
                XmlElement desc = docXml.CreateElement("description");
                element.AppendChild(desc);
                desc.InnerText = reader.GetString(1);
                XmlElement desc_spec = docXml.CreateElement("description_specificity");
                element.AppendChild(desc_spec);
                desc_spec.InnerText = reader.GetString(2);
                XmlElement jurisdiction = docXml.CreateElement("jurisdiction");
                element.AppendChild(jurisdiction);
                jurisdiction.InnerText = reader.GetString(3);

            }

            connection.Close();
            docXml.Save(nom_fichier);
            Console.WriteLine("\nLe fichier " + nom_fichier + " a bien été créé\n");
            Console.WriteLine("\nAppuyer sur n'importe quelle touche pour revenir au menu ...");
            Console.ReadKey();
            Console.Clear();

        }



        /// <summary>
        /// Fonction permettant à l'utilisateur d'insérer un crimes dans la base de données (mais aussi une description de crime ou une juridiction)
        /// </summary>
        /// <param name="connection"></param>
        static void SaisirUnCrime(MySqlConnection connection)
        {
            Console.Clear();
            connection.Open();
            string borough = "";
            string date = "";
            int coordX = 0;
            int coordY = 0;
            int id_desc = -1;
            int id_jurisd = -1;

            //saisie sécurisée de la date, ne permet d'écrire seulement une date qui existe en 2012, afin de ne pas sabotter la base de donnée
            date = saisir_date();

            //saisie du quartier avec affichage de ceux présents dans la base de donnée
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = " SELECT DISTINCT borough "
                                + " FROM crime "
                                + " ORDER BY borough;"; // On trie pour faire plus propre

            MySqlDataReader readerBorough;
            readerBorough = command.ExecuteReader();
            string borough_liste = "";
            Console.WriteLine("Liste des quartiers déjà rentrés dans la base de donnée :\n");
            while (readerBorough.Read())   // parcours ligne par ligne
            {
                borough_liste = readerBorough.GetString(0); // récupération 1ère colonne
                Console.WriteLine("  =>  " + borough_liste);
            }
            readerBorough.Close();

            Console.WriteLine("\nSaisir un quartier déjà enregistré dans la base de donnée ou un nouveau :");
            borough = Console.ReadLine();
            Console.Clear();

            //saisie de l'index de la description du crime avec affichage de ceux présents dans la base de donnée
            command = connection.CreateCommand();
            command.CommandText = " SELECT id, description, desc_specificity "
                                + " FROM crime_description "
                                + " ORDER BY id;"; // On trie pour faire plus propre

            MySqlDataReader readerDesc;
            readerDesc = command.ExecuteReader();

            string id_desc_liste = "";
            string description_liste = "";
            string desc_specificity_liste = "";
            Console.WriteLine("Liste des des descriptions de crime déjà rentrées dans la base de données:\n");
            while (readerDesc.Read())   // parcours ligne par ligne
            {
                id_desc_liste = readerDesc.GetString(0); // récupération 1ère colonne
                description_liste = readerDesc.GetString(1);
                desc_specificity_liste = readerDesc.GetString(2);
                Console.WriteLine(" " + id_desc_liste + "  =>  " + description_liste + " (" + desc_specificity_liste + ")");
            }
            readerDesc.Close();
            int id_desc_max = Convert.ToInt32(id_desc_liste);
            do
            {
                Console.WriteLine("\nSaisir un id de crime existant ou saisir 0 pour en écrire un nouveau :");
                id_desc = Convert.ToInt32(Console.ReadLine());
            } while (id_desc > id_desc_max || id_desc < 0);
            if (id_desc == 0) // Création d'une nouvelle description
            {
                Console.WriteLine("\nSaisir la description du nouveau crime :");
                string desc_crime = Console.ReadLine();
                Console.WriteLine("\nSaisir la description détaillée du nouveau crime :");
                string desc_specificity = Console.ReadLine();
                id_desc = id_desc_max + 1;
                command.CommandText = "INSERT INTO NY_Crimes.Crime_description VALUES (" + id_desc.ToString() + " , '" + desc_crime + "' , '" + desc_specificity + "')";
                command.ExecuteNonQuery();
            }
            Console.Clear();

            //saisie de l'index de la jurisdiction avec affichage de ceux présents dans la base de donnée
            command = connection.CreateCommand();
            command.CommandText = " SELECT id, name "
                                + " FROM jurisdiction "
                                + " ORDER BY id;"; // On trie pour faire plus propre

            MySqlDataReader readerJuri;
            readerJuri = command.ExecuteReader();

            string id_juri_liste = "";
            string name_juri_liste = "";
            Console.WriteLine("Liste des juridictions déjà rentrées dans la base de données :\n");
            while (readerJuri.Read())   // parcours ligne par ligne
            {
                id_juri_liste = readerJuri.GetString(0); // récupération 1ère colonne
                name_juri_liste = readerJuri.GetString(1);
                Console.WriteLine(" " + id_juri_liste + "  =>  " + name_juri_liste);
            }
            readerJuri.Close();
            int id_juri_max = Convert.ToInt32(id_juri_liste);
            do
            {
                Console.WriteLine("\nSaisir un id de jurisdiction existante ou saisir 0 pour en écrire une nouvelle :");
                id_jurisd = Convert.ToInt32(Console.ReadLine());
            } while (id_jurisd > id_juri_max || id_jurisd < 0);
            if (id_jurisd == 0) // Création d'une nouvelle juridiction
            {
                Console.WriteLine("\nSaisir le nom de la nouvelle juridiction :");
                string name_juri = Console.ReadLine();
                id_jurisd = id_juri_max + 1;
                command.CommandText = "INSERT INTO NY_Crimes.jurisdiction VALUES (" + id_jurisd.ToString() + " , '" + name_juri + "')";
                command.ExecuteNonQuery();
            }
            Console.Clear();

            //saisie des coordonnées
            Console.WriteLine("\nSaisir la coordonnée en X du crime (par défault 0) :");
            string saisie = Console.ReadLine();
            try
            { coordX = Convert.ToInt32(saisie); }
            catch
            { coordX = 0; }; //si l'utilisateur fais une faute, on place la coordonnée par défault
            Console.WriteLine("\nSaisir la coordonnée en Y du crime (par défault 0) :");
            saisie = Console.ReadLine();
            try
            { coordY = Convert.ToInt32(saisie); }
            catch
            { coordY = 0; }; //si l'utilisateur fais une faute, on place la coordonnée par défault
            Console.Clear();

            //enregistrement dans la base de donnée
            command.CommandText = "INSERT INTO NY_Crimes.Crime (date,borough,coord_X,coord_Y,crime_description_id,jurisdiction_id) "
                                                    + "VALUES ('" + date + "', '" + borough + "'," + coordX.ToString() + "," + coordY.ToString() + ", " + id_desc.ToString() + ", " + id_jurisd.ToString() + "); ";
            command.ExecuteNonQuery();

            connection.Close();
            Console.WriteLine("\nLe crime a bien été ajouté à la base de données\n");
            Console.WriteLine("\nAppuyer sur n'importe quelle touche pour revenir au menu ...");
            Console.ReadKey();
            Console.Clear();

        }




        /// <summary>
        /// Fonction permettant de retourner le nombre de crimes par quartier et par catégorie pour un jour donné par l'utilisateur
        /// </summary>
        /// <param name="connection"></param>
        static void CrimesParQuartier(MySqlConnection connection)
        {
            connection.Open();
            // saisie sécurisée de la date
            string date = saisir_date();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = " SELECT c.borough, d.description, count(*) AS nb_crime "
                                + " FROM crime AS c JOIN crime_description AS d ON c.crime_description_id=d.id "
                                + " WHERE c.date = \"" + date + "\" "
                                + " GROUP BY  c.borough , d.description"
                                + " ORDER BY c.borough, nb_crime DESC;"; // On trie pour faire plus propre

            MySqlDataReader reader;
            reader = command.ExecuteReader();

            string borough = "";
            string crime = "";
            string nb_crime = "";
            Console.WriteLine("Nombre de crimes commis a New York city le " + date + ", trie par quartier et par type de crime :");
            while (reader.Read())   // parcours ligne par ligne
            {
                borough = reader.GetString(0); // récupération 1ère colonne
                crime = reader.GetString(1); // récupération 2ème colonne
                nb_crime = reader.GetString(2);  // récupération 3ème colonne
                Console.WriteLine(borough + ", " + crime + " : " + nb_crime + " crime(s) commi(s) ");
            }

            connection.Close();
            Console.WriteLine("\nAppuyer sur n'importe quelle touche pour revenir au menu ...");
            Console.ReadKey();
            Console.Clear();

        }



        /// <summary>
        /// Fonction permettant de  donner le récapitulatif  des ‘grand larcery ‘ quartier par quartier pour un mois donné par l'utilisateur
        /// </summary>
        /// <param name="connection"></param>
        static void RecapitulatifMensuel(MySqlConnection connection)
        {
            connection.Open();
            string mois = saisir_mois();
            MySqlCommand command = connection.CreateCommand();

            command.CommandText = " SELECT c.borough, count(*) AS nb_crime "
                                + " FROM crime AS c JOIN crime_description AS d ON c.crime_description_id=d.id "
                                + " WHERE d.description = \"grand larceny\" AND c.date LIKE \"" + mois + "/%/2012\" "
                                + " GROUP BY c.borough"
                                + " ORDER BY nb_crime DESC;"; // On trie pour faire plus propre

            MySqlDataReader reader;
            reader = command.ExecuteReader();

            string borough = "";
            string nb_crime = "";
            Console.WriteLine("Nombre de crimes de grand larcin commis a New York city le mois " + mois + ", trie par quartier  :");
            while (reader.Read())   // parcours ligne par ligne
            {
                borough = reader.GetString(0); // récupération 1ère colonne 
                nb_crime = reader.GetString(1);  // récupération 2ème colonne 
                Console.WriteLine(borough + " : " + nb_crime + " crime(s) commi(s) ");
            }

            connection.Close();
            Console.WriteLine("\nAppuyer sur n'importe quelle touche pour revenir au menu ...");
            Console.ReadKey();
            Console.Clear();
        }



        /// <summary>
        /// Fonction permettant de  donner l'évolution mois par mois du % de crimes à New-York par quartier pour les ‘grand larcery ‘ (% par rapport au total des crimes de la ville tous quartiers confondus)
        /// </summary>
        /// <param name="connection"></param>
        static void EvolutionMoisParMois(MySqlConnection connection)
        {
            connection.Open();
            int int_mois = 0;
            string mois = "00";
            while (int_mois < 12)
            {
                int_mois++;
                if (int_mois < 10)
                {
                    mois = "0" + Convert.ToString(int_mois);
                }
                else
                {
                    mois = Convert.ToString(int_mois);
                }

                MySqlCommand command = connection.CreateCommand();

                command.CommandText = " SELECT count(*) AS nb_crime "
                                   + " FROM crime AS c JOIN crime_description AS d ON c.crime_description_id=d.id "
                                   + " WHERE d.description = \"grand larceny\" AND c.date LIKE \"" + mois + "/%/2012\"";

                MySqlDataReader readerNb;
                readerNb = command.ExecuteReader();
                readerNb.Read();
                string nb_crime_tot = "";
                nb_crime_tot = readerNb.GetString(0);
                readerNb.Close();

                command.CommandText = " SELECT c.borough, (count(borough)*100/" + nb_crime_tot + ") AS pc_crime "
                                    + " FROM crime AS c JOIN crime_description AS d ON c.crime_description_id=d.id "
                                    + " WHERE d.description = \"grand larceny\" AND c.date LIKE \"" + mois + "/%/2012\""
                                    + " GROUP BY c.borough"
                                    + " ORDER BY c.borough"; // On trie pour faire plus propre

                MySqlDataReader readerBorough;
                readerBorough = command.ExecuteReader();

                string borough = "";
                string pourcent_crime = "";
                Console.WriteLine("Pour le mois " + mois + "  :");
                while (readerBorough.Read())   // parcours ligne par ligne
                {
                    borough = readerBorough.GetString(0); // récupération 1ère colonne
                    pourcent_crime = readerBorough.GetString(1);  // récupération 2ème colonne
                    Console.WriteLine("       " + borough + " : " + pourcent_crime + "% des crime(s) de grand larcin commi(s) ce mois-ci ");
                }
                readerBorough.Close();
                Console.WriteLine();
            }

            connection.Close();
            Console.WriteLine("\nAppuyer sur n'importe quelle touche pour revenir au menu ...");
            Console.ReadKey();
            Console.Clear();
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        static void PalmaresAnnuel(MySqlConnection connection)
        {
            connection.Open();
            MySqlCommand command = connection.CreateCommand();

            command.CommandText = " SELECT borough, count(borough) AS nb_crime"
                                    + " FROM crime"
                                    + " WHERE date LIKE \"%2012\""
                                    + " GROUP BY borough HAVING nb_crime >= ALL(SELECT count(borough)"
                                                                                + " FROM crime "
                                                                                + " WHERE date LIKE \"%2012\""
                                                                                + " GROUP BY borough); ";

            MySqlDataReader reader;
            reader = command.ExecuteReader();
            reader.Read();
            string borough = "";
            string nb_crime = "";
            borough = reader.GetString(0); // récupération 1ère colonne
            nb_crime = reader.GetString(1); // récupération 2ème colonne
            Console.WriteLine("Le quartier le plus criminogène de New York City en 2012 est " + borough + " avec un nombre total de " + nb_crime + " crime(s)\n");
            reader.Close();

            MySqlCommand command2 = connection.CreateCommand();

            command2.CommandText = " SELECT d.description, d.desc_specificity, count(*) AS nb_crime "
                                    + " FROM crime AS c JOIN crime_description AS d ON c.crime_description_id = d.id "
                                    + " WHERE c.date LIKE \"%2012\" AND c.borough = \"" + borough + "\""
                                    + " GROUP BY  c.borough , d.description, d.desc_specificity "
                                    + " HAVING nb_crime >= ("+nb_crime+" * 5 / 100) " // On sélectionne seulement les crimes avec plus de 5% d'occurences
                                    + " ORDER BY c.borough, nb_crime DESC; ";

            Console.WriteLine("La liste des crimes les plus courrants est : \n");

            MySqlDataReader reader2;
            reader2 = command2.ExecuteReader();
            string desc = "";
            string desc_spe = "";
            string nb_crime_spe = "";

            while (reader2.Read())   // parcours ligne par ligne
            {
                desc = reader2.GetString(0); // récupération 1ère colonne
                desc_spe = reader2.GetString(1);  // récupération 2ème colonne
                nb_crime_spe = reader2.GetString(2); // récupération 3ème colonne
                Console.WriteLine(desc + ", " + desc_spe + " => " + nb_crime_spe + " crime(s) commi(s) ");
            }

            connection.Close();
            Console.WriteLine("\nAppuyer sur n'importe quelle touche pour revenir au menu ...");
            Console.ReadKey();
            Console.Clear();
        }



        /// <summary>
        /// Fonction permettant de  donner l'évolution mois par mois du % des crimes comis le jour par rapport à ceux commis la nuit à New-York pour les cambriolage (% par rapport au total des crimes de la ville tous quartiers confondus)
        /// </summary>
        /// <param name="connection"></param>
        static void EvolutionMoisParMoisCambriolageJourNuit(MySqlConnection connection)
        {
            //Cette fonction est possible car dans la desc_specifique des cambriolages, il est toujours indiqué s'il a eu lieu le jour, la nuit ou à un horraire inconnu
            connection.Open();
            int int_mois = 0;
            string mois = "00";
            while (int_mois < 12)
            {
                int_mois++;
                if (int_mois < 10)
                {
                    mois = "0" + Convert.ToString(int_mois);
                }
                else
                {
                    mois = Convert.ToString(int_mois);
                }

                MySqlCommand command = connection.CreateCommand();

                command.CommandText = " SELECT count(*) AS nb_crime"
                                   + " FROM crime AS c JOIN crime_description AS d ON c.crime_description_id=d.id "
                                   + " WHERE d.description = \"burglary\" AND c.date LIKE \"" + mois + "%2012\"";
                
                try //en cas de base de donnée incomplête (aucun crime saisie sur un mois)
                {
                    MySqlDataReader readerNb;
                    readerNb = command.ExecuteReader();
                    readerNb.Read();
                    string nb_crime_tot = "";
                
                    nb_crime_tot = readerNb.GetString(0);
                    readerNb.Close();

                    command.CommandText = " SELECT (count(*)*100/" + nb_crime_tot + ")"
                                        + " FROM crime AS c JOIN crime_description AS d ON c.crime_description_id=d.id "
                                        + " WHERE d.description = \"burglary\" AND c.date LIKE \"" + mois + "%2012\""
                                        + " AND d.desc_specificity LIKE \"%day%\""
                                        + " ORDER BY c.borough";
                    MySqlDataReader readerDay;
                    readerDay = command.ExecuteReader();
                    readerDay.Read();
                    string pourcent_crime_day = "";
                    pourcent_crime_day = readerDay.GetString(0);
                    readerDay.Close();

                    command.CommandText = " SELECT (count(*)*100/" + nb_crime_tot + ")"
                                        + " FROM crime AS c JOIN crime_description AS d ON c.crime_description_id=d.id "
                                        + " WHERE d.description = \"burglary\" AND c.date LIKE \"" + mois + "%2012\""
                                        + " AND d.desc_specificity LIKE \"%night%\""
                                        + " ORDER BY c.borough";
                    MySqlDataReader readerNight;
                    readerNight = command.ExecuteReader();
                    readerNight.Read();
                    string pourcent_crime_night = "";
                    pourcent_crime_night = readerNight.GetString(0);
                    readerNight.Close();

                    command.CommandText = " SELECT (count(*)*100/" + nb_crime_tot + ")"
                                        + " FROM crime AS c JOIN crime_description AS d ON c.crime_description_id=d.id "
                                        + " WHERE d.description = \"burglary\" AND c.date LIKE \"" + mois + "%2012\""
                                        + " AND d.desc_specificity LIKE \"%unknown%\""
                                        + " ORDER BY c.borough";
                    MySqlDataReader readerUnknown;
                    readerUnknown = command.ExecuteReader();
                    readerUnknown.Read();
                    string pourcent_crime_unknown = "";
                    pourcent_crime_unknown = readerUnknown.GetString(0);
                    readerUnknown.Close();

                    Console.WriteLine("Pour le mois " + mois + "  :");
                    Console.WriteLine("       Jour : " + pourcent_crime_day + "% des crime(s) de cambriolage commi(s) ce mois-ci ");
                    Console.WriteLine("       Nuit : " + pourcent_crime_night + "% des crime(s) de cambriolage commi(s) ce mois-ci ");
                    Console.WriteLine("       Inconnu : " + pourcent_crime_unknown + "% des crime(s) de cambriolage commi(s) ce mois-ci \n");

                    Console.WriteLine();
                }
                catch
                {
                    Console.WriteLine("Pour le mois " + mois + "  :");
                    Console.WriteLine("       Pas de données enregistrées ");
                    Console.WriteLine();
                }
            }
            connection.Close();
            Console.WriteLine("\nAppuyer sur n'importe quelle touche pour revenir au menu ...");
            Console.ReadKey();
            Console.Clear();
        }
        
    }
}

