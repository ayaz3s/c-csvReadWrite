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
            csvRecords.writeCsv();
        }
        
    }

    public class CsvRecords
    {
        public List<Person>? records;

        public void readCsv(){

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

        }

        public void writeCsv(){

            if (this.records != null){
                // add the bmi column

                List<Person> newRecords = new List<Person>();
                foreach (Person person in records)
                {
                    if (person.heightFt != 0 && person.weight != 0){
                        addBmiCol(person);
                        newRecords.Add(person);
                    }
                }

                using(var streamWriter = new StreamWriter(@"testcsvwithbmi.csv"))
                using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
                {
                    csvWriter.WriteRecords(newRecords);
                    csvWriter.Dispose();
                    streamWriter.Dispose();
                }
            }
        }

        public void addBmiCol(Person person){
            person.bmi = person.calcBmi();
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
            Map(m => m.unWanted).Ignore();
            
        }
    }

    public class Person
    {
        public int id {get; set;}
        public float heightFt {get; set;}
        public float weight {get; set;}
        public dynamic? unWanted {get; set;}
        public float bmi {get; set;}

        public float calcBmi(){
            float heightFt = this.heightFt;
            float weight = this.weight;

            // 1 foot = 0.3048 meter
            // bmi = kg/m^2
            float bmi = weight/((heightFt*0.3048f)*(heightFt*0.3048f));
            return bmi;
        }
    }
}