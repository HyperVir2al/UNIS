using System;
using System.Collections.Generic;
using System.Text;
using Universities.Data;
using Microsoft.EntityFrameworkCore;

namespace Universities.Queries
{
    public static class LinqDemos
    {
        // 1. Фильтрация с несколькими условиями (Where + && / ||) и сортировка
        public static void DemoFilteringAndSorting(UnisContext db)
        {
            Console.WriteLine("\n=== 1. люди с фамилией Rampage  ===");

            var result = db.Students
                .Where(t => t.LastName == "Rampage" && t.Group.Name == "Д-24")
                .OrderByDescending(t => t.BirthDate)
                .ThenBy(t => t.FirstName)
                .ToList();

            foreach (var t in result)
                Console.WriteLine($"{t.FirstName} | {t.LastName} | {t.BirthDate} г.");
        }

        // 2. Проекция (Select) в анонимный тип
        public static void DemoProjection(UnisContext db)
        {
            Console.WriteLine("\n=== 2. Проекция: Имя и уник ===");

            var result = db.Students
                .Include(t => t.Group.University)
                .Select(t => new
                {
                    Fn = t.FirstName,
                    Un = t.Group.University != null ? t.Group.University.Name : "Неизвестно",
                    Address = t.Group.University != null ? t.Group.University.Address : "-"
                })
                .ToList();

            foreach (var item in result)
                Console.WriteLine($"{item.Fn} -> {item.Un} ({item.Address})");
        }

        // 3. Агрегация (Count, Sum, Average, Min, Max)
        public static void DemoAggregations(UnisContext db)
        {
            Console.WriteLine("\n=== 3. Агрегатные вычисления по боевым записям ===");

            int currentYear = DateTime.Now.Year;

            // 1. Общий подсчет сущностей
            int totalStudents = db.Students.Count();
            int totalGroups = db.Groups.Count();
            int totalUniversities = db.Universitys.Count(); // Используем имя свойства из вашего UnisContext

            // 2. Статистика по вместимости групп (поля из модели Group)
            int totalMaxStudents = db.Groups.Sum(g => g.MaxStudents);
            int maxGroupCapacity = db.Groups.Max(g => g.MaxStudents);
            int minGroupCapacity = db.Groups.Min(g => g.MaxStudents);

            // 3. Аналитика возраста студентов (поле BirthDate из модели Student)
            // Считаем разницу лет прямо в базе
            double avgAge = db.Students.Average(s => currentYear - s.BirthDate.Year);
            int minAge = db.Students.Min(s => currentYear - s.BirthDate.Year);

            // Вывод статистики в консоль
            Console.WriteLine($"Всего студентов в базе: {totalStudents}");
            Console.WriteLine($"Всего учебных групп: {totalGroups}");
            Console.WriteLine($"Всего университетов: {totalUniversities}");
            Console.WriteLine($"Суммарная вместимость всех групп (мест): {totalMaxStudents}");
            Console.WriteLine($"Максимальная вместимость одной группы: {maxGroupCapacity} чел.");
            Console.WriteLine($"Минимальная вместимость одной группы: {minGroupCapacity} чел.");
            Console.WriteLine($"Средний возраст студента: {avgAge:F1} лет");
            Console.WriteLine($"Возраст самого молодого студента: {minAge} лет");
        }

        // 4. Группировка (GroupBy) с агрегатом
        public static void DemoGrouping(UnisContext db)
        {
            Console.WriteLine("\n=== 4. Группировка: Количество групп и средняя вместимость по годам набора ===");

            var groupSummary = db.Groups
                .GroupBy(g => g.Year)
                .Select(g => new
                {
                    Year = g.Key,
                    GroupCount = g.Count(),
                    AvgCapacity = g.Average(g => g.MaxStudents)
                })
                .OrderByDescending(g => g.GroupCount)
                .ToList();

            foreach (var item in groupSummary)
                Console.WriteLine($"Год набора: {item.Year} | Групп: {item.GroupCount} | Ср. вместимость: {item.AvgCapacity:F1} чел.");


            Console.WriteLine("\n--- Группировка учебных данных по университетам ---");

            var uniSummary = db.Groups
                .GroupBy(g => g.University!.Name)
                .Select(g => new
                {
                    UniName = g.Key,
                    GroupsCount = g.Count(),
                    TotalMaxStudents = g.Sum(g => g.MaxStudents),
                    // Считаем общее число студентов в этом университете через коллекцию Students у групп
                    TotalStudents = g.Sum(g => g.Students.Count)
                })
                .OrderByDescending(g => g.TotalStudents)
                .ToList();

            foreach (var item in uniSummary)
                Console.WriteLine($"Вуз: {item.UniName} | Групп: {item.GroupsCount} | Мест всего: {item.TotalMaxStudents} | Учится студентов: {item.TotalStudents}");

        }

        // 5. Жадная загрузка связей (Include / ThenInclude)
        public static void DemoJoinsAndIncludes(UnisContext db)
        {
            Console.WriteLine("\n=== 5. Университеты, их группы и списки студентов (Include / ThenInclude) ===");

            // Загружаем университеты, принудительно подтягивая связанные группы, 
            // а для каждой группы — список её студентов.
            var universitiesWithStudents = db.Universitys // Имя свойства из вашего UnisContext
                .Include(u => u.Group)                    // Подгружаем коллекцию групп у вуза
                    .ThenInclude(g => g.Students)         // Подгружаем коллекцию студентов у каждой группы
                .OrderBy(u => u.Name)
                .ToList();

            foreach (var u in universitiesWithStudents)
            {
                Console.WriteLine($"\nУниверситет: {u.Name} (Адрес: {u.Address})");

                if (!u.Group.Any())
                {
                    Console.WriteLine("  (В этом университете пока нет учебных групп)");
                    continue;
                }

                foreach (var g in u.Group)
                {
                    Console.WriteLine($"  - Группа: {g.Name} | Специальность: {g.Specialty} | Год: {g.Year} (Макс. студентов: {g.MaxStudents})");

                    if (!g.Students.Any())
                    {
                        Console.WriteLine("    (В этой группе еще нет студентов)");
                        continue;
                    }

                    // Выводим список студентов, учащихся в этой группе
                    foreach (var s in g.Students)
                    {
                        Console.WriteLine($"    * Студент: {s.LastName} {s.FirstName} (Дата рождения: {s.BirthDate:dd.MM.yyyy})");
                    }
                }
            }
        }
    }
}