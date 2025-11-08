using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Lab8_Variant10_Improved
{
    // === Константи з шаблонами ===
    public static class Patterns
    {
        // Lookaround: шукає великі букви (мінімум 2), опціонально + або #, без чистих чисел
        public const string Abbreviation = @"(?<![A-Za-z0-9])(?=[A-Za-z]*[A-Z])[A-Z]{2,}[A-Z0-9]*(?:\+{1,}|#{1,})?(?![A-Za-z0-9])";

        // IPv4 (простий варіант)
        public const string IP = @"(?<!\d)(?:\d{1,3}\.){3}\d{1,3}(?!\d)";

        // Дата у форматі дд.мм.рр або дд/мм/рр
        public const string Date = @"\b\d{1,2}([./-])\d{1,2}\1\d{2,4}\b";
    }

    // === Клас пошуку за різними шаблонами ===
    public class RegexSearcher
    {
        private readonly Dictionary<string, Regex> _patterns;

        public RegexSearcher(bool ignoreCase = false)
        {
            var options = RegexOptions.Compiled;
            if (ignoreCase) options |= RegexOptions.IgnoreCase;

            _patterns = new Dictionary<string, Regex>
            {
                { "абревіатура", new Regex(Patterns.Abbreviation, options) },
                { "ip-адреса",   new Regex(Patterns.IP, options) },
                { "дата",        new Regex(Patterns.Date, options) }
            };
        }

        // Загальний метод пошуку
        public Dictionary<string, List<string>> FindAll(string text)
        {
            var result = new Dictionary<string, List<string>>();
            foreach (var kv in _patterns)
            {
                var matches = kv.Value.Matches(text)
                    .Cast<Match>()
                    .Select(m => m.Value)
                    .Distinct()
                    .ToList();
                result[kv.Key] = matches;
            }
            return result;
        }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("ЛР8 — Пошук за кількома шаблонами (варіант 10)\n");

            Console.WriteLine("Введіть шлях до файлу або залиште порожнім для прикладу:");
            string? input = Console.ReadLine();

            string text;
            if (string.IsNullOrWhiteSpace(input))
            {
                text = "Приклад: Ми використовуємо C#, C++ і HTML. API повертає JSON.\n" +
                       "Дата публікації: 12.05.2024. Сервер IP: 192.168.0.1.";
                Console.WriteLine("\nВикористовується прикладовий текст:\n" + text);
            }
            else if (File.Exists(input))
            {
                try
                {
                    text = File.ReadAllText(input);
                    Console.WriteLine("Файл успішно прочитано.\n");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Помилка читання файлу: {ex.Message}");
                    return;
                }
            }
            else
            {
                Console.WriteLine("Файл не знайдено. Використовується прикладовий текст.\n");
                text = "Тестовий текст із C++, IP: 10.0.0.2, дата 01/01/2025.";
            }

            var searcher = new RegexSearcher(ignoreCase: false);
            var results = searcher.FindAll(text);

            Console.WriteLine("Результати пошуку:");
            foreach (var kv in results)
            {
                Console.WriteLine($"\nТип: {kv.Key}");
                if (kv.Value.Any())
                {
                    Console.WriteLine($"Знайдено ({kv.Value.Count}): {string.Join(", ", kv.Value)}");
                }
                else
                {
                    Console.WriteLine("Не знайдено.");
                }
            }

            Console.WriteLine("\n--- Готово ---");
            Console.ReadKey();
        }
    }
}
