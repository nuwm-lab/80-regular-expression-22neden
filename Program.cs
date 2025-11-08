using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Lab8_Variant10
{
    // Клас, що інкапсулює логіку пошуку абревіатур
    public class AbbreviationFinder
    {
        // Регулярний вираз як константа (можна змінити для інших правил)
        private readonly Regex _abbrRegex;

        public AbbreviationFinder()
        {
            // Пошук: великі літери або цифри, опціонально # або + в кінці
            _abbrRegex = new Regex(@"\b[A-Z0-9]{1,}(?:[#\+]{1,})?\b", RegexOptions.Compiled);
        }

        // Повертає список знайдених унікальних абревіатур у порядку першої появи
        public IList<string> FindAbbreviations(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));

            var matches = _abbrRegex.Matches(text);
            var result = new List<string>();
            var seen = new HashSet<string>();

            foreach (Match m in matches)
            {
                var value = m.Value;
                // Фільтруємо "звичайні" слова, якщо треба:
                // наприклад виключити слова довші ніж 1 символ, що починаються з маленької букви — тут ми вже вимагаємо великі
                if (!seen.Contains(value))
                {
                    seen.Add(value);
                    result.Add(value);
                }
            }

            return result;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("ЛР8 варіант 10 — пошук абревіатур (наприклад: C#, C++, HTML, JSON).");
            Console.WriteLine("Введіть шлях до файлу з текстом або вставте текст і натисніть Enter (порожній рядок — приклад):");

            string input = Console.ReadLine();

            string text;
            if (string.IsNullOrWhiteSpace(input))
            {
                // Приклад тексту
                text = "Приклад: Ми використовуємо C#, C++ та HTML. API повертає JSON. Також є версія G2 та ID123.";
                Console.WriteLine("Використовується прикладний текст:\n" + text);
            }
            else if (File.Exists(input))
            {
                text = File.ReadAllText(input);
                Console.WriteLine("Файл прочитано.");
            }
            else
            {
                // Беремо введений рядок як текст
                text = input;
            }

            var finder = new AbbreviationFinder();
            var found = finder.FindAbbreviations(text);

            Console.WriteLine("\nЗнайдені абревіатури (" + found.Count + "):");
            foreach (var a in found)
            {
                Console.WriteLine(" - " + a);
            }

            Console.WriteLine("\nГотово. Натисніть будь-яку клавішу для виходу...");
            Console.ReadKey();
        }
    }
}
