using System;
using CsvHelper;
using System.IO;
using System.Globalization;
using System.Linq;
using CsvHelper.Configuration;

namespace CsvReaderTest
{
    class Program
    {
        static void Main(string[] args)
        {
            CsvRecords csvRecords = new CsvRecords();
            csvRecords.readCsv();
        }
        
    }

    public class CsvRecords
    {
        public List<Person>? records;

        public void readCsv(){

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null
            };

            // using statement to release the resource
            using(var streamReader = new StreamReader(@"testcsv.csv"))
            {
                using (var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture))
                {
                    csvReader.Context.RegisterClassMap<PersonClassMap>();
                    List<Person> recordsTmp = csvReader.GetRecords<Person>().ToList();

                    this.records = recordsTmp;
                    this.viewBmi(this.records);
                }

            }

            this.addBmiCol(this.records);
        }

        public void addBmiCol(List<Person> records){
            foreach (Person person in records)
            {
                person.bmi = person.calcBmi();
            }
            using(var streamWriter = new StreamWriter(@"testcsvwithbmi.csv"))
            using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
            {
                csvWriter.WriteRecords(records);
            }
        }

        public void viewBmi(List<Person> records){
            foreach (Person person in records)
            {
                Console.WriteLine(person.calcBmi());
            }
        }

    }

    class PersonClassMap : ClassMap<Person>
    {
        public PersonClassMap()
        {
            Map(m => m.id).Name("id");
            Map(m => m.heightFt).Name("heightft");
            Map(m => m.weight).Name("weight");
            
        }
    }

    public class Person
    {
        public int id {get; set;}
        public float heightFt {get; set;}
        public float weight {get; set;}

        public float bmi {get; set;}

        public float calcBmi(){
            float heightFt = this.heightFt;
            float weight = this.weight;

            // 1 foot = 0.3048 meter
            float bmi = weight/(heightFt*0.3048f);
            return bmi;
        }
    }
}