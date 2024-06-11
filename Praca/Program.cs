using System;
using System.IO;
using System.Xml.Linq;

class Program
{
    static void Main(string[] args)
    {
        string csvFilePath = @"C:\Users\Jendr\Desktop\Test\Praca.csv";
        string xmlFilePath = @"C:\Users\Jendr\Desktop\Test\Praca.xml";

        try
        {
            XElement root = ConvertCsvToXml(csvFilePath);
            SaveXmlDocument(root, xmlFilePath);
            Console.WriteLine("Plik XML został zapisany w: " + xmlFilePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Wystąpił błąd: " + ex.Message);
        }
    }

    static XElement ConvertCsvToXml(string csvFilePath)
    {
        XElement root = new XElement("Root");

        using (StreamReader reader = new StreamReader(csvFilePath))
        {
            string line;
            string[] headers = ReadHeaders(reader);

            if (headers == null)
            {
                throw new Exception("Nie znaleziono nagłówka 'Kod pracownika' w pliku CSV.");
            }

            while ((line = reader.ReadLine()) != null)
            {
                XElement row = CreateRowElement(line, headers);
                root.Add(row);
            }
        }

        return root;
    }

    static string[] ReadHeaders(StreamReader reader)
    {
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            if (line.StartsWith("Kod pracownika"))
            {
                return line.Split(';');
            }
        }
        return null;
    }

    static XElement CreateRowElement(string line, string[] headers)
    {
        XElement row = new XElement("Row");
        XElement dniPlanu = new XElement("DniPlanu");
        string[] values = line.Split(';');
        string kodPracownika = values[0];

        for (int i = 1; i < headers.Length; i++)
        {
            XElement dzienPlanu = CreateDzienPlanuElement(headers[i], kodPracownika, values.Length > i ? values[i] : string.Empty);
            dniPlanu.Add(dzienPlanu);
        }

        row.Add(dniPlanu);
        return row;
    }

    static XElement CreateDzienPlanuElement(string header, string kodPracownika, string value)
    {
        XElement dzienPlanu = new XElement("DzienPlanu");
        string formattedDate = header.Replace('.', '/');

        dzienPlanu.Add(new XElement("Pracownik", kodPracownika));
        dzienPlanu.Add(new XElement("data", formattedDate));

        switch (value)
        {
            case "X":
                dzienPlanu.Add(new XElement("Defnicja", "wolny"));
                break;
            case "1":
                AddPracyElement(dzienPlanu, "6", "8");
                break;
            case "2":
                AddPracyElement(dzienPlanu, "14", "8");
                break;
            default:
                AddPracyElement(dzienPlanu, "22", "8");
                break;
        }

        return dzienPlanu;
    }

    static void AddPracyElement(XElement dzienPlanu, string odGodziny, string czas)
    {
        dzienPlanu.Add(new XElement("Defnicja", "Pracy"));
        dzienPlanu.Add(new XElement("OdGodziny", odGodziny));
        dzienPlanu.Add(new XElement("Czas", czas));
    }

    static void SaveXmlDocument(XElement root, string xmlFilePath)
    {
        XDocument doc = new XDocument(root);
        doc.Save(xmlFilePath);
    }
}
