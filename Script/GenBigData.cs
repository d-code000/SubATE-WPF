using System.Text;

namespace Script
{
    public class GenBigData
    {
        public static void Main(string[] args)
        {
            string inputFilePath = @"..\..\..\..\SubATE-WPF\data\100row.csv";
            string outputFilePath = @"..\..\..\..\SubATE-WPF\data\BigData.csv";
            
            int repeatCount = 10000;
            
            string[] lines = File.ReadAllLines(inputFilePath);
            if (lines.Length < 2)
            {
                Console.WriteLine("Исходный файл должен содержать как минимум 2 строки (заголовок и данные).");
                return;
            }
            
            string header = lines[0];
            var dataLines = lines[1..];

            if (dataLines.Length > 100)
            {
                dataLines = dataLines[..100];
            }
            
            using (var writer = new StreamWriter(outputFilePath, false, Encoding.UTF8))
            {
                writer.WriteLine(header);
                for (int i = 0; i < repeatCount; i++)
                {
                    foreach (var line in dataLines)
                    {
                        writer.WriteLine(line);
                    }
                }
            }

            Console.WriteLine($"Файл '{outputFilePath}' успешно сгенерирован с {dataLines.Length * repeatCount} строками.");
        }
    }
}